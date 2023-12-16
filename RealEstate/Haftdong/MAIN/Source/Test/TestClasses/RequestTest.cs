using System.Threading;
using Compositional.Composer;
using JahanJooy.RealEstateAgency.Domain.Request;
using JahanJooy.RealEstateAgency.Test.PageObjects.Common;
using JahanJooy.RealEstateAgency.Test.PageObjects.Request;
using JahanJooy.RealEstateAgency.TestPreparationService.Util;
using JahanJooy.RealEstateAgency.Util.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Remote;

namespace JahanJooy.RealEstateAgency.Test.TestClasses
{
    [TestClass]
    public class RequestTest : BaseTest
    {
        public static RemoteWebDriver Drv;

        [ComponentPlug]
        public DataPrepration DataPrepration { get; set; }

        #region TestPrepration

        [ClassInitialize]
        public static  void ClassInitialize(TestContext tc)
        {
            ManageDriverPO manageDriver = new ManageDriverPO();
            Drv = manageDriver.Startup();

            LoginAsAdministrator(Drv);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            SetupComposer();
            Composer.InitializePlugs(this);
            Composer.GetComponent<DataPrepration>();

            if (!IsLogedInAsAdministrator(Drv))
                LoginAsAdministrator(Drv);

            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            if (IsInAdministratorPanel(Drv))
                topMenuPO.GoToAppPanel();
            Thread.Sleep(10000);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Thread.Sleep(2000);
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.GoHome();
            DataPrepration.RebuildData();
            Thread.Sleep(10000);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Thread.Sleep(4000);
            Logout(Drv);
            Drv.Quit();
        }

        #endregion

        [TestMethod]
        public void ListTest()
        {
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadRequestListPage();
            RequestListPO requestListPageObject = new RequestListPO(Drv);
            Assert.IsTrue(requestListPageObject.SearchResultCount >= 1);
        }
       
        [TestMethod]
        public void ShowDetailsTest()
        {
            TopMenuPO topMenuPo = new TopMenuPO(Drv);
            topMenuPo.LoadRequestListPage();

            var requestListPo = new RequestListPO(Drv);
            requestListPo.SelectByIndex(0);

            RequestDetailPO requestDetailPo = new RequestDetailPO(Drv);
            Assert.IsTrue(requestDetailPo.HeaderText.Contains("جزئیات درخواست"));
        }
     
        [TestMethod]
        public void CreateNewRequestForResidencyMultiplePropertyTypesTest()
        {
            TopMenuPO topMenuPo = new TopMenuPO(Drv);
            topMenuPo.LoadRequestCreatePage();

            RequestNewPO requestNewPO = new RequestNewPO(Drv);
            requestNewPO.SetUsageType(StaticEnumResources.UsageType_Residency);
            requestNewPO.SetPropertyType(StaticEnumResources.PropertyType_Land, true);
            requestNewPO.SetPropertyType(StaticEnumResources.PropertyType_Apartment, true);
            requestNewPO.SetIntentionOfCustomer(StaticEnumResources.IntentionOfCustomer_ForBuy);
            requestNewPO.SetTotalPrice(_testResidencyMultiplePropertyTypes.TotalPrice.ToString());
            requestNewPO.SetOwner("Nazi");
            requestNewPO.ClickSaveButton();

            RequestDetailPO requestDetailPo = new RequestDetailPO(Drv);
            Assert.IsTrue(requestDetailPo.HeaderText.Contains("جزئیات درخواست"));
        }

        [TestMethod]
        public void CreateNewRequestForOfficeOfficialResidencyForRentTest()
        {
            TopMenuPO topMenuPo = new TopMenuPO(Drv);
            topMenuPo.LoadRequestCreatePage();

            RequestNewPO requestNewPO = new RequestNewPO(Drv);
            requestNewPO.SetUsageType(StaticEnumResources.UsageType_Office);
            requestNewPO.SetPropertyType(StaticEnumResources.PropertyType_OfficialResidency, true);
            requestNewPO.SetIntentionOfCustomer(StaticEnumResources.IntentionOfCustomer_ForRent);
            requestNewPO.SetMortgage(_testOfficeOfficialResidencyForRent.Mortgage.ToString());
            requestNewPO.SetRent(_testOfficeOfficialResidencyForRent.Rent.ToString());
            requestNewPO.SetUnitArea(_testOfficeOfficialResidencyForRent.UnitArea.ToString());
            requestNewPO.SetNumberOfRooms(_testOfficeOfficialResidencyForRent.NumberOfRooms.ToString());
            requestNewPO.SetNumberOfParkingsForUnit(_testOfficeOfficialResidencyForRent.NumberOfParkings.ToString());
            requestNewPO.SetUnitFloorNumber(_testOfficeOfficialResidencyForRent.UnitFloorNumber.ToString());
            requestNewPO.SetNumberOfUnitsPerFloor(_testOfficeOfficialResidencyForRent.NumberOfUnitsPerFloor.ToString());
            requestNewPO.SetTotalNumberOfUnits(_testOfficeOfficialResidencyForRent.TotalNumberOfUnits.ToString());
            requestNewPO.SetTotalNumberOfFloors(
                _testOfficeOfficialResidencyForRent.TotalNumberOfFloors.ToString());
            requestNewPO.SetBuildingAgeYearsForHouse(_testOfficeOfficialResidencyForRent.BuildingAgeYears.ToString());
            requestNewPO.SetHouseIranianLavatoryForHouse();
            requestNewPO.SetSwimmingPool();
            requestNewPO.SetOwner("لی لا");
            requestNewPO.ClickSaveButton();

            RequestDetailPO requestDetailPO = new RequestDetailPO(Drv);
            Assert.IsTrue(requestDetailPO.HeaderText.Contains("جزئیات درخواست"));
        }

