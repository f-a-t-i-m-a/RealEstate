using System.Threading;
using JahanJooy.RealEstateAgency.Test.PageObjects.Common;
using JahanJooy.RealEstateAgency.Util.Resources;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace JahanJooy.RealEstateAgency.Test.PageObjects.Contract
{
    public class ContractListPO
    {
        private readonly IWebElement _elem;

        public ContractListPO(RemoteWebDriver drv)
        {
            _elem = drv.FindElement(By.CssSelector(".contractView"));
        }

        public void SelectByIndex(int index)
        {
            _elem.FindElements(By.CssSelector("a[ui-sref^='contracts.details']"))[index].Click();
            Thread.Sleep(4000);
        }

        public void SetSearchUsageType(string value)
        {
            DropDownPO dropDown = new DropDownPO(_elem, null, "searchInput.UsageType");
            dropDown.Select(value);
        }

        public void SetSearchPropertyType(string value)
        {
            DropDownPO dropDown = new DropDownPO(_elem, null, "searchInput.PropertyType");
            dropDown.Select(value);
        }

        public void SetSearchState(string value)
        {
            DropDownPO dropDown = new DropDownPO(_elem, "jj-select-contract-state", "searchInput.State");
            dropDown.Select(value);
        }

        public void GetCanceledContracts()
        {
            DropDownPO dropDown = new DropDownPO(_elem, "jj-select-contract-state", "searchInput.State");
            dropDown.Select(StaticEnumResources.ContractState_Cancellation);

            _elem.FindElement(By.CssSelector(".searchButton")).Click();
            Thread.Sleep(4000);
        }

        public void ClickSearchButton()
        {
            _elem.FindElement(By.CssSelector(".searchButton")).Click();
            Thread.Sleep(4000);
        }

        public string HeaderText => _elem.FindElement(By.XPath("div[@class='page-header']")).Text;
        public string ContentText => _elem.FindElement(By.XPath("//div[contains(@class, 'content')]")).Text;
        public int SearchResultCount
            => _elem.FindElements(By.XPath("//table[contains(@class, 'searchResult')]/tbody/tr")).Count;
    }
}