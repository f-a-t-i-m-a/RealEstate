using System.Threading;
using Compositional.Composer;
using JahanJooy.RealEstateAgency.Test.PageObjects.Common;
using JahanJooy.RealEstateAgency.Test.PageObjects.Vicinity;
using JahanJooy.RealEstateAgency.TestPreparationService.Util;
using JahanJooy.RealEstateAgency.Util.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Remote;

namespace JahanJooy.RealEstateAgency.Test.TestClasses
{
    [TestClass]
    public class VicinityTest : BaseTest
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

            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.GoToAdministratorPanel();
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
            if (!IsInAdministratorPanel(Drv))
                topMenuPO.GoToAdministratorPanel();
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
            topMenuPO.LoadVicinityListPage();

            VicinityListPO vicinityListPO = new VicinityListPO(Drv);
            Assert.IsTrue(vicinityListPO.HeaderText.Contains("فهرست محله ها"));
        }

        [TestMethod]
        public void ShowDetailTest()
        {
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadVicinityListPage();

            VicinityListPO vicinityListPO = new VicinityListPO(Drv);
            var vicinityName = vicinityListPO.GetNameByIndex(0);
            vicinityListPO.SelectByIndex(0);

            vicinityListPO = new VicinityListPO(Drv);
            Assert.IsTrue(vicinityListPO.DetailText.Contains(vicinityName));
        }

        [TestMethod]
        public void CreateVicinityInRootTest()
        {
            var name = "آمریکا";
            var alternativeName = "ایالات متحده آمریکا";

            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadVicinityListPage();

            VicinityListPO vicinityListPO = new VicinityListPO(Drv);
            vicinityListPO.ClickNewVicinityButton();

            VicinityNewPO vicinityNewPO = new VicinityNewPO(Drv);
            vicinityNewPO.SetVicinityType(StaticEnumResources.VicinityType_Country);
            vicinityNewPO.SetName(name);
            vicinityNewPO.SetDisplayType();
            vicinityNewPO.SetAlternativeNames(alternativeName);
            vicinityNewPO.SetWellKnownScope(StaticEnumResources.VicinityType_Country);
            vicinityNewPO.ClickSaveButton();

            vicinityListPO = new VicinityListPO(Drv);
            Assert.IsTrue(vicinityListPO.ContentText.Contains(name));
        }

        [TestMethod]
        public void CreateVicinityAsChildTest()
        {
            var root = "ایران";
            var name = "تهران";

            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadVicinityListPage();

            VicinityListPO vicinityListPO = new VicinityListPO(Drv);
            vicinityListPO.SelectByName(root);
            vicinityListPO = new VicinityListPO(Drv);
            vicinityListPO.ClickNewVicinityButton();

            VicinityNewPO vicinityNewPO = new VicinityNewPO(Drv);
            vicinityNewPO.SetVicinityType(StaticEnumResources.VicinityType_City);
            vicinityNewPO.SetName(name);
            vicinityNewPO.SetDisplayType();
            vicinityNewPO.SetAlternativeNames(name);
            vicinityNewPO.SetWellKnownScope(StaticEnumResources.VicinityType_City);
            vicinityNewPO.ClickSaveButton();

            vicinityListPO = new VicinityListPO(Drv);
            Assert.IsTrue(vicinityListPO.DetailText.Contains(root));
        }

        [TestMethod]
        public void SearchTest()
        {
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadVicinityListPage();

            VicinityListPO vicinityListPO = new VicinityListPO(Drv);
            vicinityListPO.SetSearchVicinity("همه محله ها");
            vicinityListPO.SetSearchText("ایران");
            vicinityListPO.ClickSearchButton();
            Assert.IsTrue(vicinityListPO.SearchResultCount >= 1);
        }

        [TestMethod]
        public void UpdateTest()
        {
            var name = "تست برای ویرایش کردن";
            var newName = "محله ویرایش شده";

            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadVicinityListPage();

            VicinityListPO vicinityListPO = new VicinityListPO(Drv);
            vicinityListPO.ClickEditButton(name);

            VicinityEditPO vicinityEditPO = new VicinityEditPO(Drv);
            vicinityEditPO.SetName(newName);
            vicinityEditPO.ClickSaveButton();

            vicinityListPO = new VicinityListPO(Drv);
            Assert.IsTrue(vicinityListPO.ContentText.Contains(newName));
        }

        [TestMethod]
        public void EnableTest()
        {
            var name = "تست برای فعال کردن";

            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadVicinityListPage();

            VicinityListPO vicinityListPO = new VicinityListPO(Drv);
            vicinityListPO.CheckedByName(name);
            vicinityListPO.ClickEnableButton();
            vicinityListPO.ClickAlertOkButton(Drv);

            Assert.IsTrue(vicinityListPO.IsEnabled(name));
        }

        [TestMethod]
        public void DisableTest()
        {
            var name = "تست برای غیر فعال کردن";

            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadVicinityListPage();

            VicinityListPO vicinityListPO = new VicinityListPO(Drv);
            vicinityListPO.CheckedByName(name);
            vicinityListPO.ClickDisableButton();
            vicinityListPO.ClickAlertOkButton(Drv);

            Assert.IsFalse(vicinityListPO.IsEnabled(name));
        }

        [TestMethod]
        public void DeleteTest()
        {
            var name = "تست برای حذف کردن";

            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadVicinityListPage();

            VicinityListPO vicinityListPO = new VicinityListPO(Drv);
            vicinityListPO.CheckedByName(name);
            vicinityListPO.ClickDeleteButton();
            vicinityListPO.ClickAlertOkButton(Drv);

            Assert.IsFalse(vicinityListPO.ContentText.Contains(name));
        }

        [TestMethod]
        public void DeleteOneTest()
        {
            var name = "تست برای تکی حذف کردن";

            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadVicinityListPage();

            VicinityListPO vicinityListPO = new VicinityListPO(Drv);
            vicinityListPO.ClickDeleteButton(name);
            vicinityListPO.ClickAlertOkButton(Drv);

            Assert.IsFalse(vicinityListPO.ContentText.Contains(name));
        }
    }
}