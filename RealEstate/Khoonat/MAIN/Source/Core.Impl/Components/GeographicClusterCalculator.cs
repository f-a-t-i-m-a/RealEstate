using System;
using System.Collections.Generic;
using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.Collections;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Core.Services.Dto.GlobalSearch;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Enums;

namespace JahanJooy.RealEstate.Core.Impl.Components
{
	[Contract]
	[Component]
	[ComponentCache(null)]
	public class GeographicClusterCalculator
	{
		private const int MinIndividualItemsInVicinityToGroup = 5;
		private const double MinVicinityAreaCoefficientForIndividualItemsGrouping = 0.4;
		private const double MinCoefficientForIntraVicinityClustering = 16;

		private double _minimumAreaToBreakDown;

		private readonly Dictionary<long, GlobalSearchResultCluster> _multiItemClusters = new Dictionary<long, GlobalSearchResultCluster>();
		private readonly Dictionary<long, GlobalSearchResultItem> _singleItemClusters = new Dictionary<long, GlobalSearchResultItem>();
		private readonly Dictionary<long, List<GlobalSearchResultItem>> _individualItemsPerVicinity = new Dictionary<long, List<GlobalSearchResultItem>>();

		[ComponentPlug]
		public IVicinityCache VicinityCache { get; set; }

		public void SetMinimumAreaToBreakDown(double minimumAreaToBreakDown)
		{
			_minimumAreaToBreakDown = minimumAreaToBreakDown;
		}

		public void ClusterItems(IEnumerable<GlobalSearchResultItem> items)
		{
			foreach (var item in items.EmptyIfNull())
			{
				if (item.GeographicLocation == null)
					continue;

				var vicinity = VicinityCache[item.VicinityID.GetValueOrDefault()];
				if (vicinity == null)
					continue;

				var distinguishableVicinity = FindDistinguishableVicinity(vicinity, _minimumAreaToBreakDown);
				if (distinguishableVicinity == null)
					continue;

				var centerPoint = GetVicinityCenterPoint(distinguishableVicinity) ?? GetVicinityCenterPoint(vicinity);
				if (centerPoint == null)
					continue;

				if (item.GeographicLocationType == GeographicLocationSpecificationType.InferredFromVicinity)
				{
					PlaceItemInCluster(distinguishableVicinity.ID, centerPoint, item);
				}
				else
				{
					// If the listing's vicinity is large enough that the items inside it can be distinguished,
					// return the property listing as an individual. Otherwise, aggregate it in the parent vicinities.

					if (vicinity.ID == distinguishableVicinity.ID)
					{
						if (_individualItemsPerVicinity.ContainsKey(vicinity.ID))
							_individualItemsPerVicinity[vicinity.ID].Add(item);
						else
							_individualItemsPerVicinity[vicinity.ID] = new List<GlobalSearchResultItem> { item };
					}
					else
					{
						PlaceItemInCluster(distinguishableVicinity.ID, centerPoint, item);
					}
				}
			}
		}

		public void FillResult(GlobalSearchResult result)
		{
			foreach (var individualItemsPerVicinity in _individualItemsPerVicinity)
			{
				// Checking to see if we need to cluster individual items in a vicinity.
				// For example, if there are many individual results in a vicinity that occupies a small
				// area on the user screen

				var items = individualItemsPerVicinity.Value;
				if (items == null)
					continue;

				// If there are too few results in a single vicinity, don't cluster no matter how small
				if (items.Count < MinIndividualItemsInVicinityToGroup)
					continue;

				// Extract vicinity area
				var vicinityId = individualItemsPerVicinity.Key;
				var vicinity = VicinityCache[vicinityId];
				var vicinityArea = GetVicinityAreaEstimate(vicinity);

				if (vicinityArea <= 0)
					continue;

				// If the area share for each item is not too small, don't cluster and report items individually
				var dividedArea = vicinityArea / items.Count;
				if (dividedArea > _minimumAreaToBreakDown * MinVicinityAreaCoefficientForIndividualItemsGrouping)
					continue;

				// If the vicinity is not displayed too large on the screen, group all items within it 
				// in a single result item.

				// TODO: Uncomment the condition below when clustering is implemented
				//if (vicinityArea <= _minimumAreaToBreakDown*MinCoefficientForIntraVicinityClustering)
				//{
				var centerPoint = GetVicinityCenterPoint(vicinity) ??
								  LatLngBounds.BoundsOf(items.Select(i => i.GeographicLocation).Where(l => l != null)).GetCenter();

				foreach (var listing in items)
				{
					PlaceItemInCluster(vicinityId, centerPoint, listing);
				}

				items.Clear();
				//}
				//else
				//{
				//	TODO: Cluster
				//}
			}

			result.Clusters = _multiItemClusters;
			result.Items = _singleItemClusters.Values
				.Concat(_individualItemsPerVicinity.SelectMany(pair => pair.Value))
				.ToList();
		}

