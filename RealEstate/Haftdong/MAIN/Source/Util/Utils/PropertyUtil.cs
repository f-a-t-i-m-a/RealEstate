using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using AutoMapper;
using Compositional.Composer;
using ImageResizer;
using JahanJooy.Common.Util.Components;
using JahanJooy.Common.Util.Configuration;
using JahanJooy.Common.Util.Identity;
using JahanJooy.Common.Util.Owin;
using JahanJooy.Common.Util.Streams;
using JahanJooy.Common.Util.Validation;
using JahanJooy.Common.Util.Web.Multipart;
using JahanJooy.RealEstateAgency.Domain.Audit;
using JahanJooy.RealEstateAgency.Domain.Base;
using JahanJooy.RealEstateAgency.Domain.Contract;
using JahanJooy.RealEstateAgency.Domain.Customers;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using JahanJooy.RealEstateAgency.Domain.MasterData;
using JahanJooy.RealEstateAgency.Domain.Property;
using JahanJooy.RealEstateAgency.Domain.Request;
using JahanJooy.RealEstateAgency.Domain.Supply;
using JahanJooy.RealEstateAgency.Domain.Users;
using JahanJooy.RealEstateAgency.Domain.Vicinities;
using JahanJooy.RealEstateAgency.Domain.Workflows;
using JahanJooy.RealEstateAgency.Util.Attachments;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Enums;
using JahanJooy.RealEstateAgency.Util.Log4Net;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using JahanJooy.RealEstateAgency.Util.Models.Customers;
using JahanJooy.RealEstateAgency.Util.Models.Properties;
using JahanJooy.RealEstateAgency.Util.Owin;
using JahanJooy.RealEstateAgency.Util.UserActivityLog;
using JahanJooy.RealEstateAgency.Util.Utils.ContactMethods;
using JahanJooy.RealEstateAgency.Util.VicinityCache;
using log4net;
using Microsoft.SqlServer.Types;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using ServiceStack;

// ReSharper disable ConditionIsAlwaysTrueOrFalse

namespace JahanJooy.RealEstateAgency.Util.Utils
{
    [Contract]
    [Component]
    public class PropertyUtil
    {
        #region Injected dependencies

        private static readonly ILog Log = LogManager.GetLogger(typeof (PropertyUtil));
        public const string KhoonatPropertyLastFetchTime = "KhoonatPropertyLastFetchTime";
        public const int NumberOfItems = 50;

        private const string UntouchedStoreId = "propertyPhoto-Untouched";

        private static bool _initialized;

        #region Image resource constants

        private const string ResourceKeyImageCouldNotBeRetrieved =
            "JahanJooy.RealEstateAgency.Util.Content.ErrorImages.imageCouldNotBeRetrieved.png";

        #endregion

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public SupplyUtil SupplyUtil { get; set; }

        [ComponentPlug]
        public RequestUtil RequestUtil { get; set; }

        [ComponentPlug]
        public CustomerUtil CustomerUtil { get; set; }

        [ComponentPlug]
        public IVicinityCache VicinityCache { get; set; }

        [ComponentPlug]
        public VicinityUtil VicinityUtil { get; set; }

        [ComponentPlug]
        public IApplicationSettings ApplicationSettings { get; set; }

        [ComponentPlug]
        public BinaryStorageComponent BinaryStorage { get; set; }

        [ComponentPlug]
        public PhotoGridStore PhotoGridStore { get; set; }

        [ComponentPlug]
        public LocalPhoneNumberUtil LocalPhoneNumberUtil { get; set; }

        [ComponentPlug]
        public LocalContactMethodUtil LocalContactMethodUtil { get; set; }

        static PropertyUtil()
        {
            ApplicationSettingKeys.RegisterKey(KhoonatPropertyLastFetchTime);
            BinaryStorageComponent.RegisterStoreConfigurationKeys(UntouchedStoreId);
        }

        #endregion

        #region Action methods

        public ValidationResult SaveProperty(Property property, Supply supply, Request request,
            bool isPublic, SourceType sourceType, ApplicationType applicationType)
        {
            PreparePropertyForSave(property, sourceType);

            if (property.Owner != null && property.Owner.ID != ObjectId.Empty)
            {
                var filter = Builders<Customer>.Filter.Eq("ID", property.Owner.ID);
                var customer = DbManager.Customer.Find(filter).SingleOrDefaultAsync();

                if (customer?.Result != null)
                {
                    var customerReference = Mapper.Map<CustomerReference>(customer.Result);
                    property.Owner = customerReference;
                }
            }

            if (property.GeographicLocation != null)
            {
                property.GeographicLocationType = GeographicLocationSpecificationType.UserSpecifiedExact;
            }
            else if (property.Vicinity != null && property.Vicinity.ID != ObjectId.Empty
                     && property.Vicinity.CenterPoint != null)
            {
                property.GeographicLocation = property.Vicinity.CenterPoint;
                property.GeographicLocationType = GeographicLocationSpecificationType.InferredFromVicinity;
            }

            try
            {
                DbManager.Property.InsertOneAsync(property).Wait();

                if (property.Owner != null && property.Owner.ID != ObjectId.Empty)
                {
                    var filter = Builders<Customer>.Filter.Eq("ID", property.Owner.ID);
                    var update =
                        Builders<Customer>.Update.Inc("PropertyCount", +1)
                            .Set("LastIndexingTime", BsonNull.Value)
                            .Set("LastVisitTime", DateTime.Now);
                    var result = DbManager.Customer.UpdateOneAsync(filter, update);
                    if (result.Result.MatchedCount != 1 ||
                        (result.Result.MatchedCount == 1 && result.Result.ModifiedCount != 1))
                    {
                        Log.WarnFormat(
                            "Property count in customer with id {0} could not been increased in adding property process",
                            property.Owner.ID);
                    }
                }

                UserActivityLogUtils.ReportAdditionalActivity(new UserActivity
                {
                    ApplicationType = applicationType,
                    TargetType = EntityType.Supply,
                    TargetID = supply.ID,
                    ActivityType = UserActivityType.Create,
                    ParentType = EntityType.Property,
                    ParentID = property.ID
                });

                var supplyResult = SupplyUtil.SaveSupply(supply, property.ID, isPublic);
                if (!supplyResult.IsValid)
                {
                    return supplyResult;
                }

                var supplyReference = Mapper.Map<SupplyReference>(supply);

                if (supply.IntentionOfOwner == IntentionOfOwner.ForSwap && request != null && supply.SwapAdditionalComments.IsNullOrEmpty())
                {
                    request.Supply = supplyReference;
                    var requestResult = RequestUtil.SaveRequest(request, Mapper.Map<Customer>(property.Owner), isPublic, sourceType, supply.ID);
                    if (!requestResult.IsValid)
                    {
                        return requestResult;
                    }
                }

                property.Supplies.Add(supplyReference);
                return ValidationResult.Success;
            }
            catch (Exception e)
            {
                Log.Error("An exception has been occured in saving Property", e);
                return ValidationResult.Failure("Property",
                    PropertyValidationErrors.UnexpectedError);
            }
        }

        public PropertyContactInfoSummary GetContactInfos(string propertyId, bool publicOnly)
        {
            var contactInfos = new PropertyContactInfoSummary
            {
                Owner = null,
                ContactInfos = new List<SupplyContactInfoSummary>()
            };

            if (!publicOnly || OwinRequestScopeContext.Current.GetUserId() != null)
            {
                var filter = Builders<Property>.Filter.Eq("ID", ObjectId.Parse(propertyId));
                var property = DbManager.Property.Find(filter).SingleOrDefaultAsync();

                if (property?.Result?.Supplies == null ||
                    (publicOnly && property.Result.Supplies.TrueForAll(s => !s.IsPublic)))
                {
                    return contactInfos;
                }

                contactInfos.Owner = Mapper.Map<CustomerSummary>(property.Result.Owner);

                property.Result.Supplies.ForEach(s =>
                {
                    var supplyFilter = Builders<Supply>.Filter.Eq("ID", s.ID);
                    var supply = DbManager.Supply.Find(supplyFilter).SingleOrDefaultAsync();

                    if (supply?.Result != null)
                    {
                        var contactSummary = Mapper.Map<SupplyContactInfoSummary>(supply.Result);
                        if (!contactSummary.OwnerCanBeContacted)
                        {
                            contactSummary.OwnerContact = null;
                        }
                        contactInfos.ContactInfos.Add(contactSummary);
                    }
                });
            }

            return contactInfos;
        }

