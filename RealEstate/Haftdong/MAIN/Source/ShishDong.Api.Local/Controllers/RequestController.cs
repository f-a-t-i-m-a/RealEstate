using System.Web.Http;
using AutoMapper;
using Compositional.Composer;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Enums.User;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using JahanJooy.RealEstateAgency.Domain.Request;
using JahanJooy.RealEstateAgency.Domain.Users;
using JahanJooy.RealEstateAgency.Domain.Workflows;
using JahanJooy.RealEstateAgency.ShishDong.Api.Local.Base;
using JahanJooy.RealEstateAgency.Util.Identity;
using JahanJooy.RealEstateAgency.Util.Models.Requests;
using JahanJooy.RealEstateAgency.Util.UserActivityLog;
using JahanJooy.RealEstateAgency.Util.Utils;
using JahanJooy.RealEstateAgency.Util.Utils.ContactMethods;
using MongoDB.Bson;

namespace JahanJooy.RealEstateAgency.ShishDong.Api.Local.Controllers
{
    [Contract]
    [Component]
    [ComponentCache(null)]
    [RoutePrefix("requests")]
    public class RequestController : ExtendedApiController
    {
        #region Injected dependencies

        [ComponentPlug]
        public RequestUtil RequestUtil { get; set; }

        [ComponentPlug]
        public LocalPhoneNumberUtil LocalPhoneNumberUtil { get; set; }

        [ComponentPlug]
        public LocalContactMethodUtil LocalContactMethodUtil { get; set; }

        [ComponentPlug]
        public UserUtil UserUtil { get; set; }

        [ComponentPlug]
        public SupplyUtil SupplyUtil { get; set; }

        private int MaxPageSize = 10000;
        private int DefaultPageSize = 10;

        #endregion

        #region Action methods

