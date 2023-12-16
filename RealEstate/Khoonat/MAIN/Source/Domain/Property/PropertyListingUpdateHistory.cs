using System;
using JahanJooy.RealEstate.Domain.Audit;

namespace JahanJooy.RealEstate.Domain.Property
{
	public class PropertyListingUpdateHistory
	{
		public long ID { get; set; }

		public DateTime UpdateTime { get; set; }
		public string UpdateDetails { get; set; }

		public long PropertyListingID { get; set; }
		public PropertyListing PropertyListing { get; set; }

		public long? SessionID { get; set; }
		public HttpSession Session { get; set; }

		public long? UserID { get; set; }
		public User User { get; set; }
	}
}