        public PropertyDetails GetProperty(string id, bool publicOnly)
        {
            var filter = Builders<Property>.Filter.Eq("ID", ObjectId.Parse(id));

            var property =
                    DbManager.Property.Find(filter)
                        .SingleOrDefaultAsync()
                        .Result;

            if (property == null)
            {
                return null;
            }

            if (!AuthorizeForView(property, publicOnly))
            {
                return new PropertyDetails();
            }

            if (property.Supplies != null && publicOnly)
            {
                property.Supplies = property.Supplies.Where(s => s.IsPublic).ToList();
            }

            var propertyDetail = Mapper.Map<PropertyDetails>(property);
            if (propertyDetail.Vicinity != null)
            {
                propertyDetail.Vicinity.CompleteName = VicinityUtil.GetFullName(propertyDetail.Vicinity.ID);
            }
            return propertyDetail;
        }

        public List<PropertyDetails> GetRelatedProperties(string id, bool publicOnly)
        {
            var result = new List<PropertyDetails>();
            var filter = Builders<Property>.Filter.Eq("ID", ObjectId.Parse(id));
            var property =
                DbManager.Property.Find(filter)
                    .SingleOrDefaultAsync()
                    .Result;
            if (property != null && property.CorrelationID != Guid.Empty)
            {
                var builder = Builders<Property>.Filter;
                var relatedFilter = builder.Eq("CorrelationID", property.CorrelationID) &
                                    !builder.Eq("ID", ObjectId.Parse(id));
                var relatedProperties =
                    DbManager.Property.Find(relatedFilter)
                        .ToListAsync()
                        .Result;

                relatedProperties?.ForEach(p =>
                {
                    if (publicOnly)
                    {
                        if (p?.Supplies != null && p.Supplies.Any(s => s.IsPublic))
                            result.Add(Mapper.Map<PropertyDetails>(p));
                    }
                    else
                    {
                        if (AuthorizeForView(p, publicOnly))
                            result.Add(Mapper.Map<PropertyDetails>(p));
                    }
                });
            }

            return result;
        }

        public ValidationResult UpdateProperty(UpdatePropertyInput input, bool publicOnly)
        {
            var propertyFilter = Builders<Property>.Filter.Eq("ID", input.ID);
            var property = DbManager.Property.Find(propertyFilter).SingleOrDefaultAsync().Result;

            if (!AuthorizeForEdit(property, publicOnly))
            {
                return ValidationResult.Failure("Property", GeneralValidationErrors.AccessDenied);
            }

            VicinityReference vicinityReference = null;
            if (input.Vicinity != null && input.Vicinity.ID != ObjectId.Empty)
            {
                var vicinity = VicinityCache[input.Vicinity.ID];
                if (vicinity != null)
                {
                    vicinityReference = Mapper.Map<VicinityReference>(vicinity);
                }
            }

            var ownerReference = new CustomerReference();
            if (property.SourceType == SourceType.Haftdong)
            {
                var owner = Mapper.Map<Customer>(input.Owner);
                var ownerContact = new ContactMethodCollection
                {
                    Emails = new List<EmailInfo>(),
                    Phones = new List<PhoneInfo>(),
                    Addresses = new List<AddressInfo>(),
                    ContactName = input.Owner.DisplayName
                };

                input.Owner?.Contact?.Phones?.ForEach(p => LocalContactMethodUtil.MapAndAddContactMethod(ownerContact, p));
                var contactResult = LocalContactMethodUtil.PrepareContactMethods(ownerContact, true, false, false);
                if (!contactResult.IsValid)
                    return contactResult;

                if (ownerContact.Phones == null || ownerContact.Phones.Count == 0)
                {
                    return ValidationResult.Failure("Customer.Phones", GeneralValidationErrors.ValueNotSpecified);
                }

                owner.Contact = ownerContact;
                var ownerResult = CustomerUtil.GetCustomerByCustomerDetails(owner);
                if (!ownerResult.IsValid)
                {
                    return ownerResult;
                }
                ownerReference = ownerResult.Result;
            }

            var updateProperty = Builders<Property>.Update
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("LastModificationTime", DateTime.Now)
                .Set("PropertyType", input.PropertyType)
                .Set("UsageType", input.UsageType)
                .Set("Address", input.Address)
                .Set("Vicinity", vicinityReference)
                .Set("GeographicLocation", Mapper.Map<GeoJson2DCoordinates>(input.GeographicLocation))
                .Set("IsHidden", input.IsHidden)
                .Set("ConversionWarning", input.ConversionWarning)
                .Set("EstateArea", input.EstateArea)
                .Set("EstateDirection", input.EstateDirection)
                .Set("PassageEdgeLength", input.PassageEdgeLength)
                .Set("EstateVoucherType", input.EstateVoucherType)
                .Set("TotalNumberOfUnits", input.TotalNumberOfUnits)
                .Set("LicencePlate", input.LicencePlate)
                .Set("BuildingAgeYears", input.BuildingAgeYears)
                .Set("NumberOfUnitsPerFloor", input.NumberOfUnitsPerFloor)
                .Set("TotalNumberOfFloors", input.TotalNumberOfFloors)
                .Set("UnitArea", input.UnitArea)
                .Set("OfficeArea", input.OfficeArea)
                .Set("CeilingHeight", input.CeilingHeight)
                .Set("StorageRoomArea", input.StorageRoomArea)
                .Set("NumberOfRooms", input.NumberOfRooms)
                .Set("NumberOfParkings", input.NumberOfParkings)
                .Set("UnitFloorNumber", input.UnitFloorNumber)
                .Set("AdditionalSpecialFeatures", input.AdditionalSpecialFeatures)
                .Set("NumberOfMasterBedrooms", input.NumberOfMasterBedrooms)
                .Set("KitchenCabinetType", input.KitchenCabinetType)
                .Set("MainDaylightDirection", input.MainDaylightDirection)
                .Set("LivingRoomFloor", input.LivingRoomFloor)
                .Set("FaceType", input.FaceType)
                .Set("IsDuplex", input.IsDuplex)
                .Set("HasIranianLavatory", input.HasIranianLavatory)
                .Set("HasForeignLavatory", input.HasForeignLavatory)
                .Set("HasPrivatePatio", input.HasPrivatePatio)
                .Set("HasBeenReconstructed", input.HasBeenReconstructed)
                .Set("HasElevator", input.HasElevator)
                .Set("HasGatheringHall", input.HasGatheringHall)
                .Set("HasAutomaticParkingDoor", input.HasAutomaticParkingDoor)
                .Set("HasVideoEyePhone", input.HasVideoEyePhone)
                .Set("HasSwimmingPool", input.HasSwimmingPool)
                .Set("HasSauna", input.HasSauna)
                .Set("HasJacuzzi", input.HasJacuzzi)
                .Set("Owner", ownerReference);

            var propertyResult = DbManager.Property.UpdateOneAsync(propertyFilter, updateProperty).Result;
            if (propertyResult.MatchedCount != 1)
                return ValidationResult.Failure("Property", PropertyValidationErrors.NotFound);

            if (propertyResult.MatchedCount == 1 && propertyResult.ModifiedCount != 1)
                return ValidationResult.Failure("Property", PropertyValidationErrors.NotModified);

            if (property.Owner != null)
                UserActivityLogUtils.SetMainActivity(parentType: EntityType.Customer, parentId: property.Owner.ID);

            var supplyFilter = Builders<Supply>.Filter.Eq("Property.ID", input.ID);
            var supplyUpdate = Builders<Supply>.Update
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("Property.PropertyType", input.PropertyType)
                .Set("Property.UsageType", input.UsageType)
                .Set("Property.IsHidden", input.IsHidden)
                .Set("Property.ConversionWarning", input.ConversionWarning)
                .Set("Property.Owner", ownerReference)
                .Set("Property.Address", input.Address)
                .Set("Property.Vicinity",
                    (input.Vicinity != null && input.Vicinity.ID != ObjectId.Empty)
                        ? Mapper.Map<VicinityReference>(VicinityCache[input.Vicinity.ID])
                        : null)
                .Set("Property.GeographicLocation", Mapper.Map<GeoJson2DCoordinates>(input.GeographicLocation))
                .Set("Property.NumberOfRooms", input.NumberOfRooms)
                .Set("Property.LicencePlate", input.LicencePlate)
                .Set("Property.EstateArea", input.EstateArea)
                .Set("Property.UnitArea", input.UnitArea);

            var result = DbManager.Supply.UpdateManyAsync(supplyFilter, supplyUpdate);
            if (result.Result.MatchedCount != result.Result.ModifiedCount)
                Log.WarnFormat(
                    "Property reference with id {0} in related supply(ies) could not been updated in updating property process",
                    input.ID);
            else
            {
                Log.InfoFormat(
                    "Property reference with id {0} in related supply(ies) has been updated in updating property process",
                    input.ID);
            }

            var contractFilter = Builders<Contract>.Filter.Eq("PropertyReference.ID", input.ID);
            var contractUpdate = Builders<Contract>.Update
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("PropertyReference.Address", input.Address)
                .Set("PropertyReference.PropertyType", input.PropertyType)
                .Set("PropertyReference.IsHidden", input.IsHidden)
                .Set("PropertyReference.ConversionWarning", input.ConversionWarning)
                .Set("PropertyReference.UsageType", input.UsageType)
                .Set("PropertyReference.Owner", ownerReference)
                .Set("PropertyReference.Vicinity",
                    (input.Vicinity != null && input.Vicinity.ID != ObjectId.Empty)
                        ? Mapper.Map<VicinityReference>(VicinityCache[input.Vicinity.ID])
                        : null)
                .Set("PropertyReference.GeographicLocation", Mapper.Map<GeoJson2DCoordinates>(input.GeographicLocation))
                .Set("PropertyReference.NumberOfRooms", input.NumberOfRooms)
                .Set("PropertyReference.LicencePlate", input.LicencePlate)
                .Set("PropertyReference.EstateArea", input.EstateArea)
                .Set("PropertyReference.UnitArea", input.UnitArea);

            result = DbManager.Contract.UpdateManyAsync(contractFilter, contractUpdate);
            if (result.Result.MatchedCount != result.Result.ModifiedCount)
            {
                Log.WarnFormat(
                    "Property reference with id {0} in related contract(s) could not been updated in updating property process",
                    input.ID);
            }
            else
            {
                Log.InfoFormat(
                    "Property reference with id {0} in related contract(s) has been updated in updating property process",
                    input.ID);
            }

            return ValidationResult.Success;
        }

