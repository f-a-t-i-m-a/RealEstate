using System;
using System.Collections.Generic;
using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.Collections;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Core.Services.Dto;
using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Directory;
using JahanJooy.RealEstate.Domain.Enums;

namespace JahanJooy.RealEstate.Core.Impl.Components
{
	[Contract]
	[Component]
	[ComponentCache(null)]
	public class OldGeographicClusterCalculator
	{
		private const int MinIndividualItemsInVicinityToGroup = 5;
		private const double MinVicinityAreaCoefficientForIndividualItemsGrouping = 0.4;
		private const double MinCoefficientForIntraVicinityClustering = 16;

		private double _minimumAreaToBreakDown;

		private readonly Dictionary<long, GeoSearchVicinityResult> _vicinityResultItems = new Dictionary<long, GeoSearchVicinityResult>();

		private readonly Dictionary<long, PropertyListingSummary> _vicinitySinglePropertyListings = new Dictionary<long, PropertyListingSummary>();
		private readonly Dictionary<long, List<PropertyListingSummary>> _individualPropertyListingsInVicinity = new Dictionary<long, List<PropertyListingSummary>>();

		private readonly Dictionary<long, AgencyBranch> _vicinitySingleAgencyBranches = new Dictionary<long, AgencyBranch>();
		private readonly Dictionary<long, List<AgencyBranch>> _individualAgencyBranchesInVicinity = new Dictionary<long, List<AgencyBranch>>();

		[ComponentPlug]
		public IVicinityCache VicinityCache { get; set; }

		public void SetMinimumAreaToBreakDown(double minimumAreaToBreakDown)
		{
			_minimumAreaToBreakDown = minimumAreaToBreakDown;
		}

		public void ClusterPropertyListings(IEnumerable<PropertyListingSummary> propertyListings)
		{
			foreach (var propertyListing in propertyListings.EmptyIfNull())
			{
				if (propertyListing.GeographicLocation == null)
					continue;

				var listingVicinity = VicinityCache[propertyListing.VicinityID.GetValueOrDefault()];
				if (listingVicinity == null)
					continue;

				var distinguishableVicinity = FindDistinguishableVicinity(listingVicinity, _minimumAreaToBreakDown);
				if (distinguishableVicinity == null)
					continue;

				var centerPoint = GetVicinityCenterPoint(distinguishableVicinity) ?? GetVicinityCenterPoint(listingVicinity);
				if (centerPoint == null)
					continue;

				if (propertyListing.GeographicLocationType == GeographicLocationSpecificationType.InferredFromVicinity)
				{
					PlaceResultInVicinityCluster(distinguishableVicinity.ID, centerPoint, propertyListing);
				}
				else
				{
					// If the listing's vicinity is large enough that the items inside it can be distinguished,
					// return the property listing as an individual. Otherwise, aggregate it in the parent vicinities.

					if (listingVicinity.ID == distinguishableVicinity.ID)
					{
						if (_individualPropertyListingsInVicinity.ContainsKey(listingVicinity.ID))
							_individualPropertyListingsInVicinity[listingVicinity.ID].Add(propertyListing);
						else
							_individualPropertyListingsInVicinity[listingVicinity.ID] = new List<PropertyListingSummary> {propertyListing};
					}
					else
					{
						PlaceResultInVicinityCluster(distinguishableVicinity.ID, centerPoint, propertyListing);
					}
				}
			}
		}

		public void ClusterAgencyBranches(IEnumerable<AgencyBranch> agencyBranches)
		{
			foreach (var agencyBranch in agencyBranches ?? Enumerable.Empty<AgencyBranch>())
			{
				// TODO
			}
		}

		public void PrepareResults(GeoSearchResult result)
		{
			foreach (var vicinityIndividualResults in _individualPropertyListingsInVicinity)
			{
				// Checking to see if we need to cluster individual items in a vicinity.
				// For example, if there are many individual results in a vicinity that occupies a small
				// area on the user screen

				var listings = vicinityIndividualResults.Value;
				if (listings == null)
					continue;

				// If there are too few results in a single vicinity, don't cluster no matter how small
				if (listings.Count < MinIndividualItemsInVicinityToGroup)
					continue;

				// Extract vicinity area
				var vicinityId = vicinityIndividualResults.Key;
				var vicinity = VicinityCache[vicinityId];
				var vicinityArea = GetVicinityAreaEstimate(vicinity);

				if (vicinityArea <= 0)
					continue;

				// If the area share for each item is not too small, don't cluster and report items individually
				var dividedArea = vicinityArea/listings.Count;
				if (dividedArea > _minimumAreaToBreakDown * MinVicinityAreaCoefficientForIndividualItemsGrouping)
					continue;

				// If the vicinity is not displayed too large on the screen, group all items within it 
				// in a single result item.

				// TODO: Uncomment the condition below when clustering is implemented
				//if (vicinityArea <= _minimumAreaToBreakDown*MinCoefficientForIntraVicinityClustering)
				//{
					var centerPoint = GetVicinityCenterPoint(vicinity) ??
					                  LatLngBounds.BoundsOf(listings.Select(l => l.GeographicLocation.ToLatLng()).Where(l => l != null)).GetCenter();

				foreach (var listing in listings)
				{
					PlaceResultInVicinityCluster(vicinityId, centerPoint, listing);
				}

				listings.Clear();
				//}
				//else
				//{
				//	TODO: Cluster
				//}
			}

			result.VicinityResults = _vicinityResultItems.Values.ToList();
			result.PropertyListings = _vicinitySinglePropertyListings.Values
				.Concat(_individualPropertyListingsInVicinity.SelectMany(pair => pair.Value))
				.ToList();
			result.Agencies = new List<AgencyBranch>();
		}

		private void PlaceResultInVicinityCluster(long vicinityId, LatLng centerPoint, PropertyListingSummary propertyListing)
		{
			// If this is the first one in the vicinity, we want to return it individually.
			// Otherwise - when there is more than one listing in the same vicinity -
			// group them together in a vicinity.

			if (_vicinityResultItems.ContainsKey(vicinityId))
			{
				_vicinityResultItems[vicinityId].NumberOfPropertyListings++;
			}
			else if (_vicinitySinglePropertyListings.ContainsKey(vicinityId))
			{
				_vicinityResultItems.Add(vicinityId, BuildGeoSearchVicinityResult(vicinityId, centerPoint, propertyListing));
				_vicinityResultItems[vicinityId].NumberOfPropertyListings += 2; // One from before, one is being added now

				_vicinitySinglePropertyListings.Remove(vicinityId);
			}
			else
			{
				_vicinitySinglePropertyListings.Add(vicinityId, propertyListing);
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
						vicinity.Children.IfNotNull(children => children.Count, 2)/2));
			}

			return vicinity.Boundary.Area.GetValueOrDefault()*multiplier;
		}

		private static GeoSearchVicinityResult BuildGeoSearchVicinityResult(long vicinityId, LatLng centerPoint, PropertyListingSummary propertyListing)
		{
			return new GeoSearchVicinityResult
			       {
				       VicinityID = vicinityId,
				       GeographicLocation = centerPoint,
				       Decomposable = propertyListing.GeographicLocationType != GeographicLocationSpecificationType.InferredFromVicinity,
				       NumberOfPropertyListings = 0,
				       NumberOfAgencies = 0
			       };
		}

		private static LatLng GetVicinityCenterPoint(Vicinity vicinity)
		{
			return vicinity.CenterPoint.ToLatLng() ??
			       vicinity.Boundary.FindBoundingBox().IfNotNull(bb => bb.GetCenter());
		}
	}
}