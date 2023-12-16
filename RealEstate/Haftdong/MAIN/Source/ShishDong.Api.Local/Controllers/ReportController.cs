using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using Compositional.Composer;
using JahanJooy.RealEstateAgency.Domain.Enums.User;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using JahanJooy.RealEstateAgency.Domain.MasterData;
using JahanJooy.RealEstateAgency.ShishDong.Api.Local.Base;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Enums;
using JahanJooy.RealEstateAgency.Util.Identity;
using JahanJooy.RealEstateAgency.Util.Models.Report;
using JahanJooy.RealEstateAgency.Util.UserActivityLog;
using MongoDB.Bson;
using MongoDB.Driver;

namespace JahanJooy.RealEstateAgency.ShishDong.Api.Local.Controllers
{
    [Contract]
    [Component]
    [ComponentCache(null)]
    [RoutePrefix("report")]
    public class ReportController : ExtendedApiController
    {
        #region Injected dependencies

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        #endregion

        #region Action methods

        [AllowAnonymous]
        [HttpGet, Route("list")]
        public ReportListOutput List()
        {
            var filter = new BsonDocument();
            var templates = DbManager.ReportTemplate.Find(filter)
                .Project(p => Mapper.Map<ReportTemplateSummary>(p))
                .ToListAsync().Result;

            return new ReportListOutput
            {
                Templates = templates
            };
        }

