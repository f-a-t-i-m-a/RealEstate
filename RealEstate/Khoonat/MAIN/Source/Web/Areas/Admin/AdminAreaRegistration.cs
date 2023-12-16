using System.Web.Mvc;
using JahanJooy.RealEstate.Web.Areas.Admin.Controllers;

namespace JahanJooy.RealEstate.Web.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Admin_default",
                "admin/{controller}/{action}/{id}",
                new { controller = "AdminHome", action = "Index", id = UrlParameter.Optional },
                new[] { typeof(AdminHomeController).Namespace }
            );
        }
    }
}
