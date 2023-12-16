using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace JahanJooy.RealEstateAgency.Test.PageObjects.Request
{
    public class RequestEditPO
    {
        private readonly IWebElement _elem;

        public RequestEditPO(RemoteWebDriver drv)
        {
            _elem = drv.FindElement(By.CssSelector(".editRequestView"));
        }
        public void SetPropertyType(string name, bool value)
        {
            var item = _elem.FindElements(By.XPath("//label"))
                .ToList()
                .SingleOrDefault(s => s.Text.Contains(name));

            if (item?.FindElement(By.TagName("input")) != null &&
                item.FindElement(By.TagName("input")).Selected != value)
                item.Click();

            Thread.Sleep(2000);
        }

        public void ClickSaveButton()
        {
            _elem.FindElement(By.CssSelector(".saveButton")).Click();
            Thread.Sleep(4000);
        }
    }
}