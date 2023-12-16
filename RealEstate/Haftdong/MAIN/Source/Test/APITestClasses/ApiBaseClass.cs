using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Compositional.Composer;
using Compositional.Composer.Utility;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceStack.Text;

namespace JahanJooy.RealEstateAgency.Test.APITestClasses
{
    [TestClass]
    public class ApiBaseClass
    {
        public static ComponentContext Composer { get; private set; }
        private static readonly ILog Log = LogManager.GetLogger(typeof(ApiBaseClass));

        public readonly string BaseAddress = "http://localhost:60515";
        public readonly string UrlPrefix = "api/app/v1";

        public void LoginAsAdministrator(HttpClient client)
        {
            var str = "grant_type=password&username=administrator&password=123456";
            try
            {
                var content = new StringContent(str, Encoding.UTF8, "application/json");
                var response = client.PostAsync("/token", content).Result;
                var body = response.Content.ReadAsStringAsync().Result;
                JsonObject jsonBody = JsonObject.Parse(body);
                var oathToken = jsonBody["access_token"];
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oathToken);
            }
            catch (Exception e)
            {
                Log.Error("Unhandled exception in ApiBaseTest class during login as Administrator  action", e);
                throw;
            }
        }

        public void LoginAsNormalUser(HttpClient client, string userName, string password)
        {
            var str = "grant_type=password&" + "username=" + userName + "&password=" + password;

            try
            {
                var content = new StringContent(str, Encoding.UTF8, "application/json");
                var response = client.PostAsync("/token", content).Result;
                var body = response.Content.ReadAsStringAsync().Result;
                JsonObject jsonBody = JsonObject.Parse(body);
                var oathToken = jsonBody["access_token"];
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oathToken);
            }
            catch (Exception e)
            {
                Log.Error("Unhandled exception in ApiBaseTest class during login as normal user action", e);
                throw;
            }
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