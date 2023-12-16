using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Compositional.Composer;
using JahanJooy.RealEstateAgency.Domain.Audit;
using JahanJooy.RealEstateAgency.Domain.Contract;
using JahanJooy.RealEstateAgency.Domain.Customers;
using JahanJooy.RealEstateAgency.Domain.MasterData;
using JahanJooy.RealEstateAgency.Domain.Messages;
using JahanJooy.RealEstateAgency.Domain.Property;
using JahanJooy.RealEstateAgency.Domain.Request;
using JahanJooy.RealEstateAgency.Domain.Supply;
using JahanJooy.RealEstateAgency.Domain.Users;
using JahanJooy.RealEstateAgency.Domain.Vicinities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace JahanJooy.RealEstateAgency.Util.DataAccess
{
    [Contract]
    [Component]
    public class DbManager
    {
        private const string ConnectionStringName = "MongoConnection";
        private static MongoClient Client { get; set; }
        public static IMongoDatabase Database { get; set; }

        public DbManager()
        {
            Connect();
        }

        public void ConfigForTesting(string connectionStringForTest)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var connectionStringsSection = (ConnectionStringsSection) config.GetSection("connectionStrings");
            connectionStringsSection.ConnectionStrings[ConnectionStringName].ConnectionString = connectionStringForTest;
            config.Save();
            ConfigurationManager.RefreshSection("connectionStrings");
            Connect();
        }

        private void Connect()
        {
            var connectionStringSettings = ConfigurationManager.ConnectionStrings[ConnectionStringName];
            if (connectionStringSettings == null)
                throw new ConfigurationErrorsException(
                    "MongoDB Connection String for attachments is not configured. A connection string named '" +
                    ConnectionStringName + "' should be present in the application configuration.");

            var connectionString = connectionStringSettings.ConnectionString;
            if (connectionString == null)
                throw new ConfigurationErrorsException(
                    "MongoDB Connection String for attachments (named '" + ConnectionStringName + "') is empty. ");

            var mongoUrl = MongoUrl.Create(connectionString);
            var databaseName = mongoUrl.DatabaseName;
            MongoClientSettings settings = MongoClientSettings.FromUrl(mongoUrl);
            settings.WriteConcern = WriteConcern.Acknowledged;
            Client = new MongoClient(settings);
            Database = Client.GetDatabase(databaseName);
        }

        // ReSharper disable once InconsistentNaming
        public void ClearDB()
        {
            Database.DropCollection(nameof(Customer));
            Database.DropCollection(nameof(Property));
            Database.DropCollection(nameof(Supply));
            Database.DropCollection(nameof(Request));
            Database.DropCollection(nameof(Contract));
            Database.DropCollection(nameof(ApplicationUser));
            Database.DropCollection(nameof(ReportTemplate));
            Database.DropCollection(ConfigurationManager.AppSettings["ConfigurationDataItemCollection"]);
            Database.DropCollection(nameof(OutgoingSmsMessage));
            Database.DropCollection(nameof(Vicinity));
            Database.DropCollection(nameof(MobileSession));
            Database.DropCollection(nameof(PasswordResetRequest));
        }

        public IAsyncCursor<BsonDocument> Collections => Database.ListCollectionsAsync().Result;
        public IMongoCollection<Customer> Customer => Database.GetCollection<Customer>(typeof(Customer).Name);
        public IMongoCollection<Property> Property => Database.GetCollection<Property>(typeof(Property).Name);
        public IMongoCollection<Supply> Supply => Database.GetCollection<Supply>(typeof(Supply).Name);
        public IMongoCollection<Request> Request => Database.GetCollection<Request>(typeof(Request).Name);
        public IMongoCollection<Contract> Contract => Database.GetCollection<Contract>(typeof(Contract).Name);

        public IMongoCollection<ApplicationUser> ApplicationUser
            => Database.GetCollection<ApplicationUser>(typeof(ApplicationUser).Name);

        public IMongoCollection<ReportTemplate> ReportTemplate
            => Database.GetCollection<ReportTemplate>(typeof(ReportTemplate).Name);

        public IMongoCollection<ChangeHistory> ChangeHistory
            => Database.GetCollection<ChangeHistory>(typeof(ChangeHistory).Name);

        public IMongoCollection<UserActivity> UserActivity
            => Database.GetCollection<UserActivity>(typeof(UserActivity).Name);

        public IMongoCollection<OutgoingSmsMessage> OutgoingSmsMessage
            => Database.GetCollection<OutgoingSmsMessage>(typeof(OutgoingSmsMessage).Name);


        public IMongoCollection<ConfigurationDataItem> ConfigurationDataItem
            => GetConfigutationDataItem();


        public IMongoCollection<Vicinity> Vicinity => Database.GetCollection<Vicinity>(typeof(Vicinity).Name);

        public IMongoCollection<MobileSession> MobileSession
            => Database.GetCollection<MobileSession>(typeof(MobileSession).Name);

        public IMongoCollection<PasswordResetRequest> PasswordResetRequest
            => Database.GetCollection<PasswordResetRequest>(typeof(PasswordResetRequest).Name);


        public List<Type> GetAllRelatives(Type entity)
        {
            List<Type> relatives = new List<Type>();
            Collections.ForEachAsync(c =>
            {
                Type collectionType = c.GetType();
                var attributes = Attribute.GetCustomAttributes(collectionType).Any(attr => attr.GetType() == entity);
                if (attributes)
                {
                    relatives.Add(c.GetType());
                }
            });
            return relatives;
        }

        public IMongoCollection<T> GetCollection<T>()
        {
            var collection = Database.GetCollection<T>(typeof(T).Name);
            return collection;
        }

        private IMongoCollection<ConfigurationDataItem> GetConfigutationDataItem()
        {
            var configurationDataItemCollection = ConfigurationManager.AppSettings["ConfigurationDataItemCollection"];
            if (string.IsNullOrEmpty(configurationDataItemCollection))
            {
                return null;
            }
            return Database.GetCollection<ConfigurationDataItem>(configurationDataItemCollection);
        }
    }
}