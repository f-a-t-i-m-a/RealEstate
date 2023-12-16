using System.Web.Mvc;
using System.Web.Routing;
using JahanJooy.Common.Util.Web.Routing;
using JahanJooy.RealEstate.Web.Controllers;

namespace JahanJooy.RealEstate.Web.Application.Config
{
	public static class RouteConfig
	{
		public static void RegisterAllRoutes(RouteCollection routes)
		{
			RegisterShortcutRoutes(routes);
			RegisterRoutes(routes);
		}

		private static void RegisterShortcutRoutes(RouteCollection routes)
		{
			routes.MapRoute(
				"ShortcutForProperty",
				"p/{code}",
				new { controller = "Property", action = "ViewDetailsByCode" },
				new[] { typeof(PropertyController).Namespace }
				);
		}

		private static void RegisterRoutes(RouteCollection routes)
		{
			routes.LowercaseUrls = true;

			// Ignored routes

			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.([iI][cC][oO]|[gG][iI][fF])(/.*)?" });

			// Similar to the default route, without default action and controller parameters. Since most of
			// the requests do have controller and action values, this route is repeated here to prevent
			// evaluation of the next (more specific) routes on every request.
			// to prevent evaluating 

			routes.MapRoute(
							"ControllerActionId",
							"{controller}/{action}/{id}",
							new { id = UrlParameter.Optional },
							new[] { typeof(HomeController).Namespace }
				);

			// Sitemap route, that handles requests to URLs similar to "sitemap.<action>.xml" for
			// Search Engine Optimization.

			routes.Add("Sitemap",
					   new RegexRoute("^sitemap(\\.(?<action>\\w*))?(\\.(?<param1>\\w*))?(\\.(?<param2>\\w*))?\\.xml$",
									  new { controller = "Sitemap", action = "Index" }));

			routes.MapRoute(
							"Default",
							"{controller}/{action}/{id}",
							new { controller = "Home", action = "Index", id = UrlParameter.Optional },
							new[] { typeof(HomeController).Namespace }
				);
		}
	}
}