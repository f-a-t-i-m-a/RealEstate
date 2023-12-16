using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Domain.Audit;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminVisitors
{
	public class AdminVisitorsListModel
	{
		public PagedList<AdminVisitorsVisitorModel> Visitors { get; set; }
		public string Page { get; set; }

		public long? IdFilter { get; set; }
		public long? UserIdFilter { get; set; }
		public string UniqueIdentifierFilter { get; set; }
	}
}