using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace JahanJooy.RealEstateAgency.Test.PageObjects.Vicinity
{
    public class VicinityEditPO
    {
        private readonly IWebElement _elem;

        public VicinityEditPO(RemoteWebDriver drv)
        {
            _elem = drv.FindElement(By.CssSelector(".editVicinityView"));
        }

        public void SetName(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='item.Name']")).Clear();
            _elem.FindElement(By.CssSelector("input[ng-model*='item.Name']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void ClickSaveButton()
        {
            _elem.FindElement(By.CssSelector(".saveButton")).Click();
            Thread.Sleep(4000);
        }
    }
}