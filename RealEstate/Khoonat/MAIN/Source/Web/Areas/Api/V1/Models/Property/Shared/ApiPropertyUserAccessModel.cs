namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Property.Shared
{
	public class ApiPropertyUserAccessModel
	{
		public bool IsPublic { get; set; }
		public int? DaysBeforeAutoArchive { get; set; }
	}
}