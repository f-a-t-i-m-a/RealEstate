using System;
using System.Linq;
using System.Threading;
using JahanJooy.RealEstateAgency.Test.PageObjects.Common;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace JahanJooy.RealEstateAgency.Test.PageObjects.Request
{
    public class RequestNewPO
    {
        private readonly IWebElement _elem;

        public RequestNewPO(RemoteWebDriver drv)
        {
            _elem = drv.FindElement(By.CssSelector(".newRequestView"));
        }

        public void SetUsageType(string value)
        {
            DropDownPO dropDown = new DropDownPO(_elem, "jj-select-usage-type", "request.UsageType");
            dropDown.Select(value);
        }

        public void SetPropertyType(string name, bool value)
        {
            var item = _elem.FindElements(By.XPath("//label"))
                .ToList()
                .SingleOrDefault(s => s.Text.Contains(name));

            if(item?.FindElement(By.TagName("input")) != null &&
                item.FindElement(By.TagName("input")).Selected != value)
                item.Click();

            Thread.Sleep(2000);
        }

        public void SetIntentionOfCustomer(string value)
        {
            DropDownPO dropDown = new DropDownPO(_elem, "jj-select-intention-of-customer");
            dropDown.Select(value);
        }

        public void SetTotalPrice(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='request.TotalPrice']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetNeighborhood(string value)
        {
            _elem.FindElement(By.CssSelector("textarea[ng-model*='request.Neighborhood']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetOwner(string value = null, int? number = null)
        {
            if (value == null)
            {
                throw new Exception();
            }

            DropDownPO dropDown = new DropDownPO(_elem, "jj-select-customer");
            dropDown.Search(value);
        }

        public void ClickSaveButton()
        {
            _elem.FindElement(By.CssSelector(".saveButton")).Click();
            Thread.Sleep(4000);
        }

        public void SetMortgage(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='request.Mortgage']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetRent(string value)
        {
            _elem.FindElement(By.CssSelector(".showRentPanel"))
                .FindElement(By.CssSelector("input[ng-model*='request.Rent']"))
                .SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetUnitArea(string value)
        {
            _elem.FindElement(By.CssSelector(".unitPanel"))
                .FindElement(By.CssSelector("input[ng-model*='request.UnitArea']"))
                .SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetNumberOfRooms(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='request.NumberOfRooms']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetNumberOfParkingsForUnit(string value)
        {
            _elem.FindElement(By.CssSelector(".unitPanel"))
                .FindElement(By.CssSelector("input[ng-model*='request.NumberOfParkings']"))
                .SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetUnitFloorNumber(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='request.UnitFloorNumber']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetTotalNumberOfUnits(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='request.TotalNumberOfUnits']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetNumberOfUnitsPerFloor(string value)
        {
            _elem.FindElement(By.CssSelector(".housePanel"))
                .FindElement(By.CssSelector("input[ng-model*='request.NumberOfUnitsPerFloor']"))
                .SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetTotalNumberOfFloors(string value)
        {
            _elem.FindElement(By.CssSelector("input[ng-model*='request.TotalNumberOfFloors']")).SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetBuildingAgeYearsForHouse(string value)
        {
            _elem.FindElement(By.CssSelector(".housePanel"))
                .FindElement(By.CssSelector("input[ng-model*='request.BuildingAgeYears']"))
                .SendKeys(value);
            Thread.Sleep(2000);
        }

        public void SetHouseIranianLavatoryForHouse()
        {
            _elem.FindElement(By.CssSelector(".extraHousePanel"));
            _elem.FindElement(By.XPath(".//a[text()[contains(.,'خصوصیات بیشتر')]]")).Click();
            _elem.FindElement(By.Id("HasIranianLavatory")).Click();

            Thread.Sleep(2000);
        }

        public void SetHouseIranianLavatoryForShop()
        {
            _elem.FindElement(By.CssSelector(".shopPanel"));
            _elem.FindElement(By.Id("HasIranianLavatory")).Click();

            Thread.Sleep(2000);
        }

        public void SetSwimmingPool()
        {
            _elem.FindElement(By.Id("HasSwimmingPool")).Click();
            Thread.Sleep(2000);
        }

        public bool IsSaveButtonEnabled => _elem.FindElement(By.CssSelector(".saveButton")).Enabled;
    }
}