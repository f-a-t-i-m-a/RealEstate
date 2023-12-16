using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using JahanJooy.RealEstateAgency.Test.PageObjects.Common;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace JahanJooy.RealEstateAgency.Test.PageObjects.Account
{
    public class MyProfilePO
    {
        private readonly IWebElement _elem;

        public MyProfilePO(RemoteWebDriver drv)
        {
            _elem = drv.FindElement(By.CssSelector(".myProfileView"));
        }

        public void ClickOnMoreDetails()
        {
            _elem.FindElement(By.XPath(".//a[text()[contains(.,'جزئیات بیشتر')]]")).Click();
            Thread.Sleep(2000);
        }

        public void SetMobileNumber(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='user.MobileNumber']")).Clear();
            _elem.FindElement(By.CssSelector("input[ng-model*='user.MobileNumber']")).SendKeys(value);
            Thread.Sleep(2000);
        }
        public void SetEmailAddress(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='user.EmailAddress']")).Clear();
            _elem.FindElement(By.CssSelector("input[ng-model*='user.EmailAddress']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetMobileVerificationSecret(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='user.MobileVerificationSecret']")).Clear();
            _elem.FindElement(By.CssSelector("input[ng-model*='user.MobileVerificationSecret']")).SendKeys(value);
            Thread.Sleep(2000);
        }
        public void SetEmailVerificationSecret(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='user.EmailVerificationSecret']")).Clear();
            _elem.FindElement(By.CssSelector("input[ng-model*='user.EmailVerificationSecret']")).SendKeys(value);
            Thread.Sleep(2000);
        }


        public void ClickAddNewMobileNumber()
        {
            _elem.FindElement(By.CssSelector(".addNewMobileNumber")).Click();
            Thread.Sleep(4000);
        }

        public void ClickAddNewEmailButton()
        {
            _elem.FindElement(By.CssSelector(".addNewEmailButton")).Click();
            Thread.Sleep(4000);
        }
        public void ClickCompleteRegistrationVerifyMobileNumberButton()
        {
            _elem.FindElement(By.CssSelector(".completeRegistrationVerifyMobileNumberButton")).Click();
            Thread.Sleep(4000);
        }

        public void ClickCompleteRegistrationVerifyEmailAddressButton()
        {
            _elem.FindElement(By.CssSelector(".completeRegistrationVerifyEmailAddressButton")).Click();
            Thread.Sleep(4000);
        }

        public void ClickOnRestartMobileVerification()
        {
            _elem.FindElement(By.CssSelector(".restartMobileVerification")).Click();
            Thread.Sleep(4000);
        }
        public void ClickOnRestartEmailAddressVerification()
        {
            _elem.FindElement(By.CssSelector(".restartEmailAddressVerification")).Click();
            Thread.Sleep(4000);
        }

        public bool IsVerified(string value)
        {
            ReadOnlyCollection<IWebElement> result= null;
            foreach (var elm in _elem.FindElements(By.XPath("//span")))
            {
                if (elm.Text.Equals(value))
                    result = elm.FindElements(By.XPath(".//span[contains(@class,'tick')]"));
            }
            if (result != null && result.Count == 1)
                return true;
            else
            {
                return false;
            }

        }

        public string ContentText => _elem.Text;


    }
}