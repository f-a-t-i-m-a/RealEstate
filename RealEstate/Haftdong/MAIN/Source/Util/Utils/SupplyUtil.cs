using System;
using System.Collections.Generic;
using AutoMapper;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.Common.Util.Identity;
using JahanJooy.Common.Util.Owin;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstateAgency.Domain.Base;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using JahanJooy.RealEstateAgency.Domain.Property;
using JahanJooy.RealEstateAgency.Domain.Supply;
using JahanJooy.RealEstateAgency.Domain.Users;
using JahanJooy.RealEstateAgency.Domain.Workflows;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Enums;
using JahanJooy.RealEstateAgency.Util.Interfaces;
using JahanJooy.RealEstateAgency.Util.Models.Supplies;
using JahanJooy.RealEstateAgency.Util.Owin;
using JahanJooy.RealEstateAgency.Util.UserActivityLog;
using JahanJooy.RealEstateAgency.Util.Utils.ContactMethods;
using log4net;
using MongoDB.Bson;
using MongoDB.Driver;
using ServiceStack;

namespace JahanJooy.RealEstateAgency.Util.Utils
{
    [Contract]
    [Component]
    public class SupplyUtil
    {
        #region Injected dependencies

        private static readonly ILog Log = LogManager.GetLogger(typeof(SupplyUtil));

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public UserUtil UserUtil { get; set; }

        [ComponentPlug]
        public RequestUtil RequestUtil { get; set; }

        [ComponentPlug]
        public CustomerUtil CustomerUtil { get; set; }

        [ComponentPlug]
        public LocalContactMethodUtil LocalContactMethodUtil { get; set; }

        [ComponentPlug]
        public IComposer Composer { get; set; }

        private readonly decimal _mortgageToRentRate = Math.Ceiling(33M);

        #endregion

        #region Action methods

        public ValidationResult SaveSupply(Supply supply, ObjectId propertyId, bool isPublic)
        {
            PrepareSupplyForSave(supply, isPublic);

            var propertyFilter = Builders<Property>.Filter.Eq("ID", propertyId);
            var property = DbManager.Property.Find(propertyFilter).SingleOrDefaultAsync();

            if (property?.Result == null)
                return ValidationResult.Failure("Property",
                    PropertyValidationErrors.NotFound);

            var validationResult = ValidateForSave(supply, property.Result, isPublic);
            if (!validationResult.IsValid)
                return validationResult;

            supply.Property = new PropertyReference
            {
                ID = propertyId
            };

            if (!RecalculatePrices(supply, property.Result))
                return ValidationResult.Failure("Supply.Price",
                    SupplyValidationErrors.IsNotCalculated);

            try
            {
                DbManager.Supply.InsertOneAsync(supply).Wait();

                if (supply.Property.ID != ObjectId.Empty)
                {
                    var supplyReference = Mapper.Map<SupplyReference>(supply);
                    var filter = Builders<Property>.Filter.Eq("ID", propertyId);
                    var update = Builders<Property>.Update.AddToSet("Supplies", supplyReference)
                        .Set("LastIndexingTime", BsonNull.Value);
                    var result = DbManager.Property.UpdateOneAsync(filter, update);
                    if (result.Result.MatchedCount != 1 ||
                        (result.Result.MatchedCount == 1 && result.Result.ModifiedCount != 1))
                    {
                        Log.WarnFormat(
                            "Supply reference(s) in property with id {0} could not been updated in adding supply process",
                            propertyId);
                    }

                    var updatedProperty = DbManager.Property.Find(filter).SingleOrDefaultAsync();
                    var updatedPropertyReference = Mapper.Map<PropertyReference>(updatedProperty.Result);
                    var supplyFilter = Builders<Supply>.Filter.Eq("Property.ID", updatedPropertyReference.ID);
                    var supplyUpdate = Builders<Supply>.Update
                        .Set("Property", updatedPropertyReference)
                        .Set("LastIndexingTime", BsonNull.Value);
                    result = DbManager.Supply.UpdateManyAsync(supplyFilter, supplyUpdate);
                    if (result.Result.MatchedCount != result.Result.ModifiedCount)
                    {
                        Log.WarnFormat(
                            "Property reference with id {0} in related supply(s) could not been updated in adding supply process",
                            updatedPropertyReference.ID);
                    }
                }

                return ValidationResult.Success;
            }
            catch (Exception e)
            {
                Log.Error("An exception has been occured in saving Supply", e);
                return ValidationResult.Failure("Supply",
                    SupplyValidationErrors.UnexpectedError);
            }
        }

