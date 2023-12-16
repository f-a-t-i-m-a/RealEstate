namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Shared
{
	public class ApiInputVicinityFilterModel
	{
		public long[] VicinityIDs { get; set; }
		public bool IncludeAdjacentVicinities { get; set; }
	}
}