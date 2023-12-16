using System;
using System.Threading;
using JahanJooy.RealEstateAgency.Test.PageObjects.Common;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace JahanJooy.RealEstateAgency.Test.PageObjects.Contract
{
    public class ContractNewPO
    {
        private readonly IWebElement _elem;

        public ContractNewPO(RemoteWebDriver drv)
        {
            _elem = drv.FindElement(By.CssSelector(".newContractView"));
        }

        public void SetContractDate(string day, int months = 0)
        {
            DatePickerPO datePicker = new DatePickerPO(_elem, "contract.ContractDate");
            datePicker.Set(day, months);
        }

        public void SetDeliveryDate(string day, int months = 0)
        {
            DatePickerPO datePicker = new DatePickerPO(_elem, "contract.DeliveryDate");
            datePicker.Set(day, months);
        }
        public void SetProperty()
        {
            RedioButtonPO radioButton = new RedioButtonPO(_elem, "selectedSupply", 0);
            radioButton.Select();
        }

        public void SetRequest()
        {
            RedioButtonPO radioButton = new RedioButtonPO(_elem, "selectedRequest", 0);
            radioButton.Select();
        }

//        public void SetUsageType(string value)
//        {
//            DropDownPO dropDown = new DropDownPO(_elem, "jj-select-unit-usage-type", "property.UsageType");
//            dropDown.Select(value);
//        }
//
//        public void SetEstateArea(string value)
//        {
//            _elem.FindElement(By.CssSelector("input[ng-model*='property.EstateArea']")).Clear();
//            _elem.FindElement(By.CssSelector("input[ng-model*='property.EstateArea']")).SendKeys(value);
//            Thread.Sleep(2000);
//        }

        public void SetSeller(string value)
        {
            DropDownPO dropDown = new DropDownPO(_elem, "jj-select-customer", "contract.Seller");
            dropDown.Search(value);
        }

        public void SetBuyer(string value)
        {
            DropDownPO dropDown = new DropDownPO(_elem, "jj-select-customer", "contract.Buyer");
            dropDown.Search(value);
        }

        public void SetUsageType(string value)
        {
            DropDownPO dropDown = new DropDownPO(_elem, "jj-select-usage-type", "contract.UsageType");
            dropDown.Select(value);
        }

        public void UnSetUsageType()
        {
            DropDownPO dropDown = new DropDownPO(_elem, "jj-select-usage-type", "contract.UsageType");
            dropDown.UnSelect();
        }

        public void SetPropertyType(string value)
        {
            DropDownPO dropDown = new DropDownPO(_elem, null, "contract.PropertyType");
            dropDown.Select(value);
        }

        public void SetIntentionOfOwner(string value)
        {
            DropDownPO dropDown = new DropDownPO(_elem, "jj-select-intention-of-owner");
            dropDown.Select(value);
        }

        public void SetTotalPrice(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='contract.ContractTotalPrice']")).Clear();
            _elem.FindElement(By.CssSelector("input[ng-model*='contract.ContractTotalPrice']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetRent(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='contract.ContractRent']")).Clear();
            _elem.FindElement(By.CssSelector("input[ng-model*='contract.ContractRent']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetMortgage(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='contract.ContractMortgage']")).Clear();
            _elem.FindElement(By.CssSelector("input[ng-model*='contract.ContractMortgage']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void ClickNewSellerButton()
        {
            _elem.FindElement(By.CssSelector(".newSeller")).Click();
            Thread.Sleep(4000);
        }

        public void ClickNewBuyerButton()
        {
            _elem.FindElement(By.CssSelector(".newBuyer")).Click();
            Thread.Sleep(4000);
        }

        public void ClickSaveButton()
        {
            _elem.FindElement(By.CssSelector(".saveButton")).Click();
            Thread.Sleep(4000);
        }

        public bool IsSaveButtonEnabled => _elem.FindElement(By.CssSelector(".saveButton")).Enabled;
    }
}