using System.Web;
using JahanJooy.Common.Util.Text;
using JahanJooy.Common.Util.Web.Extensions;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Util.Resources;
using JahanJooy.RealEstate.Domain.Enums;

namespace JahanJooy.RealEstate.Util.Presentation.Property
{
	public static class PropertyViewSummaryHelper
	{
		public static string PropertyType(PropertyListingDetails listing)
		{
			return listing == null ? null : listing.PropertyType.Label(DomainEnumResources.ResourceManager);
		}

		public static IHtmlString Area(PropertyListingDetails listing)
		{
			if (listing == null)
				return null;

			if (!listing.PropertyType.IsEstateSignificant())
				return listing.Unit != null && listing.Unit.Area.HasValue ? new HtmlString(string.Format(PropertyViewSummaryHelperResources.UnitArea, listing.Unit.Area.Value)) : null;

			string result;

			if (listing.PropertyType.HasUnit() && listing.Unit != null && listing.Unit.Area.HasValue)
				result = string.Format(PropertyViewSummaryHelperResources.UnitArea, listing.Unit.Area.Value);
			else
				result = string.Empty;

			if (listing.Estate != null && listing.Estate.Area.HasValue)
			{
				if (!string.IsNullOrWhiteSpace(result))
					result += "<br/>";

				result += string.Format(PropertyViewSummaryHelperResources.EstateArea, listing.Estate.Area.Value);
			}

			return new HtmlString(result);
		}

		public static string EstateVoucherType(PropertyListingDetails listing)
		{
			if (listing == null || listing.Estate == null || !listing.Estate.VoucherType.HasValue)
				return null;

			return string.Format(PropertyViewSummaryHelperResources.EstateVoucherType, listing.Estate.VoucherType.Value.Label(DomainEnumResources.ResourceManager));
		}

		public static string EstateDirection(PropertyListingDetails listing)
		{
			if (listing == null || listing.Estate == null || !listing.Estate.Direction.HasValue)
				return null;

			return string.Format(PropertyViewSummaryHelperResources.EstateDirection, listing.Estate.Direction.Value.Label(DomainEnumResources.ResourceManager));
		}

		public static string EstatePassageEdgeLength(PropertyListingDetails listing)
		{
			if (listing == null || listing.Estate == null || !listing.Estate.PassageEdgeLength.HasValue)
				return null;

			return string.Format(PropertyViewSummaryHelperResources.PassageEdgeLength, listing.Estate.PassageEdgeLength.Value);
		}

		public static string EstateSides(PropertyListingDetails listing)
		{
			if (listing == null || listing.Estate == null || !listing.Estate.Sides.HasValue)
				return null;

			return string.Format(PropertyViewSummaryHelperResources.EstateSides, NumericStringUtils.FullyTextualNumber(listing.Estate.Sides.Value));
		}

		public static string EstatePassageWidth(PropertyListingDetails listing)
		{
			if (listing == null || listing.Estate == null || !listing.Estate.PassageWidth.HasValue)
				return null;

			return string.Format(PropertyViewSummaryHelperResources.PassageWidth, listing.Estate.PassageWidth.Value);
		}

		public static string EstateSlopeAmount(PropertyListingDetails listing)
		{
			if (listing == null || listing.Estate == null || !listing.Estate.SlopeAmount.HasValue)
				return null;

			return string.Format(PropertyViewSummaryHelperResources.SlopeAmount, listing.Estate.SlopeAmount.Value.Label(DomainEnumResources.ResourceManager));
		}

		public static string EstateSurfaceType(PropertyListingDetails listing)
		{
			if (listing == null || listing.Estate == null || !listing.Estate.SurfaceType.HasValue)
				return null;

			return string.Format(PropertyViewSummaryHelperResources.SurfaceType, listing.Estate.SurfaceType.Value.Label(DomainEnumResources.ResourceManager));
		}

