using System;
using Compositional.Composer;
using Compositional.Composer.Utility;
using System.Threading;
using JahanJooy.RealEstateAgency.Test.PageObjects;
using JahanJooy.RealEstateAgency.Test.PageObjects.Common;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Remote;

namespace JahanJooy.RealEstateAgency.Test.TestClasses
{
    [TestClass]
    public class BaseTest
    {
        public static ComponentContext Composer { get; private set; }
        private static readonly ILog Log = LogManager.GetLogger(typeof(BaseTest));

        public static void LoginAsAdministrator(RemoteWebDriver drv)
        {
            AccountPO accountPageObject = new AccountPO(drv);
            accountPageObject.SetUserName("administrator");
            accountPageObject.SetPassword("123456");
            accountPageObject.ClickLoginButton();
            Thread.Sleep(2000);
        }

        public static void LoginAsNormalUser(RemoteWebDriver drv, string userName, string password)
        {
            AccountPO accountPageObject = new AccountPO(drv);
            accountPageObject.SetUserName(userName);
            accountPageObject.SetPassword(password);
            accountPageObject.ClickLoginButton();
            Thread.Sleep(2000);
        }

        public static void Logout(RemoteWebDriver drv)
        {
            TopMenuPO topMenuPO = new TopMenuPO(drv);
            topMenuPO.ClickLogout();
        }

        public static bool IsLogedInAsAdministrator(RemoteWebDriver drv)
        {
            TopMenuPO topMenuPO = new TopMenuPO(drv);
            return topMenuPO.MainMenuText.Contains("administrator");
        }

        public static bool IsLogedIn(RemoteWebDriver drv)
        {
            TopMenuPO topMenuPO = new TopMenuPO(drv);
            return !topMenuPO.MainMenuText.Contains("کاربر مهمان");
        }

        public static bool IsLogedIn(RemoteWebDriver drv, string userName)
        {
            TopMenuPO topMenuPO = new TopMenuPO(drv);
            return topMenuPO.MainMenuText.Contains(userName);
        }

        public static bool IsInAdministratorPanel(RemoteWebDriver drv)
        {
            TopMenuPO topMenuPO = new TopMenuPO(drv);
            return topMenuPO.MainMenuText.Contains("پنل مدیریت");
        }
        public static void SetupComposer()
        {
            Composer = new ComponentContext();
            Composer.RegisterAssembly("JahanJooy.RealEstateAgency.Test");

            try
            {
                Composer.ProcessCompositionXmlFromResource("JahanJooy.RealEstateAgency.Test.Composition.xml");
            }
            catch (Exception e)
            {
                Log.Error("Unhandled exception in BaseTest class during setup composer action", e);
                throw;
            }

            ComposerLocalThreadBinder.Bind(Composer);
        }
    }
}