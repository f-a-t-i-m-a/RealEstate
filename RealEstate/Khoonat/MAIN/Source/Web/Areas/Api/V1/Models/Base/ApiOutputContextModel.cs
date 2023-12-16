namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base
{
	public class ApiOutputContextModel
	{
		public string AppVersion { get; set; }
		public string ApiVersion { get; set; }
		public bool IsObsolete { get; set; }

		public string[] ServerMessages { get; set; }
		public string[] CustomMessages { get; set; }
	}
}