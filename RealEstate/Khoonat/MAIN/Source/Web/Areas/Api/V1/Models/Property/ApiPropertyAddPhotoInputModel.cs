using System.ComponentModel.DataAnnotations;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Property.Shared;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Property
{
	public class ApiPropertyAddPhotoInputModel : ApiInputModelBase
	{
		public long PropertyListingID { get; set; }
		public string EditPassword { get; set; }

		[Required]
		public ApiPropertyPhotoInputModel[] Photos { get; set; }
	}
}