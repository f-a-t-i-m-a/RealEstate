using System.Threading;
using JahanJooy.RealEstateAgency.Test.PageObjects.Common;
using JahanJooy.RealEstateAgency.Test.PageObjects.DropDowns;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace JahanJooy.RealEstateAgency.Test.PageObjects.Request
{

    public class RequestListPO 
    {
        private readonly IWebElement _elem;

        public RequestListPO(RemoteWebDriver drv)
        {
            _elem = drv.FindElement(By.CssSelector(".requestView"));
        }

        public void SelectByIndex(int index)
        {
            _elem.FindElements(By.CssSelector("a[ui-sref^='requests.details']"))[index].Click();
            Thread.Sleep(4000);
        }

        public void SetSearchPropertyType(string value)
        {
            DropDownPO dropDown = new DropDownPO(_elem, "jj-select-property-type", "searchInput.PropertyTypeID");
            dropDown.Select(value);
        }
        public void SetSearchIntentionOfCustomer(string value)
        {
            DropDownPO dropDown = new SelectIntentionOfCustomerPO(_elem, "searchInput.IntentionOfCustomer");
            Thread.Sleep(4000);
            dropDown.Select(value);
        }

        public void GetArchivedRequests()
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='searchInput.IsArchived']")).Click();
            Thread.Sleep(2000);
            ClickSearchButton();
        }

        public void SetSearchState(string value)
        {
            DropDownPO dropDown = new DropDownPO(_elem, "jj-select-request-state", "searchInput.State");
            dropDown.Select(value);
        }

        public void ClickSearchButton()
        {
            _elem.FindElement(By.CssSelector(".searchButton")).Click();
            Thread.Sleep(4000);
        }

//        public string HeaderText => _elem.FindElement(By.XPath("div[@class='page-header']")).Text;
        public int SearchResultCount
            => _elem.FindElements(By.XPath("//table[contains(@class, 'searchResult')]/tbody/tr")).Count;
    }
}
