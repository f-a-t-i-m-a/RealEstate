using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using Compositional.Composer;
using JahanJooy.RealEstateAgency.Api.App.Models.Customer;
using JahanJooy.RealEstateAgency.TestPreparationService.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using ServiceStack;

namespace JahanJooy.RealEstateAgency.Test.APITestClasses
{
    [TestClass]
    public class ApiCustomerTest : ApiBaseClass
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
            var searchInput = new AppSearchCustomerInput();
            var content = new StringContent(searchInput.ToJson(), Encoding.UTF8, "application/json");
            var response = Client.PostAsync(UrlPrefix + "/customers/search", content).Result;
            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [TestMethod]
        public void CorrectListTest()
        {
            LoginAsNormalUser(Client, "a.kiarostami", "123456");
            var searchInput = new AppSearchCustomerInput();
            var content = new StringContent(searchInput.ToJson(), Encoding.UTF8, "application/json");
            var response = Client.PostAsync(UrlPrefix + "/customers/search", content).Result;
            var body = response.Content.ReadAsStringAsync().Result;
            var output = body.FromJson<AppSearchCustomersOutput>();
            Assert.IsTrue(output.ToJson().Equals(ListCustomersSample));
        }

        [TestMethod]
        public void SearchTest()
        {
            var searchInput = new AppSearchCustomerInput
            {
                DisplayName = "سیما"
            };
            var content = new StringContent(searchInput.ToJson(), Encoding.UTF8, "application/json");
            var response = Client.PostAsync(UrlPrefix + "/customers/search", content).Result;
            var body = response.Content.ReadAsStringAsync().Result;
            var output = body.FromJson<AppSearchCustomersOutput>();
            Assert.IsTrue(output.CustomerPagedList.PageItems.Count >= 1);
        }

        [TestMethod]
        public void GetCustomerTest()
        {
            var customerId = "577b7b95b615241b4818ade0";
            var response = Client.GetAsync(UrlPrefix + "/customers/get/" + customerId).Result;
            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [TestMethod]
        public void GetCorrectCustomerTest()
        {
            var customerId = "577b7b95b615241b4818ade0";
            var response = Client.GetAsync(UrlPrefix + "/customers/get/" + customerId).Result;
            var body = response.Content.ReadAsStringAsync().Result;
            var output = body.FromJson<AppCustomerDetails>();
            Assert.IsTrue(output.ToJson().Equals(GetCustomerSample));
        }

        [TestMethod]
        public void SaveCustomerTest()
        {
            var newCustomer = new AppNewCustomerInput
            {
                DisplayName = "اشکان بنکدار"
            };
            var content = new StringContent(newCustomer.ToJson(), Encoding.UTF8, "application/json");
            var response = Client.PostAsync(UrlPrefix + "/customers/save", content).Result;
            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [TestMethod]
        public void UpdateCustomerTest()
        {
            var newCustomer = new AppUpdateCustomerInput
            {
                ID = ObjectId.Parse("56adad38b6152420780362d8"),
                DisplayName = "سیما راست خدیو"
            };
            var content = new StringContent(newCustomer.ToJson(), Encoding.UTF8, "application/json");
            var response = Client.PostAsync(UrlPrefix + "/customers/update", content).Result;
            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        private readonly string ListCustomersSample =
            "{\"CustomerPagedList\":{\"PageItems\":[{\"ID\":\"577b7b95b615241b4818ade0\",\"DisplayName\":\"شکیبا قمری\",\"PhoneNumber\":\"02155263355\",\"MobileNumber\":\"09125582632\",\"CountPhoneNumber\":1,\"CountMobileNumber\":1,\"RequestCount\":1,\"PropertyCount\":0,\"LastVisitTime\":\"\\/Date(1467710357507+0430)\\/\"},{\"ID\":\"577b746fb615241b4818ad6d\",\"DisplayName\":\"شهرام شبپره\",\"MobileNumber\":\"09125526354\",\"CountPhoneNumber\":0,\"CountMobileNumber\":1,\"RequestCount\":0,\"PropertyCount\":1,\"LastVisitTime\":\"\\/Date(1467708527194+0430)\\/\"},{\"ID\":\"577b7426b615241b4818ad68\",\"FullName\":\"ابوالفضل پورعرب\",\"MobileNumber\":\"09124569875\",\"CountPhoneNumber\":0,\"CountMobileNumber\":1,\"RequestCount\":0,\"PropertyCount\":1,\"LastVisitTime\":\"\\/Date(1467708454070+0430)\\/\"}],\"PageNumber\":1,\"TotalNumberOfPages\":1,\"TotalNumberOfItems\":3}}";

        private readonly string GetCustomerSample =
            "{\"Email\":\"sh.ghamari@gmail.com\",\"Age\":30,\"IsMarried\":true,\"IsArchived\":false,\"PhoneNumbers\":[\"02155263355\"],\"MobileNumbers\":[\"09125582632\"],\"NameOfFather\":\"عباس\",\"Identification\":55555,\"IssuedIn\":\"شمیران\",\"SocialSecurityNumber\":\"5555555555\",\"DateOfBirth\":\"\\/Date(1467660600000+0430)\\/\",\"Address\":\"تهران\",\"ID\":\"577b7b95b615241b4818ade0\",\"DisplayName\":\"شکیبا قمری\",\"PhoneNumber\":\"02155263355\",\"MobileNumber\":\"09125582632\",\"CountPhoneNumber\":1,\"CountMobileNumber\":1,\"RequestCount\":1,\"PropertyCount\":0,\"LastVisitTime\":\"\\/Date(1467710357507+0430)\\/\"}";
    }
}