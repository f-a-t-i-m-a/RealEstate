using System.Collections.Generic;
using System.Net.Mime;
using System.Web.Http;
using AutoMapper;
using Compositional.Composer;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.Common.Util.Web.Result;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Enums.User;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using JahanJooy.RealEstateAgency.Domain.Property;
using JahanJooy.RealEstateAgency.Domain.Request;
using JahanJooy.RealEstateAgency.Domain.Supply;
using JahanJooy.RealEstateAgency.Domain.Users;
using JahanJooy.RealEstateAgency.Domain.Vicinities;
using JahanJooy.RealEstateAgency.Domain.Workflows;
using JahanJooy.RealEstateAgency.ShishDong.Api.Local.Base;
using JahanJooy.RealEstateAgency.Util.Attachments;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Identity;
using JahanJooy.RealEstateAgency.Util.Models.Map;
using JahanJooy.RealEstateAgency.Util.Models.Properties;
using JahanJooy.RealEstateAgency.Util.Models.Supplies;
using JahanJooy.RealEstateAgency.Util.Report;
using JahanJooy.RealEstateAgency.Util.Report.ApplicationImplemented;
using JahanJooy.RealEstateAgency.Util.UserActivityLog;
using JahanJooy.RealEstateAgency.Util.Utils;
using JahanJooy.RealEstateAgency.Util.Utils.ContactMethods;
using JahanJooy.RealEstateAgency.Util.VicinityCache;
using MongoDB.Bson;

namespace JahanJooy.RealEstateAgency.ShishDong.Api.Local.Controllers
{
    [Contract]
    [Component]
    [ComponentCache(null)]
    [RoutePrefix("files")]
    public class FileController : ExtendedApiController
    {
        [ComponentPlug]
        public LocalPhoneNumberUtil LocalPhoneNumberUtil { get; set; }

        [ComponentPlug]
        public LocalContactMethodUtil LocalContactMethodUtil { get; set; }

        #region Action methods

        [HttpPost, Route("search")]
        [SkipUserActivityLogging]
        public SearchFileOutput Search(SearchFileInput searchInput)
        {
            if (searchInput.PageSize == 0)
                searchInput.PageSize = DefaultPageSize;
            else if (searchInput.PageSize < 0 || searchInput.PageSize > MaxPageSize)
                searchInput.PageSize = MaxPageSize;

            var result = FileUtil.Search(searchInput, true, false);

            return new SearchFileOutput
            {
                Supplies = result
            };
        }

        [HttpPost, Route("searchinmap")]
        [SkipUserActivityLogging]
        public GeoSearchResult SearchInMap(SearchFileInput searchInput)
        {
            var mapResult = new GeoSearchResult();
            if (searchInput.Bounds == null)
                return mapResult;

            searchInput.StartIndex = 0;
            searchInput.PageSize = MaxPageSize;

            var boundingBox = new LatLngBounds
            {
                SouthWest = new LatLng {Lat = searchInput.Bounds.SouthLat, Lng = searchInput.Bounds.WestLng},
                NorthEast = new LatLng {Lat = searchInput.Bounds.NorthLat, Lng = searchInput.Bounds.EastLng}
            };
            var boundingBoxArea = boundingBox.GetArea();
            mapResult.LargestContainedRect = boundingBox.GetLargestContainedRect();
            mapResult.LargestContainedRectArea = mapResult.LargestContainedRect.GetArea();
            mapResult.MinimumDistinguishedArea = mapResult.LargestContainedRectArea/20;

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (boundingBoxArea == 0)
                return mapResult;

            FileUtil.SearchInMap(searchInput, mapResult, true, false);

            return mapResult;
        }

        [HttpGet, Route("get/{id}")]
        public PropertyDetails GetProperty(string id)
        {
            return PropertyUtil.GetProperty(id, true);
        }

        [HttpGet, Route("getsupply/{id}")]
        public SupplyDetails GetSupply(string id)
        {
            return SupplyUtil.GetSupply(id, true);
        }

        [Authorize]
        [ApplicationAuthorize(BuiltInRoles = new[] { BuiltInRole.VerifiedUser })]
        [HttpGet, Route("getContactInfos/{id}")]
        public PropertyContactInfoSummaryForPublic GetContactInfos(string id)
        {
            return Mapper.Map<PropertyContactInfoSummaryForPublic>(PropertyUtil.GetContactInfos(id, true));
        }

        [HttpGet, Route("getThumbnail/{id}")]
        public IHttpActionResult GetThumbnail(string id)
        {
            return new BinaryResult(PhotoGridStore.GetAttchmentBytes(AttachmentStoreEntityType.Image,
                ObjectId.Parse(id), PhotoStoreSize.Thumbnail), "image/jpeg");
        }

        [HttpGet, Route("getMediumSize/{id}")]
        public IHttpActionResult GetMedium(string id)
        {
            return new BinaryResult(PhotoGridStore.GetAttchmentBytes(AttachmentStoreEntityType.Image,
                ObjectId.Parse(id), PhotoStoreSize.MediumSize), "image/jpeg");
        }

