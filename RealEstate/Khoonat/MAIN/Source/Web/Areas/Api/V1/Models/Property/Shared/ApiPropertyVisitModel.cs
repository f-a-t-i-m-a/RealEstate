using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Property.Shared
{
	public class ApiPropertyVisitModel
	{
		public string AppropriateVisitTimes { get; set; }
		public string InappropriateVisitTimes { get; set; }
		public bool ShouldCoordinateBeforeVisit { get; set; }
		public string HowToCoordinateBeforeVisit { get; set; }

		public static void ConfigureMapper()
		{
			Mapper.CreateMap<PropertyListingDetails, ApiPropertyVisitModel>();

			Mapper.CreateMap<ApiPropertyVisitModel, PropertyListing>()
				.IgnoreUnmappedProperties();
		}
	}
}