using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace JahanJooy.RealEstateAgency.Test.PageObjects.Account
{
    public class SignUpPO
    {
        private readonly IWebElement _elem;

        public SignUpPO(RemoteWebDriver drv)
        {
            _elem = drv.FindElement(By.CssSelector(".signUpView"));
        }


        public void SetUserName(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='user.UserName']")).Clear();
            _elem.FindElement(By.CssSelector("input[ng-model*='user.UserName']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetPassword(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='user.Password']")).Clear();
            _elem.FindElement(By.CssSelector("input[ng-model*='user.Password']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetDisplayName(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='user.DisplayName']")).Clear();
            _elem.FindElement(By.CssSelector("input[ng-model*='user.DisplayName']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetPhoneNumber(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='mobileNumber']")).Clear();
            _elem.FindElement(By.CssSelector("input[ng-model*='mobileNumber']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void ClickSaveButton()
        {
            _elem.FindElement(By.CssSelector(".saveButton")).Click();
            Thread.Sleep(4000);
        }
    }
}