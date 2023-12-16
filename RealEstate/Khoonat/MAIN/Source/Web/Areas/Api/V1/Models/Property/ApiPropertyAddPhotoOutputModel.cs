using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Property
{
	public class ApiPropertyAddPhotoOutputModel : ApiOutputModelBase
	{
		public OutputItem[] Results { get; set; }

		public class OutputItem
		{
			public string Error { get; set; }
			public long? PhotoID { get; set; }
		}
	}
}