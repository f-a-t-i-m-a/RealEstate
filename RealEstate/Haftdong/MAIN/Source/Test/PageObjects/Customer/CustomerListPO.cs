using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace JahanJooy.RealEstateAgency.Test.PageObjects.Customer
{
    public class CustomerListPO
    {
        private readonly IWebElement _elem;
        public CustomerListPO(RemoteWebDriver drv)
        {
            _elem = drv.FindElement(By.CssSelector(".customerView"));
        }

        public void SelectByIndex(int index)
        {
            _elem.FindElements(By.CssSelector("a[ui-sref^='customers.details']"))[index].Click();
            Thread.Sleep(4000);
        }

        public void SelectByName(string name)
        {
            _elem.FindElement(By.XPath("//a[contains(text(), '" + name + "')]")).Click();
            Thread.Sleep(4000);
        }

        public void GetArchivedCustomers()
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='searchInput.IsArchived']")).Click();
            Thread.Sleep(2000);
            ClickSearchButton();
        }

        public void GetDeletedCustomers()
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='searchInput.IsDeleted']")).Click();
            Thread.Sleep(2000);
            ClickSearchButton();
        }
        public void ClickSearchButton()
        {
            _elem.FindElement(By.CssSelector(".searchButton")).Click();
            Thread.Sleep(4000);
        }

        public void SetSearchName(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='searchInput.DisplayName']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetSearchPhoenNumber(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='searchInput.PhoneNumber']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public int SearchResultCount
            => _elem.FindElements(By.XPath("//table[contains(@class, 'searchResult')]/tbody/tr")).Count;
    }
}