		public static string BuildingAge(PropertyListingDetails listing)
		{
			if (listing == null || listing.Building == null || !listing.Building.BuildingAgeYears.HasValue)
				return null;

			if (listing.Building.BuildingAgeYears.Value < 1)
				return PropertyViewSummaryHelperResources.BuildingAge_Zero;

			return string.Format(PropertyViewSummaryHelperResources.BuildingAge, listing.Building.BuildingAgeYears.Value);
		}

		public static string BuildingFloors(PropertyListingDetails listing)
		{
			if (listing == null || listing.Building == null || !listing.Building.NumberFloorsAboveGround.HasValue)
				return null;

			return string.Format(PropertyViewSummaryHelperResources.BuildingFloors, NumericStringUtils.FullyTextualNumber(listing.Building.NumberFloorsAboveGround.Value));
		}

		public static string BuildingUnitsPerFloor(PropertyListingDetails listing)
		{
			if (listing == null || listing.Building == null || !listing.Building.NumberOfUnitsPerFloor.HasValue)
				return null;

			if (listing.Building.NumberOfUnitsPerFloor.Value == 1)
				return PropertyViewSummaryHelperResources.BuildingUnitsPerFloor_One;

			return string.Format(PropertyViewSummaryHelperResources.BuildingUnitsPerFloow, NumericStringUtils.FullyTextualNumber(listing.Building.NumberOfUnitsPerFloor.Value));
		}

		public static string BuildingTotalUnits(PropertyListingDetails listing)
		{
			if (listing == null || listing.Building == null || !listing.Building.TotalNumberOfUnits.HasValue)
				return null;

			return string.Format(PropertyViewSummaryHelperResources.BuildingTotalUnits, listing.Building.TotalNumberOfUnits.Value);
		}

		public static string BuildingFaceType(PropertyListingDetails listing)
		{
			if (listing == null || listing.Building == null || !listing.Building.FaceType.HasValue)
				return null;

			return string.Format(PropertyViewSummaryHelperResources.BuildingFaceType, listing.Building.FaceType.Label(DomainEnumResources.ResourceManager));
		}

		public static string UnitUsageType(PropertyListingDetails listing)
		{
			if (listing == null || listing.Unit == null || !listing.Unit.UsageType.HasValue)
				return null;

			return listing.Unit.UsageType.Value.Label(DomainEnumResources.ResourceManager);
		}

		public static string UnitNumberOfRooms(PropertyListingDetails listing)
		{
			if (listing == null || listing.Unit == null || !listing.Unit.NumberOfRooms.HasValue)
				return null;

			var nor = listing.Unit.NumberOfRooms.Value;
			if (nor <= 0)
				return PropertyViewSummaryHelperResources.NumberOfRooms_Zero;

			if (listing.Unit.UsageType.HasValue && listing.Unit.UsageType.Value == Domain.Enums.UnitUsageType.Residency)
				return string.Format(PropertyViewSummaryHelperResources.NumberOfRooms_Residential, NumericStringUtils.FullyTextualNumber(nor));

			return string.Format(PropertyViewSummaryHelperResources.NumberOfRooms, NumericStringUtils.FullyTextualNumber(nor));
		}

		public static string UnitFloorNumber(PropertyListingDetails listing)
		{
			if (listing == null || listing.Unit == null || !listing.Unit.FloorNumber.HasValue)
				return null;

			var fn = listing.Unit.FloorNumber.Value;
			if (fn == 0)
				return PropertyViewSummaryHelperResources.UnitFloorNumber_Zero;

			return string.Format(PropertyViewSummaryHelperResources.UnitFloorNumber, NumericStringUtils.FullyTextualOrdinalNumber(fn));
		}

		public static string UnitDaylightDirection(PropertyListingDetails listing)
		{
			if (listing == null || listing.Unit == null || !listing.Unit.MainDaylightDirection.HasValue)
				return null;

			return string.Format(PropertyViewSummaryHelperResources.UnitDaylightDirection, listing.Unit.MainDaylightDirection.Value.Label(DomainEnumResources.ResourceManager));
		}

