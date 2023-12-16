using System.Web;
using System.Web.Mvc;
using JahanJooy.RealEstate.Web.Application.Filters;

namespace JahanJooy.RealEstate.Web.Application.Config
{
	public static class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new ServiceContextActionFilter());
			filters.Add(new HandleErrorAttribute { View = "_Errors/ExceptionInAction" });
			filters.Add(new HandleErrorAttribute { View = "_Errors/RequestValidationError", ExceptionType = typeof(HttpRequestValidationException) });
		}
	}
}