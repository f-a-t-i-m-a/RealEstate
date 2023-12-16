using System.Linq;
using System.Threading;
using OpenQA.Selenium;

namespace JahanJooy.RealEstateAgency.Test.PageObjects.Common
{
    public class CheckboxPO
    {
        private readonly IWebElement _elem;

        public CheckboxPO(IWebElement elem, string name)
        {
            _elem = elem.FindElements(By.XPath("//label")).ToList().SingleOrDefault(s => s.Text.Contains(name));
            Thread.Sleep(2000);
        }

        public void Click()
        {
            _elem.Click();
            Thread.Sleep(2000);
        }
    }
}