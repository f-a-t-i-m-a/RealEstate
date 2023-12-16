using System.Data.Entity.SqlServer;
using System.Globalization;
using System.Linq;
using JahanJooy.Common.Util;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Domain.Property;
using JahanJooy.RealEstate.Domain.SavedSearch;

namespace JahanJooy.RealEstate.Core.Search
{
    public static class PropertySearchUtil
    {
        public static PropertySearch BuildPropertySearch(SavedPropertySearch savedSearch)
        {
            var result = PropertySearchQueryUtil.ParseQuery(savedSearch.AdditionalFilters) ?? new PropertySearch();

            result.PropertyType = savedSearch.PropertyType;

            return result;
        }

        public static bool MatchListing(PropertySearch search, PropertyListing l,
            bool applyUserAccountDependentFilters = true)
        {
            if (!MatchListingToSearchOptions(search, l, applyUserAccountDependentFilters))
                return false;

            return ApplySearchQueryForSearchProperties(search, new[] {l}.AsQueryable()).Any();
        }

        public static IQueryable<PropertyListing> ApplySearchQuery(PropertySearch search,
            IQueryable<PropertyListing> query)
        {
            query = ApplySearchQueryForSearchOptions(search, query);
            query = ApplySearchQueryForSearchProperties(search, query);

            return query;
        }

        public static IQueryable<PropertyListing> ApplySearchOrder(PropertySearch search,
            IQueryable<PropertyListing> query)
        {
            switch (search.SortOrder)
            {
                case null:
                case PropertySearchSortOrder.NewestFirst:
                    query = query.OrderByDescending(l => l.PublishDate);
                    break;

                case PropertySearchSortOrder.MostPopularFirst:
                    query = query.OrderByDescending(l => l.NumberOfVisits);
                    break;

                case PropertySearchSortOrder.CheapestFirst:
                    query = query.OrderBy(l => l.NormalizedPrice);
                    break;

                case PropertySearchSortOrder.MoreExpensiveFirst:
                    query = query.OrderByDescending(l => l.NormalizedPrice);
                    break;

                case PropertySearchSortOrder.LargestFirst:
                    query = query.OrderByDescending(l => l.Unit.Area ?? l.Estate.Area);
                    break;

                case PropertySearchSortOrder.SmallestFirst:
                    query = query.OrderBy(l => l.Unit.Area ?? l.Estate.Area);
                    break;
            }

            return query;
        }

        #region Private helper methods

