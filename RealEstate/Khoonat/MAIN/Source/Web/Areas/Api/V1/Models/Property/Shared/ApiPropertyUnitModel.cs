using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Property.Shared
{
	public class ApiPropertyUnitModel
	{
		//
		// Address

		public int? FloorNumber { get; set; }

		//
		// Number of Rooms

		public int? NumberOfRooms { get; set; }
		public int? NumberOfParkings { get; set; }
		public int? NumberOfLavatories { get; set; }
		public int? NumberOfBathrooms { get; set; }
		public int? NumberOfKitchens { get; set; }
		public int? NumberOfClosets { get; set; }
		public int? NumberOfBalconies { get; set; }

		//
		// Specifications

		public UnitUsageType? UsageType { get; set; }
		public decimal? Area { get; set; }
		public decimal? KitchenArea { get; set; }
		public decimal? BalconyArea { get; set; }
		public decimal? StorageRoomArea { get; set; }
		public decimal? CurrentMonthlyChargeAmount { get; set; }

		//
		// Features

		public bool? IsDuplex { get; set; }
		public bool? IsFurnished { get; set; }
		public bool? HasHalfFloorInside { get; set; }
		public bool? HasIranianLavatory { get; set; }
		public bool? HasForeignLavatory { get; set; }
		public bool? HasSeparateLivingRoom { get; set; }
		public bool? HasPrivatePatio { get; set; }
		public bool? HasPrivateGarden { get; set; }
		public bool? HasSecuritySystem { get; set; }
		public bool? HasOpenKitchen { get; set; }
		public bool? HasFurnishedKitchen { get; set; }
		public KitchenCabinetTopType? KitchenCabinetTopType { get; set; }
		public KitchenCabinetBodyType? KitchenCabinetBodyType { get; set; }
		public string AdditionalSpecialFeatures { get; set; }

		//
		// Covers and Windows

		public DaylightDirection? MainDaylightDirection { get; set; }
		public FloorCoverType? LivingRoomFloor { get; set; }
		public WallCoverType? LivingRoomWalls { get; set; }
		public FloorCoverType? RoomsFloor { get; set; }
		public WallCoverType? RoomsWall { get; set; }
		public WindowType? WindowType { get; set; }

		//
		// Heating and Cooling

		public HeatingSystemType? HeatingSystem { get; set; }
		public bool? HasIndependentHeatingPackage { get; set; }
		public CoolingSystemType? CoolingSystem { get; set; }
		public bool? HasIndependentCoolingPackage { get; set; }
		public bool? HasFireplace { get; set; }
		public bool? WindowsHasDoubleGlass { get; set; }
		public bool? HasInsulatedWalls { get; set; }
		public bool? HasIndependentGasometer { get; set; }
		public string AdditionalHeatingAndCoolingInformation { get; set; }

		//
		// Current conditions

		public bool? HasBeenReconstructed { get; set; }
		public bool? NeedsDestruction { get; set; }
		public bool? NeedsReconstruction { get; set; }
		public bool? NeedsPainting { get; set; }

		//
		// Luxury features

		public int? NumberOfMasterBedrooms { get; set; }
		public decimal? CeilingHeight { get; set; }
		public bool? HasAllSideView { get; set; }
		public bool? HasAllSideBalcony { get; set; }
		public bool? HasPrivatePool { get; set; }
		public bool? HasPrivateElevator { get; set; }
		public bool? HasInUnitParking { get; set; }
		public bool? HasOpenningCeiling { get; set; }
		public bool? HasMobileWall { get; set; }
		public bool? HasGardenInBalcony { get; set; }
		public bool? HasRemoteControlledCurtains { get; set; }
		public bool? HasBuildingManagementSystem { get; set; }
		public bool? HasPrivateJanitorUnit { get; set; }
		public bool? HasGuestSuite { get; set; }
		public string AdditionalLuxuryFeatures { get; set; }

		public static void ConfigureMapper()
		{
			Mapper.CreateMap<Unit, ApiPropertyUnitModel>();

			Mapper.CreateMap<ApiPropertyUnitModel, PropertyListing>()
				.IgnoreAll()
				.ForMember(l => l.Unit, opt => opt.MapFrom(m => m));

			Mapper.CreateMap<ApiPropertyUnitModel, Unit>()
				.Ignore(u => u.ID)
				.Ignore(u => u.BuildingID)
				.Ignore(u => u.Building);
		}
	}
}