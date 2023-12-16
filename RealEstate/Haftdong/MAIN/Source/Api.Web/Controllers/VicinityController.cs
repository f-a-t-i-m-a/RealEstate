using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Spatial;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using Compositional.Composer;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstateAgency.Api.Web.Base;
using JahanJooy.RealEstateAgency.Domain.Audit;
using JahanJooy.RealEstateAgency.Domain.Enums.User;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using JahanJooy.RealEstateAgency.Domain.Enums.Vicinity;
using JahanJooy.RealEstateAgency.Domain.Property;
using JahanJooy.RealEstateAgency.Domain.Vicinities;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Enums;
using JahanJooy.RealEstateAgency.Util.Identity;
using JahanJooy.RealEstateAgency.Util.Models.Vicinities;
using JahanJooy.RealEstateAgency.Util.UserActivityLog;
using JahanJooy.RealEstateAgency.Util.Utils;
using JahanJooy.RealEstateAgency.Util.VicinityCache;
using log4net;
using Microsoft.SqlServer.Types;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;

namespace JahanJooy.RealEstateAgency.Api.Web.Controllers
{
    [Contract]
    [Component]
    [ComponentCache(null)]
    [RoutePrefix("vicinities")]
    public class VicinityController : ExtendedApiController
    {
        #region Injected dependencies

        private static readonly ILog Log = LogManager.GetLogger(typeof (VicinityController));

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public VicinityUtil VicinityUtil { get; set; }

        [ComponentPlug]
        public IVicinityCache VicinityCache { get; set; }

        private readonly int MaxPageSize = 10000;
        private readonly int DefaultPageSize = 10;

        #endregion

        #region Vicinity action methods

