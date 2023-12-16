using System;
using System.Web.Mvc;
using JahanJooy.Common.Util.Configuration;

namespace JahanJooy.Common.Util.Web.Attributes.Filters
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
	public class RejectNonSecureInProductionAttribute : RejectNonSecureAttribute
	{
		protected override void HandleNonHttpsRequest(AuthorizationContext filterContext)
		{
			if (ApplicationEnvironmentUtil.Type == ApplicationEnvironmentType.Development)
				return;

			base.HandleNonHttpsRequest(filterContext);
		}
	}
}