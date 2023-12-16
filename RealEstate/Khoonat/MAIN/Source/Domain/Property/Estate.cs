using System.Collections.Generic;
using System.Data.Entity.Spatial;
using JahanJooy.RealEstate.Domain.Enums;

namespace JahanJooy.RealEstate.Domain.Property
{
	public class Estate
	{
		public long ID { get; set; }

		//
		// Location

        public long? VicinityID { get; set; }
        public Vicinity Vicinity { get; set; }
        
        public string Address { get; set; }
		public string AdditionalAddressInfo { get; set; }

		public DbGeography GeographicLocation{ get; set; }
		public GeographicLocationSpecificationType? GeographicLocationType { get; set; }

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

		//
		// Related entities

		public ICollection<Building> Buildings { get; set; } // TODO: Remove this relationship

	}
}