        [Authorize]
        [HttpPost, Route("save")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.VerifiedUser})]
        [UserActivity(UserActivityType.Create, EntityType.Request, ApplicationType.Sheshdong)]
        public IHttpActionResult SaveRequest(NewRequestInput input)
        {
            var request = Mapper.Map<Request>(input);

            if (input.ContactInfos == null || !input.OwnerCanBeContacted.HasValue)
            {
                request.OwnerContact = null;
                request.AgencyContact = null;
            }
            else if (input.OwnerCanBeContacted.Value)
            {
                request.OwnerContact = Mapper.Map<ContactMethodCollection>(input.ContactInfos);

                request.OwnerContact?.Phones?.RemoveAll(p => string.IsNullOrEmpty(p.Value));
                request.OwnerContact?.Emails?.RemoveAll(e => string.IsNullOrEmpty(e.Value));
                request.OwnerContact?.Addresses?.RemoveAll(a => string.IsNullOrEmpty(a.Value));

                var contactResult = LocalContactMethodUtil.PrepareContactMethods(request.OwnerContact, true, false,
                    false);
                if (!contactResult.IsValid)
                    return ValidationResult(contactResult);
            }
            else
            {
                request.AgencyContact = Mapper.Map<ContactMethodCollection>(input.ContactInfos);

                request.AgencyContact?.Phones?.RemoveAll(p => string.IsNullOrEmpty(p.Value));
                request.AgencyContact?.Emails?.RemoveAll(e => string.IsNullOrEmpty(e.Value));
                request.AgencyContact?.Addresses?.RemoveAll(a => string.IsNullOrEmpty(a.Value));

                var contactResult = LocalContactMethodUtil.PrepareContactMethods(request.AgencyContact, true, false,
                    false);
                if (!contactResult.IsValid)
                    return ValidationResult(contactResult);
            }

            var result = RequestUtil.SaveRequest(request, null, true, SourceType.Sheshdong, null);

            if (!result.IsValid)
                return ValidationResult(result);

            return Ok(request);
        }

        [HttpPost, Route("search")]
        [SkipUserActivityLogging]
        public SearchRequestOutput Search(SearchRequestInput searchInput)
        {
            if (searchInput.PageSize == 0)
                searchInput.PageSize = DefaultPageSize;
            else if (searchInput.PageSize < 0 || searchInput.PageSize > MaxPageSize)
                searchInput.PageSize = MaxPageSize;

            var result = RequestUtil.Search(searchInput, true, false);

            bool extendMenu = searchInput.IntentionOfCustomer != 0 ||
                              searchInput.PropertyType != 0;

            return new SearchRequestOutput
            {
                ExtendMenu = extendMenu,
                Requests = result
            };
        }

        [HttpGet, Route("get/{id}")]
        [SkipUserActivityLogging]
        public RequestDetails GetRequest(string id)
        {
            var request=  RequestUtil.GetRequest(id, true);
            if (request.Supply != null)
            {
                request.Supply = SupplyUtil.GetSupply(request.Supply.ID.ToString(), false);
            }
            return request;
        }

        [Authorize]
        [HttpPost, Route("update")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.VerifiedUser})]
        [UserActivity(UserActivityType.Edit, EntityType.Request, ApplicationType.Sheshdong)]
        public IHttpActionResult UpdateRequest(UpdateRequestInput input)
        {
            UserActivityLogUtils.SetMainActivity(targetId: input.ID);
            return ValidationResult(RequestUtil.UpdateRequest(input, true));
        }

        [Authorize]
        [HttpPost, Route("delete/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.VerifiedUser})]
        [UserActivity(UserActivityType.ChangeState, EntityType.Request, ApplicationType.Sheshdong,
            TargetState = RequestState.Deleted)]
        public IHttpActionResult DeleteRequest(string id)
        {
            UserActivityLogUtils.SetMainActivity(targetId: ObjectId.Parse(id));
            return ValidationResult(RequestUtil.DeleteRequest(id, true));
        }

        [Authorize]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.VerifiedUser})]
        [HttpGet, Route("getContactInfos/{id}")]
        public RequestContactInfoSummaryForPublic GetContactInfos(string id)
        {
            return Mapper.Map<RequestContactInfoSummaryForPublic>(RequestUtil.GetContactInfos(id, true));
        }

        [Authorize]
        [HttpPost, Route("myrequests")]
        [ApplicationAuthorize(BuiltInRoles = new[] { BuiltInRole.VerifiedUser })]
        [SkipUserActivityLogging]
        public SearchRequestOutput MyRequests(SearchRequestInput searchInput)
        {
            if (searchInput.PageSize == 0)
                searchInput.PageSize = DefaultPageSize;
            else if (searchInput.PageSize < 0 || searchInput.PageSize > MaxPageSize)
                searchInput.PageSize = MaxPageSize;

            var result = RequestUtil.Search(searchInput, true, true);

            bool extendMenu = searchInput.IntentionOfCustomer != 0 ||
                              searchInput.PropertyType != 0;

            return new SearchRequestOutput
            {
                ExtendMenu = extendMenu,
                Requests = result
            };
        }

        /*
        [HttpGet, Route("getcustomernewrequests/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        public List<RequestSummary> GetNewRequestsByCustomerID(string id)
        {
            var requestSummaries = new List<RequestSummary>();
            var filter = Builders<Request>.Filter.Eq("Owner.ID", ObjectId.Parse(id))
                         & Builders<Request>.Filter.Eq("State", RequestState.New);
            var requests = DbManager.Request.Find(filter)
                .ToListAsync().Result;

            requests.ForEach(p =>
            {
                if (p?.CreatedByID != null &&
                    (OwinRequestScopeContext.Current.GetUser().Identity.Name.Equals("administrator") ||
                     p.CreatedByID == ObjectId.Parse(OwinRequestScopeContext.Current.GetUserId())))
                {
                    requestSummaries.Add(Mapper.Map<RequestSummary>(p));
                }
            });

            return requestSummaries;
        }

        [HttpGet, Route("getcustomerallrequests/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] { BuiltInRole.RealEstateAgent })]
        public List<RequestSummary> GetAllRequestsByCustomerID(string id)
        {
            var requestSummaries = new List<RequestSummary>();
            var filter = Builders<Request>.Filter.Eq("Owner.ID", ObjectId.Parse(id));
            var requests = DbManager.Request.Find(filter)
                .ToListAsync().Result;

            requests.ForEach(p =>
            {
                if (p?.CreatedByID != null &&
                    (OwinRequestScopeContext.Current.GetUser().Identity.Name.Equals("administrator") ||
                     p.CreatedByID == ObjectId.Parse(OwinRequestScopeContext.Current.GetUserId())))
                {
                    requestSummaries.Add(Mapper.Map<RequestSummary>(p));
                }
            });

            return requestSummaries;
        }

        [HttpGet, Route("getallrequestsbycreatedbyid/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] { BuiltInRole.RealEstateAgent })]
        public List<RequestSummary> GetAllRequestsByCreatedByID(string id)
        {
            var filter = Builders<Request>.Filter.Eq("CreatedByID", ObjectId.Parse(id));
            var requests = DbManager.Request.Find(filter).Project(r => Mapper.Map<RequestSummary>(r))
                .ToListAsync().Result;

            return requests;
        }

        [HttpPost, Route("complete/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.ChangeState, EntityType.Request, TargetState = RequestState.Compelete)]
        public IHttpActionResult CompleteRequest(string id)
        {
            UserActivityLogUtils.SetMainActivity(targetId: ObjectId.Parse(id));

            var filter = Builders<Request>.Filter.Eq("ID", ObjectId.Parse(id));
            var request = DbManager.Request.Find(filter).SingleOrDefaultAsync().Result;

            if (request?.CreatedByID == null ||
                (!OwinRequestScopeContext.Current.GetUser().Identity.Name.Equals("administrator") &&
                request.CreatedByID != ObjectId.Parse(OwinRequestScopeContext.Current.GetUserId())))
            {
                return ValidationResult("Request", GeneralValidationErrors.AccessDenied);
            }

            var result = RequestUtil.CompleteRequest(id);
            return ValidationResult(result);
        }






        [HttpPost, Route("archived/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.Edit, EntityType.Request)]
        public IHttpActionResult Archived(string id)
        {
            UserActivityLogUtils.SetMainActivity(
                targetId: ObjectId.Parse(id),
                activitySubType: "Archived"
                );

            var filter = Builders<Request>.Filter.Eq("ID", ObjectId.Parse(id));
            var request = DbManager.Request.Find(filter).SingleOrDefaultAsync().Result;

            if (request?.CreatedByID == null ||
                (!OwinRequestScopeContext.Current.GetUser().Identity.Name.Equals("administrator") &&
                request.CreatedByID != ObjectId.Parse(OwinRequestScopeContext.Current.GetUserId())))
            {
                return ValidationResult("Request", GeneralValidationErrors.AccessDenied);
            }

            var update = Builders<Request>.Update
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("IsArchived", true);

            var result = DbManager.Request.UpdateOneAsync(filter, update).Result;
            if (result.MatchedCount != 1)
                return ValidationResult("Request", RequestValidationErrors.NotFound);

            if (result.MatchedCount == 1 && result.ModifiedCount != 1)
                return ValidationResult("Request", RequestValidationErrors.NotModified);

            UserActivityLogUtils.SetMainActivity(parentType: EntityType.Customer, parentId: request.Owner?.ID);
            return ValidationResult(Common.Util.Validation.ValidationResult.Success);
        }

        [HttpPost, Route("unarchived/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.Edit, EntityType.Request)]
        public IHttpActionResult UnArchived(string id)
        {
            UserActivityLogUtils.SetMainActivity(
                targetId: ObjectId.Parse(id),
                activitySubType: "UnArchived"
                );

            var filter = Builders<Request>.Filter.Eq("ID", ObjectId.Parse(id));
            var request = DbManager.Request.Find(filter).SingleOrDefaultAsync().Result;

            if (request?.CreatedByID == null ||
                (!OwinRequestScopeContext.Current.GetUser().Identity.Name.Equals("administrator") &&
                request.CreatedByID != ObjectId.Parse(OwinRequestScopeContext.Current.GetUserId())))
            {
                return ValidationResult("Request", GeneralValidationErrors.AccessDenied);
            }

            var update = Builders<Request>.Update
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("IsArchived", false);

            var result = DbManager.Request.UpdateOneAsync(filter, update).Result;
            if (result.MatchedCount != 1)
                return ValidationResult("Request", RequestValidationErrors.NotFound);

            if (result.MatchedCount == 1 && result.ModifiedCount != 1)
                return ValidationResult("Request", RequestValidationErrors.NotModified);

            UserActivityLogUtils.SetMainActivity(parentType: EntityType.Customer, parentId: request.Owner?.ID);
            return ValidationResult(Common.Util.Validation.ValidationResult.Success);
        }

        [HttpPost, Route("details/{id}/print")]
        [UserActivity(UserActivityType.PrintDetail, EntityType.Request)]
        public IHttpActionResult Print(string id, PrintRequestInput input)
        {
            UserActivityLogUtils.SetMainActivity(targetId: ObjectId.Parse(id));

            var filter = Builders<Request>.Filter.Eq("ID", ObjectId.Parse(id));
            var request = DbManager.Request.Find(filter).SingleOrDefaultAsync().Result;

            if (request?.CreatedByID == null ||
                (!OwinRequestScopeContext.Current.GetUser().Identity.Name.Equals("administrator") &&
                request.CreatedByID != ObjectId.Parse(OwinRequestScopeContext.Current.GetUserId())))
            {
                return ValidationResult("Request", GeneralValidationErrors.AccessDenied);
            }

            var report = ReportRepository.GetApplicationImplemented(
                ApplicationImplementedReportDataSourceType.Request,
                input.IfNotNull(i => i.ReportTemplateID));

            if (report == null)
                return
                    ValidationResult("ReportTemplate", ReportTemplateValidationErrors.NotFound);
            Report.PopulateRequestDetails(report, id);

            StiExportFormat format;
            if (!Enum.TryParse(input.Format, true, out format))
                format = StiExportFormat.Pdf;

            return new ReportBinaryResult(report, format);
        }

        [HttpPost, Route("print")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.Print, EntityType.Request)]
        public IHttpActionResult PrintAll(PrintRequestsInput input)
        {
            if (input != null)
            {
                var report = ReportRepository.GetApplicationImplemented(
                    ApplicationImplementedReportDataSourceType.Requests,
                    input.IfNotNull(i => i.ReportTemplateID));

                if (report == null)
                    return
                        ValidationResult("ReportTemplate", ReportTemplateValidationErrors.NotFound);

                Report.PopulateRequestList(report, input.SearchInput, input.Ids.Select(ObjectId.Parse).ToList());

                StiExportFormat format;
                if (!Enum.TryParse(input.Format, true, out format))
                    format = StiExportFormat.Pdf;

                return new ReportBinaryResult(report, format);
            }

            return ValidationResult("Request.Print", RequestValidationErrors.UnexpectedError);
        }*/

        #endregion

        #region Private helper methods 

        /* public void RecalculatePrices(Request request)
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
         }*/

        #endregion
    }
}