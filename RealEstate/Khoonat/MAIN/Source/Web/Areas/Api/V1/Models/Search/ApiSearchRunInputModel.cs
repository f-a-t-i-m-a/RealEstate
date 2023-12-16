using System.ComponentModel.DataAnnotations;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Search
{
	public class ApiSearchRunInputModel : ApiInputModelBase
	{
		public ApiSearchRunInputModel()
		{
			Pagination = new ApiInputPaginationSpecModel();
		}

		[Required(ErrorMessage = "NoSearchCriteriaSpecified")]
		public ApiSearchRunInputCriteriaModel Criteria { get; set; }

		// Not using standard pagination, since we are allowing far larger page sizes for this API
		public ApiInputPaginationSpecModel Pagination { get; set; }
		public class ApiInputPaginationSpecModel
		{
			public ApiInputPaginationSpecModel()
			{
				StartIndex = 0;
				PageSize = 500;
			}

			[Range(0, 50000, ErrorMessage = "StartIndexOutOfRange")]
			public int StartIndex { get; set; }

			[Range(1, 2000, ErrorMessage = "PageSizeOutOfRange")]
			public int PageSize { get; set; }
		}
	}
}