using System.Linq;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.Common.Util.Web.Extensions;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Util.DomainUtil;
using JahanJooy.RealEstate.Util.Resources;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Shared.Spatial;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Shared
{
	public class ApiOutputVicinityDetailsModel : ApiOutputVicinityModel
	{
		public string AlternativeNames { get; set; }
		public VicinityType Type { get; set; }
		public string TypeLabel { get; set; }
		public string FullName { get; set; }
		public bool HasChildren { get; set; }
		public bool CanContainPropertyRecords { get; set; }
		public bool ShowTypeInTitle { get; set; }

		public ApiGeoPoint GeoCenter { get; set; }
		public ApiGeoBox GeoBox { get; set; }
		public ApiGeoPath GeoPath { get; set; }

		public static ApiOutputVicinityDetailsModel FromVicinityID(long? vicinityID, IVicinityCache vicinityCache, bool includeGeoBox = false, bool includeGeoPath = false)
		{
			if (!vicinityID.HasValue)
				return null;

			var vicinity = vicinityCache[vicinityID.Value];
			return FromVicinity(vicinity, includeGeoBox, includeGeoPath);
		}

		public static ApiOutputVicinityDetailsModel FromVicinity(Domain.Vicinity vicinity, bool includeGeoBox = false, bool includeGeoPath = false)
		{
			if (vicinity == null)
				return null;

			var result = new ApiOutputVicinityDetailsModel();
			result.PopulateFromVicinity(vicinity, includeGeoBox, includeGeoPath);
			return result;
		}

		public override void PopulateFromVicinity(Domain.Vicinity vicinity, bool includeHierarchy)
		{
			base.PopulateFromVicinity(vicinity, includeHierarchy);

			AlternativeNames = vicinity.AlternativeNames;
			Type = vicinity.Type;
			TypeLabel = vicinity.Type.Label(DomainEnumResources.ResourceManager);
			FullName = vicinity.GetDisplayFullName();
			HasChildren = (vicinity.Children != null && vicinity.Children.Any());
			CanContainPropertyRecords = vicinity.CanContainPropertyRecords;
			ShowTypeInTitle = vicinity.ShowTypeInTitle;
		}

		public void PopulateFromVicinity(Domain.Vicinity vicinity, bool includeGeoBox, bool includeGeoPath)
		{
			if (vicinity == null)
				return;

			PopulateFromVicinity(vicinity, true);

			if (includeGeoBox)
			{
				GeoBox = ApiGeoBox.FromLatLngBounds(vicinity.Boundary.FindBoundingBox());
				GeoCenter = ApiGeoPoint.FromLatLng(vicinity.CenterPoint.ToLatLng() ?? vicinity.Boundary.FindBoundingBox().GetCenter());
			}

			if (includeGeoPath)
			{
				GeoPath = ApiGeoPath.FromLatLngPath(vicinity.Boundary.ToLatLngPath());
			}
		}
	}
}