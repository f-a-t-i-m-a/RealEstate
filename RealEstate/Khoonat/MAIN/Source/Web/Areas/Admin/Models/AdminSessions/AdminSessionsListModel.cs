using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Domain.Enums;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminSessions
{
	public class AdminSessionsListModel
	{
		public PagedList<AdminSessionsSessionModel> Sessions { get; set; }
		public string Page { get; set; }

		public long? IdFilter { get; set; }
		public bool ApplyUserIdFilter { get; set; }
		public long? UserIdFilter { get; set; }
		public long? VisitorIdFilter { get; set; }
		public bool ApplyEndReasonFilter { get; set; }
		public SessionEndReason? EndReasonFilter { get; set; }
		public bool? InteractiveSessionFilter { get; set; }

		public string HttpSessionIdFilter { get; set; }
		public string UserAgentFilter { get; set; }
		public string StartupUriFilter { get; set; }
		public string ReferrerUriFilter { get; set; }
		public string ClientAddressFilter { get; set; }

		public int? MinimumActivityCount { get; set; }
	}
}