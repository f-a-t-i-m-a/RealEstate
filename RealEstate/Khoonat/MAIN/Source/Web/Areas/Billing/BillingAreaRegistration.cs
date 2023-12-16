using System.Web.Mvc;
using JahanJooy.RealEstate.Web.Areas.Billing.Controllers;

namespace JahanJooy.RealEstate.Web.Areas.Billing
{
    public class BillingAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "billing";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Billing_default",
                "billing/{controller}/{action}/{id}",
                new { controller = "BillingHome", action = "Index", id = UrlParameter.Optional },
                new[] { typeof(BillingHomeController).Namespace }
            );
        }
    }
}