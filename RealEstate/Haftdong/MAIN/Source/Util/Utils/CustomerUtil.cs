using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Compositional.Composer;
using JahanJooy.Common.Util.ApiModel.Pagination;
using JahanJooy.Common.Util.Identity;
using JahanJooy.Common.Util.Owin;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstateAgency.Domain.Customers;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Enums;
using JahanJooy.RealEstateAgency.Util.Log4Net;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using JahanJooy.RealEstateAgency.Util.Models.Customers;
using JahanJooy.RealEstateAgency.Util.Owin;
using JahanJooy.RealEstateAgency.Util.Utils.ContactMethods;
using MongoDB.Bson;
using MongoDB.Driver;
using Nest;
using ServiceStack;
using Types = JahanJooy.RealEstateAgency.Domain.Enums.Elastic.Types;

namespace JahanJooy.RealEstateAgency.Util.Utils
{
    [Contract]
    [Component]
    public class CustomerUtil
    {
        #region Injected dependencies

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public UserUtil UserUtil { get; set; }

        [ComponentPlug]
        public ElasticManager ElasticManager { get; set; }

        [ComponentPlug]
        public LocalContactMethodUtil LocalContactMethodUtil { get; set; }

        [ComponentPlug]
        public LocalPhoneNumberUtil LocalPhoneNumberUtil { get; set; }

        #endregion

        #region Action methods

        public ValidationResult SaveCustomer(Customer customer)
        {
            var result = ValidateForSaveAndUpdate(customer);
            if (!result.IsValid)
                return result;

            customer.LastIndexingTime = null;
            customer.CreationTime = DateTime.Now;
            customer.CreatedByID = ObjectId.Parse(OwinRequestScopeContext.Current.GetUserId());
            customer.LastModificationTime = DateTime.Now;
            customer.LastModifiedTimeByID = ObjectId.Parse(OwinRequestScopeContext.Current.GetUserId());

            if (customer.Deputy != null && !customer.Deputy.DisplayName.IsEmpty())
            {
                var customerReference = Mapper.Map<CustomerReference>(customer.Deputy);
                customer.Deputy = customerReference;
            }

            DbManager.Customer.InsertOneAsync(customer).Wait();
            return ValidationResult.Success;
        }

        public CustomerDetails GetCustomerDetailById(ObjectId id)
        {
            var filter = Builders<Customer>.Filter.Eq("ID", id);
            var customer = DbManager.Customer.Find(filter)
                        .SingleOrDefaultAsync()
                        .Result;

            if (customer == null)
                return null;

            if (!AuthorizeForView(customer))
            {
                return new CustomerDetails();
            }

            return Mapper.Map<CustomerDetails>(customer);
        }

        public Customer GetCustomerById(ObjectId id)
        {
            var filter = Builders<Customer>.Filter.Eq("ID", id);
            var customer =
                DbManager.Customer.Find(filter)
                    .SingleOrDefaultAsync()
                    .Result;

            if (customer == null)
                return null;

            if (!AuthorizeForView(customer))
            {
                return null;
            }

            return customer;
        }


        public ValidatedResult<CustomerReference> GetCustomerByCustomerDetails(Customer input)
        {
            var userId = OwinRequestScopeContext.Current.GetUserId();

            if (input == null ||
                (input.ID == ObjectId.Empty && input.DisplayName.IsNullOrEmpty())
                || input.ID == ObjectId.Parse(userId))
            {
                var existingCustomer = GetCustomerByUserId(userId);
                if (existingCustomer == null)
                {
                    var existingUser = UserUtil.GetUser(userId);

                    if (existingUser == null)
                        return ValidatedResult<CustomerReference>.Failure("User", UserValidationError.NotFound);

                    var newCustomer = new Customer
                    {
                        DisplayName = existingUser.DisplayName,
                        UserID = ObjectId.Parse(userId),
                        Contact = existingUser.Contact
                    };

                    var result = SaveCustomer(newCustomer);
                    if (!result.IsValid)
                        return ValidatedResult<CustomerReference>.Failure(result.Errors);

                    return ValidatedResult<CustomerReference>.Success(Mapper.Map<CustomerReference>(newCustomer));
                }

                return ValidatedResult<CustomerReference>.Success(Mapper.Map<CustomerReference>(existingCustomer));
            }
            else
            {
                if (input.ID == ObjectId.Empty) 
                {
                    if (input.DisplayName.IsNullOrEmpty())
                    {
                        return ValidatedResult<CustomerReference>.Failure("Customer.DisplayName", CustomerValidationErrors.NotSpecified);
                    }
                    if (input.Contact.Phones.Count <= 0)
                    {
                        return ValidatedResult<CustomerReference>.Failure("Customer.Phone", CustomerValidationErrors.NotSpecified);
                    }

                    var newCustomer = new Customer
                    {
                        DisplayName = input.DisplayName,
                        Contact = input.Contact
                    };

                    var result = SaveCustomer(newCustomer);
                    if (!result.IsValid)
                        return ValidatedResult<CustomerReference>.Failure(result.Errors);

                    return ValidatedResult<CustomerReference>.Success(Mapper.Map<CustomerReference>(newCustomer));
                }

                var existingCustomer = GetCustomerDetailById(input.ID);
                if (existingCustomer == null)
                {
                    return ValidatedResult<CustomerReference>.Failure("Customer", CustomerValidationErrors.NotFound);
                }
                return ValidatedResult<CustomerReference>.Success(Mapper.Map<CustomerReference>(existingCustomer));
            }
        }

