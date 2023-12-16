using System.Collections.Generic;
using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Domain.Ad;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Web.Models.Property
{
	public class PropertyViewDetailsModel
	{
		public PropertyViewDetailsParamsModel Params { get; set; }
		public string PrevUrl { get; set; }
		public string ReturnUrl { get; set; }
		public string NextUrl { get; set; }

		public PropertyListingDetails Listing { get; set; }
		public PropertyListingSummary ListingSummary { get; set; }
		public PropertyListingPhoto CoverPhoto { get; set; }
		public bool IsOwner { get; set; }
        public SponsoredPropertyListing SponsoredPropertyListing { get; set; }
       
	}
}