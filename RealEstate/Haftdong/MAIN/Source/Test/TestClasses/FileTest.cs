using System.Threading;
using Compositional.Composer;
using JahanJooy.RealEstateAgency.Test.PageObjects.Common;
using JahanJooy.RealEstateAgency.Test.PageObjects.File;
using JahanJooy.RealEstateAgency.Test.PageObjects.Supply;
using JahanJooy.RealEstateAgency.TestPreparationService.Util;
using JahanJooy.RealEstateAgency.Util.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Remote;

namespace JahanJooy.RealEstateAgency.Test.TestClasses
{
    [TestClass]
    public class FileTest : BaseTest
    {
        public static RemoteWebDriver Drv;

        [ComponentPlug]
        public DataPrepration DataPrepration { get; set; }

        #region TestPrepration

        [ClassInitialize]
        public static void ClassInitialize(TestContext tc)
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
        }

        [TestCleanup]
        public void TestCleanup()
        {
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
            topMenuPO.LoadFileListPage();
            FileListPO filePageObject = new FileListPO(Drv);
            Assert.IsTrue(filePageObject.SearchResultCount >= 1);
        }

        [TestMethod]
        public void ShowDetailsTest()
        {
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadFileListPage();

            FileListPO fileListPO = new FileListPO(Drv);
            fileListPO.SelectByIndex(0);

            FileDetailPO fileDetailPO = new FileDetailPO(Drv);
            Assert.IsTrue(fileDetailPO.HeaderText.Contains("جزئیات ملک"));
        }

        [TestMethod]
        public void CreateOfficialLandForSaleTest()
        {
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadFileCreatePage();

            FileNewPO fileNewPO = new FileNewPO(Drv);
            fileNewPO.SetUsageType(StaticEnumResources.UsageType_Office);
            fileNewPO.SetPropertyType(StaticEnumResources.PropertyType_Land);
            fileNewPO.SetIntentionOfOwner(StaticEnumResources.IntentionOfOwner_ForSale);
            fileNewPO.SetEstateArea("1000");
            fileNewPO.SetEstateDirection(StaticEnumResources.EstateDirection_North);
            fileNewPO.SetPassageEdgeLength("20");
            fileNewPO.SetEstateVoucherType(StaticEnumResources.EstateVoucherType_Normal);
            fileNewPO.SetPriceSpecificationType(StaticEnumResources.SalePriceSpecificationType_Total);
            fileNewPO.SetPrice("123000000");
            fileNewPO.SetAddress("tehran");
            fileNewPO.SetNewOwner("سیما خدیو", "09124901829");
            fileNewPO.ClickSaveButton();

            FileDetailPO fileDetailPO = new FileDetailPO(Drv);
            Assert.IsTrue(fileDetailPO.HeaderText.Contains("جزئیات ملک"));
        }

        [TestMethod]
        public void CreateResidencyApartmentForRentTest()
        {
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadFileCreatePage();

            FileNewPO fileNewPO = new FileNewPO(Drv);
            fileNewPO.SetUsageType(StaticEnumResources.UsageType_Residency);
            fileNewPO.SetPropertyType(StaticEnumResources.PropertyType_Apartment);
            fileNewPO.SetIntentionOfOwner(StaticEnumResources.IntentionOfOwner_ForRent);
            fileNewPO.SetUnitArea("100");
            fileNewPO.SetNumberOfRooms("2");
            fileNewPO.SetNumberOfParkings("1");
            fileNewPO.ClickOnMoreDetails();
            fileNewPO.SetFloorCoverType(StaticEnumResources.FloorCoverType_Stone);
            fileNewPO.SetBuildingFaceType(StaticEnumResources.BuildingFaceType_Brick);
            fileNewPO.SetHouseIranianLavatory(true);
            fileNewPO.SetHouseForeignLavatory(false);
            fileNewPO.SetMortgage("50000000000");
            fileNewPO.SetRent("10000000");
            fileNewPO.SetAddress("rasht");
            fileNewPO.SetNewOwner("سیما خدیو", "09124901829");
            fileNewPO.ClickSaveButton();

            FileDetailPO fileDetailPO = new FileDetailPO(Drv);
            Assert.IsTrue(fileDetailPO.HeaderText.Contains("جزئیات ملک"));
        }

