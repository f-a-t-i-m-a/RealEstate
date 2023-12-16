using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Compositional.Composer;
using JahanJooy.Common.Util.Identity;
using JahanJooy.Common.Util.Owin;
using JahanJooy.RealEstateAgency.Domain.Request;
using JahanJooy.RealEstateAgency.Domain.Workflows;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Log4Net;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using JahanJooy.RealEstateAgency.Util.Models.Requests;
using JahanJooy.RealEstateAgency.Util.Owin;
using JahanJooy.RealEstateAgency.Util.VicinityCache;
using MongoDB.Bson;
using MongoDB.Driver;
using Nest;
using ServiceStack;
using Stimulsoft.Report;
using Types = JahanJooy.RealEstateAgency.Domain.Enums.Elastic.Types;

namespace JahanJooy.RealEstateAgency.Util.Report.ApplicationImplemented
{
    [Contract]
    [Component]
    public class RequestReport
    {
        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public ElasticManager ElasticManager { get; set; }

        [ComponentPlug]
        public IVicinityCache VicinityCache { get; set; }

        private readonly int MaxPageSize = 10000;
        private readonly int DefaultPageSize = 10;

        public void PopulateRequestDetails(StiReport report, string id)
        {
            FilterDefinition<Request> filter;
            Request request;

            if (string.IsNullOrEmpty(id))
            {
                filter = Builders<Request>.Filter.Eq("DeletionTime", BsonNull.Value);
                request = DbManager.Request.Find(filter).FirstOrDefaultAsync().Result;
            }
            else
            {
                filter = Builders<Request>.Filter.Eq("ID", ObjectId.Parse(id));
                request = DbManager.Request.Find(filter).SingleOrDefaultAsync().Result;
            }

            report.RegBusinessObject("Request", request);
            report.Dictionary.SynchronizeBusinessObjects();
        }

        public void PopulateRequestList(StiReport report, SearchRequestInput searchInput, List<ObjectId> ids,
            bool isPublic)
        {
            if (ids.Count == 0)
            {
                if (searchInput.PageSize == 0)
                    searchInput.PageSize = DefaultPageSize;
                else if (searchInput.PageSize < 0 || searchInput.PageSize > MaxPageSize)
                    searchInput.PageSize = MaxPageSize;

                var response = GetResultFromElastic(searchInput, isPublic, false);
                ids = response.Documents.Select(d => ObjectId.Parse(d.ID)).ToList();
            }

            List<RequestSummary> requests = new List<RequestSummary>();

            if (ids.Count != 0)
            {
                var filter = Builders<Request>.Filter.In("ID", ids)
                             & Builders<Request>.Filter.Eq("Request.IsPublic", isPublic);
                requests = DbManager.Request.Find(filter)
                    .Project(p => Mapper.Map<RequestSummary>(p))
                    .ToListAsync().Result;
            }

            report.RegBusinessObject("Requests", requests);
            report.Dictionary.SynchronizeBusinessObjects();
        }

