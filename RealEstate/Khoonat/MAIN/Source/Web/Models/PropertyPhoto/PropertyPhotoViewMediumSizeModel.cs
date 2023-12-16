using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Web.Models.PropertyPhoto
{
	public class PropertyPhotoViewMediumSizeModel
	{
		public PropertyListingPhoto Photo { get; set; }
		public PropertyListing Listing { get; set; }
		public bool IsOwner { get; set; }
	}
}