using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Domain.Audit;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminAbuseFlags
{
	public class AdminAbuseFlagsListModel
	{
		public PagedList<AbuseFlag> Flags { get; set; }
		public string Page { get; set; }

		public long? IdFilter { get; set; }
		public bool ApplyUserIdFilter { get; set; }
		public long? UserIdFilter { get; set; }
		public long? SessionIdFilter { get; set; }

		public AbuseFlagReason? ReasonFilter { get; set; }
		public AbuseFlagEntityType? EntityTypeFilter { get; set; }
		public long? EntityIdFilter { get; set; }

        public bool ApprovedFilter { get; set; }
	}
}