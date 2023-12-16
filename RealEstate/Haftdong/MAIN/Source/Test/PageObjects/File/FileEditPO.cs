using System;
using System.Threading;
using JahanJooy.RealEstateAgency.Test.PageObjects.Common;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace JahanJooy.RealEstateAgency.Test.PageObjects.File
{
    public class FileEditPO
    {
        private readonly IWebElement _elem;

        public FileEditPO(RemoteWebDriver drv)
        {
            _elem = drv.FindElement(By.CssSelector(".editPropertyView"));
        }

        public void SetUsageType(string value)
        {
            DropDownPO dropDown = new DropDownPO(_elem, "jj-select-usage-type", "property.UsageType");
            dropDown.Select(value);
        }

        public void UnSetUsageType()
        {
            DropDownPO dropDown = new DropDownPO(_elem, "jj-select-usage-type", "property.UsageType");
            dropDown.UnSelect();
        }

        public void SetPropertyType(string value)
        {
            DropDownPO dropDown = new DropDownPO(_elem, null, "property.PropertyType");
            dropDown.Select(value);
        }

        public void SetIntentionOfOwner(string value)
        {
            DropDownPO dropDown = new DropDownPO(_elem, "jj-select-intention-of-owner");
            dropDown.Select(value);
        }

        public void SetEstateArea(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='property.EstateArea']")).Clear();
            _elem.FindElement(By.CssSelector("input[ng-model*='property.EstateArea']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetEstateDirection(string value)
        {
            DropDownPO dropDown = new DropDownPO(_elem, "jj-select-estate-direction");
            dropDown.Select(value);
        }

        public void SetPassageEdgeLength(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='property.PassageEdgeLength']")).Clear();
            _elem.FindElement(By.CssSelector("input[ng-model*='property.PassageEdgeLength']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetEstateVoucherType(string value)
        {
            DropDownPO dropDown = new DropDownPO(_elem, "jj-select-estate-voucher-type");
            dropDown.Select(value);
        }

        public void SetPriceSpecificationType(string value)
        {
            DropDownPO dropDown = new DropDownPO(_elem, null, "property.PriceSpecificationType");
            dropDown.Select(value);
        }

        public void SetPrice(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='property.Price']")).Clear();
            _elem.FindElement(By.CssSelector("input[ng-model*='property.Price']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetAddress(string value)
        {
            _elem.FindElement(By.CssSelector("textarea[ng-model*='property.Address']")).Clear();
            _elem.FindElement(By.CssSelector("textarea[ng-model*='property.Address']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetOwner(string value = null, int? index = null)
        {
            if (!(value == null ^ index == null))
            {
                throw new Exception();
            }

            DropDownPO dropDown = new DropDownPO(_elem, "jj-select-customer");

            if (value != null)
            {
                dropDown.Select(value);
            }
            else
            {
                dropDown.Select((int) index);
            }
        }

        public void SetUnitArea(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='property.UnitArea']")).Clear();
            _elem.FindElement(By.CssSelector("input[ng-model*='property.UnitArea']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetNumberOfRooms(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='property.NumberOfRooms']")).Clear();
            _elem.FindElement(By.CssSelector("input[ng-model*='property.NumberOfRooms']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetNumberOfParkings(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='property.NumberOfParkings']")).Clear();
            _elem.FindElement(By.CssSelector("input[ng-model*='property.NumberOfParkings']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void ClickOnMoreDetails()
        {
            _elem.FindElement(By.XPath(".//a[text()[contains(.,'خصوصیات بیشتر')]]")).Click();
            Thread.Sleep(2000);
        }

        public void SetFloorCoverType(string value)
        {
            DropDownPO dropDown = new DropDownPO(_elem, "jj-select-floor-cover-type");
            dropDown.Select(value);
        }

        public void SetBuildingFaceType(string value)
        {
            DropDownPO dropDown = new DropDownPO(_elem, "jj-select-building-face-type");
            dropDown.Select(value);
        }

        public void SetHouseIranianLavatory(bool value)
        {
            RedioButtonPO radioButton = new RedioButtonPO(_elem, "HouseHasIranianLavatory", value);
            radioButton.Select();
        }

        public void SetHouseForeignLavatory(bool value)
        {
            RedioButtonPO radioButton = new RedioButtonPO(_elem, "HouseHasForeignLavatory", value);
            radioButton.Select();
        }

        public void SetMortgage(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='property.Mortgage']")).Clear();
            _elem.FindElement(By.CssSelector("input[ng-model*='property.Mortgage']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetRent(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='property.Rent']")).Clear();
            _elem.FindElement(By.CssSelector("input[ng-model*='property.Rent']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetLicencePlate(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='property.LicencePlate']")).Clear();
            _elem.FindElement(By.CssSelector("input[ng-model*='property.LicencePlate']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void ClickSaveButton()
        {
            _elem.FindElement(By.CssSelector(".saveButton")).Click();
            Thread.Sleep(4000);
        }

        public bool IsSaveButtonEnabled => _elem.FindElement(By.CssSelector(".saveButton")).Enabled;
        public DropDownPO PropertyTypeDropDown => new DropDownPO(_elem, null, "property.PropertyType");
        public string ContentText => _elem.FindElement(By.XPath("//div[contains(@class, 'content')]")).Text;
    }
}