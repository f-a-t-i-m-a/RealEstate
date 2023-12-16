using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Property.Shared
{
	public class ApiPropertyEstateModel
	{
		//
		// Specification

		public decimal? Area { get; set; }
		public EstateDirection? Direction { get; set; }
		public EstateVoucherType? VoucherType { get; set; }

		public int? Sides { get; set; }
		public decimal? PassageEdgeLength { get; set; }
		public decimal? PassageWidth { get; set; }

		public bool? IsInDeadEnd { get; set; }
		public bool? IsCloseToHighway { get; set; }
		public bool? IsOnMainPassage { get; set; }
		public bool? IsOnGreenPassage { get; set; }
		public bool? IsCloseToPark { get; set; }

		public EstateSlopeAmount? SlopeAmount { get; set; }
		public EstateSurfaceType? SurfaceType { get; set; }

		//
		// Utilities

		public bool? HasElectricity { get; set; }
		public bool? HasThreePhaseElectricity { get; set; }
		public bool? HasIndustrialElectricity { get; set; }
		public bool? HasDrinkingWater { get; set; }
		public bool? HasCultivationWater { get; set; }
		public bool? HasGasPiping { get; set; }
		public bool? HasSewerExtension { get; set; }
		public bool? HasWaterWells { get; set; }
		public bool? HasWaterWellsPrivilege { get; set; }
		public bool? HasNiches { get; set; }

		public static void ConfigureMapper()
		{
			Mapper.CreateMap<Estate, ApiPropertyEstateModel>();

			Mapper.CreateMap<ApiPropertyEstateModel, PropertyListing>()
				.IgnoreAll()
				.ForMember(l => l.Estate, opt => opt.MapFrom(m => m));

			Mapper.CreateMap<ApiPropertyEstateModel, Estate>()
				.Ignore(e => e.ID)
				.Ignore(e => e.VicinityID)
				.Ignore(e => e.Vicinity)
				.Ignore(e => e.Address)
				.Ignore(e => e.AdditionalAddressInfo)
				.Ignore(e => e.GeographicLocation)
				.Ignore(e => e.GeographicLocationType)
				.Ignore(e => e.Buildings);
		}
	}
}