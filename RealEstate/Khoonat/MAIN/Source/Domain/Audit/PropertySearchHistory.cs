using System;
using JahanJooy.RealEstate.Domain.Enums;

namespace JahanJooy.RealEstate.Domain.Audit
{
	public class PropertySearchHistory
	{
		public long ID { get; set; }

		//
		// Search criteria

		public DateTime SearchDate { get; set; }
		public string SearchQuery { get; set; }
		public int RequestedPage { get; set; }

		//
		// Session properties

		public long? SessionID { get; set; }
		public HttpSession Session { get; set; }

		public long? UserID { get; set; }
		public User User { get; set; }

		//
		// Redundant properties for reporting

		public PropertySearchSortOrder? SortOrder { get; set; }
		public PropertyType? PropertyType { get; set; }
		public IntentionOfOwner? IntentionOfOwner { get; set; }

		public long? ProvinceID { get; set; }
		public long? CityID { get; set; }
		public long? CityRegionID { get; set; }
		public long? NeighborhoodID { get; set; }
	}
}