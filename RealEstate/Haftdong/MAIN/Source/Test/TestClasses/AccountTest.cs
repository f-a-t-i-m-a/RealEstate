using System.Threading;
using Compositional.Composer;
using JahanJooy.RealEstateAgency.Domain.Users;
using JahanJooy.RealEstateAgency.Test.PageObjects;
using JahanJooy.RealEstateAgency.Test.PageObjects.Account;
using JahanJooy.RealEstateAgency.Test.PageObjects.Common;
using JahanJooy.RealEstateAgency.TestPreparationService.Util;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using OpenQA.Selenium.Remote;

namespace JahanJooy.RealEstateAgency.Test.TestClasses
{
    [TestClass]
    public class AccountTest : BaseTest
    {
        public static RemoteWebDriver Drv;

        [ComponentPlug]
        public DataPrepration DataPrepration { get; set; }

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        #region TestPrepration

        [ClassInitialize]
        public static void ClassInitialize(TestContext tc)
        {
            ManageDriverPO manageDriver = new ManageDriverPO();
            Drv = manageDriver.Startup();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            SetupComposer();
            Composer.InitializePlugs(this);
            Composer.GetComponent<DataPrepration>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            DataPrepration.RebuildData();
            Thread.Sleep(10000);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Thread.Sleep(4000);
            if (IsLogedIn(Drv))
                Logout(Drv);
            Drv.Quit();
        }

        #endregion

        [TestMethod]
        public void LoginTest()
        {
            LoginAsAdministrator(Drv);
            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            Assert.IsTrue(topMenuPO.MainMenuText.Contains("administrator"));
        }

        [TestMethod]
        public void LogoutTest()
        {
            if (!IsLogedIn(Drv))
                LoginAsAdministrator(Drv);
            Logout(Drv);

            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            Assert.IsTrue(topMenuPO.MainMenuText.Contains("کاربر مهمان"));
        }

        [TestMethod]
        public void SignUpTest()
        {
            AccountPO accountPageObject = new AccountPO(Drv);
            accountPageObject.ClickSignUpButton();

            SignUpPO signUpPO = new SignUpPO(Drv);
            signUpPO.SetUserName("gh.ghabelian");
            signUpPO.SetPassword("123456");
            signUpPO.SetDisplayName("gholi ghabeli");
            signUpPO.SetPhoneNumber("09124901829");
            signUpPO.ClickSaveButton();

            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            Assert.IsTrue(topMenuPO.MainMenuText.Contains("gh.ghabelian"));
        }

        [TestMethod]
        public void AddNewEmailContactMethodTest()
        {
            if (IsLogedInAsAdministrator(Drv))
                Logout(Drv);
            LoginAsNormalUser(Drv, "a.farhadi", "123456");

            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadMyProfilePage();

            MyProfilePO myProfilePO = new MyProfilePO(Drv);
            myProfilePO.ClickOnMoreDetails();
            myProfilePO.SetEmailAddress("a.farhadi@yahoo.com");
            myProfilePO.ClickAddNewEmailButton();

            Assert.IsTrue(myProfilePO.ContentText.Contains("قبل از اینکه بتوانیم از آدرس ایمیل"));
        }

        [TestMethod]
        public void VerifiedMobileContactMethod()
        {
            if (IsLogedInAsAdministrator(Drv))
                Logout(Drv);
            LoginAsNormalUser(Drv, "shila", "123456");

            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadMyProfilePage();

            MyProfilePO myProfilePO = new MyProfilePO(Drv);

            var userFilter = Builders<ApplicationUser>.Filter.Eq("UserName", "shila");
            ApplicationUser user =
                DbManager.ApplicationUser.Find(userFilter).SingleOrDefault();

            myProfilePO.SetMobileVerificationSecret(user.Contact.Phones[0].UserContactMethodVerification.VerificationSecret);
            myProfilePO.ClickCompleteRegistrationVerifyMobileNumberButton();

            Assert.IsTrue(myProfilePO.IsVerified("09124471086"));
        }

        [TestMethod]
        public void VerifiedEmailContactMethod()
        {

            if (IsLogedInAsAdministrator(Drv))
                Logout(Drv);
            LoginAsNormalUser(Drv, "shila", "123456");

            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadMyProfilePage();

            MyProfilePO myProfilePO = new MyProfilePO(Drv);
            myProfilePO.ClickOnMoreDetails();
            myProfilePO.SetEmailAddress("lilaleeloo@yahoo.com");
            myProfilePO.ClickAddNewEmailButton();

            var userFilter = Builders<ApplicationUser>.Filter.Eq("UserName", "shila");
            ApplicationUser user =
                DbManager.ApplicationUser.Find(userFilter).SingleOrDefault();

            myProfilePO.SetEmailVerificationSecret(user.Contact.Emails[0].UserContactMethodVerification.VerificationSecret);
            myProfilePO.ClickCompleteRegistrationVerifyEmailAddressButton();

            Assert.IsTrue(myProfilePO.IsVerified("lilaleeloo@yahoo.com"));
        }

        [TestMethod]
        public void GetAnotherSecretForMobileContactMethod()
        {

            if (IsLogedInAsAdministrator(Drv))
                Logout(Drv);
            LoginAsNormalUser(Drv, "nina", "123456");

            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadMyProfilePage();

            MyProfilePO myProfilePO = new MyProfilePO(Drv);
            myProfilePO.ClickOnRestartMobileVerification();

            var userFilter = Builders<ApplicationUser>.Filter.Eq("UserName", "nina");
            ApplicationUser user =
                DbManager.ApplicationUser.Find(userFilter).SingleOrDefault();

            myProfilePO.SetMobileVerificationSecret(user.Contact.Phones[0].UserContactMethodVerification.VerificationSecret);
            myProfilePO.ClickCompleteRegistrationVerifyMobileNumberButton();

            Assert.IsTrue(myProfilePO.IsVerified("09124471086"));
        }

        [TestMethod]
        public void GetAnotherSecretForEmailContactMethod()
        {

            if (IsLogedInAsAdministrator(Drv))
                Logout(Drv);
            LoginAsNormalUser(Drv, "nina", "123456");

            TopMenuPO topMenuPO = new TopMenuPO(Drv);
            topMenuPO.LoadMyProfilePage();

            MyProfilePO myProfilePO = new MyProfilePO(Drv);
            myProfilePO.ClickOnRestartEmailAddressVerification();

            var userFilter = Builders<ApplicationUser>.Filter.Eq("UserName", "nina");
            ApplicationUser user =
                DbManager.ApplicationUser.Find(userFilter).SingleOrDefault();


            myProfilePO.SetEmailVerificationSecret(user.Contact.Emails[0].UserContactMethodVerification.VerificationSecret);
            myProfilePO.ClickCompleteRegistrationVerifyEmailAddressButton();

            Assert.IsTrue(myProfilePO.IsVerified("lilaleeloo@yahoo.com"));
        }
    }
}