        public ValidationResult DeleteProperty(string id, bool publicOnly, ApplicationType applicationType)
        {
            var filter = Builders<Property>.Filter.Eq("ID", ObjectId.Parse(id));

            var property = DbManager.Property.Find(filter).SingleOrDefaultAsync().Result;

            if (!AuthorizeForEdit(property, publicOnly))
            {
                return ValidationResult.Failure("Property", GeneralValidationErrors.AccessDenied);
            }

            UserActivityLogUtils.SetMainActivity(parentType: EntityType.Customer, parentId: property.ID);

            var validationResult = ValidateForDelete(property);
            if (!validationResult.IsValid)
                return validationResult;

            property.Supplies.ForEach(s => s.State = SupplyState.Deleted);
            var update = Builders<Property>.Update
                .Set("DeletionTime", DateTime.Now)
                .Set("DeletedByID", ObjectId.Parse(OwinRequestScopeContext.Current.GetUserId()))
                .Set("State", PropertyState.Deleted)
                .Set("Supplies", property.Supplies)
                .Set("LastIndexingTime", BsonNull.Value);

            var result = DbManager.Property.UpdateOneAsync(filter, update).Result;
            if (result.MatchedCount != 1)
                return ValidationResult.Failure("Property", PropertyValidationErrors.NotFound);

            if (result.MatchedCount == 1 && result.ModifiedCount != 1)
                return ValidationResult.Failure("Property", PropertyValidationErrors.NotModified);

            var ids = property.Supplies.Select(s => s.ID).ToList();
            var supplyFilter = Builders<Supply>.Filter.In("ID", ids);
            var supplyUpdate = Builders<Supply>.Update
                .Set("State", SupplyState.Deleted)
                .Set("DeletionTime", DateTime.Now)
                .Set("DeletedByID", ObjectId.Parse(OwinRequestScopeContext.Current.GetUserId()))
                .Set("LastIndexingTime", BsonNull.Value);

            result = DbManager.Supply.UpdateManyAsync(supplyFilter, supplyUpdate).Result;
            if (result.MatchedCount != result.ModifiedCount)
            {
                Log.WarnFormat(
                    "Property reference with id {0} in related supply(ies) could not been deleted in deleting property process",
                    id);
            }

            ids.ForEach(i =>
            {
                UserActivityLogUtils.ReportAdditionalActivity(new UserActivity
                {
                    ApplicationType = applicationType,
                    TargetType = EntityType.Supply,
                    TargetID = i,
                    ActivityType = UserActivityType.ChangeState,
                    TargetState = SupplyState.Deleted.ToString(),
                    ParentType = EntityType.Property,
                    ParentID = property.ID
                });
            });

            if (property.Owner != null && property.Owner.ID != ObjectId.Empty)
            {
                var ownerFilter = Builders<Customer>.Filter.Eq("ID", property.Owner.ID);
                var ownerUpdate =
                    Builders<Customer>.Update.Inc("PropertyCount", -1)
                        .Set("LastIndexingTime", BsonNull.Value)
                        .Set("LastVisitTime", DateTime.Now);
                var ownerResult = DbManager.Customer.UpdateOneAsync(ownerFilter, ownerUpdate).Result;
                if (ownerResult.MatchedCount != 1 ||
                    (ownerResult.MatchedCount == 1 && ownerResult.ModifiedCount != 1))
                {
                    Log.WarnFormat(
                        "Property count in customer with id {0} could not been decreased in deleting property process",
                        property.Owner.ID);
                }
            }

            return ValidationResult.Success;
        }

