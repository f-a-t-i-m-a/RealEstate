using System.Threading;
using OpenQA.Selenium;

namespace JahanJooy.RealEstateAgency.Test.PageObjects.Common
{
    public class RedioButtonPO
    {
        private readonly IWebElement _elem;

        public RedioButtonPO(IWebElement elem, string name, bool value)
        {
            _elem =
                elem.FindElement(By.XPath(("//input[@name='" + name + "'][@value='" + value.ToString().ToLower() + "']")));
            Thread.Sleep(2000);
        }

        public RedioButtonPO(IWebElement elem, string name, string value)
        {
            _elem =
                elem.FindElement(By.XPath(("//input[@name='" + name + "'][@value='" + value + "']")));
            Thread.Sleep(2000);
        }

        public RedioButtonPO(IWebElement elem, string name, int index)
        {
            _elem =
                elem.FindElements(By.XPath(("//input[@name='" + name + "']")))[index];
            Thread.Sleep(2000);
        }

        public void Select()
        {
            _elem.Click();
            Thread.Sleep(2000);
        }
    }
}