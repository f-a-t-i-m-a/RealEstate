using AutoMapper;
using JahanJooy.Common.Util.Spatial;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Shared.Spatial
{
	public class ApiGeoBox
	{
		public double NorthLat { get; set; }
		public double SouthLat { get; set; }
		public double EastLng { get; set; }
		public double WestLng { get; set; }

		public LatLngBounds ToLatLngBounds()
		{
			return new LatLngBounds
			       {
				       NorthLat = NorthLat,
				       SouthLat = SouthLat,
				       EastLng = EastLng,
				       WestLng = WestLng
			       };
		}

		public static ApiGeoBox FromLatLngBounds(LatLngBounds bounds)
		{
			if (bounds == null)
				return null;

			return new ApiGeoBox
			       {
				       NorthLat = bounds.NorthLat,
				       SouthLat = bounds.SouthLat,
				       EastLng = bounds.EastLng,
				       WestLng = bounds.WestLng
			       };
		}

		public static void ConfigureMapper()
		{
			Mapper.CreateMap<ApiGeoBox, LatLngBounds>().ConvertUsing(box => box.ToLatLngBounds());
			Mapper.CreateMap<LatLngBounds, ApiGeoBox>().ConvertUsing(FromLatLngBounds);
		}
	}
}