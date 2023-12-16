using JahanJooy.RealEstate.Core.Services.Dto.Property;

namespace JahanJooy.RealEstate.Web.Models.Property
{
	public class ViewPropertyDetailsDetailsTabModel
	{
		public PropertyListingDetails Listing { get; set; }
		public PropertyListingSummary ListingSummary { get; set; }
		public bool IsOwner { get; set; }
	}
}