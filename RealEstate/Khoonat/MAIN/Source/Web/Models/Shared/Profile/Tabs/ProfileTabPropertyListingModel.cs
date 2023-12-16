using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Core.Services.Dto.Property;

namespace JahanJooy.RealEstate.Web.Models.Shared.Profile.Tabs
{
	public class ProfileTabPropertyListingModel
	{
		public PagedList<PropertyListingSummary> PropertyListings { get; set; }
		public string PaginationUrlTemplate { get; set; }

		public ProfileModel.ProfileActiveTab ActiveTab { get; set; }
		public bool EnableEdit { get; set; }
	}
}