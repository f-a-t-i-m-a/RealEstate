using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstateAgency.Api.App.Base;
using JahanJooy.RealEstateAgency.Api.App.Models.Property;
using JahanJooy.RealEstateAgency.Api.App.Models.Request;
using JahanJooy.RealEstateAgency.Api.App.Models.Supply;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using JahanJooy.RealEstateAgency.Domain.Property;
using JahanJooy.RealEstateAgency.Domain.Request;
using JahanJooy.RealEstateAgency.Domain.Supply;
using JahanJooy.RealEstateAgency.Domain.Workflows;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Enums;
using JahanJooy.RealEstateAgency.Util.Interfaces;
using JahanJooy.RealEstateAgency.Util.Report;
using JahanJooy.RealEstateAgency.Util.Report.ApplicationImplemented;
using JahanJooy.RealEstateAgency.Util.UserActivityLog;
using JahanJooy.RealEstateAgency.Util.Utils;
using JahanJooy.RealEstateAgency.Util.VicinityCache;
using log4net;
using MongoDB.Bson;
using MongoDB.Driver;
using ServiceStack.Text;

namespace JahanJooy.RealEstateAgency.Api.App.Controllers
{
    [Contract]
    [Component]
    [ComponentCache(null)]
    [RoutePrefix("supplies")]
    public class AppSupplyController : AppExtendedApiController
    {
        #region Injected dependencies

        private static readonly ILog Log = LogManager.GetLogger(typeof(AppSupplyController));

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public SupplyUtil SupplyUtil { get; set; }

        [ComponentPlug]
        public PropertyUtil PropertyUtil { get; set; }

        [ComponentPlug]
        public VicinityUtil VicinityUtil { get; set; }

        [ComponentPlug]
        public IComposer Composer { get; set; }

        [ComponentPlug]
        public SupplyReport Report { get; set; }

        [ComponentPlug]
        public ReportRepository ReportRepository { get; set; }

        [ComponentPlug]
        public IVicinityCache VicinityCache { get; set; }

        #endregion

        #region Action methods

        [HttpPost, Route("update")]
        [UserActivity(UserActivityType.Edit, EntityType.Supply, ApplicationType.Haftdong)]
        public IHttpActionResult UpdateSupply(AppUpdateSupplyInput input)
        {
            UserActivityLogUtils.SetMainActivity(targetId: input.ID);
            var supplyFilter = Builders<Supply>.Filter.Eq("ID", input.ID);
            var newSupply = Mapper.Map<Supply>(input);

            var propertyFilter = Builders<Property>.Filter.Eq("Supplies.ID", input.ID);
            var property = DbManager.Property.Find(propertyFilter).SingleOrDefaultAsync();

            if (property?.Result == null)
                return
                    ValidationResult("Property", PropertyValidationErrors.NotFound);

            var validationResult = ValidateForSaveOrUpdate(newSupply, property.Result);
            if (!validationResult.IsValid)
                return ValidationResult(validationResult);

            var updateSupply = Builders<Supply>.Update
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("LastModificationTime", DateTime.Now)
                .Set("IntentionOfOwner", input.IntentionOfOwner)
                .Set("PriceSpecificationType", input.PriceSpecificationType)
                .Set("TotalPrice", input.TotalPrice)
                .Set("PricePerEstateArea", input.PricePerEstateArea)
                .Set("PricePerUnitArea", input.PricePerUnitArea)
                .Set("HasTransferableLoan", input.HasTransferableLoan)
                .Set("TransferableLoanAmount", input.TransferableLoanAmount)
                .Set("Mortgage", input.Mortgage)
                .Set("Rent", input.Rent)
                .Set("MortgageAndRentConvertible", input.MortgageAndRentConvertible)
                .Set("MinimumMortgage", input.MinimumMortgage)
                .Set("MinimumRent", input.MinimumRent);

            var result = DbManager.Supply.UpdateOneAsync(supplyFilter, updateSupply);
            if (result.Result.MatchedCount != 1)
                return ValidationResult("Supply", SupplyValidationErrors.NotFound);

            if (result.Result.MatchedCount == 1 && result.Result.ModifiedCount != 1)
                return ValidationResult("Supply", SupplyValidationErrors.NotModified);

            var propertyUpdate = Builders<Property>.Update
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("Supplies.$.IntentionOfOwner", input.IntentionOfOwner)
                .Set("Supplies.$.PriceSpecificationType", input.PriceSpecificationType)
                .Set("Supplies.$.TotalPrice", input.TotalPrice)
                .Set("Supplies.$.PricePerEstateArea", input.PricePerEstateArea)
                .Set("Supplies.$.PricePerUnitArea", input.PricePerUnitArea)
                .Set("Supplies.$.Mortgage", input.Mortgage)
                .Set("Supplies.$.Rent", input.Rent)
                .Set("Supplies.$.HasTransferableLoan", input.HasTransferableLoan)
                .Set("Supplies.$.TransferableLoanAmount", input.TransferableLoanAmount)
                .Set("Supplies.$.MortgageAndRentConvertible", input.MortgageAndRentConvertible)
                .Set("Supplies.$.MinimumMortgage", input.MinimumMortgage)
                .Set("Supplies.$.MinimumRent", input.MinimumRent);

            property = DbManager.Property.FindOneAndUpdateAsync(propertyFilter, propertyUpdate);
            if (property?.Result == null)
                return
                    ValidationResult("Property", PropertyValidationErrors.NotFound);

            UserActivityLogUtils.SetMainActivity(
                parentType: EntityType.Property,
                parentId: property.Result.ID);
            Log.InfoFormat(
                "Supply reference(s) in related property with id {0} has been updated in updating supply process",
                property.Result.ID);

            return Ok(new JsonObject());
        }