        public ValidationResult AddNewImage(HttpRequestMessage request, bool publicOnly)
        {
            if (!request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var provider = MultipartFormDataUtil.ReadRequestContent(request.Content).Result;
            var files = provider.FileData;
            var fields = provider.FormData;

            var originalFileName =
                InMemoryMultipartFormDataStreamProvider.UnquoteToken(files[0].Headers.ContentDisposition.FileName);
            var originalFileExtension = Path.GetExtension(originalFileName);
            var contentType = files[0].Headers.ContentType.ToString();

            if (fields["propertyId"] == null)
                return
                    ValidationResult.Failure("Property.ID", PropertyValidationErrors.IsNotValid);

            ObjectId propertyId = ObjectId.Parse(fields["propertyId"]);
            var filter = Builders<Property>.Filter.Eq("ID", propertyId);

            var property = DbManager.Property.Find(filter).SingleOrDefaultAsync().Result;

            if (!AuthorizeForEdit(property, publicOnly))
            {
                return ValidationResult.Failure("Property", GeneralValidationErrors.AccessDenied);
            }

            var photoId = ObjectId.GenerateNewId();
            var image = new PhotoInfo
            {
                ID = photoId,
                Title = fields["title"],
                Description = fields["description"],
                OriginalFileName = originalFileName,
                OriginalFileExtension = originalFileExtension,
                ContentType = contentType,
                CreationTime = DateTime.Now
            };

            if (image.Title == "")
                image.Title = image.OriginalFileName.Substring(0,
                    image.OriginalFileName.Length > 15 ? 15 : image.OriginalFileName.Length);

            UpdateDefinition<Property> update;
            if (property.Photos == null || property.Photos.All(p => p.DeletionTime != null))
            {
                update = Builders<Property>.Update.Push("Photos", image)
                    .Set("CoverImageID", photoId);

                //updating propertyreference
                var supplyFilter = Builders<Supply>.Filter.Eq("Property.ID", propertyId);
                var updateSupply = Builders<Supply>.Update
                    .Set("Property.CoverImageID", photoId)
                    .Set("LastIndexingTime", BsonNull.Value);
                var supplyResult = DbManager.Supply.UpdateManyAsync(supplyFilter, updateSupply).Result;

                if (supplyResult.MatchedCount != supplyResult.ModifiedCount)
                {
                    Log.WarnFormat(
                        "Property reference with id {0} in related supply(ies) could not been updated in adding image to property process",
                        propertyId);
                }
                else
                {
                    Log.InfoFormat(
                        "Property reference with id {0} in related supply(ies) has been updated in adding image to property process",
                        propertyId);
                }
            }
            else
            {
                update = Builders<Property>.Update.Push("Photos", image);
            }

            var result = DbManager.Property.UpdateOneAsync(filter, update).Result;
            if (result.MatchedCount != 1)
                return ValidationResult.Failure("Property", PropertyValidationErrors.NotFound);

            if (result.MatchedCount == 1 && result.ModifiedCount != 1)
                return ValidationResult.Failure("Property", PropertyValidationErrors.NotModified);

            UserActivityLogUtils.SetMainActivity(
                targetId: photoId,
                parentType: EntityType.Property,
                parentId: propertyId
                );

            SavePhoto(image, files[0].Stream.ToArray());
            return ValidationResult.Success;
        }

        public ValidationResult DeleteImage(string id, bool publicOnly)
        {
            ObjectId newId = ObjectId.Parse(id);
            var filter = Builders<Property>.Filter.Eq("Photos.ID", newId);
            UpdateDefinition<Property> update;

            var property = DbManager.Property.Find(filter).SingleOrDefaultAsync().Result;

            if (!AuthorizeForEdit(property, publicOnly))
            {
                return ValidationResult.Failure("Property", GeneralValidationErrors.AccessDenied);
            }

            if (property.Photos.Count < 1)
                return ValidationResult.Failure("Property", PropertyValidationErrors.NotFound);

            if (property.CoverImageID == newId)
            {
                UpdateDefinition<Supply> updateSupply;
                var supplyFilter = Builders<Supply>.Filter.Eq("Property.ID", property.ID);
                var photos = property.Photos.Where(p => p.DeletionTime == null).ToList();
                object coverImageId;

                if (photos.Count == 1)
                {
                    coverImageId = ObjectId.Empty;
                    update = Builders<Property>.Update.Set("Photos.$.DeletionTime", DateTime.Now)
                        .Set("CoverImageID", coverImageId);
                    updateSupply = Builders<Supply>.Update.Set("Property.CoverImageID", coverImageId);
                }
                else
                {
                    coverImageId = photos.First(p => p.ID != newId).ID;
                    update = Builders<Property>.Update.Set("Photos.$.DeletionTime", DateTime.Now)
                        .Set("CoverImageID", coverImageId);
                    updateSupply = Builders<Supply>.Update.Set("Property.CoverImageID", coverImageId);
                }

                var supplyResult = DbManager.Supply.UpdateManyAsync(supplyFilter, updateSupply).Result;
                if (supplyResult.MatchedCount != supplyResult.ModifiedCount)
                {
                    Log.WarnFormat(
                        "Property reference with id {0} in related supply(ies) could not been updated in adding image to property process",
                        property.ID);
                }
                else
                {
                    Log.InfoFormat(
                        "Property reference with id {0} in related supply(ies) has been updated in adding image to property process",
                        property.ID);
                }
            }
            else
            {
                update = Builders<Property>.Update.Set("Photos.$.DeletionTime", DateTime.Now);
            }

            var result = DbManager.Property.UpdateOneAsync(filter, update);
            if (result.Result.MatchedCount != 1)
                return ValidationResult.Failure("Property", PropertyValidationErrors.NotFound);

            if (result.Result.MatchedCount == 1 && result.Result.ModifiedCount != 1)
                return ValidationResult.Failure("Property", PropertyValidationErrors.NotModified);

            UserActivityLogUtils.SetMainActivity(
                targetId: newId,
                parentType: EntityType.Property,
                parentId: property.ID);

            return ValidationResult.Success;
        }

        public ValidationResult ChangeCover(string id, bool publicOnly)
        {
            var filter = Builders<Property>.Filter.Eq("Photos.ID", ObjectId.Parse(id));
            var update = Builders<Property>.Update.Set("CoverImageID", ObjectId.Parse(id));

            var property = DbManager.Property.Find(filter).SingleOrDefaultAsync().Result;

            if (!AuthorizeForEdit(property, publicOnly))
            {
                return ValidationResult.Failure("Property", GeneralValidationErrors.AccessDenied);
            }

            var result = DbManager.Property.UpdateOneAsync(filter, update).Result;
            if (result == null)
                return ValidationResult.Failure("Property", PropertyValidationErrors.NotFound);

            UserActivityLogUtils.SetMainActivity(targetId: property.ID, parentType: EntityType.Customer,
                parentId: property.Owner.ID);

            var supplyFilter = Builders<Supply>.Filter.Eq("Property.ID", property.ID);
            var updateSupply = Builders<Supply>.Update.Set("Property.CoverImageID", ObjectId.Parse(id));
            var supplyResult = DbManager.Supply.UpdateManyAsync(supplyFilter, updateSupply).Result;

            if (supplyResult.MatchedCount != supplyResult.ModifiedCount)
            {
                Log.WarnFormat(
                    "Property reference with id {0} in related supply(ies) could not been updated in adding image to property process",
                    property.ID);
            }
            else
            {
                Log.InfoFormat(
                    "Property reference with id {0} in related supply(ies) has been updated in adding image to property process",
                    property.ID);
            }

            return ValidationResult.Success;
        }

        public bool RetrieveFromKhoonat()
        {
            var supplyResults = new[] {0, 0, 0}; //{successful, unsuccessful, withoutChange}
            var propertyResults = new[] {0, 0, 0}; //{successful, unsuccessful, withoutChange}
            var propertySqlIds = new List<string>();

            var lastFetchTime = GetLastFetchTime();
            var maxModificationTime = lastFetchTime;

            ApplicationStaticLogs.KhoonatServiceCallLog.InfoFormat(
                "Start of retrieving {0} property(ies) from Khoonat since {1}...", NumberOfItems, lastFetchTime);

            var conn = GetSqlConnection();
            var cmd = GetPropertyQuery(lastFetchTime, conn);
            conn.Open();
            using (conn)
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var supply = ReadSupplyFromSql(reader);
                        var property = ReadPropertyFromSql(reader, supply);

                        SupplyUtil.RecalculatePrices(supply, property);

                        CalculateResult(AddSupply(supply, lastFetchTime, property), supplyResults);

                        if(property?.Supplies != null && property.Supplies.Count == 1)
                            property.Supplies[0].ID = supply.ID;

                        var addPropertyResult = AddProperty(property, lastFetchTime);
                        CalculateResult(addPropertyResult, propertyResults);

                        if (property != null)
                        {
                            propertySqlIds.Add(property.ExternalID);

                            if (addPropertyResult != null && addPropertyResult == true)
                            {
                                supply.Property = Mapper.Map<PropertyReference>(property);
                                UpdateSupply(supply);
                            }

                            if (property.LastModificationTime.HasValue &&
                                property.LastModificationTime > maxModificationTime)
                                maxModificationTime = property.LastModificationTime.Value;
                        }
                    }

                    reader.Close();
                }

                propertySqlIds.ForEach(peopertySqlId =>
                {
                    GetImageQuery(peopertySqlId.ToString(), cmd);
                    using (var photoReader = cmd.ExecuteReader())
                    {
                        while (photoReader.Read())
                        {
                            byte[] photoBinary;
                            PhotoInfo photo;
                            ReadPhotosFromSql(photoReader, out photo, out photoBinary);
                            AddPhoto(peopertySqlId, photo, photoBinary);
                        }
                        photoReader.Close();
                    }
                });

                UpdateFetchTime(maxModificationTime);

                ApplicationStaticLogs.KhoonatServiceCallLog.InfoFormat(
                    "End of retrieving property(ies): {0} success(s), {1} failure(s), {2} without change(s)" +
                    Environment.NewLine +
                    "End of retrieving supply(ies): {3} success(s), {4} failure(s), {5} without change(s)",
                    propertyResults[0], propertyResults[1], propertyResults[2], supplyResults[0], supplyResults[1],
                    supplyResults[2]);