        public ISearchResponse<RequestIE> GetResultFromElastic(SearchRequestInput searchInput, bool isPublic, bool? mineOnly)
        {
            var currentUserId = OwinRequestScopeContext.Current.GetUserId();
            var response = ElasticManager.Client.Search<RequestIE>(r => r
                .Index(ElasticManager.Index)
                .Type(Types.RequestType)
                .Query(q =>
                {
                    QueryContainer query = null;
                    if (!isPublic)
                    {
                        if (mineOnly.HasValue && mineOnly.Value && searchInput.IsPublic != true)
                        {
                            query &= q.Term(pr => pr.CreatedByID, currentUserId)
                                     && !q.Term(pr => pr.IsPublic, true);
                        }
                        else if (mineOnly.HasValue && mineOnly.Value && searchInput.IsPublic == true)
                        {
                            query &= q.Term(pr => pr.CreatedByID, currentUserId)
                                     && q.Term(pr => pr.IsPublic, true);
                        }
                        else if ((!mineOnly.HasValue || !mineOnly.Value) && searchInput.IsPublic == true)
                        {
                            query &= q.Term(pr => pr.IsPublic, true);
                        }
                        else if (!JJOwinRequestScopeContextExtensions.IsAdministrator())
                        {
                            query &= q.Term(re => re.IsPublic, true)
                                     || q.Term(pr => pr.CreatedByID, currentUserId);
                        }
                    }
                    else
                    {
                        query &= q.Term(pr => pr.IsPublic, true);

                        if (mineOnly.HasValue && mineOnly.Value && !currentUserId.IsNullOrEmpty())
                        {
                            query &= q.Term(re => re.CreatedByID, currentUserId);
                        }
                        else
                        {
                            query &= !q.Range(pr => pr.Field(o => o.ExpirationTime)
                                .LessThan(DateTime.Today.Ticks));
                        }
                    }

                    if (searchInput.UsageType != null)
                        query &= q.Term(pr => pr.Description, searchInput.UsageType.ToString().ToLower());

                    if (searchInput.State != null)
                    {
                        query &= q.Term(pr => pr.Description, searchInput.State.ToString().ToLower());
                    }
                    else
                        query &= !q.Term(pr => pr.Description, RequestState.Deleted.ToString().ToLower());
                    if (searchInput.IsArchived == true)
                        query &= q.Term(pr => pr.IsArchived, true);
                    else
                        query &= !q.Term(pr => pr.IsArchived, true);
                    if (searchInput.IntentionOfCustomer != null)
                        query &= q.Term(t => t.Description, searchInput.IntentionOfCustomer.ToString().ToLower());
                    if (searchInput.PropertyType != null)
                        query &= q.Term(t => t.PropertyTypes, ((byte) searchInput.PropertyType));

                    if (searchInput.Vicinity != null)
                    {
                        query &= q.Term(pr => pr.VicinityIds, searchInput.Vicinity.ID);
                    }

                    if (searchInput.VicinityID.HasValue && searchInput.VicinityID != ObjectId.Empty)
                    {
                        query &= q.Term(pr => pr.VicinityIds, searchInput.VicinityID);
                    }

                    if (searchInput.EstateAreaMin != null)
                        query &= q.Range(pr =>
                            pr.Field(o => o.EstateArea).GreaterThanOrEquals((double?)searchInput.EstateAreaMin));
                    if (searchInput.EstateAreaMax != null)
                        query &= q.Range(
                            pr => pr.Field(o => o.EstateArea).LessThanOrEquals((double?)searchInput.EstateAreaMax));
                    if (searchInput.UnitAreaMin != null)
                        query &= q.Range(
                            pr => pr.Field(o => o.UnitArea).GreaterThanOrEquals((double?)searchInput.UnitAreaMin));
                    if (searchInput.UnitAreaMax != null)
                        query &=
                            q.Range(pr => pr.Field(o => o.UnitArea).LessThanOrEquals((double?)searchInput.UnitAreaMax));

                    if (searchInput.MortgageMin != null)
                        query &= q.Range(
                            pr => pr.Field(o => o.Mortgage).GreaterThanOrEquals((double?)searchInput.MortgageMin));
                    if (searchInput.MortgageMax != null)
                        query &=
                            q.Range(pr => pr.Field(o => o.Mortgage).LessThanOrEquals((double?)searchInput.MortgageMax));
                    if (searchInput.RentMin != null)
                        query &= q.Range(pr => pr.Field(o => o.Rent).GreaterThanOrEquals((double?)searchInput.RentMin));
                    if (searchInput.RentMax != null)
                        query &= q.Range(pr => pr.Field(o => o.Rent).LessThanOrEquals((double?)searchInput.RentMax));
                    if (searchInput.PriceMin != null)
                        query &=
                            q.Range(
                                pr => pr.Field(o => o.TotalPrice).GreaterThanOrEquals((double?)searchInput.PriceMin));
                    if (searchInput.PriceMax != null)
                        query &=
                            q.Range(pr => pr.Field(o => o.TotalPrice).LessThanOrEquals((double?)searchInput.PriceMax));

                    return query;
                }
                ).From(searchInput.StartIndex)
                .Take(searchInput.PageSize)
                .Sort(s =>
                {
                    SortDescriptor<RequestIE> sort = new SortDescriptor<RequestIE>();
                    if (searchInput.SortColumn.HasValue && searchInput.SortDirection.HasValue)
                    {
                        switch (searchInput.SortColumn.Value)
                        {
                            case RequestSortColumn.TotalPrice:
                                sort = searchInput.SortDirection.Value.Equals(SortDirectionType.Asc)
                                    ? s.Ascending(rie => rie.TotalPrice)
                                    : s.Descending(rie => rie.TotalPrice);
                                break;
                            case RequestSortColumn.Mortgage:
                                sort = searchInput.SortDirection.Value.Equals(SortDirectionType.Asc)
                                    ? s.Ascending(rie => rie.Mortgage)
                                    : s.Descending(rie => rie.Mortgage);
                                break;
                            case RequestSortColumn.Rent:
                                sort = searchInput.SortDirection.Value.Equals(SortDirectionType.Asc)
                                    ? s.Ascending(rie => rie.Rent)
                                    : s.Descending(rie => rie.Rent);
                                break;
                        }
                    }
                    else
                    {
                        sort = s.Descending(rie => rie.CreationTime);
                    }
                    return sort;
                })
                );

            if (!response.IsValid)
            {
                ApplicationStaticLogs.ElasticLog.ErrorFormat(
                    "An error occured while searching requests, debug information: {0}",
                    response.DebugInformation);
            }

            return response;
        }
    }
}