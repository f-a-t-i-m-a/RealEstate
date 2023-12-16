using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using Compositional.Composer;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstateAgency.Api.Web.Base;
using JahanJooy.RealEstateAgency.Domain.Base;
using JahanJooy.RealEstateAgency.Domain.Customers;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Enums.Request;
using JahanJooy.RealEstateAgency.Domain.Enums.User;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using JahanJooy.RealEstateAgency.Domain.MasterData;
using JahanJooy.RealEstateAgency.Domain.Request;
using JahanJooy.RealEstateAgency.Domain.Users;
using JahanJooy.RealEstateAgency.Domain.Workflows;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Enums;
using JahanJooy.RealEstateAgency.Util.Identity;
using JahanJooy.RealEstateAgency.Util.Models.Requests;
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
    [RoutePrefix("requests")]
    public class RequestController : ExtendedApiController
    {
        #region Injected dependencies

        private static readonly ILog Log = LogManager.GetLogger(typeof(RequestController));

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public RequestUtil RequestUtil { get; set; }

        [ComponentPlug]
        public SupplyUtil SupplyUtil { get; set; }

        [ComponentPlug]
        public CustomerUtil CustomerUtil { get; set; }

        [ComponentPlug]
        public IComposer Composer { get; set; }

        [ComponentPlug]
        public LocalPhoneNumberUtil LocalPhoneNumberUtil { get; set; }

        [ComponentPlug]
        public LocalContactMethodUtil LocalContactMethodUtil { get; set; }

        [ComponentPlug]
        public RequestReport Report { get; set; }

        [ComponentPlug]
        public ReportRepository ReportRepository { get; set; }

        [ComponentPlug]
        public UserUtil UserUtil { get; set; }

        [ComponentPlug]
        public IVicinityCache VicinityCache { get; set; }

        private int MaxPageSize = 10000;
        private int DefaultPageSize = 10;
        private readonly decimal _mortgageToRentRate = Math.Ceiling(33M);

        #endregion

        #region Action methods

        [HttpPost, Route("save")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.Create, EntityType.Request, ApplicationType.Haftdong)]
        public IHttpActionResult SaveRequest(NewRequestInput input)
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
                return
                    ValidationResult(Common.Util.Validation.ValidationResult.Failure("Customer.Phones",
                        GeneralValidationErrors.ValueNotSpecified));
            }

            owner.Contact = ownerContact;

            var request = Mapper.Map<Request>(input);

            var result = RequestUtil.SaveRequest(request, owner, false, SourceType.Haftdong, null);
            if (!result.IsValid)
                return ValidationResult(result);

            UserActivityLogUtils.SetMainActivity(targetId: request.ID, parentType: EntityType.Customer,
                parentId: input.Owner.ID);

            return Ok(request);
        }

        [HttpPost, Route("update")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.Edit, EntityType.Request, ApplicationType.Haftdong)]
        public IHttpActionResult UpdateRequest(UpdateRequestInput input)
        {
            UserActivityLogUtils.SetMainActivity(targetId: input.ID);
            if (input.Owner != null)
            {
                UserActivityLogUtils.SetMainActivity(parentType: EntityType.Customer,
                    parentId: input.Owner.ID);
            }

            return ValidationResult(RequestUtil.UpdateRequest(input, false));
        }

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
                if (RequestUtil.AuthorizeForView(p, false))
                {
                    requestSummaries.Add(Mapper.Map<RequestSummary>(p));
                }
            });

            return requestSummaries;
        }

        [HttpGet, Route("getcustomerallrequests/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        public List<RequestSummary> GetAllRequestsByCustomerID(string id)
        {
            var requestSummaries = new List<RequestSummary>();
            var filter = Builders<Request>.Filter.Eq("Owner.ID", ObjectId.Parse(id));
            var requests = DbManager.Request.Find(filter)
                .ToListAsync().Result;

            requests.ForEach(p =>
            {
                if (RequestUtil.AuthorizeForView(p, false))
                {
                    requestSummaries.Add(Mapper.Map<RequestSummary>(p));
                }
            });

            return requestSummaries;
        }

        [HttpGet, Route("getallrequestsbycreatedbyid/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        public List<RequestSummary> GetAllRequestsByCreatedByID(string id)
        {
            var filter = Builders<Request>.Filter.Eq("CreatedByID", ObjectId.Parse(id));
            var requests = DbManager.Request.Find(filter).Project(r => Mapper.Map<RequestSummary>(r))
                .ToListAsync().Result;

            return requests;
        }

        [HttpPost, Route("complete/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.ChangeState, EntityType.Request, ApplicationType.Haftdong,
            TargetState = RequestState.Compelete)]
        public IHttpActionResult CompleteRequest(string id)
        {
            UserActivityLogUtils.SetMainActivity(targetId: ObjectId.Parse(id));

            var filter = Builders<Request>.Filter.Eq("ID", ObjectId.Parse(id));
            var request = DbManager.Request.Find(filter).SingleOrDefaultAsync().Result;

            if (!RequestUtil.AuthorizeForEdit(request, false))
            {
                return ValidationResult("Request", GeneralValidationErrors.AccessDenied);
            }

            var result = RequestUtil.CompleteRequest(id);
            return ValidationResult(result);
        }

        [HttpGet, Route("get/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        public RequestDetails GetRequest(string id)
        {
            var request = RequestUtil.GetRequest(id, false);

            if (request.Supply != null)
            {
                request.Supply = SupplyUtil.GetSupply(request.Supply.ID.ToString(), false);
            }

            return request;
        }

        [HttpGet, Route("getContactInfos/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        public RequestContactInfoSummary GetContactInfos(string id)
        {
            return RequestUtil.GetContactInfos(id, false);
        }

        [HttpPost, Route("search")]
        [SkipUserActivityLogging]
        public SearchRequestOutput Search(SearchRequestInput searchInput)
        {
            if (searchInput.PageSize == 0)
                searchInput.PageSize = DefaultPageSize;
            else if (searchInput.PageSize < 0 || searchInput.PageSize > MaxPageSize)
                searchInput.PageSize = MaxPageSize;

            var result = RequestUtil.Search(searchInput, false, false);

            bool extendMenu = searchInput.IntentionOfCustomer != 0 ||
                              searchInput.PropertyType != 0;

            return new SearchRequestOutput
            {
                ExtendMenu = extendMenu,
                Requests = result
            };
        }

        [HttpPost, Route("myrequests")]
        [SkipUserActivityLogging]
        public SearchRequestOutput MyRequests(SearchRequestInput searchInput)
        {
            if (searchInput.PageSize == 0)
                searchInput.PageSize = DefaultPageSize;
            else if (searchInput.PageSize < 0 || searchInput.PageSize > MaxPageSize)
                searchInput.PageSize = MaxPageSize;

            var result = RequestUtil.Search(searchInput, false, true);

            bool extendMenu = searchInput.IntentionOfCustomer != 0 ||
                              searchInput.PropertyType != 0;

            return new SearchRequestOutput
            {
                ExtendMenu = extendMenu,
                Requests = result
            };
        }

        [HttpPost, Route("mypublicrequests")]
        [SkipUserActivityLogging]
        public SearchRequestOutput MyPublicRequests(SearchRequestInput searchInput)
        {
            searchInput.IsPublic = true;
            if (searchInput.PageSize == 0)
                searchInput.PageSize = DefaultPageSize;
            else if (searchInput.PageSize < 0 || searchInput.PageSize > MaxPageSize)
                searchInput.PageSize = MaxPageSize;

            var result = RequestUtil.Search(searchInput, false, true);

            bool extendMenu = searchInput.IntentionOfCustomer != 0 ||
                              searchInput.PropertyType != 0;

            return new SearchRequestOutput
            {
                ExtendMenu = extendMenu,
                Requests = result
            };
        }

        [HttpPost, Route("delete/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.ChangeState, EntityType.Request, ApplicationType.Haftdong,
            TargetState = RequestState.Deleted)]
        public IHttpActionResult DeleteRequest(string id)
        {
            UserActivityLogUtils.SetMainActivity(targetId: ObjectId.Parse(id));

            return ValidationResult(RequestUtil.DeleteRequest(id, false));
        }

        [HttpPost, Route("archived/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.Edit, EntityType.Request, ApplicationType.Haftdong)]
        public IHttpActionResult Archived(string id)
        {
            UserActivityLogUtils.SetMainActivity(
                targetId: ObjectId.Parse(id),
                activitySubType: "Archived"
                );

            var filter = Builders<Request>.Filter.Eq("ID", ObjectId.Parse(id));
            var request = DbManager.Request.Find(filter).SingleOrDefaultAsync().Result;

            if (!RequestUtil.AuthorizeForEdit(request, false))
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
        [UserActivity(UserActivityType.Edit, EntityType.Request, ApplicationType.Haftdong)]
        public IHttpActionResult UnArchived(string id)
        {
            UserActivityLogUtils.SetMainActivity(
                targetId: ObjectId.Parse(id),
                activitySubType: "UnArchived"
                );

            var filter = Builders<Request>.Filter.Eq("ID", ObjectId.Parse(id));
            var request = DbManager.Request.Find(filter).SingleOrDefaultAsync().Result;

            if (!RequestUtil.AuthorizeForEdit(request, false))
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

        [HttpPost, Route("publish")]
        [UserActivity(UserActivityType.Publish, EntityType.Request, ApplicationType.Haftdong)]
        public IHttpActionResult Publish(PublishRequestInput input)
        {
            UserActivityLogUtils.SetMainActivity(
                targetId: input.ID
                );

            var filter = Builders<Request>.Filter.Eq("ID", input.ID);
            var request = DbManager.Request.Find(filter).SingleOrDefaultAsync().Result;

            if (!RequestUtil.AuthorizeForEdit(request, false))
            {
                return ValidationResult("Request", GeneralValidationErrors.AccessDenied);
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
                    return ValidationResult("Request.ContactInfo", GeneralValidationErrors.ValueNotSpecified);

                ownerContact = contactInfo;
                input.OwnerCanBeContacted = true;

                if (ownerContact.Phones == null || ownerContact.Phones.Count == 0)
                {
                    return
                        ValidationResult(Common.Util.Validation.ValidationResult.Failure("Request.Phones",
                            GeneralValidationErrors.ValueNotSpecified));
                }
            }
            else if (input.OwnerCanBeContacted.Value)
            {
                if (request == null)
                {
                    return ValidationResult("Request", RequestValidationErrors.IsNotValid);
                }

                if (request.SourceType == SourceType.Haftdong)
                {
                    if (request.Owner == null)
                    {
                        return ValidationResult("Request.Owner", CustomerValidationErrors.NotFound);
                    }

                    var owner = CustomerUtil.GetCustomerById(request.Owner.ID);
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
                    return
                        ValidationResult(Common.Util.Validation.ValidationResult.Failure("Request.Phones",
                            GeneralValidationErrors.ValueNotSpecified));
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
                    return
                        ValidationResult(Common.Util.Validation.ValidationResult.Failure("Request.Phones",
                            GeneralValidationErrors.ValueNotSpecified));
                }
            }

            var update = Builders<Request>.Update
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("ExpirationTime", input.ExpirationTime)
                .Set("OwnerCanBeContacted", input.OwnerCanBeContacted)
                .Set("OwnerContact", ownerContact)
                .Set("AgencyContact", agencyContact)
                .Set("IsPublic", true);

            var result = DbManager.Request.UpdateOneAsync(filter, update);

            if (result?.Result == null)
                return ValidationResult("Request", RequestValidationErrors.NotFound);

            return ValidationResult(Common.Util.Validation.ValidationResult.Success);
        }

        [HttpPost, Route("unpublish/{id}")]
        [UserActivity(UserActivityType.Unpublish, EntityType.Request, ApplicationType.Haftdong)]
        public IHttpActionResult Unpublish(string id)
        {
            UserActivityLogUtils.SetMainActivity(
                targetId: ObjectId.Parse(id)
                );

            var filter = Builders<Request>.Filter.Eq("ID", ObjectId.Parse(id));
            var request = DbManager.Request.Find(filter).SingleOrDefaultAsync().Result;

            if (!RequestUtil.AuthorizeForEdit(request, false))
            {
                return ValidationResult("Request", GeneralValidationErrors.AccessDenied);
            }
            var update = Builders<Request>.Update
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("IsPublic", false)
                .Set("ExpirationTime", BsonNull.Value);

            var supplyResult = DbManager.Request.UpdateOneAsync(filter, update);
            if (supplyResult.Result.MatchedCount != supplyResult.Result.ModifiedCount)
            {
                Log.WarnFormat("Request with id {0} could not been unpublished", id);
                return ValidationResult("Request", RequestValidationErrors.UnexpectedError);
            }

            Log.InfoFormat("Request with id {0} has been unpublished", id);
            return ValidationResult(Common.Util.Validation.ValidationResult.Success);
        }

        [HttpPost, Route("details/{id}/print")]
        [UserActivity(UserActivityType.PrintDetail, EntityType.Request, ApplicationType.Haftdong)]
        public IHttpActionResult Print(string id, PrintRequestInput input)
        {
            UserActivityLogUtils.SetMainActivity(targetId: ObjectId.Parse(id));

            var filter = Builders<Request>.Filter.Eq("ID", ObjectId.Parse(id));
            var request = DbManager.Request.Find(filter).SingleOrDefaultAsync().Result;

            if (!RequestUtil.AuthorizeForView(request, false))
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
        [UserActivity(UserActivityType.Print, EntityType.Request, ApplicationType.Haftdong)]
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

                Report.PopulateRequestList(report, input.SearchInput, input.Ids.Select(ObjectId.Parse).ToList(), false);

                StiExportFormat format;
                if (!Enum.TryParse(input.Format, true, out format))
                    format = StiExportFormat.Pdf;

                return new ReportBinaryResult(report, format);
            }

            return ValidationResult("Request.Print", RequestValidationErrors.UnexpectedError);
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