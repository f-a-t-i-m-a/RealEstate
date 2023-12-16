using System.Collections.Generic;
using System.Web.Http;
using AutoMapper;
using Compositional.Composer;
using JahanJooy.RealEstateAgency.Api.Web.Base;
using JahanJooy.RealEstateAgency.Domain.Base;
using JahanJooy.RealEstateAgency.Domain.Customers;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Enums.User;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using JahanJooy.RealEstateAgency.Domain.Property;
using JahanJooy.RealEstateAgency.Domain.Request;
using JahanJooy.RealEstateAgency.Domain.Supply;
using JahanJooy.RealEstateAgency.Domain.Users;
using JahanJooy.RealEstateAgency.Domain.Workflows;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Enums;
using JahanJooy.RealEstateAgency.Util.Identity;
using JahanJooy.RealEstateAgency.Util.Models.Supplies;
using JahanJooy.RealEstateAgency.Util.Report;
using JahanJooy.RealEstateAgency.Util.Report.ApplicationImplemented;
using JahanJooy.RealEstateAgency.Util.UserActivityLog;
using JahanJooy.RealEstateAgency.Util.Utils;
using JahanJooy.RealEstateAgency.Util.Utils.ContactMethods;
using JahanJooy.RealEstateAgency.Util.VicinityCache;
using log4net;
using MongoDB.Bson;
using MongoDB.Driver;
using ServiceStack;

namespace JahanJooy.RealEstateAgency.Api.Web.Controllers
{
    [Contract]
    [Component]
    [ComponentCache(null)]
    [RoutePrefix("supplies")]
    public class SupplyController : ExtendedApiController
    {
        #region Injected dependencies

        private static readonly ILog Log = LogManager.GetLogger(typeof (SupplyController));

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public SupplyUtil SupplyUtil { get; set; }

        [ComponentPlug]
        public UserUtil UserUtil { get; set; }

        [ComponentPlug]
        public FileUtil FileUtil { get; set; }

        [ComponentPlug]
        public RequestUtil RequestUtil { get; set; }

        [ComponentPlug]
        public PropertyUtil PropertyUtil { get; set; }

        [ComponentPlug]
        public CustomerUtil CustomerUtil { get; set; }

        [ComponentPlug]
        public IComposer Composer { get; set; }

        [ComponentPlug]
        public SupplyReport Report { get; set; }

        [ComponentPlug]
        public ReportRepository ReportRepository { get; set; }

        [ComponentPlug]
        public LocalContactMethodUtil LocalContactMethodUtil { get; set; }

        [ComponentPlug]
        public IVicinityCache VicinityCache { get; set; }

        #endregion

        #region Action methods

        [HttpPost, Route("save")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.Create, EntityType.Supply, ApplicationType.Haftdong)]
        public IHttpActionResult Save(NewSupplyInput input)
        {
            var supply = Mapper.Map<Supply>(input);
            var request = Mapper.Map<Request>(input);

            if (supply.IntentionOfOwner == IntentionOfOwner.ForSwap)
            {
                var result = RequestUtil.ValidateForSave(request);
                if (supply.SwapAdditionalComments.IsNullOrEmpty() && !result.IsValid)
                    return ValidationResult(result);
            }

            var saveResult = SupplyUtil.SaveSupply(supply, input.PropertyId, false);

            var supplyReference = Mapper.Map<SupplyReference>(supply);

            if (supply.IntentionOfOwner == IntentionOfOwner.ForSwap && request != null && supply.SwapAdditionalComments.IsNullOrEmpty())
            {
                var propertyContactInfo = PropertyUtil.GetContactInfos(input.PropertyId.ToString(), false);
                if (propertyContactInfo?.Owner == null)
                {
                    return ValidationResult("Property.Owner", PropertyValidationErrors.NotFound);
                }
                request.Supply = supplyReference;
                var requestResult = RequestUtil.SaveRequest(request, Mapper.Map<Customer>(propertyContactInfo.Owner), false, SourceType.Haftdong, supply.ID);
                if (!requestResult.IsValid)
                {
                    return ValidationResult(requestResult);
                }
            }

            UserActivityLogUtils.SetMainActivity(
                targetId: supply.ID,
                parentType: EntityType.Property,
                parentId: input.PropertyId);

            if (!saveResult.IsValid)
                return ValidationResult(saveResult);

            return Ok(supply);
        }

