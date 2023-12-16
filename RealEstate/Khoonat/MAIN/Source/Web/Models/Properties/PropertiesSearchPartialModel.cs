using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using JahanJooy.Common.Util.Collections;
using JahanJooy.Common.Util.General;
using JahanJooy.Common.Util.Web.Attributes;
using JahanJooy.RealEstate.Core.Search;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Web.Resources.Models.Properties;

namespace JahanJooy.RealEstate.Web.Models.Properties
{
	public class PropertiesSearchPartialModel
	{
		//
		// Hidden fields

		public string SearchQuery { get; set; }

		//
		// Type 

		[Display(ResourceType = typeof(PropertiesSearchResources), Name = "Label_PropertyType")]
		public PropertyType? PropertyType { get; set; }

		[Display(ResourceType = typeof(PropertiesSearchResources), Name = "Label_IntentionOfOwner")]
		public IntentionOfOwner? IntentionOfOwner { get; set; }

		//
		// Location

        public string VicinityIDsCsv
        {
            get { return CsvUtils.ToCsvString(VicinityIDs); }
            set { VicinityIDs = CsvUtils.ParseInt64Enumerable(value).ToArray(); }
        }
        [Display(ResourceType = typeof(PropertiesSearchResources), Name = "Label_VicinityIDs")]
        public long[] VicinityIDs { get; set; }

		//
		// Ranges

