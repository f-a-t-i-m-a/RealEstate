namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base
{
	public class ApiInputContextModel 
	{
		public string UserCulture { get; set; }
		public string SessionId { get; set; }

		public string ApiKey { get; set; }
		public string Token { get; set; }
	}
}