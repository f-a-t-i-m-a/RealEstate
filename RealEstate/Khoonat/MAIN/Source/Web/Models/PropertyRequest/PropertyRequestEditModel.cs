using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Spatial;
using System.Web.Mvc;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.Common.Util.Web.Attributes;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Domain.Property;
using JahanJooy.RealEstate.Web.Models.Property;
using JahanJooy.RealEstate.Web.Resources.Controllers.Property;
using JahanJooy.RealEstate.Web.Resources.Models;
using ValidationResult = JahanJooy.Common.Util.Validation.ValidationResult;

namespace JahanJooy.RealEstate.Web.Models.PropertyRequest
{
    [Bind(Exclude = "ID,Code,PublishValidationResult,ExistingContactInfos")]
    public class PropertyRequestEditModel
    {

        #region from VicinityAdminEditGrographyModel

//        public Vicinity Vicinity { get; set; }
        public Vicinity ParentVicinity { get; set; }

//        public long? ID { get; set; }

        public DbGeography CenterPoint { get; set; }
        public DbGeography Boundary { get; set; }
        public long? ParentID { get; set; }

        public string CenterPointWkt
        {
            get { return CenterPoint.ToWkt(); }
            set
            {
                /*CenterPoint = DbGeographyUtil.CreatePoint(value); */
                String temp = value;
                temp += "";
            }
        }

        public string BoundaryWkt
        {
            get { return Boundary.ToWkt(); }
            set
            {
                /*Boundary = DbGeographyUtil.CreatePolygon(value); */
                String temp = value;
                temp += "";
            }
        }

        #region Convert to/from domain

        public static PropertyRequestEditModel FromDomain(Vicinity vicinity, Vicinity parentVicinity)
        {
            var result = new PropertyRequestEditModel
            {
                Vicinity = vicinity,
                ParentVicinity = parentVicinity,

                ID = vicinity.ID,
                CenterPoint = vicinity.CenterPoint,
                Boundary = vicinity.Boundary,
                ParentID = vicinity.ParentID
            };

            return result;
        }

        public void UpdateDomain(Vicinity domain)
        {
            domain.CenterPoint = CenterPoint;
            domain.Boundary = Boundary;
        }

        #endregion
        
        #endregion
        #region Required properties (record type specification)

        [Required(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource),ErrorMessageResourceName = "Validation_Required_IntentionOfRequester")]
        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_IntentionOfRequester")]
        public IntentionOfRequester? IntentionOfRequester { get; set; }

