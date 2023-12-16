using System.Linq;
using JahanJooy.Common.Util.Spatial;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Shared.Spatial
{
	public class ApiGeoPath
	{
		public ApiGeoPoint[] Points { get; set; }

		public static ApiGeoPath FromLatLngPath(LatLngPath path)
		{
			if (path == null || path.Points == null)
				return null;

			return new ApiGeoPath
			       {
				       Points = path.Points.Select(ApiGeoPoint.FromLatLng).ToArray()
			       };
		}
	}
}