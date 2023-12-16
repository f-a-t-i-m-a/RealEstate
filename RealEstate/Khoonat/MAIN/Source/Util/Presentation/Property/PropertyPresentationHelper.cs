using System;
using System.Globalization;
using System.Web;
using System.Web.Security.AntiXss;
using Compositional.Composer;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.Localization;
using JahanJooy.Common.Util.Text;
using JahanJooy.Common.Util.Web.Extensions;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Domain.Directory;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Domain.Property;
using JahanJooy.RealEstate.Util.Resources;

namespace JahanJooy.RealEstate.Util.Presentation.Property
{
    [Contract]
    [Component]
    public class PropertyPresentationHelper
    {
        private const string BoldStart = "<strong>";
        private const string BoldEnd = "</strong>";

        [ComponentPlug]
        public IVicinityCache VicinityCache { get; set; }

        [ComponentPlug]
        public VicinityPresentationHelper VicinityPresentationHelper { get; set; }

        #region Public methods

        public string BuildPageTitleString(PropertyListingSummary listing)
        {
            string result =
                PropertyPresentationHelperResources.PageTitleRecordNumber.FormatIfNotEmpty(
                    listing.Code.ToString(CultureInfo.InvariantCulture)) + ": " +
                listing.IntentionOfOwner.Label(DomainEnumResources.ResourceManager) + " " +
                listing.PropertyType.Label(DomainEnumResources.ResourceManager) + " ";

            switch (listing.PropertyType)
            {
                case PropertyType.Land:
                    result += BuildEstateTitle(listing);
                    break;

                case PropertyType.Garden:
                    result += BuildGardenTitle(listing);
                    break;

                case PropertyType.House:
                    result += StringUtils.JoinNonEmpty(" ", BuildHouseTitleDirection(listing),
                        BuildHouseTitleUnitsAndFloors(listing));
                    break;

                case PropertyType.Villa:
                    result += BuildVillaTitle(listing);
                    break;

                case PropertyType.Apartment:
                    result += StringUtils.JoinNonEmpty(" ", BuildApartmentTitleUsageType(listing),
                        BuildNumberOfRooms(listing));
                    break;

                case PropertyType.Penthouse:
                    result += StringUtils.JoinNonEmpty(" ", BuildPenthouseTitleFloorNumber(listing),
                        BuildNumberOfRooms(listing));
                    break;

                case PropertyType.Shop:
                    result += BuildShopTitle(listing);
                    break;
            }

            if (listing.VicinityID.HasValue)
                result += " " +
                          VicinityDisplayItem.ToString(
							  VicinityPresentationHelper.BuildHierarchyString(listing.VicinityID.Value,
                                  useWellknownScope: true, summary: true));

            return result;
        }

        public HtmlString BuildPageDescription(PropertyListingSummary summary, PropertyListingDetails details)
        {
            var result = "";

            if (details.ContactInfo != null && !string.IsNullOrWhiteSpace(details.ContactInfo.AgencyName))
                result += AntiXssEncoder.HtmlEncode(details.ContactInfo.AgencyName, false) + " - ";
            else if (details.ContactInfo != null && !string.IsNullOrWhiteSpace(details.ContactInfo.ContactName))
                result += AntiXssEncoder.HtmlEncode(details.ContactInfo.ContactName, false) + " - ";

            result += string.Format(PropertyPresentationHelperResources.PageDescriptionTitleDetails,
                BuildPageTitleString(summary));

            if (details.Unit != null && !string.IsNullOrWhiteSpace(details.Unit.AdditionalSpecialFeatures))
            {
                result += " - " + AntiXssEncoder.HtmlEncode(details.Unit.AdditionalSpecialFeatures, false);
            }
            else
            {
                result += " - " + BuildAreaString(summary);

                switch (summary.IntentionOfOwner)
                {
                    case IntentionOfOwner.ForSale:
                        result += " - " + BuildPriceAndPricePerAreaStringForSale(summary);
                        break;

                    case IntentionOfOwner.ForRent:
                        result += " - " + BuildPriceStringForRent(summary);
                        break;
                }
            }

            return new HtmlString(result);
        }

        public HtmlString BuildSingleLineSummary(PropertyListingSummary listing)
        {
            return new HtmlString(string.Format(PropertyPresentationHelperResources.SingleLineSummaryFormat,
                listing.Code.ToString(CultureInfo.InvariantCulture),
                BuildRegion(listing).ToHtmlString(),
                BuildTitle(listing).ToHtmlString(),
                BuildArea(listing).ToHtmlString(),
                BuildPriceAndPricePerArea(listing).ToHtmlString()));
        }

