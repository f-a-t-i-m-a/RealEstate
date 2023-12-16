using JahanJooy.Common.Util.Spatial;
using MongoDB.Bson;

namespace JahanJooy.RealEstateAgency.Util.Models.Map
{
    public class GeoSearchVicinityResult
    {
        public ObjectId VicinityID { get; set; }
        public string Name { get; set; }
        public LatLng GeographicLocation { get; set; }

        public int NumberOfPropertyListings { get; set; }
    }
}
