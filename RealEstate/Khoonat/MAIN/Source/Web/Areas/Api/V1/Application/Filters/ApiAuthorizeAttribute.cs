using System.Web.Mvc;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Application.Filters
{
    public class ApiAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!ApiCallContext.Exists ||
                ApiCallContext.Current.EndUser == null ||
                !ApiCallContext.Current.EndUser.IsAuthenticated)
            {
                
            }
        }
    }
}