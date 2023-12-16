using System.Data.Entity.Spatial;

namespace JahanJooy.Common.Util.Spatial
{
	public class SpatialBoundary
	{
		public DbGeography Geography { get; set; }
		public string Title { get; set; }
	}
}