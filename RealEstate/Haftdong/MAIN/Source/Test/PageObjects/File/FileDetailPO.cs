using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace JahanJooy.RealEstateAgency.Test.PageObjects.File
{
    public class FileDetailPO
    {
        private readonly RemoteWebDriver _drv;
        public readonly IWebElement _elem;

        public FileDetailPO(RemoteWebDriver drv)
        {
            _drv = drv;
            _elem = _drv.FindElement(By.CssSelector(".detailFileView"));
        }

        public void AddNewSupply()
        {
            _drv.FindElement(By.CssSelector(".newSupplyButton")).Click();
            Thread.Sleep(4000);
        }

        public void ClickEditButton()
        {
            _elem.FindElement(By.CssSelector(".editButton")).Click();
            Thread.Sleep(4000);
        }

        public void ClickEditSupplyButton()
        {
            _elem.FindElements(By.CssSelector(".supplyMenu"))[0].Click();
            Thread.Sleep(1000);

            _elem.FindElements(By.CssSelector(".editSupplyButton"))[0].Click();
            Thread.Sleep(4000);
        }

        public void ClickDeleteButton()
        {
            _elem.FindElement(By.CssSelector(".deleteButton")).Click();
            Thread.Sleep(4000);
        }

        public void ClickDeleteSupplyButton()
        {
            _elem.FindElements(By.CssSelector(".supplyMenu"))[0].Click();
            Thread.Sleep(1000);

            _elem.FindElements(By.CssSelector(".deleteSupplyButton"))[0].Click();
            Thread.Sleep(4000);
        }

        public void ClickArchiveButton()
        {
            _elem.FindElement(By.CssSelector(".archiveButton")).Click();
            Thread.Sleep(4000);
        }
        public void ClickArchiveSupplyButton()
        {
            _elem.FindElements(By.CssSelector(".supplyMenu"))[0].Click();
            Thread.Sleep(1000);

            _elem.FindElements(By.CssSelector(".archiveSupplyButton"))[0].Click();
            Thread.Sleep(4000);
        }

        public void ClickUnArchiveButton()
        {
            _elem.FindElement(By.CssSelector(".unArchiveButton")).Click();
            Thread.Sleep(4000);
        }

        public void ClickUnArchiveSupplyButton()
        {
            _elem.FindElements(By.CssSelector(".supplyMenu"))[0].Click();
            Thread.Sleep(1000);

            _elem.FindElements(By.CssSelector(".unArchiveSupplyButton"))[0].Click();
            Thread.Sleep(4000);
        }

        public void ClickAlertOkButton()
        {
            var alert = _drv.SwitchTo().ActiveElement();
            alert.FindElement(By.XPath("//button[contains(text(), 'بله')]")).Click();
            Thread.Sleep(4000);
        }

        public string HeaderText => _elem.FindElement(By.XPath("div[@class='page-header']")).Text;
//        public string ContentText => _elem.FindElement(By.XPath("//div[contains(@class, 'content')]")).Text;
        public string ContentText => _elem.Text;
        public int SuppliesNumber => _elem.FindElements(By.XPath("//table[contains(@class, 'supplyTable')]/tbody/tr")).Count;
        public string SuppliesText => _elem.FindElement(By.XPath("//table[contains(@class, 'supplyTable')]/tbody/tr")).Text;
    }
}