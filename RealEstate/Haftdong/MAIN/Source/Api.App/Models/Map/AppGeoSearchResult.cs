using System.Collections.Generic;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.RealEstateAgency.Api.App.Models.Supply;
using JahanJooy.RealEstateAgency.Util.Models.Map;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Map
{
    [TsClass]
    [AutoMapperConfig]
    public class AppGeoSearchResult
    {
        public LatLngBounds LargestContainedRect { get; set; }
        public double LargestContainedRectArea { get; set; }
        public double MinimumDistinguishedArea { get; set; }

        public List<AppGeoSearchVicinityResult> SupplyGroupsSummaries { get; set; }
        public List<AppSupplySummary> SupplySummaries { get; set; }
        public bool ReachedMaxResult { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<GeoSearchResult, AppGeoSearchResult>()
                .IgnoreUnmappedProperties();
        }
    }
}
