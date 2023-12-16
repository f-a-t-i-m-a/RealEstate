using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace JahanJooy.RealEstateAgency.Test.PageObjects.Contract
{
    public class ContractCustomerNewPO
    {
        private readonly IWebElement _elem;

        public ContractCustomerNewPO(RemoteWebDriver drv)
        {
            _elem = drv.SwitchTo().ActiveElement();
        }

        public void SetFullName(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='customer.DisplayName']")).Clear();
            _elem.FindElement(By.CssSelector("input[ng-model*='customer.DisplayName']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void ClickSaveButton()
        {
            _elem.FindElement(By.CssSelector(".saveButton")).Click();
            Thread.Sleep(4000);
        }
    }
}