        public HtmlString BuildRegion(PropertyListingSummary listing)
        {
            return BuildRegion(listing.VicinityID);
        }

        public HtmlString BuildRegion(PropertyListing listing)
        {
            return BuildRegion(listing.VicinityID);
        }

        public HtmlString BuildRegion(long? vicinityId)
        {
            string vicinityName = vicinityId.IfHasValue(vid => VicinityCache[vid])
               .IfNotNull(v => v.Name);

            if (string.IsNullOrWhiteSpace(vicinityName))
                return HtmlPresentationHelper.Disabled((PropertyPresentationHelperResources.NotSpecified));

           string result = "";
            if (vicinityId!= null){
				result += VicinityDisplayItem.ToString(VicinityPresentationHelper.BuildHierarchyString(vicinityId.Value, false, false, true, true));
            }
            return new HtmlString(result);
        }

        public string BuildUserEnteredAddress(PropertyListingDetails listing)
        {
	        return StringUtils.JoinNonEmpty(PropertyPresentationHelperResources.Separator,
		        listing.Estate.IfNotNull(e => e.Address),
		        PropertyPresentationHelperResources.UserEnteredAddressFloorNo.FormatIfNotEmpty(
			        listing.Unit.IfNotNull(
				        u => u.FloorNumber.IfHasValue(fn => fn.ToString(CultureInfo.InvariantCulture))))
		        );
        }

        public string BuildRegionString(PropertyListingSummary listing)
        {
            string vicinityName = listing.VicinityID.IfHasValue(vid => VicinityCache[vid]).IfNotNull(v => v.Name);

            if (string.IsNullOrWhiteSpace(vicinityName))
                return PropertyPresentationHelperResources.NotSpecified;
            string result = "";
            if (listing.VicinityID.HasValue)
                result =
					VicinityDisplayItem.ToString(VicinityPresentationHelper.BuildHierarchyString(
                        listing.VicinityID.Value, false,
                        false, true, true));
            return result;
        }

        public string BuildShortRegion(PropertyListingSummary listing)
        {
            string vicinityName = listing.VicinityID.IfHasValue(vid => VicinityCache[vid].IfNotNull(v => v.Name));
            return vicinityName ?? PropertyPresentationHelperResources.NotSpecified;
        }

        public HtmlString BuildTitle(PropertyListingSummary listing)
        {
            string result = BoldStart +
                            HtmlPresentationHelper.NoBreaking(
                                listing.PropertyType.Label(DomainEnumResources.ResourceManager)).ToHtmlString() +
                            BoldEnd + " ";

            switch (listing.PropertyType)
            {
                case PropertyType.Land:
                    result += HtmlPresentationHelper.NoBreaking(BuildEstateTitle(listing)).ToHtmlString();
                    break;

                case PropertyType.Garden:
                    result += HtmlPresentationHelper.NoBreaking(BuildGardenTitle(listing)).ToHtmlString();
                    break;

                case PropertyType.House:
                    result += BuildHouseTitle(listing).ToHtmlString();
                    break;

                case PropertyType.Villa:
                    result += HtmlPresentationHelper.NoBreaking(BuildVillaTitle(listing)).ToHtmlString();
                    break;

                case PropertyType.Apartment:
                    result += BuildApartmentTitle(listing).ToHtmlString();
                    break;

                case PropertyType.Penthouse:
                    result += BuildPenthouseTitle(listing).ToHtmlString();
                    break;

                case PropertyType.Shop:
                    result += HtmlPresentationHelper.NoBreaking(BuildShopTitle(listing)).ToHtmlString();
                    break;
                default:
                    return HtmlPresentationHelper.Disabled(PropertyPresentationHelperResources.NotSpecified);
            }

            return new HtmlString(result);
        }

        public string BuildShortTitle(PropertyListingSummary listing)
        {
            string result = listing.PropertyType.Label(DomainEnumResources.ResourceManager) + " ";

            switch (listing.PropertyType)
            {
                case PropertyType.Land:
                    result += BuildEstateTitle(listing);
                    break;

                case PropertyType.Garden:
                    result += BuildGardenTitle(listing);
                    break;

                case PropertyType.House:
                    result += BuildHouseShortTitle(listing);
                    break;

                case PropertyType.Villa:
                    result += BuildVillaTitle(listing);
                    break;

                case PropertyType.Apartment:
                    result += BuildApartmentShortTitle(listing);
                    break;

                case PropertyType.Penthouse:
                    result += BuildPenthouseShortTitle(listing);
                    break;

                case PropertyType.Shop:
                    result += BuildShopTitle(listing);
                    break;

                default:
                    return PropertyPresentationHelperResources.NotSpecified;
            }

            return result;
        }

