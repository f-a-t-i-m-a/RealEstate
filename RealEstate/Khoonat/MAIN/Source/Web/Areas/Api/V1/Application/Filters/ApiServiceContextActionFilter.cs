using System.Web;
using System.Web.Mvc;
using JahanJooy.RealEstate.Core;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Application.Filters
{
    public class ApiServiceContextActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            SetServiceContext();
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            ServiceContext.Reset();
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            SetServiceContext();
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            ServiceContext.Reset();
        }

        private static void SetServiceContext()
        {
            if (!ApiCallContext.Exists)
                return;

            ServiceContext.Set(ApiCallContext.Current.Session, ApiCallContext.Current.EndUser);
            HttpContext.Current.User = ApiCallContext.Current.EndUser;
        }
    }
}