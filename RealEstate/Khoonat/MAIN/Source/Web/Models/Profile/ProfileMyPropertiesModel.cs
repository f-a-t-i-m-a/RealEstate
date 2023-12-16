using System.Collections.Generic;
using JahanJooy.RealEstate.Core.Services.Dto.Property;

namespace JahanJooy.RealEstate.Web.Models.Profile
{
	public class ProfileMyPropertiesModel
	{
		public IEnumerable<PropertyListingSummary> Listings { get; set; }
	}
}