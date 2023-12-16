using System;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using Compositional.Composer;
using JahanJooy.Common.Util.Configuration;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstateAgency.Api.Web.Base;
using JahanJooy.RealEstateAgency.Domain.Enums.User;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using JahanJooy.RealEstateAgency.Domain.MasterData;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Enums;
using JahanJooy.RealEstateAgency.Util.Identity;
using JahanJooy.RealEstateAgency.Util.Models.ConfigureDataItem;
using JahanJooy.RealEstateAgency.Util.UserActivityLog;
using log4net;
using MongoDB.Bson;
using MongoDB.Driver;

namespace JahanJooy.RealEstateAgency.Api.Web.Controllers
{
    [Contract]
    [Component]
    [ComponentCache(null)]
    [RoutePrefix("configure")]
    public class ConfigureDataItemController : ExtendedApiController
    {
        #region Injected dependencies

        private static readonly ILog Log = LogManager.GetLogger(typeof(ConfigureDataItemController));

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IApplicationSettings ApplicationSettings { get; set; }

        #endregion

        #region Action methods

        [HttpGet, Route("all")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.Administrator})]
        public GetAllOutput GetAllItems()
        {
            var filter = new BsonDocument();
            var result = DbManager.ConfigurationDataItem
                .Find(filter).Project(c => Mapper.Map<ConfigureDataItemSummary>(c)).ToListAsync().Result;

            foreach (var key in ApplicationSettingKeys.RegisteredKeys)
                if (result.All(cdi => cdi.Identifier != key))
                    result.Add(new ConfigureDataItemSummary
                    {
                        Identifier = key,
                        Value = null
                    });

            return new GetAllOutput
            {
                Items = result
            };
        }

        [HttpGet, Route("details")]
        [ApplicationAuthorize(BuiltInRoles = new[] { BuiltInRole.Administrator })]
        public GetOutput Get(string identifier)
        {
            var filter = Builders<ConfigurationDataItem>.Filter.Eq("Identifier", identifier);
            var result = DbManager.ConfigurationDataItem
                .Find(filter).Project(c => Mapper.Map<ConfigureDataItemSummary>(c))
                .SingleOrDefaultAsync().Result ?? new ConfigureDataItemSummary
                {
                    Identifier = identifier,
                    Value = ApplicationSettings[identifier],
                    ID = ObjectId.Empty
                };

            return new GetOutput
            {
                Item = result,
            };
        }

        [HttpPost, Route("remove/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] { BuiltInRole.Administrator })]
        [UserActivity(UserActivityType.Delete, EntityType.ConfigurationDataItem, ApplicationType.Haftdong)]
        public IHttpActionResult Remove(string id)
        {
            if (ObjectId.Parse(id).Equals(ObjectId.Empty))
                return
                    ValidationResult("ConfigurationDataItem", ConfigureDataItemValidationErrors.NotFound);

            UserActivityLogUtils.SetMainActivity(targetId: ObjectId.Parse(id));
            var filter = Builders<ConfigurationDataItem>.Filter.Eq("ID", ObjectId.Parse(id));
            var result = DbManager.ConfigurationDataItem
                .DeleteOneAsync(filter).Result;
            if (result != null)
            {
                ApplicationSettings.Reload();
                return ValidationResult(Common.Util.Validation.ValidationResult.Success);
            }
            return ValidationResult("ConfigurationDataItem", ConfigureDataItemValidationErrors.UnexpectedError);
        }

        [HttpPost, Route("add")]
        [ApplicationAuthorize(BuiltInRoles = new[] { BuiltInRole.Administrator })]
        [UserActivity(UserActivityType.Create, EntityType.ConfigurationDataItem, ApplicationType.Haftdong)]
        public IHttpActionResult SaveItem(AddNewInput input)
        {
            var item = Mapper.Map<ConfigurationDataItem>(input);
            var validationResult = ValidateForSaveOrUpdate(item);
            if (!validationResult.IsValid)
                return ValidationResult(validationResult);

            try
            {
                DbManager.ConfigurationDataItem.InsertOneAsync(item).Wait();
                ApplicationSettings.Reload();
                UserActivityLogUtils.SetMainActivity(targetId: item.ID);
                return ValidationResult(Common.Util.Validation.ValidationResult.Success);
            }
            catch (Exception e)
            {
                Log.Error("An exception has been occured in saving ConfigureDataItem", e);
                return ValidationResult("ConfigurationDataItem", ConfigureDataItemValidationErrors.UnexpectedError);
            }
        }

        [HttpPost, Route("update")]
        [ApplicationAuthorize(BuiltInRoles = new[] { BuiltInRole.Administrator })]
        [UserActivity(UserActivityType.Edit, EntityType.ConfigurationDataItem, ApplicationType.Haftdong)]
        public IHttpActionResult UpdateItem(UpdateInput input)
        {
            ObjectId targetId;
            if (input.ID == ObjectId.Empty)
            {
                var newInput = Mapper.Map<ConfigurationDataItem>(input);
                var validationResult = ValidateForSaveOrUpdate(newInput);
                if (!validationResult.IsValid)
                    return ValidationResult(validationResult);

                try
                {
                    DbManager.ConfigurationDataItem.InsertOneAsync(newInput).Wait();
                    targetId = newInput.ID;
                }
                catch (Exception e)
                {
                    Log.Error("An exception has been occured in updating ConfigureDataItem", e);
                    return ValidationResult("ConfigurationDataItem", ConfigureDataItemValidationErrors.UnexpectedError);
                }
            }
            else
            {
                var item = Mapper.Map<ConfigurationDataItem>(input);
                var validationResult = ValidateForSaveOrUpdate(item);
                if (!validationResult.IsValid)
                    return ValidationResult(validationResult);

                var filter = Builders<ConfigurationDataItem>.Filter.Eq("ID", input.ID);
                var update = Builders<ConfigurationDataItem>.Update
                    .Set("Identifier", item.Identifier)
                    .Set("Value", item.Value);

                var result = DbManager.ConfigurationDataItem.UpdateOneAsync(filter, update);
                if (result.Result.MatchedCount != 1)
                {
                    return ValidationResult("ConfigurationDataItem", ConfigureDataItemValidationErrors.NotFound);
                }

                if (result.Result.MatchedCount == 1 && result.Result.ModifiedCount != 1)
                {
                    return ValidationResult("ConfigurationDataItem", ConfigureDataItemValidationErrors.NotModified);
                }
                targetId = input.ID;
            }

            UserActivityLogUtils.SetMainActivity(targetId: targetId);
            ApplicationSettings.Reload();
            return ValidationResult(Common.Util.Validation.ValidationResult.Success);
        }

        #endregion

        #region Private helper methods 

        private ValidationResult ValidateForSaveOrUpdate(ConfigurationDataItem newItem)
        {
            if (string.IsNullOrEmpty(newItem.Identifier))
                return Common.Util.Validation.ValidationResult.Failure("ConfigurationDataItem.Identifier",
                    GeneralValidationErrors.ValueNotSpecified);

            return Common.Util.Validation.ValidationResult.Success;
        }

        #endregion
    }
}