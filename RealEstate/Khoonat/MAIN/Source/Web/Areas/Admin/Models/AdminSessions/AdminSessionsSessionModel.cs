using JahanJooy.RealEstate.Domain.Audit;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminSessions
{
	public class AdminSessionsSessionModel : HttpSession
	{
		public long ActivityLogCount { get; set; }
		public string UserLoginName { get; set; }
	}
}