        [HttpGet, Route("get/{id}")]
        public AppSupplyDetails GetSupply(string id)
        {
            var filter = Builders<Supply>.Filter.Eq("ID", ObjectId.Parse(id));
            var supply =
                DbManager.Supply.Find(filter)
                    .Project(p => Mapper.Map<AppSupplyDetails>(p))
                    .SingleOrDefaultAsync()
                    .Result;

            var propertyFilter = Builders<Property>.Filter.Eq("Supplies.ID", supply.ID);
            supply.PropertyDetail = DbManager.Property.Find(propertyFilter)
                .Project(p => Mapper.Map<AppPropertyDetails>(p))
                .SingleOrDefaultAsync()
                .Result;

            if (supply.Request != null)
            {
                var requestFilter = Builders<Request>.Filter.Eq("ID", supply.Request.ID);
                supply.Request = DbManager.Request.Find(requestFilter)
                    .Project(r => Mapper.Map<AppRequestDetails>(r))
                    .SingleOrDefaultAsync()
                    .Result;
            }

            if (supply.PropertyDetail.Vicinity != null)
            {
                supply.PropertyDetail.Vicinity.CompleteName = VicinityUtil.GetFullName(supply.PropertyDetail.Vicinity.ID);
                if (supply.PropertyDetail.Vicinity.CenterPoint == null &&
                    supply.PropertyDetail.Vicinity.ParentID != null)
                {
                    supply.PropertyDetail.Vicinity.CenterPoint =
                        VicinityUtil.GetParentCenterPoint(supply.PropertyDetail.Vicinity.ParentID.Value);
                }
            }

            supply.PropertyDetail.Photos = supply.PropertyDetail.Photos.Where(p => p.DeletionTime == null).ToList();

            return supply;
        }

        [HttpPost, Route("delete/{id}")]
        [UserActivity(UserActivityType.ChangeState, EntityType.Supply, ApplicationType.Haftdong,
            TargetState = SupplyState.Deleted)]
        public IHttpActionResult DeleteSupply(string id)
        {
            UserActivityLogUtils.SetMainActivity(targetId: ObjectId.Parse(id));
            return ValidationResult(SupplyUtil.DeleteSupply(id, false));
        }

        #endregion

        #region Private helper methods 

        private ValidationResult ValidateForSaveOrUpdate(Supply supply, Property property)
        {
            var errors = new List<ValidationError>();
            var allValidators = Composer.GetAllComponents<ISupplyValidatorForSave>();
            allValidators.ForEach(v =>
            {
                var result = v.Validate(supply, property);
                if (result != Common.Util.Validation.ValidationResult.Success)
                {
                    errors.AddRange(result.Errors);
                }
            });

            return new ValidationResult {Errors = errors};
        }

        #endregion
    }
}