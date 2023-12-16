using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using JahanJooy.RealEstate.Domain.Enums;

namespace JahanJooy.RealEstate.Core.Search
{
	public class PropertySearch
	{
		//
		// Search query text, should contain a string representation of the rest of the search spec.

		public string OriginalQuery { get; set; }
		public string Query { get; set; }

		//
		// Sort order

		public PropertySearchSortOrder? SortOrder { get; set; }

		//
		// Type 

		public PropertyType? PropertyType { get; set; }
		public IntentionOfOwner? IntentionOfOwner { get; set; }

		//
		// Location

		public long[] VicinityIDs { get; set; }
		public DbGeography GeographyBounds { get; set; }

		//
		// Estate

		public decimal? EstateAreaMinimum { get; set; }
		public decimal? EstateAreaMaximum { get; set; }

		//
		// Unit

		public int? NumberOfRoomsMinimum { get; set; }
		public int? NumberOfRoomsMaximum { get; set; }
		public int? NumberOfParkingsMinimum { get; set; }
		public decimal? UnitAreaMinimum { get; set; }
		public decimal? UnitAreaMaximum { get; set; }

		//
		// Price

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

		//
		// Options

		public List<string> Options { get; set; }

		#region Initialization

		public PropertySearch()
		{
			Options = new List<string>();
		}

		public PropertySearch(PropertySearch source)
		{
			SortOrder = source.SortOrder;
			PropertyType = source.PropertyType;
			IntentionOfOwner = source.IntentionOfOwner;

            VicinityIDs = source.VicinityIDs == null ? null : (long[])source.VicinityIDs.Clone();
			GeographyBounds = source.GeographyBounds;

			EstateAreaMinimum = source.EstateAreaMinimum;
			EstateAreaMaximum = source.EstateAreaMaximum;

			NumberOfRoomsMinimum = source.NumberOfRoomsMinimum;
			NumberOfRoomsMaximum = source.NumberOfRoomsMaximum;
			NumberOfParkingsMinimum = source.NumberOfParkingsMinimum;
			UnitAreaMinimum = source.UnitAreaMinimum;
			UnitAreaMaximum = source.UnitAreaMaximum;

			SalePriceMinimum = source.SalePriceMinimum;
			SalePriceMaximum = source.SalePriceMaximum;
			SalePricePerEstateAreaMinimum = source.SalePricePerEstateAreaMinimum;
			SalePricePerEstateAreaMaximum = source.SalePricePerEstateAreaMaximum;
			SalePricePerUnitAreaMinimum = source.SalePricePerUnitAreaMinimum;
			SalePricePerUnitAreaMaximum = source.SalePricePerUnitAreaMaximum;

			RentMortgageMinimum = source.RentMortgageMinimum;
			RentMortgageMaximum = source.RentMortgageMaximum;
			RentMinimum = source.RentMinimum;
			RentMaximum = source.RentMaximum;

			Options = source.Options == null ? new List<string>() : new List<string>(source.Options);
		}

		public static PropertySearch Copy(PropertySearch source)
		{
			return source == null ? null : new PropertySearch(source);
		}

		public static ICollection<PropertySearch> Copy(ICollection<PropertySearch> source)
		{
			return source == null ? null : source.Select(Copy).ToList();
		}

		#endregion

		#region Methods

