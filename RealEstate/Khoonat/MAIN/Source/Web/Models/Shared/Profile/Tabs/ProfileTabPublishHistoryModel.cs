using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Web.Models.Shared.Profile.Tabs
{
	public class ProfileTabPublishHistoryModel
	{
		public PagedList<PropertyListingPublishHistory> PropertyPublishes { get; set; }
		public string PaginationUrlTemplate { get; set; }
	}
}