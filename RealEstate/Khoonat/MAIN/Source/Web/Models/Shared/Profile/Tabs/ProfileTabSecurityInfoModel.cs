using System.Collections.Generic;
using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Domain.Audit;

namespace JahanJooy.RealEstate.Web.Models.Shared.Profile.Tabs
{
	public class ProfileTabSecurityInfoModel
	{
		public PagedList<HttpSession> Sessions { get; set; }
		public string PaginationUrlTemplate { get; set; }

		public IEnumerable<PasswordResetRequest> LatestPasswordResets { get; set; }
		public IEnumerable<LoginNameQuery> LatestLoginNameQueries { get; set; }

		public bool AdministrativeView { get; set; }
	}
}