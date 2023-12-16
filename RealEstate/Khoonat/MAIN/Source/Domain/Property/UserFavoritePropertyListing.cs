using System;
using JahanJooy.RealEstate.Domain.Audit;

namespace JahanJooy.RealEstate.Domain.Property
{
	public class UserFavoritePropertyListing
	{
		public long ID { get; set; }
		public DateTime CreationDate { get; set; }

		public HttpSession CreationSession { get; set; }
		public long CreationSessionID { get; set; }

		public User User { get; set; }
		public long UserID { get; set; }

		public PropertyListing Listing { get; set; }
		public long ListingID { get; set; }
	}
}