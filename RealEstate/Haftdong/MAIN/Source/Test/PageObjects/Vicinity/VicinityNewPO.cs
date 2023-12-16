using System.Threading;
using JahanJooy.RealEstateAgency.Test.PageObjects.Common;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace JahanJooy.RealEstateAgency.Test.PageObjects.Vicinity
{
    public class VicinityNewPO
    {
        private readonly IWebElement _elem;

        public VicinityNewPO(RemoteWebDriver drv)
        {
            _elem = drv.FindElement(By.CssSelector(".newVicinityView"));
        }

        public void SetVicinityType(string value)
        {
            DropDownPO dropDown = new DropDownPO(_elem, "jj-select-vicinity-type", "item.Type");
            dropDown.Select(value);
        }

        public void SetWellKnownScope(string value)
        {
            DropDownPO dropDown = new DropDownPO(_elem, "jj-select-vicinity-type", "item.WellKnownScope");
            dropDown.Select(value);
        }

        public void SetName(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='item.Name']")).Clear();
            _elem.FindElement(By.CssSelector("input[ng-model*='item.Name']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetAlternativeNames(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='item.AlternativeNames']")).Clear();
            _elem.FindElement(By.CssSelector("input[ng-model*='item.AlternativeNames']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetDisplayType()
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='item.ShowTypeInTitle']")).Click();
            Thread.Sleep(2000);
        }

        public void ClickSaveButton()
        {
            _elem.FindElement(By.CssSelector(".saveButton")).Click();
            Thread.Sleep(4000);
        }
    }
}