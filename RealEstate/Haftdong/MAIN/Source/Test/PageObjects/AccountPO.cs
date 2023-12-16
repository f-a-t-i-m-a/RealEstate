using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace JahanJooy.RealEstateAgency.Test.PageObjects
{
    public class AccountPO
    {
        private readonly RemoteWebDriver _drv;

        public AccountPO(RemoteWebDriver drv)
        {
            _drv = drv;
        }

        public void SetUserName(string value)
        {
            _drv.FindElement(By.Id("txtUserName")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetPassword(string value)
        {
            _drv.FindElement(By.Id("txtPassword")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void ClickLoginButton()
        {
            _drv.FindElementByCssSelector("form[name='frmLogin'] button[type='submit']").Click();
            Thread.Sleep(4000);
        }

        public void ClickSignUpButton()
        {
            _drv.FindElement(By.CssSelector(".signUpButton")).Click();
            Thread.Sleep(4000);
        }
    }
}