        [TestMethod]
        public void SearchTest()
        {
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadFileListPage();

            FileListPO fileListPO = new FileListPO(Drv);
            fileListPO.ClickOnSearchDetails();
            fileListPO.SetSearchUsageType(StaticEnumResources.UsageType_Residency);
            fileListPO.SetSearchPropertyType(StaticEnumResources.PropertyType_Land);
            fileListPO.SetSearchState(StaticEnumResources.SupplyState_New);
            fileListPO.ClickSearchButton();
            Assert.IsTrue(fileListPO.SearchResultCount >= 1);
        }

        [TestMethod]
        public void UpdateTest()
        {
            var newLicencePlateValue = "654987321";

            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadFileListPage();

            FileListPO fileListPO = new FileListPO(Drv);
            fileListPO.SelectByIndex(0);

            FileDetailPO fileDetailPO = new FileDetailPO(Drv);
            fileDetailPO.ClickEditButton();

            FileEditPO fileEditPO = new FileEditPO(Drv);
            fileEditPO.SetLicencePlate(newLicencePlateValue);
            fileEditPO.ClickSaveButton();

            fileDetailPO = new FileDetailPO(Drv);
            Assert.IsTrue(fileDetailPO.ContentText.Contains(newLicencePlateValue));
        }

        [TestMethod]
        public void ArchiveTest()
        {
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadFileListPage();

            FileListPO fileListPO = new FileListPO(Drv);
            fileListPO.SelectByIndex(0);

            FileDetailPO fileDetailPO = new FileDetailPO(Drv);
            fileDetailPO.ClickArchiveButton();
            fileDetailPO.ClickAlertOkButton();

            Assert.IsTrue(fileDetailPO.HeaderText.Contains("آرشیو"));
        }

        [TestMethod]
        public void UnArchiveTest()
        {
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadFileListPage();

            FileListPO fileListPO = new FileListPO(Drv);
            fileListPO.ClickOnSearchDetails();
            fileListPO.GetArchivedSupplies();
            fileListPO.SelectByName("ملک تجاری");

            FileDetailPO fileDetailPO = new FileDetailPO(Drv);
            fileDetailPO.ClickUnArchiveButton();
            fileDetailPO.ClickAlertOkButton();
            Assert.IsFalse(fileDetailPO.HeaderText.Contains("آرشیو"));
        }

        [TestMethod]
        public void DeleteTest()
        {
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadFileListPage();

            FileListPO fileListPO = new FileListPO(Drv);
            fileListPO.ClickOnSearchDetails();
            fileListPO.SetSearchState(StaticEnumResources.SupplyState_New);
            fileListPO.ClickSearchButton();
            fileListPO.SelectByIndex(0);

            FileDetailPO fileDetailPO = new FileDetailPO(Drv);
            fileDetailPO.ClickDeleteButton();
            fileDetailPO.ClickAlertOkButton();

            Thread.Sleep(10000);

            fileListPO = new FileListPO(Drv);
            fileListPO.SetSearchState(StaticEnumResources.SupplyState_Deleted);
            fileListPO.ClickSearchButton();
            Assert.IsTrue(fileListPO.SearchResultCount >= 1);
        }

        [TestMethod]
        public void ValidateSaveButtonAbilityWithoutUsageTypeTest()
        {
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadFileCreatePage();

            FileNewPO fileNewPO = new FileNewPO(Drv);
            fileNewPO.SetIntentionOfOwner(StaticEnumResources.IntentionOfOwner_ForRent);
            Assert.IsFalse(fileNewPO.IsSaveButtonEnabled);
        }

        [TestMethod]
        public void ValidateSaveButtonAbilityWithoutPropertyTypeTest()
        {
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadFileCreatePage();

            FileNewPO fileNewPO = new FileNewPO(Drv);
            fileNewPO.SetUsageType(StaticEnumResources.UsageType_Residency);
            fileNewPO.SetIntentionOfOwner(StaticEnumResources.IntentionOfOwner_ForRent);
            Assert.IsFalse(fileNewPO.IsSaveButtonEnabled);
        }

        [TestMethod]
        public void ValidateSaveButtonAbilityWithoutIntentionOfOwnerTest()
        {
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadFileCreatePage();

            FileNewPO fileNewPO = new FileNewPO(Drv);
            fileNewPO.SetUsageType(StaticEnumResources.UsageType_Residency);
            fileNewPO.SetPropertyType(StaticEnumResources.PropertyType_Apartment);
            Assert.IsFalse(fileNewPO.IsSaveButtonEnabled);
        }

