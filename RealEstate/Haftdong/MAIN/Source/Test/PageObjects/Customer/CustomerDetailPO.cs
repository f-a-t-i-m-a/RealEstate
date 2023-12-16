using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace JahanJooy.RealEstateAgency.Test.PageObjects.Customer
{
    public class CustomerDetailPO
    {

        private readonly RemoteWebDriver _drv;
        private readonly IWebElement _elem;


        public CustomerDetailPO(RemoteWebDriver drv)
        {
            _drv = drv;
            _elem = _drv.FindElement(By.CssSelector(".detailCustomerView"));
        }

        public void ClickFileTab()
        {
            _elem.FindElement(By.XPath("//a/tab-heading[contains(text(), 'فایل')]"))
                .FindElement(By.XPath("..")).Click();
            Thread.Sleep(4000);
        }

        public void ClickRequestTab()
        {
            _elem.FindElement(By.XPath("//a/tab-heading[contains(text(), 'درخواست')]"))
                .FindElement(By.XPath("..")).Click();
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

        public void ClickEditButton()
        {
            _elem.FindElement(By.CssSelector(".editButton")).Click();
            Thread.Sleep(4000);
        }

        public void ClickNewPropertyButton()
        {
            _elem.FindElement(By.CssSelector(".newPropertyButton")).Click();
            Thread.Sleep(4000);
        }

        public void ClickNewRequestButton()
        {
            _elem.FindElement(By.CssSelector(".newRequestButton")).Click();
            Thread.Sleep(4000);
        }

        public void ClickDeleteButton()
        {
            _elem.FindElement(By.CssSelector(".deleteButton")).Click();
            Thread.Sleep(4000);
        }


        public string HeaderText => _elem.FindElement(By.XPath("div[@class='page-header']")).Text;
        public string ContentText => _elem.FindElement(By.XPath("//div[contains(@class, 'content')]")).Text;
        public int SearchResultCount
         => _elem.FindElements(By.XPath("//table[contains(@class, 'searchResult')]/tbody/tr")).Count;
    }
}
