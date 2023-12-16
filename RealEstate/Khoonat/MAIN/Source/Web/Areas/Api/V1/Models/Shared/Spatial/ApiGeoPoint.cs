using System.Data.Entity.Spatial;
using AutoMapper;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.Spatial;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Shared.Spatial
{
	public class ApiGeoPoint
	{
		public double Lat { get; set; }
		public double Lng { get; set; }

		public LatLng ToLatLng()
		{
			return new LatLng {Lat = Lat, Lng = Lng};
		}

		public string ToWkt()
		{
			return string.Format("POINT({0} {1})", Lng, Lat);
		}

		public DbGeography ToDbGeography()
		{
			return DbGeographyUtil.CreatePoint(ToWkt());
		}

		public static ApiGeoPoint FromDbGeography(DbGeography geography)
		{
			if (geography == null || geography.IsEmpty || !geography.Latitude.HasValue || !geography.Longitude.HasValue)
				return null;

			return new ApiGeoPoint { Lat = geography.Latitude.Value, Lng = geography.Longitude.Value };
		}

		public static ApiGeoPoint FromLatLng(LatLng point)
		{
			if (point == null)
				return null;

			return new ApiGeoPoint
			       {
				       Lat = point.Lat,
				       Lng = point.Lng
			       };
		}

		public static void ConfigureMapper()
		{
			Mapper.CreateMap<DbGeography, ApiGeoPoint>().ConvertUsing(FromDbGeography);
			Mapper.CreateMap<LatLng, ApiGeoPoint>().ConvertUsing(FromLatLng);

			Mapper.CreateMap<ApiGeoPoint, LatLng>()
				.ConvertUsing(p => p.IfNotNull(q => q.ToLatLng()));
			Mapper.CreateMap<ApiGeoPoint, DbGeography>()
				.ConvertUsing(p => p.IfNotNull(q => q.ToLatLng().ToDbGeography()));
		}
	}
}