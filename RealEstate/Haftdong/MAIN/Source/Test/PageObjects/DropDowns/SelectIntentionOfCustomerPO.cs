using JahanJooy.RealEstateAgency.Test.PageObjects.Common;
using OpenQA.Selenium;

namespace JahanJooy.RealEstateAgency.Test.PageObjects.DropDowns
{
    public class SelectIntentionOfCustomerPO : DropDownPO
    {
        public SelectIntentionOfCustomerPO(IWebElement elem, string ngModel = null) : base(elem, "jj-select-intention-of-customer", ngModel)
        {
        }
    }
}