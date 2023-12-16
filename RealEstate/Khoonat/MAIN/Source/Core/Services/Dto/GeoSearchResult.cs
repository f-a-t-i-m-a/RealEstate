using System.Collections.Generic;
using System.Data.Entity.Spatial;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Domain.Directory;

namespace JahanJooy.RealEstate.Core.Services.Dto
{
	public class GeoSearchResult
	{
		public DbGeography Boundary { get; set; }
		public LatLngBounds BoundingBox { get; set; }
		public double BoundingBoxArea { get; set; }
		public LatLngBounds LargestContainedRect { get; set; }
		public double LargestContainedRectArea { get; set; }
		public double MinimumDistinguishedArea { get; set; }

		public List<GeoSearchVicinityResult> VicinityResults { get; set; }

		public List<PropertyListingSummary> PropertyListings { get; set; }
		public List<AgencyBranch> Agencies { get; set; }
        public bool ReachedMaxResult { get; set; }
        public int PageSize { get; set; }
	}

	public class GeoSearchVicinityResult
	{
		public long VicinityID { get; set; }
		public LatLng GeographicLocation { get; set; }
		public bool Decomposable { get; set; }

		public int NumberOfPropertyListings { get; set; }
		public int NumberOfAgencies { get; set; }
	}
}