        [HttpPost, Route("update")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.Edit, EntityType.Supply, ApplicationType.Haftdong)]
        public IHttpActionResult UpdateSupply(UpdateSupplyInput input)
        {
            UserActivityLogUtils.SetMainActivity(targetId: input.ID);
            return ValidationResult(SupplyUtil.UpdateSupply(input, false));
        }

        [HttpGet, Route("get/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        public SupplyDetails GetSupply(string id)
        {
            return SupplyUtil.GetSupply(id, false);
        }

        [HttpGet, Route("getcustomersupplies/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        public List<SupplySummary> GetCustomerSupplies(string id)
        {
            var supplySummaries = new List<SupplySummary>();
            var filter = Builders<Supply>.Filter.Eq("Property.Owner.ID", ObjectId.Parse(id))
                         & Builders<Supply>.Filter.Eq("State", SupplyState.New);
            var supplies =
                DbManager.Supply.Find(filter).ToListAsync().Result;

            supplies.ForEach(p =>
            {
                if (SupplyUtil.AuthorizeForView(p, false))
                {
                    supplySummaries.Add(Mapper.Map<SupplySummary>(p));
                }
            });

            return supplySummaries;
        }

        [HttpGet, Route("getallsuppliesbycreatedbyid/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.Administrator})]
        public List<SupplySummary> GetAllSuppliesByCreatedByID(string id)
        {
            var filter = Builders<Supply>.Filter.Eq("CreatedByID", ObjectId.Parse(id));

            var supplies =
                DbManager.Supply.Find(filter).Project(p => Mapper.Map<SupplySummary>(p)).ToListAsync().Result;

            return supplies;
        }

        [HttpPost, Route("archived/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.Edit, EntityType.Supply, ApplicationType.Haftdong)]
        public IHttpActionResult Archived(string id)
        {
            UserActivityLogUtils.SetMainActivity(
                targetId: ObjectId.Parse(id),
                activitySubType: "Archived"
                );

            var filter = Builders<Supply>.Filter.Eq("ID", ObjectId.Parse(id));

            var supply = DbManager.Supply.Find(filter).SingleOrDefaultAsync().Result;

            if (!SupplyUtil.AuthorizeForEdit(supply, false))
            {
                return ValidationResult("Supply", GeneralValidationErrors.AccessDenied);
            }

            var update = Builders<Supply>.Update
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("IsArchived", true);

            var result = DbManager.Supply.FindOneAndUpdateAsync(filter, update);
            if (result.Result == null)
                return ValidationResult("Supply", SupplyValidationErrors.NotModified);

            var propertyFilter = Builders<Property>.Filter.Eq("Supplies.ID", ObjectId.Parse(id));
            var propertyUpdate = Builders<Property>.Update.Set("Supplies.$.IsArchived", true);
            var propertyResult = DbManager.Property.FindOneAndUpdateAsync(propertyFilter, propertyUpdate).Result;
            if (propertyResult == null)
            {
                Log.WarnFormat(
                    "Supply reference with id {0} in related property could not been archived in archiving supply process",
                    id);
            }
            else
            {
                UserActivityLogUtils.SetMainActivity(parentType: EntityType.Property, parentId: propertyResult.ID);

                Log.InfoFormat(
                    "Supply reference(s) in related property with id {0} has been updated in archiving supply process",
                    propertyResult.ID);
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

            var filter = Builders<Supply>.Filter.Eq("ID", ObjectId.Parse(id));

            var supply = DbManager.Supply.Find(filter).SingleOrDefaultAsync().Result;

            if (!SupplyUtil.AuthorizeForEdit(supply, false))
            {
                return ValidationResult("Supply", GeneralValidationErrors.AccessDenied);
            }

            var update = Builders<Supply>.Update
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("IsArchived", false);

            var result = DbManager.Supply.FindOneAndUpdateAsync(filter, update);
            if (result.Result == null)
                ValidationResult("Supply", SupplyValidationErrors.NotModified);

            var propertyFilter = Builders<Property>.Filter.Eq("Supplies.ID", ObjectId.Parse(id));
            var propertyUpdate = Builders<Property>.Update.Set("Supplies.$.IsArchived", false);
            var propertyResult = DbManager.Property.FindOneAndUpdateAsync(propertyFilter, propertyUpdate).Result;
            if (propertyResult == null)
            {
                Log.WarnFormat(
                    "Supply reference with id {0} in related property could not been unarchived in unarchiving supply process",
                    id);
            }
            else
            {
                UserActivityLogUtils.SetMainActivity(parentType: EntityType.Property, parentId: propertyResult.ID);

                Log.InfoFormat(
                    "Supply reference(s) in related property with id {0} has been updated in unarchiving supply process",
                    propertyResult.ID);
            }
            return ValidationResult(Common.Util.Validation.ValidationResult.Success);
        }

        [HttpPost, Route("delete/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.ChangeState, EntityType.Supply, ApplicationType.Haftdong, TargetState = SupplyState.Deleted)]
        public IHttpActionResult DeleteSupply(string id)
        {
            UserActivityLogUtils.SetMainActivity(targetId: ObjectId.Parse(id));
            return ValidationResult(SupplyUtil.DeleteSupply(id, false));
        }

        [HttpPost, Route("cancel/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.ChangeState, EntityType.Supply, ApplicationType.Haftdong, TargetState = SupplyState.Canceled)]
        public IHttpActionResult CancelSupply(string id)
        {
            UserActivityLogUtils.SetMainActivity(targetId: ObjectId.Parse(id));
            var filter = Builders<Supply>.Filter.Eq("ID", ObjectId.Parse(id));

            var supply = DbManager.Supply.Find(filter).SingleOrDefaultAsync().Result;

            if (!SupplyUtil.AuthorizeForEdit(supply, false))
            {
                return ValidationResult("Supply", GeneralValidationErrors.AccessDenied);
            }

            var update = Builders<Supply>.Update
                .Set("State", SupplyState.Canceled)
                .Set("LastIndexingTime", BsonNull.Value);

            var result = DbManager.Supply.UpdateOneAsync(filter, update);
            if (result.Result.MatchedCount != 1)
                return ValidationResult("Supply", SupplyValidationErrors.NotFound);

            if (result.Result.MatchedCount == 1 && result.Result.ModifiedCount != 1)
                return ValidationResult("Supply", SupplyValidationErrors.NotModified);

            var propertyFilter = Builders<Property>.Filter.Eq("Supplies.ID", ObjectId.Parse(id));
            var propertyUpdate = Builders<Property>.Update
                .Set("Supplies.$.State", SupplyState.Canceled);

            var propertyResult = DbManager.Property.FindOneAndUpdateAsync(propertyFilter, propertyUpdate).Result;
            if (propertyResult == null)
            {
                Log.WarnFormat(
                    "Supply reference with id {0} in related property could not been canceled in canceling supply process",
                    id);
            }
            else
            {
                UserActivityLogUtils.SetMainActivity(parentType: EntityType.Property, parentId: propertyResult.ID);

                Log.InfoFormat(
                    "Supply reference(s) in related property with id {0} has been canceled in canceling supply process",
                    propertyResult.ID);
            }

            return ValidationResult(Common.Util.Validation.ValidationResult.Success);
        }

        [HttpPost, Route("complete")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.ChangeState, EntityType.Supply, ApplicationType.Haftdong, TargetState = SupplyState.Completed)]
        public IHttpActionResult CompleteSupply(CompleteSupplyInput input)
        {
            UserActivityLogUtils.SetMainActivity(targetId: ObjectId.Parse(input.ID));

            var filter = Builders<Supply>.Filter.Eq("ID", ObjectId.Parse(input.ID));
            var supply = DbManager.Supply.Find(filter).SingleOrDefaultAsync().Result;

            if (!SupplyUtil.AuthorizeForEdit(supply, false))
            {
                return ValidationResult("Supply", GeneralValidationErrors.AccessDenied);
            }

            var result = SupplyUtil.CompleteSupply(ObjectId.Parse(input.ID), input.Reason);
            if (!result.IsValid)
                return ValidationResult(result);

            return ValidationResult(Common.Util.Validation.ValidationResult.Success);
        }

        [HttpPost, Route("reserve/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.ChangeState, EntityType.Supply,ApplicationType.Haftdong, TargetState = SupplyState.Reserved)]
        public IHttpActionResult ReserveSupply(string id)
        {
            UserActivityLogUtils.SetMainActivity(targetId: ObjectId.Parse(id));

            var filter = Builders<Supply>.Filter.Eq("ID", ObjectId.Parse(id));
            var supply = DbManager.Supply.Find(filter).SingleOrDefaultAsync().Result;

            if (!SupplyUtil.AuthorizeForEdit(supply, false))
            {
                return ValidationResult("Supply", GeneralValidationErrors.AccessDenied);
            }


            var result = SupplyUtil.ReserveSupply(id);
            if (!result.IsValid)
                return ValidationResult(result);

            return ValidationResult(Common.Util.Validation.ValidationResult.Success);
        }

        [HttpPost, Route("publish")]
        [UserActivity(UserActivityType.Publish, EntityType.Supply, ApplicationType.Haftdong)]
        public IHttpActionResult Publish(PublishSupplyInput input)
        {
            UserActivityLogUtils.SetMainActivity(
                targetId: input.ID
                );

            var filter = Builders<Supply>.Filter.Eq("ID", input.ID);
            var supply = DbManager.Supply.Find(filter).SingleOrDefaultAsync().Result;

            if (!SupplyUtil.AuthorizeForEdit(supply, false))
            {
                return ValidationResult("Supply", GeneralValidationErrors.AccessDenied);
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
                if (supply?.Property == null)
                {
                    return ValidationResult("Supply.Property", SupplyValidationErrors.IsNotValid);
                }

                if (supply.Property.SourceType == SourceType.Haftdong)
                {
                    if (supply.Property.Owner == null)
                    {
                        return ValidationResult("Property.Owner", CustomerValidationErrors.NotFound);
                    }

                    var owner = CustomerUtil.GetCustomerById(supply.Property.Owner.ID);
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

            var update = Builders<Supply>.Update
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("IsPublic", true)
                .Set("OwnerCanBeContacted", input.OwnerCanBeContacted)
                .Set("OwnerContact", ownerContact)
                .Set("AgencyContact", agencyContact)
                .Set("ExpirationTime", input.ExpirationTime);

            var supplyResult = DbManager.Supply.UpdateManyAsync(filter, update);
            if (supplyResult.Result.MatchedCount != supplyResult.Result.ModifiedCount)
            {
                Log.WarnFormat(
                    "Supply with id {0} could not been published",
                    input.ID);
            }
            else
            {
                Log.InfoFormat(
                    "Supply with id {0} has been published",
                    input.ID);
            }

            var propertyFilter = Builders<Property>.Filter.Eq("Supplies.ID", input.ID);
            var propertyUpdate = Builders<Property>.Update
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("Supplies.$.IsPublic", true)
                .Set("Supplies.$.ContactToOwner", input.OwnerCanBeContacted)
                .Set("Supplies.$.ContactToAgency", !input.OwnerCanBeContacted)
                .Set("Supplies.$.ExpirationTime", input.ExpirationTime);

            var propertyResult = DbManager.Property.FindOneAndUpdateAsync(propertyFilter, propertyUpdate).Result;
            if (propertyResult == null)
            {
                Log.WarnFormat(
                    "Supply reference with id {0} in related property could not been published in publishing supply process",
                    input.ID);
            }
            else
            {
                UserActivityLogUtils.SetMainActivity(parentType: EntityType.Property, parentId: propertyResult.ID);

                Log.InfoFormat(
                    "Supply reference(s) in related property with id {0} has been published in publishing supply process",
                    propertyResult.ID);
            }

            return ValidationResult(Common.Util.Validation.ValidationResult.Success);
        }

        [HttpPost, Route("unpublish/{id}")]
        [UserActivity(UserActivityType.Unpublish, EntityType.Supply, ApplicationType.Haftdong)]
        public IHttpActionResult Unpublish(string id)
        {
            UserActivityLogUtils.SetMainActivity(
                targetId: ObjectId.Parse(id)
                );

            var filter = Builders<Supply>.Filter.Eq("ID", ObjectId.Parse(id));
            var supply = DbManager.Supply.Find(filter).SingleOrDefaultAsync().Result;

            if (!SupplyUtil.AuthorizeForEdit(supply, false))
            {
                return ValidationResult("Supply", GeneralValidationErrors.AccessDenied);
            }
            var update = Builders<Supply>.Update
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("IsPublic", false)
                .Set("ExpirationTime", BsonNull.Value);

            var supplyResult = DbManager.Supply.UpdateOneAsync(filter, update);
            if (supplyResult.Result.MatchedCount != supplyResult.Result.ModifiedCount)
            {
                Log.WarnFormat("Supply with id {0} could not been unpublished", id);
                return ValidationResult("Supply", SupplyValidationErrors.UnexpectedError);
            }

            Log.InfoFormat("Supply with id {0} has been unpublished", id);

            var propertyFilter = Builders<Property>.Filter.Eq("Supplies.ID", ObjectId.Parse(id));
            var propertyUpdate = Builders<Property>.Update
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("Supplies.$.IsPublic", false)
                .Set("Supplies.$.ExpirationTime", BsonNull.Value);

            var propertyResult = DbManager.Property.FindOneAndUpdateAsync(propertyFilter, propertyUpdate).Result;
            if (propertyResult == null)
            {
                Log.WarnFormat(
                    "Supply reference with id {0} in related property could not been unpublished in unpublishing supply process",
                    id);
            }
            else
            {
                UserActivityLogUtils.SetMainActivity(parentType: EntityType.Property, parentId: propertyResult.ID);

                Log.InfoFormat(
                    "Supply reference(s) in related property with id {0} has been unpublished in unpublishing supply process",
                    propertyResult.ID);
            }

            return ValidationResult(Common.Util.Validation.ValidationResult.Success);
        }

        [HttpPost, Route("print")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.Print, EntityType.Supply, ApplicationType.Haftdong)]
        public IHttpActionResult PrintAll(PrintSuppliesInput input)
        {
            return FileUtil.PrintAll(input, false);
        }

        #endregion

        #region Private helper methods 

        #endregion
    }
}