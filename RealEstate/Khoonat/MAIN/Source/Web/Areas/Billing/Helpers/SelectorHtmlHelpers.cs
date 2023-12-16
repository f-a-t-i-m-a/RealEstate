using System.Web.Mvc;
using System.Web.Mvc.Html;
using JahanJooy.RealEstate.Web.Areas.Billing.Models.AdminUserSelector;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Helpers
{
    public static class SelectorHtmlHelpers
    {
        public static MvcHtmlString AdminUserSelector(this HtmlHelper html, string name, int maxNumberOfSelections = 1)
        {
            return html.Partial("_Selectors/_User/Selector", new AdminUserSelectorConfiguration
                                                                  {
                                                                      MaxNumberOfSelections = maxNumberOfSelections,
                                                                      Name = name
                                                                  });
        }
    }
}