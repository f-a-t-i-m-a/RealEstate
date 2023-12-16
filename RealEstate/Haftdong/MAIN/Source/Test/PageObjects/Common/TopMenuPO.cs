using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace JahanJooy.RealEstateAgency.Test.PageObjects.Common
{
    public class TopMenuPO
    {
        private readonly RemoteWebDriver _drv;

        public TopMenuPO(RemoteWebDriver drv)
        {
            _drv = drv;
        }

        public void LoadFileListPage()
        {
            _drv.FindElementByCssSelector(".fileMenu").Click();
            Thread.Sleep(1000);

            _drv.FindElementByCssSelector("a[ui-sref='files']").Click();
            Thread.Sleep(4000);
        }

        public void LoadFileCreatePage()
        {
            _drv.FindElementByCssSelector(".fileMenu").Click();
            Thread.Sleep(1000);

            _drv.FindElementByCssSelector("a[ui-sref='files.new']").Click();
            Thread.Sleep(4000);
        }

        public void LoadMyProfilePage()
        {
            _drv.FindElement(By.CssSelector(".logindropdown")).Click();
            Thread.Sleep(1000);

            _drv.FindElement(By.Id("myProfile")).Click();
           
            Thread.Sleep(4000);
        }

        public void LoadContractListPage()
        {
            _drv.FindElementByCssSelector(".contractMenu").Click();
            Thread.Sleep(1000);

            _drv.FindElementByCssSelector("a[ui-sref='contracts']").Click();
            Thread.Sleep(4000);
        }

        public void LoadContractCreatePage()
        {
            _drv.FindElementByCssSelector(".contractMenu").Click();
            Thread.Sleep(1000);

            _drv.FindElementByCssSelector("a[ui-sref='contracts.new']").Click();
            Thread.Sleep(4000);
        }

        public void ClickLogout()
        {
            _drv.FindElement(By.CssSelector(".logindropdown")).Click();
            Thread.Sleep(1000);

            _drv.FindElement(By.CssSelector("a[ng-click^='logout']")).Click();
            Thread.Sleep(4000);
        }

        public void GoHome()
        {
            Thread.Sleep(2000);
            _drv.FindElement(By.CssSelector("a[class*='homeLink']")).Click();
            Thread.Sleep(5000);
        }

        public void LoadRequestListPage()
        {
            _drv.FindElementByCssSelector(".requestMenu").Click();
            Thread.Sleep(1000);

            _drv.FindElementByCssSelector("a[ui-sref='requests']").Click();
            Thread.Sleep(4000);
        }

        public void LoadRequestCreatePage()
        {
            _drv.FindElementByCssSelector(".requestMenu").Click();
            Thread.Sleep(1000);

            _drv.FindElementByCssSelector("a[ui-sref='requests.new']").Click();
            Thread.Sleep(4000);
        }

        public void GoToAdministratorPanel()
        {
            _drv.FindElement(By.CssSelector(".paneldropdown")).Click();
            Thread.Sleep(1000);

            _drv.FindElement(By.CssSelector("a[href='/adminapp']")).Click();
            Thread.Sleep(4000);
        }

        public void GoToAppPanel()
        {
            _drv.FindElement(By.CssSelector(".paneldropdown")).Click();
            Thread.Sleep(1000);

            _drv.FindElement(By.CssSelector("a[href='/app']")).Click();
            Thread.Sleep(4000);
        }

        public void LoadVicinityListPage()
        {
            _drv.FindElementByCssSelector(".managementMenu").Click();
            Thread.Sleep(1000);

            _drv.FindElementByCssSelector("a[ui-sref*='vicinities']").Click();
            Thread.Sleep(4000);
        }
        
        public void LoadCustomerListPage()
        {
            _drv.FindElementByCssSelector(".customerMenu").Click();
            Thread.Sleep(1000);

            _drv.FindElementByCssSelector("a[ui-sref='customers']").Click();
            Thread.Sleep(4000);
        }

        public void LoadCustomerCreatePage()
        {
            _drv.FindElementByCssSelector(".customerMenu").Click();
            Thread.Sleep(1000);

            _drv.FindElementByCssSelector("a[ui-sref='customers.new']").Click();
            Thread.Sleep(4000);
        }

        public string MainMenuText => _drv.FindElement(By.CssSelector("#layout-main-navbar-collapse")).Text;
    }
}