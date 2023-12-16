using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Compositional.Composer;
using JahanJooy.RealEstateAgency.Api.Web.Base;
using JahanJooy.RealEstateAgency.Domain.Base;
using JahanJooy.RealEstateAgency.Domain.Contract;
using JahanJooy.RealEstateAgency.Domain.Customers;
using JahanJooy.RealEstateAgency.Domain.Enums.Elastic;
using JahanJooy.RealEstateAgency.Domain.Enums.User;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using JahanJooy.RealEstateAgency.Domain.Property;
using JahanJooy.RealEstateAgency.Domain.Request;
using JahanJooy.RealEstateAgency.Domain.Supply;
using JahanJooy.RealEstateAgency.Domain.Users;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Identity;
using JahanJooy.RealEstateAgency.Util.Log4Net;
using JahanJooy.RealEstateAgency.Util.UserActivityLog;
using MongoDB.Bson;
using MongoDB.Driver;

namespace JahanJooy.RealEstateAgency.Api.Web.Controllers
{
    [Contract]
    [Component]
    [ComponentCache(null)]
    [RoutePrefix("operations")]
    public class OperationController : ExtendedApiController
    {
        #region Private helper methods 

        public void CreateIndex()
        {
            var response = ElasticManager.Client.CreateIndex(ElasticManager.Index, d => d.Mappings(
                md => md
                    .Map<SupplyIE>(m => m.AutoMap())
                    .Map<PropertyIE>(m => m.AutoMap())
                    .Map<ContractIE>(m => m.AutoMap())
                    .Map<CustomerIE>(m => m.AutoMap())
                    .Map<RequestIE>(m => m.AutoMap())
                    .Map<ApplicationUserIE>(m => m.AutoMap())
                    .Map<SearchIE>(m => m.AutoMap())
                ));

            if (!response.IsValid)
            {
                ApplicationStaticLogs.ElasticLog.ErrorFormat(
                    "An error occured while creating index, debug information: {0}",
                    response.DebugInformation);
            }
        }

        public void DeleteIndex()
        {
            ElasticManager.Client.DeleteIndex(ElasticManager.Index);
        }

        #endregion

        #region Injected dependencies

        [ComponentPlug]
        public ElasticManager ElasticManager { get; set; }

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        #endregion

        #region Action methods

        [HttpGet, Route("all")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.Administrator})]
        public List<string> GetAllTypes()
        {
            return typeof (Types).GetFields().Select(i => i.Name).ToList();
        }

        [HttpPost, Route("deleteindex")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.Administrator})]
        [UserActivity(UserActivityType.Edit, EntityType.Index, ApplicationType.Haftdong, ActivitySubType = "Delete and Create Index")]
        public void DeleteAndCreate()
        {
            DeleteIndex();
            CreateIndex();
        }

        [HttpPost, Route("reindex/{type}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.Administrator})]
        [UserActivity(UserActivityType.Edit, EntityType.Index, ApplicationType.Haftdong)]
        public void ReIndex(string type)
        {
            UserActivityLogUtils.SetMainActivity(activitySubType: type);
            var filter = new BsonDocument();
            switch (type)
            {
                case nameof(Types.PropertyType):
                    var update = Builders<Property>.Update.Set("LastIndexingTime", BsonNull.Value);
                    DbManager.Property.UpdateManyAsync(filter, update);
                    break;

                case nameof(Types.RequestType):
                    var requestUpdate = Builders<Request>.Update.Set("LastIndexingTime", BsonNull.Value);
                    DbManager.Request.UpdateManyAsync(filter, requestUpdate);
                    break;

                case nameof(Types.CustomerType):
                    var customerUpdate = Builders<Customer>.Update.Set("LastIndexingTime", BsonNull.Value);
                    DbManager.Customer.UpdateManyAsync(filter, customerUpdate);
                    break;


                case nameof(Types.UserType):
                    var userUpdate = Builders<ApplicationUser>.Update.Set("LastIndexingTime", BsonNull.Value);
                    DbManager.ApplicationUser.UpdateManyAsync(filter, userUpdate);
                    break;

                case nameof(Types.SupplyType):
                    var supplyUpdate = Builders<Supply>.Update.Set("LastIndexingTime", BsonNull.Value);
                    DbManager.Supply.UpdateManyAsync(filter, supplyUpdate);
                    break;

                case nameof(Types.ContractType):
                    var contractUpdate = Builders<Contract>.Update.Set("LastIndexingTime", BsonNull.Value);
                    DbManager.Contract.UpdateManyAsync(filter, contractUpdate);
                    break;
            }
        }

        [HttpPost, Route("deleteandreindexall")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.Administrator})]
        [UserActivity(UserActivityType.Edit, EntityType.Index,ApplicationType.Haftdong ,ActivitySubType = "Delete and Reindex All")]
        public void DeleteAndReIndexAll()
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

        [HttpPost, Route("reindexall")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.Administrator})]
        [UserActivity(UserActivityType.Edit, EntityType.Index, ApplicationType.Haftdong, ActivitySubType = "ReindexAll")]
        public void ReIndexAll()
        {
            var filter = new BsonDocument();

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

        #endregion
    }
}