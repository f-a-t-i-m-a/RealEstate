using System.ComponentModel.DataAnnotations;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Domain.Property;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Shared;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Shared.Spatial;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Property.Shared
{
	public class ApiPropertyGeneralModel
	{
		[Required]
		public PropertyType PropertyType { get; set; }
		[Required]
		public IntentionOfOwner IntentionOfOwner { get; set; }

		public bool IsAgencyListing { get; set; }
		public bool IsAgencyActivityAllowed { get; set; }

		public ApiOutputVicinityModel VicinityDetails { get; set; }
		public long? VicinityID { get; set; }

		public string Address { get; set; }
		public string AdditionalAddressInfo { get; set; }

		public ApiGeoPoint GeographicLocation { get; set; }
		public GeographicLocationSpecificationType? GeographicLocationType { get; set; }

		public static void ConfigureMapper()
		{
			Mapper.CreateMap<PropertyListingDetails, ApiPropertyGeneralModel>()
				.ForMember(m => m.VicinityDetails, opt => opt.ResolveUsing(ApiOutputVicinityModel.VicinityIDValueResolver).FromMember(d => d.VicinityID))
				.ForMember(m => m.Address, opt => opt.MapFrom(d => d.Estate.Address))
				.ForMember(m => m.AdditionalAddressInfo, opt => opt.MapFrom(d => d.Estate.AdditionalAddressInfo));

			Mapper.CreateMap<ApiPropertyGeneralModel, PropertyListing>()
				.IgnoreSource(m => m.VicinityDetails)
				.ForMember(l => l.Estate, opt => opt.MapFrom(m => m))
				.IgnoreUnmappedProperties();

			Mapper.CreateMap<ApiPropertyGeneralModel, Estate>()
				.IgnoreSource(m => m.PropertyType)
				.IgnoreSource(m => m.IntentionOfOwner)
				.IgnoreSource(m => m.IsAgencyListing)
				.IgnoreSource(m => m.IsAgencyActivityAllowed)
				.IgnoreSource(m => m.VicinityDetails)
				.IgnoreUnmappedProperties();
		}

	}
}