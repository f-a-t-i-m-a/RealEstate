﻿using System.Data.Entity.Spatial;

namespace JahanJooy.Common.Util.Spatial
{
	public class LatLng
	{
        public static readonly LatLng Zero = new LatLng {Lat = 0, Lng = 0};

		public double Lat { get; set; }
		public double Lng { get; set; }

		public string ToGoogleApi()
		{
			return string.Format("new google.maps.LatLng({0}, {1})", Lat, Lng);
		}

		public string ToWkt()
		{
			return string.Format("POINT({0} {1})", Lng, Lat);
		}

		public DbGeography ToDbGeography()
		{
			return DbGeographyUtil.CreatePoint(ToWkt());
		}

		public static LatLng FromWkt(string wkt)
		{
			return DbGeographyUtil.CreatePoint(wkt).ToLatLng();
		}
	}
}