using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Compositional.Composer;
using JahanJooy.Common.Util.Identity;
using JahanJooy.Common.Util.Owin;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Supply;
using JahanJooy.RealEstateAgency.Domain.Workflows;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Log4Net;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using JahanJooy.RealEstateAgency.Util.Models.Supplies;
using JahanJooy.RealEstateAgency.Util.Owin;
using JahanJooy.RealEstateAgency.Util.VicinityCache;
using MongoDB.Bson;
using MongoDB.Driver;
using Nest;
using ServiceStack;
using Stimulsoft.Report;
using Types = JahanJooy.RealEstateAgency.Domain.Enums.Elastic.Types;

// ReSharper disable PossibleInvalidOperationException

namespace JahanJooy.RealEstateAgency.Util.Report.ApplicationImplemented
{
    [Contract]
    [Component]
    public class SupplyReport
    {
        private readonly int DefaultPageSize = 10;
        private readonly int MaxPageSize = 10000;

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IVicinityCache VicinityCache { get; set; }

        [ComponentPlug]
        public ElasticManager ElasticManager { get; set; }

        public void PopulateSupplyDetails(StiReport report, string id)
        {
            FilterDefinition<Supply> filter;
            Supply supply;

            if (string.IsNullOrEmpty(id))
            {
                filter = Builders<Supply>.Filter.Eq("DeletionTime", BsonNull.Value);
                supply = DbManager.Supply.Find(filter).FirstOrDefaultAsync().Result;
            }
            else
            {
                filter = Builders<Supply>.Filter.Eq("ID", ObjectId.Parse(id));
                supply = DbManager.Supply.Find(filter).SingleOrDefaultAsync().Result;
            }

            report.RegBusinessObject("Supply", supply);
            report.Dictionary.SynchronizeBusinessObjects();
        }

        public void PopulateSupplyList(StiReport report, SearchFileInput searchInput,
            List<ObjectId> ids, bool isPublic)
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

            var supplies = new List<SupplySummary>();

            if (ids.Count != 0)
            {
                var filter = Builders<Supply>.Filter.In("ID", ids)
                             & Builders<Supply>.Filter.Eq("Property.IsPublic", isPublic);
                supplies = DbManager.Supply.Find(filter)
                    .Project(p => Mapper.Map<SupplySummary>(p))
                    .ToListAsync().Result;
            }

            report.RegBusinessObject("Supplies", supplies);
            report.Dictionary.SynchronizeBusinessObjects();
        }

