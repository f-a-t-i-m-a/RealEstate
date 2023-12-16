using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using Compositional.Composer;
using JahanJooy.RealEstateAgency.Api.App.Models.Customer;
using JahanJooy.RealEstateAgency.Api.App.Models.Property;
using JahanJooy.RealEstateAgency.Api.App.Models.Supply;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.TestPreparationService.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using ServiceStack;

namespace JahanJooy.RealEstateAgency.Test.APITestClasses
{
    [TestClass]
    public class ApiFileTest : ApiBaseClass
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
            var searchInput = new AppSearchFileInput();
            var content = new StringContent(searchInput.ToJson(), Encoding.UTF8, "application/json");
            var response = Client.PostAsync(UrlPrefix + "/files/search", content).Result;
            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [TestMethod]
        public void CorrectListTest()
        {
            LoginAsNormalUser(Client, "a.kiarostami", "123456");
            var searchInput = new AppSearchFileInput();
            var content = new StringContent(searchInput.ToJson(), Encoding.UTF8, "application/json");
            var response = Client.PostAsync(UrlPrefix + "/files/search", content).Result;
            var body = response.Content.ReadAsStringAsync().Result;
            var output = body.FromJson<AppSearchFileOutput>();
            Assert.IsTrue(output.ToJson().Equals(FileListSample));
        }

        [TestMethod]
        public void SearchTest()
        {
            var searchInput = new AppSearchFileInput
            {
                UsageType = UsageType.Residency,
                PropertyType = PropertyType.Land
            };
            var content = new StringContent(searchInput.ToJson(), Encoding.UTF8, "application/json");
            var response = Client.PostAsync(UrlPrefix + "/files/search", content).Result;
            var body = response.Content.ReadAsStringAsync().Result;
            var output = body.FromJson<AppSearchFileOutput>();
            Assert.IsTrue(output.SupplyPagedList.PageItems.Count >= 1);
        }