        public HtmlString BuildArea(PropertyListingSummary listing)
        {
            string unitArea = null;
            string estateArea = null;

            if (listing.PropertyType.IsEstateSignificant())
                estateArea =
                    listing.EstateArea.IfHasValue(a => string.Format(PropertyPresentationHelperResources.AreaEstate, a));

            if (listing.PropertyType.HasUnit())
                unitArea =
                    listing.UnitArea.IfHasValue(a => string.Format(PropertyPresentationHelperResources.AreaUnit, a));

            if (unitArea != null && estateArea != null)
                return
                    new HtmlString(
                        HtmlPresentationHelper.NoBreaking(unitArea + PropertyPresentationHelperResources.AreaConnector)
                            .ToHtmlString() + " " +
                        HtmlPresentationHelper.NoBreaking(estateArea).ToHtmlString());

            if (unitArea != null)
                return HtmlPresentationHelper.NoBreaking(unitArea);

            if (estateArea != null)
                return HtmlPresentationHelper.NoBreaking(estateArea);

            return HtmlPresentationHelper.Disabled(PropertyPresentationHelperResources.NotSpecified);
        }

        public string BuildAreaString(PropertyListingSummary listing)
        {
            string unitArea = null;
            string estateArea = null;

            if (listing.PropertyType.IsEstateSignificant())
                estateArea =
                    listing.EstateArea.IfHasValue(a => string.Format(PropertyPresentationHelperResources.AreaEstate, a));

            if (listing.PropertyType.HasUnit())
                unitArea =
                    listing.UnitArea.IfHasValue(a => string.Format(PropertyPresentationHelperResources.AreaUnit, a));

            if (unitArea != null && estateArea != null)
                return unitArea + PropertyPresentationHelperResources.AreaConnector + " " + estateArea;

            return unitArea ?? estateArea ?? PropertyPresentationHelperResources.NotSpecified;
        }

        public HtmlString BuildPriceAndPricePerArea(PropertyListingSummary listing)
        {
            switch (listing.IntentionOfOwner)
            {
                case IntentionOfOwner.ForSale:
                    return BuildPriceAndPricePerAreaForSale(listing);

                case IntentionOfOwner.ForRent:
                    return BuildPriceForRent(listing);
            }

            return HtmlPresentationHelper.Disabled(PropertyPresentationHelperResources.NotSpecified);
        }

        public string BuildPriceString(PropertyListingSummary listing)
        {
            switch (listing.IntentionOfOwner)
            {
                case IntentionOfOwner.ForSale:
                    return BuildPriceStringForSale(listing);

                case IntentionOfOwner.ForRent:
                    return BuildPriceStringForRent(listing);
            }

            return PropertyPresentationHelperResources.NotSpecified;
        }

        public string BuildPricePerAreaString(PropertyListingSummary listing)
        {
            if (listing.IntentionOfOwner == IntentionOfOwner.ForSale)
                return BuildPricePerAreaStringForSale(listing);

            return null;
        }

        public static HtmlString BuildStatus(PropertyListingSummary listing)
        {
            if (listing.PublishEndDate == null)
                return HtmlPresentationHelper.Disabled(PropertyPresentationHelperResources.NotPublished);

            if (listing.PublishEndDate < DateTime.Now)
                return
                    new HtmlString(AntiXssEncoder.HtmlEncode(PropertyPresentationHelperResources.PublishExpired, false));

            return new HtmlString(AntiXssEncoder.HtmlEncode(listing.PublishEndDate.ToLocalizedDateString(), false));
        }

        public static string BuildNumberOfRooms(PropertyListingSummary listing)
        {
            return listing.NumberOfRooms.IfHasValue(
                nor => string.Format(
                    listing.UsageType.HasValue && listing.UsageType.Value == UnitUsageType.Residency
                        ? PropertyPresentationHelperResources.NumberOfRoomsResidency
                        : PropertyPresentationHelperResources.NumberOfRooms,
                    NumericStringUtils.FullyTextualNumber(nor)));
        }