        [HttpGet, Route("getFullSize/{id}")]
        public IHttpActionResult GetFullSize(string id)
        {
            return new BinaryResult(PhotoGridStore.GetAttchmentBytes(AttachmentStoreEntityType.Image,
                ObjectId.Parse(id), PhotoStoreSize.FullSize), "image/jpeg");
        }

        [HttpGet, Route("download/{id}")]
        public IHttpActionResult Download(string id)
        {
            return new BinaryResult(PhotoGridStore.GetAttchmentBytes(AttachmentStoreEntityType.Image,
                ObjectId.Parse(id), PhotoStoreSize.FullSize), MediaTypeNames.Application.Octet);
        }

        [HttpGet, Route("getrelatedproperties/{id}")]
        public List<PropertyDetails> GetRelatedProperties(string id)
        {
            return PropertyUtil.GetRelatedProperties(id, true);
        }
        
        [HttpPost, Route("details/{id}/print")]
        [UserActivity(UserActivityType.PrintDetail, EntityType.Property, ApplicationType.Sheshdong)]
        public IHttpActionResult Print(string id, PrintPropertyInput input)
        {
            return FileUtil.Print(id, input, true);
        }

        [HttpPost, Route("print")]
        [SkipUserActivityLogging]
        public IHttpActionResult PrintAll(PrintSuppliesInput input)
        {
            return FileUtil.PrintAll(input, true);
        }

        [Authorize]
        [HttpPost, Route("save")]
        [ApplicationAuthorize(BuiltInRoles = new[] { BuiltInRole.VerifiedUser })]
        [UserActivity(UserActivityType.Create, EntityType.Property, ApplicationType.Sheshdong)]
        public IHttpActionResult Save(NewPropertyInput input)
        {
            var property = Mapper.Map<Property>(input);
            var supply = Mapper.Map<Supply>(input);
            var request = Mapper.Map<Request>(input);


            if (input.VicinityID != ObjectId.Empty)
            {
                var vicinity = VicinityCache[input.VicinityID];
                var vicinityRefrence = Mapper.Map<VicinityReference>(vicinity);
                property.Vicinity = vicinityRefrence;
            }

            if (input.ContactInfos == null || !input.OwnerCanBeContacted.HasValue)
            {
                supply.OwnerContact = null;
                supply.AgencyContact = null;
            }
            else if (input.OwnerCanBeContacted.Value)
            {
                supply.OwnerContact = Mapper.Map<ContactMethodCollection>(input.ContactInfos);

                supply.OwnerContact?.Phones?.RemoveAll(p => string.IsNullOrEmpty(p.Value));
                supply.OwnerContact?.Emails?.RemoveAll(e => string.IsNullOrEmpty(e.Value));
                supply.OwnerContact?.Addresses?.RemoveAll(a => string.IsNullOrEmpty(a.Value));

                var contactResult = LocalContactMethodUtil.PrepareContactMethods(supply.OwnerContact, true, false, false);
                if (!contactResult.IsValid)
                    return ValidationResult(contactResult);
            }
            else
            {
                supply.AgencyContact = Mapper.Map<ContactMethodCollection>(input.ContactInfos);

                supply.AgencyContact?.Phones?.RemoveAll(p => string.IsNullOrEmpty(p.Value));
                supply.AgencyContact?.Emails?.RemoveAll(e => string.IsNullOrEmpty(e.Value));
                supply.AgencyContact?.Addresses?.RemoveAll(a => string.IsNullOrEmpty(a.Value));

                var contactResult = LocalContactMethodUtil.PrepareContactMethods(supply.AgencyContact, true, false, false);
                if (!contactResult.IsValid)
                    return ValidationResult(contactResult);
            }

            var result = FileUtil.SaveFile(property, supply, null, request, true, SourceType.Sheshdong, ApplicationType.Sheshdong);
            if (!result.IsValid)
                return ValidationResult(result);

            return Ok(property);
        }

        [Authorize]
        [HttpPost, Route("update")]
        [ApplicationAuthorize(BuiltInRoles = new[] { BuiltInRole.VerifiedUser })]
        [UserActivity(UserActivityType.Edit, EntityType.Property, ApplicationType.Sheshdong)]
        public IHttpActionResult UpdateProperty(UpdatePropertyInput input)
        {
            UserActivityLogUtils.SetMainActivity(targetId: input.ID);
            return ValidationResult(PropertyUtil.UpdateProperty(input, true));
        }

