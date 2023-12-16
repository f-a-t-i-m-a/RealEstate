using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Shared.Spatial;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Geo
{
	public class ApiGeoCodeInputModel : ApiInputModelBase
	{
		public ApiGeoPoint Point { get; set; }
		public bool IncludeGeoBox { get; set; }
		public bool IncludeGeoPath { get; set; }
	}
}