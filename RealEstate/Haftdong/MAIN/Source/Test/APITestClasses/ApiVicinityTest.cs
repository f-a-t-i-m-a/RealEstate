using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using Compositional.Composer;
using JahanJooy.RealEstateAgency.Api.App.Models.Vicinity;
using JahanJooy.RealEstateAgency.TestPreparationService.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceStack;

namespace JahanJooy.RealEstateAgency.Test.APITestClasses
{
    [TestClass]
    public class ApiVicinityTest : ApiBaseClass
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
        public void SearchTest()
        {
            var startInput = new AppSearchVicinityInput
            {
                SearchText = "ایران"
            };
            var content = new StringContent(startInput.ToJson(), Encoding.UTF8, "application/json");
            var response = Client.PostAsync(UrlPrefix + "/vicinities/search", content).Result;
            var body = response.Content.ReadAsStringAsync().Result;
            var output = body.FromJson<AppSearchVicinityOutput>();
            Assert.IsTrue(output.VicinityPagedList.PageItems.Count >= 1);
        }

        [TestMethod]
        public void CorrectListTest()
        {
            var startInput = new AppSearchVicinityInput
            {
                SearchText = "تانزانیا"
            };
            var content = new StringContent(startInput.ToJson(), Encoding.UTF8, "application/json");
            var response = Client.PostAsync(UrlPrefix + "/vicinities/search", content).Result;
            var body = response.Content.ReadAsStringAsync().Result;
            var output = body.FromJson<AppSearchVicinityOutput>();
            Assert.IsTrue(output.ToJson().Equals(VicinityListSample));
        }

        private readonly string VicinityListSample =
            "{\"VicinityPagedList\":{\"PageItems\":[{\"ID\":\"577b8077b615241b4818ae8d\",\"Name\":\"تانزانیا\",\"AlternativeNames\":\"تانزانیا\",\"AdditionalSearchText\":\"تانزانیا\",\"Enabled\":true,\"Order\":0,\"Type\":\"Country\",\"WellKnownScope\":\"Country\",\"ShowTypeInTitle\":true,\"ShowInHierarchy\":false,\"ShowInSummary\":false,\"CanContainPropertyRecords\":true,\"Children\":[]}],\"PageNumber\":1,\"TotalNumberOfPages\":1,\"TotalNumberOfItems\":1}}";
    }
}