        private static IQueryable<PropertyListing> ApplySearchQueryForSearchProperties(PropertySearch search,
            IQueryable<PropertyListing> query)
        {
            var hierarchyVicinityIds = search.VicinityIDs.IfNotNull(svids =>
                svids.Select(vid => "/" + vid.ToString(CultureInfo.InvariantCulture) + "/").ToList());

            if (search.PropertyType.HasValue)
                query = query.Where(l => l.PropertyType == search.PropertyType);

            if (search.IntentionOfOwner.HasValue)
                query = query.Where(l => l.IntentionOfOwner == search.IntentionOfOwner);

            if (hierarchyVicinityIds != null && hierarchyVicinityIds.Any())
                query =
                    query.Where(
                        l =>
                            l.VicinityID != null &&
                            hierarchyVicinityIds.Any(hvid => l.VicinityHierarchyString.Contains(hvid)));

	        if (search.GeographyBounds != null)
		        query = query.Where(
			        l => search.GeographyBounds.Intersects(l.GeographicLocation));

            if (search.EstateAreaMinimum.HasValue)
                query =
                    query.Where(
                        l =>
                            l.Estate != null && l.Estate.Area != null && l.Estate.Area >= search.EstateAreaMinimum.Value);
            if (search.EstateAreaMaximum.HasValue)
                query =
                    query.Where(
                        l =>
                            l.Estate != null && l.Estate.Area != null && l.Estate.Area <= search.EstateAreaMaximum.Value);

            if (search.NumberOfRoomsMinimum.HasValue)
                query =
                    query.Where(
                        l =>
                            l.Unit != null && l.Unit.NumberOfRooms != null &&
                            l.Unit.NumberOfRooms >= search.NumberOfRoomsMinimum.Value);
            if (search.NumberOfRoomsMaximum.HasValue)
                query =
                    query.Where(
                        l =>
                            l.Unit != null && l.Unit.NumberOfRooms != null &&
                            l.Unit.NumberOfRooms <= search.NumberOfRoomsMaximum.Value);

            if (search.NumberOfParkingsMinimum.HasValue)
                query =
                    query.Where(
                        l =>
                            l.Unit != null && l.Unit.NumberOfParkings != null &&
                            l.Unit.NumberOfParkings >= search.NumberOfParkingsMinimum.Value);

            if (search.UnitAreaMinimum.HasValue)
                query =
                    query.Where(
                        l => l.Unit != null && l.Unit.Area != null && l.Unit.Area >= search.UnitAreaMinimum.Value);
            if (search.UnitAreaMaximum.HasValue)
                query =
                    query.Where(
                        l => l.Unit != null && l.Unit.Area != null && l.Unit.Area <= search.UnitAreaMaximum.Value);

            if (search.SalePriceMinimum.HasValue)
                query =
                    query.Where(
                        l =>
                            l.SaleConditions != null && l.SaleConditions.Price != null &&
                            l.SaleConditions.Price >= search.SalePriceMinimum.Value);
            if (search.SalePriceMaximum.HasValue)
                query =
                    query.Where(
                        l =>
                            l.SaleConditions != null && l.SaleConditions.Price != null &&
                            l.SaleConditions.Price <= search.SalePriceMaximum.Value);

            if (search.SalePricePerEstateAreaMinimum.HasValue)
                query =
                    query.Where(
                        l =>
                            l.SaleConditions != null && l.SaleConditions.PricePerEstateArea != null &&
                            l.SaleConditions.PricePerEstateArea >= search.SalePricePerEstateAreaMinimum.Value);
            if (search.SalePricePerEstateAreaMaximum.HasValue)
                query =
                    query.Where(
                        l =>
                            l.SaleConditions != null && l.SaleConditions.PricePerEstateArea != null &&
                            l.SaleConditions.PricePerEstateArea <= search.SalePricePerEstateAreaMaximum.Value);

            if (search.SalePricePerUnitAreaMinimum.HasValue)
                query =
                    query.Where(
                        l =>
                            l.SaleConditions != null && l.SaleConditions.PricePerUnitArea != null &&
                            l.SaleConditions.PricePerUnitArea >= search.SalePricePerUnitAreaMinimum.Value);
            if (search.SalePricePerUnitAreaMaximum.HasValue)
                query =
                    query.Where(
                        l =>
                            l.SaleConditions != null && l.SaleConditions.PricePerUnitArea != null &&
                            l.SaleConditions.PricePerUnitArea <= search.SalePricePerUnitAreaMaximum.Value);

            // TODO: Filter based on SalePaymentForContractMaximum

            if (search.RentMortgageMinimum.HasValue)
                query =
                    query.Where(
                        l =>
                            l.RentConditions != null && l.RentConditions.Mortgage != null &&
                            l.RentConditions.Mortgage >= search.RentMortgageMinimum.Value);
            if (search.RentMortgageMaximum.HasValue)
                query =
                    query.Where(
                        l =>
                            l.RentConditions != null && l.RentConditions.Mortgage != null &&
                            l.RentConditions.Mortgage <= search.RentMortgageMaximum.Value);

            if (search.RentMinimum.HasValue)
                query =
                    query.Where(
                        l =>
                            l.RentConditions != null && l.RentConditions.Rent != null &&
                            l.RentConditions.Rent >= search.RentMinimum.Value);
            if (search.RentMaximum.HasValue)
                query =
                    query.Where(
                        l =>
                            l.RentConditions != null && l.RentConditions.Rent != null &&
                            l.RentConditions.Rent <= search.RentMaximum.Value);

            return query;
        }

        private static IQueryable<PropertyListing> ApplySearchQueryForSearchOptions(PropertySearch search,
            IQueryable<PropertyListing> query)
        {
            // ReSharper disable LoopCanBeConvertedToQuery
            if (search.Options != null)
                foreach (var optionKey in search.Options)
                    if (PropertySearchOptions.AllOptions[optionKey] != null &&
                        PropertySearchOptions.AllOptions[optionKey].Expression != null)
                        query = query.Where(PropertySearchOptions.AllOptions[optionKey].Expression);
            // ReSharper restore LoopCanBeConvertedToQuery

            return query;
        }

        private static bool MatchListingToSearchOptions(PropertySearch search, PropertyListing listing,
            bool applyUserAccountDependentFilters)
        {
            if (search.Options != null)
            {
                foreach (var optionKey in search.Options)
                {
                    if (PropertySearchOptions.AllOptions[optionKey] != null &&
                        PropertySearchOptions.AllOptions[optionKey].Delegate != null)
                    {
                        if (!applyUserAccountDependentFilters &&
                            PropertySearchOptions.AllOptions[optionKey].DependsOnUserAccount)
                            continue;

                        if (!PropertySearchOptions.AllOptions[optionKey].Delegate(listing))
                            return false;
                    }
                }
            }

            return true;
        }

        #endregion
    }
}