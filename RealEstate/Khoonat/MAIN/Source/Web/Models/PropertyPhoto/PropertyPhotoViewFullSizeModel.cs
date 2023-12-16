using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Web.Models.PropertyPhoto
{
	public class PropertyPhotoViewFullSizeModel
	{
		public PropertyListingPhoto Photo { get; set; }
		public PropertyListing Listing { get; set; }

		public long? PrevID { get; set; }
		public long? NextID { get; set; }

		public int Index { get; set; }
		public int Count { get; set; }
	}
}