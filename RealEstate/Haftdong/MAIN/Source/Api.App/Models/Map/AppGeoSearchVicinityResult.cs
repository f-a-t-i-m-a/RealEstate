using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.RealEstateAgency.Util.Models.Map;
using MongoDB.Bson;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Map
{
    [AutoMapperConfig]
    public class AppGeoSearchVicinityResult
    {
        public ObjectId VicinityID { get; set; }
        public string Name { get; set; }
        public LatLng GeographicLocation { get; set; }

        public int NumberOfPropertyListings { get; set; }

        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<GeoSearchVicinityResult, AppGeoSearchVicinityResult>()
                .IgnoreUnmappedProperties();
        }
    }
}
