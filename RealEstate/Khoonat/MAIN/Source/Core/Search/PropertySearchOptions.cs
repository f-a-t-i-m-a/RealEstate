using System.Collections.Generic;
using System.Linq;
using JahanJooy.RealEstate.Domain.Enums;

namespace JahanJooy.RealEstate.Core.Search
{
	public static class PropertySearchOptions
	{
		private static readonly IDictionary<string, PropertySearchOption> EstateOptionsPrivate;
		private static readonly IDictionary<string, PropertySearchOption> BuildingOptionsPrivate;
		private static readonly IDictionary<string, PropertySearchOption> UnitOptionsPrivate;
		private static readonly IDictionary<string, PropertySearchOption> OtherOptionsPrivate;

		private static readonly IDictionary<string, PropertySearchOption> AllOptionsPrivate;

		public const string OptionHasElevator = "bo.ele";
		public const string OptionHasParking = "uo.prk";
		public const string OptionHasStorageRoom = "uo.strg";
		
		public const string OptionHasPhotos = "o.photo";
		public const string OptionUserIsOwner = "u.owner";
		public const string OptionUserHasStarred = "u.star";

        public const string OptionNoAgencyListing = "o.nal";
        public const string OptionAgencyActivityAllowed = "o.aaa";

		static PropertySearchOptions()
		{
			var estateOptionsList =
				new List<PropertySearchOption>
				{
					new PropertySearchOption("EstateDirectionIsNorth", "eo.dirn", l => l.Estate != null && l.Estate.Direction == EstateDirection.North),
					new PropertySearchOption("EstateDirectionIsSouth", "eo.dirs", l => l.Estate != null && l.Estate.Direction == EstateDirection.South),
					new PropertySearchOption("EstateDirectionIsBoth", "eo.dirb", l => l.Estate != null && l.Estate.Direction == EstateDirection.NorthAndSouth),
					new PropertySearchOption("EstateVoucherTypeIsNormal", "eo.vtn", l => l.Estate != null && l.Estate.VoucherType == EstateVoucherType.Normal),
					new PropertySearchOption("EstateHasTwoSidesMinimum", "eo.2side", l => l.Estate != null && l.Estate.Sides >= 2),
					new PropertySearchOption("EstateHasThreeSidesMinimum", "eo.3side", l => l.Estate != null && l.Estate.Sides >= 3),
					new PropertySearchOption("IsInDeadEnd", "eo.ddnd", l => l.Estate != null && l.Estate.IsInDeadEnd == true),
					new PropertySearchOption("IsNotInDeadEnd", "eo.nddnd", l => l.Estate != null && l.Estate.IsInDeadEnd == false),
					new PropertySearchOption("IsNotCloseToHighway", "eo.nchw", l => l.Estate != null && l.Estate.IsCloseToHighway == false),
					new PropertySearchOption("IsOnMainPassage", "eo.omp", l => l.Estate != null && l.Estate.IsOnMainPassage == true),
					new PropertySearchOption("IsNotOnMainPassage", "eo.nomp", l => l.Estate != null && l.Estate.IsOnMainPassage == false),
					new PropertySearchOption("IsOnGreenPassage", "eo.ogp", l => l.Estate != null && l.Estate.IsOnGreenPassage == true),
					new PropertySearchOption("IsCloseToPark", "eo.ctp", l => l.Estate != null && l.Estate.IsCloseToPark == true),
					new PropertySearchOption("EstateHasLowSlope", "eo.slpl", l => l.Estate != null && (byte) (l.Estate.SlopeAmount) <= 5),
				};

			var buildingOptionsList =
				new List<PropertySearchOption>
				{
					new PropertySearchOption("IsNotInConstruction", "bo.ncnst", l => l.PropertyStatus != PropertyStatus.InConstruction),
					new PropertySearchOption("BuildingHasLessThan5Units", "bo.le5u", l => l.Building != null && l.Building.TotalNumberOfUnits <= 5),
					new PropertySearchOption("BuildingHasLessThan10Units", "bo.le10u", l => l.Building != null && l.Building.TotalNumberOfUnits <= 10),
					new PropertySearchOption("BuildingHasMoreThan10Units", "bo.ge10u", l => l.Building != null && l.Building.TotalNumberOfUnits >= 10),
					new PropertySearchOption("IsSingleUnitPerFloor", "bo.1upf", l => l.Building != null && l.Building.NumberOfUnitsPerFloor == 1),
					new PropertySearchOption("HasJanitorUnit", "bo.janit", l => l.Building != null && l.Building.HasJanitorUnit == true),
					new PropertySearchOption("HasEntranceLobby", "bo.lob", l => l.Building != null && l.Building.HasEntranceLobby == true),
					new PropertySearchOption("HasStoneFace", "bo.fcst", l => l.Building != null && l.Building.FaceType == BuildingFaceType.Stone),
					new PropertySearchOption("HasCompositePanelFace", "bo.fcpn", l => l.Building != null && l.Building.FaceType == BuildingFaceType.CompositePanel),
					new PropertySearchOption("StructureIsIronBeam", "bo.stir", l => l.Building != null && l.Building.StructureType == BuildingStructureType.IronBeam),
					new PropertySearchOption("StructureIsConcrete", "bo.stcr", l => l.Building != null && l.Building.StructureType == BuildingStructureType.Concrete),
					new PropertySearchOption("HasElevator", OptionHasElevator, l => l.Building != null && l.Building.HasElevator == true),
					new PropertySearchOption("HasFurnitureElevator", "bo.fele", l => l.Building != null && l.Building.HasFurnitureElevator == true),
					new PropertySearchOption("HasCentralTelevisionAntenna", "bo.ctva", l => l.Building != null && l.Building.HasCentralTelevisionAntenna == true),
					new PropertySearchOption("HasCentralInternetConnection", "bo.cnet", l => l.Building != null && l.Building.HasCentralInternetConnection == true),
					new PropertySearchOption("HasGuestParking", "bo.gpark", l => l.Building != null && l.Building.HasGuestParking == true),
					new PropertySearchOption("HasCentralBoiler", "bo.cblr", l => l.Building != null && l.Building.HasCentralBoiler == true),
					new PropertySearchOption("HasCentralChiller", "bo.cchl", l => l.Building != null && l.Building.HasCentralChiller == true),
					new PropertySearchOption("HasCentralVacuum", "bo.cvcm", l => l.Building != null && l.Building.HasCentralVacuum == true),
					new PropertySearchOption("HasGatheringHall", "bo.hall", l => l.Building != null && l.Building.HasGatheringHall == true),
					new PropertySearchOption("HasAutomaticParkingDoor", "bo.apkd", l => l.Building != null && l.Building.HasAutomaticParkingDoor == true),
					new PropertySearchOption("HasVideoEyePhone", "bo.viph", l => l.Building != null && l.Building.HasVideoEyePhone == true),
					new PropertySearchOption("HasCarWash", "bo.cwsh", l => l.Building != null && l.Building.HasCarWash == true),
					new PropertySearchOption("HasAccessibilityFeatures", "bo.acc", l => l.Building != null && l.Building.HasAccessibilityFeatures == true),
					new PropertySearchOption("HasGreenGarden", "bo.grng", l => l.Building != null && l.Building.HasGreenGarden == true),
					new PropertySearchOption("HasSwimmingPool", "bo.pool", l => l.Building != null && l.Building.HasSwimmingPool == true),
					new PropertySearchOption("HasSauna", "bo.saun", l => l.Building != null && l.Building.HasSauna == true),
					new PropertySearchOption("HasJacuzzi", "bo.jacz", l => l.Building != null && l.Building.HasJacuzzi == true),
					new PropertySearchOption("HasGym", "bo.gym", l => l.Building != null && l.Building.HasGym == true),
					new PropertySearchOption("HasGuardian", "bo.guard", l => l.Building != null && l.Building.HasGuardian == true),
				};

			var unitOptionsList =
				new List<PropertySearchOption>
				{
					new PropertySearchOption("HasParking", OptionHasParking, l => l.Unit != null && l.Unit.NumberOfParkings > 0),
					new PropertySearchOption("HasStorageRoom", OptionHasStorageRoom, l => l.Unit != null && l.Unit.StorageRoomArea > 0),
					new PropertySearchOption("IsEmpty", "uo.empty", l => l.Unit != null && (l.PropertyStatus == PropertyStatus.NoOccupantYet || l.PropertyStatus == PropertyStatus.Emptied)),
					new PropertySearchOption("IsOnGroundFloor", "uo.grnd", l => l.Unit != null && l.Unit.FloorNumber <= 1),
					new PropertySearchOption("IsResidential", "uo.resd", l => l.Unit != null && l.Unit.UsageType == UnitUsageType.Residency),
					new PropertySearchOption("IsDuplex", "uo.dplx", l => l.Unit != null && l.Unit.IsDuplex == true),
					new PropertySearchOption("HasIranianLavatory", "uo.lavi", l => l.Unit != null && l.Unit.HasIranianLavatory == true),
					new PropertySearchOption("HasForeignLavatory", "uo.lavf", l => l.Unit != null && l.Unit.HasForeignLavatory == true),
					new PropertySearchOption("HasPrivatePatio", "uo.pati", l => l.Unit != null && l.Unit.HasPrivatePatio == true),
					new PropertySearchOption("HasOpenKitchen", "uo.kopn", l => l.Unit != null && l.Unit.HasOpenKitchen == true),
					new PropertySearchOption("HasMdfTypeKitchen", "uo.kmdf", l => l.Unit != null && (l.Unit.KitchenCabinetTopType == KitchenCabinetTopType.Mdf || l.Unit.KitchenCabinetBodyType == KitchenCabinetBodyType.MdfWithMdfDoors)),
					new PropertySearchOption("HasSouthDaylight", "uo.dlso", l => l.Unit != null && l.Unit.MainDaylightDirection == DaylightDirection.South),
					new PropertySearchOption("HasUpvcWindows", "uo.wupvc", l => l.Unit != null && l.Unit.WindowType == WindowType.Upvc),
					new PropertySearchOption("HasIndependentHeatingPackage", "uo.hpkg", l => l.Unit != null && l.Unit.HasIndependentHeatingPackage == true),
					new PropertySearchOption("HasIndependentCoolingPackage", "uo.cpkg", l => l.Unit != null && l.Unit.HasIndependentCoolingPackage == true),
					new PropertySearchOption("HasFireplace", "uo.frpl", l => l.Unit != null && l.Unit.HasFireplace == true),
					new PropertySearchOption("HasDualLayerWindows", "uo.2lw", l => l.Unit != null && l.Unit.WindowsHasDoubleGlass == true),
					new PropertySearchOption("HasBeenReconstructed", "uo.rcstr", l => l.Unit != null && l.Unit.HasBeenReconstructed == true),
				};

			var otherOptionsList =
				new List<PropertySearchOption>
				{
					new PropertySearchOption("HasPhotos", OptionHasPhotos, l => l.Photos.Any(p => p.Approved.HasValue && p.Approved.Value && !p.DeleteTime.HasValue),
						@delegate: l => l.Photos != null && l.Photos.Any(p => p.Approved.HasValue && p.Approved.Value && !p.DeleteTime.HasValue)),
					new PropertySearchOption("UserIsOwner", OptionUserIsOwner, l => l.OwnerUserID == ServiceContext.Principal.CoreIdentity.UserId, 
						dependsOnUserAccount: true),
					new PropertySearchOption("UserHasStarred", OptionUserHasStarred, l => l.FavoritedBy.Any(lf => lf.UserID == ServiceContext.Principal.CoreIdentity.UserId), 
						dependsOnUserAccount: true),
                    new PropertySearchOption("NoAgencyListing", OptionNoAgencyListing, l => l.IsAgencyListing == false),
                    new PropertySearchOption("AgencyActivityAllowed", OptionAgencyActivityAllowed, l => l.IsAgencyActivityAllowed)
				};

			EstateOptionsPrivate = estateOptionsList.ToDictionary(pso => pso.Key.ToLower());
			BuildingOptionsPrivate = buildingOptionsList.ToDictionary(pso => pso.Key.ToLower());
			UnitOptionsPrivate = unitOptionsList.ToDictionary(pso => pso.Key.ToLower());
			OtherOptionsPrivate = otherOptionsList.ToDictionary(pso => pso.Key.ToLower());

			var allOptionsList = estateOptionsList.Union(buildingOptionsList).Union(unitOptionsList).Union(otherOptionsList).ToList();
			AllOptionsPrivate = allOptionsList.ToDictionary(pso => pso.Key.ToLower());
		}

		public static IDictionary<string, PropertySearchOption> AllOptions
		{
			get { return AllOptionsPrivate; }
		}

		public static IDictionary<string, PropertySearchOption> EstateOptions
		{
			get { return EstateOptionsPrivate; }
		}

		public static IDictionary<string, PropertySearchOption> BuildingOptions
		{
			get { return BuildingOptionsPrivate; }
		}

		public static IDictionary<string, PropertySearchOption> UnitOptions
		{
			get { return UnitOptionsPrivate; }
		}

		public static IDictionary<string, PropertySearchOption> OtherOptions
		{
			get { return OtherOptionsPrivate; }
		}
	}
}