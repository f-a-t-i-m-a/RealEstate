using JahanJooy.RealEstate.Domain.Directory;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminAgency
{
	public class AdminAgencyBranchModel
	{
		public long? AgencyID { get; set; }
		public long? BranchID { get; set; }

		public AgencyBranchContent Branch { get; set; }
	}
}