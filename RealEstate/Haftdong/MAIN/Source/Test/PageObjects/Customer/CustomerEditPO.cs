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
    public class CustomerEditPO
    {
        private readonly IWebElement _elem;

        public CustomerEditPO(RemoteWebDriver drv)
        {
            _elem = drv.FindElement(By.CssSelector(".editCustomerView"));
        }
        public void SetEmail(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='customer.Email']")).Clear();
            _elem.FindElement(By.CssSelector("input[ng-model*='customer.Email']")).SendKeys(value);
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

        public void DeleteNumberByIndex(int index)
        {
            _elem.FindElements(By.CssSelector(".deletePhoneNumber"))[index].Click();
            Thread.Sleep(4000);
        }

        public void ClickAddPhoneNumberButton()
        {
            _elem.FindElement(By.CssSelector(".addPhoneNumberButton")).Click();
            Thread.Sleep(4000);
        }

        public void ClickSaveButton()
        {
            _elem.FindElement(By.CssSelector(".saveButton")).Click();
            Thread.Sleep(4000);
        }

      

    }
}
