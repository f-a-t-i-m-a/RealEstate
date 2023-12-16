using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util.Identity;
using JahanJooy.Common.Util.Owin;
using JahanJooy.RealEstateAgency.Domain.Base;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Log4Net;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using JahanJooy.RealEstateAgency.Util.Models.Dashboard;
using JahanJooy.RealEstateAgency.Util.Owin;
using Nest;
using Types = JahanJooy.RealEstateAgency.Domain.Enums.Elastic.Types;

namespace JahanJooy.RealEstateAgency.Util.Utils
{
    [Contract]
    [Component]
    public class DashBoardUtil
    {
        #region Injected dependencies

        [ComponentPlug]
        public ElasticManager ElasticManager { get; set; }

        #endregion

        #region Action methods

        public ISearchResponse<SearchIE> UserQuickSearch(QuickSearchInput input)
        {
           
            var currentUserId = OwinRequestScopeContext.Current.GetUserId();
            var response = ElasticManager.Client.Search<SearchIE>(si => si
                .Index(ElasticManager.Index)
                .Type(Types.SearchType)
                .Query(q =>
                {
                    QueryContainer query = null;
                    if (!JJOwinRequestScopeContextExtensions.IsAdministrator())
                        query &= q.Term(pr => pr.CreatedByID, currentUserId);
                    query &= q.MatchPhrasePrefix(m => m.Field(o => o.IndexedText).Query(input.Text.ToLower()));

                    //                    QueryContainer qq = null;
                    //                    input.DataTypes.ForEach(d =>
                    //                    {
                    //                        qq |= q.Term(pr => pr.DataType, d.ToString());
                    //                    });
                    //
                    //                    query &= qq;
                    query &=
                        q.Terms(
                            c => c.Field(p => p.DataType).Terms(input.DataTypes.Select(d => d.ToString())));
                    return query;
                })
                .From(input.StartIndex)
                .Take(input.PageSize)
                .Sort(s =>
                {
                    var sort = new SortDescriptor<SearchIE>();
                    if (input.SortColumn.HasValue && input.SortDirection.HasValue)
                    {
                        switch (input.SortColumn.Value)
                        {
                            case DashboardSortColumn.CreationTime:
                                sort = input.SortDirection.Value.Equals(SortDirectionType.Asc)
                                    ? s.Ascending(e => e.CreationTime)
                                    : s.Descending(e => e.CreationTime);
                                break;
                            case DashboardSortColumn.ModificationTime:
                                sort = input.SortDirection.Value.Equals(SortDirectionType.Asc)
                                    ? s.Ascending(e => e.ModificationTime)
                                    : s.Descending(e => e.ModificationTime);
                                break;
                        }
                    }
                    else
                    {
                        sort = s.Descending(e => e.CreationTime);
                    }
                    return sort;
                })
                );

            if (!response.IsValid)
            {
                ApplicationStaticLogs.ElasticLog.ErrorFormat(
                    "An error occured while user quick searching, debug information: {0}",
                    response.DebugInformation);
            }


            return response;
        }

        #endregion

        #region Private helper methods 
       
        #endregion
    }
}