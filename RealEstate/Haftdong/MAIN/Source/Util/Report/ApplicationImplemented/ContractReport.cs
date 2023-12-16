using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Compositional.Composer;
using JahanJooy.Common.Util.Identity;
using JahanJooy.Common.Util.Owin;
using JahanJooy.RealEstateAgency.Domain.Contract;
using JahanJooy.RealEstateAgency.Domain.Workflows;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Log4Net;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using JahanJooy.RealEstateAgency.Util.Models.Contracts;
using JahanJooy.RealEstateAgency.Util.Owin;
using MongoDB.Bson;
using MongoDB.Driver;
using Nest;
using Stimulsoft.Report;
using Types = JahanJooy.RealEstateAgency.Domain.Enums.Elastic.Types;

namespace JahanJooy.RealEstateAgency.Util.Report.ApplicationImplemented
{
    [Contract]
    [Component]
    public class ContractReport
    {
        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public ElasticManager ElasticManager { get; set; }

        private int MaxPageSize = 10000;
        private int DefaultPageSize = 10;

        public void PopulateContractDetails(StiReport report, string id)
        {
            FilterDefinition<Contract> filter;
            Contract contract;

            if (string.IsNullOrEmpty(id))
            {
                filter = Builders<Contract>.Filter.Eq("DeletionTime", BsonNull.Value);
                contract = DbManager.Contract.Find(filter).FirstOrDefaultAsync().Result;
            }
            else
            {
                filter = Builders<Contract>.Filter.Eq("ID", ObjectId.Parse(id));
                contract = DbManager.Contract.Find(filter).SingleOrDefaultAsync().Result;
            }

            report.RegBusinessObject("Contract", contract);
            report.Dictionary.SynchronizeBusinessObjects();
        }

        public void PopulateContractList(StiReport report, SearchContractInput searchInput, List<ObjectId> ids)
        {
            if (ids.Count == 0)
            {
                if (searchInput.PageSize == 0)
                    searchInput.PageSize = DefaultPageSize;
                else if (searchInput.PageSize < 0 || searchInput.PageSize > MaxPageSize)
                    searchInput.PageSize = MaxPageSize;

                var response = GetResultFromElastic(searchInput);
                ids = response.Documents.Select(d => ObjectId.Parse(d.ID)).ToList();
            }

            List<ContractSummary> contracts = new List<ContractSummary>();

            if (ids.Count != 0)
            {
                var filter = Builders<Contract>.Filter.In("ID", ids);
                contracts = DbManager.Contract.Find(filter)
                    .Project(p => Mapper.Map<ContractSummary>(p))
                    .ToListAsync().Result;
            }

            report.RegBusinessObject("Contracts", contracts);
            report.Dictionary.SynchronizeBusinessObjects();
        }

        public ISearchResponse<ContractIE> GetResultFromElastic(SearchContractInput searchInput)
        {
            var currentUserId = OwinRequestScopeContext.Current.GetUserId();
            var response = ElasticManager.Client.Search<ContractIE>(c =>
            {
                var searchDescriptor = c
                    .Index(ElasticManager.Index)
                    .Type(Types.ContractType)
                    .Query(q =>
                    {
                        QueryContainer query = null;
                        if (!JJOwinRequestScopeContextExtensions.IsAdministrator())
                            query &= q.Term(pr => pr.CreatedByID, currentUserId);
                        if (searchInput.State != null)
                        {
                            query &= q.Term(pr => pr.Description, searchInput.State.ToString().ToLower());
                        }
                        else
                        {
                            query &= !q.Term(pr => pr.Description, ContractState.Cancellation.ToString().ToLower());
                        }
                        if (searchInput.UsageType != null)
                            query &= q.Term(co => co.Description, searchInput.UsageType.ToString().ToLower());
                        if (searchInput.PropertyType != null)
                            query &= q.Term(co => co.Description, searchInput.PropertyType.ToString().ToLower());
                        if (searchInput.IntentionOfOwner != null)
                            query &= q.Term(co => co.Description, searchInput.IntentionOfOwner.ToString().ToLower());
                        if (searchInput.FromDate != null)
                            query &=
                                q.Range(
                                    r =>
                                        r.Field(o => o.ContractDate)
                                            .GreaterThanOrEquals(searchInput.FromDate.Value.Ticks));
                        if (searchInput.ToDate != null)
                            query &=
                                q.Range(
                                    r =>
                                        r.Field(o => o.ContractDate)
                                            .LessThanOrEquals(searchInput.ToDate.Value.Ticks));
                        return query;
                    })
                    .From(searchInput.StartIndex)
                    .Take(searchInput.PageSize)
                    .Sort(s =>
                    {
                        SortDescriptor<ContractIE> sort = new SortDescriptor<ContractIE>();
                        if (searchInput.SortColumn.HasValue && searchInput.SortDirection.HasValue)
                        {
                            switch (searchInput.SortColumn.Value)
                            {
                                case ContractSortColumn.ContractDate:
                                    sort = searchInput.SortDirection.Value.Equals(SortDirectionType.Asc)
                                        ? s.Ascending(cie => cie.ContractDate)
                                        : s.Descending(cie => cie.ContractDate);
                                    break;
                            }
                        }
                        else
                        {
                            sort = s.Descending(cie => cie.ContractDate);
                        }
                        return sort;
                    });
                return searchDescriptor;
            });

            if (!response.IsValid)
            {
                ApplicationStaticLogs.ElasticLog.ErrorFormat("An error occured while searching contracts, debug information: {0}",
                    response.DebugInformation);
            }

            return response;
        }
    }
}