        [Authorize]
        [HttpPost, Route("delete/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] { BuiltInRole.VerifiedUser })]
        [UserActivity(UserActivityType.ChangeState, EntityType.Property, ApplicationType.Sheshdong, TargetState = PropertyState.Deleted)]
        public IHttpActionResult DeleteProperty(string id)
        {
            UserActivityLogUtils.SetMainActivity(targetId: ObjectId.Parse(id));

            return ValidationResult(PropertyUtil.DeleteProperty(id, true, ApplicationType.Sheshdong));
        }

        [Authorize]
        [HttpPost, Route("addimage")]
        [ApplicationAuthorize(BuiltInRoles = new[] { BuiltInRole.VerifiedUser })]
        [UserActivity(UserActivityType.Create, EntityType.PhotoInfo, ApplicationType.Sheshdong)]
        public IHttpActionResult AddNewImage()
        {
            return ValidationResult(PropertyUtil.AddNewImage(Request, true));
        }

        [Authorize]
        [HttpPost, Route("deleteimage/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] { BuiltInRole.VerifiedUser })]
        [UserActivity(UserActivityType.Delete, EntityType.PhotoInfo, ApplicationType.Sheshdong)]
        public IHttpActionResult DeleteImage(string id)
        {
            return ValidationResult(PropertyUtil.DeleteImage(id, true));
        }

        [Authorize]
        [HttpPost, Route("changecover/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] { BuiltInRole.VerifiedUser })]
        [UserActivity(UserActivityType.Edit, EntityType.Property,ApplicationType.Sheshdong, ActivitySubType = "ChangingCover")]
        public IHttpActionResult ChangeCover(string id)
        {
            return ValidationResult(PropertyUtil.ChangeCover(id, true));
        }

        [Authorize]
        [HttpPost, Route("updatesupply")]
        [ApplicationAuthorize(BuiltInRoles = new[] { BuiltInRole.VerifiedUser })]
        [UserActivity(UserActivityType.Edit, EntityType.Supply, ApplicationType.Sheshdong)]
        public IHttpActionResult UpdateSupply(UpdateSupplyInput input)
        {
            UserActivityLogUtils.SetMainActivity(targetId: input.ID);
            return ValidationResult(SupplyUtil.UpdateSupply(input, true));
        }

        [Authorize]
        [HttpPost, Route("deletesupply/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] { BuiltInRole.VerifiedUser })]
        [UserActivity(UserActivityType.ChangeState, EntityType.Supply, ApplicationType.Sheshdong, TargetState = SupplyState.Deleted)]
        public IHttpActionResult DeleteSupply(string id)
        {
            UserActivityLogUtils.SetMainActivity(targetId: ObjectId.Parse(id));
            return ValidationResult(SupplyUtil.DeleteSupply(id, true));
        }

        [Authorize]
        [HttpPost, Route("myfiles")]
        [ApplicationAuthorize(BuiltInRoles = new[] { BuiltInRole.VerifiedUser })]
        [SkipUserActivityLogging]
        public SearchFileOutput MyFiles(SearchFileInput searchInput)
        {
            if (searchInput.PageSize == 0)
                searchInput.PageSize = DefaultPageSize;
            else if (searchInput.PageSize < 0 || searchInput.PageSize > MaxPageSize)
                searchInput.PageSize = MaxPageSize;

            var result = FileUtil.Search(searchInput, true, true);

            return new SearchFileOutput
            {
                Supplies = result
            };
        }

        [HttpPost, Route("myfilesinmap")]
        [SkipUserActivityLogging]
        public GeoSearchResult MyFilesInMap(SearchFileInput searchInput)
        {
            var mapResult = new GeoSearchResult();
            if (searchInput.Bounds == null)
                return mapResult;

            searchInput.StartIndex = 0;
            searchInput.PageSize = MaxPageSize;

            var boundingBox = new LatLngBounds
            {
                SouthWest = new LatLng { Lat = searchInput.Bounds.SouthLat, Lng = searchInput.Bounds.WestLng },
                NorthEast = new LatLng { Lat = searchInput.Bounds.NorthLat, Lng = searchInput.Bounds.EastLng }
            };
            var boundingBoxArea = boundingBox.GetArea();
            mapResult.LargestContainedRect = boundingBox.GetLargestContainedRect();
            mapResult.LargestContainedRectArea = mapResult.LargestContainedRect.GetArea();
            mapResult.MinimumDistinguishedArea = mapResult.LargestContainedRectArea / 20;

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (boundingBoxArea == 0)
                return mapResult;

            FileUtil.SearchInMap(searchInput, mapResult, true, true);

            return mapResult;
        }

        #endregion

        #region Injected dependencies

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public SupplyUtil SupplyUtil { get; set; }

        [ComponentPlug]
        public FileUtil FileUtil { get; set; }

        [ComponentPlug]
        public PropertyUtil PropertyUtil { get; set; }

        [ComponentPlug]
        public IComposer Composer { get; set; }

        [ComponentPlug]
        public SupplyReport SupplyReport { get; set; }

        [ComponentPlug]
        public PropertyReport PropertyReport { get; set; }

        [ComponentPlug]
        public ReportRepository ReportRepository { get; set; }

        [ComponentPlug]
        public IVicinityCache VicinityCache { get; set; }

        [ComponentPlug]
        public UserUtil UserUtil { get; set; }

        [ComponentPlug]
        public PhotoGridStore PhotoGridStore { get; set; }


        private readonly int MaxPageSize = 10000;
        private readonly int DefaultPageSize = 10;

        #endregion

        #region Private helper methods 

        #endregion
    }
}