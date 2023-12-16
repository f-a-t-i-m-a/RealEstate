using System.Threading;
using JahanJooy.RealEstateAgency.Test.PageObjects.Common;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace JahanJooy.RealEstateAgency.Test.PageObjects.File
{
    public class FileListPO
    {
        private readonly IWebElement _elem;

        public FileListPO(RemoteWebDriver drv)
        {
            _elem = drv.FindElement(By.CssSelector(".fileView"));
        }

        public void SelectByIndex(int index)
        {
            _elem.FindElements(By.CssSelector("a[ui-sref^='files.details']"))[index].Click();
            Thread.Sleep(4000);
        }

        public void SelectByName(string name)
        {
            _elem.FindElement(By.XPath("//td/a[contains(@ui-sref,'files.details') and contains(text(),'" + name + "')]")).Click();
            Thread.Sleep(4000);
        }

        public void ClickOnSearchDetails()
        {
            _elem.FindElement(By.XPath(".//a[text()[contains(.,'جستجو')]]")).Click();
            Thread.Sleep(2000);
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

        public void SetSearchIntentionOfOwner(string value)
        {
            DropDownPO dropDown = new DropDownPO(_elem, null, "searchInput.IntentionOfOwner");
            dropDown.Select(value);
        }

        public void SetSearchState(string value)
        {
            DropDownPO dropDown = new DropDownPO(_elem, "jj-select-supply-state", "searchInput.State");
            dropDown.Select(value);
        }

        public void GetArchivedSupplies()
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='searchInput.IsArchived']")).Click();
            Thread.Sleep(2000);
            ClickSearchButton();
        }

        public void ClickSearchButton()
        {
            _elem.FindElement(By.CssSelector(".searchButton")).Click();
            Thread.Sleep(4000);
        }

        public string ContentText => _elem.FindElement(By.XPath("//div[contains(@class, 'content')]")).Text;
        public int SearchResultCount
            => _elem.FindElements(By.XPath("//table[contains(@class, 'searchResult')]/tbody/tr")).Count;
    }
}