using System;
using System.Collections.Generic;
using System.Web.Http;
using AutoMapper;
using Compositional.Composer;
using JahanJooy.Common.Util.ApiModel.Pagination;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstateAgency.Api.App.Base;
using JahanJooy.RealEstateAgency.Api.App.Models.Customer;
using JahanJooy.RealEstateAgency.Domain.Audit;
using JahanJooy.RealEstateAgency.Domain.Customers;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using JahanJooy.RealEstateAgency.Domain.Property;
using JahanJooy.RealEstateAgency.Domain.Request;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Enums;
using JahanJooy.RealEstateAgency.Util.Models.Customers;
using JahanJooy.RealEstateAgency.Util.UserActivityLog;
using JahanJooy.RealEstateAgency.Util.Utils;
using JahanJooy.RealEstateAgency.Util.Utils.ContactMethods;
using MongoDB.Bson;
using MongoDB.Driver;
using ServiceStack.Text;

namespace JahanJooy.RealEstateAgency.Api.App.Controllers
{
    [Contract]
    [Component]
    [ComponentCache(null)]
    [RoutePrefix("customers")]
    public class AppCustomerController : AppExtendedApiController
    {
        #region Injected dependencies

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public ElasticManager ElasticManager { get; set; }

        [ComponentPlug]
        public CustomerUtil CustomerUtil { get; set; }

        [ComponentPlug]
        public LocalContactMethodUtil LocalContactMethodUtil { get; set; }

        private int MaxPageSize = 10000;
        private int DefaultPageSize = 10;

        #endregion

        #region Action methods

        [HttpPost, Route("save")]
        [UserActivity(UserActivityType.Create, EntityType.Customer, ApplicationType.Haftdong)]
        public IHttpActionResult SaveCustomer(AppNewCustomerInput input)
        {
            var customer = Mapper.Map<Customer>(input);
            customer.Contact = LocalContactMethodUtil.MapContactMethods(input.ContactInfos, input.DisplayName);
            var result = CustomerUtil.SaveCustomer(customer);
            if (!result.IsValid)
                return ValidationResult(result);

            UserActivityLogUtils.SetMainActivity(targetId: customer.ID);

            return Ok(Mapper.Map<AppCustomerSummary>(customer));
        }

        [HttpPost, Route("update")]
        [UserActivity(UserActivityType.Edit, EntityType.Customer, ApplicationType.Haftdong)]
        public IHttpActionResult UpdateCustomer(AppUpdateCustomerInput input)
        {
            UserActivityLogUtils.SetMainActivity(
                targetId: input.ID);

            var filter = Builders<Customer>.Filter.Eq("ID", input.ID);
            var newCustomer = Mapper.Map<Customer>(input);

            newCustomer.Contact = LocalContactMethodUtil.MapContactMethods(input.ContactInfos, input.DisplayName);
            var contactResult = LocalContactMethodUtil.PrepareContactMethods(newCustomer.Contact, true, false, false);
            if (!contactResult.IsValid)
                return ValidationResult(contactResult);

            var validationResult = ValidateForSaveAndUpdate(newCustomer);
            if (!validationResult.IsValid)
                return ValidationResult(validationResult);

            var update = Builders<Customer>.Update
                .Set("DisplayName", input.DisplayName)
                .Set("Contact", newCustomer.Contact)
                .Set("Description", input.Description)
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("LastModificationTime", DateTime.Now);

            var result = DbManager.Customer.UpdateOneAsync(filter, update);

            if (result.Result.MatchedCount != 1)
                return ValidationResult("Customer", CustomerValidationErrors.NotFound);

            if (result.Result.MatchedCount == 1 && result.Result.ModifiedCount != 1)
                return ValidationResult("Customer", CustomerValidationErrors.NotModified);

            var propertyFilter = Builders<Property>.Filter.Eq("Owner.ID", input.ID);
            var newCustomerRefrence = Mapper.Map<CustomerReference>(newCustomer);

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
            DbManager.Property.UpdateManyAsync(propertyFilter, propertyUpdate);

            var properties = DbManager.Property.Find(propertyFilter).ToListAsync().Result;
            properties.ForEach(p =>
            {
                UserActivityLogUtils.ReportAdditionalActivity(new UserActivity
                {
                    ApplicationType = ApplicationType.Haftdong,
                    TargetType = EntityType.Property,
                    TargetID = p.ID,
                    ActivityType = UserActivityType.Edit,
                    ActivitySubType = "InEdittingCustomer"
                });
            });

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
            DbManager.Request.UpdateManyAsync(requestFilter, requestUpdate);

            var requests = DbManager.Request.Find(requestFilter).ToListAsync().Result;
            requests.ForEach(r =>
            {
                UserActivityLogUtils.ReportAdditionalActivity(new UserActivity
                {
                    ApplicationType = ApplicationType.Haftdong,
                    TargetType = EntityType.Request,
                    TargetID = r.ID,
                    ActivityType = UserActivityType.Edit,
                    ActivitySubType = "InEdittingCustomer"
                });
            });

            return Ok(new JsonObject());
        }

        [HttpGet, Route("get/{id}")]
        public AppCustomerDetails GetCustomer(string id)
        {
            var customer = CustomerUtil.GetCustomerDetailById(ObjectId.Parse(id));
            return Mapper.Map<AppCustomerDetails>(customer);
        }

        [HttpPost, Route("search")]
        [SkipUserActivityLogging]
        public AppSearchCustomersOutput Search(AppSearchCustomerInput searchInput)
        {
            if (searchInput.PageSize == 0)
                searchInput.PageSize = DefaultPageSize;
            else if (searchInput.PageSize < 0 || searchInput.PageSize > MaxPageSize)
                searchInput.PageSize = MaxPageSize;

            var input = Mapper.Map<SearchCustomerInput>(searchInput);
            var result = CustomerUtil.Search(input);
            var appCustomerSummary = new List<AppCustomerSummary>();
            result.Customers.PageItems?.ForEach(c =>
            {
                appCustomerSummary.Add(Mapper.Map<AppCustomerSummary>(c));
            });

            var appResult = new PagedListOutput<AppCustomerSummary>
            {
                PageItems = appCustomerSummary,
                PageNumber = result.Customers.PageNumber,
                TotalNumberOfItems = result.Customers.TotalNumberOfItems,
                TotalNumberOfPages = result.Customers.TotalNumberOfPages
            };

            return new AppSearchCustomersOutput
            {
                CustomerPagedList = appResult
            };
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

        #endregion
    }
}