using System;
using System.Collections.Generic;
using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.Collections;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.RealEstateAgency.Domain.Vicinities;
using JahanJooy.RealEstateAgency.Util.Models.Map;
using JahanJooy.RealEstateAgency.Util.Models.Supplies;
using JahanJooy.RealEstateAgency.Util.VicinityCache;
using MongoDB.Bson;
using MongoDB.Driver.GeoJsonObjectModel;

namespace JahanJooy.RealEstateAgency.Util.Utils.Map
{
    [Contract]
    [Component]
    [ComponentCache(null)]
    public class GeographicClusterCalculator
    {
        private const int MinIndividualItemsInVicinityToGroup = 5;
        private const double MinVicinityAreaCoefficientForIndividualItemsGrouping = 0.4;

        private readonly Dictionary<ObjectId, List<SupplySummary>> _individualPropertiesInVicinity =
            new Dictionary<ObjectId, List<SupplySummary>>();

        private readonly Dictionary<ObjectId, GeoSearchVicinityResult> _vicinityResultItems =
            new Dictionary<ObjectId, GeoSearchVicinityResult>();

        private readonly Dictionary<ObjectId, SupplySummary> _vicinitySingleProperties =
            new Dictionary<ObjectId, SupplySummary>();

        private double _minimumAreaToBreakDown;

        [ComponentPlug]
        public IVicinityCache VicinityCache { get; set; }

        public void SetMinimumAreaToBreakDown(double minimumAreaToBreakDown)
        {
            _minimumAreaToBreakDown = minimumAreaToBreakDown;
        }

        public void ClusterProperties(IEnumerable<SupplySummary> supplies)
        {
            foreach (var supply in supplies.EmptyIfNull())
            {
                if (supply.Property.GeographicLocation == null)
                    continue;

                Vicinity vicinity = null;
                if (supply.Property?.Vicinity?.ID != null)
                    vicinity = VicinityCache[supply.Property.Vicinity.ID];
                if (vicinity == null)
                    continue;

                var distinguishableVicinity = FindDistinguishableVicinity(vicinity, _minimumAreaToBreakDown);
                if (distinguishableVicinity == null)
                    continue;

                var centerPoint = GetVicinityCenterPoint(distinguishableVicinity) ?? GetVicinityCenterPoint(vicinity);
                if (centerPoint == null)
                    continue;


                // If the listing's vicinity is large enough that the items inside it can be distinguished,
                // return the property listing as an individual. Otherwise, aggregate it in the parent vicinities.

                if (vicinity.ID == distinguishableVicinity.ID)
                {
                    if (_individualPropertiesInVicinity.ContainsKey(vicinity.ID))
                        _individualPropertiesInVicinity[vicinity.ID].Add(supply);
                    else
                        _individualPropertiesInVicinity[vicinity.ID] = new List<SupplySummary> {supply};
                }
                else
                {
                    PlaceResultInVicinityCluster(distinguishableVicinity.ID, centerPoint, supply);
                }
            }
        }

        public void PrepareResults(GeoSearchResult result)
        {
            foreach (var vicinityIndividualResults in _individualPropertiesInVicinity)
            {
                // Checking to see if we need to cluster individual items in a vicinity.
                // For example, if there are many individual results in a vicinity that occupies a small
                // area on the user screen

                var properties = vicinityIndividualResults.Value;
                if (properties == null)
                    continue;

                // If there are too few results in a single vicinity, don't cluster no matter how small
                if (properties.Count < MinIndividualItemsInVicinityToGroup)
                    continue;

                // Extract vicinity area
                var vicinityId = vicinityIndividualResults.Key;
                var vicinity = VicinityCache[vicinityId];
                var vicinityArea = GetVicinityAreaEstimate(vicinity);

                if (vicinityArea <= 0)
                    continue;

                // If the area share for each item is not too small, don't cluster and report items individually
                var dividedArea = vicinityArea/properties.Count;
                if (dividedArea > _minimumAreaToBreakDown*MinVicinityAreaCoefficientForIndividualItemsGrouping)
                    continue;

                // If the vicinity is not displayed too large on the screen, group all items within it 
                // in a single result item.

                // TODO: Uncomment the condition below when clustering is implemented
                //if (vicinityArea <= _minimumAreaToBreakDown*MinCoefficientForIntraVicinityClustering)
                //{
                var centerPoint = GetVicinityCenterPoint(vicinity) ?? GetPropertiesCenterPoint(properties);


                foreach (var listing in properties)
                {
                    PlaceResultInVicinityCluster(vicinityId, centerPoint, listing);
                }

                properties.Clear();
                //}
                //else
                //{
                //	TODO: Cluster
                //}
            }

            result.SupplyGroupsSummaries = _vicinityResultItems.Values.ToList();
            result.SupplySummaries = _vicinitySingleProperties.Values
                .Concat(_individualPropertiesInVicinity.SelectMany(pair => pair.Value))
                .ToList();
        }