        public ValidationResult ReserveSupply(string id)
        {
            var filter = Builders<Supply>.Filter.Eq("ID", ObjectId.Parse(id));
            var update = Builders<Supply>.Update
                .Set("State", SupplyState.Reserved)
                .Set("LastIndexingTime", BsonNull.Value);

            var result = DbManager.Supply.UpdateOneAsync(filter, update);
            if (result.Result.MatchedCount != 1)
                return ValidationResult.Failure("Supply",
                    SupplyValidationErrors.NotFound);

            if (result.Result.MatchedCount == 1 && result.Result.ModifiedCount != 1)
                return ValidationResult.Failure("Supply",
                    SupplyValidationErrors.NotModified);

            var propertyFilter = Builders<Property>.Filter.Eq("Supplies.ID", ObjectId.Parse(id));
            var propertyUpdate = Builders<Property>.Update
                .Set("Supplies.$.State", SupplyState.Reserved);

            var property = DbManager.Property.FindOneAndUpdateAsync(propertyFilter, propertyUpdate).Result;
            if (property == null)
            {
                Log.WarnFormat(
                    "Supply reference with id {0} in related property(s) could not been reserved in reserving supply process",
                    id);
            }
            else
            {
                Log.InfoFormat(
                    "Supply reference(s) in related property with id {0} has been reserved in reserving supply process",
                    property.ID);
            }

            return ValidationResult.Success;
        }

        public ValidationResult CompleteSupply(ObjectId id, SupplyCompletionReason reason)
        {
            var filter = Builders<Supply>.Filter.Eq("ID", id);
            var update = Builders<Supply>.Update
                .Set("State", SupplyState.Completed)
                .Set("CompletionReason", reason)
                .Set("LastIndexingTime", BsonNull.Value);

            var result = DbManager.Supply.UpdateOneAsync(filter, update);
            if (result.Result.MatchedCount != 1)
                return ValidationResult.Failure("Supply",
                    SupplyValidationErrors.NotFound);

            if (result.Result.MatchedCount == 1 && result.Result.ModifiedCount != 1)
                return ValidationResult.Failure("Supply",
                    SupplyValidationErrors.NotModified);

            var propertyFilter = Builders<Property>.Filter.Eq("Supplies.ID", id);
            var propertyUpdate = Builders<Property>.Update
                .Set("Supplies.$.State", SupplyState.Completed);

            var property = DbManager.Property.FindOneAndUpdateAsync(propertyFilter, propertyUpdate).Result;
            if (property == null)
            {
                Log.WarnFormat(
                    "Supply reference with id {0} in related property(s) could not been completed in completing supply process",
                    id);
            }
            else
            {
                Log.InfoFormat(
                    "Supply reference(s) in related property with id {0} has been completed in completing supply process",
                    property.ID);
            }

            return ValidationResult.Success;
        }

        public SupplyDetails GetSupply(string id, bool isPublic)
        {
            var filter = Builders<Supply>.Filter.Eq("ID", ObjectId.Parse(id));
            var supply =
                DbManager.Supply.Find(filter)
                    .SingleOrDefaultAsync()
                    .Result;

            if (!AuthorizeForView(supply, isPublic))
            {
                return new SupplyDetails();
            }

            var supplyDetail = Mapper.Map<SupplyDetails>(supply);

            if (supply.IntentionOfOwner == IntentionOfOwner.ForSwap && supply.Request != null)
            {
                supplyDetail.Request = RequestUtil.GetRequest(supply.Request.ID.ToString(), isPublic);
            }

            return supplyDetail;
        }

