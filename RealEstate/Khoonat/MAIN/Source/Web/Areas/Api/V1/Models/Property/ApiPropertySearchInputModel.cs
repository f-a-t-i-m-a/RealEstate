using System.Linq;
using JahanJooy.RealEstate.Core.Search;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Shared;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Property
{
	public class ApiPropertySearchInputModel : ApiInputPaginatingModelBase
	{
		public ApiPropertySearchInputModel()
		{
			VicinityFilter = new ApiInputVicinityFilterModel();
			PropertyFilter = new PropertyListingFilter();
		}

		public ApiInputVicinityFilterModel VicinityFilter { get; set; }
		public PropertyListingFilter PropertyFilter { get; set; }
		
		public bool IncludeCoverPhotoThumbnailBytes { get; set; }

        public bool ReturnEssentialInfoOnly { get; set; }

		public class PropertyListingFilter
		{
			public PropertyType? PropertyType { get; set; }
			public IntentionOfOwner? IntentionOfOwner { get; set; }

			public decimal? EstateAreaMinimum { get; set; }
			public decimal? EstateAreaMaximum { get; set; }

			public int? NumberOfRoomsMinimum { get; set; }
			public int? NumberOfRoomsMaximum { get; set; }
			public int? NumberOfParkingsMinimum { get; set; }
			public decimal? UnitAreaMinimum { get; set; }
			public decimal? UnitAreaMaximum { get; set; }

			public decimal? SalePriceMinimum { get; set; }
			public decimal? SalePriceMaximum { get; set; }
			public decimal? SalePricePerEstateAreaMinimum { get; set; }
			public decimal? SalePricePerEstateAreaMaximum { get; set; }
			public decimal? SalePricePerUnitAreaMinimum { get; set; }
			public decimal? SalePricePerUnitAreaMaximum { get; set; }

			public decimal? RentMortgageMinimum { get; set; }
			public decimal? RentMortgageMaximum { get; set; }
			public decimal? RentMinimum { get; set; }
			public decimal? RentMaximum { get; set; }

			public string[] Tags { get; set; }
		}

		#region Methods

		public PropertySearch ToPropertySearch()
		{
			var result = new PropertySearch();

			if (VicinityFilter != null)
			{
				result.VicinityIDs = VicinityFilter.VicinityIDs;
			}

			if (PropertyFilter != null)
			{
				result.PropertyType = PropertyFilter.PropertyType;
				result.IntentionOfOwner = PropertyFilter.IntentionOfOwner;


				result.EstateAreaMinimum = PropertyFilter.EstateAreaMinimum;
				result.EstateAreaMaximum = PropertyFilter.EstateAreaMaximum;

				result.NumberOfRoomsMinimum = PropertyFilter.NumberOfRoomsMinimum;
				result.NumberOfRoomsMaximum = PropertyFilter.NumberOfRoomsMaximum;
				result.NumberOfParkingsMinimum = PropertyFilter.NumberOfParkingsMinimum;
				result.UnitAreaMinimum = PropertyFilter.UnitAreaMinimum;
				result.UnitAreaMaximum = PropertyFilter.UnitAreaMaximum;

				result.SalePriceMinimum = PropertyFilter.SalePriceMinimum;
				result.SalePriceMaximum = PropertyFilter.SalePriceMaximum;
				result.SalePricePerEstateAreaMinimum = PropertyFilter.SalePricePerEstateAreaMinimum;
				result.SalePricePerEstateAreaMaximum = PropertyFilter.SalePricePerEstateAreaMaximum;
				result.SalePricePerUnitAreaMinimum = PropertyFilter.SalePricePerUnitAreaMinimum;
				result.SalePricePerUnitAreaMaximum = PropertyFilter.SalePricePerUnitAreaMaximum;

				result.RentMortgageMinimum = PropertyFilter.RentMortgageMinimum;
				result.RentMortgageMaximum = PropertyFilter.RentMortgageMaximum;
				result.RentMinimum = PropertyFilter.RentMinimum;
				result.RentMaximum = PropertyFilter.RentMaximum;

				if (PropertyFilter.Tags != null && PropertyFilter.Tags.Any())
				{
					result.Options = PropertyFilter.Tags.Where(tag => PropertySearchOptions.AllOptions.Keys.Contains(tag)).ToList();
				}
			}

			return result;
		}

		#endregion
	}
}