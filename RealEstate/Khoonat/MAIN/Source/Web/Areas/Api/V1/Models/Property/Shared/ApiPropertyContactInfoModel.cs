using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Property.Shared
{
	public class ApiPropertyContactInfoModel
	{
		public string AgencyName { get; set; }
		public string AgencyAddress { get; set; }
		public string ContactName { get; set; }
		public string ContactPhone1 { get; set; }
		public string ContactPhone2 { get; set; }
		public string ContactEmail { get; set; }

		public bool ContactPhone1Verified { get; set; }
		public bool ContactPhone2Verified { get; set; }
		public bool ContactEmailVerified { get; set; }

		public bool OwnerCanBeContacted { get; set; }
		public string OwnerName { get; set; }
		public string OwnerPhone1 { get; set; }
		public string OwnerPhone2 { get; set; }
		public string OwnerEmail { get; set; }

		public static void ConfigureMapper()
		{
			Mapper.CreateMap<PropertyListingContactInfo, ApiPropertyContactInfoModel>();

			Mapper.CreateMap<ApiPropertyContactInfoModel, PropertyListing>()
				.IgnoreAll()
				.ForMember(l => l.ContactInfo, opt => opt.MapFrom(m => m));

			Mapper.CreateMap<ApiPropertyContactInfoModel, PropertyListingContactInfo>()
				.Ignore(ci => ci.ID);
		}
	}
}