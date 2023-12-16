using JahanJooy.Common.Util.Spatial;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Vicinities
{
    [TsClass]
    public class SearchVicinityByPointInput
    {
        public LatLng UserPoint { get; set; }
    }
}
