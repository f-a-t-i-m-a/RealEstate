using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Util.Presentation.Property;

namespace JahanJooy.RealEstate.Web.Models.Map
{
    public class MapSearchPropertyListingItemModel
    {
        public long ID { get; set; }
        public LatLng Point { get; set; }
		public GeographicLocationSpecificationType PointType { get; set; }
        public string Code { get; set; }
        public string Region { get; set; }
        public string Title { get; set; }
        public string Area { get; set; }
        public string PriceAndPricePerArea { get; set; }
        public PropertyType PropertyType { get; set; }
        public IntentionOfOwner IntentionOfOwner { get; set; }
        public long? CoverPhotoId { get; set; }

	    public static MapSearchPropertyListingItemModel FromDto(PropertyListingSummary listing, 
			PropertyPresentationHelper propertyPresentationHelper)
	    {
		    return new MapSearchPropertyListingItemModel
		           {
			           ID = listing.ID,
			           Point = listing.GeographicLocation.ToLatLng(),
			           PointType = listing.GeographicLocationType ?? GeographicLocationSpecificationType.InferredFromVicinity,
			           Code = listing.Code.ToString(CultureInfo.InvariantCulture),
			           Region = propertyPresentationHelper.BuildRegionString(listing),
			           Title = propertyPresentationHelper.BuildShortTitle(listing),
			           Area = propertyPresentationHelper.BuildAreaString(listing),
			           PriceAndPricePerArea = propertyPresentationHelper.BuildPriceAndPricePerArea(listing).ToString(),
			           PropertyType = listing.PropertyType,
			           IntentionOfOwner = listing.IntentionOfOwner,
			           CoverPhotoId = listing.CoverPhotoId
		           };
	    }

	    public static IEnumerable<MapSearchPropertyListingItemModel> FromDto(IEnumerable<PropertyListingSummary> listings,
			PropertyPresentationHelper propertyPresentationHelper)
	    {
		    return (listings ?? Enumerable.Empty<PropertyListingSummary>())
                .Select(pls => FromDto(pls, propertyPresentationHelper));
	    }
    }
}