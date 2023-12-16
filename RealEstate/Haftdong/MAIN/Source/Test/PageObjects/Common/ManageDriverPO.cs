using System.Threading;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace JahanJooy.RealEstateAgency.Test.PageObjects.Common
{
    public class ManageDriverPO
    {
        private const string URL = "http://localhost:60515/";
        public RemoteWebDriver Startup()
        {
            //            var drv = new InternetExplorerDriver();
            //            var drv = new FirefoxDriver();
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--disable-extensions");
            var drv = new ChromeDriver(options);
            drv.Manage().Window.Maximize();
            do
            {
                drv.Navigate().GoToUrl(URL);
            } while (!drv.Url.Contains(URL));

            
            Thread.Sleep(10000);
            return drv;
        }
    }
}