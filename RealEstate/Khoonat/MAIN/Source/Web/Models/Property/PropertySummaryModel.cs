using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.Common.Util.Web.Attributes;
using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Domain.Property;
using JahanJooy.RealEstate.Web.Resources.Controllers.Property;
using JahanJooy.RealEstate.Web.Resources.Models;
using JahanJooy.RealEstate.Web.Resources.Models.Property;
using ValidationResult = JahanJooy.Common.Util.Validation.ValidationResult;

namespace JahanJooy.RealEstate.Web.Models.Property
{
    [Bind(Exclude = "ID,Code,PublishValidationResult,ExistingContactInfos")]
    public class PropertySummaryModel
    {
        #region Required properties (record type specification)

        [Required(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_Required_IntentionOfOwner")]
        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_IntentionOfOwner")]
        public IntentionOfOwner? IntentionOfOwner { get; set; }

        [Required(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_Required_PropertyType")]
        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_PropertyType")]
        public PropertyType? PropertyType { get; set; }

        [Required(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_Required_AgencyListing")]
        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_AgencyListing")]
        public PropertyListingCreatorRoleType? AgencyListing { get; set; }

        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_NoAgencyListing_AgencyPermitted")]
        public NonAgencyPropertyListingAgencyActivityAllowance? NoAgencyListingAgencyPermitted { get; set; }

        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_AgencyListing_OtherAgencyPermitted")]
        public AgencyPropertyListingAgencyActivityAllowance? AgencyListingOtherAgencyPermitted { get; set; }

        #endregion

        #region Location properties

        public long? SelectedVicinityID { get; set; }
        public Vicinity Vicinity { get; set; }

        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_Address")]
        [StringLength(150, ErrorMessageResourceType = typeof (GeneralValidationErrorResources),ErrorMessageResourceName = "StringLength")]
        public string Address { get; set; }

        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_AdditionalAddressInfo")]
        [StringLength(1000, ErrorMessageResourceType = typeof (GeneralValidationErrorResources),ErrorMessageResourceName = "StringLength")]
        public string AdditionalAddressInfo { get; set; }

        public double? UserPointLat { get; set; }
        public double? UserPointLng { get; set; }
        public GeographicLocationSpecificationType? GeographicLocationType { get; set; }

        #endregion

        #region Estate properties

        [Numeric(ErrorMessageResourceType = typeof (PropertySummaryResources), ErrorMessageResourceName = "Validation_EstateArea_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_EstateArea_Numeric")]
        [Range(1, 10000000, ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_EstateArea_Range")]
        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_EstateArea")]
        public decimal? EstateArea { get; set; }

        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_EstateDirection")]
        public EstateDirection? EstateDirection { get; set; }

        [Numeric(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_PassageEdgeLength_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_PassageEdgeLength_Numeric")]
        [Range(1, 1000, ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_PassageEdgeLength_Range")]
        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_PassageEdgeLength")]
        public decimal? PassageEdgeLength { get; set; }

        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_EstateVoucherType")]
        public EstateVoucherType? EstateVoucherType { get; set; }

        #endregion

        #region Building properties

        [Numeric(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_TotalNumberOfUnits_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_TotalNumberOfUnits_Numeric")]
        [Range(0, 99999, ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_TotalNumberOfUnits_Range")]
        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_TotalNumberOfUnits")]
        public int? TotalNumberOfUnits { get; set; }

        [Numeric(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_BuildingAgeYears_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_BuildingAgeYears_Numeric")]
        [Range(0, 200, ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_BuildingAgeYears_Range")]
        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_BuildingAgeYears")]
        public int? BuildingAgeYears { get; set; }

        [Numeric(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_NumberOfUnitsPerFloor_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_NumberOfUnitsPerFloor_Numeric")]
        [Range(0, 999, ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_NumberOfUnitsPerFloor_Range")]
        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_NumberOfUnitsPerFloor")]
        public int? NumberOfUnitsPerFloor { get; set; }

        [Numeric(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_NumberFloorsAboveGround_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_NumberFloorsAboveGround_Numeric")]
        [Range(0, 200, ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_NumberFloorsAboveGround_Range")]
        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_NumberFloorsAboveGround")]
        public int? NumberFloorsAboveGround { get; set; }

        #endregion

        #region Unit properties

        [Numeric(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_UnitArea_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_UnitArea_Numeric")]
        [Range(1, 99999, ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_UnitArea_Range")]
        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_UnitArea")]
        public decimal? UnitArea { get; set; }

        [Numeric(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_StorageRoomArea_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_StorageRoomArea_Numeric")]
        [Range(0, 9999, ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_StorageRoomArea_Range")]
        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_StorageRoomArea")]
        public decimal? StorageRoomArea { get; set; }

        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_UnitUsageType")]
        public UnitUsageType? UnitUsageType { get; set; }

        [Numeric(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_NumberOfRooms_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_NumberOfRooms_Numeric")]
        [Range(0, 99, ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_NumberOfRooms_Range")]
        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_NumberOfRooms")]
        public int? NumberOfRooms { get; set; }

        [Numeric(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_NumberOfParkings_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_NumberOfParkings_Numeric")]
        [Range(0, 99, ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_NumberOfParkings_Range")]
        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_NumberOfParkings")]
        public int? NumberOfParkings { get; set; }

        [Numeric(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_UnitFloorNumber_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_UnitFloorNumber_Numeric")]
        [Range(-2, 200, ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_UnitFloorNumber_Range")]
        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_UnitFloorNumber")]
        public int? UnitFloorNumber { get; set; }

        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_AdditionalSpecialFeatures")]
        [StringLength(1000, ErrorMessageResourceType = typeof (GeneralValidationErrorResources),ErrorMessageResourceName = "StringLength")]
        public string AdditionalSpecialFeatures { get; set; }

        #endregion

        #region Extra unit properties

        [Numeric(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_NumberOfMasterBedrooms_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_NumberOfMasterBedrooms_Numeric")]
        [Range(0, 99, ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_NumberOfMasterBedrooms_Range")]
        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_NumberOfMasterBedrooms")]
        public int? NumberOfMasterBedrooms { get; set; }

        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_KitchenCabinetTopType")]
        public KitchenCabinetTopType? KitchenCabinetTopType { get; set; }

        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_KitchenCabinetBodyType")]
        public KitchenCabinetBodyType? KitchenCabinetBodyType { get; set; }

        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_MainDaylightDirection")]
        public DaylightDirection? MainDaylightDirection { get; set; }

        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_LivingRoomFloor")]
        public FloorCoverType? LivingRoomFloor { get; set; }

        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_FaceType")]
        public BuildingFaceType? FaceType { get; set; }

        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_IsDuplex")]
        public bool? IsDuplex { get; set; }

        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_HasIranianLavatory")]
        public bool? HasIranianLavatory { get; set; }

        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_HasForeignLavatory")]
        public bool? HasForeignLavatory { get; set; }

        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_HasPrivatePatio")]
        public bool? HasPrivatePatio { get; set; }

        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_HasBeenReconstructed")]
        public bool? HasBeenReconstructed { get; set; }

        #endregion

        #region Extra building properties

        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_HasElevator")]
        public bool? HasElevator { get; set; }

        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_HasGatheringHall")]
        public bool? HasGatheringHall { get; set; }

        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_HasAutomaticParkingDoor")]
        public bool? HasAutomaticParkingDoor { get; set; }

        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_HasVideoEyePhone")]
        public bool? HasVideoEyePhone { get; set; }

        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_HasSwimmingPool")]
        public bool? HasSwimmingPool { get; set; }

        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_HasSauna")]
        public bool? HasSauna { get; set; }

        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_HasJacuzzi")]
        public bool? HasJacuzzi { get; set; }

        #endregion

        #region Sales price

        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_PriceSpecificationType")]
        public SalePriceSpecificationType? PriceSpecificationType { get; set; }

        [SkipValidationIfProperty("PriceSpecificationType", PropertyValidationComparisonUtil.ComparisonType.NotEquals,SalePriceSpecificationType.Total)]
        [Numeric(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_Price_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_Price_Numeric")]
        [Range(1, 9999999999999, ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_Price_Range")]
        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_Price")]
        public decimal? Price { get; set; }

        [SkipValidationIfProperty("PriceSpecificationType", PropertyValidationComparisonUtil.ComparisonType.NotEquals,SalePriceSpecificationType.PerEstateArea)]
        [Numeric(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_PricePerEstateArea_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_PricePerEstateArea_Numeric")]
        [Range(1, 9999999999, ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_PricePerEstateArea_Range")]
        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_PricePerEstateArea")]
        public decimal? PricePerEstateArea { get; set; }

        [SkipValidationIfProperty("PriceSpecificationType", PropertyValidationComparisonUtil.ComparisonType.NotEquals,SalePriceSpecificationType.PerUnitArea)]
        [Numeric(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_PricePerUnitArea_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_PricePerUnitArea_Numeric")]
        [Range(1, 9999999999, ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_PricePerUnitArea_Range")]
        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_PricePerUnitArea")]
        public decimal? PricePerUnitArea { get; set; }

        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_HasTransferableLoan")]
        public bool HasTransferableLoan { get; set; }

        [SkipValidationIfProperty("HasTransferableLoan", PropertyValidationComparisonUtil.ComparisonType.Equals, false)]
        [Numeric(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_TransferableLoanAmount_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_TransferableLoanAmount_Numeric")]
        [Range(0, 9999999999999, ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_TransferableLoanAmount_Range")]
        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_TransferableLoanAmount")]
        public decimal? TransferableLoanAmount { get; set; }

        #endregion

        #region Rent price

        [Numeric(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_Mortgage_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_Mortgage_Numeric")]
        [Range(0, 99999999999, ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_Mortgage_Range")]
        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_Mortgage")]
        public decimal? Mortgage { get; set; }

        [Numeric(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_Rent_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_Rent_Numeric")]
        [Range(0, 9999999999, ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_Rent_Range")]
        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_Rent")]
        public decimal? Rent { get; set; }

        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_MortgageAndRentConvertible")]
        public bool MortgageAndRentConvertible { get; set; }

        [SkipValidationIfProperty("MortgageAndRentConvertible",PropertyValidationComparisonUtil.ComparisonType.NotEquals, true)]
        [Numeric(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_MinimumMortgage_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_MinimumMortgage_Numeric")]
        [Range(0, 99999999999, ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_MinimumMortgage_Range")]
        [CompareToPropertyValue(PropertyValidationComparisonUtil.ComparisonType.LessThanOrEquals, "Mortgage",ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_MinimumMortgage_CompareToPropertyValue")]
        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_MinimumMortgage")]
        public decimal? MinimumMortgage { get; set; }

        [SkipValidationIfProperty("MortgageAndRentConvertible",PropertyValidationComparisonUtil.ComparisonType.NotEquals, true)]
        [Numeric(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_MinimumRent_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_MinimumRent_Numeric")]
        [Range(0, 9999999999, ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_MinimumRent_Range")]
        [CompareToPropertyValue(PropertyValidationComparisonUtil.ComparisonType.LessThanOrEquals, "Rent",ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_MinimumRent_CompareToPropertyValue")]
        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_MinimumRent")]
        public decimal? MinimumRent { get; set; }

        #endregion

        #region Contact info

        public long? ContactInfoID { get; set; }

        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_AgencyName")]
        [StringLength(80, ErrorMessageResourceType = typeof (GeneralValidationErrorResources),ErrorMessageResourceName = "StringLength")]
        public string AgencyName { get; set; }

        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_AgencyAddress")]
        [StringLength(200, ErrorMessageResourceType = typeof (GeneralValidationErrorResources),ErrorMessageResourceName = "StringLength")]
        public string AgencyAddress { get; set; }

        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_ContactName")]
        [StringLength(80, ErrorMessageResourceType = typeof (GeneralValidationErrorResources),ErrorMessageResourceName = "StringLength")]
        public string ContactName { get; set; }

        [PossiblePhoneNumber(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_ContactPhone1_PhoneNumber")]
        [StringLength(25, ErrorMessageResourceType = typeof (GeneralValidationErrorResources),ErrorMessageResourceName = "StringLength")]
        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_ContactPhone1")]
        public string ContactPhone1 { get; set; }

        [PossiblePhoneNumber(ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_ContactPhone2_PhoneNumber")]
        [StringLength(25, ErrorMessageResourceType = typeof (GeneralValidationErrorResources),ErrorMessageResourceName = "StringLength")]
        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_ContactPhone2")]
        public string ContactPhone2 { get; set; }

        [Common.Util.Web.Attributes.EmailAddress(true, ErrorMessageResourceType = typeof (PropertySummaryResources),ErrorMessageResourceName = "Validation_ContactEmail_Email")]
        [StringLength(100, ErrorMessageResourceType = typeof (GeneralValidationErrorResources),ErrorMessageResourceName = "StringLength")]
        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_ContactEmail")]
        public string ContactEmail { get; set; }

        #endregion

        #region Publish

        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_PublishDuration")]
        public PropertyPublishDurationEnum? PublishDuration { get; set; }

        #endregion

        #region Shop-specific properties

        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_HasIranianLavatory")]
        public bool? ShopHasIranianLavatory { get; set; }

        [Display(ResourceType = typeof (PropertySummaryResources), Name = "Label_HasForeignLavatory")]
        public bool? ShopHasForeignLavatory { get; set; }

        #endregion

        #region Output-only properties

        public long? ID { get; set; }
        public long? Code { get; set; }
        public ValidationResult PublishValidationResult { get; set; }
        public List<PropertyListingContactInfo> ExistingContactInfos { get; set; }

        #endregion

        #region Conversion methods

        public PropertyListing ToDomain()
        {
            var result = new PropertyListing();

            if (PropertyType.HasValue) result.PropertyType = PropertyType.Value;
            if (IntentionOfOwner.HasValue) result.IntentionOfOwner = IntentionOfOwner.Value;

            UpdateDomain(result);

            return result;
        }

        public PropertyListing UpdateDomain(PropertyListing listing)
        {
            if (listing.Estate == null)
                listing.Estate = new Estate();

            listing.Estate.VicinityID = SelectedVicinityID;
            listing.Estate.Address = Address;
            listing.Estate.AdditionalAddressInfo = AdditionalAddressInfo;
            listing.Estate.Area = EstateArea;
            listing.Estate.Direction = EstateDirection;
            listing.Estate.PassageEdgeLength = PassageEdgeLength;
            listing.Estate.VoucherType = EstateVoucherType;
            if (GeographicLocationType != null)
            {
                listing.Estate.GeographicLocationType = GeographicLocationType;
                if (UserPointLat != null && UserPointLng != null)
                {
                    var userpoint = new LatLng
                    {
                        Lat = UserPointLat.Value,
                        Lng = UserPointLng.Value
                    };
                    listing.Estate.GeographicLocation = userpoint.ToDbGeography();
                }
                else
                {
                    listing.Estate.GeographicLocation = null;
                }
            }
            else
            {
                listing.Estate.GeographicLocationType = null;
                listing.Estate.GeographicLocation = null;
            }

            if (listing.Building == null)
                listing.Building = new Building();

            listing.Building.TotalNumberOfUnits = TotalNumberOfUnits;
            listing.Building.BuildingAgeYears = BuildingAgeYears;
            listing.Building.NumberOfUnitsPerFloor = NumberOfUnitsPerFloor;
            listing.Building.NumberFloorsAboveGround = NumberFloorsAboveGround;
            listing.Building.FaceType = FaceType;

            listing.Building.HasElevator = HasElevator;
            listing.Building.HasGatheringHall = HasGatheringHall;
            listing.Building.HasAutomaticParkingDoor = HasAutomaticParkingDoor;
            listing.Building.HasVideoEyePhone = HasVideoEyePhone;
            listing.Building.HasSwimmingPool = HasSwimmingPool;
            listing.Building.HasSauna = HasSauna;
            listing.Building.HasJacuzzi = HasJacuzzi;

            if (listing.Unit == null)
                listing.Unit = new Unit();

            listing.Unit.FloorNumber = UnitFloorNumber;
            listing.Unit.UsageType = UnitUsageType;
            listing.Unit.Area = UnitArea;
            listing.Unit.StorageRoomArea = StorageRoomArea;
            listing.Unit.NumberOfRooms = NumberOfRooms;
            listing.Unit.NumberOfParkings = NumberOfParkings;
            listing.Unit.AdditionalSpecialFeatures = AdditionalSpecialFeatures;

            listing.Unit.NumberOfMasterBedrooms = NumberOfMasterBedrooms;
            listing.Unit.KitchenCabinetTopType = KitchenCabinetTopType;
            listing.Unit.KitchenCabinetBodyType = KitchenCabinetBodyType;
            listing.Unit.MainDaylightDirection = MainDaylightDirection;
            listing.Unit.LivingRoomFloor = LivingRoomFloor;
            listing.Unit.IsDuplex = IsDuplex;
            listing.Unit.HasPrivatePatio = HasPrivatePatio;
            listing.Unit.HasBeenReconstructed = HasBeenReconstructed;

            if (IntentionOfOwner.HasValue && IntentionOfOwner.Value == Domain.Enums.IntentionOfOwner.ForSale)
            {
                if (listing.SaleConditions == null)
                    listing.SaleConditions = new SaleConditions();

                listing.SaleConditions.PriceSpecificationType = PriceSpecificationType;
                listing.SaleConditions.Price = Price;
                listing.SaleConditions.PricePerEstateArea = PricePerEstateArea;
                listing.SaleConditions.PricePerUnitArea = PricePerUnitArea;
                listing.SaleConditions.HasTransferableLoan = HasTransferableLoan;
                listing.SaleConditions.TransferableLoanAmount = TransferableLoanAmount;
            }

            if (IntentionOfOwner.HasValue && IntentionOfOwner.Value == Domain.Enums.IntentionOfOwner.ForRent)
            {
                if (listing.RentConditions == null)
                    listing.RentConditions = new RentConditions();

                listing.RentConditions.Mortgage = Mortgage;
                listing.RentConditions.Rent = Rent;
                listing.RentConditions.MortgageAndRentConvertible = MortgageAndRentConvertible;
                listing.RentConditions.MinimumRent = MinimumRent;
                listing.RentConditions.MinimumMortgage = MinimumMortgage;
            }

            //
            // ContactInfo selector / editor logic

            if (ContactInfoID.HasValue)
            {
                // User has a previously-saved contact info selected

                if (listing.ContactInfo != null && ContactInfoID.Value == listing.ContactInfo.ID)
                {
                    // The selected contact info is the same as the one previously saved on the record (The selection is not changed by the user)
                    // So, just update the content

                    listing.ContactInfo.AgencyName = AgencyName;
                    listing.ContactInfo.AgencyAddress = AgencyAddress;
                    listing.ContactInfo.ContactName = ContactName;
                    listing.ContactInfo.ContactPhone1 = ContactPhone1;
                    listing.ContactInfo.ContactPhone2 = ContactPhone2;
                    listing.ContactInfo.ContactEmail = ContactEmail;
                }
                else
                {
                    // User has changed the selection. Since the user can't change the selection and edit the texts at the same time,
                    // we just replace the ID.

                    listing.ContactInfoID = ContactInfoID;
                }
            }
            else
            {
                // User has specified "I want to enter new contact info" option
                // So we create and assign a new ContactInfo entity.

                listing.ContactInfo = new PropertyListingContactInfo();

                listing.ContactInfo.AgencyName = AgencyName;
                listing.ContactInfo.AgencyAddress = AgencyAddress;
                listing.ContactInfo.ContactName = ContactName;
                listing.ContactInfo.ContactPhone1 = ContactPhone1;
                listing.ContactInfo.ContactPhone2 = ContactPhone2;
                listing.ContactInfo.ContactEmail = ContactEmail;
            }

            if (listing.Estate.Buildings == null || !listing.Estate.Buildings.Contains(listing.Building))
                listing.Estate.Buildings = new List<Building> {listing.Building};

            if (listing.Building.Units == null || !listing.Building.Units.Contains(listing.Unit))
                listing.Building.Units = new List<Unit> {listing.Unit};

            listing.IsAgencyListing = AgencyListing == PropertyListingCreatorRoleType.AgencyListing;

            listing.IsAgencyActivityAllowed = ((AgencyListing == PropertyListingCreatorRoleType.NoAgencyListing &&
                                                NoAgencyListingAgencyPermitted ==
                                                NonAgencyPropertyListingAgencyActivityAllowance.AgencyPermitted)
                                               ||
                                               (AgencyListing == PropertyListingCreatorRoleType.AgencyListing &&
                                                AgencyListingOtherAgencyPermitted ==
                                                AgencyPropertyListingAgencyActivityAllowance.OtherAgencyPermitted));

            //
            // Property-type specific values

            if (listing.PropertyType == Domain.Enums.PropertyType.Shop)
            {
                listing.Unit.HasIranianLavatory = ShopHasIranianLavatory;
                listing.Unit.HasForeignLavatory = ShopHasForeignLavatory;
            }
            else
            {
                listing.Unit.HasIranianLavatory = HasIranianLavatory;
                listing.Unit.HasForeignLavatory = HasForeignLavatory;
            }


            return listing;
        }

        public static PropertySummaryModel FromDomain(PropertyListingDetails listing)
        {
            var result = new PropertySummaryModel();

            result.ID = listing.ID;
            result.Code = listing.Code;
            result.PropertyType = listing.PropertyType;
            result.IntentionOfOwner = listing.IntentionOfOwner;
            result.ContactInfoID = listing.ContactInfoID;

            if (listing.Estate != null)
            {
                result.SelectedVicinityID = listing.Estate.VicinityID;
                result.Address = listing.Estate.Address;
                result.AdditionalAddressInfo = listing.Estate.AdditionalAddressInfo;
                result.EstateArea = listing.Estate.Area;
                result.EstateDirection = listing.Estate.Direction;
                result.PassageEdgeLength = listing.Estate.PassageEdgeLength;
                result.EstateVoucherType = listing.Estate.VoucherType;
                result.GeographicLocationType = listing.Estate.GeographicLocationType;
                if (listing.Estate.GeographicLocation != null)
                {
                    result.UserPointLat = listing.Estate.GeographicLocation.Latitude;
                    result.UserPointLng = listing.Estate.GeographicLocation.Longitude;
                }
            }

            if (listing.Building != null)
            {
                result.TotalNumberOfUnits = listing.Building.TotalNumberOfUnits;
                result.BuildingAgeYears = listing.Building.BuildingAgeYears;
                result.NumberOfUnitsPerFloor = listing.Building.NumberOfUnitsPerFloor;
                result.NumberFloorsAboveGround = listing.Building.NumberFloorsAboveGround;
                result.FaceType = listing.Building.FaceType;

                result.HasElevator = listing.Building.HasElevator;
                result.HasGatheringHall = listing.Building.HasGatheringHall;
                result.HasAutomaticParkingDoor = listing.Building.HasAutomaticParkingDoor;
                result.HasVideoEyePhone = listing.Building.HasVideoEyePhone;
                result.HasSwimmingPool = listing.Building.HasSwimmingPool;
                result.HasSauna = listing.Building.HasSauna;
                result.HasJacuzzi = listing.Building.HasJacuzzi;
            }

            if (listing.Unit != null)
            {
                result.UnitFloorNumber = listing.Unit.FloorNumber;
                result.UnitUsageType = listing.Unit.UsageType;
                result.UnitArea = listing.Unit.Area;
                result.StorageRoomArea = listing.Unit.StorageRoomArea;
                result.NumberOfRooms = listing.Unit.NumberOfRooms;
                result.NumberOfParkings = listing.Unit.NumberOfParkings;
                result.AdditionalSpecialFeatures = listing.Unit.AdditionalSpecialFeatures;

                result.NumberOfMasterBedrooms = listing.Unit.NumberOfMasterBedrooms;
                result.KitchenCabinetTopType = listing.Unit.KitchenCabinetTopType;
                result.KitchenCabinetBodyType = listing.Unit.KitchenCabinetBodyType;
                result.MainDaylightDirection = listing.Unit.MainDaylightDirection;
                result.LivingRoomFloor = listing.Unit.LivingRoomFloor;
                result.IsDuplex = listing.Unit.IsDuplex;
                result.HasIranianLavatory = listing.Unit.HasIranianLavatory;
                result.HasForeignLavatory = listing.Unit.HasForeignLavatory;
                result.ShopHasIranianLavatory = listing.Unit.HasIranianLavatory;
                result.ShopHasForeignLavatory = listing.Unit.HasForeignLavatory;
                result.HasPrivatePatio = listing.Unit.HasPrivatePatio;
                result.HasBeenReconstructed = listing.Unit.HasBeenReconstructed;
            }

            if (listing.SaleConditions != null)
            {
                result.PriceSpecificationType = listing.SaleConditions.PriceSpecificationType;
                result.Price = listing.SaleConditions.Price;
                result.PricePerEstateArea = listing.SaleConditions.PricePerEstateArea;
                result.PricePerUnitArea = listing.SaleConditions.PricePerUnitArea;
                result.HasTransferableLoan = listing.SaleConditions.HasTransferableLoan;
                result.TransferableLoanAmount = listing.SaleConditions.TransferableLoanAmount;
            }

            if (listing.RentConditions != null)
            {
                result.Mortgage = listing.RentConditions.Mortgage;
                result.Rent = listing.RentConditions.Rent;
                result.MortgageAndRentConvertible = listing.RentConditions.MortgageAndRentConvertible;
                result.MinimumRent = listing.RentConditions.MinimumRent;
                result.MinimumMortgage = listing.RentConditions.MinimumMortgage;
            }

            if (listing.ContactInfo != null)
            {
                result.AgencyName = listing.ContactInfo.AgencyName;
                result.AgencyAddress = listing.ContactInfo.AgencyAddress;
                result.ContactName = listing.ContactInfo.ContactName;
                result.ContactPhone1 = listing.ContactInfo.ContactPhone1;
                result.ContactPhone2 = listing.ContactInfo.ContactPhone2;
                result.ContactEmail = listing.ContactInfo.ContactEmail;
            }

            if (listing.IsAgencyListing)
            {
                result.AgencyListing = PropertyListingCreatorRoleType.AgencyListing;
                result.AgencyListingOtherAgencyPermitted = listing.IsAgencyActivityAllowed
                    ? AgencyPropertyListingAgencyActivityAllowance.OtherAgencyPermitted
                    : AgencyPropertyListingAgencyActivityAllowance.OtherAgencyNotPermitted;
                result.NoAgencyListingAgencyPermitted = NonAgencyPropertyListingAgencyActivityAllowance.AgencyPermitted;
            }
            else
            {
                result.AgencyListing = PropertyListingCreatorRoleType.NoAgencyListing;
                result.AgencyListingOtherAgencyPermitted =
                    AgencyPropertyListingAgencyActivityAllowance.OtherAgencyPermitted;
                result.NoAgencyListingAgencyPermitted = listing.IsAgencyActivityAllowed
                    ? NonAgencyPropertyListingAgencyActivityAllowance.AgencyPermitted
                    : NonAgencyPropertyListingAgencyActivityAllowance.AgencyNotPermitted;
            }

            return result;
        }

        public void AddDomainValidationErrors(ValidationResult publishValidation, ModelStateDictionary modelState)
        {
            foreach (var validationError in publishValidation.Errors)
            {
                modelState.AddModelError(MapValidationErrorPropertyPath(validationError.PropertyPath),
                    PublishValidationErrors.ResourceManager.GetString(validationError.FullResourceKey));
            }
        }

        private string MapValidationErrorPropertyPath(string domainPath)
        {
            if (ValidationErrorPropertyPathMap.ContainsKey(domainPath))
                return ValidationErrorPropertyPathMap[domainPath];

            var splitDomainPath = domainPath.Split('.');
            return splitDomainPath[splitDomainPath.Length - 1];
        }

        private static readonly Dictionary<string, string> ValidationErrorPropertyPathMap =
            new Dictionary<string, string>
            {
                {"Estate.Vicinity", "VicinityID"},
                {"Estate.PlateNumber", "PlateNo"},
                {"Estate.Area", "EstateArea"},
                {"Estate.Direction", "EstateDirection"},
                {"Estate.VoucherType", "EstateVoucherType"},
                {"Unit.BlockNumber", "BlockNo"},
                {"Unit.FlatNumber", "FlatNo"},
                {"Unit.FloorNumber", "UnitFloorNumber"},
                {"Unit.UsageType", "UnitUsageType"},
                {"Unit.Area", "UnitArea"}
            };

        #endregion
    }
}