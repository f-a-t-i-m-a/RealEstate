using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using Compositional.Composer;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.ApiModel.Pagination;
using JahanJooy.Common.Util.Identity;
using JahanJooy.Common.Util.Owin;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstateAgency.Api.Web.Base;
using JahanJooy.RealEstateAgency.Domain.Audit;
using JahanJooy.RealEstateAgency.Domain.Contract;
using JahanJooy.RealEstateAgency.Domain.Customers;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Enums.User;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using JahanJooy.RealEstateAgency.Domain.MasterData;
using JahanJooy.RealEstateAgency.Domain.Property;
using JahanJooy.RealEstateAgency.Domain.Request;
using JahanJooy.RealEstateAgency.Domain.Supply;
using JahanJooy.RealEstateAgency.Domain.Workflows;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Enums;
using JahanJooy.RealEstateAgency.Util.Identity;
using JahanJooy.RealEstateAgency.Util.Models.Contracts;
using JahanJooy.RealEstateAgency.Util.Models.Customers;
using JahanJooy.RealEstateAgency.Util.Models.Properties;
using JahanJooy.RealEstateAgency.Util.Models.Requests;
using JahanJooy.RealEstateAgency.Util.Models.Supplies;
using JahanJooy.RealEstateAgency.Util.Report;
using JahanJooy.RealEstateAgency.Util.Report.ApplicationImplemented;
using JahanJooy.RealEstateAgency.Util.UserActivityLog;
using JahanJooy.RealEstateAgency.Util.Utils;
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
    [RoutePrefix("contracts")]
    public class ContractController : ExtendedApiController
    {
        #region Injected dependencies

        private static readonly ILog Log = LogManager.GetLogger(typeof (ContractController));

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public PropertyUtil PropertyUtil { get; set; }

        [ComponentPlug]
        public SupplyUtil SupplyUtil { get; set; }

        [ComponentPlug]
        public RequestUtil RequestUtil { get; set; }

        [ComponentPlug]
        public ContractReport Report { get; set; }

        [ComponentPlug]
        public ReportRepository ReportRepository { get; set; }
        [ComponentPlug]
        public UserUtil UserUtil { get; set; }

        private int MaxPageSize = 10000;
        private int DefaultPageSize = 10;

        #endregion

        #region Action methods

        [HttpPost, Route("save")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.Create, EntityType.Contract, ApplicationType.Haftdong)]
        public IHttpActionResult SaveContract(NewContractInput input)
        {
            var contract = Mapper.Map<Contract>(input);

            PropertyReference propertyReference;
            SupplyReference supplyReference;
            var propertyResult = SaveProperty(input, out propertyReference, out supplyReference,
                SourceType.Haftdong, ApplicationType.Haftdong);
            if (!propertyResult.IsValid)
                return ValidationResult(propertyResult);

            if (supplyReference == null || propertyReference == null)
                return
                    ValidationResult("Contract", ContractValidationErrors.UnexpectedError);

            contract.PropertyReference = propertyReference;
            contract.SupplyReference = supplyReference;

            if (input.SellerID != null)
            {
                CustomerReference sellerReference;
                SaveSellerOrBuyer(ObjectId.Parse(input.SellerID), out sellerReference);
                contract.SellerReference = sellerReference;
            }

            if (input.BuyerID != null)
            {
                CustomerReference buyerReference;
                SaveSellerOrBuyer(ObjectId.Parse(input.BuyerID), out buyerReference);
                contract.BuyerReference = buyerReference;
            }

            if (input.RequestID != null)
            {
                RequestReference requestReference;
                SaveRequest(input, out requestReference);
                contract.RequestReference = requestReference;
            }

            PrepareContractForSave(contract);
            var result = ValidateForSaveOrUpdate(contract);
            if (!result.IsValid)
                return ValidationResult(result);

            try
            {
                DbManager.Contract.InsertOneAsync(contract).Wait();
                UserActivityLogUtils.SetMainActivity(targetId: contract.ID);

                return Ok(contract);
            }
            catch (Exception e)
            {
                Log.Error("An exception has been occured in saving Contract", e);
                return ValidationResult("Contract", ContractValidationErrors.UnexpectedError);
            }
        }

        [HttpPost, Route("update")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.Edit, EntityType.Contract, ApplicationType.Haftdong)]
        public IHttpActionResult UpdateContract(UpdateContractInput input)
        {
            UserActivityLogUtils.SetMainActivity(targetId: input.ID);

            var filter = Builders<Contract>.Filter.Eq("ID", input.ID);
            var newContract = Mapper.Map<Contract>(input);

            var update = Builders<Contract>.Update
                .Set("ContractDate", newContract.ContractDate)
                .Set("DeliveryDate", newContract.DeliveryDate)
                .Set("Portion", newContract.Portion)
                .Set("District", newContract.District)
                .Set("RegistrationZone", newContract.RegistrationZone)
                .Set("OwnershipEvidenceSerialNumber", newContract.OwnershipEvidenceSerialNumber)
                .Set("NotaryPublicPageNumber", newContract.NotaryPublicPageNumber)
                .Set("NotaryPublic", newContract.NotaryPublic)
                .Set("PublicSpace", newContract.PublicSpace)
                .Set("Description", newContract.Description)
                .Set("TotalPrice", newContract.TotalPrice)
                .Set("Mortgage", newContract.Mortgage)
                .Set("Rent", newContract.Rent)
                .Set("UnitArea", newContract.UnitArea)
                .Set("EstateArea", newContract.EstateArea)
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("LastModificationTime", DateTime.Now);

            try
            {
                var result = DbManager.Contract.UpdateOneAsync(filter, update);
                if (result.Result.MatchedCount != 1)
                    return ValidationResult("Contract", ContractValidationErrors.NotFound);

                if (result.Result.MatchedCount == 1 && result.Result.ModifiedCount != 1)
                    return ValidationResult("Contract", ContractValidationErrors.NotModified);

                return ValidationResult(Common.Util.Validation.ValidationResult.Success);
            }
            catch (Exception e)
            {
                Log.Error("An exception has been occured in updating Contract", e);
                return ValidationResult("Contract", ContractValidationErrors.UnexpectedError);
            }
        }

        [HttpGet, Route("get/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        public ContractDetails GetContract(string id)
        {
            RequestSummary request = null;

            var filter = Builders<Contract>.Filter.Eq("ID", ObjectId.Parse(id));
            var contract =
                DbManager.Contract.Find(filter)
                    .SingleOrDefaultAsync()
                    .Result;

            var filterSupply = Builders<Supply>.Filter.Eq("ID", contract.SupplyReference.ID);
            var supply =
                DbManager.Supply.Find(filterSupply)
                    .Project(c => Mapper.Map<SupplyDetails>(c))
                    .SingleOrDefaultAsync()
                    .Result;
            var filterProperty = Builders<Property>.Filter.Eq("ID", contract.PropertyReference.ID);
            var property =
                DbManager.Property.Find(filterProperty)
                    .Project(c => Mapper.Map<PropertyDetails>(c))
                    .SingleOrDefaultAsync()
                    .Result;
            var filterSeller = Builders<Customer>.Filter.Eq("ID", contract.SellerReference.ID);
            var seller =
                DbManager.Customer.Find(filterSeller)
                    .Project(c => Mapper.Map<CustomerDetails>(c))
                    .SingleOrDefaultAsync()
                    .Result;

            var filterBuyer = Builders<Customer>.Filter.Eq("ID", contract.BuyerReference.ID);
            var buyer =
                DbManager.Customer.Find(filterBuyer)
                    .Project(c => Mapper.Map<CustomerDetails>(c))
                    .SingleOrDefaultAsync()
                    .Result;

            if (contract.RequestReference != null)
            {
                var filterRequest = Builders<Request>.Filter.Eq("ID", contract.RequestReference.ID);
                request =
                    DbManager.Request.Find(filterRequest)
                        .Project(c => Mapper.Map<RequestSummary>(c))
                        .SingleOrDefaultAsync()
                        .Result;
            }

            var contractDetail = Mapper.Map<ContractDetails>(contract);
            contractDetail.SupplyDetails = supply;
            contractDetail.SupplyDetails.PropertyDetail = property;
            contractDetail.RequestSummary = request;
            contractDetail.Seller = seller;
            contractDetail.Buyer = buyer;
            return contractDetail;
        }

        [HttpGet, Route("getallcontractsbycreatedbyid/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] { BuiltInRole.Administrator })]
        public List<ContractSummary> GetAllContractsByCreatedByID(string id)
        {
            var filter = Builders<Contract>.Filter.Eq("CreatedByID", ObjectId.Parse(id));
            List<ContractSummary> contracts = DbManager.Contract.Find(filter).Project(p => Mapper.Map<ContractSummary>(p)).ToListAsync().Result;
            return contracts;
        }

        [HttpGet, Route("all")]
        public List<Contract> GetAllContracts()
        {
            var filter = new BsonDocument();
            List<Contract> contracts = DbManager.Contract.Find(filter).ToListAsync().Result;

            return contracts;
        }

        [HttpPost, Route("cancel/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.ChangeState, EntityType.Contract, ApplicationType.Haftdong, TargetState = ContractState.Cancellation)]
        public IHttpActionResult CancelContract(string id)
        {
            UserActivityLogUtils.SetMainActivity(targetId: ObjectId.Parse(id));

            var filter = Builders<Contract>.Filter.Eq("ID", ObjectId.Parse(id));
            var contract = DbManager.Contract.Find(filter).SingleOrDefaultAsync().Result;
            var validationResult = ValidateForCancel(contract);
            if (!validationResult.IsValid)
                return ValidationResult(validationResult);

            var update = Builders<Contract>.Update
                .Set("State", ContractState.Cancellation)
                .Set("LastIndexingTime", BsonNull.Value);

            var result = DbManager.Contract.UpdateOneAsync(filter, update);
            if (result.Result.MatchedCount != 1)
                return ValidationResult("Contract", ContractValidationErrors.NotFound);

            if (result.Result.MatchedCount == 1 && result.Result.ModifiedCount != 1)
                return ValidationResult("Contract", ContractValidationErrors.NotModified);

            if (contract.SupplyReference != null)
            {
                var supplyFilter = Builders<Supply>.Filter.Eq("ID", contract.SupplyReference.ID);
                var supplyUpdate = Builders<Supply>.Update
                    .Set("State", SupplyState.New)
                    .Set("LastIndexingTime", BsonNull.Value);

                result = DbManager.Supply.UpdateOneAsync(supplyFilter, supplyUpdate);
                if (result.Result.MatchedCount != 1 ||
                    (result.Result.MatchedCount == 1 && result.Result.ModifiedCount != 1))
                    Log.WarnFormat(
                        "Supply reference with id {0} in related contract could not been updated in canceling contract process",
                        contract.SupplyReference.ID);
                else
                {
                    UserActivityLogUtils.ReportAdditionalActivity(new UserActivity
                    {
                        ApplicationType = ApplicationType.Haftdong,
                        TargetType = EntityType.Supply,
                        TargetID = contract.SupplyReference.ID,
                        ActivityType = UserActivityType.ChangeState,
                        TargetState = SupplyState.New.ToString(),
                        ActivitySubType = "InCancelingContract"
                    });
                    Log.InfoFormat(
                        "Supply reference with id {0} in related contract has been updated in canceling contract process",
                        contract.SupplyReference.ID);
                }
            }

            if (contract.RequestReference != null)
            {
                var requestFilter = Builders<Request>.Filter.Eq("ID", contract.RequestReference.ID);
                var requestUpdate = Builders<Request>.Update
                    .Set("State", RequestState.New)
                    .Set("LastIndexingTime", BsonNull.Value);

                result = DbManager.Request.UpdateOneAsync(requestFilter, requestUpdate);
                if (result.Result.MatchedCount != 1 ||
                    (result.Result.MatchedCount == 1 && result.Result.ModifiedCount != 1))
                    Log.WarnFormat(
                        "Request reference with id {0} in related contract could not been updated in canceling contract process",
                        contract.RequestReference.ID);

                else
                {
                    UserActivityLogUtils.ReportAdditionalActivity(new UserActivity
                    {
                        ApplicationType = ApplicationType.Haftdong,
                        TargetType = EntityType.Request,
                        TargetID = contract.RequestReference.ID,
                        ActivityType = UserActivityType.ChangeState,
                        TargetState = RequestState.New.ToString(),
                        ActivitySubType = "InCancelingContract"
                    });

                    Log.InfoFormat(
                        "Request reference with id {0} in related contract has been updated in canceling contract process",
                        contract.RequestReference.ID);
                }
            }

            return ValidationResult(Common.Util.Validation.ValidationResult.Success);
        }

        [HttpPost, Route("close/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.ChangeState, EntityType.Contract, ApplicationType.Haftdong, TargetState = ContractState.Contracted)]
        public IHttpActionResult CloseContract(string id)
        {
            UserActivityLogUtils.SetMainActivity(targetId: ObjectId.Parse(id));

            var filter = Builders<Contract>.Filter.Eq("ID", ObjectId.Parse(id));
            var contract = DbManager.Contract.Find(filter).SingleOrDefaultAsync().Result;
            var validationResult = ValidateForClose(contract);
            if (!validationResult.IsValid)
                return ValidationResult(validationResult);

            var update = Builders<Contract>.Update
                .Set("State", ContractState.Contracted)
                .Set("LastIndexingTime", BsonNull.Value);

            var result = DbManager.Contract.UpdateOneAsync(filter, update);
            if (result.Result.MatchedCount != 1)
                return ValidationResult("Contract", ContractValidationErrors.NotFound);

            if (result.Result.MatchedCount == 1 && result.Result.ModifiedCount != 1)
                return ValidationResult("Contract", ContractValidationErrors.NotModified);

            if (contract.SupplyReference != null)
            {
                UserActivityLogUtils.ReportAdditionalActivity(new UserActivity
                {
                    ApplicationType = ApplicationType.Haftdong,
                    TargetType = EntityType.Supply,
                    TargetID = contract.SupplyReference.ID,
                    ActivityType = UserActivityType.ChangeState,
                    TargetState = SupplyState.Completed.ToString(),
                    ActivitySubType = "InCompletingContract"
                });
                SupplyUtil.CompleteSupply(contract.SupplyReference.ID, SupplyCompletionReason.Contracted);
            }

            return ValidationResult(Common.Util.Validation.ValidationResult.Success);
        }

        [HttpPost, Route("search")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [SkipUserActivityLogging]
        public SearchContractOutput Search(SearchContractInput searchInput)
        {
            if (searchInput.PageSize == 0)
                searchInput.PageSize = DefaultPageSize;
            else if (searchInput.PageSize < 0 || searchInput.PageSize > MaxPageSize)
                searchInput.PageSize = MaxPageSize;

            var response = Report.GetResultFromElastic(searchInput);
            var ids = response.Documents.Select(d => ObjectId.Parse(d.ID)).ToList();
            var result = new PagedListOutput<ContractSummary>();

            if (ids.Count != 0)
            {
                var filter = Builders<Contract>.Filter.In("ID", ids);
                List<ContractSummary> contracts = DbManager.Contract.Find(filter)
                    .Project(p => Mapper.Map<ContractSummary>(p))
                    .ToListAsync().Result;

                contracts.ForEach( c => c.CreatorFullName = UserUtil.GetUserName(c.CreatedByID));
                List<ContractSummary> sortedContracts = new List<ContractSummary>();
                ids.ForEach(id => sortedContracts.Add(contracts.SingleOrDefault(c => c.ID == id)));
                sortedContracts.RemoveAll(s => s == null);

                result = new PagedListOutput<ContractSummary>
                {
                    PageItems = sortedContracts,
                    PageNumber = (searchInput.StartIndex/searchInput.PageSize) + 1,
                    TotalNumberOfItems = (int) response.Total,
                    TotalNumberOfPages = (int) Math.Ceiling((decimal) response.Total/searchInput.PageSize)
                };
            }

            return new SearchContractOutput
            {
                Contracts = result
            };
        }

        [HttpPost, Route("details/{id}/print")]
        [UserActivity(UserActivityType.PrintDetail, EntityType.Contract, ApplicationType.Haftdong)]
        public IHttpActionResult Print(string id, PrintContractInput input)
        {
            UserActivityLogUtils.SetMainActivity(targetId: ObjectId.Parse(id));
            var report = ReportRepository.GetApplicationImplemented(
                ApplicationImplementedReportDataSourceType.Contract,
                input.IfNotNull(i => i.ReportTemplateID));

            if (report == null)
                return
                    ValidationResult("ReportTemplate", ReportTemplateValidationErrors.NotFound);
            Report.PopulateContractDetails(report, id);

            StiExportFormat format;
            if (!Enum.TryParse(input.Format, true, out format))
                format = StiExportFormat.Pdf;

            return new ReportBinaryResult(report, format);
        }

        [HttpPost, Route("print")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.Print, EntityType.Contract, ApplicationType.Haftdong)]
        public IHttpActionResult PrintAll(PrintContractsInput input)
        {
            if (input != null)
            {
                var report = ReportRepository.GetApplicationImplemented(
                    ApplicationImplementedReportDataSourceType.Contracts,
                    input.IfNotNull(i => i.ReportTemplateID));

                if (report == null)
                    return
                        ValidationResult("ReportTemplate", ReportTemplateValidationErrors.NotFound);

                Report.PopulateContractList(report, input.SearchInput, input.Ids.Select(ObjectId.Parse).ToList());

                StiExportFormat format;
                if (!Enum.TryParse(input.Format, true, out format))
                    format = StiExportFormat.Pdf;

                return new ReportBinaryResult(report, format);
            }

            return ValidationResult("Contract.Print", ContractValidationErrors.UnexpectedError);
        }

        #endregion

        #region Private helper methods 

        public ValidationResult ValidateForCancel(Contract contract)
        {
            if (contract.State != ContractState.Agreement)
            {
                return Common.Util.Validation.ValidationResult.Failure("Contract.State",
                    ContractValidationErrors.IsNotValid);
            }
            return Common.Util.Validation.ValidationResult.Success;
        }

        public ValidationResult ValidateForClose(Contract contract)
        {
            if (contract.State != ContractState.Agreement)
            {
                return Common.Util.Validation.ValidationResult.Failure("Contract.State",
                    ContractValidationErrors.IsNotValid);
            }
            return Common.Util.Validation.ValidationResult.Success;
        }

        public ValidationResult ValidateForSaveOrUpdate(Contract contract)
        {
            if (contract.SellerReference == null)
            {
                return Common.Util.Validation.ValidationResult.Failure("Contract.SellerReference",
                    GeneralValidationErrors.ValueNotSpecified);
            }

            if (contract.BuyerReference == null)
            {
                return Common.Util.Validation.ValidationResult.Failure("Contract.BuyerReference",
                    GeneralValidationErrors.ValueNotSpecified);
            }

            return Common.Util.Validation.ValidationResult.Success;
        }

        private ValidationResult SaveProperty(NewContractInput input, out PropertyReference propertyReference,
            out SupplyReference supplyReference, SourceType sourceType, ApplicationType applicationType)
        {
            propertyReference = null;
            supplyReference = null;

            if (input.PropertyID == null)
            {
                var newProperty = Mapper.Map<Property>(input);
                var newSupply = Mapper.Map<Supply>(input);
                newSupply.TotalPrice = input.ContractTotalPrice;
                newSupply.Rent = input.ContractRent;
                newSupply.Mortgage = input.ContractMortgage;

                if (newSupply.IntentionOfOwner == IntentionOfOwner.ForSale)
                    newSupply.PriceSpecificationType = SalePriceSpecificationType.Total;

                var customerRefrence = new CustomerReference {ID = ObjectId.Parse(input.SellerID)};
                newProperty.Owner = customerRefrence;
                newProperty.Supplies = new List<SupplyReference>();

                var propertyResult =
                    PropertyUtil.SaveProperty(newProperty, newSupply, null, false, sourceType, applicationType);
                if (propertyResult.IsValid)
                {
                    UserActivityLogUtils.ReportAdditionalActivity(new UserActivity
                    {
                        ApplicationType = ApplicationType.Haftdong,
                        TargetType = EntityType.Property,
                        ActivityType = UserActivityType.Create,
                        TargetID = newProperty.ID,
                        ParentType = EntityType.Contract
                    });

                    supplyReference = newProperty.Supplies[0];
                    propertyReference = Mapper.Map<PropertyReference>(newProperty);

                }
                else
                {
                    return propertyResult;
                }
            }
            else
            {
                var propertyFilter = Builders<Property>.Filter.Eq("ID", input.PropertyID);
                var updateProperty = Builders<Property>.Update
                    .Set("LicencePlate", input.LicencePlate);

                var propertyResult = DbManager.Property.FindOneAndUpdateAsync(propertyFilter, updateProperty);

                if (input.SupplyID != null)
                {
                    UserActivityLogUtils.ReportAdditionalActivity(new UserActivity
                    {
                        ApplicationType = ApplicationType.Haftdong,
                        TargetType = EntityType.Supply,
                        TargetID = input.SupplyID,
                        ActivityType = UserActivityType.ChangeState,
                        TargetState = SupplyState.Reserved.ToString(),
                        ActivitySubType = "InCreatingContract"
                    });

                    SupplyUtil.ReserveSupply(input.SupplyID.ToString());

                    var supplyfilter = Builders<Supply>.Filter.Eq("ID", input.SupplyID);
                    var supply = DbManager.Supply.Find(supplyfilter).SingleOrDefaultAsync().Result;
                    supplyReference = Mapper.Map<SupplyReference>(supply);

                    if (propertyResult.Result != null)
                    {
                        propertyReference = Mapper.Map<PropertyReference>(propertyResult.Result);
                    }
                }
            }
            return Common.Util.Validation.ValidationResult.Success;
        }

        private void PrepareContractForSave(Contract contract)
        {
            contract.State = ContractState.Agreement;
            contract.LastIndexingTime = null;
            contract.CreationTime = DateTime.Now;
            contract.CreatedByID = ObjectId.Parse(OwinRequestScopeContext.Current.GetUserId());
            contract.LastModificationTime = DateTime.Now;
            contract.LastModifiedTimeByID = ObjectId.Parse(OwinRequestScopeContext.Current.GetUserId());
        }

        private void SaveRequest(NewContractInput input, out RequestReference requestReference)
        {
            requestReference = null;
            var filter = Builders<Request>.Filter.Eq("ID", input.RequestID);
            var request = DbManager.Request.Find(filter).SingleOrDefaultAsync().Result;

            if (request != null)
            {
                requestReference = Mapper.Map<RequestReference>(request);

                UserActivityLogUtils.ReportAdditionalActivity(new UserActivity
                {
                    ApplicationType = ApplicationType.Haftdong,
                    TargetType = EntityType.Request,
                    TargetID = input.RequestID,
                    ActivityType = UserActivityType.ChangeState,
                    TargetState = RequestState.Compelete.ToString(),
                    ActivitySubType = "InCreatingContract"
                });
                RequestUtil.CompleteRequest(input.RequestID.ToString());
            }
        }

        private void SaveSellerOrBuyer(ObjectId id, out CustomerReference userReference)
        {
            userReference = null;

            var filter = Builders<Customer>.Filter.Eq("ID", id);
            var user = DbManager.Customer.Find(filter).SingleOrDefaultAsync().Result;

            if (user != null)
            {
                userReference = Mapper.Map<CustomerReference>(user);
            }
        }

        #endregion
    }
}