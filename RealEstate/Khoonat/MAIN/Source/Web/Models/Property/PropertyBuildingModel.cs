using System;
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
	public class PropertyBuildingModel : PropertySectionModelBase
	{
		public PropertyBuildingModel(long propertyListingID, long? propertyListingCode = null, PropertyType? propertyType = null, IntentionOfOwner? intentionOfOwner = null)
			: base(propertyListingID, propertyListingCode, propertyType, intentionOfOwner)
		{
		}

		#region User-input: Units Arrangement

		[Numeric(ErrorMessageResourceType = typeof(PropertyBuildingResources), ErrorMessageResourceName = "Validation_TotalNumberOfUnits_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyBuildingResources), ErrorMessageResourceName = "Validation_TotalNumberOfUnits_Numeric")]
		[Range(0, 99999, ErrorMessageResourceType = typeof(PropertyBuildingResources), ErrorMessageResourceName = "Validation_TotalNumberOfUnits_Range")]
		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_TotalNumberOfUnits")]
		public int? TotalNumberOfUnits { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertyBuildingResources), ErrorMessageResourceName = "Validation_NumberOfUnitsPerFloor_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyBuildingResources), ErrorMessageResourceName = "Validation_NumberOfUnitsPerFloor_Numeric")]
		[Range(0, 999, ErrorMessageResourceType = typeof(PropertyBuildingResources), ErrorMessageResourceName = "Validation_NumberOfUnitsPerFloor_Range")]
		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_NumberOfUnitsPerFloor")]
		public int? NumberOfUnitsPerFloor { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertyBuildingResources), ErrorMessageResourceName = "Validation_NumberFloorsAboveGround_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyBuildingResources), ErrorMessageResourceName = "Validation_NumberFloorsAboveGround_Numeric")]
		[Range(0, 200, ErrorMessageResourceType = typeof(PropertyBuildingResources), ErrorMessageResourceName = "Validation_NumberFloorsAboveGround_Range")]
		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_NumberFloorsAboveGround")]
		public int? NumberFloorsAboveGround { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertyBuildingResources), ErrorMessageResourceName = "Validation_NumberOfParkingFloors_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyBuildingResources), ErrorMessageResourceName = "Validation_NumberOfParkingFloors_Numeric")]
		[Range(0, 50, ErrorMessageResourceType = typeof(PropertyBuildingResources), ErrorMessageResourceName = "Validation_NumberOfParkingFloors_Range")]
		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_NumberOfParkingFloors")]
		public int? NumberOfParkingFloors { get; set; }

		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_HasJanitorUnit")]
		public bool? HasJanitorUnit { get; set; }

		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_HasEntranceLobby")]
		public bool? HasEntranceLobby { get; set; }

		#endregion

		#region User-input: Building Properties

		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_FaceType")]
		public BuildingFaceType? FaceType { get; set; }

		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_StructureType")]
		public BuildingStructureType? StructureType { get; set; }

		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_RoofCoverType")]
		public BuildingRoofCoverType? RoofCoverType { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertyBuildingResources), ErrorMessageResourceName = "Validation_BuildingAgeYears_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyBuildingResources), ErrorMessageResourceName = "Validation_BuildingAgeYears_Numeric")]
		[Range(0, 200, ErrorMessageResourceType = typeof(PropertyBuildingResources), ErrorMessageResourceName = "Validation_BuildingAgeYears_Range")]
		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_BuildingAgeYears")]
		public int? BuildingAgeYears { get; set; }

		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_HasEndOfConstructionCertificate")]
		public bool? HasEndOfConstructionCertificate { get; set; }

		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyBuildingResources), ErrorMessageResourceName = "Validation_PermitDate_Date")]
		[DynamicDateRange(double.NaN, 365, ErrorMessageResourceType = typeof(PropertyBuildingResources), ErrorMessageResourceName = "Validation_PermitDate_Range")]
		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_PermitDate")]
		public DateTime? PermitDate { get; set; }

		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyBuildingResources), ErrorMessageResourceName = "Validation_ConstructionCompletionDate_Date")]
		[DynamicDateRange(double.NaN, 1460, ErrorMessageResourceType = typeof(PropertyBuildingResources), ErrorMessageResourceName = "Validation_ConstructionCompletionDate_Range")]
		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_ConstructionCompletionDate")]
		public DateTime? ConstructionCompletionDate { get; set; }

		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyBuildingResources), ErrorMessageResourceName = "Validation_EndOfConstructionCertificateDate_Date")]
		[DynamicDateRange(double.NaN, 1460, ErrorMessageResourceType = typeof(PropertyBuildingResources), ErrorMessageResourceName = "Validation_EndOfConstructionCertificateDate_Range")]
		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_EndOfConstructionCertificateDate")]
		public DateTime? EndOfConstructionCertificateDate { get; set; }

		#endregion

		#region User-input: Standard features

		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_HasElevator")]
		public bool? HasElevator { get; set; }

		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_HasFurnitureElevator")]
		public bool? HasFurnitureElevator { get; set; }

		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_HasCentralTelevisionAntenna")]
		public bool? HasCentralTelevisionAntenna { get; set; }

		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_HasCentralInternetConnection")]
		public bool? HasCentralInternetConnection { get; set; }

		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_HasGuestParking")]
		public bool? HasGuestParking { get; set; }

		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_HasTrashChute")]
		public bool? HasTrashChute { get; set; }

		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_HasCentralBoiler")]
		public bool? HasCentralBoiler { get; set; }

		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_HasCentralChiller")]
		public bool? HasCentralChiller { get; set; }

		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_HasCentralVacuum")]
		public bool? HasCentralVacuum { get; set; }

		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_HasGatheringHall")]
		public bool? HasGatheringHall { get; set; }

		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_OtherFeatures")]
		[StringLength(1000, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
		public string OtherFeatures { get; set; }

		#endregion

		#region User-input: Welfare features

		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_HasAutomaticParkingDoor")]
		public bool? HasAutomaticParkingDoor { get; set; }

		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_HasVideoEyePhone")]
		public bool? HasVideoEyePhone { get; set; }

		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_HasCarWash")]
		public bool? HasCarWash { get; set; }

		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_HasBackupWaterTank")]
		public bool? HasBackupWaterTank { get; set; }

		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_HasAutomaticStairwayLights")]
		public bool? HasAutomaticStairwayLights { get; set; }

		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_HasAccessibilityFeatures")]
		public bool? HasAccessibilityFeatures { get; set; }

		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_OtherWelfareFeatures")]
		[StringLength(1000, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
		public string OtherWelfareFeatures { get; set; }

		#endregion

		#region User-input: Recreation features

		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_HasGreenGarden")]
		public bool? HasGreenGarden { get; set; }

        [Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_HasBowerInGarden")]
        public bool? HasBowerInGarden { get; set; }

        [Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_HasBowerOnRoof")]
        public bool? HasBowerOnRoof { get; set; }

		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_HasSwimmingPool")]
		public bool? HasSwimmingPool { get; set; }

		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_HasSauna")]
		public bool? HasSauna { get; set; }

		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_HasJacuzzi")]
		public bool? HasJacuzzi { get; set; }

		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_HasGym")]
		public bool? HasGym { get; set; }

		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_OtherRecreationFeatures")]
		[StringLength(1000, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
		public string OtherRecreationFeatures { get; set; }

		#endregion

		#region User-input: Safety features

		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_HasFireAlarm")]
		public bool? HasFireAlarm { get; set; }

		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_HasEmergencyEscape")]
		public bool? HasEmergencyEscape { get; set; }

		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_HasClosedCircuitCamera")]
		public bool? HasClosedCircuitCamera { get; set; }

		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_HasGuardian")]
		public bool? HasGuardian { get; set; }

		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_HasQuakeInsurance")]
		public bool? HasQuakeInsurance { get; set; }

		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_HasFireInsurance")]
		public bool? HasFireInsurance { get; set; }

		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_HasLightningArrester")]
		public bool? HasLightningArrester { get; set; }

		[Display(ResourceType = typeof(PropertyBuildingResources), Name = "Label_OtherSafetyFeatures")]
		[StringLength(1000, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
		public string OtherSafetyFeatures { get; set; }

		#endregion

		#region Convert to/from domain

		public static PropertyBuildingModel CreateFromDto(PropertyListingDetails domain, bool showAllAttributes = false)
		{
			var result = new PropertyBuildingModel(domain.ID, domain.Code, domain.PropertyType, domain.IntentionOfOwner) {ShowAllAttributes = showAllAttributes};

			if (domain.Building != null)
			{
				result.TotalNumberOfUnits = domain.Building.TotalNumberOfUnits;
				result.NumberOfUnitsPerFloor = domain.Building.NumberOfUnitsPerFloor;
				result.NumberFloorsAboveGround = domain.Building.NumberFloorsAboveGround;
				result.NumberOfParkingFloors = domain.Building.NumberOfParkingFloors;
				result.HasJanitorUnit = domain.Building.HasJanitorUnit;
				result.HasEntranceLobby = domain.Building.HasEntranceLobby;

				result.FaceType = domain.Building.FaceType;
				result.StructureType = domain.Building.StructureType;
				result.RoofCoverType = domain.Building.RoofCoverType;
				result.BuildingAgeYears = domain.Building.BuildingAgeYears;
				result.HasEndOfConstructionCertificate = domain.Building.HasEndOfConstructionCertificate;
				result.PermitDate = domain.Building.PermitDate;
				result.ConstructionCompletionDate = domain.Building.ConstructionCompletionDate;
				result.EndOfConstructionCertificateDate = domain.Building.EndOfConstructionCertificateDate;

				result.HasElevator = domain.Building.HasElevator;
				result.HasFurnitureElevator = domain.Building.HasFurnitureElevator;
				result.HasCentralTelevisionAntenna = domain.Building.HasCentralTelevisionAntenna;
				result.HasCentralInternetConnection = domain.Building.HasCentralInternetConnection;
				result.HasGuestParking = domain.Building.HasGuestParking;
				result.HasTrashChute = domain.Building.HasTrashChute;
				result.HasCentralBoiler = domain.Building.HasCentralBoiler;
				result.HasCentralChiller = domain.Building.HasCentralChiller;
				result.HasCentralVacuum = domain.Building.HasCentralVacuum;
				result.HasGatheringHall = domain.Building.HasGatheringHall;
				result.OtherFeatures = domain.Building.OtherFeatures;
				
				result.HasAutomaticParkingDoor = domain.Building.HasAutomaticParkingDoor;
				result.HasVideoEyePhone = domain.Building.HasVideoEyePhone;
				result.HasCarWash = domain.Building.HasCarWash;
				result.HasBackupWaterTank = domain.Building.HasBackupWaterTank;
				result.HasAutomaticStairwayLights = domain.Building.HasAutomaticStairwayLights;
				result.HasAccessibilityFeatures = domain.Building.HasAccessibilityFeatures;
				result.OtherWelfareFeatures = domain.Building.OtherWelfareFeatures;
				
				result.HasGreenGarden = domain.Building.HasGreenGarden;
				result.HasBowerInGarden = domain.Building.HasBowerInGarden;
				result.HasBowerOnRoof = domain.Building.HasBowerOnRoof;
				result.HasSwimmingPool = domain.Building.HasSwimmingPool;
				result.HasSauna = domain.Building.HasSauna;
				result.HasJacuzzi = domain.Building.HasJacuzzi;
				result.HasGym = domain.Building.HasGym;
				result.OtherRecreationFeatures = domain.Building.OtherRecreationFeatures;
				
				result.HasFireAlarm = domain.Building.HasFireAlarm;
				result.HasEmergencyEscape = domain.Building.HasEmergencyEscape;
				result.HasClosedCircuitCamera = domain.Building.HasClosedCircuitCamera;
				result.HasGuardian = domain.Building.HasGuardian;
				result.HasQuakeInsurance = domain.Building.HasQuakeInsurance;
				result.HasFireInsurance = domain.Building.HasFireInsurance;
				result.HasLightningArrester = domain.Building.HasLightningArrester;
				result.OtherSafetyFeatures = domain.Building.OtherSafetyFeatures;
			}

			return result;
		}

		public void UpdateDomain(PropertyListing listing)
		{
			if (listing.Building == null)
				listing.Building = new Building();

			listing.Building.TotalNumberOfUnits = TotalNumberOfUnits;
			listing.Building.NumberOfUnitsPerFloor = NumberOfUnitsPerFloor;
			listing.Building.NumberFloorsAboveGround = NumberFloorsAboveGround;
			listing.Building.NumberOfParkingFloors = NumberOfParkingFloors;
			listing.Building.HasJanitorUnit = HasJanitorUnit;
			listing.Building.HasEntranceLobby = HasEntranceLobby;

			listing.Building.FaceType = FaceType;
			listing.Building.StructureType = StructureType;
			listing.Building.RoofCoverType = RoofCoverType;
			listing.Building.BuildingAgeYears = BuildingAgeYears;
			listing.Building.HasEndOfConstructionCertificate = HasEndOfConstructionCertificate;
			listing.Building.PermitDate = PermitDate;
			listing.Building.ConstructionCompletionDate = ConstructionCompletionDate;
			listing.Building.EndOfConstructionCertificateDate = EndOfConstructionCertificateDate;

			listing.Building.HasElevator = HasElevator;
			listing.Building.HasFurnitureElevator = HasFurnitureElevator;
			listing.Building.HasCentralTelevisionAntenna = HasCentralTelevisionAntenna;
			listing.Building.HasCentralInternetConnection = HasCentralInternetConnection;
			listing.Building.HasGuestParking = HasGuestParking;
			listing.Building.HasTrashChute = HasTrashChute;
			listing.Building.HasCentralBoiler = HasCentralBoiler;
			listing.Building.HasCentralChiller = HasCentralChiller;
			listing.Building.HasCentralVacuum = HasCentralVacuum;
			listing.Building.HasGatheringHall = HasGatheringHall;
			listing.Building.OtherFeatures = OtherFeatures;

			listing.Building.HasAutomaticParkingDoor = HasAutomaticParkingDoor;
			listing.Building.HasVideoEyePhone = HasVideoEyePhone;
			listing.Building.HasCarWash = HasCarWash;
			listing.Building.HasBackupWaterTank = HasBackupWaterTank;
			listing.Building.HasAutomaticStairwayLights = HasAutomaticStairwayLights;
			listing.Building.HasAccessibilityFeatures = HasAccessibilityFeatures;
			listing.Building.OtherWelfareFeatures = OtherWelfareFeatures;

			listing.Building.HasGreenGarden = HasGreenGarden;
			listing.Building.HasBowerInGarden = HasBowerInGarden;
			listing.Building.HasBowerOnRoof = HasBowerOnRoof;
			listing.Building.HasSwimmingPool = HasSwimmingPool;
			listing.Building.HasSauna = HasSauna;
			listing.Building.HasJacuzzi = HasJacuzzi;
			listing.Building.HasGym = HasGym;
			listing.Building.OtherRecreationFeatures = OtherRecreationFeatures;

			listing.Building.HasFireAlarm = HasFireAlarm;
			listing.Building.HasEmergencyEscape = HasEmergencyEscape;
			listing.Building.HasClosedCircuitCamera = HasClosedCircuitCamera;
			listing.Building.HasGuardian = HasGuardian;
			listing.Building.HasQuakeInsurance = HasQuakeInsurance;
			listing.Building.HasFireInsurance = HasFireInsurance;
			listing.Building.HasLightningArrester = HasLightningArrester;
			listing.Building.OtherSafetyFeatures = OtherSafetyFeatures;
		}

		#endregion
	}
}