        public ValidationResult UpdateSupply(UpdateSupplyInput input, bool isPublic)
        {
            var supplyFilter = Builders<Supply>.Filter.Eq("ID", input.ID);

            var supply = DbManager.Supply.Find(supplyFilter).SingleOrDefaultAsync().Result;

            if (!AuthorizeForEdit(supply, isPublic))
            {
                return ValidationResult.Failure("Property", GeneralValidationErrors.AccessDenied);
            }

            var newSupply = Mapper.Map<Supply>(input);

            var validationResult = ValidateForUpdate(newSupply, Mapper.Map<Property>(supply.Property));
            if (!validationResult.IsValid)
                return validationResult;

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
                    return ValidationResult.Failure("Supply.ContactInfo", GeneralValidationErrors.ValueNotSpecified);

                ownerContact = contactInfo;
                input.OwnerCanBeContacted = true;


                if (ownerContact.Phones == null || ownerContact.Phones.Count == 0)
                {
                    return ValidationResult.Failure("Supply.Phones", GeneralValidationErrors.ValueNotSpecified);
                }
            }
            else if (input.OwnerCanBeContacted.Value)
            {
                if (!isPublic && supply.Property.SourceType == SourceType.Haftdong)
                {
                    if (supply.Property?.Owner == null)
                    {
                        return ValidationResult.Failure("Property.Owner", CustomerValidationErrors.NotFound);
                    }

                    var owner = CustomerUtil.GetCustomerById(supply.Property.Owner.ID);
                    ownerContact = owner.Contact;

                    if (ownerContact.Phones == null || ownerContact.Phones.Count == 0)
                    {
                        return ValidationResult.Failure("Supply.Phones", GeneralValidationErrors.ValueNotSpecified);
                    }
                }
                else if (input.OwnerContact != null)
                {
                    ownerContact.OrganizationName = input.OwnerContact.OrganizationName;
                    ownerContact.ContactName = input.OwnerContact.ContactName;
                    input.OwnerContact.Phones.ForEach(p => LocalContactMethodUtil.MapAndAddContactMethod(ownerContact, p));
                    input.OwnerContact.Emails.ForEach(e => LocalContactMethodUtil.MapAndAddContactMethod(ownerContact, e));
                    input.OwnerContact.Addresses.ForEach(a => LocalContactMethodUtil.MapAndAddContactMethod(ownerContact, a));
                    var contactResult = LocalContactMethodUtil.PrepareContactMethods(ownerContact, true, false, false);
                    if (!contactResult.IsValid)
                        return contactResult;

                    if (ownerContact.Phones == null || ownerContact.Phones.Count == 0)
                    {
                        return ValidationResult.Failure("Supply.Phones", GeneralValidationErrors.ValueNotSpecified);
                    }
                }
            }
            else if (!input.OwnerCanBeContacted.Value && input.AgencyContact != null)
            {
                agencyContact.OrganizationName = input.AgencyContact.OrganizationName;
                agencyContact.ContactName = input.AgencyContact.ContactName;
                input.AgencyContact.Phones.ForEach(p => LocalContactMethodUtil.MapAndAddContactMethod(agencyContact, p));
                input.AgencyContact.Emails.ForEach(e => LocalContactMethodUtil.MapAndAddContactMethod(agencyContact, e));
                input.AgencyContact.Addresses.ForEach(a => LocalContactMethodUtil.MapAndAddContactMethod(agencyContact, a));
                var contactResult = LocalContactMethodUtil.PrepareContactMethods(agencyContact, true, false, false);
                if (!contactResult.IsValid)
                    return contactResult;

                if (agencyContact.Phones == null || agencyContact.Phones.Count == 0)
                {
                    return ValidationResult.Failure("Supply.Phones", GeneralValidationErrors.ValueNotSpecified);
                }
            }

            var propertyFilter = Builders<Property>.Filter.Eq("Supplies.ID", input.ID);
            var property = DbManager.Property.Find(propertyFilter).SingleOrDefaultAsync().Result;

            supply = Mapper.Map<Supply>(input);
            if (!RecalculatePrices(supply, property))
                return ValidationResult.Failure("Supply.Price",
                    SupplyValidationErrors.IsNotCalculated);