        [TestMethod]
        public void ValidatePropertyTypeDropDownAbilityWithoutUsageTypeTest()
        {
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadFileCreatePage();

            FileNewPO fileNewPO = new FileNewPO(Drv);
            var beforeChange = fileNewPO.PropertyTypeDropDown.IsEnabled;

            fileNewPO.SetUsageType(StaticEnumResources.UsageType_Shop);
            var duringChange = fileNewPO.PropertyTypeDropDown.IsEnabled;

            fileNewPO.UnSetUsageType();
            var afterChange = fileNewPO.PropertyTypeDropDown.IsEnabled;
            Assert.IsFalse(beforeChange & !duringChange & afterChange);
        }

        [TestMethod]
        public void CreateSupplyForSaleTest()
        {
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadFileListPage();

            FileListPO fileListPO = new FileListPO(Drv);
            fileListPO.ClickOnSearchDetails();
            fileListPO.SetSearchState(StaticEnumResources.SupplyState_New);
            fileListPO.ClickSearchButton();
            fileListPO.SelectByIndex(0);

            FileDetailPO fileDetailPO = new FileDetailPO(Drv);
            var beforeCount = fileDetailPO.SuppliesNumber;
            fileDetailPO.AddNewSupply();

            SupplyNewPO supplyNewPO = new SupplyNewPO(Drv);
            supplyNewPO.SetIntentionOfOwner(StaticEnumResources.IntentionOfOwner_ForSale);
            supplyNewPO.SetPriceSpecificationType(StaticEnumResources.SalePriceSpecificationType_Total);
            supplyNewPO.SetPrice("150000000000");
            supplyNewPO.ClickSaveButton();

            fileDetailPO = new FileDetailPO(Drv);
            var afterCount = fileDetailPO.SuppliesNumber;
            Assert.IsTrue(afterCount - beforeCount == 1);
        }

        [TestMethod]
        public void CreateSupplyForRentTest()
        {
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadFileListPage();

            FileListPO fileListPO = new FileListPO(Drv);
            fileListPO.ClickOnSearchDetails();
            fileListPO.SetSearchState(StaticEnumResources.SupplyState_New);
            fileListPO.ClickSearchButton();
            fileListPO.SelectByIndex(0);

            FileDetailPO fileDetailPO = new FileDetailPO(Drv);
            var beforeCount = fileDetailPO.SuppliesNumber;
            fileDetailPO.AddNewSupply();

            SupplyNewPO supplyNewPO = new SupplyNewPO(Drv);
            supplyNewPO.SetIntentionOfOwner(StaticEnumResources.IntentionOfOwner_ForRent);
            supplyNewPO.SetMortgage("150000000");
            supplyNewPO.SetMonthlyRent("1500000");
            supplyNewPO.ClickSaveButton();

            fileDetailPO = new FileDetailPO(Drv);
            var afterCount = fileDetailPO.SuppliesNumber;
            Assert.IsTrue(afterCount - beforeCount == 1);
        }

        [TestMethod]
        public void UpdateSupplyTest()
        {
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadFileListPage();

            FileListPO fileListPO = new FileListPO(Drv);
            fileListPO.ClickOnSearchDetails();
            fileListPO.SetSearchState(StaticEnumResources.SupplyState_New);
            fileListPO.ClickSearchButton();
            fileListPO.SelectByIndex(0);

            FileDetailPO fileDetailPO = new FileDetailPO(Drv);
            fileDetailPO.ClickEditSupplyButton();

            SupplyEditPO supplyEditPO = new SupplyEditPO(Drv);
            supplyEditPO.SetIntentionOfOwner(StaticEnumResources.IntentionOfOwner_ForDailyRent);
            supplyEditPO.SetDailyRent("123456000");
            supplyEditPO.ClickSaveButton();

            fileDetailPO = new FileDetailPO(Drv);
            Assert.IsTrue(fileDetailPO.SuppliesText.Contains("اجاره روزانه"));
        }