		private void PlaceItemInCluster(long vicinityId, LatLng centerPoint, GlobalSearchResultItem item)
		{
			// If this is the first one in the vicinity, we want to return it individually.
			// Otherwise - when there is more than one listing in the same vicinity -
			// group them together in a vicinity.

			if (_multiItemClusters.ContainsKey(vicinityId))
			{
				IncrementNumberOfRecordType(_multiItemClusters[vicinityId], item);
			}
			else if (_singleItemClusters.ContainsKey(vicinityId))
			{
				var previousItem = _singleItemClusters[vicinityId];
				_singleItemClusters.Remove(vicinityId);

				_multiItemClusters.Add(vicinityId, new GlobalSearchResultCluster
				                                   {
					                                   VicinityID = vicinityId,
					                                   GeographicLocation = centerPoint,
					                                   Decomposable = false,
													   NumberOfRecords = new Dictionary<GlobalSearchRecordType, int>()
				                                   });

				IncrementNumberOfRecordType(_multiItemClusters[vicinityId], item);
				IncrementNumberOfRecordType(_multiItemClusters[vicinityId], previousItem);
			}
			else
			{
				_singleItemClusters.Add(vicinityId, item);
			}
		}

		private Vicinity FindDistinguishableVicinity(Vicinity vicinity, double minimumAreaToBreakDown)
		{
			var hierarchy = vicinity.GetParentsInclusive().Reverse();
			var result = vicinity;

			foreach (var v in hierarchy)
			{
				var area = GetVicinityAreaEstimate(v);
				if (area <= 0)
					continue;

				result = v;

				if (area < minimumAreaToBreakDown)
					break;
			}

			return result;
		}

		private double GetVicinityAreaEstimate(Vicinity vicinity)
		{
			if (vicinity == null)
				return 0;

			double multiplier = 1;
			while (vicinity.Boundary == null)
			{
				vicinity = vicinity.Parent;
				if (vicinity == null)
					return 0;

				multiplier /= Math.Max(2,
					Math.Min(10,
						vicinity.Children.IfNotNull(children => children.Count, 2) / 2));
			}

			return vicinity.Boundary.Area.GetValueOrDefault() * multiplier;
		}

		private static LatLng GetVicinityCenterPoint(Vicinity vicinity)
		{
			return vicinity.CenterPoint.ToLatLng() ??
				   vicinity.Boundary.FindBoundingBox().IfNotNull(bb => bb.GetCenter());
		}

		private static void IncrementNumberOfRecordType(GlobalSearchResultCluster cluster, GlobalSearchResultItem item)
		{
			if (!cluster.NumberOfRecords.ContainsKey(item.Type))
				cluster.NumberOfRecords.Add(item.Type, 0);

			cluster.NumberOfRecords[item.Type] = cluster.NumberOfRecords[item.Type] + 1;
			cluster.TotalNumberOfRecords = cluster.TotalNumberOfRecords + 1;
			cluster.Decomposable = cluster.Decomposable | (item.GeographicLocationType != GeographicLocationSpecificationType.InferredFromVicinity);
		}
	}
}