        [Authorize]
        [HttpPost, Route("search")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.Administrator})]
        [SkipUserActivityLogging]
        public ReportListOutput Search(SearchReportTemplateInput input)
        {
            var templateFilter = Builders<ReportTemplate>.Filter.Eq("DataSourceType", input.DataSourceType);
            templateFilter &= Builders<ReportTemplate>.Filter.Eq("ApplicationImplementedDataSourceType",
                input.ApplicationImplementedDataSourceType);
            var templates = DbManager.ReportTemplate.Find(templateFilter)
                .Project(r => Mapper.Map<ReportTemplateSummary>(r))
                .ToListAsync().Result;

            return new ReportListOutput
            {
                Templates = templates
            };
        }

        [Authorize]
        [HttpPost, Route("save")]
        [UserActivity(UserActivityType.Create, EntityType.ReportTemplate, ApplicationType.Sheshdong)]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.Administrator })]
        public IHttpActionResult Save(NewReportTemplateInput input)
        {
            var template = Mapper.Map<ReportTemplate>(input);
            DbManager.ReportTemplate.InsertOneAsync(template).Wait();

            UserActivityLogUtils.SetMainActivity(targetId: template.ID);
            return Ok();
        }

        [Authorize]
        [HttpGet, Route("get/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] { BuiltInRole.Administrator })]
        public ReportTemplateSummary GetReport(string id)
        {
            var filter = Builders<ReportTemplate>.Filter.Eq("ID", ObjectId.Parse(id));
            var template = DbManager.ReportTemplate.Find(filter)
                .Project(r => Mapper.Map<ReportTemplateSummary>(r)).SingleOrDefaultAsync().Result;
            return template;
        }

        [Authorize]
        [HttpPost, Route("update")]
        [ApplicationAuthorize(BuiltInRoles = new[] { BuiltInRole.Administrator })]
        [UserActivity(UserActivityType.Edit, EntityType.ReportTemplate, ApplicationType.Sheshdong)]
        public IHttpActionResult UpdateReport(UpdateReportTemplateInput input)
        {
            UserActivityLogUtils.SetMainActivity(targetId: input.ID);

            var filter = Builders<ReportTemplate>.Filter.Eq("ID", input.ID);
            var update = Builders<ReportTemplate>.Update
                    .Set("Name", input.Name)
                    .Set("Key", input.Key)
                    .Set("Description", input.Description)
                    .Set("Definition", input.Definition)
                    .Set("Order", input.Order);

            var result = DbManager.ReportTemplate.UpdateOneAsync(filter, update);
            if (result.Result.MatchedCount != 1)
                return ValidationResult("ReportTemplate", ReportTemplateValidationErrors.NotFound);

            if (result.Result.MatchedCount == 1 && result.Result.ModifiedCount != 1)
                return ValidationResult("ReportTemplate", ReportTemplateValidationErrors.NotModified);

            return ValidationResult(Common.Util.Validation.ValidationResult.Success);
        }

        [Authorize]
        [HttpGet, Route("delete/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] { BuiltInRole.RealEstateAgent })]
        public IHttpActionResult DeleteReport(string id)
        {
            var filter = Builders<ReportTemplate>.Filter.Eq("ID", ObjectId.Parse(id));
            var result = DbManager.ReportTemplate.DeleteOneAsync(filter).Result;
            if (result.DeletedCount != 1)
                return ValidationResult("ReportTemplate", ReportTemplateValidationErrors.UnexpectedError);
            return ValidationResult(Common.Util.Validation.ValidationResult.Success);
        }

        [Authorize]
        [HttpPost, Route("addparameter")]
        [ApplicationAuthorize(BuiltInRoles = new[] { BuiltInRole.Administrator })]
        [UserActivity(UserActivityType.Edit, EntityType.ReportTemplate, ApplicationType.Sheshdong)]
        public IHttpActionResult AddParameter(AddParameterInput input)
        {
            UserActivityLogUtils.SetMainActivity(targetId: input.ID, activitySubType:"addingParameter");

            var filter = Builders<ReportTemplate>.Filter.Eq("ID", input.ID);
            var update = Builders<ReportTemplate>.Update.AddToSet("Parameters", input.Parameter);

            var result = DbManager.ReportTemplate.UpdateOneAsync(filter, update);
            if (result.Result.MatchedCount != 1)
                return ValidationResult("ReportTemplate", ReportTemplateValidationErrors.NotFound);

            if (result.Result.MatchedCount == 1 && result.Result.ModifiedCount != 1)
                return ValidationResult("ReportTemplate", ReportTemplateValidationErrors.NotModified);

            return ValidationResult(Common.Util.Validation.ValidationResult.Success);
        }

        [Authorize]
        [HttpPost, Route("setparameters")]
        [ApplicationAuthorize(BuiltInRoles = new[] { BuiltInRole.Administrator })]
        [UserActivity(UserActivityType.Edit, EntityType.ReportTemplate, ApplicationType.Sheshdong)]
        public IHttpActionResult SetParameters(SetParametersInput input)
        {
            UserActivityLogUtils.SetMainActivity(targetId: input.ID, activitySubType:"EdittingParameters");

            var filter = Builders<ReportTemplate>.Filter.Eq("ID", input.ID);
            var update = Builders<ReportTemplate>.Update.Set("Parameters", input.Parameters);

            var result = DbManager.ReportTemplate.UpdateOneAsync(filter, update);
            if (result.Result.MatchedCount != 1)
                return ValidationResult("ReportTemplate", ReportTemplateValidationErrors.NotFound);

            if (result.Result.MatchedCount == 1 && result.Result.ModifiedCount != 1)
                return ValidationResult("ReportTemplate", ReportTemplateValidationErrors.NotModified);

            return ValidationResult(Common.Util.Validation.ValidationResult.Success);
        }

        [Authorize]
        [HttpPost, Route("removeparameter")]
        [ApplicationAuthorize(BuiltInRoles = new[] { BuiltInRole.Administrator })]
        [UserActivity(UserActivityType.Edit, EntityType.ReportTemplate, ApplicationType.Sheshdong)]
        public IHttpActionResult RemoveParameter(RemoveParameterInput input)
        {
            UserActivityLogUtils.SetMainActivity(targetId: input.ReportTemplateID, activitySubType:"RemovingParameter");

            var filter = Builders<ReportTemplate>.Filter.Eq("ID", input.ReportTemplateID);
            var update = Builders<ReportTemplate>.Update.PullFilter("Parameters",
                Builders<ReportTemplateParameter>.Filter.Eq("ID", input.ParameterID));
            var result = DbManager.ReportTemplate.UpdateOneAsync(filter, update);
            if (result.Result.MatchedCount != 1)
                return ValidationResult("ReportTemplate", ReportTemplateValidationErrors.NotFound);

            if (result.Result.MatchedCount == 1 && result.Result.ModifiedCount != 1)
                return ValidationResult("ReportTemplate", ReportTemplateValidationErrors.NotModified);

            return ValidationResult(Common.Util.Validation.ValidationResult.Success);
        }

        [Authorize]
        [HttpGet, Route("allreporttemplateparameter/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.Administrator})]
        public ReportTemplateParameterOutput GetAllReportTemplateParameter(string id)
        {
            return new ReportTemplateParameterOutput
            {
                ReportTemplateParameterSummaries = LoadReportTemplateParameter(id)
            };
        }

        #endregion

        #region Private helper methods 

        private List<ReportTemplateParameterSummary> LoadReportTemplateParameter(string id)
        {
            var filter = Builders<ReportTemplate>.Filter.Eq("ID", ObjectId.Parse(id));
            var template = DbManager.ReportTemplate.Find(filter).SingleOrDefaultAsync().Result;

            var reportTemplateParameterSummaries =
                template.Parameters.Select(Mapper.Map<ReportTemplateParameterSummary>).ToList();
            return reportTemplateParameterSummaries;
        }

        #endregion
    }
}