﻿using System.Web.Mvc;
using System.Web.Routing;

namespace JahanJooy.RealEstateAgency.HaftDong.Server.Config
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("TranslationsJs", "app/js/translations", new { controller = "App", action = "TranslationsJs" });
            routes.MapRoute("App", "app/{*any}", new { controller = "App", action = "InitApp" });
            routes.MapRoute("AdminApp", "adminapp/{*any}", new { controller = "AdminApp", action = "InitAdminApp" });
			routes.MapRoute("Home", "", new {controller = "App", action = "Index"});
		}
	}
}
