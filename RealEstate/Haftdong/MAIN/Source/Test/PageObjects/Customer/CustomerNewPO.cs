using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace JahanJooy.RealEstateAgency.Test.PageObjects.Customer
{
    public class CustomerNewPO
    {
        private readonly IWebElement _elem;

        public CustomerNewPO(RemoteWebDriver drv)
        {
            _elem = drv.FindElement(By.CssSelector(".newCustomerView"));
        }

        public void SetFullName(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='customer.DisplayName']")).Clear();
            _elem.FindElement(By.CssSelector("input[ng-model*='customer.DisplayName']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetEmail(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='customer.Email']")).Clear();
            _elem.FindElement(By.CssSelector("input[ng-model*='customer.Email']")).SendKeys(value);
            Thread.Sleep(2000);
        }
        public void SetAge(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='customer.Age']")).Clear();
            _elem.FindElement(By.CssSelector("input[ng-model*='customer.Age']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetNumber(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='customer.Number']")).Clear();
            _elem.FindElement(By.CssSelector("input[ng-model*='customer.Number']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetCode(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='customer.Code']")).Clear();
            _elem.FindElement(By.CssSelector("input[ng-model*='customer.Code']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetMobile(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='customer.Mobile']")).Clear();
            _elem.FindElement(By.CssSelector("input[ng-model*='customer.Mobile']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetMobileCode(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='customer.MobileCode']")).Clear();
            _elem.FindElement(By.CssSelector("input[ng-model*='customer.MobileCode']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetDescription(string value)
        {
            _elem.FindElement(By.CssSelector("textarea[ng-model*='customer.Description']")).Clear();
            _elem.FindElement(By.CssSelector("textarea[ng-model*='customer.Description']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void ClickAddPhoneNumberButton()
        {
            _elem.FindElement(By.CssSelector(".addPhoneNumberButton")).Click();
            Thread.Sleep(4000);
        }

        public void ClickAddMobileNumberButton()
        {
            _elem.FindElement(By.CssSelector(".addMobileNumberButton")).Click();
            Thread.Sleep(4000);
        }

        public void ClickSaveButton()
        {
            _elem.FindElement(By.CssSelector(".saveButton")).Click();
            Thread.Sleep(4000);
        }

        public void DeleteNumberByIndex(int index)
        {
            _elem.FindElements(By.CssSelector(".deletePhoneNumber"))[index].Click();
            Thread.Sleep(4000);
        }
        public string ContentText => _elem.FindElement(By.XPath("//div[contains(@class, 'content')]")).Text;
        public bool IsSaveButtonEnabled => _elem.FindElement(By.CssSelector(".saveButton")).Enabled;
    }
}