        public HtmlString IsMainBranch(AgencyBranch agencyBranch)
        {
            return agencyBranch.IsMainBranch
                ? new HtmlString(PropertyPresentationHelperResources.Yes)
                : new HtmlString(PropertyPresentationHelperResources.No);
        }

        #endregion

        #region Private helper methods

        private static string BuildEstateTitle(PropertyListingSummary listing)
        {
            if (listing.EstateDirection.HasValue && listing.EstateDirection.Value == EstateDirection.Other)
                return string.Empty;

            string direction = listing.EstateDirection.IfHasValue(d => d.Label(DomainEnumResources.ResourceManager),
                string.Empty);
            return string.Format(PropertyPresentationHelperResources.TitleEstate, direction);
        }

        private static string BuildGardenTitle(PropertyListingSummary listing)
        {
            string surface = listing.EstateSurfaceType.IfHasValue(d => d.Label(DomainEnumResources.ResourceManager),
                string.Empty);
            return string.Format(PropertyPresentationHelperResources.TitleGarden, surface);
        }

        private static HtmlString BuildHouseTitle(PropertyListingSummary listing)
        {
            string titlePartOne = BuildHouseTitleDirection(listing);
            string titlePartTwo = BuildHouseTitleUnitsAndFloors(listing);

            if (string.IsNullOrWhiteSpace(titlePartTwo))
                return HtmlPresentationHelper.NoBreaking(titlePartOne);

            return
                new HtmlString(HtmlPresentationHelper.NoBreaking(titlePartOne).ToHtmlString() + " " +
                               HtmlPresentationHelper.NoBreaking(titlePartTwo).ToHtmlString());
        }

        private static string BuildShopTitle(PropertyListingSummary listing)
        {
            string estateArea =
                listing.EstateArea.IfHasValue(a => string.Format(PropertyPresentationHelperResources.AreaEstate, a));
            return string.Format(PropertyPresentationHelperResources.TitleShop, estateArea);
        }

        private static string BuildHouseShortTitle(PropertyListingSummary listing)
        {
            return BuildHouseTitleUnitsAndFloors(listing);
        }

        private static string BuildHouseTitleDirection(PropertyListingSummary listing)
        {
            string direction = listing.EstateDirection.IfHasValue(d => d.Label(DomainEnumResources.ResourceManager),
                string.Empty);
            return string.Format(PropertyPresentationHelperResources.TitleHouseDirection, direction);
        }

        private static string BuildHouseTitleUnitsAndFloors(PropertyListingSummary listing)
        {
            string titlePartTwoFloors =
                listing.TotalNumberOfFloorsInBuilding.IfHasValue(
                    fl => string.Format(PropertyPresentationHelperResources.TitleHouseFloors, fl));
            string titlePartTwoUnits =
                listing.TotalNumberOfUnitsInBuilding.IfHasValue(
                    un => string.Format(PropertyPresentationHelperResources.TitleHouseUnits, un));
            return StringUtils.JoinNonEmpty(PropertyPresentationHelperResources.Separator, titlePartTwoFloors,
                titlePartTwoUnits);
        }

        private static string BuildVillaTitle(PropertyListingSummary listing)
        {
            if (listing.TotalNumberOfFloorsInBuilding.HasValue)
                return string.Format(PropertyPresentationHelperResources.TitleVilla,
                    listing.TotalNumberOfFloorsInBuilding.Value);

            return string.Empty;
        }

        private static HtmlString BuildApartmentTitle(PropertyListingSummary listing)
        {
            string titlePartOne = BuildApartmentTitleUsageType(listing);
            string titlePartTwo = BuildNumberOfRooms(listing);

            if (titlePartTwo != null)
                return
                    new HtmlString(HtmlPresentationHelper.NoBreaking(titlePartOne).ToHtmlString() + " " +
                                   HtmlPresentationHelper.NoBreaking(titlePartTwo).ToHtmlString());

            return HtmlPresentationHelper.NoBreaking(titlePartOne);
        }

        private static string BuildApartmentShortTitle(PropertyListingSummary listing)
        {
            return BuildApartmentTitleUsageType(listing);
        }

        private static string BuildApartmentTitleUsageType(PropertyListingSummary listing)
        {
            string usageType = listing.UsageType.IfHasValue(ut => ut.Label(DomainEnumResources.ResourceManager),
                string.Empty);
            return string.Format(PropertyPresentationHelperResources.TitleApartment, usageType);
        }