            var updateSupply = Builders<Supply>.Update
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("LastModificationTime", DateTime.Now)
                .Set("ExpirationTime", input.ExpirationTime)
                .Set("IntentionOfOwner", input.IntentionOfOwner)
                .Set("PriceSpecificationType", input.PriceSpecificationType)
                .Set("TotalPrice", supply.TotalPrice)
                .Set("PricePerEstateArea", supply.PricePerEstateArea)
                .Set("PricePerUnitArea", supply.PricePerUnitArea)
                .Set("HasTransferableLoan", input.HasTransferableLoan)
                .Set("TransferableLoanAmount", input.TransferableLoanAmount)
                .Set("Mortgage", input.Mortgage)
                .Set("Rent", input.Rent)
                .Set("OwnerCanBeContacted", input.OwnerCanBeContacted)
                .Set("OwnerContact", ownerContact)
                .Set("AgencyContact", agencyContact)
                .Set("MortgageAndRentConvertible", input.MortgageAndRentConvertible)
                .Set("AdditionalRentalComments", input.AdditionalRentalComments)
                .Set("MinimumMortgage", input.MinimumMortgage)
                .Set("MinimumRent", input.MinimumRent);

            var result = DbManager.Supply.UpdateOneAsync(supplyFilter, updateSupply);
            if (result.Result.MatchedCount != 1)
                return ValidationResult.Failure("Supply", SupplyValidationErrors.NotFound);

            if (result.Result.MatchedCount == 1 && result.Result.ModifiedCount != 1)
                return ValidationResult.Failure("Supply", SupplyValidationErrors.NotModified);

            bool? contactToAgency = null;
            bool? contactToOwner = null;
            if (input.OwnerCanBeContacted.Value)
            {
                contactToOwner = true;
            }
            else
            {
                contactToAgency = true;
            }

            var propertyUpdate = Builders<Property>.Update
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("Supplies.$.IntentionOfOwner", input.IntentionOfOwner)
                .Set("Supplies.$.ExpirationTime", input.ExpirationTime)
                .Set("Supplies.$.PriceSpecificationType", input.PriceSpecificationType)
                .Set("Supplies.$.TotalPrice", supply.TotalPrice)
                .Set("Supplies.$.PricePerEstateArea", supply.PricePerEstateArea)
                .Set("Supplies.$.PricePerUnitArea", supply.PricePerUnitArea)
                .Set("Supplies.$.Mortgage", input.Mortgage)
                .Set("Supplies.$.Rent", input.Rent)
                .Set("Supplies.$.ContactToOwner", contactToOwner)
                .Set("Supplies.$.ContactToAgency", contactToAgency)
                .Set("Supplies.$.HasTransferableLoan", input.HasTransferableLoan)
                .Set("Supplies.$.TransferableLoanAmount", input.TransferableLoanAmount)
                .Set("Supplies.$.MortgageAndRentConvertible", input.MortgageAndRentConvertible)
                .Set("Supplies.$.AdditionalRentalComments", input.AdditionalRentalComments)
                .Set("Supplies.$.MinimumMortgage", input.MinimumMortgage)
                .Set("Supplies.$.MinimumRent", input.MinimumRent);

            property = DbManager.Property.FindOneAndUpdateAsync(propertyFilter, propertyUpdate).Result;
            if (property == null)
                return ValidationResult.Failure("Property", PropertyValidationErrors.NotFound);

            UserActivityLogUtils.SetMainActivity(
                parentType: EntityType.Property,
                parentId: property.ID);

            Log.InfoFormat(
                "Supply reference(s) in related property with id {0} has been updated in updating supply process",
                property.ID);

            return ValidationResult.Success;
        }

