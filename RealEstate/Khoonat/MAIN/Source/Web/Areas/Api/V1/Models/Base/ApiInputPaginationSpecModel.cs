using System.ComponentModel.DataAnnotations;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base
{
	public class ApiInputPaginationSpecModel
	{
		public ApiInputPaginationSpecModel()
		{
			StartIndex = 0;
			PageSize = 20;
		}

		[Range(0, 10000, ErrorMessage = "StartIndexOutOfRange")]
		public int StartIndex { get; set; }

		[Range(1, 100, ErrorMessage = "PageSizeOutOfRange")]
		public int PageSize { get; set; }
	}
}