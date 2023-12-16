using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using Compositional.Composer;
using JahanJooy.RealEstateAgency.Api.App.Models.Session;
using JahanJooy.RealEstateAgency.TestPreparationService.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceStack;

namespace JahanJooy.RealEstateAgency.Test.APITestClasses
{
    [TestClass]
    public class ApiSessionTest : ApiBaseClass
    {
        public HttpClient Client;

        [ComponentPlug]
        public DataPrepration DataPrepration { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            SetupComposer();
            Composer.InitializePlugs(this);
            Composer.GetComponent<DataPrepration>();

            Client = new HttpClient {BaseAddress = new Uri(BaseAddress)};
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            LoginAsAdministrator(Client);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Client.Dispose();
            DataPrepration.RebuildData();
            Thread.Sleep(20000);
        }

        [TestMethod]
        public void StartSessionTest()
        {
            var startInput = new StartSessionInput
            {
                PhoneOperator = "MCI",
                PhoneSerialNumber = "123456789",
                PhoneSubscriberId = "09125262321"
            };
            var content = new StringContent(startInput.ToJson(), Encoding.UTF8, "application/json");
            var response = Client.PostAsync(UrlPrefix + "/session/start", content).Result;
            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [TestMethod]
        public void ReportCrashTest()
        {
            var content = new StringContent("This is an error!");
            var response = Client.PostAsync(UrlPrefix + "/session/crash", content).Result;
            Assert.IsTrue(response.IsSuccessStatusCode);
        }
    }
}