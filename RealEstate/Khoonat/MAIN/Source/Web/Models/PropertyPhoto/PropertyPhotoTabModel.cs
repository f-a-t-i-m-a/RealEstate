using System.Collections.Generic;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Web.Models.PropertyPhoto
{
	public class PropertyPhotoTabModel
	{
		public PropertyListing Listing { get; set; }
		public List<PropertyListingPhoto> Photos { get; set; }
		public long? SelectedPhotoID { get; set; }

		public bool IsOwner { get; set; }
	}
}