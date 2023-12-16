using System.Collections.Generic;
using System.Linq;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Core.Services.Dto;
using JahanJooy.RealEstate.Util.Presentation.Property;

namespace JahanJooy.RealEstate.Web.Models.Map
{
    public class MapSearchResultModel
    {
		public LatLngBounds BoundingBox { get; set; }
		public double BoundingBoxArea { get; set; }
		public LatLngBounds LargestContainedRect { get; set; }
		public double LargestContainedRectArea { get; set; }
		public double MinimumDistinguishedArea { get; set; }

        public List<MapSearchPropertyListingItemModel> PropertyListings { get; set; }
		public List<MapSearchVicinityItemModel> Vicinities { get; set; }
		public List<MapSearchAgencyBranchItemModel> AgencyBranches { get; set; }
        public bool ReachedMaxResult { get; set; }
        public int PageSize { get; set; }

	    public static MapSearchResultModel FromDto(GeoSearchResult dto, 
			PropertyPresentationHelper propertyPresentationHelper, IVicinityCache vicinityCache)
	    {
		    return new MapSearchResultModel
		           {
			           BoundingBox = dto.BoundingBox,
			           BoundingBoxArea = dto.BoundingBoxArea,
			           LargestContainedRect = dto.LargestContainedRect,
			           LargestContainedRectArea = dto.LargestContainedRectArea,
			           MinimumDistinguishedArea = dto.MinimumDistinguishedArea,
			           PropertyListings = MapSearchPropertyListingItemModel.FromDto(dto.PropertyListings, propertyPresentationHelper).ToList(),
			           Vicinities = MapSearchVicinityItemModel.FromDto(dto.VicinityResults, vicinityCache).ToList(),
					   AgencyBranches = MapSearchAgencyBranchItemModel.FromDto(dto.Agencies).ToList(),
                       ReachedMaxResult = dto.ReachedMaxResult,
                       PageSize= dto.PageSize
		           };
	    }
    }
}