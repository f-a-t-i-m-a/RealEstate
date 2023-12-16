using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Vicinity
{
	public class ApiVicinityBrowseInputModel : ApiInputModelBase
	{
		public long? ParentID { get; set; }
		public bool IncludeGeoBox { get; set; }
		public bool IncludeGeoPath { get; set; }
	}
}