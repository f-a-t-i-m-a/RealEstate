using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace JahanJooy.RealEstateAgency.Test.PageObjects.Contract
{
    public class ContractDetailPO
    {
        private readonly RemoteWebDriver _drv;
        private readonly IWebElement _elem;

        public ContractDetailPO(RemoteWebDriver drv)
        {
            _drv = drv;
            _elem = _drv.FindElement(By.CssSelector(".detailContractView"));
        }

        public void ClickEditButton()
        {
            _elem.FindElement(By.CssSelector(".editButton")).Click();
            Thread.Sleep(4000);
        }

        public void ClickCancelButton()
        {
            _elem.FindElement(By.CssSelector(".cancelButton")).Click();
            Thread.Sleep(4000);
        }

        public void ClickCloseButton()
        {
            _elem.FindElement(By.CssSelector(".closeButton")).Click();
            Thread.Sleep(4000);
        }

        public void ClickAlertOkButton()
        {
            var alert = _drv.SwitchTo().ActiveElement();
            alert.FindElement(By.XPath("//button[contains(text(), 'بله')]")).Click();
            Thread.Sleep(4000);
        }

        public string HeaderText => _elem.FindElement(By.XPath("div[@class='page-header']")).Text;
        public string ContentText => _elem.FindElement(By.XPath("//div[contains(@class, 'content')]")).Text;
        public bool IsCloseButtonEnabled => _elem.FindElement(By.CssSelector(".closeButton")).Displayed;
    }
}