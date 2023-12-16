using System.Collections.Generic;
using System.Linq;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.RealEstate.Domain.Directory;
using JahanJooy.RealEstate.Util.Presentation.Property;
using JahanJooy.RealEstate.Web.Application.DomainModel;

namespace JahanJooy.RealEstate.Web.Models.Map
{
    public class MapSearchAgencyBranchItemModel
    {
        public long AgencyID { get; set; }

        public LatLng Point { get; set; }

        public string Title { get; set; }

        public string Phone1 { get; set; }

        public string Address { get; set; }


        public static MapSearchAgencyBranchItemModel FromDto(AgencyBranch dto)
        {
            return new MapSearchAgencyBranchItemModel
            {
                AgencyID = dto.AgencyID,
                Point = dto.GetContent().GeographicLocation.ToLatLng(),
                Title = dto.Agency.GetContent().Name,
                Phone1 = dto.GetContent().Phone1,
                Address = dto.GetContent().FullAddress
            };
        }

        public static IEnumerable<MapSearchAgencyBranchItemModel> FromDto(IEnumerable<AgencyBranch> agencies)
        {
            return (agencies ?? Enumerable.Empty<AgencyBranch>()).Select(FromDto);
        }
    }
}