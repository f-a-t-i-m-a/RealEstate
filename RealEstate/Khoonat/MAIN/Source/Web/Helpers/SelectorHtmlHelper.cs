using System.Web.Mvc;
using System.Web.Mvc.Html;
using JahanJooy.RealEstate.Web.Models.AgencySelector;
using JahanJooy.RealEstate.Web.Models.VicinitySelector;

namespace JahanJooy.RealEstate.Web.Helpers
{
    public  static class SelectorHtmlHelper
    {
        public static MvcHtmlString VicinitySelector(this HtmlHelper html, bool fromHomePage, string name, 
			long? searchScope = null, int maxNumberOfSelections = 1, string initialValue = "", 
			bool includeDisabled = false)
        {
            return html.Partial("_Selectors/_Vicinity/Selector", new VicinitySelectorConfiguration
            {
                MaxNumberOfSelections = maxNumberOfSelections,
                Name = name,
                SearchScope = searchScope,
                InitialValue = initialValue,
				IncludeDisabled = includeDisabled,
                FromHomePage = fromHomePage
            });
        }

        public static MvcHtmlString AgencySelector(this HtmlHelper html, string name,
            int maxNumberOfSelections = 1)
        {
            return html.Partial("_Selectors/_Agency/Selector", new AgencySelectorConfiguration
            {
                Name = name,
                MaxNumberOfSelections = maxNumberOfSelections
            });
        }
    }
}