        public ValidationResult DeleteSupply(string id, bool isPublic)
        {
            var filter = Builders<Supply>.Filter.Eq("ID", ObjectId.Parse(id));
            var supply = DbManager.Supply.Find(filter).SingleOrDefaultAsync().Result;

            if (!AuthorizeForEdit(supply, isPublic))
            {
                return ValidationResult.Failure("Property", GeneralValidationErrors.AccessDenied);
            }

            var validationResult = ValidateForDelete(supply);
            if (!validationResult.IsValid)
                return validationResult;

            var update = Builders<Supply>.Update
                .Set("DeletionTime", DateTime.Now)
                .Set("DeletedByID", ObjectId.Parse(OwinRequestScopeContext.Current.GetUserId()))
                .Set("State", SupplyState.Deleted)
                .Set("LastIndexingTime", BsonNull.Value);

            var result = DbManager.Supply.UpdateOneAsync(filter, update);
            if (result.Result.MatchedCount != 1)
                return ValidationResult.Failure("Supply", SupplyValidationErrors.NotFound);

            if (result.Result.MatchedCount == 1 && result.Result.ModifiedCount != 1)
                return ValidationResult.Failure("Supply", SupplyValidationErrors.NotModified);

            var propertyFilter = Builders<Property>.Filter.Eq("Supplies.ID", ObjectId.Parse(id));
            var propertyUpdate = Builders<Property>.Update
                .Set("Supplies.$.State", SupplyState.Deleted);

            var propertyResult = DbManager.Property.FindOneAndUpdateAsync(propertyFilter, propertyUpdate).Result;
            if (propertyResult == null)
            {
                Log.WarnFormat(
                    "Supply reference with id {0} in related property could not been deleted in deleting supply process",
                    id);
            }
            else
            {
                UserActivityLogUtils.SetMainActivity(parentType: EntityType.Property, parentId: propertyResult.ID);

                Log.InfoFormat(
                    "Supply reference(s) in related property with id {0} has been deleted in deleting supply process",
                    propertyResult.ID);
            }

            return ValidationResult.Success;
        }

        #endregion

        #region Private helper methods 

        public bool RecalculatePrices(Supply supply, Property property)
        {
            bool result = false;

            switch (supply.IntentionOfOwner)
            {
                case IntentionOfOwner.ForSwap:
                case IntentionOfOwner.ForCooperation:
                case IntentionOfOwner.ForSale:
                    if (supply.PriceSpecificationType != null)
                        switch (supply.PriceSpecificationType.Value)
                        {
                            case SalePriceSpecificationType.PerEstateArea:
                                supply.TotalPrice = (supply.PricePerEstateArea.HasValue &&
                                                     property.EstateArea.HasValue)
                                    ? property.EstateArea.Value*supply.PricePerEstateArea
                                    : null;
                                break;

                            case SalePriceSpecificationType.PerUnitArea:
                                supply.TotalPrice = (supply.PricePerUnitArea.HasValue && property.UnitArea.HasValue)
                                    ? property.UnitArea.Value*supply.PricePerUnitArea
                                    : null;
                                break;

                            case SalePriceSpecificationType.Total:
                                supply.PricePerEstateArea = (supply.TotalPrice.HasValue &&
                                                             property.EstateArea.HasValue &&
                                                             property.EstateArea.Value != decimal.Zero)
                                    ? supply.TotalPrice/property.EstateArea.Value
                                    : null;
                                supply.PricePerUnitArea = (supply.TotalPrice.HasValue && property.UnitArea.HasValue &&
                                                           property.UnitArea.Value != decimal.Zero)
                                    ? supply.TotalPrice/property.UnitArea.Value
                                    : null;
                                break;
                        }

                    if (supply.TotalPrice.HasValue)
                        supply.TotalPrice = decimal.Round(supply.TotalPrice.Value, 0);
                    if (supply.PricePerEstateArea.HasValue)
                        supply.PricePerEstateArea = decimal.Round(supply.PricePerEstateArea.Value, 0);
                    if (supply.PricePerUnitArea.HasValue)
                        supply.PricePerUnitArea = decimal.Round(supply.PricePerUnitArea.Value, 0);
                    result = true;
                    break;
                case IntentionOfOwner.ForRent:
                    if (supply.Mortgage.HasValue && supply.Rent.HasValue)
                        supply.TotalPrice = supply.Mortgage + (supply.Rent*_mortgageToRentRate);
                    result = true;
                    break;
                case IntentionOfOwner.ForFullMortgage:
                    if (supply.Mortgage.HasValue)
                        supply.TotalPrice = supply.Mortgage;
                    result = true;
                    break;
                case IntentionOfOwner.ForDailyRent:
                    if (supply.Rent.HasValue)
                        supply.TotalPrice = supply.Rent;
                    result = true;
                    break;
            }

            return result;
        }