		[Numeric(ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Area_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Area_Numeric")]
		[Range(1, 999999, ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Area_Range")]
		[Display(ResourceType = typeof(PropertiesSearchResources), Name = "Label_UnitAreaMinimum")]
		public decimal? UnitAreaMinimum { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Area_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Area_Numeric")]
		[Range(1, 999999, ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Area_Range")]
		[Display(ResourceType = typeof(PropertiesSearchResources), Name = "Label_UnitAreaMaximum")]
		public decimal? UnitAreaMaximum { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Area_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Area_Numeric")]
		[Range(1, 999999, ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Area_Range")]
		[Display(ResourceType = typeof(PropertiesSearchResources), Name = "Label_EstateAreaMinimum")]
		public decimal? EstateAreaMinimum { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Area_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Area_Numeric")]
		[Range(1, 999999, ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Area_Range")]
		[Display(ResourceType = typeof(PropertiesSearchResources), Name = "Label_EstateAreaMaximum")]
		public decimal? EstateAreaMaximum { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Rooms_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Rooms_Numeric")]
		[Range(1, 999999, ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Rooms_Range")]
		[Display(ResourceType = typeof(PropertiesSearchResources), Name = "Label_NumberOfRoomsMinimum")]
		public int? NumberOfRoomsMinimum { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Rooms_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Rooms_Numeric")]
		[Range(1, 999999, ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Rooms_Range")]
		[Display(ResourceType = typeof(PropertiesSearchResources), Name = "Label_NumberOfRoomsMaximum")]
		public int? NumberOfRoomsMaximum { get; set; }

		//
		// Options

		[Display(ResourceType = typeof(PropertiesSearchResources), Name = "Label_ShouldHaveParking")]
		public bool ShouldHaveParking { get; set; }

		[Display(ResourceType = typeof(PropertiesSearchResources), Name = "Label_ShouldHaveElevator")]
		public bool ShouldHaveElevator { get; set; }

		[Display(ResourceType = typeof(PropertiesSearchResources), Name = "Label_ShouldHaveStorageRoom")]
		public bool ShouldHaveStorageRoom { get; set; }

		[Display(ResourceType = typeof(PropertiesSearchResources), Name = "Label_ShouldHavePhotos")]
		public bool ShouldHavePhotos { get; set; }

		//
		// Sale price

		[Numeric(ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[Range(1, 999999999999, ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Price_Range")]
		[Display(ResourceType = typeof(PropertiesSearchResources), Name = "Label_SalePriceMinimum")]
		public decimal? SalePriceMinimum { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[Range(1, 999999999999, ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Price_Range")]
		[Display(ResourceType = typeof(PropertiesSearchResources), Name = "Label_SalePriceMaximum")]
		public decimal? SalePriceMaximum { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[Range(1, 999999999999, ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Price_Range")]
		[Display(ResourceType = typeof(PropertiesSearchResources), Name = "Label_SalePricePerEstateAreaMinimum")]
		public decimal? SalePricePerEstateAreaMinimum { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[Range(1, 999999999999, ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Price_Range")]
		[Display(ResourceType = typeof(PropertiesSearchResources), Name = "Label_SalePricePerEstateAreaMaximum")]
		public decimal? SalePricePerEstateAreaMaximum { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[Range(1, 999999999999, ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Price_Range")]
		[Display(ResourceType = typeof(PropertiesSearchResources), Name = "Label_SalePricePerUnitAreaMinimum")]
		public decimal? SalePricePerUnitAreaMinimum { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[Range(1, 999999999999, ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Price_Range")]
		[Display(ResourceType = typeof(PropertiesSearchResources), Name = "Label_SalePricePerUnitAreaMaximum")]
		public decimal? SalePricePerUnitAreaMaximum { get; set; }

		//
		// Rent price

		[Numeric(ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[Range(1, 999999999999, ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Price_Range")]
		[Display(ResourceType = typeof(PropertiesSearchResources), Name = "Label_RentMortgageMinimum")]
		public decimal? RentMortgageMinimum { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[Range(1, 999999999999, ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Price_Range")]
		[Display(ResourceType = typeof(PropertiesSearchResources), Name = "Label_RentMortgageMaximum")]
		public decimal? RentMortgageMaximum { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[Range(1, 999999999999, ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Price_Range")]
		[Display(ResourceType = typeof(PropertiesSearchResources), Name = "Label_RentMinimum")]
		public decimal? RentMinimum { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Price_Numeric")]
		[Range(1, 999999999999, ErrorMessageResourceType = typeof(PropertiesSearchResources), ErrorMessageResourceName = "Validation_Price_Range")]
		[Display(ResourceType = typeof(PropertiesSearchResources), Name = "Label_RentMaximum")]
		public decimal? RentMaximum { get; set; }


        //
        // Agency Listing

        [Display(ResourceType = typeof(PropertiesSearchResources), Name = "Label_AgencyListing")]
        public PropertyAgencyListingSearchType? AgencyListing { get; set; }

		//
		// Conversion methods

		private void UpdatePropertySearch(PropertySearch search)
		{
			if (search == null)
				throw new ArgumentNullException("search");

			search.PropertyType = PropertyType;
			search.IntentionOfOwner = IntentionOfOwner;

			search.VicinityIDs = VicinityIDs;

			search.UnitAreaMinimum = UnitAreaMinimum;
			search.UnitAreaMaximum = UnitAreaMaximum;
			search.EstateAreaMinimum = EstateAreaMinimum;
			search.EstateAreaMaximum = EstateAreaMaximum;
			search.NumberOfRoomsMinimum = NumberOfRoomsMinimum;
			search.NumberOfRoomsMaximum = NumberOfRoomsMaximum;

			search.SalePriceMinimum = SalePriceMinimum;
			search.SalePriceMaximum = SalePriceMaximum;
			search.SalePricePerEstateAreaMinimum = SalePricePerEstateAreaMinimum;
			search.SalePricePerEstateAreaMaximum = SalePricePerEstateAreaMaximum;
			search.SalePricePerUnitAreaMinimum = SalePricePerUnitAreaMinimum;
			search.SalePricePerUnitAreaMaximum = SalePricePerUnitAreaMaximum;

			search.RentMortgageMinimum = RentMortgageMinimum;
			search.RentMortgageMaximum = RentMortgageMaximum;
			search.RentMinimum = RentMinimum;
			search.RentMaximum = RentMaximum;

            if (search.Options == null)
                search.Options = new List<string>();

            search.Options.SetItemExistance(PropertySearchOptions.OptionHasParking, ShouldHaveParking);
            search.Options.SetItemExistance(PropertySearchOptions.OptionHasElevator, ShouldHaveElevator);
            search.Options.SetItemExistance(PropertySearchOptions.OptionHasStorageRoom, ShouldHaveStorageRoom);
            search.Options.SetItemExistance(PropertySearchOptions.OptionHasPhotos, ShouldHavePhotos);

            search.Options.SetItemExistance(PropertySearchOptions.OptionNoAgencyListing, AgencyListing == PropertyAgencyListingSearchType.NoAgencyListing ||
		                                                                                 AgencyListing == PropertyAgencyListingSearchType.NoAgencyListingWithAgencyActivityAllowed);
		    search.Options.SetItemExistance(PropertySearchOptions.OptionAgencyActivityAllowed, AgencyListing == PropertyAgencyListingSearchType.AgencyActivityAllowed ||
		                                                                                       AgencyListing == PropertyAgencyListingSearchType.NoAgencyListingWithAgencyActivityAllowed);
		}

		public PropertySearch ToPropertySearch()
		{
			var result = PropertySearchQueryUtil.ParseQuery(SearchQuery) ?? new PropertySearch();
			UpdatePropertySearch(result);

			return result;
		}

		public static PropertiesSearchPartialModel FromPropertySearch(PropertySearch search)
		{
			var result = new PropertiesSearchPartialModel();
			if (search == null)
				return result;

			result.SearchQuery = PropertySearchQueryUtil.GenerateQuery(search);

			result.PropertyType = search.PropertyType;
			result.IntentionOfOwner = search.IntentionOfOwner;

			result.VicinityIDs = search.VicinityIDs;

			result.UnitAreaMinimum = search.UnitAreaMinimum;
			result.UnitAreaMaximum = search.UnitAreaMaximum;
			result.EstateAreaMinimum = search.EstateAreaMinimum;
			result.EstateAreaMaximum = search.EstateAreaMaximum;
			result.NumberOfRoomsMinimum = search.NumberOfRoomsMinimum;
			result.NumberOfRoomsMaximum = search.NumberOfRoomsMaximum;

			result.SalePriceMinimum = search.SalePriceMinimum;
			result.SalePriceMaximum = search.SalePriceMaximum;
			result.SalePricePerEstateAreaMinimum = search.SalePricePerEstateAreaMinimum;
			result.SalePricePerEstateAreaMaximum = search.SalePricePerEstateAreaMaximum;
			result.SalePricePerUnitAreaMinimum = search.SalePricePerUnitAreaMinimum;
			result.SalePricePerUnitAreaMaximum = search.SalePricePerUnitAreaMaximum;

			result.RentMortgageMinimum = search.RentMortgageMinimum;
			result.RentMortgageMaximum = search.RentMortgageMaximum;
			result.RentMinimum = search.RentMinimum;
			result.RentMaximum = search.RentMaximum;

		    if (search.Options != null)
		    {
                result.ShouldHaveParking = search.Options.Contains(PropertySearchOptions.OptionHasParking);
                result.ShouldHaveElevator = search.Options.Contains(PropertySearchOptions.OptionHasElevator);
                result.ShouldHaveStorageRoom = search.Options.Contains(PropertySearchOptions.OptionHasStorageRoom);
                result.ShouldHavePhotos = search.Options.Contains(PropertySearchOptions.OptionHasPhotos);

		        result.AgencyListing = search.Options.Contains(PropertySearchOptions.OptionNoAgencyListing)
		            ? (search.Options.Contains(PropertySearchOptions.OptionAgencyActivityAllowed)
		                ? PropertyAgencyListingSearchType.NoAgencyListingWithAgencyActivityAllowed
		                : PropertyAgencyListingSearchType.NoAgencyListing)
		            : (search.Options.Contains(PropertySearchOptions.OptionAgencyActivityAllowed)
		                ? (PropertyAgencyListingSearchType?) PropertyAgencyListingSearchType.AgencyActivityAllowed
		                : null);
		    }
		    else
		    {
		        result.ShouldHaveParking = false;
		        result.ShouldHaveElevator = false;
		        result.ShouldHaveStorageRoom = false;
		        result.ShouldHaveParking = false;
		        
                result.AgencyListing = null;
		    }

			return result;
		}
	}
}