        [TestMethod]
        public void ArchiveSupplyTest()
        {
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadFileListPage();

            FileListPO fileListPO = new FileListPO(Drv);
            fileListPO.ClickOnSearchDetails();
            fileListPO.SetSearchUsageType(StaticEnumResources.UsageType_Shop);
            fileListPO.SetSearchPropertyType(StaticEnumResources.PropertyType_Shop);
            fileListPO.SetSearchIntentionOfOwner(StaticEnumResources.IntentionOfOwner_ForFullMortgage);
            fileListPO.SetSearchState(StaticEnumResources.SupplyState_New);
            fileListPO.ClickSearchButton();
            fileListPO.SelectByIndex(0);

            FileDetailPO fileDetailPO = new FileDetailPO(Drv);
            fileDetailPO.ClickArchiveSupplyButton();
            fileDetailPO.ClickAlertOkButton();
            Assert.IsTrue(fileDetailPO.SuppliesText.Contains("آرشیو"));
        }

        [TestMethod]
        public void UnArchiveSupplyTest()
        {
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadFileListPage();

            FileListPO fileListPO = new FileListPO(Drv);
            fileListPO.ClickOnSearchDetails();
            fileListPO.SetSearchUsageType(StaticEnumResources.UsageType_Shop);
            fileListPO.SetSearchPropertyType(StaticEnumResources.PropertyType_CommercialResidency);
            fileListPO.SetSearchIntentionOfOwner(StaticEnumResources.IntentionOfOwner_ForFullMortgage);
            fileListPO.SetSearchState(StaticEnumResources.SupplyState_New);
            fileListPO.GetArchivedSupplies();
            fileListPO.ClickSearchButton();
            fileListPO.SelectByIndex(0);

            FileDetailPO fileDetailPO = new FileDetailPO(Drv);
            fileDetailPO.ClickUnArchiveSupplyButton();
            fileDetailPO.ClickAlertOkButton();
            Assert.IsFalse(fileDetailPO.SuppliesText.Contains("آرشیو"));
        }

        [TestMethod]
        public void DeleteSupplyTest()
        {
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadFileListPage();

            FileListPO fileListPO = new FileListPO(Drv);
            fileListPO.ClickOnSearchDetails();
            fileListPO.SetSearchUsageType(StaticEnumResources.UsageType_Shop);
            fileListPO.SetSearchPropertyType(StaticEnumResources.PropertyType_Shop);
            fileListPO.SetSearchIntentionOfOwner(StaticEnumResources.IntentionOfOwner_ForFullMortgage);
            fileListPO.SetSearchState(StaticEnumResources.SupplyState_New);
            fileListPO.ClickSearchButton();
            fileListPO.SelectByIndex(0);

            FileDetailPO fileDetailPO = new FileDetailPO(Drv);
            fileDetailPO.ClickDeleteSupplyButton();
            fileDetailPO.ClickAlertOkButton();

            Thread.Sleep(3000);
            topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadFileListPage();
            fileListPO = new FileListPO(Drv);
            fileListPO.SetSearchState(StaticEnumResources.SupplyState_Deleted);
            fileListPO.ClickSearchButton();
            Assert.IsTrue(fileListPO.SearchResultCount >= 1);
        }

        [TestMethod]
        public void ValidateSaveButtonActivationWithoutIntentionOfOwnerTest()
        {
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadFileListPage();

            FileListPO fileListPO = new FileListPO(Drv);
            fileListPO.ClickOnSearchDetails();
            fileListPO.SetSearchState(StaticEnumResources.SupplyState_New);
            fileListPO.ClickSearchButton();
            fileListPO.SelectByIndex(0);

            FileDetailPO fileDetailPO = new FileDetailPO(Drv);
            fileDetailPO.AddNewSupply();

            SupplyNewPO supplyNewPO = new SupplyNewPO(Drv);
            Assert.IsFalse(supplyNewPO.IsSaveButtonEnabled);
            supplyNewPO.ClickCancelButton();
        }

        [TestMethod]
        public void ShowPropertiesOnlyToCreatorTest()
        {
            if (IsLogedInAsAdministrator(Drv))
                Logout(Drv);
            LoginAsNormalUser(Drv, "a.farhadi", "123456");

            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadFileListPage();

            FileListPO fileListPO = new FileListPO(Drv);
            Assert.IsTrue(fileListPO.SearchResultCount.Equals(2));
        }
    }
}