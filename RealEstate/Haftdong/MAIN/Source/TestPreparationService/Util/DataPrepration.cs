using System.Collections.Generic;
using System.IO;
using Compositional.Composer;
using JahanJooy.RealEstateAgency.Domain.Contract;
using JahanJooy.RealEstateAgency.Domain.Customers;
using JahanJooy.RealEstateAgency.Domain.MasterData;
using JahanJooy.RealEstateAgency.Domain.Property;
using JahanJooy.RealEstateAgency.Domain.Request;
using JahanJooy.RealEstateAgency.Domain.Supply;
using JahanJooy.RealEstateAgency.Domain.Users;
using JahanJooy.RealEstateAgency.Domain.Vicinities;
using JahanJooy.RealEstateAgency.TestPreparationService.Services;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using MongoDB.Bson;
using MongoDB.Driver;
using ServiceStack.Text;

namespace JahanJooy.RealEstateAgency.TestPreparationService.Util
{
    [Contract]
    [Component]
    public class DataPrepration
    {
        private const string FolderPath = "TestData";

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public PropertyService PropertyService { get; set; }

        [ComponentPlug]
        public CustomerService CustomerService { get; set; }

        [ComponentPlug]
        public RequestService RequestService { get; set; }

        [ComponentPlug]
        public SupplyService SupplyService { get; set; }

        [ComponentPlug]
        public ContractService ContractService { get; set; }

        [ComponentPlug]
        public ApplicationUserService ApplicationUserService { get; set; }

        [ComponentPlug]
        public ConfigurationDataItemService ConfigurationDataItemService { get; set; }

        [ComponentPlug]
        public VicinityService VicinityService { get; set; }


        [ComponentPlug]
        public ElasticManager ElasticManager { get; set; }

        public void RebuildData()
        {
            DbManager.ClearDB();
            ReadJsonFile();
            ReIndexAll();
        }

        private void ReadJsonFile()
        {
            foreach (string file in Directory.EnumerateFiles(FolderPath, "*.json"))
            {
                string json = File.ReadAllText(file);
                string fileName = file.Substring(file.IndexOf('\\') + 1);
                fileName = fileName.Substring(0, fileName.Length - 5);
                switch (fileName.ToLower())
                {
                    case "property":
                        List<Property> properties = JsonSerializer.DeserializeFromString<List<Property>>(json);
                        properties.ForEach(i =>
                        {
                            if (i != null) PropertyService.SaveProperty(i);
                        });
                        break;
                    case "customer":
                        List<Customer> customers = JsonSerializer.DeserializeFromString<List<Customer>>(json);
                        customers.ForEach(i =>
                        {
                            if (i != null) CustomerService.SaveCustomer(i);
                        });
                        break;
                    case "request":
                        List<Request> requests = JsonSerializer.DeserializeFromString<List<Request>>(json);
                        requests.ForEach(i =>
                        {
                            if (i != null) RequestService.SaveRequest(i);
                        });
                        break;
                    case "supply":
                        List<Supply> supplies = JsonSerializer.DeserializeFromString<List<Supply>>(json);
                        supplies.ForEach(i =>
                        {
                            if (i != null) SupplyService.SaveSupply(i);
                        });
                        break;
                    case "contract":
                        List<Contract> contracts = JsonSerializer.DeserializeFromString<List<Contract>>(json);
                        contracts.ForEach(i =>
                        {
                            if (i != null) ContractService.SaveContract(i);
                        });
                        break;
                    case "applicationuser":
                        List<ApplicationUser> users = JsonSerializer.DeserializeFromString<List<ApplicationUser>>(json);
                        users.ForEach(i =>
                        {
                            if (i != null) ApplicationUserService.SaveApplicationUser(i);
                        });
                        break;
                    case "configurationdataitem":
                        List<ConfigurationDataItem> items =
                            JsonSerializer.DeserializeFromString<List<ConfigurationDataItem>>(json);
                        items.ForEach(i =>
                        {
                            if (i != null) ConfigurationDataItemService.SaveConfigurationDataItem(i);
                        });
                        break;
                    case "vicinity":
                        List<Vicinity> vicinities = JsonSerializer.DeserializeFromString<List<Vicinity>>(json);
                        vicinities.ForEach(i =>
                        {
                            if (i != null) VicinityService.SaveVicinity(i);
                        });
                        break;
                }
            }
        }

        public void ReIndexAll()
        {
            var filter = new BsonDocument();

            DeleteIndex();
            CreateIndex();

            var update = Builders<Property>.Update.Set("LastIndexingTime", BsonNull.Value);
            DbManager.Property.UpdateManyAsync(filter, update);

            var requestUpdate = Builders<Request>.Update.Set("LastIndexingTime", BsonNull.Value);
            DbManager.Request.UpdateManyAsync(filter, requestUpdate);

            var customerUpdate = Builders<Customer>.Update.Set("LastIndexingTime", BsonNull.Value);
            DbManager.Customer.UpdateManyAsync(filter, customerUpdate);

            var userUpdate = Builders<ApplicationUser>.Update.Set("LastIndexingTime", BsonNull.Value);
            DbManager.ApplicationUser.UpdateManyAsync(filter, userUpdate);

            var supplyUpdate = Builders<Supply>.Update.Set("LastIndexingTime", BsonNull.Value);
            DbManager.Supply.UpdateManyAsync(filter, supplyUpdate);

            var contractUpdate = Builders<Contract>.Update.Set("LastIndexingTime", BsonNull.Value);
            DbManager.Contract.UpdateManyAsync(filter, contractUpdate);
        }

        private void CreateIndex()
        {
            ElasticManager.Client.CreateIndex(ElasticManager.Index, d => d.Mappings(
                md => md
                    .Map<SupplyIE>(m => m.AutoMap())
                    .Map<PropertyIE>(m => m.AutoMap())
                    .Map<ContractIE>(m => m.AutoMap())
                    .Map<CustomerIE>(m => m.AutoMap())
                    .Map<RequestIE>(m => m.AutoMap())
                    .Map<ApplicationUserIE>(m => m.AutoMap())
                ));
        }

        private void DeleteIndex()
        {
            ElasticManager.Client.DeleteIndex(ElasticManager.Index);
        }
    }
}