using System.Threading;
using Compositional.Composer;
using JahanJooy.RealEstateAgency.Test.PageObjects.Common;
using JahanJooy.RealEstateAgency.Test.PageObjects.Customer;
using JahanJooy.RealEstateAgency.Test.PageObjects.File;
using JahanJooy.RealEstateAgency.Test.PageObjects.Request;
using JahanJooy.RealEstateAgency.TestPreparationService.Util;
using JahanJooy.RealEstateAgency.Util.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Remote;

namespace JahanJooy.RealEstateAgency.Test.TestClasses
{
    [TestClass]
    public class CustomerTest : BaseTest
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
            Thread.Sleep(20000);
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
            topMenuPO.LoadCustomerListPage();
            CustomerListPO customerListPO = new CustomerListPO(Drv);
            Assert.IsTrue(customerListPO.SearchResultCount >= 1);
        }

        [TestMethod]
        public void ShowDetailsTest()
        {
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadCustomerListPage();

            CustomerListPO customerListPO = new CustomerListPO(Drv);
            customerListPO.SelectByIndex(0);

            CustomerDetailPO customerDetailPO = new CustomerDetailPO(Drv);
            Assert.IsTrue(customerDetailPO.HeaderText.Contains("جزئیات مشتری"));
        }

        [TestMethod]
        public void SearchTest()
        {
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadCustomerListPage();

            CustomerListPO customerListPO = new CustomerListPO(Drv);
            customerListPO.SetSearchName("روزبه");
            customerListPO.SetSearchPhoenNumber("88075566");
            customerListPO.ClickSearchButton();

            Assert.IsTrue(customerListPO.SearchResultCount >= 1);
        }

        [TestMethod]
        public void CreateTest()
        {
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadCustomerCreatePage();

            CustomerNewPO customerNewPO = new CustomerNewPO(Drv);
            customerNewPO.SetFullName("زهره");
            customerNewPO.SetEmail("Zohreh@yahoo.com");
            customerNewPO.SetAge("27");
            customerNewPO.SetNumber("02188097475");
            customerNewPO.ClickAddPhoneNumberButton();
            customerNewPO.SetMobile("09107742369");
            customerNewPO.SetDescription("ایشون دختر  آقای شمسایی هستند");
            customerNewPO.ClickSaveButton();

            CustomerDetailPO customerDetailPO = new CustomerDetailPO(Drv);
            Assert.IsTrue(customerDetailPO.HeaderText.Contains("جزئیات مشتری"));
        }

        [TestMethod]
        public void UpdateTest()
        {
            var number = "07499099996";

            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadCustomerListPage();

            CustomerListPO customerListPO = new CustomerListPO(Drv);
            customerListPO.SelectByIndex(0);

            CustomerDetailPO customerDetailPO = new CustomerDetailPO(Drv);
            customerDetailPO.ClickEditButton();

            CustomerEditPO customerEditPO = new CustomerEditPO(Drv);
            customerEditPO.SetEmail("liloo@yahoo.com");
            customerEditPO.SetNumber(number);
            customerEditPO.ClickSaveButton();

            customerDetailPO = new CustomerDetailPO(Drv);
            Assert.IsTrue(customerDetailPO.ContentText.Contains(number));
        }

        [TestMethod]
        public void AddNewPropertyTest()
        {
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadCustomerListPage();

            CustomerListPO customerListPO = new CustomerListPO(Drv);
            customerListPO.SetSearchName("روزبه");
            customerListPO.ClickSearchButton();
            customerListPO.SelectByIndex(0);

            CustomerDetailPO customerDetailPO = new CustomerDetailPO(Drv);
            customerDetailPO.ClickNewPropertyButton();

            FileNewPO fileNewPO = new FileNewPO(Drv);
            fileNewPO.SetUsageType(StaticEnumResources.UsageType_Agricultural);
            fileNewPO.SetPropertyType(StaticEnumResources.PropertyType_Land);
            fileNewPO.SetIntentionOfOwner(StaticEnumResources.IntentionOfOwner_ForSale);
            fileNewPO.SetEstateArea("5000");
            fileNewPO.SetEstateDirection(StaticEnumResources.EstateDirection_North);
            fileNewPO.SetPassageEdgeLength("20");
            fileNewPO.SetPriceSpecificationType(StaticEnumResources.SalePriceSpecificationType_Total);
            fileNewPO.SetPrice("987000000");
            fileNewPO.SetAddress("لواسان");
            fileNewPO.ClickSaveButton();

            topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadCustomerListPage();

            customerListPO = new CustomerListPO(Drv);
            customerListPO.SetSearchName("روزبه");
            customerListPO.ClickSearchButton();
            customerListPO.SelectByIndex(0);

            customerDetailPO = new CustomerDetailPO(Drv);
            customerDetailPO.ClickFileTab();

            Assert.IsTrue(customerDetailPO.ContentText.Contains(StaticEnumResources.PropertyType_Land));
        }

        [TestMethod]
        public void AddNewRequestTest()
        {
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadCustomerListPage();

            CustomerListPO customerListPO = new CustomerListPO(Drv);
            customerListPO.SetSearchName("روزبه");
            customerListPO.ClickSearchButton();
            customerListPO.SelectByIndex(0);

            CustomerDetailPO customerDetailPO = new CustomerDetailPO(Drv);
            customerDetailPO.ClickNewRequestButton();

            RequestNewPO requestNewPO = new RequestNewPO(Drv);
            requestNewPO.SetUsageType(StaticEnumResources.UsageType_Industrial);
            requestNewPO.SetPropertyType(StaticEnumResources.PropertyType_Factory, true);
            requestNewPO.SetIntentionOfCustomer(StaticEnumResources.IntentionOfCustomer_ForBuy);
            requestNewPO.SetTotalPrice("50000000");
            requestNewPO.ClickSaveButton();

            topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadCustomerListPage();

            customerListPO = new CustomerListPO(Drv);
            customerListPO.SetSearchName("روزبه");
            customerListPO.ClickSearchButton();
            customerListPO.SelectByIndex(0);

            customerDetailPO = new CustomerDetailPO(Drv);
            customerDetailPO.ClickRequestTab();

            Assert.IsTrue(customerDetailPO.ContentText.Contains(StaticEnumResources.PropertyType_Factory));
        }

        [TestMethod]
        public void ArchiveTest()
        {
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadCustomerListPage();

            CustomerListPO customerListPO = new CustomerListPO(Drv);
            customerListPO.SelectByIndex(0);

            CustomerDetailPO customerDetailPO = new CustomerDetailPO(Drv);
            customerDetailPO.ClickArchiveButton();
            customerDetailPO.ClickAlertOkButton();

            Assert.IsTrue(customerDetailPO.HeaderText.Contains("آرشیو"));
        }

        [TestMethod]
        public void UnArchiveTest()
        {
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadCustomerListPage();

            CustomerListPO customerListPO = new CustomerListPO(Drv);
            customerListPO.GetArchivedCustomers();
            customerListPO.SelectByIndex(0);

            CustomerDetailPO customerDetailPO = new CustomerDetailPO(Drv);
            customerDetailPO.ClickUnArchiveButton();
            customerDetailPO.ClickAlertOkButton();
            Assert.IsFalse(customerDetailPO.HeaderText.Contains("آرشیو"));
        }

        [TestMethod]
        public void DeleteTest()
        {
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadCustomerListPage();

            CustomerListPO customerListPO = new CustomerListPO(Drv);
            customerListPO.SelectByIndex(0);

            CustomerDetailPO customerDetailPO = new CustomerDetailPO(Drv);
            customerDetailPO.ClickDeleteButton();
            customerDetailPO.ClickAlertOkButton();

            Thread.Sleep(10000);

            customerListPO = new CustomerListPO(Drv);
            customerListPO.GetDeletedCustomers();

            Assert.IsTrue(customerListPO.SearchResultCount >= 1);
        }

        [TestMethod]
        public void DeleteNumberTest()
        {
            var number = "02188097475";

            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadCustomerCreatePage();

            CustomerNewPO customerNewPO = new CustomerNewPO(Drv);
            customerNewPO.ClickAddPhoneNumberButton();
            customerNewPO.SetNumber(number);
            var beforDeletion = customerNewPO.ContentText.Contains(number);

            customerNewPO.DeleteNumberByIndex(1);

            var afterDeletion = customerNewPO.ContentText.Contains(number);

            Assert.IsTrue(beforDeletion && !afterDeletion);
        }

        [TestMethod]
        public void ValidateSaveButtonWithoutFullName()
        {
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadCustomerCreatePage();

            CustomerNewPO customerNewPO = new CustomerNewPO(Drv);

            Assert.IsFalse(customerNewPO.IsSaveButtonEnabled);
        }
    }
}