        private void PrepareSupplyForSave(Supply supply, bool isPublic)
        {
            supply.LastIndexingTime = DateTime.Now; //For preventing indexing proccess
            supply.CreationTime = DateTime.Now;
            supply.State = SupplyState.New;
            supply.CreatedByID = ObjectId.Parse(OwinRequestScopeContext.Current.GetUserId());
            supply.LastModificationTime = DateTime.Now;
            supply.LastModifiedTimeByID = ObjectId.Parse(OwinRequestScopeContext.Current.GetUserId());
            supply.IsPublic = isPublic;
        }

        public ValidationResult ValidateForSave(Supply supply, Property property, bool isPublic)
        {
            var currentDate = DateTime.Now;

            var errors = new List<ValidationError>();
            if (supply.IntentionOfOwner == 0)
            {
                errors.Add(new ValidationError("Supply.IntentionOfOwner",
                    GeneralValidationErrors.ValueNotSpecified));
            }

            if (isPublic && supply.ExpirationTime == null || supply.ExpirationTime < currentDate)
            {
                errors.Add(new ValidationError("Supply.ExpirationTime",
                    GeneralValidationErrors.NotValid));
            }

            var allValidators = Composer.GetAllComponents<ISupplyValidatorForSave>();
            allValidators.ForEach(v =>
            {
                var result = v.Validate(supply, property);
                if (result != ValidationResult.Success)
                {
                    errors.AddRange(result.Errors);
                }
            });

            return new ValidationResult {Errors = errors};
        }

        private ValidationResult ValidateForUpdate(Supply supply, Property property)
        {
            var errors = new List<ValidationError>();
            if (supply.IntentionOfOwner == 0)
            {
                errors.Add(new ValidationError("Supply.IntentionOfOwner",
                    GeneralValidationErrors.ValueNotSpecified));
            }
            var allValidators = Composer.GetAllComponents<ISupplyValidatorForSave>();
            allValidators.ForEach(v =>
            {
                var result = v.Validate(supply, property);
                if (result != ValidationResult.Success)
                {
                    errors.AddRange(result.Errors);
                }
            });

            return new ValidationResult {Errors = errors};
        }

        public ValidationResult ValidateForDelete(Supply supply)
        {
            if (supply.State != SupplyState.New)
            {
                return ValidationResult.Failure("Supply.State", SupplyValidationErrors.IsNotValid);
            }
            return ValidationResult.Success;
        }

        public bool AuthorizeForView(Supply supply, bool publicOnly)
        {
            var currentUserId = OwinRequestScopeContext.Current.GetUserId();

            if (publicOnly)
            {
                if (!supply.IsPublic)
                {
                    return false;
                }

                if (!currentUserId.IsNullOrEmpty() &&
                    supply.CreatedByID.Equals(ObjectId.Parse(currentUserId)))
                {
                    return true;
                }

                if (supply.ExpirationTime >= DateTime.Today)
                {
                    return true;
                }

                return false;
            }

            if (JJOwinRequestScopeContextExtensions.IsAdministrator())
            {
                return true;
            }

            if (supply.Property.SourceType != SourceType.Haftdong)
            {
                return true;
            }

            if (!currentUserId.IsNullOrEmpty() && supply.CreatedByID.Equals(ObjectId.Parse(currentUserId)))
            {
                return true;
            }

            return false;
        }

        public bool AuthorizeForEdit(Supply supply, bool publicOnly)
        {
            if (publicOnly)
            {
                if (!supply.IsPublic)
                {
                    return false;
                }

                if (JJOwinRequestScopeContextExtensions.IsAdministrator())
                {
                    return true;
                }

                if (supply.CreatedByID == ObjectId.Parse(OwinRequestScopeContext.Current.GetUserId()))
                {
                    return true;
                }

                return false;
            }

            if (JJOwinRequestScopeContextExtensions.IsAdministrator())
            {
                return true;
            }

            if (supply.CreatedByID == ObjectId.Parse(OwinRequestScopeContext.Current.GetUserId()))
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}