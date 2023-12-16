using System.Collections.Generic;
using System.Linq;

namespace JahanJooy.Common.Util.Spatial
{
	public class LatLngPath
	{
		public LatLngPath()
		{
			Points = new List<LatLng>();
		}

		public List<LatLng> Points { get; set; } 
		public string Title { get; set; }

		public LatLngPath SetTitle(string title)
		{
			Title = title;
			return this;
		}

		public string ToGoogleApi()
		{
			return "[" + string.Join(", ", Points.Select(ll => ll.ToGoogleApi())) + "]";
		}
	}
}