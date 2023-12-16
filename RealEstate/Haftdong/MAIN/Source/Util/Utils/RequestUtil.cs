using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Compositional.Composer;
using JahanJooy.Common.Util.ApiModel.Pagination;
using JahanJooy.Common.Util.Identity;
using JahanJooy.Common.Util.Owin;
using JahanJooy.Common.Util.Text;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstateAgency.Domain.Base;
using JahanJooy.RealEstateAgency.Domain.Contract;
using JahanJooy.RealEstateAgency.Domain.Customers;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Enums.Request;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using JahanJooy.RealEstateAgency.Domain.Request;
using JahanJooy.RealEstateAgency.Domain.Supply;
using JahanJooy.RealEstateAgency.Domain.Users;
using JahanJooy.RealEstateAgency.Domain.Workflows;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Enums;
using JahanJooy.RealEstateAgency.Util.Models.Requests;
using JahanJooy.RealEstateAgency.Util.Models.Vicinities;
using JahanJooy.RealEstateAgency.Util.Owin;
using JahanJooy.RealEstateAgency.Util.Report.ApplicationImplemented;
using JahanJooy.RealEstateAgency.Util.UserActivityLog;
using JahanJooy.RealEstateAgency.Util.Utils.ContactMethods;
using JahanJooy.RealEstateAgency.Util.VicinityCache;
using log4net;
using MongoDB.Bson;
using MongoDB.Driver;
using ServiceStack;

namespace JahanJooy.RealEstateAgency.Util.Utils
{
    [Contract]
    [Component]
    public class RequestUtil
    {
        #region Injected dependencies

        private static readonly ILog Log = LogManager.GetLogger(typeof(PropertyUtil));

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public RequestReport Report { get; set; }

        [ComponentPlug]
        public UserUtil UserUtil { get; set; }

        [ComponentPlug]
        public VicinityUtil VicinityUtil { get; set; }

        [ComponentPlug]
        public LocalPhoneNumberUtil LocalPhoneNumberUtil { get; set; }

        [ComponentPlug]
        public LocalContactMethodUtil LocalContactMethodUtil { get; set; }

        [ComponentPlug]
        public CustomerUtil CustomerUtil { get; set; }

        [ComponentPlug]
        public IVicinityCache VicinityCache { get; set; }

        private readonly decimal _mortgageToRentRate = Math.Ceiling(33M);

        #endregion

        #region Action methods

        public ValidationResult SaveRequest(Request request, Customer customer, bool isPublic, SourceType sourceType,
            ObjectId? supplyId)
        {
            if (!isPublic)
            {
                var ownerResult = CustomerUtil.GetCustomerByCustomerDetails(customer);
                if (!ownerResult.IsValid)
                {
                    return ownerResult;
                }
                request.Owner = ownerResult.Result;
            }
            else
            {
                if (request.AgencyContact == null && request.OwnerContact == null)
                {
                    var contactInfo = UserUtil.GetContactInfoOfCurrentUser();
                    if (contactInfo == null)
                        return ValidationResult.Failure("Request.ContactInfo", GeneralValidationErrors.ValueNotSpecified);

                    request.OwnerCanBeContacted = true;
                    request.OwnerContact = contactInfo;
                }

                if (request.OwnerCanBeContacted.HasValue && !request.OwnerCanBeContacted.Value &&
                    (request.AgencyContact?.Phones == null || request.AgencyContact?.Phones.Count == 0))
                {
                    return ValidationResult.Failure("Request.Phones", GeneralValidationErrors.ValueNotSpecified);
                }

                if (request.OwnerCanBeContacted.HasValue && request.OwnerCanBeContacted.Value &&
                    (request.OwnerContact?.Phones == null || request.OwnerContact?.Phones.Count == 0))
                {
                    return ValidationResult.Failure("Request.Phones", GeneralValidationErrors.ValueNotSpecified);
                }
            }

            var saveValidationResult = ValidateForSave(request);
            if (!saveValidationResult.IsValid)
                return saveValidationResult;

            var validationResult = ValidateForSaveAndUpdate(request, isPublic);
            if (!validationResult.IsValid)
                return validationResult;

            PrepareRequest(request, isPublic, sourceType);
            DbManager.Request.InsertOneAsync(request).Wait();

            if (request.Owner != null)
            {
                var filter = Builders<Customer>.Filter.Eq("ID", request.Owner.ID);
                var update =
                    Builders<Customer>.Update.Inc("RequestCount", +1)
                        .Set("LastIndexingTime", BsonNull.Value)
                        .Set("LastVisitTime", DateTime.Now);
                var result = DbManager.Customer.UpdateOneAsync(filter, update);
                if (result.Result.MatchedCount != 1 ||
                    (result.Result.MatchedCount == 1 && result.Result.ModifiedCount != 1))
                {
                    Log.WarnFormat(
                        "Request count in customer with id {0} could not been increased in adding request process",
                        request.Owner.ID);
                }
            }

            if (supplyId != null && supplyId != ObjectId.Empty)
            {
                var requestReference = Mapper.Map<RequestReference>(request);
                var filter = Builders<Supply>.Filter.Eq("ID", supplyId);
                var update = Builders<Supply>.Update.Set("Request", requestReference)
                    .Set("LastIndexingTime", BsonNull.Value);
                var result = DbManager.Supply.UpdateOneAsync(filter, update);
                if (result.Result.MatchedCount != 1 ||
                    (result.Result.MatchedCount == 1 && result.Result.ModifiedCount != 1))
                {
                    Log.WarnFormat(
                        "Request reference(s) in supply with id {0} could not been updated in adding request process",
                        supplyId);
                }
            }

            return ValidationResult.Success;
        }

