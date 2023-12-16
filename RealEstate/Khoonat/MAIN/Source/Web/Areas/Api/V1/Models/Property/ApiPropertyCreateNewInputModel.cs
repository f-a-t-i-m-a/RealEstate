using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Property.Shared;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Property
{
	public class ApiPropertyCreateNewInputModel : ApiInputModelBase
	{
		public ApiPropertyDetailsModel PropertyListingDetails { get; set; }
	}
}