        private static HtmlString BuildPenthouseTitle(PropertyListingSummary listing)
        {
            string titlePartOne = BuildPenthouseTitleFloorNumber(listing);
            string titlePartTwo = BuildNumberOfRooms(listing);

            if (titlePartTwo != null)
                return
                    new HtmlString(
                        HtmlPresentationHelper.NoBreaking(titlePartOne + PropertyPresentationHelperResources.Separator)
                            .ToHtmlString() + " " +
                        HtmlPresentationHelper.NoBreaking(titlePartTwo).ToHtmlString());

            return HtmlPresentationHelper.NoBreaking(titlePartOne);
        }

        private static string BuildPenthouseShortTitle(PropertyListingSummary listing)
        {
            return BuildPenthouseTitleFloorNumber(listing);
        }

        private static string BuildPenthouseTitleFloorNumber(PropertyListingSummary listing)
        {
            return listing.FloorNumber.HasValue
                ? string.Format(PropertyPresentationHelperResources.TitlePenthouseFloorNumber, listing.FloorNumber.Value)
                : string.Empty;
        }

        private static HtmlString BuildPriceAndPricePerAreaForSale(PropertyListingSummary listing)
        {
            string result = BoldStart +
                            HtmlPresentationHelper.NoBreaking(PropertyPresentationHelperResources.SaleStart)
                                .ToHtmlString() + BoldEnd;

            result += listing.Price.HasValue
                ? HtmlPresentationHelper.NoBreaking(string.Format(
                    PropertyPresentationHelperResources.SaleSpecifiedPrice,
                    NumericStringUtils.ShortNumericString(listing.Price)))
                : HtmlPresentationHelper.Disabled(
                    HtmlPresentationHelper.NoBreaking(PropertyPresentationHelperResources.SaleUnspecifiedPrice));

            result += PropertyPresentationHelperResources.SaleSeparator;
            result += BuildPricePerAreaForSale(listing).ToHtmlString();

            if (listing.HasTransferableLoan.HasValue && listing.HasTransferableLoan.Value)
                result += BoldStart + PropertyPresentationHelperResources.HasTransferableLoan + BoldEnd;

            return new HtmlString(result);
        }

        private static HtmlString BuildPricePerAreaForSale(PropertyListingSummary listing)
        {
            if (listing.PropertyType.IsEstateSignificant())
                return listing.PricePerEstateArea.HasValue
                    ? HtmlPresentationHelper.NoBreaking(
                        string.Format(PropertyPresentationHelperResources.SalePricePerEstateArea,
                            NumericStringUtils.ShortNumericString(listing.PricePerEstateArea)))
                    : HtmlPresentationHelper.Disabled(
                        HtmlPresentationHelper.NoBreaking(
                            PropertyPresentationHelperResources.SalePricePerEstateAreaUnspecified));

            return listing.PricePerUnitArea.HasValue
                ? HtmlPresentationHelper.NoBreaking(string.Format(
                    PropertyPresentationHelperResources.SalePricePerUnitArea,
                    NumericStringUtils.ShortNumericString(listing.PricePerUnitArea)))
                : HtmlPresentationHelper.Disabled(
                    HtmlPresentationHelper.NoBreaking(
                        PropertyPresentationHelperResources.SalePricePerUnitAreaUnspecified));
        }

        private static string BuildPriceAndPricePerAreaStringForSale(PropertyListingSummary listing)
        {
            string result = "";

            if (listing.Price.HasValue)
                result +=
                    string.Format(PropertyPresentationHelperResources.SaleSpecifiedPrice,
                        NumericStringUtils.ShortNumericString(listing.Price)) +
                    PropertyPresentationHelperResources.SaleSeparator;

            result += BuildPricePerAreaStringForSale(listing);

            if (listing.HasTransferableLoan.HasValue && listing.HasTransferableLoan.Value)
                result += PropertyPresentationHelperResources.HasTransferableLoan;

            return result;
        }

        private static string BuildPriceStringForSale(PropertyListingSummary listing)
        {
            string result = "";

            if (listing.Price.HasValue)
                result += string.Format(PropertyPresentationHelperResources.SaleSpecifiedPrice,
                    NumericStringUtils.ShortNumericString(listing.Price));

            if (listing.HasTransferableLoan.HasValue && listing.HasTransferableLoan.Value)
                result += PropertyPresentationHelperResources.SaleSeparator +
                          PropertyPresentationHelperResources.HasTransferableLoan;

            return result;
        }