        public PagedListOutput<RequestSummary> Search(SearchRequestInput searchInput, bool publicOnly, bool? mineOnly)
        {
            var response = Report.GetResultFromElastic(searchInput, publicOnly, mineOnly);
            var ids = response.Documents.Select(d => ObjectId.Parse(d.ID)).ToList();
            var result = new PagedListOutput<RequestSummary>();

            if (ids.Count != 0)
            {
                var filter = Builders<Request>.Filter.In("ID", ids);
                List<Request> requests = DbManager.Request.Find(filter)
                    .ToListAsync().Result;

                List<RequestSummary> requestSummaries = new List<RequestSummary>();

                requests.ForEach(r =>
                {
                    string vicinityCompleteName;
                    var vicinityIds = r.Vicinities?.ToList();
                    var summary = Mapper.Map<RequestSummary>(r);
                    summary.Vicinities = new List<string>();
                    summary.CreatorFullName = UserUtil.GetUserName(r.CreatedByID);
                    vicinityIds?.ForEach(v =>
                    {
                        vicinityCompleteName = VicinityUtil.GetFullName(v);
                        if (!vicinityCompleteName.IsNullOrWhitespace())
                            summary.Vicinities.Add(vicinityCompleteName);
                    });

                    requestSummaries.Add(summary);
                });

                List<RequestSummary> sortedRequests = new List<RequestSummary>();
                ids.ForEach(id => sortedRequests.Add(requestSummaries.SingleOrDefault(pr => pr.ID == id)));
                sortedRequests.RemoveAll(s => s == null);

                result = new PagedListOutput<RequestSummary>
                {
                    PageItems = sortedRequests,
                    PageNumber = (searchInput.StartIndex/searchInput.PageSize) + 1,
                    TotalNumberOfItems = (int) response.Total,
                    TotalNumberOfPages = (int) Math.Ceiling((decimal) response.Total/searchInput.PageSize)
                };
            }

            return result;
        }

        public RequestDetails GetRequest(string id, bool publicOnly)
        {
            var filter = Builders<Request>.Filter.Eq("ID", ObjectId.Parse(id));
            Request request =
                DbManager.Request.Find(filter)
                    .SingleOrDefaultAsync()
                    .Result;
            if (!AuthorizeForView(request, publicOnly))
            {
                return new RequestDetails();
            }

            var requestDetail = Mapper.Map<RequestDetails>(request);
            var vicinityIds = request.Vicinities.ToList();
            requestDetail.Vicinities = new List<string>();

            vicinityIds.ForEach(v =>
            {
                var vicinityCompleteName = VicinityUtil.GetFullName(v);
                if (!vicinityCompleteName.IsNullOrWhitespace())
                    requestDetail.Vicinities.Add(vicinityCompleteName);
            });

            List<VicinitySummary> vicinityList =
                request.Vicinities.Select(vicinity => Mapper.Map<VicinitySummary>(VicinityCache[vicinity])).ToList();
            requestDetail.SelectedVicinities = vicinityList;

            return requestDetail;
        }