        [Required(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_Required_PropertyType")]
        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_PropertyType")]
        public PropertyType? PropertyType { get; set; }

        [Required(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource),ErrorMessageResourceName = "Validation_Required_AgencyListing")]
        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_AgencyListing")]
        public PropertyListingCreatorRoleType? AgencyListing { get; set; }

        [Display(ResourceType = typeof(PropertyRequestEditModelResources),Name = "Label_NoAgencyListing_AgencyPermitted")]
        public NonAgencyPropertyListingAgencyActivityAllowance? NoAgencyListingAgencyPermitted { get; set; }

        [Display(ResourceType = typeof(PropertyRequestEditModelResources),Name = "Label_AgencyListing_OtherAgencyPermitted")]
        public AgencyPropertyListingAgencyActivityAllowance? AgencyListingOtherAgencyPermitted { get; set; }

        #endregion

        #region Location properties

        public long? SelectedVicinityID { get; set; }
        public Vicinity Vicinity { get; set; }

        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_Address")]
        [StringLength(150, ErrorMessageResourceType = typeof(GeneralValidationErrorResources),ErrorMessageResourceName = "StringLength")]
        public string Address { get; set; }

        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_AdditionalAddressInfo")]
        [StringLength(1000, ErrorMessageResourceType = typeof(GeneralValidationErrorResources),ErrorMessageResourceName = "StringLength")]
        public string AdditionalAddressInfo { get; set; }

        public double? UserPointLat { get; set; }
        public double? UserPointLng { get; set; }
        public GeographicLocationSpecificationType? GeographicLocationType { get; set; }

        #endregion

        #region Estate properties

//        [Numeric(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_EstateArea_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_EstateArea_Numeric")]
        [Range(1, 10000000, ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_EstateArea_Range")]
        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_EstateArea")]
        public decimal? EstateArea { get; set; }

        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_EstateAreaMinimum")]
        public decimal? EstateAreaMinimum { get; set; }

        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_EstateAreaMaximum")]
        public decimal? EstateAreaMaximum { get; set; }


        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_EstateDirection")]
        public EstateDirection? EstateDirection { get; set; }

//        [Numeric(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_PassageEdgeLength_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_PassageEdgeLength_Numeric")]
        [Range(1, 1000, ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_PassageEdgeLength_Range")]

        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_PassageEdgeLength")]
        public decimal? PassageEdgeLength { get; set; }

        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_EstateVoucherType")]
        public EstateVoucherType? EstateVoucherType { get; set; }

        #endregion

        #region Building properties

//        [Numeric(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_TotalNumberOfUnits_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_TotalNumberOfUnits_Numeric")]
        [Range(0, 99999, ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_TotalNumberOfUnits_Range")]
        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_TotalNumberOfUnits")]
        public int? TotalNumberOfUnits { get; set; }

//        [Numeric(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_BuildingAgeYears_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_BuildingAgeYears_Numeric")]
        [Range(0, 200, ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_BuildingAgeYears_Range")]
        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_BuildingAgeYears")]
        public int? BuildingAgeYears { get; set; }

//        [Numeric(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_NumberOfUnitsPerFloor_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_NumberOfUnitsPerFloor_Numeric")]
        [Range(0, 999, ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_NumberOfUnitsPerFloor_Range")]
        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_NumberOfUnitsPerFloor")]
        public int? NumberOfUnitsPerFloor { get; set; }

//        [Numeric(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_NumberFloorsAboveGround_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_NumberFloorsAboveGround_Numeric")]
        [Range(0, 200, ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_NumberFloorsAboveGround_Range")]
        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_NumberFloorsAboveGround")]
        public int? NumberFloorsAboveGround { get; set; }

        #endregion

        #region Unit properties

//        [Numeric(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_UnitArea_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_UnitArea_Numeric")]
        [Range(1, 99999, ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_UnitArea_Range")]
        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_UnitArea")]
        public decimal? UnitArea { get; set; }

//        [Numeric(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_StorageRoomArea_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_StorageRoomArea_Numeric")]
        [Range(0, 9999, ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_StorageRoomArea_Range")]
        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_StorageRoomArea")]
        public decimal? StorageRoomArea { get; set; }

        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_UnitUsageType")]
        public UnitUsageType? UnitUsageType { get; set; }

//        [Numeric(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_NumberOfRooms_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_NumberOfRooms_Numeric")]
        [Range(0, 99, ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_NumberOfRooms_Range")]
        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_NumberOfRooms")]
        public int? NumberOfRooms { get; set; }

//        [Numeric(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_NumberOfParkings_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_NumberOfParkings_Numeric")]
        [Range(0, 99, ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_NumberOfParkings_Range")]
        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_NumberOfParkings")]
        public int? NumberOfParkings { get; set; }

//        [Numeric(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_UnitFloorNumber_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_UnitFloorNumber_Numeric")]
        [Range(-2, 200, ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_UnitFloorNumber_Range")]
        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_UnitFloorNumber")]
        public int? UnitFloorNumber { get; set; }

        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_AdditionalSpecialFeatures")]
        [StringLength(1000, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
        public string AdditionalSpecialFeatures { get; set; }

        #endregion

        #region Extra unit properties
//        [Numeric(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_NumberOfMasterBedrooms_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_NumberOfMasterBedrooms_Numeric")]
        [Range(0, 99, ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_NumberOfMasterBedrooms_Range")]
        
        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_NumberOfMasterBedrooms")]
        public int? NumberOfMasterBedrooms { get; set; }

        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_KitchenCabinetTopType")]
        public KitchenCabinetTopType? KitchenCabinetTopType { get; set; }

        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_KitchenCabinetBodyType")]
        public KitchenCabinetBodyType? KitchenCabinetBodyType { get; set; }

        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_MainDaylightDirection")]
        public DaylightDirection? MainDaylightDirection { get; set; }

        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_LivingRoomFloor")]
        public FloorCoverType? LivingRoomFloor { get; set; }

        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_FaceType")]
        public BuildingFaceType? FaceType { get; set; }

        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_IsDuplex")]
        public bool? IsDuplex { get; set; }

        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_HasIranianLavatory")]
        public bool? HasIranianLavatory { get; set; }

        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_HasForeignLavatory")]
        public bool? HasForeignLavatory { get; set; }

        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_HasPrivatePatio")]
        public bool? HasPrivatePatio { get; set; }

        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_HasBeenReconstructed")]
        public bool? HasBeenReconstructed { get; set; }

        #endregion

        #region Extra building properties

        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_HasElevator")]
        public bool? HasElevator { get; set; }

        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_HasGatheringHall")]
        public bool? HasGatheringHall { get; set; }

        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_HasAutomaticParkingDoor")]
        public bool? HasAutomaticParkingDoor { get; set; }

        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_HasVideoEyePhone")]
        public bool? HasVideoEyePhone { get; set; }

        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_HasSwimmingPool")]
        public bool? HasSwimmingPool { get; set; }

        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_HasSauna")]
        public bool? HasSauna { get; set; }

        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_HasJacuzzi")]
        public bool? HasJacuzzi { get; set; }

        #endregion

        #region Sales price

        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_PriceSpecificationType")]
        public SalePriceSpecificationType? PriceSpecificationType { get; set; }

        [SkipValidationIfProperty("PriceSpecificationType", PropertyValidationComparisonUtil.ComparisonType.NotEquals, SalePriceSpecificationType.Total)]
//        [Numeric(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_Price_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_Price_Numeric")]
        [Range(1, 9999999999999, ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_Price_Range")]
        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_Price")]
        public decimal? Price { get; set; }

        [SkipValidationIfProperty("PriceSpecificationType", PropertyValidationComparisonUtil.ComparisonType.NotEquals, SalePriceSpecificationType.PerEstateArea)]
//        [Numeric(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_PricePerEstateArea_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_PricePerEstateArea_Numeric")]
        [Range(1, 9999999999, ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_PricePerEstateArea_Range")]
        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_PricePerEstateArea")]
        public decimal? PricePerEstateArea { get; set; }

        [SkipValidationIfProperty("PriceSpecificationType", PropertyValidationComparisonUtil.ComparisonType.NotEquals, SalePriceSpecificationType.PerUnitArea)]
//        [Numeric(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_PricePerUnitArea_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_PricePerUnitArea_Numeric")]
        [Range(1, 9999999999, ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_PricePerUnitArea_Range")]
        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_PricePerUnitArea")]
        public decimal? PricePerUnitArea { get; set; }

        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_HasTransferableLoan")]
        public bool HasTransferableLoan { get; set; }

        [SkipValidationIfProperty("HasTransferableLoan", PropertyValidationComparisonUtil.ComparisonType.Equals, false)]
//        [Numeric(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_TransferableLoanAmount_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_TransferableLoanAmount_Numeric")]
        [Range(0, 9999999999999, ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_TransferableLoanAmount_Range")]
        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_TransferableLoanAmount")]
        public decimal? TransferableLoanAmount { get; set; }

        #endregion

        #region Rent price

//        [Numeric(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_Mortgage_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_Mortgage_Numeric")]
        [Range(0, 99999999999, ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_Mortgage_Range")]
        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_Mortgage")]
        public decimal? Mortgage { get; set; }

//        [Numeric(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_Rent_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_Rent_Numeric")]
        [Range(0, 9999999999, ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_Rent_Range")]
        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_Rent")]
        public decimal? Rent { get; set; }

        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_MortgageAndRentConvertible")]
        public bool MortgageAndRentConvertible { get; set; }

        [SkipValidationIfProperty("MortgageAndRentConvertible", PropertyValidationComparisonUtil.ComparisonType.NotEquals, true)]
//        [Numeric(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_MinimumMortgage_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_MinimumMortgage_Numeric")]
        [Range(0, 99999999999, ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_MinimumMortgage_Range")]
        [CompareToPropertyValue(PropertyValidationComparisonUtil.ComparisonType.LessThanOrEquals, "Mortgage", ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_MinimumMortgage_CompareToPropertyValue")]
        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_MinimumMortgage")]
        public decimal? MinimumMortgage { get; set; }

        [SkipValidationIfProperty("MortgageAndRentConvertible", PropertyValidationComparisonUtil.ComparisonType.NotEquals, true)]
//        [Numeric(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_MinimumRent_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_MinimumRent_Numeric")]
        [Range(0, 9999999999, ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_MinimumRent_Range")]
        [CompareToPropertyValue(PropertyValidationComparisonUtil.ComparisonType.LessThanOrEquals, "Rent", ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_MinimumRent_CompareToPropertyValue")]
        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_MinimumRent")]
        public decimal? MinimumRent { get; set; }



        #endregion

        #region Contact info

        public long? ContactInfoID { get; set; }

        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_AgencyName")]
        [StringLength(80, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
        public string AgencyName { get; set; }

        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_AgencyAddress")]
        [StringLength(200, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
        public string AgencyAddress { get; set; }

        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_ContactName")]
        [StringLength(80, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
        public string ContactName { get; set; }

        [PossiblePhoneNumber(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_ContactPhone1_PhoneNumber")]
        [StringLength(25, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_ContactPhone1")]
        public string ContactPhone1 { get; set; }

        [PossiblePhoneNumber(ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_ContactPhone2_PhoneNumber")]
        [StringLength(25, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_ContactPhone2")]
        public string ContactPhone2 { get; set; }

        [Common.Util.Web.Attributes.EmailAddress(true, ErrorMessageResourceType = typeof(PropertyRequestEditModelValidationResource), ErrorMessageResourceName = "Validation_ContactEmail_Email")]
        [StringLength(100, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_ContactEmail")]
        public string ContactEmail { get; set; }
        #endregion

        #region Publish

        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_PublishDuration")]
        public PropertyPublishDurationEnum? PublishDuration { get; set; }

        #endregion

        #region Shop-specific properties

        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_HasIranianLavatory")]
        public bool? ShopHasIranianLavatory { get; set; }

        [Display(ResourceType = typeof(PropertyRequestEditModelResources), Name = "Label_HasForeignLavatory")]
        public bool? ShopHasForeignLavatory { get; set; }

        #endregion

        #region Output-only properties

        public long? ID { get; set; }
        public long? Code { get; set; }
        public ValidationResult PublishValidationResult { get; set; }
        public List<PropertyListingContactInfo> ExistingContactInfos { get; set; }
        #endregion

        public static void ConfigureMapper()
        {
        }
    }
}


