using System;
using System.Threading;
using OpenQA.Selenium;

namespace JahanJooy.RealEstateAgency.Test.PageObjects.Common
{
    public class DropDownPO
    {
        private readonly IWebElement _elem;

        public DropDownPO(IWebElement elem, string tagName = null, string ngModel = null)
        {
            if (tagName == null && ngModel == null)
            {
                throw new ArgumentNullException();
            }
            if (tagName != null && ngModel != null)
            {
                _elem =
                    elem.FindElement(
                        By.XPath("//" + tagName + "[contains(@model, '" + ngModel +
                                 "')]/div/div/span[@ng-click='$select.activate()']"));
            }
            else if (tagName != null)
            {
                _elem = elem.FindElement(By.CssSelector(tagName + " span[ng-click='$select.activate()']"));
            }
            else
            {
                _elem =
                    elem.FindElement(By.XPath("//div[contains(@ng-model, '" + ngModel + "')]/div/span[@ng-click='$select.activate()']"));
            }
        }

        public void Select(string value)
        {
            _elem.Click();
            Thread.Sleep(2000);

            _elem.FindElement(By.XPath(("//div/ul/li/div/a/span[text()[contains(., '" + value + "')]]"))).Click();
            Thread.Sleep(2000);
        }

        public void Search(string value)
        {
            _elem.Click();
            _elem.FindElement(By.XPath("./../../input[@ng-model='$select.search']")).SendKeys(value);
            Thread.Sleep(2000);

            _elem.FindElement(By.XPath(("//a[contains(@class, 'ui-select-choices-row-inner')][1]"))).Click();
            Thread.Sleep(2000);
        }

        public void Select(int number)
        {
            _elem.Click();
            Thread.Sleep(2000);

            _elem.FindElement(By.XPath("//div/ul/li/div[a][" + number + "]")).Click();
            Thread.Sleep(2000);
        }

        public void UnSelect()
        {
            _elem.FindElement(By.XPath(".//a[contains(@ng-click,'$select.clear')]")).Click();
            Thread.Sleep(2000);
        }

        public bool IsEnabled
        {
            get
            {
                if (_elem.GetAttribute("disabled") == null || _elem.GetAttribute("disabled").Equals("false"))
                    return true;
                return false;
            }
        }
    }
}