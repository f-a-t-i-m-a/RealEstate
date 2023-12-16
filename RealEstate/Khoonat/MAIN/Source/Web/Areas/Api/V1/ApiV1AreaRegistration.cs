using System.Web.Mvc;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Controllers;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1
{
	public class ApiV1AreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get
			{
				return "apiv1";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute(
				"Api_default",
				"api/v1/{controller}/{action}/{id}",
				new { controller = "ApiHome", action = "Index", id = UrlParameter.Optional },
				new[] { typeof(ApiHomeController).Namespace }
			);
		}
	}
}