using System.Threading;
using JahanJooy.RealEstateAgency.Test.PageObjects.Common;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace JahanJooy.RealEstateAgency.Test.PageObjects.Supply
{
    public class SupplyNewPO
    {
        private readonly IWebElement _elem;

        public SupplyNewPO(RemoteWebDriver drv)
        {
            _elem = drv.FindElement(By.CssSelector(".newSupplyView"));
        }

        public void SetIntentionOfOwner(string value)
        {
            DropDownPO dropDown = new DropDownPO(_elem, "jj-select-intention-of-owner");
            dropDown.Select(value);
        }

        public void SetPriceSpecificationType(string value)
        {
            DropDownPO dropDown = new DropDownPO(_elem, null, "supply.PriceSpecificationType");
            dropDown.Select(value);
        }

        public void SetPrice(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='supply.Price']")).Clear();
            _elem.FindElement(By.CssSelector("input[ng-model*='supply.Price']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetMortgage(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='supply.Mortgage']")).Clear();
            _elem.FindElement(By.CssSelector("input[ng-model*='supply.Mortgage']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetMonthlyRent(string value)
        {
            _elem.FindElement(By.CssSelector(".monthly-rent")).Clear();
            _elem.FindElement(By.CssSelector(".monthly-rent")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetDailyRent(string value)
        {
            _elem.FindElement(By.CssSelector(".daily-rent")).Clear();
            _elem.FindElement(By.CssSelector(".daily-rent")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void ClickSaveButton()
        {
            _elem.FindElement(By.CssSelector(".saveButton")).Click();
            Thread.Sleep(4000);
        }

        public void ClickCancelButton()
        {
            _elem.FindElement(By.CssSelector(".cancelButton")).Click();
            Thread.Sleep(4000);
        }

        public bool IsSaveButtonEnabled => _elem.FindElement(By.CssSelector(".saveButton")).Enabled;
        public string HeaderText => _elem.FindElement(By.XPath("div[@class='page-header']")).Text;
        public string ContentText => _elem.FindElement(By.XPath("//div[contains(@class, 'content')]")).Text;
    }
}