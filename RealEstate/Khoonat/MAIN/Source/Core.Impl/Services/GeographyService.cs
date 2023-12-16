using System;
using System.Data.Entity.Spatial;
using Compositional.Composer;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Core.Impl.Components;
using JahanJooy.RealEstate.Core.Search;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Core.Services.Dto;

namespace JahanJooy.RealEstate.Core.Impl.Services
{
	[Component]
	public class GeographyService : IGeographyService
	{
		[ComponentPlug]
		public IPropertyService PropertyService { get; set; }

		[ComponentPlug]
		public IVicinityCache VicinityCache { get; set; }

		[ComponentPlug]
		public IComposer Composer { get; set; }

		public GeoSearchResult RunSearch(DbGeography boundary, PropertySearch propertyListingFilter = null)
		{
			if (boundary == null)
				throw new ArgumentNullException("boundary");

			var result = new GeoSearchResult();
			result.Boundary = boundary;
			result.BoundingBox = boundary.FindBoundingBox();
			result.BoundingBoxArea = result.BoundingBox.GetArea();
			result.LargestContainedRect = result.BoundingBox.GetLargestContainedRect();
			result.LargestContainedRectArea = result.LargestContainedRect.GetArea();
			result.MinimumDistinguishedArea = result.LargestContainedRectArea / 20;

			if (result.BoundingBoxArea <= 0)
				return result;

			propertyListingFilter = propertyListingFilter ?? new PropertySearch();
			propertyListingFilter.GeographyBounds = boundary;
		    const int pageSize = 1000;
            var propertyListings = PropertyService.RunSearch(propertyListingFilter, 0, pageSize, false, false);
			// TODO: Using "RunSearch" increases "number of searches" and keeps a history each time a geo-query is performed. Is it okay?
		    if (propertyListings.PageResults.Count >= pageSize)
		    {
                result.ReachedMaxResult = true;
		    }
		    result.PageSize = pageSize;
		    var clusterCalculator = Composer.GetComponent<OldGeographicClusterCalculator>();
			clusterCalculator.SetMinimumAreaToBreakDown(result.MinimumDistinguishedArea);
			clusterCalculator.ClusterPropertyListings(propertyListings.PageResults);
			clusterCalculator.PrepareResults(result);

			return result;
		}

	}
}