        [TestMethod]
        public void GetPropertyTest()
        {
            var propertyId = "56e3f96bb615264f70ed400d";
            var response = Client.GetAsync(UrlPrefix + "/properties/get/" + propertyId).Result;
            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [TestMethod]
        public void GetCorrectPropertyTest()
        {
            var propertyId = "56e3f96bb615264f70ed400d";
            var response = Client.GetAsync(UrlPrefix + "/properties/get/" + propertyId).Result;
            var body = response.Content.ReadAsStringAsync().Result;
            var output = body.FromJson<AppPropertyDetails>();
            Assert.IsTrue(output.ToJson().Equals(GetPropertySample));
        }

        [TestMethod]
        public void GetSupplyTest()
        {
            var supplyId = "56e3f96bb615264f70ed400e";
            var response = Client.GetAsync(UrlPrefix + "/supplies/get/" + supplyId).Result;
            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [TestMethod]
        public void GetCorrectSupplyTest()
        {
            var supplyId = "56e3f96bb615264f70ed400e";
            var response = Client.GetAsync(UrlPrefix + "/supplies/get/" + supplyId).Result;
            var body = response.Content.ReadAsStringAsync().Result;
            var output = body.FromJson<AppSupplyDetails>();
            Assert.IsTrue(output.ToJson().Equals(GetSupplySample));
        }

        [TestMethod]
        public void SavePropertyTest()
        {
            var newProperty = new AppNewPropertyInput
            {
                PropertyUsageType = UsageType.Office,
                PropertyType = PropertyType.Land,
                IntentionOfOwner = IntentionOfOwner.ForSale,
                PropertyEstateArea = 1000,
                EstateDirection = EstateDirection.North,
                PriceSpecificationType = SalePriceSpecificationType.Total,
                PropertyTotalPrice = 123000000,
                Owner = new AppNewCustomerInput
                {
                    ID = ObjectId.Parse("56adad38b6152420780362d8")
                }
            };
            var content = new StringContent(newProperty.ToJson(), Encoding.UTF8, "application/json");
            var response = Client.PostAsync(UrlPrefix + "/properties/save", content).Result;
            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [TestMethod]
        public void SaveSupplyTest()
        {
            var newProperty = new AppNewSupplyInput
            {
                PropertyId = ObjectId.Parse("56e3f96bb615264f70ed400d"),
                IntentionOfOwner = IntentionOfOwner.ForDailyRent,
                Rent = 1500000
            };
            var content = new StringContent(newProperty.ToJson(), Encoding.UTF8, "application/json");
            var response = Client.PostAsync(UrlPrefix + "/supplies/save", content).Result;
            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [TestMethod]
        public void UpdatePropertyTest()
        {
            var newProperty = new AppUpdatePropertyInput
            {
                ID = ObjectId.Parse("56e3f96bb615264f70ed400d")
            };
            var content = new StringContent(newProperty.ToJson(), Encoding.UTF8, "application/json");
            var response = Client.PostAsync(UrlPrefix + "/properties/update", content).Result;
            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [TestMethod]
        public void UpdateSupplyTest()
        {
            var newProperty = new AppUpdateSupplyInput
            {
                ID = ObjectId.Parse("56e3f96bb615264f70ed400e"),
                PricePerEstateArea = 5500000
            };
            var content = new StringContent(newProperty.ToJson(), Encoding.UTF8, "application/json");
            var response = Client.PostAsync(UrlPrefix + "/supplies/update", content).Result;
            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [TestMethod]
        public void GetCustomerPropertiesTest()
        {
            var customerId = "577b7426b615241b4818ad68";
            var response = Client.GetAsync(UrlPrefix + "/properties/getcustomerproperties/" + customerId).Result;
            var body = response.Content.ReadAsStringAsync().Result;
            var output = body.FromJson<List<AppPropertySummary>>();
            Assert.IsTrue(output.Count >= 1);
        }

        [TestMethod]
        public void GetCorrectCustomerPropertiesTest()
        {
            var customerId = "577b7426b615241b4818ad68";
            var response = Client.GetAsync(UrlPrefix + "/properties/getcustomerproperties/" + customerId).Result;
            var body = response.Content.ReadAsStringAsync().Result;
            var output = body.FromJson<List<AppPropertySummary>>();
            Assert.IsTrue(output.ToJson().Equals(CustomerPropertiesSample));
        }

        [TestMethod]
        public void GetCustomerSuppliesTest()
        {
            var customerId = "577b7426b615241b4818ad68";
            var response = Client.GetAsync(UrlPrefix + "/supplies/getcustomersupplies/" + customerId).Result;
            var body = response.Content.ReadAsStringAsync().Result;
            var output = body.FromJson<List<AppPropertySummary>>();
            Assert.IsTrue(output.Count >= 1);
        }

        [TestMethod]
        public void GetCorrectCustomerSuppliesTest()
        {
            var customerId = "577b7426b615241b4818ad68";
            var response = Client.GetAsync(UrlPrefix + "/supplies/getcustomersupplies/" + customerId).Result;
            var body = response.Content.ReadAsStringAsync().Result;
            var output = body.FromJson<List<AppSupplySummary>>();
            Assert.IsTrue(output.ToJson().Equals(CustomerSuppliesSample));
        }

        //        [TestMethod]
        public void GetThumbnailTest()
        {
            var photoId = "56adaff8b6152420780362da";
            var response = Client.GetAsync(UrlPrefix + "/properties/getThumbnail/" + photoId).Result;
            Assert.IsTrue(response.IsSuccessStatusCode);
        }

//        [TestMethod]
        public void GetMediumSizeTest()
        {
            var photoId = "56adaff8b6152420780362da";
            var response = Client.GetAsync(UrlPrefix + "/properties/getMediumSize/" + photoId).Result;
            Assert.IsTrue(response.IsSuccessStatusCode);
        }

//        [TestMethod]
        public void GetFullSizeTest()
        {
            var photoId = "56adaff8b6152420780362da";
            var response = Client.GetAsync(UrlPrefix + "/properties/getFullSize/" + photoId).Result;
            Assert.IsTrue(response.IsSuccessStatusCode);
        }

//        [TestMethod]
        public void DownloadTest()
        {
            var photoId = "56adaff8b6152420780362da";
            var response = Client.GetAsync(UrlPrefix + "/properties/download/" + photoId).Result;
            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        private readonly string GetPropertySample =
            "{\"Photos\":[],\"Supplies\":[{\"ID\":\"56e3f96bb615264f70ed400e\",\"IntentionOfOwner\":\"ForSale\",\"State\":\"New\",\"CreationTime\":\"\\/Date(1457781099452+0330)\\/\",\"PriceSpecificationType\":\"PerEstateArea\",\"TotalPrice\":1200000000.0,\"PricePerEstateArea\":6000000.0}],\"LicencePlate\":\"987456\",\"IsAgencyListing\":false,\"IsAgencyActivityAllowed\":false,\"EstateDirection\":\"North\",\"PassageEdgeLength\":20.0,\"EstateVoucherType\":\"Normal\",\"TotalNumberOfUnits\":1,\"BuildingAgeYears\":5,\"NumberOfUnitsPerFloor\":1,\"TotalNumberOfFloors\":1,\"StorageRoomArea\":10.0,\"NumberOfRooms\":1,\"NumberOfParkings\":1,\"UnitFloorNumber\":1,\"NumberOfMasterBedrooms\":0,\"KitchenCabinetType\":\"Mdf\",\"MainDaylightDirection\":\"North\",\"LivingRoomFloor\":\"Ceramic\",\"FaceType\":\"Concrete\",\"IsDuplex\":false,\"HasIranianLavatory\":true,\"HasForeignLavatory\":true,\"HasPrivatePatio\":true,\"HasBeenReconstructed\":false,\"HasElevator\":false,\"HasGatheringHall\":false,\"HasAutomaticParkingDoor\":true,\"HasVideoEyePhone\":true,\"HasSwimmingPool\":true,\"HasSauna\":false,\"HasJacuzzi\":false,\"HasTransferableLoan\":false,\"MortgageAndRentConvertible\":false,\"ID\":\"56e3f96bb615264f70ed400d\",\"PropertyType\":\"Villa\",\"State\":\"New\",\"IsArchived\":false,\"CreationTime\":\"\\/Date(1457781099385+0330)\\/\",\"LastModificationTime\":\"\\/Date(1467705652594+0430)\\/\",\"Address\":\"برای تست API ها\",\"EstateArea\":200.0,\"UnitArea\":50.0,\"UsageType\":\"Residency\",\"SourceType\":\"Haftdong\",\"Owner\":{\"ID\":\"56adad38b6152420780362d8\",\"DisplayName\":\"سیما راست خدیو\",\"PhoneNumber\":\"02188885566\",\"MobileNumber\":\"09124449966\",\"NameOfFather\":\"علی\",\"Identification\":11111,\"IssuedIn\":\"تهران\",\"SocialSecurityNumber\":\"1111111111\",\"DateOfBirth\":\"\\/Date(1467660600000+0430)\\/\",\"Address\":\"\"}}";

        private readonly string GetSupplySample =
            "{\"IsArchived\":false,\"HasTransferableLoan\":true,\"TransferableLoanAmount\":10000000.0,\"MortgageAndRentConvertible\":false,\"ID\":\"56e3f96bb615264f70ed400e\",\"IntentionOfOwner\":\"ForSale\",\"State\":\"New\",\"CreationTime\":\"\\/Date(1457781099452+0330)\\/\",\"LastModificationTime\":\"\\/Date(1467705688614+0430)\\/\",\"PriceSpecificationType\":\"PerEstateArea\",\"TotalPrice\":1200000000.0,\"PricePerEstateArea\":6000000.0,\"ContactInfo\":{\"ID\":\"000000000000000000000000\",\"ParentID\":\"000000000000000000000000\",\"ContactPhoneNumbers\":[],\"OwnerCanBeContacted\":true,\"OwnerName\":\"سیما راست خدیو\",\"OwnerPhoneNumbers\":[\"09124449966\"],\"OwnerEmailAddress\":\"s.rastkhadiv@gmail.com\"},\"PropertyDetail\":{\"Photos\":[],\"Supplies\":[{\"ID\":\"56e3f96bb615264f70ed400e\",\"IntentionOfOwner\":\"ForSale\",\"State\":\"New\",\"CreationTime\":\"\\/Date(1457781099452+0330)\\/\",\"PriceSpecificationType\":\"PerEstateArea\",\"TotalPrice\":1200000000.0,\"PricePerEstateArea\":6000000.0}],\"LicencePlate\":\"987456\",\"IsAgencyListing\":false,\"IsAgencyActivityAllowed\":false,\"EstateDirection\":\"North\",\"PassageEdgeLength\":20.0,\"EstateVoucherType\":\"Normal\",\"TotalNumberOfUnits\":1,\"BuildingAgeYears\":5,\"NumberOfUnitsPerFloor\":1,\"TotalNumberOfFloors\":1,\"StorageRoomArea\":10.0,\"NumberOfRooms\":1,\"NumberOfParkings\":1,\"UnitFloorNumber\":1,\"NumberOfMasterBedrooms\":0,\"KitchenCabinetType\":\"Mdf\",\"MainDaylightDirection\":\"North\",\"LivingRoomFloor\":\"Ceramic\",\"FaceType\":\"Concrete\",\"IsDuplex\":false,\"HasIranianLavatory\":true,\"HasForeignLavatory\":true,\"HasPrivatePatio\":true,\"HasBeenReconstructed\":false,\"HasElevator\":false,\"HasGatheringHall\":false,\"HasAutomaticParkingDoor\":true,\"HasVideoEyePhone\":true,\"HasSwimmingPool\":true,\"HasSauna\":false,\"HasJacuzzi\":false,\"HasTransferableLoan\":false,\"MortgageAndRentConvertible\":false,\"ID\":\"56e3f96bb615264f70ed400d\",\"PropertyType\":\"Villa\",\"State\":\"New\",\"IsArchived\":false,\"CreationTime\":\"\\/Date(1457781099385+0330)\\/\",\"LastModificationTime\":\"\\/Date(1467705652594+0430)\\/\",\"Address\":\"برای تست API ها\",\"EstateArea\":200.0,\"UnitArea\":50.0,\"UsageType\":\"Residency\",\"SourceType\":\"Haftdong\",\"Owner\":{\"ID\":\"56adad38b6152420780362d8\",\"DisplayName\":\"سیما راست خدیو\",\"PhoneNumber\":\"02188885566\",\"MobileNumber\":\"09124449966\",\"NameOfFather\":\"علی\",\"Identification\":11111,\"IssuedIn\":\"تهران\",\"SocialSecurityNumber\":\"1111111111\",\"DateOfBirth\":\"\\/Date(1467660600000+0430)\\/\",\"Address\":\"\"}}}";

        private readonly string FileListSample =
            "{\"SupplyPagedList\":{\"PageItems\":[{\"ID\":\"577b746fb615241b4818ad6f\",\"IntentionOfOwner\":\"ForSale\",\"State\":\"New\",\"CreationTime\":\"\\/Date(1467708527194+0430)\\/\",\"PriceSpecificationType\":\"Total\",\"TotalPrice\":500000000.0,\"PricePerEstateArea\":100000.0,\"PropertyDetail\":{\"Photos\":[],\"Supplies\":[{\"ID\":\"577b746fb615241b4818ad6f\",\"IntentionOfOwner\":\"ForSale\",\"State\":\"New\",\"CreationTime\":\"\\/Date(1467708527194+0430)\\/\",\"PriceSpecificationType\":\"Total\",\"TotalPrice\":500000000.0,\"PricePerEstateArea\":100000.0}],\"LicencePlate\":\"321456987\",\"IsAgencyListing\":false,\"IsAgencyActivityAllowed\":false,\"EstateDirection\":\"North\",\"PassageEdgeLength\":100.0,\"EstateVoucherType\":\"Normal\",\"HasTransferableLoan\":false,\"MortgageAndRentConvertible\":false,\"ID\":\"577b746fb615241b4818ad6e\",\"PropertyType\":\"Garden\",\"State\":\"New\",\"IsArchived\":false,\"CreationTime\":\"\\/Date(1467708527191+0430)\\/\",\"Address\":\"برای تست محتوای لیست فایل ها\",\"EstateArea\":5000.0,\"UsageType\":\"Agricultural\",\"SourceType\":\"Haftdong\",\"Owner\":{\"ID\":\"577b746fb615241b4818ad6d\",\"DisplayName\":\"شهرام شبپره\",\"MobileNumber\":\"09125526354\"}}},{\"ID\":\"577b7426b615241b4818ad6a\",\"IntentionOfOwner\":\"ForRent\",\"State\":\"New\",\"CreationTime\":\"\\/Date(1467708454078+0430)\\/\",\"TotalPrice\":290800000.0,\"Mortgage\":125800000.0,\"Rent\":5000000.0,\"PropertyDetail\":{\"Photos\":[],\"Supplies\":[{\"ID\":\"577b7426b615241b4818ad6a\",\"IntentionOfOwner\":\"ForRent\",\"State\":\"New\",\"CreationTime\":\"\\/Date(1467708454078+0430)\\/\",\"TotalPrice\":290800000.0,\"Mortgage\":125800000.0,\"Rent\":5000000.0}],\"LicencePlate\":\"123654789\",\"IsAgencyListing\":false,\"IsAgencyActivityAllowed\":false,\"TotalNumberOfUnits\":8,\"BuildingAgeYears\":20,\"NumberOfUnitsPerFloor\":2,\"TotalNumberOfFloors\":4,\"StorageRoomArea\":50.0,\"NumberOfRooms\":3,\"NumberOfParkings\":2,\"UnitFloorNumber\":3,\"NumberOfMasterBedrooms\":0,\"KitchenCabinetType\":\"Stone\",\"MainDaylightDirection\":\"West\",\"LivingRoomFloor\":\"Moquette\",\"FaceType\":\"Brick\",\"IsDuplex\":false,\"HasIranianLavatory\":true,\"HasForeignLavatory\":true,\"HasPrivatePatio\":false,\"HasBeenReconstructed\":false,\"HasElevator\":true,\"HasGatheringHall\":true,\"HasAutomaticParkingDoor\":true,\"HasVideoEyePhone\":true,\"HasSwimmingPool\":false,\"HasSauna\":false,\"HasJacuzzi\":false,\"HasTransferableLoan\":false,\"MortgageAndRentConvertible\":false,\"ID\":\"577b7426b615241b4818ad69\",\"PropertyType\":\"Apartment\",\"State\":\"New\",\"IsArchived\":false,\"CreationTime\":\"\\/Date(1467708454038+0430)\\/\",\"Address\":\"برای تست محتوای سرچ فایل ها\",\"UnitArea\":200.0,\"UsageType\":\"Residency\",\"SourceType\":\"Haftdong\",\"Owner\":{\"ID\":\"577b7426b615241b4818ad68\",\"DisplayName\":\"ابوالفضل پورعرب\",\"MobileNumber\":\"09124569875\"}}}],\"PageNumber\":1,\"TotalNumberOfPages\":1,\"TotalNumberOfItems\":2}}";

        private readonly string CustomerPropertiesSample =
            "[{\"ID\":\"577b7426b615241b4818ad69\",\"PropertyType\":\"Apartment\",\"State\":\"New\",\"IsArchived\":false,\"CreationTime\":\"\\/Date(1467708454038+0430)\\/\",\"Address\":\"برای تست محتوای سرچ فایل ها\",\"UnitArea\":200.0,\"UsageType\":\"Residency\",\"SourceType\":\"Haftdong\",\"Owner\":{\"ID\":\"577b7426b615241b4818ad68\",\"DisplayName\":\"ابوالفضل پورعرب\",\"MobileNumber\":\"09124569875\"}}]";

        private readonly string CustomerSuppliesSample =
            "[{\"ID\":\"577b7426b615241b4818ad6a\",\"IntentionOfOwner\":\"ForRent\",\"State\":\"New\",\"CreationTime\":\"\\/Date(1467708454078+0430)\\/\",\"TotalPrice\":290800000.0,\"Mortgage\":125800000.0,\"Rent\":5000000.0}]";
    }
}