		public void FixLogicalErrors()
		{
			if (IntentionOfOwner.HasValue && IntentionOfOwner.Value != Domain.Enums.IntentionOfOwner.ForRent)
			{
				RentMortgageMinimum = null;
				RentMortgageMaximum = null;
				RentMinimum = null;
				RentMaximum = null;
			}

			if (IntentionOfOwner.HasValue && IntentionOfOwner.Value != Domain.Enums.IntentionOfOwner.ForSale)
			{
				SalePriceMinimum = null;
				SalePriceMaximum = null;
				SalePricePerEstateAreaMinimum = null;
				SalePricePerEstateAreaMaximum = null;
				SalePricePerUnitAreaMinimum = null;
				SalePricePerUnitAreaMaximum = null;
			}

			if (!IntentionOfOwner.HasValue)
			{
				RentMortgageMinimum = null;
				RentMortgageMaximum = null;
				RentMinimum = null;
				RentMaximum = null;
				SalePriceMinimum = null;
				SalePriceMaximum = null;
				SalePricePerEstateAreaMinimum = null;
				SalePricePerEstateAreaMaximum = null;
				SalePricePerUnitAreaMinimum = null;
				SalePricePerUnitAreaMaximum = null;
			}

			if (PropertyType.HasValue && !PropertyType.Value.HasUnit())
			{
				UnitAreaMinimum = null;
				UnitAreaMaximum = null;
				NumberOfRoomsMinimum = null;
				NumberOfRoomsMaximum = null;
				SalePricePerUnitAreaMinimum = null;
				SalePricePerUnitAreaMaximum = null;
			}

			if (PropertyType.HasValue && !PropertyType.Value.IsEstateSignificant())
			{
				EstateAreaMinimum = null;
				EstateAreaMaximum = null;
				SalePricePerEstateAreaMinimum = null;
				SalePricePerEstateAreaMaximum = null;
			}

			if (!PropertyType.HasValue)
			{
				UnitAreaMinimum = null;
				UnitAreaMaximum = null;
				NumberOfRoomsMinimum = null;
				NumberOfRoomsMaximum = null;
				SalePricePerUnitAreaMinimum = null;
				SalePricePerUnitAreaMaximum = null;
				EstateAreaMinimum = null;
				EstateAreaMaximum = null;
				SalePricePerEstateAreaMinimum = null;
				SalePricePerEstateAreaMaximum = null;
			}

			if (EstateAreaMinimum.HasValue && EstateAreaMaximum.HasValue && EstateAreaMaximum.Value < EstateAreaMinimum.Value)
			{
				var tempValue = EstateAreaMaximum.Value;
				EstateAreaMaximum = EstateAreaMinimum;
				EstateAreaMinimum = tempValue;
			}

			if (UnitAreaMinimum.HasValue && UnitAreaMaximum.HasValue && UnitAreaMaximum.Value < UnitAreaMinimum.Value)
			{
				var tempValue = UnitAreaMaximum.Value;
				UnitAreaMaximum = UnitAreaMinimum;
				UnitAreaMinimum = tempValue;
			}

			if (NumberOfRoomsMinimum.HasValue && NumberOfRoomsMaximum.HasValue && NumberOfRoomsMaximum.Value < NumberOfRoomsMinimum.Value)
			{
				var tempValue = NumberOfRoomsMaximum.Value;
				NumberOfRoomsMaximum = NumberOfRoomsMinimum;
				NumberOfRoomsMinimum = tempValue;
			}

			if (SalePriceMinimum.HasValue && SalePriceMaximum.HasValue && SalePriceMaximum.Value < SalePriceMinimum.Value)
			{
				var tempValue = SalePriceMaximum.Value;
				SalePriceMaximum = SalePriceMinimum;
				SalePriceMinimum = tempValue;
			}

			if (SalePricePerEstateAreaMinimum.HasValue && SalePricePerEstateAreaMaximum.HasValue && SalePricePerEstateAreaMaximum.Value < SalePricePerEstateAreaMinimum.Value)
			{
				var tempValue = SalePricePerEstateAreaMaximum.Value;
				SalePricePerEstateAreaMaximum = SalePricePerEstateAreaMinimum;
				SalePricePerEstateAreaMinimum = tempValue;
			}

			if (SalePricePerUnitAreaMinimum.HasValue && SalePricePerUnitAreaMaximum.HasValue && SalePricePerUnitAreaMaximum.Value < SalePricePerUnitAreaMinimum.Value)
			{
				var tempValue = SalePricePerUnitAreaMaximum.Value;
				SalePricePerUnitAreaMaximum = SalePricePerUnitAreaMinimum;
				SalePricePerUnitAreaMinimum = tempValue;
			}

			if (RentMinimum.HasValue && RentMaximum.HasValue && RentMaximum.Value < RentMinimum.Value)
			{
				var tempValue = RentMaximum.Value;
				RentMaximum = RentMinimum;
				RentMinimum = tempValue;
			}

			if (RentMortgageMinimum.HasValue && RentMortgageMaximum.HasValue && RentMortgageMaximum.Value < RentMortgageMinimum.Value)
			{
				var tempValue = RentMortgageMaximum.Value;
				RentMortgageMaximum = RentMortgageMinimum;
				RentMortgageMinimum = tempValue;
			}
			
		}

		#endregion
	}
}