        private void PlaceResultInVicinityCluster(ObjectId vicinityId, LatLng centerPoint, SupplySummary supply)
        {
            // If this is the first one in the vicinity, we want to return it individually.
            // Otherwise - when there is more than one listing in the same vicinity -
            // group them together in a vicinity.

            if (_vicinityResultItems.ContainsKey(vicinityId))
            {
                _vicinityResultItems[vicinityId].NumberOfPropertyListings++;
            }
            else if (_vicinitySingleProperties.ContainsKey(vicinityId))
            {
                _vicinityResultItems.Add(vicinityId,
                    BuildGeoSearchVicinityResult(vicinityId, centerPoint));
                _vicinityResultItems[vicinityId].NumberOfPropertyListings += 2;
                // One from before, one is being added now

                _vicinitySingleProperties.Remove(vicinityId);
            }
            else
            {
                _vicinitySingleProperties.Add(vicinityId, supply);
            }
        }

        private Vicinity FindDistinguishableVicinity(Vicinity vicinity, double minimumAreaToBreakDown)
        {
            var hierarchy = VicinityCache.GetParentsInclusive(vicinity.ID).Reverse();
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

            return vicinity.BoundaryArea.GetValueOrDefault()*multiplier;
        }

        private GeoSearchVicinityResult BuildGeoSearchVicinityResult(ObjectId vicinityId, LatLng centerPoint)
        {
            var vicinity = VicinityCache[vicinityId];
            return new GeoSearchVicinityResult
            {
                VicinityID = vicinityId,
                Name = vicinity.Name,
                GeographicLocation = centerPoint,
                NumberOfPropertyListings = 0
            };
        }

        private static LatLng GetVicinityCenterPoint(Vicinity vicinity)
        {
            if (vicinity.CenterPoint != null)
            {
                return new LatLng
                {
                    Lat = vicinity.CenterPoint.Y,
                    Lng = vicinity.CenterPoint.X
                };
            }
            var x = (vicinity.Boundary.BoundingBox.Max.X + vicinity.Boundary.BoundingBox.Min.X)/2;
            var y = (vicinity.Boundary.BoundingBox.Max.Y + vicinity.Boundary.BoundingBox.Min.Y)/2;

            return new LatLng
            {
                Lat = y,
                Lng = x
            };
        }

        private static LatLng GetPropertiesCenterPoint(List<SupplySummary> supplies)
        {
            var coordinates = supplies.Select(s => s.Property.GeographicLocation).Where(c => c != null).ToList();
            var linearRingCoordinates =
                new GeoJsonLinearRingCoordinates<GeoJson2DCoordinates>(coordinates);
            var polygonCoordinates =
                new GeoJsonPolygonCoordinates<GeoJson2DCoordinates>(linearRingCoordinates);
            var polygon = new GeoJsonPolygon<GeoJson2DCoordinates>(polygonCoordinates);

            var x = (polygon.BoundingBox.Max.X + polygon.BoundingBox.Min.X)/2;
            var y = (polygon.BoundingBox.Max.Y + polygon.BoundingBox.Min.Y)/2;

            return new LatLng
            {
                Lat = y,
                Lng = x
            };
        }
    }
}