        public ISearchResponse<SupplyIE> GetResultFromElastic(SearchFileInput searchInput, bool publicOnly,
            bool? mineOnly)
        {
            var currentUserId = OwinRequestScopeContext.Current.GetUserId();
            var response = ElasticManager.Client.Search<SupplyIE>(p => p
                .Index(ElasticManager.Index)
                .Type(Types.SupplyType)
                .Query(q =>
                {
                    QueryContainer query = null;
                    if (!publicOnly)
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
                            query &= !q.Term(pr => pr.Description, SourceType.Haftdong.ToString().ToLower()) ||
                                     q.Term(pr => pr.IsPublic, true) ||
                                     q.Term(pr => pr.CreatedByID, currentUserId);
                        }
                    }
                    else
                    {
                        query &= q.Term(pr => pr.IsPublic, true);

                        if (mineOnly.HasValue && mineOnly.Value && !currentUserId.IsNullOrEmpty())
                        {
                            query &= q.Term(pr => pr.CreatedByID, currentUserId);
                        }
                        else
                        {
                            query &= !q.Range(pr => pr.Field(o => o.ExpirationTime)
                                .LessThan(DateTime.Today.Ticks));
                        }
                    }

                    if (searchInput.State != null)
                    {
                        query &= q.Term(pr => pr.Description, searchInput.State.ToString().ToLower());
                    }
                    else
                    {
                        query &= !q.Term(pr => pr.Description, SupplyState.Deleted.ToString().ToLower());
                    }

                    if (searchInput.IsArchived == true)
                        query &= q.Term(pr => pr.IsArchived, true);
                    else
                        query &= !q.Term(pr => pr.IsArchived, true);

                    if (searchInput.HasPhoto == true)
                        query &= q.Term(pr => pr.HasPhoto, true);

                    if (searchInput.IsHidden == true && JJOwinRequestScopeContextExtensions.IsAdministrator())
                        query &= q.Term(pr => pr.IsHidden, true);
                    else
                        query &= !q.Term(pr => pr.IsHidden, true);

                    if (searchInput.HasWarning == true && JJOwinRequestScopeContextExtensions.IsAdministrator())
                        query &= q.Term(pr => pr.HasWarning, true);

                    if (searchInput.UsageType != null)
                        query &= q.Term(pr => pr.Description, searchInput.UsageType.ToString().ToLower());
                    if (searchInput.PropertyType != null)
                        query &= q.Term(pr => pr.Description, searchInput.PropertyType.ToString().ToLower());
                    if (searchInput.IntentionOfOwner != null)
                        query &= q.Term(pr => pr.Description, searchInput.IntentionOfOwner.ToString().ToLower());
                    if (searchInput.SourceType != null)
                        query &= q.Term(pr => pr.Description, searchInput.SourceType.ToString().ToLower());
                    if (searchInput.EstateAreaMin != null)
                        query &= q.Range(pr =>
                            pr.Field(o => o.EstateArea).GreaterThanOrEquals((double?) searchInput.EstateAreaMin));
                    if (searchInput.EstateAreaMax != null)
                        query &= q.Range(
                            pr => pr.Field(o => o.EstateArea).LessThanOrEquals((double?) searchInput.EstateAreaMax));
                    if (searchInput.UnitAreaMin != null)
                        query &= q.Range(
                            pr => pr.Field(o => o.UnitArea).GreaterThanOrEquals((double?) searchInput.UnitAreaMin));
                    if (searchInput.UnitAreaMax != null)
                        query &=
                            q.Range(pr => pr.Field(o => o.UnitArea).LessThanOrEquals((double?) searchInput.UnitAreaMax));
                    if (searchInput.NumberOfRoomsMin != null)
                        query &= q.Range(
                            pr => pr.Field(o => o.NumberOfRooms).GreaterThanOrEquals(searchInput.NumberOfRoomsMin));
                    if (searchInput.NumberOfRoomsMax != null)
                        query &=
                            q.Range(pr => pr.Field(o => o.NumberOfRooms).LessThanOrEquals(searchInput.NumberOfRoomsMax));
                    if (searchInput.MortgageMin != null)
                        query &= q.Range(
                            pr => pr.Field(o => o.Mortgage).GreaterThanOrEquals((double?) searchInput.MortgageMin));
                    if (searchInput.MortgageMax != null)
                        query &=
                            q.Range(pr => pr.Field(o => o.Mortgage).LessThanOrEquals((double?) searchInput.MortgageMax));
                    if (searchInput.RentMin != null)
                        query &= q.Range(pr => pr.Field(o => o.Rent).GreaterThanOrEquals((double?) searchInput.RentMin));
                    if (searchInput.RentMax != null)
                        query &= q.Range(pr => pr.Field(o => o.Rent).LessThanOrEquals((double?) searchInput.RentMax));
                    if (searchInput.PriceMin != null)
                        query &=
                            q.Range(
                                pr => pr.Field(o => o.TotalPrice).GreaterThanOrEquals((double?) searchInput.PriceMin));
                    if (searchInput.PriceMax != null)
                        query &=
                            q.Range(pr => pr.Field(o => o.TotalPrice).LessThanOrEquals((double?) searchInput.PriceMax));
                    if (searchInput.PricePerEstateAreaMin != null)
                        query &= q.Range(pr => pr.Field(o => o.PricePerEstateArea)
                            .GreaterThanOrEquals((double?) searchInput.PricePerEstateAreaMin));
                    if (searchInput.PricePerEstateAreaMax != null)
                        query &= q.Range(pr => pr.Field(o => o.PricePerEstateArea)
                            .LessThanOrEquals((double?) searchInput.PricePerEstateAreaMax));
                    if (searchInput.PricePerUnitAreaMin != null)
                        query &= q.Range(pr => pr.Field(o => o.PricePerUnitArea)
                            .GreaterThanOrEquals((double?) searchInput.PricePerUnitAreaMin));
                    if (searchInput.PricePerUnitAreaMax != null)
                        query &= q.Range(pr => pr.Field(o => o.PricePerUnitArea)
                            .LessThanOrEquals((double?) searchInput.PricePerUnitAreaMax));

                    if (searchInput.Vicinity != null)
                    {
                        query &= q.Term(pr => pr.VicinityIds, searchInput.Vicinity.ID);
                    }

                    if (searchInput.VicinityID.HasValue && searchInput.VicinityID != ObjectId.Empty)
                    {
                        query &= q.Term(pr => pr.VicinityIds, searchInput.VicinityID);
                    }

                    if (searchInput.Bounds != null)
                    {
                        query &=
                            q.GeoBoundingBox(
                                pr =>
                                    pr.Field(pp => pp.GeographicLocation)
                                        .BoundingBox(ppp => ppp.BottomRight(searchInput.Bounds.SouthLat,
                                            searchInput.Bounds.EastLng).TopLeft(searchInput.Bounds.NorthLat,
                                                searchInput.Bounds.WestLng)));
                    }

                    return query;
                })
                .From(searchInput.StartIndex)
                .Take(searchInput.PageSize)
                .Sort(s =>
                {
                    var sort = new SortDescriptor<SupplyIE>();
                    if (searchInput.SortColumn.HasValue && searchInput.SortDirection.HasValue)
                    {
                        switch (searchInput.SortColumn.Value)
                        {
                            case SupplySortColumn.CreationTime:
                                sort = searchInput.SortDirection.Value.Equals(SortDirectionType.Asc)
                                    ? s.Ascending(pie => pie.CreationTime)
                                    : s.Descending(pie => pie.CreationTime);
                                break;
                            case SupplySortColumn.TotalPrice:
                                sort = searchInput.SortDirection.Value.Equals(SortDirectionType.Asc)
                                    ? s.Ascending(pie => pie.TotalPrice)
                                    : s.Descending(pie => pie.TotalPrice);
                                break;
                        }
                    }
                    else
                    {
                        sort = s.Descending(pie => pie.CreationTime);
                    }
                    return sort;
                })
                );

            if (!response.IsValid)
            {
                ApplicationStaticLogs.ElasticLog.ErrorFormat(
                    "An error occured while searching supplies, debug information: {0}",
                    response.DebugInformation);
            }

            return response;
        }
    }
}