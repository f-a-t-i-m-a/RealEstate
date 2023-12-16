using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using Compositional.Composer;
using JahanJooy.RealEstateAgency.Api.App.Models.Dashboard;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using JahanJooy.RealEstateAgency.TestPreparationService.Util;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceStack;

namespace JahanJooy.RealEstateAgency.Test.APITestClasses
{
    [TestClass]
    public class ApiDashboardTest : ApiBaseClass
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
        public void QuickSearchTest()
        {
            var searchInput = new AppQuickSearchInput
            {
                Text = "مغازه"
            };
            var content = new StringContent(searchInput.ToJson(), Encoding.UTF8, "application/json");
            var response = Client.PostAsync(UrlPrefix + "/dashboard/quicksearch", content).Result;
            var body = response.Content.ReadAsStringAsync().Result;
            var output = body.FromJson<AppQuickSearchOutput>();
            Assert.IsTrue(output.SearchResults.PageItems.Count >= 1);
        }

        [TestMethod]
        public void UserQuickSearchTest()
        {
            //login as a.farhadi
            LoginAsNormalUser(Client, "a.farhadi","123456");
           
            var searchInput = new AppQuickSearchInput
            {
                Text = "مغازه",
                StartIndex = 0, 
                PageSize= 5, 
                DataTypes = new List<EntityType> {EntityType.Property},
                SortColumn= 0, 
                SortDirection= SortDirectionType.Asc

            };
            var content = new StringContent(searchInput.ToJson(), Encoding.UTF8, "application/json");
            var response = Client.PostAsync(UrlPrefix + "/dashboard/userquicksearch", content).Result;
            var body = response.Content.ReadAsStringAsync().Result;
            var output = body.FromJson<AppQuickSearchOutput>();
            Assert.IsTrue(output.SearchResults.PageItems.Count == 1);
        }
    }
}