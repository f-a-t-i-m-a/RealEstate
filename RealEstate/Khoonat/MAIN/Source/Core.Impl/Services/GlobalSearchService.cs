using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.Common.Util.Search;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.Common.Util.Text;
using JahanJooy.RealEstate.Core.Impl.Components;
using JahanJooy.RealEstate.Core.Impl.Index;
using JahanJooy.RealEstate.Core.Index;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Core.Services.Dto.GlobalSearch;
using Lucene.Net.Index;
using Lucene.Net.Search;

namespace JahanJooy.RealEstate.Core.Impl.Services
{
	[Component]
	public class GlobalSearchService : IGlobalSearchService
	{
		[ComponentPlug]
		public IGlobalSearchIndex Index { get; set; }

		[ComponentPlug]
		public IComposer Composer { get; set; }

		public GlobalSearchResult RunSearch(GlobalSearchCriteria criteria, int startIndex, int pageSize)
		{
			var rootQuery = BuildRootQuery(criteria);
			var resultItems = Index.RunQuery(rootQuery, startIndex, pageSize);
			var bounds = criteria.Bounds ?? LatLngBounds.BoundsOf(resultItems.Select(i => i.GeographicLocation).WhereNotNull());
			var result = new GlobalSearchResult
			             {
				             Bounds = bounds,
				             Stats = PaginationStats.FromPaginationStats(resultItems.Stats),
				             Items = resultItems.PageItems,
				             Clusters = null
			             };

			if (criteria.ClusterGeographically)
			{
				ClusterResultsGeographically(result);
			}

			return result;
		}

		private Query BuildRootQuery(GlobalSearchCriteria criteria)
		{
			if (criteria == null)
				throw new ArgumentNullException("criteria");

			var rootQuery = new BooleanQuery();

			if (criteria.Bounds != null)
				rootQuery.Add(BuildBoundsQuery(criteria.Bounds), Occur.MUST);

			if (criteria.VicinityIDs.SafeAny())
				rootQuery.Add(BuildVicinitiesQuery(criteria.VicinityIDs), Occur.MUST);

			if (!string.IsNullOrWhiteSpace(criteria.SearchText))
			{
				var textQuery = BuildTextQuery(criteria.SearchText);
				if (textQuery != null)
					rootQuery.Add(textQuery, Occur.MUST);
			}

			if (criteria.IncludedTags.WhereNotNullOrWhitespace().SafeAny())
				rootQuery.AddAll(BuildTagQueries(criteria.IncludedTags), Occur.MUST);

			if (criteria.ExcludedTags.WhereNotNullOrWhitespace().SafeAny())
				rootQuery.AddAll(BuildTagQueries(criteria.ExcludedTags), Occur.MUST_NOT);

			if (criteria.RecordTypes.SafeAny())
				rootQuery.Add(BuildRecordTypesQuery(criteria.RecordTypes), Occur.MUST);

			// Default to current (non-archived) records, if both "Include..." flags are false
			if (!criteria.IncludeArchivedRecords)
				rootQuery.Add(new TermQuery(new Term(GlobalSearchIndexMap.FieldNames.Archived, false.ToString())), Occur.MUST);
			else if (!criteria.IncludeCurrentRecords)
				rootQuery.Add(new TermQuery(new Term(GlobalSearchIndexMap.FieldNames.Archived, true.ToString())), Occur.MUST);

			if (!criteria.IncludeDeletedRecords || !ServiceContext.Principal.IsOperator)
				rootQuery.Add(new TermQuery(new Term(GlobalSearchIndexMap.FieldNames.Deleted, false.ToString())), Occur.MUST);

			return rootQuery;
		}

		private Query BuildBoundsQuery(LatLngBounds bounds)
		{
			var boundsQuery = new BooleanQuery();
			boundsQuery.Add(NumericRangeQuery.NewDoubleRange(GlobalSearchIndexMap.FieldNames.GeographicLocationLat, bounds.SouthLat, bounds.NorthLat, true, true), Occur.MUST);
			boundsQuery.Add(NumericRangeQuery.NewDoubleRange(GlobalSearchIndexMap.FieldNames.GeographicLocationLng, bounds.WestLng, bounds.EastLng, true, true), Occur.MUST);

			return boundsQuery;
		}

		private Query BuildVicinitiesQuery(IEnumerable<long> vicinityIDs)
		{
			var vicinitiesQuery = new BooleanQuery();
			vicinitiesQuery.AddAll(vicinityIDs.Select(vid => new TermQuery(new Term(GlobalSearchIndexMap.FieldNames.VicinityIdHierarchy, vid.ToString(CultureInfo.InvariantCulture)))), Occur.SHOULD);

			return vicinitiesQuery;
		}

		private Query BuildTextQuery(string searchText)
		{
			var words = UserSearchQueryUtils.TokenizeUserQuery(searchText);
			if (words == null || !words.Any())
				return null;

			var textQuery = new BooleanQuery();
			textQuery.AddAll(words.Select(w => new TermQuery(new Term(GlobalSearchIndexMap.FieldNames.Text, w))), Occur.MUST);
			return textQuery;
		}

		private IEnumerable<Query> BuildTagQueries(List<string> tags)
		{
			throw new NotImplementedException();
		}

		private Query BuildRecordTypesQuery(List<GlobalSearchRecordType> recordTypes)
		{
			if (recordTypes.Count == 1)
				return new TermQuery(new Term(GlobalSearchIndexMap.FieldNames.Type, recordTypes[0].ToString()));

			var query = new BooleanQuery();
			query.AddAll(recordTypes.Select(rt => new TermQuery(new Term(GlobalSearchIndexMap.FieldNames.Type, rt.ToString()))), Occur.SHOULD);

			return query;
		}

		private void ClusterResultsGeographically(GlobalSearchResult result)
		{
			var largestContainedRect = result.Bounds.GetLargestContainedRect();
			var largestContainedRectArea = largestContainedRect.GetArea();
			var minimumDistinguishedArea = largestContainedRectArea / 20;

			var clusterCalculator = Composer.GetComponent<GeographicClusterCalculator>();
			clusterCalculator.SetMinimumAreaToBreakDown(minimumDistinguishedArea);
			clusterCalculator.ClusterItems(result.Items);
			clusterCalculator.FillResult(result);
		}
	}
}