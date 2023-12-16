using System.ComponentModel.DataAnnotations;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Vicinity
{
	public class ApiVicinitySearchInputModel : ApiInputPaginatingModelBase
	{
		[Required(ErrorMessage = "Search query cannot be empty")]
		[StringLength(50, ErrorMessage = "Query string length is outside acceptable bounds", MinimumLength = 1)]
		public string Query { get; set; }

		public long? Scope { get; set; }
		public bool CanContainPropertyRecordsOnly { get; set; }
	}
}