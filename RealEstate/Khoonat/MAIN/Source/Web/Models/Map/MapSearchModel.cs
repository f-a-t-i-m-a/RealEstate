using System.Collections.Generic;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.RealEstate.Domain.Enums;

namespace JahanJooy.RealEstate.Web.Models.Map
{
    public class MapSearchModel
    {
        public LatLngBounds Bounds { get; set; }
        public string q { get; set; }
        public List<PropertyType> PropertyTypes { get; set; }
        public List<IntentionOfOwner> IntentionOfOwners { get; set; }
        public List<string> Tags { get; set; } 
        
        public int? NumberOfRoomsMinimum { get; set; }
        public int? NumberOfRoomsMaximum { get; set; }

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

        public decimal? EstateAreaMinimum { get; set; }
        public decimal? EstateAreaMaximum { get; set; }
        public decimal? UnitAreaMinimum { get; set; }
        public decimal? UnitAreaMaximum { get; set; }

        public bool? ShowAgencyBranches { get; set; }

    }
}