        [TestMethod]
        public void SearchTest()
        {
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadRequestListPage();

            RequestListPO requestListPo = new RequestListPO(Drv);
            requestListPo.SetSearchPropertyType(StaticEnumResources.PropertyType_Land);
            requestListPo.SetSearchIntentionOfCustomer(StaticEnumResources.IntentionOfOwner_ForDailyRent);
            requestListPo.ClickSearchButton();
            Assert.IsTrue(requestListPo.SearchResultCount >= 1);
        }

        [TestMethod]
        public void UpdateTest()
        {
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadRequestListPage();

            RequestListPO requestListPO = new RequestListPO(Drv);
            requestListPO.SelectByIndex(0);

            RequestDetailPO requestDetailPO = new RequestDetailPO(Drv);
            requestDetailPO.ClickEditButton();

            RequestEditPO requestEditPO = new RequestEditPO(Drv);
            requestEditPO.SetPropertyType(StaticEnumResources.PropertyType_OldHouse, true);
            requestEditPO.SetPropertyType(StaticEnumResources.PropertyType_Penthouse, true);
            requestEditPO.SetPropertyType(StaticEnumResources.PropertyType_Land, false);
            requestEditPO.ClickSaveButton();

            requestDetailPO = new RequestDetailPO(Drv);
            Assert.IsTrue(requestDetailPO.ContentText.Contains(StaticEnumResources.PropertyType_OldHouse));
        }

        [TestMethod]
        public void ArchiveTest()
        {
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadRequestListPage();

            RequestListPO requestListPO = new RequestListPO(Drv);
            requestListPO.SelectByIndex(0);

            RequestDetailPO requestDetailPO = new RequestDetailPO(Drv);
            requestDetailPO.ClickArchiveButton();
            requestDetailPO.ClickAlertOkButton();

            Assert.IsTrue(requestDetailPO.HeaderText.Contains("آرشیو"));
        }

        [TestMethod]
        public void UnArchiveTest()
        {
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadRequestListPage();

            RequestListPO requestListPO = new RequestListPO(Drv);
            requestListPO.GetArchivedRequests();
            requestListPO.SelectByIndex(0);

            RequestDetailPO requestDetailPO = new RequestDetailPO(Drv);
            requestDetailPO.ClickUnArchiveButton();
            requestDetailPO.ClickAlertOkButton();
            Assert.IsFalse(requestDetailPO.HeaderText.Contains("آرشیو"));
        }

        [TestMethod]
        public void DeleteTest()
        {
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadRequestListPage();

            RequestListPO requestListPO = new RequestListPO(Drv);
            requestListPO.SetSearchState(StaticEnumResources.RequestState_New);
            requestListPO.ClickSearchButton();
            requestListPO.SelectByIndex(0);

            RequestDetailPO requestDetailPO = new RequestDetailPO(Drv);
            requestDetailPO.ClickDeleteButton();
            requestDetailPO.ClickAlertOkButton();

            Thread.Sleep(10000);

            RequestListPO requestListPo = new RequestListPO(Drv);
            requestListPo.SetSearchState(StaticEnumResources.RequestState_Deleted);
            requestListPo.ClickSearchButton();
            Assert.IsTrue(requestListPo.SearchResultCount >=1);
        }

        [TestMethod]
        public void ValidateSaveButtonAbilityWithoutUsageTypeTest()
        {
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadRequestCreatePage();

            RequestNewPO requestNewPO = new RequestNewPO(Drv);
            requestNewPO.SetIntentionOfCustomer(StaticEnumResources.IntentionOfCustomer_ForRent);
            requestNewPO.SetOwner("سیما");
            Assert.IsFalse(requestNewPO.IsSaveButtonEnabled);
        }

        [TestMethod]
        public void ValidateSaveButtonAbilityWithoutIntentionOfCustomerTest()
        {
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadRequestCreatePage();

            RequestNewPO requestNewPO = new RequestNewPO(Drv);
            requestNewPO.SetUsageType(StaticEnumResources.UsageType_Residency);
            requestNewPO.SetOwner("سیما");
            Assert.IsFalse(requestNewPO.IsSaveButtonEnabled);
        }

        [TestMethod]
        public void ValidateSaveButtonAbilityWithoutOwnerTest()
        {
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadRequestCreatePage();

            RequestNewPO requestNewPO = new RequestNewPO(Drv);
            requestNewPO.SetUsageType(StaticEnumResources.UsageType_Residency);
            requestNewPO.SetIntentionOfCustomer(StaticEnumResources.IntentionOfCustomer_ForRent);
            Assert.IsFalse(requestNewPO.IsSaveButtonEnabled);
        }

        #region Sample Data

        private readonly Request _testResidencyMultiplePropertyTypes = new Request
        {
            TotalPrice = 123000000
        };

        private readonly Request _testOfficeOfficialResidencyForRent = new Request
        {
            UnitArea = 150,
            NumberOfRooms = 2,
            NumberOfParkings = 1,
            UnitFloorNumber = 6,
            NumberOfUnitsPerFloor = 2,
            TotalNumberOfUnits = 20,
            TotalNumberOfFloors = 10,
            BuildingAgeYears = 5,
            Mortgage = 50000000000,
            Rent = 10000000
        };

        #endregion
    }
}