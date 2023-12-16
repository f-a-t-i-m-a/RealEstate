using System.Collections.Generic;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.RealEstateAgency.Util.Models.Supplies;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Map
{
    [TsClass]
    public class GeoSearchResult
    {
        public LatLngBounds LargestContainedRect { get; set; }
        public double LargestContainedRectArea { get; set; }
        public double MinimumDistinguishedArea { get; set; }

        public List<GeoSearchVicinityResult> SupplyGroupsSummaries { get; set; }
        public List<SupplySummary> SupplySummaries { get; set; }
        public bool ReachedMaxResult { get; set; }
    }
}
