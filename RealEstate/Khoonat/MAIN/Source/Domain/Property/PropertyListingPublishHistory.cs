using System;
using JahanJooy.RealEstate.Domain.Audit;

namespace JahanJooy.RealEstate.Domain.Property
{
	public class PropertyListingPublishHistory
	{
		public long ID { get; set; }

		public DateTime PublishDate { get; set; }
		public int PublishDays { get; set; }

		public DateTime? PreviousPublishDate { get; set; }
		public DateTime? PreviousPublishEndDate { get; set; }

		public long PropertyListingID { get; set; }
		public PropertyListing PropertyListing { get; set; }

		public long? SessionID { get; set; }
		public HttpSession Session { get; set; }

		public long? UserID { get; set; }
		public User User { get; set; }
	}
}