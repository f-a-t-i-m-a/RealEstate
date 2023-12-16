using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Vicinity
{
	public class ApiVicinityGetInputModel : ApiInputModelBase
	{
		public long[] VicinityIDs { get; set; }
		public bool IncludeGeoBox { get; set; }
		public bool IncludeGeoPath { get; set; }
	}
}