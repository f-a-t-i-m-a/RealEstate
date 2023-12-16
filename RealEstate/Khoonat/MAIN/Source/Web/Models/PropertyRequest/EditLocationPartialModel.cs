
using System.Data.Entity.Spatial;
using JahanJooy.Common.Util.Spatial;

namespace JahanJooy.RealEstate.Web.Models.PropertyRequest
{
    public class EditLocationPartialModel
    {
        public long? ID { get; set; }
        public DbGeography CenterPoint { get; set; }
        public DbGeography Boundary { get; set; }
        public SpatialBoundary ParentBoundary { get; set; }

        public string BoundaryWktId { get; set; }
        public string CenterPointWktId { get; set; }
        public string ToggleBoundaryEditButtonId { get; set; }

        public string AdjacentBoundariesActionUrl { get; set; }
    }
}