        public CustomerDetails GetCustomerByUserId(string id)
        {
            var filter = Builders<Customer>.Filter.Eq("UserID", ObjectId.Parse(id));
            var customer =
                DbManager.Customer.Find(filter)
                    .SingleOrDefaultAsync()
                    .Result;

            if (customer == null)
                return null;

            if (!AuthorizeForView(customer))
            {
                return new CustomerDetails();
            }

            return Mapper.Map<CustomerDetails>(customer);
        }

        public SearchCustomersOutput Search(SearchCustomerInput searchInput)
        {
            var response = GetResultFromElastic(searchInput);
            var ids = response.Documents.Select(d => ObjectId.Parse(d.ID)).ToList();
            var result = new PagedListOutput<CustomerSummary>();

            if (ids.Count != 0)
            {
                var filter = Builders<Customer>.Filter.In("ID", ids);
                List<CustomerSummary> customers = DbManager.Customer.Find(filter)
                    .Project(p => Mapper.Map<CustomerSummary>(p))
                    .ToListAsync().Result;

                customers.ForEach(c => c.CreatorFullName = UserUtil.GetUserName(c.CreatedByID));
                List<CustomerSummary> sortedCustomers = new List<CustomerSummary>();
                ids.ForEach(id => sortedCustomers.Add(customers.SingleOrDefault(c => c.ID == id)));
                sortedCustomers.RemoveAll(s => s == null);

                result = new PagedListOutput<CustomerSummary>
                {
                    PageItems = sortedCustomers,
                    PageNumber = (searchInput.StartIndex/searchInput.PageSize) + 1,
                    TotalNumberOfItems = (int) response.Total,
                    TotalNumberOfPages = (int) Math.Ceiling((decimal) response.Total/searchInput.PageSize)
                };
            }

            return new SearchCustomersOutput
            {
                Customers = result
            };
        }

        #endregion

        #region Private helper methods 

        private ValidationResult ValidateForSaveAndUpdate(Customer customer)
        {
            if (string.IsNullOrEmpty(customer.DisplayName))
                return ValidationResult.Failure("Customer.DisplayName",
                    GeneralValidationErrors.ValueNotSpecified);

            return ValidationResult.Success;
        }

        public bool AuthorizeForView(Customer customer)
        {
            if (JJOwinRequestScopeContextExtensions.IsAdministrator())
            {
                return true;
            }

            if (customer.CreatedByID.Equals(ObjectId.Parse(OwinRequestScopeContext.Current.GetUserId())))
            {
                return true;
            }

            if (customer.UserID == ObjectId.Parse(OwinRequestScopeContext.Current.GetUserId()))
            {
                return true;
            }

            return false;
        }

        private ISearchResponse<CustomerIE> GetResultFromElastic(SearchCustomerInput searchInput)
        {
            var currentUserId = OwinRequestScopeContext.Current.GetUserId();
            var response = ElasticManager.Client.Search<CustomerIE>(c => c
                .Index(ElasticManager.Index)
                .Type(Types.CustomerType)
                .Query(q =>
                {
                    QueryContainer query = null;
                    if (!JJOwinRequestScopeContextExtensions.IsAdministrator())
                    {
                        query &= q.Term(pr => pr.CreatedByID, currentUserId);
                    }

                    if (searchInput.IsArchived == true)
                        query &= q.Term(cu => cu.IsArchived, true);
                    else
                        query &= !q.Term(cu => cu.IsArchived, true);

                    if (searchInput.IsDeleted == true)
                        query &= !q.Term(cu => cu.DeletionTime, 0);
                    else
                        query &= q.Term(cu => cu.DeletionTime, 0);

                    if (!string.IsNullOrEmpty(searchInput.DisplayName))
                        query &= q.MatchPhrasePrefix(m => m.Field(o => o.DisplayName).Query(searchInput.DisplayName));
                    if (searchInput.IntentionOfVisit == 1)
                        query &= q.Range(r => r.Field(o => o.RequestCount).GreaterThan(0));
                    if (searchInput.IntentionOfVisit == 2)
                        query &= q.Range(r => r.Field(o => o.PropertyCount).GreaterThan(0));
                    if (searchInput.IntentionOfVisit == 3)
                        query &=
                            q.Bool(
                                b =>
                                    b.Must(
                                        s =>
                                            s.Range(r => r.Field(o => o.RequestCount).GreaterThan(0)) &&
                                            s.Range(r => r.Field(o => o.PropertyCount).GreaterThan(0))));
                    if (!string.IsNullOrEmpty(searchInput.Email))
                        query &= q.MatchPhrasePrefix(m => m.Field(o => o.Email).Query(searchInput.Email));
                    if (!string.IsNullOrEmpty(searchInput.PhoneNumber))
                        query &= q.MatchPhrasePrefix(m => m.Field(o => o.Numbers).Query(searchInput.PhoneNumber));
                    return query;
                })
                .From(searchInput.StartIndex)
                .Take(searchInput.PageSize)
                .Sort(s =>
                {
                    SortDescriptor<CustomerIE> sort = new SortDescriptor<CustomerIE>();
                    if (searchInput.SortColumn.HasValue && searchInput.SortDirection.HasValue)
                    {
                        switch (searchInput.SortColumn.Value)
                        {
                            case CustomerSortColumn.LastVisitTime:
                                sort = searchInput.SortDirection.Value.Equals(SortDirectionType.Asc)
                                    ? s.Ascending(cie => cie.LastVisitTime)
                                    : s.Descending(cie => cie.LastVisitTime);
                                break;
                        }
                    }
                    else
                    {
                        sort = s.Descending(cie => cie.LastVisitTime);
                    }
                    return sort;
                })
                );

            if (!response.IsValid)
            {
                ApplicationStaticLogs.ElasticLog.ErrorFormat(
                    "An error occured while searching customers, debug information: {0}",
                    response.DebugInformation);
            }

            return response;
        }

        #endregion
    }
}