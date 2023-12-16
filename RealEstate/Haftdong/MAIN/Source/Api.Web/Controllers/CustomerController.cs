using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using Compositional.Composer;
using JahanJooy.Common.Util.Identity;
using JahanJooy.Common.Util.Owin;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstateAgency.Api.Web.Base;
using JahanJooy.RealEstateAgency.Domain.Audit;
using JahanJooy.RealEstateAgency.Domain.Base;
using JahanJooy.RealEstateAgency.Domain.Customers;
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
using JahanJooy.RealEstateAgency.Util.Models.Customers;
using JahanJooy.RealEstateAgency.Util.Owin;
using JahanJooy.RealEstateAgency.Util.UserActivityLog;
using JahanJooy.RealEstateAgency.Util.Utils;
using JahanJooy.RealEstateAgency.Util.Utils.ContactMethods;
using log4net;
using MongoDB.Bson;
using MongoDB.Driver;

namespace JahanJooy.RealEstateAgency.Api.Web.Controllers
{
    [Contract]
    [Component]
    [ComponentCache(null)]
    [RoutePrefix("customers")]
    public class CustomerController : ExtendedApiController
    {
        #region Injected dependencies

        private static readonly ILog Log = LogManager.GetLogger(typeof(CustomerController));

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public ElasticManager ElasticManager { get; set; }

        [ComponentPlug]
        public CustomerUtil CustomerUtil { get; set; }

        [ComponentPlug]
        public UserUtil UserUtil { get; set; }

        [ComponentPlug]
        public LocalPhoneNumberUtil LocalPhoneNumberUtil { get; set; }

        [ComponentPlug]
        public LocalContactMethodUtil LocalContactMethodUtil { get; set; }

        private int MaxPageSize = 10000;
        private int DefaultPageSize = 10;

        #endregion

        #region Action methods

        [HttpPost, Route("save")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.Create, EntityType.Customer, ApplicationType.Haftdong)]
        public IHttpActionResult SaveCustomer(NewCustomerInput input)
        {
            var customer = Mapper.Map<Customer>(input);

            var ownerContact = new ContactMethodCollection
            {
                Emails = new List<EmailInfo>(),
                Phones = new List<PhoneInfo>(),
                Addresses = new List<AddressInfo>(),
                ContactName = input.DisplayName
            };

            input.Contact.Phones.ForEach(p => LocalContactMethodUtil.MapAndAddContactMethod(ownerContact, p));
            input.Contact.Emails.ForEach(e => LocalContactMethodUtil.MapAndAddContactMethod(ownerContact, e));
            input.Contact.Addresses.ForEach(a => LocalContactMethodUtil.MapAndAddContactMethod(ownerContact, a));
            var contactResult = LocalContactMethodUtil.PrepareContactMethods(ownerContact, true, false, false);
            if (!contactResult.IsValid)
                return ValidationResult(contactResult);

            if (ownerContact.Phones == null || ownerContact.Phones.Count == 0)
            {
                return ValidationResult(Common.Util.Validation.ValidationResult.Failure("Customer.Phones", GeneralValidationErrors.ValueNotSpecified));
            }

            customer.Contact = ownerContact;
            var result = CustomerUtil.SaveCustomer(customer);
            if (!result.IsValid)
                return ValidationResult(result);

            UserActivityLogUtils.SetMainActivity(targetId: customer.ID);
            return Ok(customer);
        }

