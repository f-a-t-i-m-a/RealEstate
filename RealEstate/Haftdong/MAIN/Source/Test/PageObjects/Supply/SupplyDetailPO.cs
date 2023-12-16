using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace JahanJooy.RealEstateAgency.Test.PageObjects.Supply
{
    public class SupplyDetailPO
    {
        private readonly IWebElement _elem;

        public SupplyDetailPO(RemoteWebDriver drv)
        {
            _elem = drv.FindElement(By.CssSelector(".detailSupplyView"));
        }

        public string HeaderText => _elem.FindElement(By.XPath("div[@class='page-header']")).Text;
        public string ContentText => _elem.FindElement(By.XPath("//div[contains(@class, 'content')]")).Text;
    }
}