using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Web.Http;
using AutoMapper;
using Compositional.Composer;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.Collections;
using JahanJooy.Common.Util.Identity;
using JahanJooy.Common.Util.Owin;
using JahanJooy.Common.Util.Validation;
using JahanJooy.Common.Util.Web.Result;
using JahanJooy.RealEstateAgency.Api.Web.Base;
using JahanJooy.RealEstateAgency.Domain.Base;
using JahanJooy.RealEstateAgency.Domain.Customers;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Enums.User;
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
using JahanJooy.RealEstateAgency.Util.Identity;
using JahanJooy.RealEstateAgency.Util.Models.Properties;
using JahanJooy.RealEstateAgency.Util.Report;
using JahanJooy.RealEstateAgency.Util.Report.ApplicationImplemented;
using JahanJooy.RealEstateAgency.Util.UserActivityLog;
using JahanJooy.RealEstateAgency.Util.Utils;
using JahanJooy.RealEstateAgency.Util.Utils.ContactMethods;
using JahanJooy.RealEstateAgency.Util.VicinityCache;
using JahanJooy.Stimulsoft.Common.Web;
using log4net;
using MongoDB.Bson;
using MongoDB.Driver;
using Stimulsoft.Report;

namespace JahanJooy.RealEstateAgency.Api.Web.Controllers
{
    [Contract]
    [Component]
    [ComponentCache(null)]
    [RoutePrefix("properties")]
    public class PropertyController : ExtendedApiController
    {
        #region Injected dependencies

        private static readonly ILog Log = LogManager.GetLogger(typeof(PropertyController));

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public PropertyUtil PropertyUtil { get; set; }

        [ComponentPlug]
        public CustomerUtil CustomerUtil { get; set; }

        [ComponentPlug]
        public UserUtil UserUtil { get; set; }

        [ComponentPlug]
        public IComposer Composer { get; set; }

        [ComponentPlug]
        public PhotoGridStore PhotoGridStore { get; set; }

        [ComponentPlug]
        public PropertyReport Report { get; set; }

        [ComponentPlug]
        public ReportRepository ReportRepository { get; set; }

        [ComponentPlug]
        public SupplyUtil SupplyUtil { get; set; }

        [ComponentPlug]
        public FileUtil FileUtil { get; set; }

        [ComponentPlug]
        public IVicinityCache VicinityCache { get; set; }

        [ComponentPlug]
        public LocalContactMethodUtil LocalContactMethodUtil { get; set; }

        [ComponentPlug]
        public LocalPhoneNumberUtil LocalPhoneNumberUtil { get; set; }

        #endregion

        #region Action methods

        [HttpPost, Route("save")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.Create, EntityType.Property, ApplicationType.Haftdong)]
        public IHttpActionResult Save(NewPropertyInput input)
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
                return ValidationResult(contactResult);

            if (ownerContact.Phones == null || ownerContact.Phones.Count == 0)
            {
                return ValidationResult(Common.Util.Validation.ValidationResult.Failure("Customer.Phones", GeneralValidationErrors.ValueNotSpecified));
            }

            owner.Contact = ownerContact;
            var property = Mapper.Map<Property>(input);
            var supply = Mapper.Map<Supply>(input);
            var request = Mapper.Map<Request>(input);

            if (input.VicinityID != ObjectId.Empty)
            {
                var vicinity = VicinityCache[input.VicinityID];
                var vicinityRefrence = Mapper.Map<VicinityReference>(vicinity);
                property.Vicinity = vicinityRefrence;
            }

            var result = FileUtil.SaveFile(property, supply, owner, request, false, SourceType.Haftdong, ApplicationType.Haftdong);
            if (!result.IsValid)
                return ValidationResult(result);

            UserActivityLogUtils.SetMainActivity(targetId: property.ID, parentType: EntityType.Customer,
                parentId: property.Owner.ID);

