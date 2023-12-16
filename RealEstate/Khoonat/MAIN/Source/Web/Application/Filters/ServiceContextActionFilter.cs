using System.Web;
using System.Web.Mvc;
using JahanJooy.RealEstate.Core;
using JahanJooy.RealEstate.Core.Security;
using JahanJooy.RealEstate.Web.Application.Session;

namespace JahanJooy.RealEstate.Web.Application.Filters
{
	public class ServiceContextActionFilter : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			ServiceContext.Set(HttpContext.Current.Session.GetSessionInfo(),
			                   HttpContext.Current.User as CorePrincipal);
		}

		public override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			ServiceContext.Reset();
		}

		public override void OnResultExecuting(ResultExecutingContext filterContext)
		{
			ServiceContext.Set(HttpContext.Current.Session.GetSessionInfo(),
							   HttpContext.Current.User as CorePrincipal);
		}

		public override void OnResultExecuted(ResultExecutedContext filterContext)
		{
			ServiceContext.Reset();
		}
	}
}