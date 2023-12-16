using System;
using System.Windows.Forms;
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
using JahanJooy.RealEstateAgency.TestPreparationService.Util;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace JahanJooy.RealEstateAgency.TestPreparationService.Forms
{
    public partial class MainForm : Form
    {
        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IComposer Composer { get; set; }

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
        public DataPrepration DataPrepration { get; set; }

        public MainForm()
        {
            InitializeComponent();
        }

        private string SelectedCollection => (lstCollection.SelectedItem as string);
        private string SelectedEntity => (lstEntity.SelectedItem as string);

        private void MainForm_Load(object sender, EventArgs e)
        {
            PrepareDb();
        }

        private void PrepareDb()
        {
            RefreshCollectionList();
        }

        private void RefreshCollectionList()
        {
            lstCollection.Items.Clear();
            lstEntity.Items.Clear();

            DbManager.Collections.ForEachAsync(c =>
            {
                var name = c.GetValue("name").ToString();
//                if (!name.Contains("system") && !name.Contains("fs"))
                if (!name.Contains("system"))
                    lstCollection.Items.Add(name);
            });
        }

        private void RefreshEntityList()
        {
            lstEntity.Items.Clear();
            txtEntity.ResetText();
            var filter = new BsonDocument();

            switch (SelectedCollection)
            {
                case "Property":
                    var properties = DbManager.Property.Find(filter).ToListAsync().Result;
                    properties.ForEach(en => lstEntity.Items.Add(en.ID.ToString()));
                    break;
                case "Customer":
                    var customers = DbManager.Customer.Find(filter).ToListAsync().Result;
                    customers.ForEach(en => lstEntity.Items.Add(en.ID.ToString()));
                    break;
                case "Request":
                    var requests = DbManager.Request.Find(filter).ToListAsync().Result;
                    requests.ForEach(en => lstEntity.Items.Add(en.ID.ToString()));
                    break;
                case "ApplicationUser":
                    var users = DbManager.ApplicationUser.Find(filter).ToListAsync().Result;
                    users.ForEach(en => lstEntity.Items.Add(en.Id.ToString()));
                    break;
                case "Supply":
                    var supplies = DbManager.Supply.Find(filter).ToListAsync().Result;
                    supplies.ForEach(en => lstEntity.Items.Add(en.ID.ToString()));
                    break;
                case "ConfigurationDataItem":
                    var configs = DbManager.ConfigurationDataItem.Find(filter).ToListAsync().Result;
                    configs.ForEach(en => lstEntity.Items.Add(en.ID.ToString()));
                    break;
                case "Contract":
                    var contracts = DbManager.Contract.Find(filter).ToListAsync().Result;
                    contracts.ForEach(en => lstEntity.Items.Add(en.ID.ToString()));
                    break;
                case "Vicinity":
                    var vicinities = DbManager.Vicinity.Find(filter).ToListAsync().Result;
                    vicinities.ForEach(en => lstEntity.Items.Add(en.ID.ToString()));
                    break;
            }
        }

      

        private void lstCollection_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshEntityList();
        }

        private void lstEntity_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtEntity.ResetText();
            switch (SelectedCollection)
            {
                case "Property":
                    var property = PropertyService.GetProperty(SelectedEntity);
                    txtEntity.Text = JsonConvert.SerializeObject(property, Formatting.Indented);
                    break;
                case "Customer":
                    var customer = CustomerService.GetCustomer(SelectedEntity);
                    txtEntity.Text = JsonConvert.SerializeObject(customer, Formatting.Indented);
                    break;
                case "Request":
                    var request = RequestService.GetRequest(SelectedEntity);
                    txtEntity.Text = JsonConvert.SerializeObject(request, Formatting.Indented);
                    break;
                case "Supply":
                    var supply = SupplyService.GetSupply(SelectedEntity);
                    txtEntity.Text = JsonConvert.SerializeObject(supply, Formatting.Indented);
                    break;
                case "Contract":
                    var contract = ContractService.GetContract(SelectedEntity);
                    txtEntity.Text = JsonConvert.SerializeObject(contract, Formatting.Indented);
                    break;
                case "ApplicationUser":
                    var user = ApplicationUserService.GetApplicationUser(SelectedEntity);
                    txtEntity.Text = JsonConvert.SerializeObject(user, Formatting.Indented);
                    break;
                case "ConfigurationDataItem":
                    var item = ConfigurationDataItemService.GetConfigurationDataItem(SelectedEntity);
                    txtEntity.Text = JsonConvert.SerializeObject(item, Formatting.Indented);
                    break;
                case "Vicinity":
                    var vicinity = VicinityService.GetVicinity(SelectedEntity);
                    txtEntity.Text = JsonConvert.SerializeObject(vicinity, Formatting.Indented);
                    break;
            }
        }

        private void btnClean_Click(object sender, EventArgs e)
        {
            DbManager.ClearDB();
            RefreshEntityList();
            MessageBox.Show("دیتابیس با موفقیت پاک سازی شد");
        }

        private void btnRebuild_Click(object sender, EventArgs e)
        {
            DataPrepration.RebuildData();
            //            DbManager.ClearDB();
            //            ReadJsonFile();
            RefreshCollectionList();
            RefreshEntityList();
            MessageBox.Show("داده های موجود در فایل ها با موفقیت در دیتابیس ذخیره شدند");
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            switch (SelectedCollection)
            {
                case "Property":
                    var propertyFilter = Builders<Property>.Filter.Eq("ID", ObjectId.Parse(SelectedEntity));
                    DbManager.Property.DeleteOneAsync(propertyFilter);
                    break;
                case "Customer":
                    var customerFilter = Builders<Customer>.Filter.Eq("ID", ObjectId.Parse(SelectedEntity));
                    DbManager.Customer.DeleteOneAsync(customerFilter);
                    break;
                case "Request":
                    var requestFilter = Builders<Request>.Filter.Eq("ID", ObjectId.Parse(SelectedEntity));
                    DbManager.Request.DeleteOneAsync(requestFilter);
                    break;
                case "Supply":
                    var supplyFilter = Builders<Supply>.Filter.Eq("ID", ObjectId.Parse(SelectedEntity));
                    DbManager.Supply.DeleteOneAsync(supplyFilter);
                    break;
                case "Contract":
                    var contractFilter = Builders<Contract>.Filter.Eq("ID", ObjectId.Parse(SelectedEntity));
                    DbManager.Contract.DeleteOneAsync(contractFilter);
                    break;
                case "ApplicationUser":
                    var userFilter = Builders<ApplicationUser>.Filter.Eq("_id", ObjectId.Parse(SelectedEntity));
                    DbManager.ApplicationUser.DeleteOneAsync(userFilter);
                    break;
                case "ConfigurationDataItem":
                    var itemFilter = Builders<ConfigurationDataItem>.Filter.Eq("ID", ObjectId.Parse(SelectedEntity));
                    DbManager.ConfigurationDataItem.DeleteOneAsync(itemFilter);
                    break;
                case "Vicinity":
                    var vicinityFilter = Builders<Vicinity>.Filter.Eq("ID", ObjectId.Parse(SelectedEntity));
                    DbManager.Vicinity.DeleteOneAsync(vicinityFilter);
                    break;
            }
            RefreshEntityList();
            MessageBox.Show("داده ی انتخاب شده با موفقیت حذف شد");
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}