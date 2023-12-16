namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base
{
	public abstract class ApiInputModelBase
	{
		protected ApiInputModelBase()
		{
			Context = new ApiInputContextModel();
		}

		public ApiInputContextModel Context { get; set; }
	}
}