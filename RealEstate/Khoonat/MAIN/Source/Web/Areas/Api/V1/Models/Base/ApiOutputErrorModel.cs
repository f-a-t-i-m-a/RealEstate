namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base
{
	public class ApiOutputErrorModel : ApiOutputModelBase
	{
		public ErrorDetails Error { get; set; }

		public class ErrorDetails
		{
			public int Code { get; set; }
			public string Label { get; set; }
			public string Message { get; set; }
			public string Details { get; set; }
		}
	}
}