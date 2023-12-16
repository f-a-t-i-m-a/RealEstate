using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using Compositional.Composer;
using JahanJooy.RealEstateAgency.Api.App.Models.Customer;
using JahanJooy.RealEstateAgency.Api.App.Models.Request;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Enums.Request;
using JahanJooy.RealEstateAgency.TestPreparationService.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using ServiceStack;

namespace JahanJooy.RealEstateAgency.Test.APITestClasses
{
    [TestClass]
    public class ApiRequestTest : ApiBaseClass
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
        public void ListTest()
        {
            var searchInput = new AppSearchRequestInput();
            var content = new StringContent(searchInput.ToJson(), Encoding.UTF8, "application/json");
            var response = Client.PostAsync(UrlPrefix + "/requests/search", content).Result;
            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [TestMethod]
        public void CorrectListTest()
        {
            LoginAsNormalUser(Client, "a.kiarostami", "123456");
            var searchInput = new AppSearchRequestInput();
            var content = new StringContent(searchInput.ToJson(), Encoding.UTF8, "application/json");
            var response = Client.PostAsync(UrlPrefix + "/requests/search", content).Result;
            var body = response.Content.ReadAsStringAsync().Result;
            var output = body.FromJson<AppSearchRequestOutput>();
            Assert.IsTrue(output.ToJson().Equals(RequestListSample));
        }

        [TestMethod]
        public void SearchTest()
        {
            var searchInput = new AppSearchRequestInput
            {
                PropertyType = PropertyType.Land,
                IntentionOfCustomer = IntentionOfCustomer.ForDailyRent
            };
            var content = new StringContent(searchInput.ToJson(), Encoding.UTF8, "application/json");
            var response = Client.PostAsync(UrlPrefix + "/requests/search", content).Result;
            var body = response.Content.ReadAsStringAsync().Result;
            var output = body.FromJson<AppSearchRequestOutput>();
            Assert.IsTrue(output.RequestPagedList.PageItems.Count >= 1);
        }

        [TestMethod]
        public void GetRequestTest()
        {
            var requestId = "577b7b95b615241b4818ade1";
            var response = Client.GetAsync(UrlPrefix + "/requests/get/" + requestId).Result;
            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [TestMethod]
        public void GetCorrectRequestTest()
        {
            var requestId = "577b7b95b615241b4818ade1";
            var response = Client.GetAsync(UrlPrefix + "/requests/get/" + requestId).Result;
            var body = response.Content.ReadAsStringAsync().Result;
            var output = body.FromJson<AppRequestDetails>();
            Assert.IsTrue(output.ToJson().Equals(GetRequestSample));
        }

        [TestMethod]
        public void SaveRequestTest()
        {
            var newRequest = new AppNewRequestInput
            {
                UsageType = UsageType.Office,
                IntentionOfCustomer = IntentionOfCustomer.ForBuy,
                PropertyTypes = new long[]{1, 2},
                TotalPrice = 123000000,
                Owner = new AppNewCustomerInput
                {
                    ID = ObjectId.Parse("56adad38b6152420780362d8")
                }
            };
            var content = new StringContent(newRequest.ToJson(), Encoding.UTF8, "application/json");
            var response = Client.PostAsync(UrlPrefix + "/requests/save", content).Result;
            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [TestMethod]
        public void UpdateRequestTest()
        {
            var newRequest = new AppUpdateRequestInput
            {
                ID = ObjectId.Parse("56e69b2bb615241e547f1b32"),
                PropertyTypes = new long[] { 1, 2, 3, 4 },
                Owner = new AppNewCustomerInput
                {
                    ID = ObjectId.Parse("56adad38b6152420780362d8")
                }
            };
            var content = new StringContent(newRequest.ToJson(), Encoding.UTF8, "application/json");
            var response = Client.PostAsync(UrlPrefix + "/requests/update", content).Result;
            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [TestMethod]
        public void GetCustomerAllRequestsTest()
        {
            var customerId = "56adad38b6152420780362d8";
            var response = Client.GetAsync(UrlPrefix + "/requests/getcustomerrequests/" + customerId).Result;
            var body = response.Content.ReadAsStringAsync().Result;
            var output = body.FromJson<List<AppRequestSummary>>();
            Assert.IsTrue(output.Count >= 1);
        }

        [TestMethod]
        public void GetCorrectCustomerAllRequestsTest()
        {
            var customerId = "577b7426b615241b4818ad68";
            var response = Client.GetAsync(UrlPrefix + "/requests/getcustomerrequests/" + customerId).Result;
            var body = response.Content.ReadAsStringAsync().Result;
            var output = body.FromJson<List<AppRequestSummary>>();
            Assert.IsTrue(output.ToJson().Equals(CustomerRequestsSample));
        }

        private readonly string RequestListSample =
            "{\"RequestPagedList\":{\"PageItems\":[{\"ID\":\"577b7b24b615241b4818adde\",\"IntentionOfCustomer\":\"ForBuy\",\"UsageType\":\"Residency\",\"State\":\"New\",\"CreationTime\":\"\\/Date(1467710244180+0430)\\/\",\"Vicinities\":[],\"TotalPrice\":500000000.0,\"Owner\":{\"ID\":\"577b7426b615241b4818ad68\",\"DisplayName\":\"ابوالفضل پورعرب\",\"MobileNumber\":\"09124569875\"}},{\"ID\":\"577b7b95b615241b4818ade1\",\"IntentionOfCustomer\":\"ForRent\",\"UsageType\":\"Office\",\"State\":\"New\",\"CreationTime\":\"\\/Date(1467710357505+0430)\\/\",\"Vicinities\":[],\"TotalPrice\":93000000.0,\"Mortgage\":60000000.0,\"Rent\":1000000.0,\"Owner\":{\"ID\":\"577b7b95b615241b4818ade0\",\"DisplayName\":\"شکیبا قمری\",\"MobileNumber\":\"09125582632\"}}],\"PageNumber\":1,\"TotalNumberOfPages\":1,\"TotalNumberOfItems\":2}}";

        private readonly string GetRequestSample =
            "{\"IsArchived\":false,\"SelectedVicinities\":[],\"BuildingAgeYears\":1,\"TotalNumberOfUnits\":2,\"NumberOfUnitsPerFloor\":1,\"TotalNumberOfFloors\":2,\"UnitArea\":50.0,\"NumberOfRooms\":1,\"NumberOfParkings\":1,\"UnitFloorNumber\":1,\"IsDublex\":false,\"HasBeenReconstructed\":true,\"HasIranianLavatory\":true,\"HasForeignLavatory\":true,\"HasElevator\":true,\"HasAutomaticParkingDoor\":true,\"HasVideoEyePhone\":true,\"MortgageAndRentConvertible\":true,\"ID\":\"577b7b95b615241b4818ade1\",\"IntentionOfCustomer\":\"ForRent\",\"UsageType\":\"Office\",\"State\":\"New\",\"Description\":\"برای تست محتویات لیست درخواست ها\",\"CreationTime\":\"\\/Date(1467710357505+0430)\\/\",\"Vicinities\":[],\"TotalPrice\":93000000.0,\"Mortgage\":60000000.0,\"Rent\":1000000.0,\"Owner\":{\"ID\":\"577b7b95b615241b4818ade0\",\"DisplayName\":\"شکیبا قمری\",\"MobileNumber\":\"09125582632\"}}";

        private readonly string CustomerRequestsSample =
            "[{\"ID\":\"577b7b24b615241b4818adde\",\"IntentionOfCustomer\":\"ForBuy\",\"UsageType\":\"Residency\",\"State\":\"New\",\"PropertyTypes\":[1,3,5,101],\"Description\":\"برای تست محتویات لیست درخواست ها\",\"CreationTime\":\"\\/Date(1467710244180+0430)\\/\",\"TotalPrice\":500000000.0,\"Owner\":{\"ID\":\"577b7426b615241b4818ad68\",\"DisplayName\":\"ابوالفضل پورعرب\",\"MobileNumber\":\"09124569875\"}}]";
    }
}