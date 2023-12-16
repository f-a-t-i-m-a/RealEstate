
namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base
{
	public abstract class ApiInputPaginatingModelBase : ApiInputModelBase
	{
		protected ApiInputPaginatingModelBase()
		{
			Pagination = new ApiInputPaginationSpecModel();
		}

		public ApiInputPaginationSpecModel Pagination { get; set; }
	}
}