		public static string UnitKitchenCabinetType(PropertyListingDetails listing)
		{
			if (listing == null || listing.Unit == null)
				return null;

			if (!listing.Unit.KitchenCabinetBodyType.HasValue && !listing.Unit.KitchenCabinetTopType.HasValue)
				return null;

			string result;
			if (!listing.Unit.KitchenCabinetBodyType.HasValue)
			{
				result = string.Format(PropertyViewSummaryHelperResources.UnitKitchenCabinetTopType, listing.Unit.KitchenCabinetTopType.Value.Label(DomainEnumResources.ResourceManager));
			}
			else if (!listing.Unit.KitchenCabinetTopType.HasValue)
			{
				result = string.Format(PropertyViewSummaryHelperResources.UnitKitchenCabinetBodyType, listing.Unit.KitchenCabinetBodyType.Value.Label(DomainEnumResources.ResourceManager));
			}
			else
			{
				result = string.Format(PropertyViewSummaryHelperResources.UnitKitchenCabinetBodyType, listing.Unit.KitchenCabinetBodyType.Value.Label(DomainEnumResources.ResourceManager)) + " + " +
				         string.Format(PropertyViewSummaryHelperResources.UnitKitchenCabinetTopType, listing.Unit.KitchenCabinetTopType.Value.Label(DomainEnumResources.ResourceManager));
			}

			return string.Format(PropertyViewSummaryHelperResources.UnitKitchenCabinetType, result);
		}

		public static string UnitKitchenCabinetBodyType(PropertyListingDetails listing)
		{
			if (listing == null || listing.Unit == null || !listing.Unit.KitchenCabinetBodyType.HasValue)
				return null;

			return string.Format(PropertyViewSummaryHelperResources.UnitKitchenCabinetBodyType, listing.Unit.KitchenCabinetBodyType.Value.Label(DomainEnumResources.ResourceManager));
		}

		public static string UnitKitchenCabinetTopType(PropertyListingDetails listing)
		{
			if (listing == null || listing.Unit == null || !listing.Unit.KitchenCabinetTopType.HasValue)
				return null;

			return string.Format(PropertyViewSummaryHelperResources.UnitKitchenCabinetTopType, listing.Unit.KitchenCabinetTopType.Value.Label(DomainEnumResources.ResourceManager));
		}

		public static string UnitLivingRoomFloor(PropertyListingDetails listing)
		{
			if (listing == null || listing.Unit == null || !listing.Unit.LivingRoomFloor.HasValue)
				return null;

			return string.Format(PropertyViewSummaryHelperResources.UnitLivingRoomFloor, listing.Unit.LivingRoomFloor.Value.Label(DomainEnumResources.ResourceManager));
		}

		public static string SalePriceTotal(PropertyListingDetails listing)
		{
			if (listing == null || listing.SaleConditions == null || !listing.SaleConditions.Price.HasValue)
				return null;

			return string.Format(PropertyViewSummaryHelperResources.SalePriceTotal, NumericStringUtils.ShortNumericString(listing.SaleConditions.Price.Value));
		}

		public static string SalePricePerUnitArea(PropertyListingDetails listing)
		{
			if (listing == null)
				return null;

			// Should not return null when there is no unit, or it may show "Unspecified" in the page
			if (!listing.PropertyType.HasUnit())
				return string.Empty;

			if (listing.SaleConditions == null || !listing.SaleConditions.PricePerUnitArea.HasValue)
				return null;

			return string.Format(PropertyViewSummaryHelperResources.SalePricePerUnitArea, NumericStringUtils.ShortNumericString(listing.SaleConditions.PricePerUnitArea.Value));
		}

		public static string SalePricePerEstateArea(PropertyListingDetails listing)
		{
			if (listing == null)
				return null;

			// Should not return null when per-estate-price doesn't have a meaning, or it may show "Unspecified" in the page
			if (!listing.PropertyType.IsEstateSignificant())
				return string.Empty;

			if (listing.SaleConditions == null || !listing.SaleConditions.PricePerEstateArea.HasValue)
				return null;

			return string.Format(PropertyViewSummaryHelperResources.SalePricePerEstateArea, NumericStringUtils.ShortNumericString(listing.SaleConditions.PricePerEstateArea.Value));
		}

