using System.Linq;
using System.Threading;
using JahanJooy.RealEstateAgency.Test.PageObjects.Common;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace JahanJooy.RealEstateAgency.Test.PageObjects.Vicinity
{
    public class VicinityListPO
    {
        private readonly IWebElement _elem;

        public VicinityListPO(RemoteWebDriver drv)
        {
            _elem = drv.FindElement(By.CssSelector(".vicinityView"));
        }

        public string GetNameByIndex(int index)
        {
            return _elem.FindElements(By.XPath("//td/a[contains(@ui-sref,'vicinities')]"))[index].Text;
        }

        public void CheckedByIndex(int index)
        {
            _elem.FindElements(By.CssSelector("input[ng-model='item.Selected']"))[index].Click();
            Thread.Sleep(4000);
        }

        public void CheckedByName(string name)
        {
            var elem = _elem.FindElements(By.XPath("//td/a[contains(text(), '" + name + "')]")).ToList();
            elem.ForEach(e =>
            {
                e = e.FindElement(By.XPath(".."));
                e = e.FindElement(By.XPath(".."));
                e.FindElement(By.CssSelector("input[ng-model='item.Selected']")).Click();
            });
            
            Thread.Sleep(4000);
        }

        public void SelectByIndex(int index)
        {
            _elem.FindElements(By.XPath("//td/a[contains(@ui-sref,'vicinities')]"))[index].Click();
            Thread.Sleep(4000);
        }

        public void SelectByName(string name)
        {
            _elem.FindElement(By.XPath("//td/a[contains(@ui-sref,'vicinities') and contains(text(),'" + name + "')]")).Click();
            Thread.Sleep(4000);
        }

        public void SetSearchVicinity(string value)
        {
            DropDownPO dropDown = new DropDownPO(_elem, null, "parentId");
            dropDown.Select(value);
        }

        public void SetSearchText(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='searchText']")).Clear();
            _elem.FindElement(By.CssSelector("input[ng-model*='searchText']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void ClickSearchButton()
        {
            _elem.FindElement(By.CssSelector(".searchButton")).Click();
            Thread.Sleep(4000);
        }

        public void ClickNewVicinityButton()
        {
            _elem.FindElement(By.CssSelector(".newVicinityButton")).Click();
            Thread.Sleep(4000);
        }

        public void ClickEnableButton()
        {
            _elem.FindElement(By.CssSelector(".enableButton")).Click();
            Thread.Sleep(4000);
        }

        public void ClickDisableButton()
        {
            _elem.FindElement(By.CssSelector(".disableButton")).Click();
            Thread.Sleep(4000);
        }

        public void ClickDeleteButton()
        {
            _elem.FindElement(By.CssSelector(".deleteButton")).Click();
            Thread.Sleep(4000);
        }

        public void ClickDeleteButton(string name)
        {
            var elem = _elem.FindElement(By.XPath("//td/a[contains(text(), '" + name + "')]"));
            elem = elem.FindElement(By.XPath(".."));
            elem = elem.FindElement(By.XPath(".."));
            elem.FindElement(By.CssSelector("button[ng-click^='onRemoveClick']")).Click();
            Thread.Sleep(4000);
        }

        public void ClickEditButton(string name)
        {
            var elem = _elem.FindElement(By.XPath("//td/a[contains(text(), '" + name + "')]"));
            elem = elem.FindElement(By.XPath(".."));
            elem = elem.FindElement(By.XPath(".."));
            elem.FindElement(By.CssSelector("button[ui-sref^='vicinities.edit']")).Click();
            Thread.Sleep(4000);
        }

        public void ClickAlertOkButton(RemoteWebDriver drv)
        {
            var alert = drv.SwitchTo().ActiveElement();
            alert.FindElement(By.XPath("//button[contains(text(), 'بله')]")).Click();
            Thread.Sleep(4000);
        }

        public bool IsEnabled(string name)
        {
            var elem = _elem.FindElement(By.XPath("//td/a[contains(text(), '" + name + "')]"));
            elem = elem.FindElement(By.XPath(".."));
            elem = elem.FindElement(By.XPath(".."));
            var classNames = elem.FindElement(By.CssSelector("span[jj-tristate='item.Enabled']")).GetAttribute("class");
            return classNames.Contains("tick");
        }

        public string HeaderText => _elem.FindElement(By.XPath("div[@class='page-header']")).Text;
        public string DetailText => _elem.FindElement(By.XPath("//div[contains(@class, 'vicinityDetail')]")).Text;
        public string ContentText => _elem.FindElement(By.XPath("//div[contains(@class, 'content')]")).Text;
        public int SearchResultCount
            => _elem.FindElements(By.XPath("//table[contains(@class, 'searchResult')]/tbody/tr")).Count;
    }
}