        [HttpPost, Route("update")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.Edit, EntityType.Customer, ApplicationType.Haftdong)]
        public IHttpActionResult UpdateCustomer(UpdateCustomerInput input)
        {
            UserActivityLogUtils.SetMainActivity(
                targetId: input.ID);

            var filter = Builders<Customer>.Filter.Eq("ID", input.ID);
            var newCustomer = Mapper.Map<Customer>(input);

            var ownerContact = new ContactMethodCollection
            {
                Emails = new List<EmailInfo>(),
                Phones = new List<PhoneInfo>(),
                Addresses = new List<AddressInfo>(),
                ContactName = input.DisplayName
            };

            input.Contact.Phones.ForEach(p => LocalContactMethodUtil.MapAndAddContactMethod(ownerContact, p));
            input.Contact.Emails.ForEach(e => LocalContactMethodUtil.MapAndAddContactMethod(ownerContact, e));
            input.Contact.Addresses.ForEach(a => LocalContactMethodUtil.MapAndAddContactMethod(ownerContact, a));
            var contactResult = LocalContactMethodUtil.PrepareContactMethods(ownerContact, true, false, false);
            if (!contactResult.IsValid)
                return ValidationResult(contactResult);

            if (ownerContact.Phones == null || ownerContact.Phones.Count == 0)
            {
                return ValidationResult(Common.Util.Validation.ValidationResult.Failure("Customer.Phones", GeneralValidationErrors.ValueNotSpecified));
            }

            var validationResult = ValidateForSaveAndUpdate(newCustomer);
            if (!validationResult.IsValid)
                return ValidationResult(validationResult);

            var update = Builders<Customer>.Update
                .Set("DisplayName", newCustomer.DisplayName)
                .Set("Age", newCustomer.Age)
                .Set("Contact", ownerContact)
                .Set("RequestCount", newCustomer.RequestCount)
                .Set("PropertyCount", newCustomer.PropertyCount)
                .Set("Description", newCustomer.Description)
                .Set("IsMarried", newCustomer.IsMarried)
                .Set("NameOfFather", newCustomer.NameOfFather)
                .Set("Identification", newCustomer.Identification)
                .Set("IssuedIn", newCustomer.IssuedIn)
                .Set("SocialSecurityNumber", newCustomer.SocialSecurityNumber)
                .Set("DateOfBirth", newCustomer.DateOfBirth)
                .Set("Deputy", newCustomer.Deputy)
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("LastModificationTime", DateTime.Now);

            var result = DbManager.Customer.UpdateOneAsync(filter, update);

            if (result.Result.MatchedCount != 1)
                return ValidationResult("Customer", CustomerValidationErrors.NotFound);

            if (result.Result.MatchedCount == 1 && result.Result.ModifiedCount != 1)
                return ValidationResult("Customer", CustomerValidationErrors.NotModified);

            newCustomer.Contact = ownerContact;
            var newCustomerRefrence = Mapper.Map<CustomerReference>(newCustomer);
            EditProperty(input, newCustomerRefrence);
            EditRequest(input, newCustomerRefrence);

            return ValidationResult(Common.Util.Validation.ValidationResult.Success);
        }

        [HttpPost, Route("archived/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.Edit, EntityType.Customer, ApplicationType.Haftdong)]
        public IHttpActionResult Archived(string id)
        {
            UserActivityLogUtils.SetMainActivity(
                targetId: ObjectId.Parse(id),
                activitySubType: "Archived"
                );

            var filter = Builders<Customer>.Filter.Eq("ID", ObjectId.Parse(id));

            var update = Builders<Customer>.Update
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("IsArchived", true);

            var result = DbManager.Customer.UpdateOneAsync(filter, update);

            if (result.Result.MatchedCount != 1)
                return ValidationResult("Customer", CustomerValidationErrors.NotFound);

            if (result.Result.MatchedCount == 1 && result.Result.ModifiedCount != 1)
                return ValidationResult("Customer", CustomerValidationErrors.NotModified);

            ArchiveProperties(id);
            ArchiveRequests(id);

            return ValidationResult(Common.Util.Validation.ValidationResult.Success);
        }

        [HttpPost, Route("unarchived/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.Edit, EntityType.Customer, ApplicationType.Haftdong)]
        public IHttpActionResult UnArchived(string id)
        {
            UserActivityLogUtils.SetMainActivity(
                targetId: ObjectId.Parse(id),
                activitySubType: "UnArchived"
                );

            var filter = Builders<Customer>.Filter.Eq("ID", ObjectId.Parse(id));

            var update = Builders<Customer>.Update
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("IsArchived", false);

            var result = DbManager.Customer.UpdateOneAsync(filter, update);

            if (result.Result.MatchedCount != 1)
                return ValidationResult("Customer", CustomerValidationErrors.NotFound);

            if (result.Result.MatchedCount == 1 && result.Result.ModifiedCount != 1)
                return ValidationResult("Customer", CustomerValidationErrors.NotModified);

            UnarchiveProperties(id);
            UnarchivedRequests(id);

            return ValidationResult(Common.Util.Validation.ValidationResult.Success);
        }

        [HttpGet, Route("get/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        public CustomerDetails GetCustomer(string id)
        {
            return CustomerUtil.GetCustomerDetailById(ObjectId.Parse(id));
        }

        [HttpPost, Route("delete/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.Delete, EntityType.Customer, ApplicationType.Haftdong)]
        public IHttpActionResult DeleteCustomer(string id)
        {
            UserActivityLogUtils.SetMainActivity(targetId: ObjectId.Parse(id));
            var filter = Builders<Customer>.Filter.Eq("ID", ObjectId.Parse(id));
            var update = Builders<Customer>.Update
                .Set("DeletionTime", DateTime.Now)
                .Set("LastIndexingTime", BsonNull.Value);

            var result = DbManager.Customer.UpdateOneAsync(filter, update);
            if (result.Result.MatchedCount != 1)
                return ValidationResult("Customer", CustomerValidationErrors.NotFound);

            if (result.Result.MatchedCount == 1 && result.Result.ModifiedCount != 1)
                return ValidationResult("Customer", CustomerValidationErrors.NotModified);

            DeleteProperties(id);
            DeleteSupplies(id);
            DeleteRequests(id);

            return ValidationResult(Common.Util.Validation.ValidationResult.Success);
        }

        [HttpGet, Route("all")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        public List<Customer> GetAllCustomer()
        {
            var currentUserId = OwinRequestScopeContext.Current.GetUserId();

            var filter = Builders<Customer>.Filter.Eq("DeletionTime", BsonNull.Value);
            if (!JJOwinRequestScopeContextExtensions.IsAdministrator())
                filter = filter & Builders<Customer>.Filter.Eq("CreatedByID", ObjectId.Parse(currentUserId));
            List<Customer> customers = DbManager.Customer.Find(filter).ToListAsync().Result;

            return customers;
        }

        [HttpGet, Route("getallcustomersbycreatedbyid/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.Administrator})]
        public List<CustomerSummary> GetAllCustomersByCreatedByID(string id)
        {
            var filter = Builders<Customer>.Filter.Eq("CreatedByID", ObjectId.Parse(id));
            List<CustomerSummary> customers =
                DbManager.Customer.Find(filter).Project(p => Mapper.Map<CustomerSummary>(p)).ToListAsync().Result;
            return customers;
        }

        [HttpPost, Route("search")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [SkipUserActivityLogging]
        public SearchCustomersOutput Search(SearchCustomerInput searchInput)
        {
            if (searchInput.PageSize == 0)
                searchInput.PageSize = DefaultPageSize;
            else if (searchInput.PageSize < 0 || searchInput.PageSize > MaxPageSize)
                searchInput.PageSize = MaxPageSize;

            return CustomerUtil.Search(searchInput);
        }

        #endregion

        #region Private helper methods 

        private ValidationResult ValidateForSaveAndUpdate(Customer customer)
        {
            if (string.IsNullOrEmpty(customer.DisplayName))
                return Common.Util.Validation.ValidationResult.Failure("Customer.DisplayName",
                    GeneralValidationErrors.ValueNotSpecified);

            return Common.Util.Validation.ValidationResult.Success;
        }

        private void ArchiveRequests(string id)
        {
            var requestFilter = Builders<Request>.Filter.Eq("Owner.ID", ObjectId.Parse(id))
                                & Builders<Request>.Filter.Eq("IsArchived", false);
            var requestUpdate = Builders<Request>.Update
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("IsArchived", true);
            var requestResult = DbManager.Request.UpdateManyAsync(requestFilter, requestUpdate);
            if (requestResult.Result.MatchedCount != requestResult.Result.ModifiedCount)
            {
                requestFilter = Builders<Request>.Filter.Eq("Owner.ID", ObjectId.Parse(id))
                                & Builders<Request>.Filter.Eq("IsArchived", false);
                var requests = DbManager.Request.Find(requestFilter).ToListAsync().Result;
                requests.ForEach(r =>
                {
                    Log.WarnFormat("Request with id {0} could not been archived in archiving customer process",
                        r.ID);
                });
            }
            else
            {
                requestFilter = Builders<Request>.Filter.Eq("Owner.ID", ObjectId.Parse(id))
                                & Builders<Request>.Filter.Eq("IsArchived", true);
                var requests = DbManager.Request.Find(requestFilter).ToListAsync().Result;
                requests.ForEach(r =>
                {
                    UserActivityLogUtils.ReportAdditionalActivity(new UserActivity
                    {
                        ApplicationType = ApplicationType.Haftdong,
                        TargetType = EntityType.Request,
                        TargetID = r.ID,
                        ActivityType = UserActivityType.Edit,
                        ActivitySubType = "SetArchived"
                    });
                });
            }
        }

        private void ArchiveSupplies(List<Property> properties)
        {
            var supplyFilter = Builders<Supply>.Filter.In("Property.ID", properties.Select(p => p.ID))
                               & Builders<Supply>.Filter.Eq("IsArchived", false);
            var supplyUpdate = Builders<Supply>.Update
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("IsArchived", true);

            var supplyResult = DbManager.Supply.UpdateManyAsync(supplyFilter, supplyUpdate);
            if (supplyResult.Result.MatchedCount != supplyResult.Result.ModifiedCount)
            {
                supplyFilter = Builders<Supply>.Filter.In("Property.ID", properties.Select(p => p.ID))
                               & Builders<Supply>.Filter.Eq("IsArchived", false);
                var supplies = DbManager.Supply.Find(supplyFilter).ToListAsync().Result;
                supplies.ForEach(s =>
                {
                    Log.WarnFormat("Supply with id {0} could not been archived in archiving customer process",
                        s.ID);
                });
            }
            else
            {
                supplyFilter = Builders<Supply>.Filter.In("Property.ID", properties.Select(p => p.ID))
                               & Builders<Supply>.Filter.Eq("IsArchived", true);
                var supplies = DbManager.Supply.Find(supplyFilter).ToListAsync().Result;
                supplies.ForEach(s =>
                {
                    UserActivityLogUtils.ReportAdditionalActivity(new UserActivity
                    {
                        ApplicationType = ApplicationType.Haftdong,
                        TargetType = EntityType.Supply,
                        TargetID = s.ID,
                        ActivityType = UserActivityType.Edit,
                        ActivitySubType = "SetArchived"
                    });
                });
            }
        }

        private void ArchiveProperties(string id)
        {
            var propertyFilter = Builders<Property>.Filter.Eq("Owner.ID", ObjectId.Parse(id))
                                 & Builders<Property>.Filter.Eq("IsArchived", false);
            var propertyUpdate = Builders<Property>.Update
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("IsArchived", true);
            var allProperties = DbManager.Property.Find(propertyFilter).ToListAsync().Result;
            var propertyResult = DbManager.Property.UpdateManyAsync(propertyFilter, propertyUpdate);
            if (propertyResult.Result.MatchedCount != propertyResult.Result.ModifiedCount)
            {
                propertyFilter = Builders<Property>.Filter.Eq("Owner.ID", ObjectId.Parse(id))
                                 & Builders<Property>.Filter.Eq("IsArchived", false);
                var properties = DbManager.Property.Find(propertyFilter).ToListAsync().Result;
                properties.ForEach(p =>
                {
                    Log.WarnFormat("Property with id {0} could not been archived in archiving customer process",
                        p.ID);
                });
            }
            else
            {
                propertyFilter = Builders<Property>.Filter.Eq("Owner.ID", ObjectId.Parse(id))
                                 & Builders<Property>.Filter.Eq("IsArchived", true);
                var properties = DbManager.Property.Find(propertyFilter).ToListAsync().Result;
                properties.ForEach(p =>
                {
                    UserActivityLogUtils.ReportAdditionalActivity(new UserActivity
                    {
                        ApplicationType = ApplicationType.Haftdong,
                        TargetType = EntityType.Property,
                        TargetID = p.ID,
                        ActivityType = UserActivityType.Edit,
                        ActivitySubType = "SetArchived"
                    });
                });
            }

            ArchiveSupplies(allProperties);
        }

        private void UnarchivedRequests(string id)
        {
            var requestFilter = Builders<Request>.Filter.Eq("Owner.ID", ObjectId.Parse(id))
                                & Builders<Request>.Filter.Eq("IsArchived", true);
            var requestUpdate = Builders<Request>.Update
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("IsArchived", false);
            var requestResult = DbManager.Request.UpdateManyAsync(requestFilter, requestUpdate);
            if (requestResult.Result.MatchedCount != requestResult.Result.ModifiedCount)
            {
                requestFilter = Builders<Request>.Filter.Eq("Owner.ID", ObjectId.Parse(id))
                                & Builders<Request>.Filter.Eq("IsArchived", true);
                var requests = DbManager.Request.Find(requestFilter).ToListAsync().Result;
                requests.ForEach(r =>
                {
                    Log.WarnFormat("Request with id {0} could not been unarchived in unarchiving customer process",
                        r.ID);
                });
            }
            else
            {
                requestFilter = Builders<Request>.Filter.Eq("Owner.ID", ObjectId.Parse(id))
                                & Builders<Request>.Filter.Eq("IsArchived", false);
                var requests = DbManager.Request.Find(requestFilter).ToListAsync().Result;
                requests.ForEach(r =>
                {
                    UserActivityLogUtils.ReportAdditionalActivity(new UserActivity
                    {
                        ApplicationType = ApplicationType.Haftdong,
                        TargetType = EntityType.Request,
                        TargetID = r.ID,
                        ActivityType = UserActivityType.Edit,
                        ActivitySubType = "UnArchived"
                    });
                });
            }
        }

        private void UnarchiveSupplies(List<Property> properties)
        {
            var supplyFilter = Builders<Supply>.Filter.In("Property.ID", properties.Select(p => p.ID))
                               & Builders<Supply>.Filter.Eq("IsArchived", true);
            var supplyUpdate = Builders<Supply>.Update
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("IsArchived", false);
            var supplyResult = DbManager.Supply.UpdateManyAsync(supplyFilter, supplyUpdate);
            if (supplyResult.Result.MatchedCount != supplyResult.Result.ModifiedCount)
            {
                supplyFilter = Builders<Supply>.Filter.In("Property.ID", properties.Select(p => p.ID))
                               & Builders<Supply>.Filter.Eq("IsArchived", true);
                var supplies = DbManager.Supply.Find(supplyFilter).ToListAsync().Result;
                supplies.ForEach(s =>
                {
                    Log.WarnFormat("Supply with id {0} could not been unarchived in unarchiving customer process",
                        s.ID);
                });
            }
            else
            {
                supplyFilter = Builders<Supply>.Filter.In("Property.ID", properties.Select(p => p.ID))
                               & Builders<Supply>.Filter.Eq("IsArchived", false);
                var supplies = DbManager.Supply.Find(supplyFilter).ToListAsync().Result;
                supplies.ForEach(s =>
                {
                    UserActivityLogUtils.ReportAdditionalActivity(new UserActivity
                    {
                        ApplicationType = ApplicationType.Haftdong,
                        TargetType = EntityType.Supply,
                        TargetID = s.ID,
                        ActivityType = UserActivityType.Edit,
                        ActivitySubType = "unArchived"
                    });
                });
            }
        }

        private void UnarchiveProperties(string id)
        {
            var propertyFilter = Builders<Property>.Filter.Eq("Owner.ID", ObjectId.Parse(id))
                                 & Builders<Property>.Filter.Eq("IsArchived", true);
            var propertyUpdate = Builders<Property>.Update
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("IsArchived", false);
            var allProperties = DbManager.Property.Find(propertyFilter).ToListAsync().Result;
            var propertyResult = DbManager.Property.UpdateManyAsync(propertyFilter, propertyUpdate);
            if (propertyResult.Result.MatchedCount != propertyResult.Result.ModifiedCount)
            {
                propertyFilter = Builders<Property>.Filter.Eq("Owner.ID", ObjectId.Parse(id))
                                 & Builders<Property>.Filter.Eq("IsArchived", true);
                var properties = DbManager.Property.Find(propertyFilter).ToListAsync().Result;
                properties.ForEach(p =>
                {
                    Log.WarnFormat("Property with id {0} could not been unarchived in unarchiving customer process",
                        p.ID);
                });
            }
            else
            {
                propertyFilter = Builders<Property>.Filter.Eq("Owner.ID", ObjectId.Parse(id))
                                 & Builders<Property>.Filter.Eq("IsArchived", false);
                var properties = DbManager.Property.Find(propertyFilter).ToListAsync().Result;
                properties.ForEach(p =>
                {
                    UserActivityLogUtils.ReportAdditionalActivity(new UserActivity
                    {
                        ApplicationType = ApplicationType.Haftdong,
                        TargetType = EntityType.Property,
                        TargetID = p.ID,
                        ActivityType = UserActivityType.Edit,
                        ActivitySubType = "UnArchived"
                    });
                });
            }

            UnarchiveSupplies(allProperties);
        }

        private void DeleteProperties(string id)
        {
            var propertyFilter = Builders<Property>.Filter.Eq("Owner.ID", ObjectId.Parse(id))
                                 & Builders<Property>.Filter.Ne("State", PropertyState.Deleted);
            var propertyUpdate = Builders<Property>.Update
                .Set("DeletionTime", DateTime.Now)
                .Set("State", PropertyState.Deleted)
                .Set("LastIndexingTime", BsonNull.Value);

            var result = DbManager.Property.UpdateManyAsync(propertyFilter, propertyUpdate);
            if (result.Result.MatchedCount != result.Result.ModifiedCount)
            {
                propertyFilter = Builders<Property>.Filter.Eq("Owner.ID", ObjectId.Parse(id))
                                 & Builders<Property>.Filter.Ne("State", PropertyState.Deleted);
                var properties = DbManager.Property.Find(propertyFilter).ToListAsync().Result;
                properties.ForEach(p =>
                {
                    Log.WarnFormat("Property with id {0} could not been deleted in deleting customer process",
                        p.ID);
                });
            }
            else
            {
                propertyFilter = Builders<Property>.Filter.Eq("Owner.ID", ObjectId.Parse(id))
                                 & Builders<Property>.Filter.Eq("State", PropertyState.Deleted);
                var properties = DbManager.Property.Find(propertyFilter).ToListAsync().Result;
                properties.ForEach(p =>
                {
                    UserActivityLogUtils.ReportAdditionalActivity(new UserActivity
                    {
                        ApplicationType = ApplicationType.Haftdong,
                        TargetType = EntityType.Property,
                        TargetID = p.ID,
                        ActivityType = UserActivityType.ChangeState,
                        TargetState = PropertyState.Deleted.ToString(),
                        ActivitySubType = "InDeletingCustomer"
                    });
                });
            }
        }

        private void DeleteSupplies(string id)
        {
            var supplyFilter = Builders<Supply>.Filter.Eq("Property.Owner.ID", ObjectId.Parse(id))
                               & Builders<Supply>.Filter.Ne("State", SupplyState.Deleted);
            var supplyUpdate = Builders<Supply>.Update
                .Set("DeletionTime", DateTime.Now)
                .Set("State", SupplyState.Deleted)
                .Set("LastIndexingTime", BsonNull.Value);

            var result = DbManager.Supply.UpdateManyAsync(supplyFilter, supplyUpdate);
            if (result.Result.MatchedCount != result.Result.ModifiedCount)
            {
                supplyFilter = Builders<Supply>.Filter.Eq("Property.Owner.ID", ObjectId.Parse(id))
                               & Builders<Supply>.Filter.Ne("State", SupplyState.Deleted);
                var supplies = DbManager.Supply.Find(supplyFilter).ToListAsync().Result;
                supplies.ForEach(s =>
                {
                    Log.WarnFormat("Supply with id {0} could not been deleted in deleting customer process",
                        s.ID);
                });
            }
            else
            {
                supplyFilter = Builders<Supply>.Filter.Eq("Property.Owner.ID", ObjectId.Parse(id))
                               & Builders<Supply>.Filter.Eq("State", SupplyState.Deleted);
                var supplies = DbManager.Supply.Find(supplyFilter).ToListAsync().Result;
                supplies.ForEach(s =>
                {
                    UserActivityLogUtils.ReportAdditionalActivity(new UserActivity
                    {
                        ApplicationType = ApplicationType.Haftdong,
                        TargetType = EntityType.Supply,
                        TargetID = s.ID,
                        ActivityType = UserActivityType.ChangeState,
                        TargetState = SupplyState.Deleted.ToString(),
                        ActivitySubType = "InDeletingCustomer"
                    });
                });
            }
        }

        private void DeleteRequests(string id)
        {
            var requestFilter = Builders<Request>.Filter.Eq("Owner.ID", ObjectId.Parse(id))
                                & Builders<Request>.Filter.Ne("State", RequestState.Deleted);
            var requestUpdate = Builders<Request>.Update
                .Set("DeletionTime", DateTime.Now)
                .Set("State", RequestState.Deleted)
                .Set("LastIndexingTime", BsonNull.Value);

            var result = DbManager.Request.UpdateManyAsync(requestFilter, requestUpdate);
            if (result.Result.MatchedCount != result.Result.ModifiedCount)
            {
                requestFilter = Builders<Request>.Filter.Eq("Owner.ID", ObjectId.Parse(id))
                                & Builders<Request>.Filter.Ne("State", RequestState.Deleted);
                var requests = DbManager.Request.Find(requestFilter).ToListAsync().Result;
                requests.ForEach(r =>
                {
                    Log.WarnFormat("Request with id {0} could not been deleted in deleting customer process",
                        r.ID);
                });
            }
            else
            {
                requestFilter = Builders<Request>.Filter.Eq("Owner.ID", ObjectId.Parse(id))
                                & Builders<Request>.Filter.Eq("State", RequestState.Deleted);
                var requests = DbManager.Request.Find(requestFilter).ToListAsync().Result;
                requests.ForEach(r =>
                {
                    UserActivityLogUtils.ReportAdditionalActivity(new UserActivity
                    {
                        ApplicationType = ApplicationType.Haftdong,
                        TargetType = EntityType.Request,
                        TargetID = r.ID,
                        ActivityType = UserActivityType.ChangeState,
                        TargetState = RequestState.Deleted.ToString(),
                        ActivitySubType = "InDeletingCustomer"
                    });
                });
            }
        }

        private void EditRequest(UpdateCustomerInput input, CustomerReference newCustomerRefrence)
        {
            var requestFilter = Builders<Request>.Filter.Eq("Owner.ID", input.ID);
            var requestUpdate = Builders<Request>.Update
                .Set("Owner.DisplayName", newCustomerRefrence.DisplayName)
                .Set("Owner.PhoneNumber", newCustomerRefrence.PhoneNumber)
                .Set("Owner.Email", newCustomerRefrence.Email)
                .Set("Owner.Address", newCustomerRefrence.Address)
                .Set("Owner.NameOfFather", newCustomerRefrence.NameOfFather)
                .Set("Owner.Identification", newCustomerRefrence.Identification)
                .Set("Owner.IssuedIn", newCustomerRefrence.IssuedIn)
                .Set("Owner.DateOfBirth", newCustomerRefrence.DateOfBirth)
                .Set("Owner.SocialSecurityNumber", newCustomerRefrence.SocialSecurityNumber);
            var result = DbManager.Request.UpdateManyAsync(requestFilter, requestUpdate).Result;
            if (result.ModifiedCount != result.MatchedCount)
            {
                Log.WarnFormat(
                    "Customer reference with id {0} in related request(s) could not been updated in updating customer process",
                    input.ID);
            }
            else
            {
                var requests = DbManager.Request.Find(requestFilter).ToListAsync().Result;
                requests.ForEach(r =>
                {
                    Log.InfoFormat(
                        "Customer reference(s) in related request(s) with id {0} has been updated in updating customer process",
                        r.ID);
                });
            }
        }

        private void EditProperty(UpdateCustomerInput input, CustomerReference newCustomerRefrence)
        {
            var propertyFilter = Builders<Property>.Filter.Eq("Owner.ID", input.ID);
            var propertyUpdate = Builders<Property>.Update
                .Set("Owner.DisplayName", newCustomerRefrence.DisplayName)
                .Set("Owner.PhoneNumber", newCustomerRefrence.PhoneNumber)
                .Set("Owner.Email", newCustomerRefrence.Email)
                .Set("Owner.Address", newCustomerRefrence.Address)
                .Set("Owner.NameOfFather", newCustomerRefrence.NameOfFather)
                .Set("Owner.Identification", newCustomerRefrence.Identification)
                .Set("Owner.IssuedIn", newCustomerRefrence.IssuedIn)
                .Set("Owner.DateOfBirth", newCustomerRefrence.DateOfBirth)
                .Set("Owner.SocialSecurityNumber", newCustomerRefrence.SocialSecurityNumber);
            var result = DbManager.Property.UpdateManyAsync(propertyFilter, propertyUpdate).Result;
            if (result.ModifiedCount != result.MatchedCount)
            {
                Log.WarnFormat(
                    "Customer reference with id {0} in related property(ies) could not been updated in updating customer process",
                    input.ID);
            }
            else
            {
                var properties = DbManager.Property.Find(propertyFilter).ToListAsync().Result;
                properties.ForEach(p =>
                {
                    Log.InfoFormat(
                        "Customer reference(s) in related property(ies) with id {0} has been updated in updating customer process",
                        p.ID);
                });
            }

            EditSupply(input, newCustomerRefrence);
        }

        private void EditSupply(UpdateCustomerInput input, CustomerReference newCustomerRefrence)
        {
            var supplyFilter = Builders<Supply>.Filter.Eq("Property.Owner.ID", input.ID);
            var supplyUpdate = Builders<Supply>.Update
                .Set("Property.Owner.DisplayName", newCustomerRefrence.DisplayName)
                .Set("Property.Owner.PhoneNumber", newCustomerRefrence.PhoneNumber)
                .Set("Property.Owner.Email", newCustomerRefrence.Email)
                .Set("Property.Owner.Address", newCustomerRefrence.Address)
                .Set("Property.Owner.NameOfFather", newCustomerRefrence.NameOfFather)
                .Set("Property.Owner.Identification", newCustomerRefrence.Identification)
                .Set("Property.Owner.IssuedIn", newCustomerRefrence.IssuedIn)
                .Set("Property.Owner.SocialSecurityNumber", newCustomerRefrence.SocialSecurityNumber)
                .Set("Property.Owner.DateOfBirth", newCustomerRefrence.DateOfBirth);
            var result = DbManager.Supply.UpdateManyAsync(supplyFilter, supplyUpdate).Result;
            if (result.ModifiedCount != result.MatchedCount)
            {
                Log.WarnFormat(
                    "Customer reference with id {0} in related supply(ies) could not been updated in updating customer process",
                    input.ID);
            }
            else
            {
                var supplies = DbManager.Supply.Find(supplyFilter).ToListAsync().Result;
                supplies.ForEach(s =>
                {
                    Log.InfoFormat(
                        "Customer reference(s) in related supply(ies) with id {0} has been updated in updating customer process",
                        s.ID);
                });
            }
        }

        #endregion
    }
}