using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Property
{
	public class ApiPropertyGetDetailsInputModel : ApiInputPaginatingModelBase
	{
		public long PropertyListingID { get; set; }
		public string EditPassword { get; set; }
	}
}