		public static bool? SaleTransferableLoanTristate(PropertyListingDetails listing)
		{
			if (listing == null || listing.SaleConditions == null)
				return null;
			
			return listing.SaleConditions.HasTransferableLoan;
		}

		public static string SaleTransferableLoan(PropertyListingDetails listing)
		{
			if (listing == null || listing.SaleConditions == null)
				return null;
			
			if (!listing.SaleConditions.HasTransferableLoan)
				return PropertyViewSummaryHelperResources.SaleTransferableLoan_False;

			if (!listing.SaleConditions.TransferableLoanAmount.HasValue)
				return PropertyViewSummaryHelperResources.SaleTransferableLoan_True_WithoutValue;

			return string.Format(PropertyViewSummaryHelperResources.SaleTransferableLoan_True_WithValue, NumericStringUtils.ShortNumericString(listing.SaleConditions.TransferableLoanAmount.Value));
		}

		public static string RentFullMortgage(PropertyListingDetails listing)
		{
			if (listing == null || listing.RentConditions == null || !listing.RentConditions.Mortgage.HasValue)
				return null;

			if (!listing.RentConditions.Rent.HasValue || listing.RentConditions.Rent.Value != 0)
				return null;

			return string.Format(PropertyViewSummaryHelperResources.RentFullMortgage, NumericStringUtils.ShortNumericString(listing.RentConditions.Mortgage.Value));
		}

		public static string RentMortgage(PropertyListingDetails listing)
		{
			if (listing == null || listing.RentConditions == null || !listing.RentConditions.Mortgage.HasValue)
				return null;

			if (listing.RentConditions.Mortgage.Value == 0)
				return PropertyViewSummaryHelperResources.RentMortgage_Zero;

			return string.Format(PropertyViewSummaryHelperResources.RentMortgage, NumericStringUtils.ShortNumericString(listing.RentConditions.Mortgage.Value));
		}

		public static string RentMonthly(PropertyListingDetails listing)
		{
			if (listing == null || listing.RentConditions == null || !listing.RentConditions.Rent.HasValue)
				return null;

			return string.Format(PropertyViewSummaryHelperResources.RentMonthly, NumericStringUtils.ShortNumericString(listing.RentConditions.Rent.Value));
		}

        public static string MortgageAndRentConvertible(PropertyListingDetails listing)
		{
			if (listing == null || listing.RentConditions == null || !listing.RentConditions.MortgageAndRentConvertible)
				return null;

			return PropertyViewSummaryHelperResources.MortgageAndRentConvertible;
		}

	    public static string Location(PropertyListingDetails listing, IVicinityCache vicinityCache)
	    {
            if (listing == null)
                return null;
            if (listing.VicinityID.HasValue)
	        {
				return VicinityDisplayItem.ToString(VicinityPresentationHelper.BuildHierarchyString(vicinityCache, listing.VicinityID.Value));
	        }
	        return null;
	    }
        
        public static string AgencyListing(PropertyListingDetails listing)
        {
            if (listing == null)
                return null;

            return listing.IsAgencyListing ? PropertyViewSummaryHelperResources.AgencyListing : PropertyViewSummaryHelperResources.NoAgencyListing;
        } 
        
        public static string AgencyActivityAllowed(PropertyListingDetails listing)
        {
            if (listing == null)
                return null;

            if (listing.IsAgencyListing)
            {
                return listing.IsAgencyActivityAllowed ? PropertyViewSummaryHelperResources.AgencyListing_OtherAgencyPermitted : PropertyViewSummaryHelperResources.AgencyListing_OtherAgencyNotPermitted;    
            }
            else
            {
                return listing.IsAgencyActivityAllowed ? PropertyViewSummaryHelperResources.NoAgencyListing_AgencyPermitted : PropertyViewSummaryHelperResources.NoAgencyListing_AgencyNotPermitted;        
            }
        }
	}
}