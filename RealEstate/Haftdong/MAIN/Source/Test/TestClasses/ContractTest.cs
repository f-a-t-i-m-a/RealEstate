using System.Threading;
using Compositional.Composer;
using JahanJooy.RealEstateAgency.Test.PageObjects.Common;
using JahanJooy.RealEstateAgency.Test.PageObjects.Contract;
using JahanJooy.RealEstateAgency.TestPreparationService.Util;
using JahanJooy.RealEstateAgency.Util.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Remote;

namespace JahanJooy.RealEstateAgency.Test.TestClasses
{
    [TestClass]
    public class ContractTest : BaseTest
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
            topMenuPO.LoadContractListPage();
            ContractListPO contractListPO = new ContractListPO(Drv);
            Assert.IsTrue(contractListPO.HeaderText.Contains("فهرست قراردادها"));
        }

        [TestMethod]
        public void ShowDetailTest()
        {
            if (!IsLogedInAsAdministrator(Drv))
                LoginAsAdministrator(Drv);

            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadContractListPage();

            ContractListPO contractListPO = new ContractListPO(Drv);
            contractListPO.SelectByIndex(0);

            ContractDetailPO contractDetailPO = new ContractDetailPO(Drv);
            Assert.IsTrue(contractDetailPO.HeaderText.Contains("جزئیات قرارداد"));
        }

        [TestMethod]
        public void CreateContractTest()
        {
            if (!IsLogedInAsAdministrator(Drv))
                LoginAsAdministrator(Drv);

            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadContractCreatePage();

            ContractNewPO contractNewPO = new ContractNewPO(Drv);
            contractNewPO.SetContractDate("۰۱");
            contractNewPO.SetDeliveryDate("۰۱", 1);
            contractNewPO.SetSeller("محمدرضا شجریان");
            contractNewPO.SetProperty();
            contractNewPO.SetBuyer("انوشیروان روحانی");
            contractNewPO.SetRequest();
            contractNewPO.ClickSaveButton();

            ContractDetailPO contractDetailPO = new ContractDetailPO(Drv);
            Assert.IsTrue(contractDetailPO.HeaderText.Contains("جزئیات قرارداد"));
        }

        [TestMethod]
        public void CreateContractWithNewCustomersTest()
        {
            if (!IsLogedInAsAdministrator(Drv))
                LoginAsAdministrator(Drv);

            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadContractCreatePage();

            ContractNewPO contractNewPO = new ContractNewPO(Drv);
            contractNewPO.SetContractDate("۰۱");
            contractNewPO.SetDeliveryDate("۰۱", 1);
            contractNewPO.ClickNewSellerButton();

            ContractCustomerNewPO contractCustomerNewPO = new ContractCustomerNewPO(Drv);
            contractCustomerNewPO.SetFullName("صادق");
            contractCustomerNewPO.ClickSaveButton();

            contractNewPO = new ContractNewPO(Drv);
            contractNewPO.ClickNewBuyerButton();

            contractCustomerNewPO = new ContractCustomerNewPO(Drv);
            contractCustomerNewPO.SetFullName("جلال");
            contractCustomerNewPO.ClickSaveButton();

            contractNewPO = new ContractNewPO(Drv);
            contractNewPO.SetUsageType(StaticEnumResources.UsageType_Office);
            contractNewPO.SetPropertyType(StaticEnumResources.PropertyType_OfficialResidency);
            contractNewPO.SetIntentionOfOwner(StaticEnumResources.IntentionOfOwner_ForSale);
            contractNewPO.SetTotalPrice("200000000");
            contractNewPO.ClickSaveButton();

            ContractDetailPO contractDetailPO = new ContractDetailPO(Drv);
            Assert.IsTrue(contractDetailPO.HeaderText.Contains("جزئیات قرارداد"));
        }

        [TestMethod]
        public void SearchTest()
        {
            if (!IsLogedInAsAdministrator(Drv))
                LoginAsAdministrator(Drv);

            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadContractListPage();

            ContractListPO contractListPO = new ContractListPO(Drv);
            contractListPO.SetSearchUsageType(StaticEnumResources.UsageType_Residency);
            contractListPO.SetSearchPropertyType(StaticEnumResources.PropertyType_Suite);
            contractListPO.SetSearchState(StaticEnumResources.ContractState_Agreement);
            contractListPO.ClickSearchButton();
            Assert.IsTrue(contractListPO.SearchResultCount >= 1);
        }

        [TestMethod]
        public void UpdateTest()
        {
            if (!IsLogedInAsAdministrator(Drv))
                LoginAsAdministrator(Drv);

            var newOwnershipEvidenceSerialNumber = "159263487";
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadContractListPage();

            ContractListPO contractListPO = new ContractListPO(Drv);
            contractListPO.SelectByIndex(0);

            ContractDetailPO contractDetailPO = new ContractDetailPO(Drv);
            contractDetailPO.ClickEditButton();

            ContractEditPO contractEditPO = new ContractEditPO(Drv);
            contractEditPO.SetOwnershipEvidenceSerialNumber(newOwnershipEvidenceSerialNumber);
            contractEditPO.ClickSaveButton();

            contractDetailPO = new ContractDetailPO(Drv);
            Assert.IsTrue(contractDetailPO.ContentText.Contains(newOwnershipEvidenceSerialNumber));
        }

        [TestMethod]
        public void CancelTest()
        {
            if (!IsLogedInAsAdministrator(Drv))
                LoginAsAdministrator(Drv);

            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadContractListPage();

            ContractListPO contractListPO = new ContractListPO(Drv);
            contractListPO.SelectByIndex(0);

            ContractDetailPO contractDetailPO = new ContractDetailPO(Drv);
            contractDetailPO.ClickCancelButton();
            contractDetailPO.ClickAlertOkButton();
            Assert.IsTrue(contractDetailPO.ContentText.Contains("فسخ شده"));
        }

        [TestMethod]
        public void CloseTest()
        {
            if (!IsLogedInAsAdministrator(Drv))
                LoginAsAdministrator(Drv);

            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadContractListPage();

            ContractListPO contractListPO = new ContractListPO(Drv);
            contractListPO.SelectByIndex(0);

            ContractDetailPO contractDetailPO = new ContractDetailPO(Drv);
            contractDetailPO.ClickCloseButton();
            contractDetailPO.ClickAlertOkButton();
            Assert.IsTrue(contractDetailPO.ContentText.Contains("بسته شده"));
        }

        [TestMethod]
        public void ValidateCloseButtonAbilityAfterCancelingTest()
        {
            if (!IsLogedInAsAdministrator(Drv))
                LoginAsAdministrator(Drv);

            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadContractListPage();

            ContractListPO contractListPO = new ContractListPO(Drv);
            contractListPO.GetCanceledContracts();
            contractListPO.SelectByIndex(0);

            ContractDetailPO contractDetailPO = new ContractDetailPO(Drv);
            Assert.IsFalse(contractDetailPO.IsCloseButtonEnabled);
        }
    }
}