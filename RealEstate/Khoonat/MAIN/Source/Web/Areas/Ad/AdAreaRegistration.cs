using System.Web.Mvc;
using JahanJooy.RealEstate.Web.Areas.Ad.Controllers;

namespace JahanJooy.RealEstate.Web.Areas.Ad
{
    public class AdAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ad";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Ad_default",
                "ad/{controller}/{action}/{id}",
                new { controller = "AdHome", action = "Index", id = UrlParameter.Optional },
                new[] { typeof(AdAdminHomeController).Namespace }
            );
        }
    }
}