            return Ok(property);
        }

        [HttpPost, Route("update")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.Edit, EntityType.Property, ApplicationType.Haftdong)]
        public IHttpActionResult UpdateProperty(UpdatePropertyInput input)
        {
            UserActivityLogUtils.SetMainActivity(targetId: input.ID);
            return ValidationResult(PropertyUtil.UpdateProperty(input, false));
        }

        [HttpGet, Route("get/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        public PropertyDetails GetProperty(string id)
        {
            return PropertyUtil.GetProperty(id, false);
        }

        [HttpGet, Route("getrelatedproperties/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        public List<PropertyDetails> GetRelatedProperties(string id)
        {
            return PropertyUtil.GetRelatedProperties(id, false);
        }

        [HttpGet, Route("getcustomerproperties/{id}")]
        public List<PropertySummary> GetCustomerProperties(string id)
        {
            var propertySummaries = new List<PropertySummary>();
            var filter = Builders<Property>.Filter.Eq("Owner.ID", ObjectId.Parse(id));
            List<Property> properties =
                DbManager.Property.Find(filter).ToListAsync().Result;

            properties.ForEach(p =>
            {
                if (PropertyUtil.AuthorizeForView(p, false))
                {
                    propertySummaries.Add(Mapper.Map<PropertySummary>(p));
                }
            });

            return propertySummaries;
        }

        [HttpGet, Route("getContactInfos/{id}")]
        public PropertyContactInfoSummary GetContactInfos(string id)
        {
            return PropertyUtil.GetContactInfos(id, false);
        }

        [HttpPost, Route("done/{id}")]
        [UserActivity(UserActivityType.Edit, EntityType.Property, ApplicationType.Haftdong)]
        public IHttpActionResult Done(string id)
        {
            UserActivityLogUtils.SetMainActivity(
                targetId: ObjectId.Parse(id),
                activitySubType: "Setting ConversionWarning False"
                );

            var filter = Builders<Property>.Filter.Eq("ID", ObjectId.Parse(id));
            var property = DbManager.Property.Find(filter).SingleOrDefaultAsync().Result;

            if (!PropertyUtil.AuthorizeForEdit(property, false))
            {
                return ValidationResult("Property", GeneralValidationErrors.AccessDenied);
            }

            var update = Builders<Property>.Update
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("IsHidden", false)
                .Set("ConversionWarning", false);

            var result = DbManager.Property.FindOneAndUpdateAsync(filter, update);

            if (result?.Result == null)
                return
                    ValidationResult("Property", PropertyValidationErrors.NotFound);

            var supplyFilter = Builders<Supply>.Filter.Eq("Property.ID", result.Result.ID);
            var supplyUpdate = Builders<Supply>.Update
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("Property.IsHidden", false)
                .Set("Property.ConversionWarning", false);

            var supplyResult = DbManager.Supply.UpdateManyAsync(supplyFilter, supplyUpdate);
            if (supplyResult.Result.MatchedCount != supplyResult.Result.ModifiedCount)
            {
                Log.WarnFormat(
                    "Property reference with id {0} in related supply(ies) could not been completed in completing property process",
                    id);
            }
            else
            {
                Log.InfoFormat(
                    "Property reference with id {0} in related supply(ies) has been completed in completing property process",
                    id);
            }

            return ValidationResult(Common.Util.Validation.ValidationResult.Success);
        }

        [HttpPost, Route("archived/{id}")]
        [UserActivity(UserActivityType.Edit, EntityType.Property, ApplicationType.Haftdong)]
        public IHttpActionResult Archived(string id)
        {
            UserActivityLogUtils.SetMainActivity(
                targetId: ObjectId.Parse(id),
                activitySubType: "Archived"
                );

            var filter = Builders<Property>.Filter.Eq("ID", ObjectId.Parse(id));

            var property = DbManager.Property.Find(filter).SingleOrDefaultAsync().Result;

            if (!PropertyUtil.AuthorizeForEdit(property, false))
            {
                return ValidationResult("Property", GeneralValidationErrors.AccessDenied);
            }

            var update = Builders<Property>.Update
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("IsArchived", true);

            var result = DbManager.Property.FindOneAndUpdateAsync(filter, update);

            if (result?.Result == null)
                return
                    ValidationResult("Property", PropertyValidationErrors.NotFound);

            UserActivityLogUtils.SetMainActivity(parentType: EntityType.Customer, parentId: result.Result.ID);

            var supplyFilter = Builders<Supply>.Filter.Eq("Property.ID", result.Result.ID);
            var supplyUpdate = Builders<Supply>.Update
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("IsArchived", true);

            var supplyResult = DbManager.Supply.UpdateManyAsync(supplyFilter, supplyUpdate);
            if (supplyResult.Result.MatchedCount != supplyResult.Result.ModifiedCount)
            {
                Log.WarnFormat(
                    "Property reference with id {0} in related supply(ies) could not been archived in archiving property process",
                    id);
            }
            else
            {
                Log.InfoFormat(
                    "Property reference with id {0} in related supply(ies) has been archived in archiving property process",
                    id);
            }

            return ValidationResult(Common.Util.Validation.ValidationResult.Success);
        }

        [HttpPost, Route("publish")]
        [UserActivity(UserActivityType.Publish, EntityType.Property, ApplicationType.Haftdong)]
        public IHttpActionResult Publish(PublishPropertyInput input)
        {
            UserActivityLogUtils.SetMainActivity(
                targetId: input.ID
                );

            var filter = Builders<Property>.Filter.Eq("ID", input.ID);
            var property = DbManager.Property.Find(filter).SingleOrDefaultAsync().Result;

            if (!PropertyUtil.AuthorizeForEdit(property, false))
            {
                return ValidationResult("Property", GeneralValidationErrors.AccessDenied);
            }

            ContactMethodCollection ownerContact = new ContactMethodCollection
            {
                Emails = new List<EmailInfo>(),
                Phones = new List<PhoneInfo>(),
                Addresses = new List<AddressInfo>()
            };
            ContactMethodCollection agencyContact = new ContactMethodCollection
            {
                Emails = new List<EmailInfo>(),
                Phones = new List<PhoneInfo>(),
                Addresses = new List<AddressInfo>()
            };

            if (!input.OwnerCanBeContacted.HasValue)
            {
                var contactInfo = UserUtil.GetContactInfoOfCurrentUser();
                if (contactInfo == null)
                    return ValidationResult("Supply.ContactInfo", GeneralValidationErrors.ValueNotSpecified);

                ownerContact = contactInfo;
                input.OwnerCanBeContacted = true;

                if (ownerContact.Phones == null || ownerContact.Phones.Count == 0)
                {
                    return ValidationResult(Common.Util.Validation.ValidationResult.Failure("Supply.Phones", GeneralValidationErrors.ValueNotSpecified));
                }
            }
            else if (input.OwnerCanBeContacted.Value)
            {
                if (property == null)
                {
                    return ValidationResult("Property", PropertyValidationErrors.IsNotValid);
                }

                if (property.SourceType == SourceType.Haftdong)
                {
                    if (property.Owner == null)
                    {
                        return ValidationResult("Property.Owner", CustomerValidationErrors.NotFound);
                    }

                    var owner = CustomerUtil.GetCustomerById(property.Owner.ID);
                    ownerContact = owner.Contact;
                }
                else
                {
                    ownerContact.OrganizationName = input.OwnerContact.OrganizationName;
                    ownerContact.ContactName = input.OwnerContact.ContactName;
                    input.OwnerContact.Phones.ForEach(p => LocalContactMethodUtil.MapAndAddContactMethod(ownerContact, p));
                    input.OwnerContact.Emails.ForEach(e => LocalContactMethodUtil.MapAndAddContactMethod(ownerContact, e));
                    input.OwnerContact.Addresses.ForEach(a => LocalContactMethodUtil.MapAndAddContactMethod(ownerContact, a));
                    var contactResult = LocalContactMethodUtil.PrepareContactMethods(ownerContact, true, false, false);
                    if (!contactResult.IsValid)
                        return ValidationResult(contactResult);
                }

                if (ownerContact.Phones == null || ownerContact.Phones.Count == 0)
                {
                    return ValidationResult(Common.Util.Validation.ValidationResult.Failure("Supply.Phones", GeneralValidationErrors.ValueNotSpecified));
                }
            }
            else
            {
                agencyContact.OrganizationName = input.AgencyContact.OrganizationName;
                agencyContact.ContactName = input.AgencyContact.ContactName;
                input.AgencyContact.Phones.ForEach(p => LocalContactMethodUtil.MapAndAddContactMethod(agencyContact, p));
                input.AgencyContact.Emails.ForEach(e => LocalContactMethodUtil.MapAndAddContactMethod(agencyContact, e));
                input.AgencyContact.Addresses.ForEach(a => LocalContactMethodUtil.MapAndAddContactMethod(agencyContact, a));
                var contactResult = LocalContactMethodUtil.PrepareContactMethods(agencyContact, true, false, false);
                if (!contactResult.IsValid)
                    return ValidationResult(contactResult);

                if (agencyContact.Phones == null || agencyContact.Phones.Count == 0)
                {
                    return ValidationResult(Common.Util.Validation.ValidationResult.Failure("Supply.Phones", GeneralValidationErrors.ValueNotSpecified));
                }
            }

            property.Supplies.Where(s => s.State == SupplyState.New)
                .SafeForEach(s =>
                {
                    s.ExpirationTime = input.ExpirationTime;
                    s.IsPublic = true;
                    s.ContactToOwner = input.OwnerCanBeContacted ?? false;
                    s.ContactToAgency = !input.OwnerCanBeContacted ?? true;
                });
            var update = Builders<Property>.Update
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("Supplies", property.Supplies);

            var result = DbManager.Property.FindOneAndUpdateAsync(filter, update);

            if (result?.Result == null)
                return
                    ValidationResult("Property", PropertyValidationErrors.NotFound);

            var supplyFilter = Builders<Supply>.Filter.Eq("Property.ID", result.Result.ID);
            var supplyUpdate = Builders<Supply>.Update
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("ExpirationTime", input.ExpirationTime)
                .Set("OwnerCanBeContacted", input.OwnerCanBeContacted)
                .Set("OwnerContact", ownerContact)
                .Set("AgencyContact", agencyContact)
                .Set("IsPublic", true);

            var supplyResult = DbManager.Supply.UpdateManyAsync(supplyFilter, supplyUpdate);
            if (supplyResult.Result.MatchedCount != supplyResult.Result.ModifiedCount)
            {
                Log.WarnFormat(
                    "Suuply(ies) of Property with id {0} could not been published in publishing property process",
                    input.ID);
            }
            else
            {
                Log.InfoFormat(
                    "Suuply(ies) of Property with id {0} has been published in publishing property process",
                    input.ID);
            }

            return ValidationResult(Common.Util.Validation.ValidationResult.Success);
        }

        [HttpPost, Route("unpublish")]
        [UserActivity(UserActivityType.Unpublish, EntityType.Property, ApplicationType.Haftdong)]
        public IHttpActionResult Unpublish(string id)
        {
            UserActivityLogUtils.SetMainActivity(
                targetId: ObjectId.Parse(id)
                );

            var filter = Builders<Property>.Filter.Eq("ID", ObjectId.Parse(id));
            var property = DbManager.Property.Find(filter).SingleOrDefaultAsync().Result;

            if (!PropertyUtil.AuthorizeForEdit(property, false))
            {
                return ValidationResult("Property", GeneralValidationErrors.AccessDenied);
            }

            property.Supplies.Where(s => s.State == SupplyState.New)
                .SafeForEach(s =>
                {
                    s.ExpirationTime = null;
                    s.IsPublic = false;
                });
            var update = Builders<Property>.Update
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("Supplies", property.Supplies);

            var result = DbManager.Property.FindOneAndUpdateAsync(filter, update);

            if (result?.Result == null)
                return
                    ValidationResult("Property", PropertyValidationErrors.NotFound);

            var supplyFilter = Builders<Supply>.Filter.Eq("Property.ID", result.Result.ID);
            var supplyUpdate = Builders<Supply>.Update
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("ExpirationTime", BsonNull.Value)
                .Set("IsPublic", false);

            var supplyResult = DbManager.Supply.UpdateManyAsync(supplyFilter, supplyUpdate);
            if (supplyResult.Result.MatchedCount != supplyResult.Result.ModifiedCount)
            {
                Log.WarnFormat(
                    "Suuply(ies) of Property with id {0} could not been unpublished in unpublishing property process",
                    id);
            }
            else
            {
                Log.InfoFormat(
                    "Suuply(ies) of Property with id {0} has been unpublished in unpublishing property process",
                    id);
            }

            return ValidationResult(Common.Util.Validation.ValidationResult.Success);
        }

        [HttpPost, Route("unarchived/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.Edit, EntityType.Property, ApplicationType.Haftdong)]
        public IHttpActionResult UnArchived(string id)
        {
            UserActivityLogUtils.SetMainActivity(
                targetId: ObjectId.Parse(id),
                activitySubType: "UnArchived"
                );

            var filter = Builders<Property>.Filter.Eq("ID", ObjectId.Parse(id));

            var property = DbManager.Property.Find(filter).SingleOrDefaultAsync().Result;

            if (!PropertyUtil.AuthorizeForEdit(property, false))
            {
                return ValidationResult("Property", GeneralValidationErrors.AccessDenied);
            }

            var update = Builders<Property>.Update
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("IsArchived", false);

            var result = DbManager.Property.FindOneAndUpdateAsync(filter, update);
            if (result?.Result == null)
                return
                    ValidationResult("Property", PropertyValidationErrors.NotFound);

            UserActivityLogUtils.SetMainActivity(parentType: EntityType.Customer, parentId: result.Result.ID);

            var supplyFilter = Builders<Supply>.Filter.Eq("Property.ID", result.Result.ID);
            var supplyUpdate = Builders<Supply>.Update
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("IsArchived", false);

            var supplyResult = DbManager.Supply.UpdateManyAsync(supplyFilter, supplyUpdate);
            if (supplyResult.Result.MatchedCount != supplyResult.Result.ModifiedCount)
            {
                Log.WarnFormat(
                    "Property reference with id {0} in related supply(ies) could not been unarchived in unarchiving property process",
                    id);
            }
            else
            {
                Log.InfoFormat(
                    "Property reference with id {0} in related supply(ies) has been unarchived in unarchiving property process",
                    id);
            }

            return ValidationResult(Common.Util.Validation.ValidationResult.Success);
        }

        [HttpPost, Route("delete/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.ChangeState, EntityType.Property, ApplicationType.Haftdong,
            TargetState = PropertyState.Deleted)]
        public IHttpActionResult DeleteProperty(string id)
        {
            UserActivityLogUtils.SetMainActivity(targetId: ObjectId.Parse(id));

            return ValidationResult(PropertyUtil.DeleteProperty(id, false, ApplicationType.Haftdong));
        }

        [HttpPost, Route("addimage")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.Create, EntityType.PhotoInfo, ApplicationType.Haftdong)]
        public IHttpActionResult AddNewImage()
        {
            return ValidationResult(PropertyUtil.AddNewImage(Request, false));
        }

        [HttpPost, Route("deleteimage/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.Delete, EntityType.PhotoInfo, ApplicationType.Haftdong)]
        public IHttpActionResult DeleteImage(string id)
        {
            return ValidationResult(PropertyUtil.DeleteImage(id, false));
        }

        [HttpPost, Route("changecover/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.Edit, EntityType.Property, ApplicationType.Haftdong,
            ActivitySubType = "ChangingCover")]
        public IHttpActionResult ChangeCover(string id)
        {
            return ValidationResult(PropertyUtil.ChangeCover(id, false));
        }

        [HttpGet, Route("getThumbnail/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        public IHttpActionResult GetThumbnail(string id)
        {
            return new BinaryResult(PhotoGridStore.GetAttchmentBytes(AttachmentStoreEntityType.Image,
                ObjectId.Parse(id), PhotoStoreSize.Thumbnail), "image/jpeg");
        }

        [HttpGet, Route("getMediumSize/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        public IHttpActionResult GetMedium(string id)
        {
            return new BinaryResult(PhotoGridStore.GetAttchmentBytes(AttachmentStoreEntityType.Image,
                ObjectId.Parse(id), PhotoStoreSize.MediumSize), "image/jpeg");
        }

        [HttpGet, Route("getFullSize/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
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

        [HttpPost, Route("details/{id}/print")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.PrintDetail, EntityType.Property, ApplicationType.Haftdong)]
        public IHttpActionResult Print(string id, PrintPropertyInput input)
        {
            return FileUtil.Print(id, input, false);
        }

        [HttpPost, Route("printfirst")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.PrintDetail, EntityType.Property, ApplicationType.Haftdong)]
        public IHttpActionResult PrintFirst(PrintPropertyInput input)
        {
            var filter = Builders<Property>.Filter.Eq("DeletionTime", BsonNull.Value) &
                         Builders<Property>.Filter.Eq("CreatedByID",
                             ObjectId.Parse(OwinRequestScopeContext.Current.GetUserId()));
            var property = DbManager.Property.Find(filter).FirstOrDefaultAsync().Result;

            UserActivityLogUtils.SetMainActivity(targetId: property.ID);
            var report = ReportRepository.GetApplicationImplemented(
                ApplicationImplementedReportDataSourceType.Property,
                input.IfNotNull(i => i.ReportTemplateID));

            if (report == null)
                return
                    ValidationResult("ReportTemplate", ReportTemplateValidationErrors.NotFound);
            Report.PopulatePropertyDetails(report, property.ID.ToString(), false);

            StiExportFormat format;
            if (!Enum.TryParse(input.Format, true, out format))
                format = StiExportFormat.Pdf;

            return new ReportBinaryResult(report, format);
        }

        #endregion

        #region Private helper methods 

        public ValidationResult ValidateForDelete(Property property)
        {
            if (property.State != PropertyState.New)
            {
                return Common.Util.Validation.ValidationResult.Failure("Property.State",
                    PropertyValidationErrors.IsNotValid);
            }
            return Common.Util.Validation.ValidationResult.Success;
        }

        #endregion
    }
}