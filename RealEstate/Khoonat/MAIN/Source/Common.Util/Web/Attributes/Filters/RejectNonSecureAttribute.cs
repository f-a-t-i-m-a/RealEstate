using System;
using System.Web.Mvc;

namespace JahanJooy.Common.Util.Web.Attributes.Filters
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
	public class RejectNonSecureAttribute : RequireHttpsAttribute
	{
		protected override void HandleNonHttpsRequest(AuthorizationContext filterContext)
		{
			filterContext.Result = new HttpUnauthorizedResult("Only secure (HTTPS) connections are allowed.");
		}
	}
}