        [HttpGet, Route("list")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.Administrator})]
        public VicinityListOutput List()
        {
            var hierarchy = new List<VicinitySummary>();
            hierarchy.Reverse();

            var childFilter = Builders<Vicinity>.Filter.Eq("ParentID", BsonNull.Value);
            var childSort = Builders<Vicinity>.Sort.Ascending("Order").Ascending("Name");
            var childVicinities = DbManager.Vicinity.Find(childFilter).Sort(childSort)
                .Project(v => Mapper.Map<VicinitySummary>(v)).ToListAsync().Result;

            var vicinityListOutput = new VicinityListOutput
            {
                Vicinities = childVicinities,
                CurrentVicinity = null,
                CurrentVicinityFromCache = null,
                Hierarchy = hierarchy,
                AllParentsEnabled = hierarchy.All(v => v.Enabled)
            };
            return vicinityListOutput;
        }

        [HttpGet, Route("list/{parentID}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.Administrator})]
        public VicinityListOutput List(string parentID)
        {
            VicinitySummary currentVicinity = null;
            if (!string.IsNullOrEmpty(parentID) && !parentID.Equals("0"))
            {
                var filter = Builders<Vicinity>.Filter.Eq("ID", ObjectId.Parse(parentID));
                currentVicinity = DbManager.Vicinity.Find(filter)
                    .Project(v => Mapper.Map<VicinitySummary>(v)).SingleOrDefaultAsync().Result;
            }

            if (!string.IsNullOrEmpty(parentID) && !parentID.Equals("0") && currentVicinity == null)
                return new VicinityListOutput();

            var hierarchy = new List<VicinitySummary>();
            var currentHierarchyItem = currentVicinity;
            while (currentHierarchyItem != null)
            {
                hierarchy.Add(currentHierarchyItem);
                if (currentHierarchyItem.ParentID.HasValue)
                {
                    var parentFilter = Builders<Vicinity>.Filter.Eq("ID",
                        currentHierarchyItem.ParentID.Value);
                    currentHierarchyItem = DbManager.Vicinity.Find(parentFilter)
                        .Project(v => Mapper.Map<VicinitySummary>(v)).SingleOrDefaultAsync().Result;
                }
                else
                {
                    currentHierarchyItem = null;
                }
            }

            hierarchy.Reverse();

            List<VicinitySummary> childVicinities;
            if (!string.IsNullOrEmpty(parentID) && !parentID.Equals("0"))
            {
                var childFilter = Builders<Vicinity>.Filter.Eq("ParentID", ObjectId.Parse(parentID));
                var childSort = Builders<Vicinity>.Sort.Ascending("Order").Ascending("Name");
                childVicinities = DbManager.Vicinity.Find(childFilter).Sort(childSort)
                    .Project(v => Mapper.Map<VicinitySummary>(v)).ToListAsync().Result;
            }
            else
            {
                var childFilter = Builders<Vicinity>.Filter.Eq("ParentID", BsonNull.Value);
                var childSort = Builders<Vicinity>.Sort.Ascending("Order").Ascending("Name");
                childVicinities = DbManager.Vicinity.Find(childFilter).Sort(childSort)
                    .Project(v => Mapper.Map<VicinitySummary>(v)).ToListAsync().Result;
            }

            VicinityListOutput vicinityListOutput;
            if (currentVicinity != null)
                vicinityListOutput = new VicinityListOutput
                {
                    Vicinities = childVicinities,
                    CurrentVicinity = currentVicinity,
                    CurrentVicinityFromCache = Mapper.Map<VicinitySummary>(VicinityCache[currentVicinity.ID]),
                    Hierarchy = hierarchy,
                    AllParentsEnabled = hierarchy.All(v => v.Enabled)
                };
            else
            {
                vicinityListOutput = new VicinityListOutput
                {
                    Vicinities = childVicinities,
                    CurrentVicinity = null,
                    CurrentVicinityFromCache = null,
                    Hierarchy = hierarchy,
                    AllParentsEnabled = hierarchy.All(v => v.Enabled)
                };
            }
            return vicinityListOutput;
        }

        [HttpPost, Route("save")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.Administrator})]
        [UserActivity(UserActivityType.Create, EntityType.Vicinity, ApplicationType.Haftdong)]
        public IHttpActionResult SaveVicinity(NewVicinityInput input)
        {
            var vicinity = new Vicinity
            {
                Name = input.Name,
                Type = input.Type ?? GetDefaultChildType(input.Type),
                ShowTypeInTitle = input.ShowTypeInTitle,
                AlternativeNames = input.AlternativeNames,
                AdditionalSearchText = input.AdditionalSearchText,
                WellKnownScope = input.WellKnownScope ?? VicinityType.HierarchyNode,
                ShowInSummary = input.ShowInSummary,
                CanContainPropertyRecords = input.CanContainPropertyRecords,
                ParentID = input.CurrentVicinityID,
                Enabled = true
            };

            DbManager.Vicinity.InsertOneAsync(vicinity);
            VicinityCache.InvalidateAll();
            if (input.CurrentVicinityID != null)
                UserActivityLogUtils.SetMainActivity(targetId: vicinity.ID, parentType: EntityType.Vicinity,
                    parentId: input.CurrentVicinityID);
            else
                UserActivityLogUtils.SetMainActivity(targetId: vicinity.ID);

            return ValidationResult(Common.Util.Validation.ValidationResult.Success);
        }

        [HttpGet, Route("get/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.Administrator})]
        public VicinitySummary Get(string id)
        {
            var filter = Builders<Vicinity>.Filter.Eq("ID", ObjectId.Parse(id));
            var vicinity = DbManager.Vicinity.Find(filter)
                .Project(v => Mapper.Map<VicinitySummary>(v)).SingleOrDefaultAsync().Result;

            return vicinity;
        }

        [HttpGet, Route("retrieve")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.Administrator})]
        public IHttpActionResult Retrieve()
        {
            SqlConnection conn;
            SqlCommand cmd;
            GetSqlConnection(out conn, out cmd);
            conn.Open();
            using (conn)
            {
                using (var reader = cmd.ExecuteReader())
                {
                    var vicinities = new List<Vicinity>();
                    while (reader.Read())
                    {
                        var centerPoint = (DBNull.Value != reader[12]) ? (SqlGeography) reader[12] : null;
                        GeoJson2DCoordinates point = null;
                        if (centerPoint?.Long != null && centerPoint?.Lat != null)
                            point = new GeoJson2DCoordinates((double) centerPoint.Long, (double) centerPoint.Lat);

                        GeoJsonPolygon<GeoJson2DCoordinates> polygon = null;
                        double? boundaryArea = 0;
                        if (DBNull.Value != reader[13])
                        {
                            try
                            {
                                var sqlGeography = ((SqlGeography) reader[13]);
                                var dbGeography = DbGeography.FromText(sqlGeography.ToString());
                                boundaryArea = dbGeography.Area;
                                var coordinates = new List<GeoJson2DCoordinates>();
                                var type = dbGeography.SpatialTypeName;
                                switch (type)
                                {
                                    case "Polygon":
                                    {
                                        for (var i = 1; i <= dbGeography.PointCount.Value; i++)
                                        {
                                            var lat = (double) dbGeography.PointAt(i).Latitude;
                                            var lng = (double) dbGeography.PointAt(i).Longitude;
                                            var polygonPoint = new GeoJson2DCoordinates(lng, lat);
                                            coordinates.Add(polygonPoint);
                                        }

                                        var linearRingCoordinates =
                                            new GeoJsonLinearRingCoordinates<GeoJson2DCoordinates>(coordinates);
                                        var polygonCoordinates =
                                            new GeoJsonPolygonCoordinates<GeoJson2DCoordinates>(linearRingCoordinates);
                                        polygon = new GeoJsonPolygon<GeoJson2DCoordinates>(polygonCoordinates);
                                        break;
                                    }
                                    case "MULTIPOLYGON":
                                    {
                                        break;
                                    }
                                    case "Geometry":
                                    {
                                        break;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Log.ErrorFormat(
                                    "Unhandled exception reading boundary from Vicinity by ID {0}, Exception: {1}",
                                    reader[0], e);
                            }
                        }

                        try
                        {
                            vicinities.Add(new Vicinity
                            {
                                SqlID = (DBNull.Value != reader[0]) ? (long) reader[0] : 0,
                                Name = (DBNull.Value != reader[1]) ? (string) reader[1] : "",
                                AlternativeNames = (DBNull.Value != reader[2]) ? (string) reader[2] : "",
                                AdditionalSearchText = (DBNull.Value != reader[3]) ? (string) reader[3] : "",
                                Description = (DBNull.Value != reader[4]) ? (string) reader[4] : "",
                                AdministrativeNotes = (DBNull.Value != reader[5]) ? (string) reader[5] : "",
                                Order = (DBNull.Value != reader[6]) ? (int) reader[6] : 0,
                                Type = (VicinityType) ((DBNull.Value != reader[7]) ? reader[7] : 0),
                                WellKnownScope = (VicinityType) ((DBNull.Value != reader[8]) ? reader[8] : 0),
                                ShowTypeInTitle = (bool) reader[9],
                                ShowInSummary = (bool) reader[10],
                                CanContainPropertyRecords = (bool) reader[11],
                                CenterPoint = point,
                                Boundary = polygon,
                                BoundaryArea = boundaryArea,
                                SqlParentID = (DBNull.Value != reader[14]) ? (long?) reader[14] : null,
                                Enabled = (bool) reader[15],
                                OfficialLinkUrl = (DBNull.Value != reader[16]) ? (string) reader[16] : "",
                                WikiLinkUrl = (DBNull.Value != reader[17]) ? (string) reader[17] : "",
                                ShowInHierarchy = (bool) reader[18]
                            });
                        }
                        catch (Exception e)
                        {
                            Log.ErrorFormat("Unhandled exception adding Vicinity by ID {0}, Exception: {1}", reader[0],
                                e);
                        }
                    }

                    var index = 0;
                    while (vicinities.Count > 0)
                    {
                        var result = AddVicinity(vicinities[index]);
                        if (result)
                            vicinities.Remove(vicinities[index]);
                        else
                            index++;

                        if (index == vicinities.Count)
                            index = 0;
                    }

                    VicinityCache.InvalidateAll();
                    reader.Close();
                    conn.Close();
                }
            }
            return ValidationResult(Common.Util.Validation.ValidationResult.Success);
        }

        [HttpPost, Route("update")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.Administrator})]
        [UserActivity(UserActivityType.Edit, EntityType.Vicinity, ApplicationType.Haftdong)]
        public IHttpActionResult Update(UpdateVicinityInput input)
        {
            UserActivityLogUtils.SetMainActivity(targetId: input.ID);
            var validationErrors = ValidateForSave(input);
            if (!validationErrors.IsValid)
                return ValidationResult(validationErrors);

            var filter = Builders<Vicinity>.Filter.Eq("ID", input.ID);
            var update = Builders<Vicinity>.Update
                .Set("Name", input.Name)
                .Set("AlternativeNames", input.AlternativeNames)
                .Set("AdditionalSearchText", input.AdditionalSearchText)
                .Set("Description", input.Description)
                .Set("OfficialLinkUrl", input.OfficialLinkUrl)
                .Set("WikiLinkUrl", input.WikiLinkUrl)
                .Set("AdministrativeNotes", input.AdministrativeNotes)
                .Set("Enabled", input.Enabled)
                .Set("Order", input.Order)
                .Set("Type", input.Type)
                .Set("WellKnownScope", input.WellKnownScope)
                .Set("ShowTypeInTitle", input.ShowTypeInTitle)
                .Set("ShowInHierarchy", input.ShowInHierarchy)
                .Set("ShowInSummary", input.ShowInSummary)
                .Set("CanContainPropertyRecords", input.CanContainPropertyRecords);

            var result = DbManager.Vicinity.UpdateOneAsync(filter, update);
            if (result.Result.MatchedCount != 1)
                return ValidationResult("Vicinity", VicinityValidationErrors.NotFound);

            if (result.Result.MatchedCount == 1 && result.Result.ModifiedCount != 1)
                return ValidationResult("Vicinity", VicinityValidationErrors.NotModified);

            VicinityCache.InvalidateAll();
            return ValidationResult(Common.Util.Validation.ValidationResult.Success);
        }

        [HttpPost, Route("search")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [SkipUserActivityLogging]
        public SearchVicinityOutput Search(SearchVicinityInput searchInput)
        {
            if (searchInput.PageSize == 0)
                searchInput.PageSize = DefaultPageSize;
            else if (searchInput.PageSize < 0 || searchInput.PageSize > MaxPageSize)
                searchInput.PageSize = MaxPageSize;

        return VicinityUtil.Search(searchInput);
        }

        [HttpPost, Route("findbypoint")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [SkipUserActivityLogging]
        public VicinityReference FindByPoint(SearchVicinityByPointInput input)
        {
            return VicinityUtil.FindByPoint(input);
        }

        [HttpPost, Route("enable")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.Administrator})]
        [UserActivity(UserActivityType.Edit, EntityType.Vicinity, ApplicationType.Haftdong)]
        public IHttpActionResult EnabledOrDisableVicinities(ListOfVicinityInput input)
        {
            UserActivityLogUtils.SetMainActivity();

            if (input.Ids == null)
                return ValidationResult(Common.Util.Validation.ValidationResult.Failure("Vicinity",
                    VicinityValidationErrors.UnexpectedError));

            var ids = input.Ids.Select(ObjectId.Parse).ToList();
            var filter = Builders<Vicinity>.Filter.In("ID", ids)
                         & Builders<Vicinity>.Filter.Ne("Enabled", input.Value);
            var update = Builders<Vicinity>.Update
                .Set("Enabled", input.Value);
            var result = DbManager.Vicinity.UpdateManyAsync(filter, update);

            if (result.Result.MatchedCount != result.Result.ModifiedCount)
            {
                filter = Builders<Vicinity>.Filter.In("ID", ids)
                         & Builders<Vicinity>.Filter.Ne("Enabled", input.Value);
                var vicinities = DbManager.Vicinity.Find(filter).ToListAsync().Result;
                vicinities.Select(v => v.ID).ToList().ForEach(i => ids.Remove(i));
                vicinities.ForEach(v =>
                {
                    Log.WarnFormat(
                        "Ability of Vicinity with id {0} could not set to {1} in enabling/disabling process",
                        v.ID, input.Value);
                });
            }
            ids.ForEach(i =>
            {
                UserActivityLogUtils.ReportAdditionalActivity(new UserActivity
                {
                    ApplicationType = ApplicationType.Haftdong,
                    TargetType = EntityType.Vicinity,
                    TargetID = i,
                    ActivityType = input.Value ? UserActivityType.Enable : UserActivityType.Disable
                });
            });

            VicinityCache.InvalidateAll();
            return ValidationResult(Common.Util.Validation.ValidationResult.Success);
        }

        [HttpPost, Route("delete")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.Administrator})]
        [UserActivity(UserActivityType.Delete, EntityType.Vicinity, ApplicationType.Haftdong)]
        public IHttpActionResult Delete(ListOfVicinityInput input)
        {
            UserActivityLogUtils.SetMainActivity();

            if (input.Ids == null)
                return ValidationResult(Common.Util.Validation.ValidationResult.Failure("Vicinity",
                    VicinityValidationErrors.UnexpectedError));

            var ids = input.Ids.Select(ObjectId.Parse).ToList();
            var parentFilter = Builders<Vicinity>.Filter.In("ParentID", ids);
            var parentsCount = DbManager.Vicinity.CountAsync(parentFilter).Result;

            if (parentsCount > 0)
                return
                    ValidationResult(Common.Util.Validation.ValidationResult.Failure("Vicinity",
                        VicinityValidationErrors.HasChild));

            var filter = Builders<Vicinity>.Filter.In("ID", ids);
            var result = DbManager.Vicinity.DeleteManyAsync(filter);
            if (result.Result.DeletedCount != ids.Count)
            {
                filter = Builders<Vicinity>.Filter.In("ID", ids);
                var vicinities = DbManager.Vicinity.Find(filter).ToListAsync().Result;
                vicinities.Select(v => v.ID).ToList().ForEach(i => ids.Remove(i));
                vicinities.ForEach(v =>
                {
                    Log.WarnFormat(
                        "Vicinity with id {0} could not been deleted in deleting process", v.ID);
                });
            }
            ids.ForEach(
                i =>
                {
                    UserActivityLogUtils.ReportAdditionalActivity(new UserActivity
                    {
                        ApplicationType = ApplicationType.Haftdong,
                        TargetType = EntityType.Vicinity,
                        TargetID = i,
                        ActivityType = UserActivityType.Delete
                    });
                });
            VicinityCache.InvalidateAll();
            return ValidationResult(Common.Util.Validation.ValidationResult.Success);
        }

        [HttpPost, Route("move")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.Administrator})]
        [UserActivity(UserActivityType.Edit, EntityType.Vicinity, ApplicationType.Haftdong)]
        public IHttpActionResult MoveVicinities(MoveVicinitiesInput input)
        {
            UserActivityLogUtils.SetMainActivity();

            if (input.Ids == null)
                return ValidationResult(Common.Util.Validation.ValidationResult.Failure("Vicinity",
                    VicinityValidationErrors.UnexpectedError));

            var parentId = input.ParentId;
            while (parentId != null)
            {
                var parentFilter = Builders<Vicinity>.Filter.Eq("ID", ObjectId.Parse(parentId));
                var parentVicinity = DbManager.Vicinity.Find(parentFilter).SingleOrDefaultAsync();

                if (parentVicinity?.Result == null)
                    return ValidationResult(Common.Util.Validation.ValidationResult.Failure("Vicinity",
                        VicinityValidationErrors.NotFound));

                if (input.Ids.Any(vid => vid == parentId))
                    throw new ArgumentException("One of the selected vicinities is the parent of target destination");

                parentId = parentVicinity.Result.ParentID?.ToString();
            }

            var ids = input.Ids.Select(ObjectId.Parse).ToList();
            var filter = Builders<Vicinity>.Filter.In("ID", ids);
            var update = Builders<Vicinity>.Update
                .Set("ParentID", ObjectId.Parse(input.ParentId));
            var result = DbManager.Vicinity.UpdateManyAsync(filter, update);

            if (result.Result.MatchedCount != result.Result.ModifiedCount)
            {
                filter = Builders<Vicinity>.Filter.In("ID", ids)
                         & Builders<Vicinity>.Filter.Ne("ParentID", ObjectId.Parse(input.ParentId));
                var vicinities = DbManager.Vicinity.Find(filter).ToListAsync().Result;
                vicinities.Select(v => v.ID).ToList().ForEach(i => ids.Remove(i));
                vicinities.ForEach(v =>
                {
                    Log.WarnFormat(
                        "Vicinity with id {0} could not move under parent with id {1} in moving process",
                        v.ID, input.ParentId);
                });
            }

            ids.ForEach(
                i =>
                {
                    UserActivityLogUtils.ReportAdditionalActivity(new UserActivity
                    {
                        ApplicationType = ApplicationType.Haftdong,
                        TargetType = EntityType.Vicinity,
                        TargetID = i,
                        ActivityType = UserActivityType.Edit,
                        ActivitySubType = "InMoving"
                    });
                });

            VicinityCache.InvalidateAll();
            return ValidationResult(Common.Util.Validation.ValidationResult.Success);
        }

        [HttpPost, Route("containproperty")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.Administrator})]
        [UserActivity(UserActivityType.Edit, EntityType.Vicinity, ApplicationType.Haftdong)]
        public IHttpActionResult SetContainProperty(ListOfVicinityInput input)
        {
            UserActivityLogUtils.SetMainActivity();

            if (input.Ids == null)
                return ValidationResult(Common.Util.Validation.ValidationResult.Failure("Vicinity",
                    VicinityValidationErrors.UnexpectedError));

            var ids = input.Ids.Select(ObjectId.Parse).ToList();
            var filter = Builders<Vicinity>.Filter.In("ID", ids);
            var update = Builders<Vicinity>.Update
                .Set("CanContainPropertyRecords", input.Value);
            var result = DbManager.Vicinity.UpdateManyAsync(filter, update);

            if (result.Result.MatchedCount != result.Result.ModifiedCount)
            {
                filter = Builders<Vicinity>.Filter.In("ID", ids)
                         & Builders<Vicinity>.Filter.Ne("CanContainPropertyRecords", input.Value);
                var vicinities = DbManager.Vicinity.Find(filter).ToListAsync().Result;
                vicinities.Select(v => v.ID).ToList().ForEach(i => ids.Remove(i));
                vicinities.ForEach(v =>
                {
                    Log.WarnFormat(
                        "CanContainPropertyRecords property of Vicinity with id {0} could not set to {1} in editing process",
                        v.ID, input.Value);
                });
            }

            ids.ForEach(
                i =>
                {
                    UserActivityLogUtils.ReportAdditionalActivity(new UserActivity
                    {
                        ApplicationType = ApplicationType.Haftdong,
                        TargetType = EntityType.Vicinity,
                        TargetID = i,
                        ActivityType = UserActivityType.Edit,
                        ActivitySubType = "InContainingProperty"
                    });
                });

            VicinityCache.InvalidateAll();
            return ValidationResult(Common.Util.Validation.ValidationResult.Success);
        }

        #endregion

        #region Private methods

        private ValidationResult ValidateForSave(UpdateVicinityInput vicinity)
        {
            var errors = new List<ValidationError>();
            if (string.IsNullOrWhiteSpace(vicinity.Name))
            {
                errors.Add(new ValidationError("Vicinity.Name", GeneralValidationErrors.ValueNotSpecified));
            }
            if (vicinity.Type == 0)
            {
                errors.Add(new ValidationError("Vicinity.Type", GeneralValidationErrors.ValueNotSpecified));
            }

            return new ValidationResult {Errors = errors};
        }

        private VicinityType GetDefaultChildType(VicinityType? value)
        {
            if (!value.HasValue)
                return VicinityType.Country;

            switch (value.Value)
            {
                case VicinityType.Country:
                case VicinityType.CountryPartition:
                    return VicinityType.Province;

                case VicinityType.State:
                case VicinityType.StatePartition:
                case VicinityType.Province:
                case VicinityType.ProvincePartition:
                    return VicinityType.County;

                case VicinityType.County:
                case VicinityType.District:
                case VicinityType.SubDistrict:
                    return VicinityType.City;

                case VicinityType.Metropolis:
                case VicinityType.MetropolisPartition:
                case VicinityType.City:
                case VicinityType.CityPartition:
                    return VicinityType.CityRegion;

                case VicinityType.Suburb:
                case VicinityType.Village:
                case VicinityType.Phase:
                case VicinityType.Town:
                case VicinityType.CityRegion:
                    return VicinityType.Neighborhood;

                case VicinityType.Campus:
                case VicinityType.Premises:
                    return VicinityType.Building;

                case VicinityType.Complex:
                    return VicinityType.Block;

                default:
                    return VicinityType.HierarchyNode;
            }
        }

        private void GetSqlConnection(out SqlConnection conn, out SqlCommand cmd)
        {
            var ConnectionStringName = "Db";
            var connectionStringSettings = ConfigurationManager.ConnectionStrings[ConnectionStringName];
            if (connectionStringSettings == null)
                throw new ConfigurationErrorsException(
                    "MongoDB Connection String for attachments is not configured. A connection string named '" +
                    ConnectionStringName + "' should be present in the application configuration.");

            var connectionString = connectionStringSettings.ConnectionString;
            if (connectionString == null)
                throw new ConfigurationErrorsException(
                    "MongoDB Connection String for attachments (named '" + ConnectionStringName + "') is empty. ");

            conn = new SqlConnection(connectionString);
            var query = "select *, Vicinity.CenterPoint.Lat, Vicinity.CenterPoint.Long from Vicinity";
            cmd = new SqlCommand(query, conn);
        }

        private bool AddVicinity(Vicinity vicinity)
        {
            var vicinityFilter = Builders<Vicinity>.Filter.Eq("SqlID", vicinity.SqlID);
            var existingVicinity = DbManager.Vicinity.Find(vicinityFilter).SingleOrDefaultAsync().Result;
            if (existingVicinity != null) //Existing item
            {
                if (vicinity.SqlParentID != null)
                {
                    var parentFilter = Builders<Vicinity>.Filter.Eq("SqlID", vicinity.SqlParentID);
                    var parent = DbManager.Vicinity.Find(parentFilter).SingleOrDefaultAsync();
                    if (parent?.Result != null)
                        vicinity.ParentID = parent.Result.ID;
                    else
                        return false;
                }

                var update = Builders<Vicinity>.Update
                    .Set("Name", vicinity.Name)
                    .Set("AlternativeNames", vicinity.AlternativeNames)
                    .Set("AdditionalSearchText", vicinity.AdditionalSearchText)
                    .Set("Description", vicinity.Description)
                    .Set("AdministrativeNotes", vicinity.AdministrativeNotes)
                    .Set("Order", vicinity.Order)
                    .Set("Type", vicinity.Type)
                    .Set("WellKnownScope", vicinity.WellKnownScope)
                    .Set("ShowTypeInTitle", vicinity.ShowTypeInTitle)
                    .Set("ShowInSummary", vicinity.ShowInSummary)
                    .Set("CanContainPropertyRecords", vicinity.CanContainPropertyRecords)
                    .Set("ParentID", vicinity.ParentID)
                    .Set("SqlParentID", vicinity.SqlParentID)
                    .Set("Enabled", vicinity.Enabled)
                    .Set("OfficialLinkUrl", vicinity.OfficialLinkUrl)
                    .Set("CenterPoint", vicinity.CenterPoint)
                    .Set("Boundary", vicinity.Boundary)
                    .Set("BoundaryArea", vicinity.BoundaryArea)
                    .Set("WikiLinkUrl", vicinity.WikiLinkUrl)
                    .Set("ShowInHierarchy", vicinity.ShowInHierarchy);

                DbManager.Vicinity.UpdateOneAsync(vicinityFilter, update);

                //Update all vicinity references
                var vicinityReference = Mapper.Map<VicinityReference>(vicinity);
                var propertyFilter = Builders<Property>.Filter.Eq("Vicinity.ID", existingVicinity.ID);
                var propertyUpdate = Builders<Property>.Update.Set("Vicinity", vicinityReference);
                var result = DbManager.Property.UpdateManyAsync(propertyFilter, propertyUpdate).Result;
                if (result.MatchedCount != result.ModifiedCount)
                    Log.WarnFormat(
                        "Vicinity reference with id {0} in related property(ies) could not been updated in updating vicinity process",
                        existingVicinity.ID);

                return true;
            }

            //New One
            if (vicinity.SqlParentID != null)
            {
                var parentFilter = Builders<Vicinity>.Filter.Eq("SqlID", vicinity.SqlParentID);
                var parent = DbManager.Vicinity.Find(parentFilter).SingleOrDefaultAsync();
                if (parent?.Result != null)
                    vicinity.ParentID = parent.Result.ID;
                else
                    return false;
            }

            DbManager.Vicinity.InsertOneAsync(vicinity);
            return true;
        }

        #endregion
    }
}