        public ValidationResult UpdateRequest(UpdateRequestInput input, bool publicOnly)
        {
            var filter = Builders<Request>.Filter.Eq("ID", input.ID);

            var request = DbManager.Request.Find(filter).SingleOrDefaultAsync().Result;

            if (!AuthorizeForEdit(request, publicOnly))
            {
                return ValidationResult.Failure("Request", GeneralValidationErrors.AccessDenied);
            }

            var newRequest = Mapper.Map<Request>(input);

            var validationResult = ValidateForSaveAndUpdate(newRequest, publicOnly);
            if (!validationResult.IsValid)
                return validationResult;

            newRequest.IntentionOfCustomer = request.IntentionOfCustomer;
            RecalculatePrices(newRequest);

            CustomerReference ownerReference = null;
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

            if (request.SourceType == SourceType.Haftdong)
            {
                var owner = Mapper.Map<Customer>(input.Owner);
                var originOwnerContact = new ContactMethodCollection
                {
                    Emails = new List<EmailInfo>(),
                    Phones = new List<PhoneInfo>(),
                    Addresses = new List<AddressInfo>(),
                    ContactName = input.Owner.DisplayName
                };

                input.Owner?.Contact?.Phones?.ForEach(p => LocalContactMethodUtil.MapAndAddContactMethod(originOwnerContact, p));
                var contactResult = LocalContactMethodUtil.PrepareContactMethods(originOwnerContact, true, false, false);
                if (!contactResult.IsValid)
                    return contactResult;

                if (originOwnerContact.Phones == null || originOwnerContact.Phones.Count == 0)
                {
                    return ValidationResult.Failure("Customer.Phones", GeneralValidationErrors.ValueNotSpecified);
                }

                owner.Contact = originOwnerContact;
                var ownerResult = CustomerUtil.GetCustomerByCustomerDetails(owner);
                if (!ownerResult.IsValid)
                {
                    return ownerResult;
                }
                ownerReference = ownerResult.Result;
            }

            if (!input.OwnerCanBeContacted.HasValue)
            {
                var contactInfo = UserUtil.GetContactInfoOfCurrentUser();
                if (contactInfo == null)
                    return ValidationResult.Failure("Request.ContactInfo", GeneralValidationErrors.ValueNotSpecified);

                ownerContact = contactInfo;
                input.OwnerCanBeContacted = true;


                if (ownerContact.Phones == null || ownerContact.Phones.Count == 0)
                {
                    return ValidationResult.Failure("Request.Phones", GeneralValidationErrors.ValueNotSpecified);
                }
            }
            else if (input.OwnerCanBeContacted.Value)
            {
                if (!publicOnly && request.SourceType == SourceType.Haftdong)
                {
                    if (request.Owner == null)
                    {
                        return ValidationResult.Failure("Request.Owner", CustomerValidationErrors.NotFound);
                    }

                    var owner = CustomerUtil.GetCustomerById(request.Owner.ID);
                    ownerContact = owner.Contact;

                    if (ownerContact.Phones == null || ownerContact.Phones.Count == 0)
                    {
                        return ValidationResult.Failure("Request.Phones", GeneralValidationErrors.ValueNotSpecified);
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
                    return ValidationResult.Failure("Request.Phones", GeneralValidationErrors.ValueNotSpecified);
                }
            }

            var update = Builders<Request>.Update
                .Set("PropertyTypes", input.PropertyTypes)
                .Set("Vicinities", input.Vicinities)
                .Set("Description", input.Description)
                .Set("EstateArea", input.EstateArea)
                .Set("EstateVoucherType", input.EstateVoucherType)
                .Set("BuildingAgeYears", input.BuildingAgeYears)
                .Set("TotalNumberOfUnits", input.TotalNumberOfUnits)
                .Set("NumberOfUnitsPerFloor", input.NumberOfUnitsPerFloor)
                .Set("TotalNumberOfFloors", input.TotalNumberOfFloors)
                .Set("UnitArea", input.UnitArea)
                .Set("OfficeArea", input.OfficeArea)
                .Set("CeilingHeight", input.CeilingHeight)
                .Set("NumberOfRooms", input.NumberOfRooms)
                .Set("NumberOfParkings", input.NumberOfParkings)
                .Set("UnitFloorNumber", input.UnitFloorNumber)
                .Set("IsDublex", input.IsDublex)
                .Set("HasBeenReconstructed", input.HasBeenReconstructed)
                .Set("HasIranianLavatory", input.HasIranianLavatory)
                .Set("HasForeignLavatory", input.HasForeignLavatory)
                .Set("HasPrivatePatio", input.HasPrivatePatio)
                .Set("HasMasterBedroom", input.HasMasterBedroom)
                .Set("HasElevator", input.HasElevator)
                .Set("HasGatheringHall", input.HasGatheringHall)
                .Set("HasSwimmingPool", input.HasSwimmingPool)
                .Set("HasStorageRoom", input.HasStorageRoom)
                .Set("HasAutomaticParkingDoor", input.HasAutomaticParkingDoor)
                .Set("HasVideoEyePhone", input.HasVideoEyePhone)
                .Set("HasSauna", input.HasSauna)
                .Set("HasJacuzzi", input.HasJacuzzi)
                .Set("TotalPrice", newRequest.TotalPrice)
                .Set("Mortgage", input.Mortgage)
                .Set("Rent", input.Rent)
                .Set("MortgageAndRentConvertible", input.MortgageAndRentConvertible)
                .Set("Owner", ownerReference)
                .Set("OwnerCanBeContacted", input.OwnerCanBeContacted)
                .Set("OwnerContact", ownerContact)
                .Set("AgencyContact", agencyContact)
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("ExpirationTime", input.ExpirationTime)
                .Set("LastModificationTime", DateTime.Now);

            var result = DbManager.Request.UpdateOneAsync(filter, update);
            if (result.Result.MatchedCount != 1)
                return ValidationResult.Failure("Request", RequestValidationErrors.NotFound);

            if (result.Result.MatchedCount == 1 && result.Result.ModifiedCount != 1)
                return ValidationResult.Failure("Request", RequestValidationErrors.NotModified);

            var contractFilter = Builders<Contract>.Filter.Eq("RequestReference.ID", input.ID);
            var contractUpdate = Builders<Contract>.Update
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("RequestReference.PropertyTypes", input.PropertyTypes)
                .Set("RequestReference.ExpirationTime", input.ExpirationTime)
                .Set("RequestReference.Vicinities", input.Vicinities)
                .Set("RequestReference.TotalPrice", newRequest.TotalPrice)
                .Set("RequestReference.Mortgage", input.Mortgage)
                .Set("RequestReference.Rent", input.Rent);

            result = DbManager.Contract.UpdateManyAsync(contractFilter, contractUpdate);
            if (result.Result.MatchedCount != result.Result.ModifiedCount)
            {
                Log.WarnFormat(
                    "Request reference with id {0} in related contract(s) could not been updated in updating request process",
                    input.ID);
            }

            Log.InfoFormat(
                "Request reference with id {0} in related contract(s) has been updated in updating request process",
                input.ID);

            var supplyFilter = Builders<Supply>.Filter.Eq("Request.ID", input.ID);
            var supplyUpdate = Builders<Supply>.Update
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("Request.PropertyTypes", input.PropertyTypes)
                .Set("Request.ExpirationTime", input.ExpirationTime)
                .Set("Request.Vicinities", input.Vicinities)
                .Set("Request.TotalPrice", newRequest.TotalPrice)
                .Set("Request.Mortgage", input.Mortgage)
                .Set("Request.Rent", input.Rent);

            result = DbManager.Supply.UpdateManyAsync(supplyFilter, supplyUpdate);
            if (result.Result.MatchedCount != result.Result.ModifiedCount)
            {
                Log.WarnFormat(
                    "Request reference with id {0} in related supply(ies) could not been updated in updating request process",
                    input.ID);
            }

            Log.InfoFormat(
                "Request reference with id {0} in related supply(ies) has been updated in updating request process",
                input.ID);

            return ValidationResult.Success;
        }

        public RequestContactInfoSummary GetContactInfos(string requestId, bool publicOnly)
        {
            var contactInfo = new RequestContactInfoSummary();

            if (!publicOnly || OwinRequestScopeContext.Current.GetUserId() != null)
            {
                var filter = Builders<Request>.Filter.Eq("ID", ObjectId.Parse(requestId));
                var request = DbManager.Request.Find(filter).SingleOrDefaultAsync();

                if (request?.Result == null ||
                    (publicOnly && !request.Result.IsPublic))
                {
                    return contactInfo;
                }

                contactInfo = Mapper.Map<RequestContactInfoSummary>(request.Result);
                if (contactInfo.OwnerCanBeContacted.HasValue && !contactInfo.OwnerCanBeContacted.Value)
                {
                    contactInfo.OwnerContact = null;
                }
            }

            return contactInfo;
        }

        public ValidationResult DeleteRequest(string id, bool publicOnly)
        {
            var filter = Builders<Request>.Filter.Eq("ID", ObjectId.Parse(id));

            var request = DbManager.Request.Find(filter).SingleOrDefaultAsync().Result;

            if (!AuthorizeForEdit(request, publicOnly))
            {
                return ValidationResult.Failure("Request", GeneralValidationErrors.AccessDenied);
            }

            if (request.Owner != null)
            {
                UserActivityLogUtils.SetMainActivity(parentType: EntityType.Customer, parentId: request.Owner.ID);
            }

            var validationResult = ValidateForDelete(request);
            if (!validationResult.IsValid)
                return validationResult;

            var update = Builders<Request>.Update
                .Set("DeletionTime", DateTime.Now)
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("DeletedByID", ObjectId.Parse(OwinRequestScopeContext.Current.GetUserId()))
                .Set("State", RequestState.Deleted);

            var result = DbManager.Request.UpdateOneAsync(filter, update);
            if (result.Result.MatchedCount != 1)
                return ValidationResult.Failure("Request", RequestValidationErrors.NotFound);

            if (result.Result.MatchedCount == 1 && result.Result.ModifiedCount != 1)
                return ValidationResult.Failure("Request", RequestValidationErrors.NotModified);

            if (request.Owner != null)
            {
                var filterCustomer = Builders<Customer>.Filter.Eq("ID", request.Owner.ID);
                var updateCustomer =
                    Builders<Customer>.Update.Inc("RequestCount", -1)
                        .Set("LastIndexingTime", BsonNull.Value)
                        .Set("LastVisitTime", DateTime.Now);
                var resultUpdateCustomer = DbManager.Customer.UpdateOneAsync(filterCustomer, updateCustomer);

                if (resultUpdateCustomer.Result.MatchedCount != 1)
                    return ValidationResult.Failure("Customer", RequestValidationErrors.NotFound);

                if (resultUpdateCustomer.Result.MatchedCount == 1 && resultUpdateCustomer.Result.ModifiedCount != 1)
                    return ValidationResult.Failure("Customer",
                        RequestValidationErrors.NotModified);
            }

            var contractFilter = Builders<Contract>.Filter.Eq("RequestReference.ID", ObjectId.Parse(id));
            var contract = DbManager.Contract.Find(contractFilter).SingleOrDefaultAsync().Result;

            if (contract != null)
            {
                var updateContract =
                    Builders<Contract>.Update
                        .Set("RequestReference.State", RequestState.Deleted);

                var resultUpdateContract = DbManager.Contract.UpdateOneAsync(contractFilter, updateContract);

                if (resultUpdateContract.Result.MatchedCount != 1)
                    return ValidationResult.Failure("Contract", RequestValidationErrors.NotFound);

                if (resultUpdateContract.Result.MatchedCount == 1 && resultUpdateContract.Result.ModifiedCount != 1)
                    return ValidationResult.Failure("Contract",
                        RequestValidationErrors.NotModified);
            }

            return ValidationResult.Success;
        }

        public ValidationResult CompleteRequest(string id)
        {
            var filter = Builders<Request>.Filter.Eq("ID", ObjectId.Parse(id));

            var request = DbManager.Request.Find(filter).SingleOrDefaultAsync().Result;
            var validationResult = ValidateForCompletion(request);
            if (!validationResult.IsValid)
                return validationResult;

            var update = Builders<Request>.Update
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("State", RequestState.Compelete);

            var result = DbManager.Request.UpdateOneAsync(filter, update);
            if (result.Result.MatchedCount != 1 || result.Result.ModifiedCount != 1)
                return ValidationResult.Failure("Request",
                    GeneralValidationErrors.NotModified);

            return ValidationResult.Success;
        }

        #endregion

        #region Private helper methods 

        private void PrepareRequest(Request request, bool isPublic, SourceType sourceType)
        {
            request.LastIndexingTime = null;
            request.CreationTime = DateTime.Now;
            request.State = RequestState.New;
            request.CreatedByID = ObjectId.Parse(OwinRequestScopeContext.Current.GetUserId());
            request.LastModificationTime = DateTime.Now;
            request.LastModifiedTimeByID = ObjectId.Parse(OwinRequestScopeContext.Current.GetUserId());
            request.IsPublic = isPublic;
            request.SourceType = sourceType;

            if (request.Vicinities == null)
                request.Vicinities = new ObjectId[0];

            RecalculatePrices(request);
        }

        public ValidationResult ValidateForSaveAndUpdate(Request request, bool isPublic)
        {
            var errors = new List<ValidationError>();

            if (!isPublic && request.SourceType == SourceType.Haftdong)
            {
                if (request.Owner == null)
                {
                    errors.Add(new ValidationError("Request.Owner",
                        RequestValidationErrors.IsNotValid));
                }
            }

            return new ValidationResult {Errors = errors};
        }

        public ValidationResult ValidateForSave(Request request)
        {
            var errors = new List<ValidationError>();

            if (request == null)
            {
                errors.Add(new ValidationError("Request",
                    RequestValidationErrors.IsNotValid));
            }
            else
            {
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
            }

            return new ValidationResult {Errors = errors};
        }

        public bool AuthorizeForView(Request request, bool publicOnly)
        {
            var currentUserId = OwinRequestScopeContext.Current.GetUserId();

            if (publicOnly)
            {
                if (!request.IsPublic)
                {
                    return false;
                }

                if (!currentUserId.IsNullOrEmpty() &&
                request.CreatedByID.Equals(ObjectId.Parse(currentUserId)))
                {
                    return true;
                }

                if (request.ExpirationTime >= DateTime.Today)
                {
                    return true;
                }

                return false;
            }

            if (JJOwinRequestScopeContextExtensions.IsAdministrator())
            {
                return true;
            }

            if (request.IsPublic)
            {
                return true;
            }

            if (request.CreatedByID.Equals(ObjectId.Parse(currentUserId)))
            {
                return true;
            }

            return false;
        }

        public bool AuthorizeForEdit(Request request, bool publicOnly)
        {
            var currentUserId = OwinRequestScopeContext.Current.GetUserId();

            if (publicOnly)
            {
                if (!request.IsPublic)
                {
                    return false;
                }

                if (JJOwinRequestScopeContextExtensions.IsAdministrator())
                {
                    return true;
                }

                if (!currentUserId.IsNullOrEmpty() && request.CreatedByID == ObjectId.Parse(currentUserId))
                {
                    return true;
                }

                return false;
            }

            if (JJOwinRequestScopeContextExtensions.IsAdministrator())
            {
                return true;
            }

            if (!currentUserId.IsNullOrEmpty() && request.CreatedByID == ObjectId.Parse(currentUserId))
            {
                return true;
            }

            return false;
        }

        public ValidationResult ValidateForCompletion(Request request)
        {
            if (request.State == RequestState.Compelete)
            {
                return ValidationResult.Failure("Request.State",
                    GeneralValidationErrors.NotValid);
            }
            return ValidationResult.Success;
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

        public ValidationResult ValidateForDelete(Request request)
        {
            if (request.State != RequestState.New)
            {
                return ValidationResult.Failure("Request.State",
                    RequestValidationErrors.IsNotValid);
            }
            return ValidationResult.Success;
        }

        #endregion
    }
}