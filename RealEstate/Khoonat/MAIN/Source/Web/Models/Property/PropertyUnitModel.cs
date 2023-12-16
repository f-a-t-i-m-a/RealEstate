using System.ComponentModel.DataAnnotations;
using JahanJooy.Common.Util.Web.Attributes;
using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Domain.Property;
using JahanJooy.RealEstate.Web.Resources.Models;
using JahanJooy.RealEstate.Web.Resources.Models.Property;

namespace JahanJooy.RealEstate.Web.Models.Property
{
	public class PropertyUnitModel : PropertySectionModelBase
	{
		public PropertyUnitModel(long propertyListingID, long? propertyListingCode = null, PropertyType? propertyType = null, IntentionOfOwner? intentionOfOwner = null)
			: base(propertyListingID, propertyListingCode, propertyType, intentionOfOwner)
		{
		}

		#region User-input: Main properties

		[Numeric(ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_FloorNumber_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_FloorNumber_Numeric")]
		[Range(-2, 200, ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_FloorNumber_Range")]
		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_FloorNumber")]
		public int? FloorNumber { get; set; }

		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_UsageType")]
		public UnitUsageType? UsageType { get; set; }

		#endregion

		#region User-input: Number of rooms

		[Numeric(ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_NumberOfRooms_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_NumberOfRooms_Numeric")]
		[Range(0, 99, ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_NumberOfRooms_Range")]
		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_NumberOfRooms")]
		public int? NumberOfRooms { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_NumberOfLavatories_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_NumberOfLavatories_Numeric")]
		[Range(0, 99, ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_NumberOfLavatories_Range")]
		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_NumberOfLavatories")]
		public int? NumberOfLavatories { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_NumberOfBathrooms_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_NumberOfBathrooms_Numeric")]
		[Range(0, 99, ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_NumberOfBathrooms_Range")]
		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_NumberOfBathrooms")]
		public int? NumberOfBathrooms { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_NumberOfKitchens_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_NumberOfKitchens_Numeric")]
		[Range(0, 99, ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_NumberOfKitchens_Range")]
		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_NumberOfKitchens")]
		public int? NumberOfKitchens { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_NumberOfClosets_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_NumberOfClosets_Numeric")]
		[Range(0, 99, ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_NumberOfClosets_Range")]
		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_NumberOfClosets")]
		public int? NumberOfClosets { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_NumberOfBalconies_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_NumberOfBalconies_Numeric")]
		[Range(0, 99, ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_NumberOfBalconies_Range")]
		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_NumberOfBalconies")]
		public int? NumberOfBalconies { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_NumberOfParkings_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_NumberOfParkings_Numeric")]
		[Range(0, 99, ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_NumberOfParkings_Range")]
		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_NumberOfParkings")]
		public int? NumberOfParkings { get; set; }

		#endregion

		#region User-input: Area

		[Numeric(ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_UsefulArea_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_UsefulArea_Numeric")]
		[Range(1, 99999, ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_UsefulArea_Range")]
		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_UsefulArea")]
		public decimal? UsefulArea { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_KitchenArea_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_KitchenArea_Numeric")]
		[Range(0, 9999, ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_KitchenArea_Range")]
		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_KitchenArea")]
		public decimal? KitchenArea { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_BalconyArea_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_BalconyArea_Numeric")]
		[Range(0, 9999, ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_BalconyArea_Range")]
		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_BalconyArea")]
		public decimal? BalconyArea { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_StorageRoomArea_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_StorageRoomArea_Numeric")]
		[Range(0, 9999, ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_StorageRoomArea_Range")]
		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_StorageRoomArea")]
		public decimal? StorageRoomArea { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_CurrentMonthlyChargeAmount_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_CurrentMonthlyChargeAmount_Numeric")]
		[Range(0, 99999999, ErrorMessageResourceType = typeof(PropertyUnitResources), ErrorMessageResourceName = "Validation_CurrentMonthlyChargeAmount_Range")]
		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_CurrentMonthlyChargeAmount")]
		public decimal? CurrentMonthlyChargeAmount { get; set; }

		#endregion

		#region User-input: Features and Properties

		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_IsDuplex")]
		public bool? IsDuplex { get; set; }

		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_IsFurnished")]
		public bool? IsFurnished { get; set; }

		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_HasHalfFloorInside")]
		public bool? HasHalfFloorInside { get; set; }

		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_HasIranianLavatory")]
		public bool? HasIranianLavatory { get; set; }

		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_HasForeignLavatory")]
		public bool? HasForeignLavatory { get; set; }

		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_HasSeparateLivingRoom")]
		public bool? HasSeparateLivingRoom { get; set; }

		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_HasPrivatePatio")]
		public bool? HasPrivatePatio { get; set; }

		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_HasPrivateGarden")]
		public bool? HasPrivateGarden { get; set; }

		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_HasSecuritySystem")]
		public bool? HasSecuritySystem { get; set; }

		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_HasOpenKitchen")]
		public bool? HasOpenKitchen { get; set; }

		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_HasFurnishedKitchen")]
		public bool? HasFurnishedKitchen { get; set; }

		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_KitchenCabinetTopType")]
		public KitchenCabinetTopType? KitchenCabinetTopType { get; set; }

		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_KitchenCabinetBodyType")]
		public KitchenCabinetBodyType? KitchenCabinetBodyType { get; set; }

		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_AdditionalSpecialFeatures")]
		[StringLength(1000, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
		public string AdditionalSpecialFeatures { get; set; }

		#endregion

		#region User-input: Covers and Windows

		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_MainDaylightDirection")]
		public DaylightDirection? MainDaylightDirection { get; set; }

		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_LivingRoomFloor")]
		public FloorCoverType? LivingRoomFloor { get; set; }

		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_LivingRoomWalls")]
		public WallCoverType? LivingRoomWalls { get; set; }

		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_RoomsFloor")]
		public FloorCoverType? RoomsFloor { get; set; }

		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_RoomsWall")]
		public WallCoverType? RoomsWall { get; set; }

		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_WindowType")]
		public WindowType? WindowType { get; set; }

		#endregion

		#region User-input: Heating and Cooling

		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_HeatingSystem")]
		public HeatingSystemType? HeatingSystem { get; set; }

		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_HasIndependentHeatingPackage")]
		public bool? HasIndependentHeatingPackage { get; set; }

		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_CoolingSystem")]
		public CoolingSystemType? CoolingSystem { get; set; }

		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_HasIndependentCoolingPackage")]
		public bool? HasIndependentCoolingPackage { get; set; }

		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_HasFireplace")]
		public bool? HasFireplace { get; set; }

		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_WindowsHasDoubleGlass")]
		public bool? WindowsHasDoubleGlass { get; set; }

		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_HasInsulatedWalls")]
		public bool? HasInsulatedWalls { get; set; }

		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_HasIndependentGasometer")]
		public bool? HasIndependentGasometer { get; set; }

		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_AdditionalHeatingAndCoolingInformation")]
		[StringLength(1000, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
		public string AdditionalHeatingAndCoolingInformation { get; set; }

		#endregion

		#region User-input: Current conditions

		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_HasBeenReconstructed")]
		public bool? HasBeenReconstructed { get; set; }

		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_NeedsDestruction")]
		public bool? NeedsDestruction { get; set; }

		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_NeedsReconstruction")]
		public bool? NeedsReconstruction { get; set; }

		[Display(ResourceType = typeof(PropertyUnitResources), Name = "Label_NeedsPainting")]
		public bool? NeedsPainting { get; set; }

		#endregion

		#region Convert to/from domain

		public static PropertyUnitModel CreateFromDto(PropertyListingDetails dto, bool showAllAttributes = false)
		{
			var result = new PropertyUnitModel(dto.ID, dto.Code, dto.PropertyType, dto.IntentionOfOwner) {ShowAllAttributes = showAllAttributes};

			if (dto.Unit != null)
			{
				result.FloorNumber = dto.Unit.FloorNumber;
				result.UsageType = dto.Unit.UsageType;

				result.NumberOfRooms = dto.Unit.NumberOfRooms;
				result.NumberOfLavatories = dto.Unit.NumberOfLavatories;
				result.NumberOfBathrooms = dto.Unit.NumberOfBathrooms;
				result.NumberOfKitchens = dto.Unit.NumberOfKitchens;
				result.NumberOfClosets = dto.Unit.NumberOfClosets;
				result.NumberOfBalconies = dto.Unit.NumberOfBalconies;
				result.NumberOfParkings = dto.Unit.NumberOfParkings;

				result.UsefulArea = dto.Unit.Area;
				result.KitchenArea = dto.Unit.KitchenArea;
				result.BalconyArea = dto.Unit.BalconyArea;
				result.StorageRoomArea = dto.Unit.StorageRoomArea;
				result.CurrentMonthlyChargeAmount = dto.Unit.CurrentMonthlyChargeAmount;

				result.IsDuplex = dto.Unit.IsDuplex;
				result.IsFurnished = dto.Unit.IsFurnished;
				result.HasHalfFloorInside = dto.Unit.HasHalfFloorInside;
				result.HasIranianLavatory = dto.Unit.HasIranianLavatory;
				result.HasForeignLavatory = dto.Unit.HasForeignLavatory;
				result.HasSeparateLivingRoom = dto.Unit.HasSeparateLivingRoom;
				result.HasPrivatePatio = dto.Unit.HasPrivatePatio;
				result.HasPrivateGarden = dto.Unit.HasPrivateGarden;
				result.HasSecuritySystem = dto.Unit.HasSecuritySystem;
				result.HasOpenKitchen = dto.Unit.HasOpenKitchen;
				result.HasFurnishedKitchen = dto.Unit.HasFurnishedKitchen;
				result.KitchenCabinetBodyType = dto.Unit.KitchenCabinetBodyType;
				result.KitchenCabinetTopType = dto.Unit.KitchenCabinetTopType;
				result.AdditionalSpecialFeatures = dto.Unit.AdditionalSpecialFeatures;

				result.MainDaylightDirection = dto.Unit.MainDaylightDirection;
				result.LivingRoomFloor = dto.Unit.LivingRoomFloor;
				result.LivingRoomWalls = dto.Unit.LivingRoomWalls;
				result.RoomsFloor = dto.Unit.RoomsFloor;
				result.RoomsWall = dto.Unit.RoomsWall;
				result.WindowType = dto.Unit.WindowType;

				result.HeatingSystem = dto.Unit.HeatingSystem;
				result.CoolingSystem = dto.Unit.CoolingSystem;
				result.HasIndependentHeatingPackage = dto.Unit.HasIndependentHeatingPackage;
				result.HasIndependentCoolingPackage = dto.Unit.HasIndependentCoolingPackage;
				result.HasFireplace = dto.Unit.HasFireplace;
				result.WindowsHasDoubleGlass = dto.Unit.WindowsHasDoubleGlass;
				result.HasInsulatedWalls = dto.Unit.HasInsulatedWalls;
				result.HasIndependentGasometer = dto.Unit.HasIndependentGasometer;
				result.AdditionalHeatingAndCoolingInformation = dto.Unit.AdditionalHeatingAndCoolingInformation;

				result.HasBeenReconstructed = dto.Unit.HasBeenReconstructed;
				result.NeedsDestruction = dto.Unit.NeedsDestruction;
				result.NeedsReconstruction = dto.Unit.NeedsReconstruction;
				result.NeedsPainting = dto.Unit.NeedsPainting;
			}

			return result;
		}

		public void UpdateDomain(PropertyListing domain)
		{
			if (domain.Unit == null)
				domain.Unit = new Unit();

			domain.Unit.FloorNumber = FloorNumber;
			domain.Unit.UsageType = UsageType;

			domain.Unit.NumberOfRooms = NumberOfRooms;
			domain.Unit.NumberOfLavatories = NumberOfLavatories;
			domain.Unit.NumberOfBathrooms = NumberOfBathrooms;
			domain.Unit.NumberOfKitchens = NumberOfKitchens;
			domain.Unit.NumberOfClosets = NumberOfClosets;
			domain.Unit.NumberOfBalconies = NumberOfBalconies;
			domain.Unit.NumberOfParkings = NumberOfParkings;

			domain.Unit.Area = UsefulArea;
			domain.Unit.KitchenArea = KitchenArea;
			domain.Unit.BalconyArea = BalconyArea;
			domain.Unit.StorageRoomArea = StorageRoomArea;
			domain.Unit.CurrentMonthlyChargeAmount = CurrentMonthlyChargeAmount;

			domain.Unit.IsDuplex = IsDuplex;
			domain.Unit.IsFurnished = IsFurnished;
			domain.Unit.HasHalfFloorInside = HasHalfFloorInside;
			domain.Unit.HasIranianLavatory = HasIranianLavatory;
			domain.Unit.HasForeignLavatory = HasForeignLavatory;
			domain.Unit.HasSeparateLivingRoom = HasSeparateLivingRoom;
			domain.Unit.HasPrivatePatio = HasPrivatePatio;
			domain.Unit.HasPrivateGarden = HasPrivateGarden;
			domain.Unit.HasSecuritySystem = HasSecuritySystem;
			domain.Unit.HasOpenKitchen = HasOpenKitchen;
			domain.Unit.HasFurnishedKitchen = HasFurnishedKitchen;
			domain.Unit.KitchenCabinetBodyType = KitchenCabinetBodyType;
			domain.Unit.KitchenCabinetTopType = KitchenCabinetTopType;
			domain.Unit.AdditionalSpecialFeatures = AdditionalSpecialFeatures;

			domain.Unit.MainDaylightDirection = MainDaylightDirection;
			domain.Unit.LivingRoomFloor = LivingRoomFloor;
			domain.Unit.LivingRoomWalls = LivingRoomWalls;
			domain.Unit.RoomsFloor = RoomsFloor;
			domain.Unit.RoomsWall = RoomsWall;
			domain.Unit.WindowType = WindowType;

			domain.Unit.HeatingSystem = HeatingSystem;
			domain.Unit.CoolingSystem = CoolingSystem;
			domain.Unit.HasIndependentHeatingPackage = HasIndependentHeatingPackage;
			domain.Unit.HasIndependentCoolingPackage = HasIndependentCoolingPackage;
			domain.Unit.HasFireplace = HasFireplace;
			domain.Unit.WindowsHasDoubleGlass = WindowsHasDoubleGlass;
			domain.Unit.HasInsulatedWalls = HasInsulatedWalls;
			domain.Unit.HasIndependentGasometer = HasIndependentGasometer;
			domain.Unit.AdditionalHeatingAndCoolingInformation = AdditionalHeatingAndCoolingInformation;

			domain.Unit.HasBeenReconstructed = HasBeenReconstructed;
			domain.Unit.NeedsDestruction = NeedsDestruction;
			domain.Unit.NeedsReconstruction = NeedsReconstruction;
			domain.Unit.NeedsPainting = NeedsPainting;
		}

		#endregion
	}
}