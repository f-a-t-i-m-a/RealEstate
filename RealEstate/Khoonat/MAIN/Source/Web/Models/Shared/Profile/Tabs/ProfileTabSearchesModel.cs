using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Domain.Audit;

namespace JahanJooy.RealEstate.Web.Models.Shared.Profile.Tabs
{
	public class ProfileTabSearchesModel
	{
		public PagedList<PropertySearchHistory> Searches { get; set; }
		public string PaginationUrlTemplate { get; set; }
	}
}