        private static string BuildPricePerAreaStringForSale(PropertyListingSummary listing)
        {
            if (listing.PropertyType.IsEstateSignificant() && listing.PricePerEstateArea.HasValue)
                return string.Format(PropertyPresentationHelperResources.SalePricePerEstateArea,
                    NumericStringUtils.ShortNumericString(listing.PricePerEstateArea));

            if (listing.PricePerUnitArea.HasValue)
                return string.Format(PropertyPresentationHelperResources.SalePricePerUnitArea,
                    NumericStringUtils.ShortNumericString(listing.PricePerUnitArea));

            return string.Empty;
        }

        private static HtmlString BuildPriceForRent(PropertyListingSummary listing)
        {
            string result;

            if (listing.Mortgage.HasValue && listing.Rent.HasValue && listing.Rent.Value == 0)
            {
                result = BoldStart +
                         HtmlPresentationHelper.NoBreaking(PropertyPresentationHelperResources.RentStart).ToHtmlString() +
                         BoldEnd +
                         HtmlPresentationHelper.NoBreaking(
                             string.Format(PropertyPresentationHelperResources.RentFullMortgagePartOne,
                                 NumericStringUtils.ShortNumericString(listing.Mortgage))).ToHtmlString() + " " +
                         HtmlPresentationHelper.NoBreaking(PropertyPresentationHelperResources.RentFullMortgagePartTwo)
                             .ToHtmlString();
            }
            else
            {
                result = BoldStart +
                         HtmlPresentationHelper.NoBreaking(PropertyPresentationHelperResources.RentStart).ToHtmlString() +
                         BoldEnd;

                if (!listing.Mortgage.HasValue)
                {
                    result +=
                        HtmlPresentationHelper.Disabled(
                            HtmlPresentationHelper.NoBreaking(
                                PropertyPresentationHelperResources.RentUnspecifiedMortgage)).ToHtmlString();
                    result += PropertyPresentationHelperResources.RentSeparator;
                }
                else if (listing.Mortgage.Value > 0)
                {
                    result +=
                        HtmlPresentationHelper.NoBreaking(
                            string.Format(PropertyPresentationHelperResources.RentSpecifiedMortgage,
                                NumericStringUtils.ShortNumericString(listing.Mortgage))).ToHtmlString();
                    result += PropertyPresentationHelperResources.RentSeparator;
                }

                if (listing.Rent.HasValue)
                    result +=
                        HtmlPresentationHelper.NoBreaking(
                            string.Format(PropertyPresentationHelperResources.RentSpecifiedRent,
                                NumericStringUtils.ShortNumericString(listing.Rent))).ToHtmlString();
                else
                    result +=
                        HtmlPresentationHelper.Disabled(
                            HtmlPresentationHelper.NoBreaking(PropertyPresentationHelperResources.RentUnspecifiedRent))
                            .ToHtmlString();
            }

            if (listing.MortgageAndRentConvertible.HasValue && listing.MortgageAndRentConvertible.Value)
            {
                result += BoldStart + PropertyPresentationHelperResources.MortgageAndRentConvertible + BoldEnd;
            }

            return new HtmlString(result);
        }

        private static string BuildPriceStringForRent(PropertyListingSummary listing)
        {
            string result = "";

            if (listing.Mortgage.HasValue && listing.Rent.HasValue && listing.Rent.Value == 0)
            {
                result =
                    string.Format(PropertyPresentationHelperResources.RentFullMortgagePartOne,
                        NumericStringUtils.ShortNumericString(listing.Mortgage)) + " " +
                    PropertyPresentationHelperResources.RentFullMortgagePartTwo;
            }
            else
            {
                if (!listing.Mortgage.HasValue)
                {
                    result += PropertyPresentationHelperResources.RentUnspecifiedMortgage;
                    result += PropertyPresentationHelperResources.RentSeparator;
                }
                else if (listing.Mortgage.Value > 0)
                {
                    result += string.Format(PropertyPresentationHelperResources.RentSpecifiedMortgage,
                        NumericStringUtils.ShortNumericString(listing.Mortgage));
                    result += PropertyPresentationHelperResources.RentSeparator;
                }

                if (listing.Rent.HasValue)
                    result += string.Format(PropertyPresentationHelperResources.RentSpecifiedRent,
                        NumericStringUtils.ShortNumericString(listing.Rent));
                else
                    result += PropertyPresentationHelperResources.RentUnspecifiedRent;
            }

            if (listing.MortgageAndRentConvertible.HasValue && listing.MortgageAndRentConvertible.Value)
            {
                result += PropertyPresentationHelperResources.MortgageAndRentConvertible;
            }

            return result;
        }

        #endregion
    }
}