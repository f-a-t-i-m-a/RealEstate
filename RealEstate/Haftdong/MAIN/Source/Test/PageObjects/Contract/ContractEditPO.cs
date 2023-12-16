using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace JahanJooy.RealEstateAgency.Test.PageObjects.Contract
{
    public class ContractEditPO
    {
        private readonly IWebElement _elem;

        public ContractEditPO(RemoteWebDriver drv)
        {
            _elem = drv.FindElement(By.CssSelector(".editContractView"));
        }

        public void SetOwnershipEvidenceSerialNumber(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='contract.OwnershipEvidenceSerialNumber']")).Clear();
            _elem.FindElement(By.CssSelector("input[ng-model*='contract.OwnershipEvidenceSerialNumber']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void ClickSaveButton()
        {
            _elem.FindElement(By.CssSelector(".saveButton")).Click();
            Thread.Sleep(4000);
        }

        public bool IsSaveButtonEnabled => _elem.FindElement(By.CssSelector(".saveButton")).Enabled;
    }
}