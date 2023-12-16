using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Domain;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminUsers
{
	public class AdminUsersListModel
	{
		public PagedList<User> Users { get; set; }
        public string Page { get; set; }

        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        
	}
}