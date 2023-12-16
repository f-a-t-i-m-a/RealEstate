using JahanJooy.RealEstate.Domain.Directory;
using JahanJooy.RealEstate.Domain.Enums;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminAgency
{
    public class AdminAgencyModel
	{
		public long? AgencyID { get; set; }
		public long? MainBranchID { get; set; }

        public AgencyContent Agency { get; set; }
		public AgencyBranchContent MainBranch { get; set; }

        public double? UserPointLat { get; set; }
        public double? UserPointLng { get; set; }
        public GeographicLocationSpecificationType? GeographicLocationType { get; set; }
	}
}