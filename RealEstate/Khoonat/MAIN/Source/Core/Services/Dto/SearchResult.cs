using System.Collections.Generic;
using JahanJooy.RealEstate.Core.Services.Dto.Property;

namespace JahanJooy.RealEstate.Core.Services.Dto
{
	public class SearchResult
	{
		public int PageSize { get; set; }
		public int TotalNumberOfRecords { get; set; }
		public int IndexOfFirstResult { get; set; }
		public int IndexOfLastResult { get; set; }

		public List<PropertyListingSummary> PageResults { get; set; }
        public List<SponsoredPropertyListingSummary> SponsoredResults { get; set; } 
	}
}