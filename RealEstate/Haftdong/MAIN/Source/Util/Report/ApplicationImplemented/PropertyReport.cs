using Compositional.Composer;
using JahanJooy.Common.Util.Identity;
using JahanJooy.Common.Util.Owin;
using JahanJooy.RealEstateAgency.Domain.Property;
using JahanJooy.RealEstateAgency.Domain.Workflows;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Log4Net;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using JahanJooy.RealEstateAgency.Util.Models.Properties;
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
    public class PropertyReport
    {
        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public ElasticManager ElasticManager { get; set; }

        public void PopulatePropertyDetails(StiReport report, string id, bool isPublic)
        {
            FilterDefinition<Property> filter;
            Property property;

            if (string.IsNullOrEmpty(id))
            {
                filter = Builders<Property>.Filter.Eq("DeletionTime", BsonNull.Value)
                    & Builders<Property>.Filter.Eq("IsPublic", isPublic);
                property = DbManager.Property.Find(filter).FirstOrDefaultAsync().Result;
            }
            else
            {
                filter = Builders<Property>.Filter.Eq("ID", ObjectId.Parse(id))
                    & Builders<Property>.Filter.Eq("IsPublic", isPublic);
                property = DbManager.Property.Find(filter).SingleOrDefaultAsync().Result;
            }

            report.RegBusinessObject("Property", property);
            report.Dictionary.SynchronizeBusinessObjects();
        }
        public ISearchResponse<PropertyIE> GetResultFromElastic(SearchPropertyInput searchInput)
        {
            var currentUserId = OwinRequestScopeContext.Current.GetUserId();
            var response = ElasticManager.Client.Search<PropertyIE>(p => p
                .Index(ElasticManager.Index)
                .Type(Types.PropertyType)
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
                        query &= !q.Term(pr => pr.Description, PropertyState.Deleted.ToString().ToLower());
                    if (searchInput.IsArchived == true)
                        query &= q.Term(pr => pr.IsArchived, true);
                    else
                        query &= !q.Term(pr => pr.IsArchived, true);
                    if (searchInput.UsageType != null)
                        query &= q.Term(pr => pr.Description, searchInput.UsageType.ToString().ToLower());
                    if (searchInput.PropertyType != null)
                        query &= q.Term(pr => pr.Description, searchInput.PropertyType.ToString().ToLower());
                    if (searchInput.IntentionOfOwner != null)
                        query &= q.Term(pr => pr.Description, searchInput.IntentionOfOwner.ToString().ToLower());
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
                    if (searchInput.NumberOfRoomsMin != null)
                        query &= q.Range(
                            pr => pr.Field(o => o.NumberOfRooms).GreaterThanOrEquals(searchInput.NumberOfRoomsMin));
                    if (searchInput.NumberOfRoomsMax != null)
                        query &=
                            q.Range(pr => pr.Field(o => o.NumberOfRooms).LessThanOrEquals(searchInput.NumberOfRoomsMax));
                    if (searchInput.OwnerName != null)
                        query &= q.Term(pr => pr.Description, searchInput.OwnerName.ToString().ToLower());
                    if (!string.IsNullOrEmpty(searchInput.Address))
                        query &= q.MatchPhrasePrefix(m => m.Field(o => o.Address).Query(searchInput.Address));
                    if (searchInput.CreationTime.HasValue)
                        query &= q.Range(pr => pr.Field(o => o.CreationTime)
                            .GreaterThanOrEquals(searchInput.CreationTime?.Ticks));
                    if (searchInput.CreationTime.HasValue)
                        query &= q.Range(pr => pr.Field(o => o.CreationTime)
                            .LessThanOrEquals(searchInput.CreationTime?.AddDays(1).Ticks));
                    return query;
                })
                .From(searchInput.StartIndex)
                .Take(searchInput.PageSize)
                .Sort(s =>
                {
                    SortDescriptor<PropertyIE> sort = new SortDescriptor<PropertyIE>();
                    if (searchInput.SortColumn.HasValue && searchInput.SortDirection.HasValue)
                    {
                        switch (searchInput.SortColumn.Value)
                        {
                            case PropertySortColumn.CreationTime:
                                sort = searchInput.SortDirection.Value.Equals(SortDirectionType.Asc)
                                    ? s.Ascending(pie => pie.CreationTime)
                                    : s.Descending(pie => pie.CreationTime);
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
                ApplicationStaticLogs.ElasticLog.ErrorFormat("An error occured while searching properties, debug information: {0}",
                    response.DebugInformation);
            }

            return response;
        }
    }
}