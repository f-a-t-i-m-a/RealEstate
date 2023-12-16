using System;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Property.Shared
{
	public class ApiPropertyDeliveryModel
	{
		public PropertyStatus? PropertyStatus { get; set; }
		public DateTime? RenterContractEndDate { get; set; }
		public DeliveryDateSpecificationType? DeliveryDateSpecificationType { get; set; }
		public DateTime? AbsoluteDeliveryDate { get; set; }
		public int? DeliveryDaysAfterContract { get; set; }

		public static void ConfigureMapper()
		{
			Mapper.CreateMap<PropertyListingDetails, ApiPropertyDeliveryModel>();

			Mapper.CreateMap<ApiPropertyDeliveryModel, PropertyListing>()
				.IgnoreUnmappedProperties();
		}
	}
}