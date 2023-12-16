using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Core.Services.Dto.Audit;
using JahanJooy.RealEstate.Domain.Audit;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminActivityLog
{
	public class AdminActivityLogListModel
	{
		public PagedList<ActivityLogDisplayInfo> Logs { get; set; }
		public string Page { get; set; }

		public long? SessionIdFilter { get; set; }
		public ActivityAction? ActionFilter { get; set; }
		
		public bool ApplyAuthenticatedUserIdFilter { get; set; }
		public long? AuthenticatedUserIdFilter { get; set; }

		public TargetEntityType? TargetEntityFilter { get; set; }
		public bool ApplyTargetEntityIdFilter { get; set; }
		public long? TargetEntityIdFilter { get; set; }

		public int? MinimumReviewWeightFilter { get; set; }
	}
}