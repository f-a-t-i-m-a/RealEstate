using System.Linq;
using JahanJooy.Common.Util.Spatial;
using MongoDB.Driver.GeoJsonObjectModel;

namespace JahanJooy.RealEstateAgency.Util.Map
{
   public  class GeographyUtils
    {
        public static LatLngBounds FindBoundingBox(GeoJsonPolygon<GeoJson2DCoordinates> coordinates)
        {
            if (coordinates == null)
                return null;
        
            var xValues = coordinates.Coordinates.Exterior.Positions.Select(c => c.X).ToList();
            var yValues = coordinates.Coordinates.Exterior.Positions.Select(c => c.Y).ToList();


            double minLat = yValues.Min();
            double minLng = xValues.Min();
            double maxLat = yValues.Max();
            double maxLng = xValues.Min();

            return new LatLngBounds { SouthWest = new LatLng { Lat = minLat, Lng = minLng }, NorthEast = new LatLng { Lat = maxLat, Lng = maxLng } };
        }
    }
}
