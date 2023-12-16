using JahanJooy.RealEstate.Domain.Audit;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminVisitors
{
	public class AdminVisitorsVisitorModel : UniqueVisitor
	{
		public int SessionCount { get; set; }
		public int UniqueUsers { get; set; }
		public string FirstUserAgent { get; set; }
	}
}