                conn.Close();
            }
            return true;
        }

        #endregion

        #region Private helper methods 

        public bool AuthorizeForView(Property property, bool publicOnly)
        {
            var currentUserId = OwinRequestScopeContext.Current.GetUserId();
            if (publicOnly)
            {
                if (property.Supplies == null || property.Supplies.Count == 0)
                {
                    return false;
                }

                if (property.Supplies.TrueForAll(s => !s.IsPublic))
                {
                    return false;
                }

                if (property.CreatedByID != null && !currentUserId.IsNullOrEmpty() &&
                property.CreatedByID.Equals(ObjectId.Parse(currentUserId)))
                {
                    return true;
                }

                if (!property.Supplies.TrueForAll(s => s.ExpirationTime < DateTime.Today))
                {
                    return true;
                }

                return false;
            }

            if (JJOwinRequestScopeContextExtensions.IsAdministrator())
            {
                return true;
            }

            if (property.SourceType != SourceType.Haftdong)
            {
                return true;
            }

            if (property.Supplies != null && property.Supplies.Any(s => s.IsPublic))
            {
                return true;
            }

            if (property.CreatedByID != null && !currentUserId.IsNullOrEmpty() &&
                property.CreatedByID.Equals(ObjectId.Parse(currentUserId)))
            {
                return true;
            }

            return false;
        }

        public bool AuthorizeForEdit(Property property, bool publicOnly)
        {
            var currentUserId = OwinRequestScopeContext.Current.GetUserId();
            if (publicOnly)
            {
                if (property.Supplies == null || property.Supplies.Count == 0)
                {
                    return false;
                }

                if (property.Supplies.TrueForAll(s => !s.IsPublic))
                {
                    return false;
                }

                if (JJOwinRequestScopeContextExtensions.IsAdministrator())
                {
                    return true;
                }

                if (!currentUserId.IsNullOrEmpty() && property.CreatedByID == ObjectId.Parse(currentUserId))
                {
                    return true;
                }

                return false;
            }

            if (JJOwinRequestScopeContextExtensions.IsAdministrator())
            {
                return true;
            }

            if (property.CreatedByID != null && !currentUserId.IsNullOrEmpty() &&
                property.CreatedByID == ObjectId.Parse(currentUserId))
            {
                return true;
            }

            return false;
        }

        public ValidationResult ValidateForSaveOrUpdate(Property property, bool isPublic)
        {
            var errors = new List<ValidationError>();

            if (property.UsageType == null || property.UsageType == 0)
                errors.Add(new ValidationError("Property.UsageType", GeneralValidationErrors.ValueNotSpecified));

            if (property.PropertyType == 0)
                errors.Add(new ValidationError("Property.PropertyType", GeneralValidationErrors.ValueNotSpecified));

            if (!isPublic)
            {
                if (property.Owner == null || property.Owner.ID == ObjectId.Empty)
                    errors.Add(new ValidationError("Property.Owner", GeneralValidationErrors.ValueNotSpecified));
            }

            return new ValidationResult { Errors = errors };
        }

        private void CalculateResult(bool? result, int[] allResults)
        {

            switch (result)
            {
                case true:
                    allResults[0] = allResults[0] + 1;
                    break;
                case false:
                    allResults[1] = allResults[1] + 1;
                    break;
                case null:
                    allResults[2] = allResults[2] + 1;
                    break;
            }
        }

        private SqlConnection GetSqlConnection()
        {
            var ConnectionStringName = "Db";
            var connectionStringSettings = ConfigurationManager.ConnectionStrings[ConnectionStringName];
            if (connectionStringSettings == null)
                throw new ConfigurationErrorsException(
                    "SQL DB Connection String for attachments is not configured. A connection string named '" +
                    ConnectionStringName + "' should be present in the application configuration.");

            var connectionString = connectionStringSettings.ConnectionString;
            if (connectionString == null)
                throw new ConfigurationErrorsException(
                    "SQL DB Connection String for attachments (named '" + ConnectionStringName + "') is empty. ");

            return new SqlConnection(connectionString);
        }

        private SqlCommand GetPropertyQuery(DateTime lastFetchTime, SqlConnection conn)
        {
            var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("JahanJooy.RealEstateAgency.Util.ExternalData.KhoonatPropertiesScript.sql");
            if (stream != null)
            {
                var streamReader = new StreamReader(stream);
                var query = streamReader.ReadToEnd();
                query = query.Replace("@lastFetchTime", lastFetchTime.ToString("o").Substring(0, 22));
                query = query.Replace("@numberOfItems", NumberOfItems.ToString());
                return new SqlCommand(query, conn);
            }
            return new SqlCommand();
        }

        private Supply ReadSupplyFromSql(SqlDataReader reader)
        {
            var supply = new Supply
            {
                ExternalID = (DBNull.Value != reader[0]) ? ((long) reader[0]).ToString() : "",
                IntentionOfOwner = (IntentionOfOwner) ((DBNull.Value != reader[2]) ? (byte) reader[2] : 0),
                PriceSpecificationType =
                    (DBNull.Value != reader[39]) ? (SalePriceSpecificationType?) (byte) reader[39] : null,
                TotalPrice = (DBNull.Value != reader[40]) ? (decimal?) reader[40] : 0,
                PricePerEstateArea = (DBNull.Value != reader[41]) ? (decimal?) reader[41] : 0,
                PricePerUnitArea = (DBNull.Value != reader[42]) ? (decimal?) reader[42] : 0,
                HasTransferableLoan = (DBNull.Value != reader[43]) && (bool) reader[43],
                TransferableLoanAmount = (DBNull.Value != reader[44]) ? (decimal?) reader[44] : 0,
                Mortgage = (DBNull.Value != reader[45]) ? (decimal?) reader[45] : 0,
                Rent = (DBNull.Value != reader[46]) ? (decimal?) reader[46] : 0,
                MortgageAndRentConvertible = (DBNull.Value != reader[47]) && (bool) reader[47],
                MinimumMortgage = (DBNull.Value != reader[48]) ? (decimal?) reader[48] : 0,
                MinimumRent = (DBNull.Value != reader[49]) ? (decimal?) reader[49] : 0,
                OwnerCanBeContacted = (bool)reader[56],
                OwnerContact = new ContactMethodCollection
                {
                    ContactName = (DBNull.Value != reader[57]) ? (string)reader[57] : "",
                    Phones = new List<PhoneInfo>
                    {
                        new PhoneInfo
                        {
                            Value = (DBNull.Value != reader[58]) ? ((string) reader[58]).Trim() : ""
                        },
                        new PhoneInfo
                        {
                            Value = (DBNull.Value != reader[59]) ? ((string) reader[59]).Trim() : ""
                        }
                    },
                    Emails = new List<EmailInfo>
                    {
                        new EmailInfo
                        {
                            Value = (DBNull.Value != reader[60]) ? (string) reader[60] : ""
                        }
                    }
                },
                AgencyContact = new ContactMethodCollection
                {
                    OrganizationName = (DBNull.Value != reader[50]) ? (string)reader[50] : "",
                    ContactName = (DBNull.Value != reader[52]) ? (string)reader[52] : "",
                    Phones = new List<PhoneInfo>
                    {
                        new PhoneInfo
                        {
                            Value = (DBNull.Value != reader[53]) ? ((string) reader[53]).Trim() : ""
                        },
                        new PhoneInfo
                        {
                            Value = (DBNull.Value != reader[54]) ? ((string) reader[54]).Trim() : ""
                        }
                    },
                    Emails = new List<EmailInfo>
                    {
                        new EmailInfo
                        {
                            Value = (DBNull.Value != reader[55]) ? (string) reader[55] : ""
                        }
                    },
                    Addresses = new List<AddressInfo>
                    {
                        new AddressInfo
                        {
                            Value = (DBNull.Value != reader[51]) ? (string) reader[51] : ""
                        }
                    }
                },
                CreationTime = (DateTime) reader[62],
                LastIndexingTime = DateTime.Now, //for preventing indexing in the middle of proccess
                LastModificationTime = (DateTime) reader[63],
                ExpirationTime = (DateTime) reader[64],
                LastFetchTime = DateTime.Now,
                State = SupplyState.New,
                IsPublic = true
            };

            return supply;
        }

        private Property ReadPropertyFromSql(SqlDataReader reader, Supply supply)
        {
            Vicinity vicinity = null;
            if ((DBNull.Value != reader[5]))
            {
                var vicinityFilter = Builders<Vicinity>.Filter.Eq("SqlID", (long) reader[5]);
                vicinity = DbManager.Vicinity.Find(vicinityFilter).SingleOrDefaultAsync().Result;
            }

            var centerPoint = (DBNull.Value != reader[65]) ? (SqlGeography) reader[65] : null;

            GeoJson2DCoordinates point = null;
            GeographicLocationSpecificationType? locationType = null;
            if (centerPoint?.Long != null && centerPoint?.Lat != null)
            {
                point = new GeoJson2DCoordinates((double) centerPoint.Long, (double) centerPoint.Lat);
                locationType = GeographicLocationSpecificationType.UserSpecifiedExact;
            }
            else if (vicinity?.CenterPoint != null)
            {
                point = vicinity.CenterPoint;
                locationType = GeographicLocationSpecificationType.InferredFromVicinity;
            }

            var property = new Property
            {
                ExternalID = (DBNull.Value != reader[0]) ? ((long) reader[0]).ToString() : "",
                SourceType = SourceType.Khoonat,
                PropertyType = (PropertyType) ((DBNull.Value != reader[1]) ? (byte) reader[1] : 0),
                IsAgencyListing = (DBNull.Value != reader[3]) && (bool) reader[3],
                IsAgencyActivityAllowed = (DBNull.Value != reader[4]) && (bool) reader[4],
                Vicinity = (vicinity != null) ? Mapper.Map<VicinityReference>(vicinity) : null,
                Address = (DBNull.Value != reader[6]) ? (string) reader[6] : "",
                EstateArea = (DBNull.Value != reader[7]) ? (decimal?) reader[7] : 0,
                EstateDirection = (DBNull.Value != reader[8]) ? (EstateDirection?) (byte) reader[8] : null,
                EstateVoucherType = (DBNull.Value != reader[9]) ? (EstateVoucherType?) (byte) reader[9] : null,
                PassageEdgeLength = (DBNull.Value != reader[10]) ? (decimal?) reader[10] : 0,
                TotalNumberOfUnits = (DBNull.Value != reader[11]) ? (int?) reader[11] : 0,
                NumberOfUnitsPerFloor = (DBNull.Value != reader[12]) ? (int?) reader[12] : 0,
                TotalNumberOfFloors = (DBNull.Value != reader[13]) ? (int?) reader[13] : 0,
                FaceType = (DBNull.Value != reader[14]) ? (BuildingFaceType?) (byte) reader[14] : null,
                BuildingAgeYears = (DBNull.Value != reader[15]) ? (int?) reader[15] : 0,
                HasElevator = (DBNull.Value != reader[16]) && (bool) reader[16],
                HasGatheringHall = (DBNull.Value != reader[17]) && (bool) reader[17],
                HasAutomaticParkingDoor = (DBNull.Value != reader[18]) && (bool) reader[18],
                HasVideoEyePhone = (DBNull.Value != reader[19]) && (bool) reader[19],
                HasSwimmingPool = (DBNull.Value != reader[20]) && (bool) reader[20],
                HasSauna = (DBNull.Value != reader[21]) && (bool) reader[21],
                HasJacuzzi = (DBNull.Value != reader[22]) && (bool) reader[22],
                UnitFloorNumber = (DBNull.Value != reader[23]) ? (int?) reader[23] : 0,
                NumberOfRooms = (DBNull.Value != reader[24]) ? (int?) reader[24] : 0,
                NumberOfParkings = (DBNull.Value != reader[25]) ? (int?) reader[25] : 0,
                UsageType = (DBNull.Value != reader[26]) ? (UsageType?) (byte) reader[26] : null,
                UnitArea = (DBNull.Value != reader[27]) ? (decimal?) reader[27] : 0,
                StorageRoomArea = (DBNull.Value != reader[28]) ? (decimal?) reader[28] : 0,
                IsDuplex = (DBNull.Value != reader[29]) && (bool) reader[29],
                HasIranianLavatory = (DBNull.Value != reader[30]) && (bool) reader[30],
                HasForeignLavatory = (DBNull.Value != reader[31]) && (bool) reader[31],
                HasPrivatePatio = (DBNull.Value != reader[32]) && (bool) reader[32],
                KitchenCabinetType =
                    (DBNull.Value != reader[33]) ? (KitchenCabinetType?) (byte) reader[33] : null,
                MainDaylightDirection = (DBNull.Value != reader[35]) ? (DaylightDirection?) (byte) reader[35] : null,
                LivingRoomFloor = (DBNull.Value != reader[36]) ? (FloorCoverType?) (byte) reader[36] : null,
                HasBeenReconstructed = (DBNull.Value != reader[37]) && (bool) reader[37],
                NumberOfMasterBedrooms = (DBNull.Value != reader[38]) ? (int?) reader[38] : 0,
                PropertyStatus = (DBNull.Value != reader[61]) ? (PropertyStatus?) (byte) reader[61] : null,
                CreationTime = (DateTime) reader[62],
                LastModificationTime = (DateTime) reader[63],
                GeographicLocation = point,
                GeographicLocationType =
                    (DBNull.Value != reader[66]) ? (GeographicLocationSpecificationType?) (byte) reader[66] : locationType,
                State = PropertyState.New,
                LastFetchTime = DateTime.Now,
                LastIndexingTime = null,
                Photos = new List<PhotoInfo>(),
                Supplies = new List<SupplyReference> {Mapper.Map<SupplyReference>(supply)}
            };
            return property;
        }

        private bool? AddSupply(Supply supply, DateTime lastFetchTime, Property property)
        {
            string message;
            var contactResult = LocalContactMethodUtil.PrepareContactMethods(supply.AgencyContact, true, false, true);
            if (!contactResult.IsValid)
                foreach (var error in contactResult.Errors)
                {
                    message = Environment.NewLine + error.FullResourceKey;
                    SetConversionWarning(property, message, true);
                }

            contactResult = LocalContactMethodUtil.PrepareContactMethods(supply.OwnerContact, true, false, true);
            if (!contactResult.IsValid)
                foreach (var error in contactResult.Errors)
                {
                    message = Environment.NewLine + error.FullResourceKey;
                    SetConversionWarning(property, message, true);
                }

            var supplyFilter = Builders<Supply>.Filter.Eq("ExternalID", supply.ExternalID);
            var existingSupply = DbManager.Supply.Find(supplyFilter).SingleOrDefaultAsync();
            if (existingSupply?.Result != null) //Existing item
            {
                supply.ID = existingSupply.Result.ID;
                if (supply.LastModificationTime > lastFetchTime)
                {
                    var updateSupply = Builders<Supply>.Update
                        .Set("LastIndexingTime", supply.LastIndexingTime)
                        .Set("ExpirationTime", supply.ExpirationTime)
                        .Set("IsPublic", supply.IsPublic)
                        .Set("LastModificationTime", DateTime.Now)
                        .Set("IntentionOfOwner", supply.IntentionOfOwner)
                        .Set("State", supply.State)
                        .Set("PriceSpecificationType", supply.PriceSpecificationType)
                        .Set("TotalPrice", supply.TotalPrice)
                        .Set("PricePerEstateArea", supply.PricePerEstateArea)
                        .Set("PricePerUnitArea", supply.PricePerUnitArea)
                        .Set("HasTransferableLoan", supply.HasTransferableLoan)
                        .Set("TransferableLoanAmount", supply.TransferableLoanAmount)
                        .Set("Mortgage", supply.Mortgage)
                        .Set("Rent", supply.Rent)
                        .Set("MortgageAndRentConvertible", supply.MortgageAndRentConvertible)
                        .Set("MinimumMortgage", supply.MinimumMortgage)
                        .Set("MinimumRent", supply.MinimumRent)
                        .Set("AgencyContact", supply.AgencyContact)
                        .Set("OwnerContact", supply.OwnerContact)
                        .Set("OwnerCanBeContacted", supply.OwnerCanBeContacted)
                        .Set("CreationTime", supply.CreationTime)
                        .Set("ExpirationTime", supply.ExpirationTime);

                    var updatedSupply = DbManager.Supply.FindOneAndUpdateAsync(supplyFilter, updateSupply);
                  
                    if (updatedSupply?.Result == null)
                    {
                        ApplicationStaticLogs.KhoonatServiceCallLog.ErrorFormat(
                            "Unexpected error while updating supply with External ID {0}", supply.ExternalID);
                        return false;
                    }
                    return true;
                }
                return null;
            }

            //New One
            DbManager.Supply.InsertOneAsync(supply);
            return true;
        }

        private bool? AddProperty(Property property, DateTime lastFetchTime)
        {
            var propertyFilter = Builders<Property>.Filter.Eq("ExternalID", property.ExternalID);
            var existingProperty = DbManager.Property.Find(propertyFilter).SingleOrDefaultAsync();
            if (existingProperty?.Result != null) //Existing item
            {
                property.ID = existingProperty.Result.ID;
                if (property.LastModificationTime > lastFetchTime)
                {
                    var updateProperty = Builders<Property>.Update
                        .Set("LastIndexingTime", property.LastIndexingTime)
                        .Set("LastModificationTime", DateTime.Now)
                        .Set("LastFetchTime", property.LastFetchTime)
                        .Set("Address", property.Address)
                        .Set("State", property.State)
                        .Set("IsAgencyListing", property.IsAgencyListing)
                        .Set("IsAgencyActivityAllowed", property.IsAgencyActivityAllowed)
                        .Set("PropertyType", property.PropertyType)
                        .Set("PropertyStatus", property.PropertyStatus)
                        .Set("Vicinity", property.Vicinity)
                        .Set("EstateArea", property.EstateArea)
                        .Set("EstateDirection", property.EstateDirection)
                        .Set("PassageEdgeLength", property.PassageEdgeLength)
                        .Set("EstateVoucherType", property.EstateVoucherType)
                        .Set("TotalNumberOfUnits", property.TotalNumberOfUnits)
                        .Set("BuildingAgeYears", property.BuildingAgeYears)
                        .Set("NumberOfUnitsPerFloor", property.NumberOfUnitsPerFloor)
                        .Set("TotalNumberOfFloors", property.TotalNumberOfFloors)
                        .Set("UnitArea", property.UnitArea)
                        .Set("StorageRoomArea", property.StorageRoomArea)
                        .Set("UsageType", property.UsageType)
                        .Set("NumberOfRooms", property.NumberOfRooms)
                        .Set("NumberOfParkings", property.NumberOfParkings)
                        .Set("UnitFloorNumber", property.UnitFloorNumber)
                        .Set("NumberOfMasterBedrooms", property.NumberOfMasterBedrooms)
                        .Set("KitchenCabinetType", property.KitchenCabinetType)
                        .Set("MainDaylightDirection", property.MainDaylightDirection)
                        .Set("LivingRoomFloor", property.LivingRoomFloor)
                        .Set("FaceType", property.FaceType)
                        .Set("IsDuplex", property.IsDuplex)
                        .Set("HasIranianLavatory", property.HasIranianLavatory)
                        .Set("HasForeignLavatory", property.HasForeignLavatory)
                        .Set("HasPrivatePatio", property.HasPrivatePatio)
                        .Set("HasBeenReconstructed", property.HasBeenReconstructed)
                        .Set("HasElevator", property.HasElevator)
                        .Set("HasGatheringHall", property.HasGatheringHall)
                        .Set("HasAutomaticParkingDoor", property.HasAutomaticParkingDoor)
                        .Set("HasVideoEyePhone", property.HasVideoEyePhone)
                        .Set("HasSwimmingPool", property.HasSwimmingPool)
                        .Set("HasSauna", property.HasSauna)
                        .Set("HasJacuzzi", property.HasJacuzzi)
                        .Set("Supplies", property.Supplies)
                        .Set("Photos", property.Photos)
                        .Set("GeographicLocation", property.GeographicLocation)
                        .Set("SourceType", property.SourceType)
                        .Set("CreationTime", property.CreationTime);

                    var updatedProperty = DbManager.Property.FindOneAndUpdateAsync(propertyFilter, updateProperty);
                    if (updatedProperty?.Result == null)
                    {
                        ApplicationStaticLogs.KhoonatServiceCallLog.ErrorFormat(
                            "Unexpected error while updating property with External ID {0}", property.ExternalID);
                        return false;
                    }
                    return true;
                }
                return null;
            }

            //New One
            DbManager.Property.InsertOneAsync(property);
            return true;
        }

        private void GetImageQuery(string propertySqlId, SqlCommand cmd)
        {
            var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("JahanJooy.RealEstateAgency.Util.ExternalData.KhoonatImagesScript.sql");
            if (stream != null)
            {
                var streamReader = new StreamReader(stream);
                var query = streamReader.ReadToEnd();
                query = query.Replace("@PropertyListingID", propertySqlId);
                cmd.CommandText = query;
            }
        }

        private void ReadPhotosFromSql(SqlDataReader reader, out PhotoInfo photo, out byte[] photoBinary)
        {
            var storeItemID = (Guid) reader[1];
            photoBinary = GetFullSizeBytes(storeItemID);

            photo = new PhotoInfo
            {
                ID = ObjectId.GenerateNewId(),
                ExternalID = (DBNull.Value != reader[0]) ? (long) reader[0] : 0,
                Title = (DBNull.Value != reader[2]) ? (string) reader[2] : storeItemID.ToString(),
                Description = (DBNull.Value != reader[3]) ? (string) reader[3] : "",
                CreationTime = (DateTime) reader[4],
                DeletionTime = (DBNull.Value != reader[5]) ? (DateTime?) reader[5] : null,
                OriginalFileExtension = ".jpg",
                ContentType = "image/jpeg"
            };
        }

        public void AddPhoto(string propertySqlId, PhotoInfo photo, byte[] photoBinary)
        {
            var propertyFilter = Builders<Property>.Filter.Eq("ExternalID", propertySqlId);
            var existingProperty = DbManager.Property.Find(propertyFilter).SingleOrDefaultAsync();

            if (existingProperty?.Result != null)
            {
                if (photo.DeletionTime.HasValue && existingProperty.Result.Photos != null)
                {
                    var existingPhoto =
                        existingProperty.Result.Photos.SingleOrDefault(
                            p => p.ExternalID == photo.ExternalID && p.DeletionTime == null);
                    if (existingPhoto != null)
                    {
                        var photoFilter = Builders<Property>.Filter.Eq("Photos.ExternalID", photo.ExternalID);
                        var update = Builders<Property>.Update.Set("Photos.$.DeletionTime", photo.DeletionTime.Value);
                        var result = DbManager.Property.UpdateOneAsync(photoFilter, update).Result;
                        if (result.MatchedCount != 1)
                            ApplicationStaticLogs.KhoonatServiceCallLog.ErrorFormat(
                                "Could not find photo with External ID {0} while updating process", photo.ExternalID);

                        if (result.MatchedCount == 1 && result.ModifiedCount != 1)
                            ApplicationStaticLogs.KhoonatServiceCallLog.ErrorFormat(
                                "Could not update photo with External ID {0} while updating process", photo.ExternalID);
                    }
                }
                else
                {
                    if ((existingProperty.Result.Photos != null &&
                         existingProperty.Result.Photos.All(p => p.ExternalID != photo.ExternalID))
                        || existingProperty.Result.Photos == null)
                    {
                        UpdateDefinition<Property> update;
                        if (existingProperty.Result.Photos == null ||
                            existingProperty.Result.Photos.Count == 0 ||
                            existingProperty.Result.Photos.All(p => p.DeletionTime != null))
                        {
                            update = Builders<Property>.Update.Push("Photos", photo)
                                .Set("CoverImageID", photo.ID);

                            //updating propertyreference
                            var supplyFilter = Builders<Supply>.Filter.Eq("Property.ID", existingProperty.Result.ID);
                            var updateSupply = Builders<Supply>.Update
                                .Set("Property.CoverImageID", photo.ID)
                                .Set("LastIndexingTime", BsonNull.Value);
                            var supplyResult = DbManager.Supply.UpdateManyAsync(supplyFilter, updateSupply).Result;

                            if (supplyResult.MatchedCount != supplyResult.ModifiedCount)
                            {
                                ApplicationStaticLogs.KhoonatServiceCallLog.WarnFormat(
                                    "Property reference with SQL id {0} in related supply(ies) could not been updated in adding image to property process",
                                    propertySqlId);
                            }
                        }
                        else
                        {
                            update = Builders<Property>.Update.Push("Photos", photo);
                        }

                        var result = DbManager.Property.UpdateOneAsync(propertyFilter, update).Result;
                        if (result.MatchedCount != 1)
                        {
                            ApplicationStaticLogs.KhoonatServiceCallLog.WarnFormat(
                                "Property with SQL id {0} could not been founded in adding image with SQL id {1} to property process",
                                propertySqlId, photo.ExternalID);
                        }
                        else if (result.MatchedCount == 1 && result.ModifiedCount != 1)
                        {
                            ApplicationStaticLogs.KhoonatServiceCallLog.WarnFormat(
                                "Photo with SQL id {0} could not been added to the property with SQL id {1} in adding image to property process",
                                photo.ExternalID, propertySqlId);
                        }
                        else
                        {
                            SavePhoto(photo, photoBinary);
                        }
                    }
                }
            }
            else
            {
                ApplicationStaticLogs.KhoonatServiceCallLog.ErrorFormat("Property with SQL ID {0} could not found",
                    propertySqlId);
            }
        }

        private void UpdateSupply(Supply supply)
        {
            var supplyFilter = Builders<Supply>.Filter.Eq("ID", supply.ID);
            var existingSupply = DbManager.Supply.Find(supplyFilter).SingleOrDefaultAsync();
            if (existingSupply?.Result != null) //Existing item
            {
                var updateSupply = Builders<Supply>.Update
                    .Set("LastIndexingTime", BsonNull.Value) //For indexing
                    .Set("Property", supply.Property);
                var result = DbManager.Supply.UpdateOneAsync(supplyFilter, updateSupply).Result;

                if (result.MatchedCount != 1)
                    ApplicationStaticLogs.KhoonatServiceCallLog.ErrorFormat(
                        "Could not find supply with ID {0} while updating process", supply.ID);

                if (result.MatchedCount == 1 && result.ModifiedCount != 1)
                    ApplicationStaticLogs.KhoonatServiceCallLog.ErrorFormat(
                        "Could not update supply with ID {0} while updating process", supply.ID);
            }
            else
            {
                ApplicationStaticLogs.KhoonatServiceCallLog.ErrorFormat(
                    "Could not find supply with ID {0} while updating process", supply.ID);
            }
        }

        public DateTime GetLastFetchTime()
        {
            var filter = Builders<ConfigurationDataItem>.Filter.Eq("Identifier",
                KhoonatPropertyLastFetchTime);
            var lastFetchTime = DbManager.ConfigurationDataItem.Find(filter).SingleOrDefaultAsync();
            if (lastFetchTime?.Result != null)
            {
                if (!string.IsNullOrEmpty(lastFetchTime.Result.Value))
                    return DateTime.Parse(lastFetchTime.Result.Value);
                return DateTime.Today.AddYears(-5);
            }
            return DateTime.Today.AddYears(-5);
        }

        private void UpdateFetchTime(DateTime lastFetchRecordDate)
        {
            var filter = Builders<ConfigurationDataItem>.Filter.Eq("Identifier",
                KhoonatPropertyLastFetchTime);
            var lastFetchTime = DbManager.ConfigurationDataItem.Find(filter).SingleOrDefaultAsync();
            if (lastFetchTime?.Result != null)
            {
                var update = Builders<ConfigurationDataItem>.Update
                    .Set("Value", lastFetchRecordDate.ToString("o"));
                var updateResult = DbManager.ConfigurationDataItem.UpdateOneAsync(filter, update).Result;
                if (updateResult.MatchedCount != 1)
                    ApplicationStaticLogs.KhoonatServiceCallLog.ErrorFormat(
                        "Could not find configuration data item with identifier {0} while updating process",
                        KhoonatPropertyLastFetchTime);

                if (updateResult.MatchedCount == 1 && updateResult.ModifiedCount != 1)
                    ApplicationStaticLogs.KhoonatServiceCallLog.ErrorFormat(
                        "Could not update configure data item with identifier {0} while updating process",
                        KhoonatPropertyLastFetchTime);
            }
            else
            {
                var newConfigDataItem = new ConfigurationDataItem
                {
                    Identifier = KhoonatPropertyLastFetchTime,
                    Value = lastFetchRecordDate.ToString("o")
                };
                DbManager.ConfigurationDataItem.InsertOneAsync(newConfigDataItem);
            }
        }

        private void EnsureInitialized()
        {
            if (_initialized)
                return;

            BinaryStorage.RegisterStoreId(UntouchedStoreId);

            _initialized = true;
        }

        public byte[] GetFullSizeBytes(Guid storeItemId)
        {
            EnsureInitialized();

            try
            {
                return BinaryStorage.RetrieveBytes(UntouchedStoreId, storeItemId);
            }
            catch (Exception e)
            {
                ApplicationStaticLogs.KhoonatServiceCallLog.Error(
                    "Could not load image ID " + storeItemId + " from store " + UntouchedStoreId, e);
                return ManifestResourceUtil.GetManifestResourceBytes(Assembly.GetExecutingAssembly(),
                    ResourceKeyImageCouldNotBeRetrieved);
            }
        }

        public void SavePhoto(PhotoInfo photo, byte[] photoBinary)
        {
            PhotoGridStore.StoreAttachmentBytes(AttachmentStoreEntityType.Image, photo.ID,
                PhotoStoreSize.FullSize, photoBinary);

            if (RebuildAttachmentSizesSupported(photo.ContentType))
            {
                RebuildAttachmentSizes(photoBinary, AttachmentStoreEntityType.Image, photo.ID);
            }
        }

        private bool RebuildAttachmentSizesSupported(string contentType)
        {
            var supportedTypes = new[] {"image/jpeg", "image/png", "image/gif", "image/bmp"};
            return supportedTypes.Contains(contentType.ToLower());
        }

        private void RebuildAttachmentSizes(byte[] sourceImageBytes, AttachmentStoreEntityType type, ObjectId photoId)
        {
            using (var ms = new MemoryStream())
            {
                ImageBuilder.Current.Build(sourceImageBytes, ms,
                    new ResizeSettings("width=80&height=80&crop=auto&format=jpg&quality=80"));

                ms.Seek(0, SeekOrigin.Begin);
                PhotoGridStore.StoreAttachmentBytes(type, photoId, PhotoStoreSize.Thumbnail, ms.ToArray());
            }

            using (var ms = new MemoryStream())
            {
                ImageBuilder.Current.Build(sourceImageBytes, ms,
                    new ResizeSettings("maxwidth=800&maxheight=2000&format=jpg&quality=80"));

                ms.Seek(0, SeekOrigin.Begin);
                PhotoGridStore.StoreAttachmentBytes(type, photoId, PhotoStoreSize.MediumSize, ms.ToArray());
            }
        }

        public ValidationResult ValidateForDelete(Property property)
        {
            if (property.State != PropertyState.New)
            {
                return ValidationResult.Failure("Property.State",
                    PropertyValidationErrors.IsNotValid);
            }
            return ValidationResult.Success;
        }

        private void PreparePropertyForSave(Property property, SourceType sourceType)
        {
            property.LastIndexingTime = DateTime.Now; //For preventing indexing proccess
            property.CreationTime = DateTime.Now;
            property.LastModificationTime = DateTime.Now;
            property.LastModifiedTimeByID = ObjectId.Parse(OwinRequestScopeContext.Current.GetUserId());
            property.CreatedByID = ObjectId.Parse(OwinRequestScopeContext.Current.GetUserId());
            property.State = PropertyState.New;
            property.IsArchived = false;
            property.SourceType = sourceType;
            property.Photos = new List<PhotoInfo>();
            property.Supplies = new List<SupplyReference>();
        }

        private static void SetConversionWarning(Property property, string message, bool setHidden)
        {
            property.IsHidden = setHidden;
            property.ConversionWarning = true;
            property.ExternalDetails += message;
        }

        #endregion
    }
}