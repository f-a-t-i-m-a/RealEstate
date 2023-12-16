using System.Collections.Generic;
using JahanJooy.RealEstate.Core.Services.Dto.Property;

namespace JahanJooy.RealEstate.Web.Models.Home
{
	public class HomeIndexModel
	{
		public IEnumerable<PropertyListingSummary> NewestListings { get; set; }
		public IEnumerable<PropertyListingSummary> MostPopularListings { get; set; }
	}
}