using System;
using System.Collections.Generic;
using System.Web.Http;
using AutoMapper;
using Compositional.Composer;
using JahanJooy.Common.Util.ApiModel.Pagination;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstateAgency.Api.App.Base;
using JahanJooy.RealEstateAgency.Api.App.Models.Request;
using JahanJooy.RealEstateAgency.Domain.Customers;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Enums.Request;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using JahanJooy.RealEstateAgency.Domain.Request;
using JahanJooy.RealEstateAgency.Domain.Workflows;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Enums;
using JahanJooy.RealEstateAgency.Util.Models.Requests;
using JahanJooy.RealEstateAgency.Util.UserActivityLog;
using JahanJooy.RealEstateAgency.Util.Utils;
using JahanJooy.RealEstateAgency.Util.Utils.ContactMethods;
using MongoDB.Bson;
using ServiceStack.Text;

namespace JahanJooy.RealEstateAgency.Api.App.Controllers
{
    [Contract]
    [Component]
    [ComponentCache(null)]
    [RoutePrefix("requests")]
    public class AppRequestController : AppExtendedApiController
    {
        #region Injected dependencies

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public LocalPhoneNumberUtil LocalPhoneNumberUtil { get; set; }

        [ComponentPlug]
        public LocalContactMethodUtil LocalContactMethodUtil { get; set; }

        [ComponentPlug]
        public RequestUtil RequestUtil { get; set; }

        private int MaxPageSize = 10000;
        private int DefaultPageSize = 10;
        private readonly decimal _mortgageToRentRate = Math.Ceiling(33M);

        #endregion

        #region Action methods

        [HttpPost, Route("save")]
        [UserActivity(UserActivityType.Create, EntityType.Request, ApplicationType.Haftdong)]
        public IHttpActionResult SaveRequest(AppNewRequestInput input)
        {
            var owner = Mapper.Map<Customer>(input.Owner);
            owner.Contact = LocalContactMethodUtil.MapContactMethods(input.Owner.ContactInfos, owner.DisplayName);

            var request = Mapper.Map<Request>(input);
            var result = RequestUtil.SaveRequest(request, owner, input.IsPublic, SourceType.Haftdong, null);
            if (!result.IsValid)
                return ValidationResult(result);

            UserActivityLogUtils.SetMainActivity(targetId: request.ID);
            return Ok(new JsonObject());
        }

        [HttpPost, Route("update")]
        [UserActivity(UserActivityType.Edit, EntityType.Request, ApplicationType.Haftdong)]
        public IHttpActionResult UpdateRequest(AppUpdateRequestInput input)
        {
            UserActivityLogUtils.SetMainActivity(targetId: input.ID);

            var updateInput = Mapper.Map<UpdateRequestInput>(input);
            return ValidationResult(RequestUtil.UpdateRequest(updateInput, false));
        }

        [HttpPost, Route("delete/{id}")]
        [UserActivity(UserActivityType.ChangeState, EntityType.Request, ApplicationType.Haftdong,
           TargetState = RequestState.Deleted)]
        public IHttpActionResult DeleteRequest(string id)
        {
            UserActivityLogUtils.SetMainActivity(targetId: ObjectId.Parse(id));

            return ValidationResult(RequestUtil.DeleteRequest(id, false));
        }

        [HttpGet, Route("get/{id}")]
        public AppRequestDetails GetRequest(string id)
        {
            var request = Mapper.Map<AppRequestDetails>(RequestUtil.GetRequest(id, false));
            return request;
        }

        [HttpGet, Route("getContactInfos/{id}")]
        public AppRequestContactInfoSummary GetContactInfos(string id)
        {
            return Mapper.Map<AppRequestContactInfoSummary>(RequestUtil.GetContactInfos(id, false));
        }

        [HttpPost, Route("search")]
        [SkipUserActivityLogging]
        public AppSearchRequestOutput Search(AppSearchRequestInput searchInput)
        {
            if (searchInput.PageSize == 0)
                searchInput.PageSize = DefaultPageSize;
            else if (searchInput.PageSize < 0 || searchInput.PageSize > MaxPageSize)
                searchInput.PageSize = MaxPageSize;

            var input = Mapper.Map<SearchRequestInput>(searchInput);
            var result = RequestUtil.Search(input, false, false);

            var appRequests = new List<AppRequestSummary>();
            result.PageItems?.ForEach(r =>
            {
                appRequests.Add(Mapper.Map<AppRequestSummary>(r));
            });

            var appResult = new PagedListOutput<AppRequestSummary>
            {
                PageItems = appRequests,
                PageNumber = result.PageNumber,
                TotalNumberOfItems = result.TotalNumberOfItems,
                TotalNumberOfPages = result.TotalNumberOfPages
            };

            return new AppSearchRequestOutput
            {
                RequestPagedList = appResult
            };
        }

        #endregion

        #region Private helper methods 

        public ValidationResult ValidateForSaveAndUpdate(Request request)
        {
            var errors = new List<ValidationError>();

            if (request.UsageType == null)
            {
                errors.Add(new ValidationError("Request.UsageType",
                    RequestValidationErrors.IsNotValid));
            }

            if (request.IntentionOfCustomer == 0)
            {
                errors.Add(new ValidationError("Request.IntentionOfCustomer",
                    RequestValidationErrors.IsNotValid));
            }

            if (request.Owner == null)
            {
                errors.Add(new ValidationError("Request.Owner",
                    RequestValidationErrors.IsNotValid));
            }

            return new ValidationResult {Errors = errors};
        }

        public void RecalculatePrices(Request request)
        {
            switch (request.IntentionOfCustomer)
            {
                case IntentionOfCustomer.ForRent:
                    if (request.Mortgage.HasValue && request.Rent.HasValue)
                        request.TotalPrice = request.Mortgage + (request.Rent*_mortgageToRentRate);
                    break;
                case IntentionOfCustomer.ForFullMortgage:
                    if (request.Mortgage.HasValue)
                        request.TotalPrice = request.Mortgage;
                    break;
                case IntentionOfCustomer.ForDailyRent:
                    if (request.Rent.HasValue)
                        request.TotalPrice = request.Rent;
                    break;
            }
        }

        #endregion
    }
}