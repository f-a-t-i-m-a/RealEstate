using System.Web.Mvc;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Application.Filters
{
	public class LogApiInvocationAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			ApiLogUtils.LogInvokedCall(filterContext);
		}
	}
}