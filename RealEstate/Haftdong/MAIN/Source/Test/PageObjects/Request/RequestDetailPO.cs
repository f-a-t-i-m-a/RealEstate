using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace JahanJooy.RealEstateAgency.Test.PageObjects.Request
{
    public class RequestDetailPO
    {
        private readonly RemoteWebDriver _drv;
        private readonly IWebElement _elem;

        public RequestDetailPO(RemoteWebDriver drv)
        {
            _drv = drv;
            _elem = _drv.FindElement(By.CssSelector(".detailRequestView"));
        }

        public void ClickEditButton()
        {
            _elem.FindElement(By.CssSelector(".editButton")).Click();
            Thread.Sleep(4000);
        }

        public void ClickArchiveButton()
        {
            _elem.FindElement(By.CssSelector(".archiveButton")).Click();
            Thread.Sleep(4000);
        }

        public void ClickUnArchiveButton()
        {
            _elem.FindElement(By.CssSelector(".unArchiveButton")).Click();
            Thread.Sleep(4000);
        }

        public void ClickAlertOkButton()
        {
            var alert = _drv.SwitchTo().ActiveElement();
            alert.FindElement(By.XPath("//button[contains(text(), 'بله')]")).Click();
            Thread.Sleep(4000);
        }

        public void ClickDeleteButton()
        {
            _elem.FindElement(By.CssSelector(".deleteButton")).Click();
            Thread.Sleep(4000);
        }

        public string HeaderText => _elem.FindElement(By.XPath("div[@class='page-header']")).Text;
        public string ContentText => _elem.FindElement(By.XPath("//div[contains(@class, 'content')]")).Text;
    }
}
