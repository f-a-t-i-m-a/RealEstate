using System;
using System.Threading;
using OpenQA.Selenium;

namespace JahanJooy.RealEstateAgency.Test.PageObjects.Common
{
    public class DatePickerPO
    {
        private readonly IWebElement _elem;

        public DatePickerPO(IWebElement elem, string ngModel)
        {
            _elem = elem.FindElement(By.XPath("//jj-jalaali-datepicker-popup[contains(@jj-model, '" + ngModel + "')]"));
        }

        public void Set(string day, int monthsLater = 0)
        {
            _elem.FindElement(By.XPath(".//button[contains(@ng-click,'openDateFilter')]")).Click();
            Thread.Sleep(2000);

            for (int i = 0; i < monthsLater; i++)
            {
                _elem.FindElement(By.XPath(".//button[@ng-click='move(1)']")).Click();
                Thread.Sleep(1000);
            }

            _elem.FindElements(By.XPath(".//button/span[contains(text(),'" + day + "')]"))[0]
                .FindElement(By.XPath("..")).Click();
            Thread.Sleep(2000);
        }
    }
}