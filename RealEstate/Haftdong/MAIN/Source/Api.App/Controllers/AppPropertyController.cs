using System.Web.Http;
using AutoMapper;
using Compositional.Composer;
using JahanJooy.Common.Util.Validation;
using JahanJooy.Common.Util.Web.Result;
using JahanJooy.RealEstateAgency.Api.App.Base;
using JahanJooy.RealEstateAgency.Api.App.Models.Property;
using JahanJooy.RealEstateAgency.Domain.Customers;
using JahanJooy.RealEstateAgency.Domain.Enums.Property;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using JahanJooy.RealEstateAgency.Domain.Property;
using JahanJooy.RealEstateAgency.Domain.Request;
using JahanJooy.RealEstateAgency.Domain.Supply;
using JahanJooy.RealEstateAgency.Domain.Vicinities;
using JahanJooy.RealEstateAgency.Domain.Workflows;
using JahanJooy.RealEstateAgency.Util.Attachments;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Enums;
using JahanJooy.RealEstateAgency.Util.Models.Properties;
using JahanJooy.RealEstateAgency.Util.UserActivityLog;
using JahanJooy.RealEstateAgency.Util.Utils;
using JahanJooy.RealEstateAgency.Util.Utils.ContactMethods;
using JahanJooy.RealEstateAgency.Util.VicinityCache;
using MongoDB.Bson;
using ServiceStack.Text;

namespace JahanJooy.RealEstateAgency.Api.App.Controllers
{
    [Contract]
    [Component]
    [ComponentCache(null)]
    [RoutePrefix("properties")]
    public class AppPropertyController : AppExtendedApiController
    {
        #region Injected dependencies

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public LocalPhoneNumberUtil LocalPhoneNumberUtil { get; set; }

        [ComponentPlug]
        public LocalContactMethodUtil LocalContactMethodUtil { get; set; }

        [ComponentPlug]
        public PropertyUtil PropertyUtil { get; set; }

        [ComponentPlug]
        public IVicinityCache VicinityCache { get; set; }

        [ComponentPlug]
        public PhotoGridStore PhotoGridStore { get; set; }

        [ComponentPlug]
        public FileUtil FileUtil { get; set; }

        #endregion

        #region Action methods

        [HttpPost, Route("save")]
        [UserActivity(UserActivityType.Create, EntityType.Property, ApplicationType.Haftdong)]
        public IHttpActionResult Save(AppNewPropertyInput input)
        {
            var owner = Mapper.Map<Customer>(input.Owner);
            owner.Contact = LocalContactMethodUtil.MapContactMethods(input.Owner.ContactInfos, owner.DisplayName);

            var property = Mapper.Map<Property>(input);
            var supply = Mapper.Map<Supply>(input);
            var request = Mapper.Map<Request>(input);

            if (input.Vicinity?.ID != null && input.Vicinity?.ID != ObjectId.Empty)
            {
                var vicinity = VicinityCache[input.Vicinity.ID];
                var vicinityRefrence = Mapper.Map<VicinityReference>(vicinity);
                property.Vicinity = vicinityRefrence;
            }
            else
            {
                property.Vicinity = null;
            }

            var result = FileUtil.SaveFile(property, supply, owner, request, input.IsPublic, SourceType.Haftdong, ApplicationType.Haftdong);
            if (!result.IsValid)
                return ValidationResult(result);

            UserActivityLogUtils.SetMainActivity(targetId: property.ID);
            return Ok(new JsonObject());
        }

        [HttpPost, Route("update")]
        [UserActivity(UserActivityType.Edit, EntityType.Property, ApplicationType.Haftdong)]
        public IHttpActionResult UpdateProperty(AppUpdatePropertyInput input)
        {
            UserActivityLogUtils.SetMainActivity(targetId: input.ID);

            var updateInput = Mapper.Map<UpdatePropertyInput>(input);
            return ValidationResult(PropertyUtil.UpdateProperty(updateInput, false));
        }

        [HttpGet, Route("getThumbnail/{id}")]
        public IHttpActionResult GetThumbnail(string id)
        {
            return new BinaryResult(PhotoGridStore.GetAttchmentBytes(AttachmentStoreEntityType.Image,
                ObjectId.Parse(id), PhotoStoreSize.Thumbnail), "image/jpeg");
        }

        [HttpGet, Route("getMediumSize/{id}")]
        public IHttpActionResult GetMedium(string id)
        {
            return new BinaryResult(PhotoGridStore.GetAttchmentBytes(AttachmentStoreEntityType.Image,
                ObjectId.Parse(id), PhotoStoreSize.MediumSize), "image/jpeg");
        }

        [HttpGet, Route("getFullSize/{id}")]
        public IHttpActionResult GetFullSize(string id)
        {
            return new BinaryResult(PhotoGridStore.GetAttchmentBytes(AttachmentStoreEntityType.Image,
                ObjectId.Parse(id), PhotoStoreSize.FullSize), "image/jpeg");
        }

        [HttpGet, Route("getContactInfos/{id}")]
        public AppPropertyContactInfoSummary GetContactInfos(string id)
        {
            return Mapper.Map<AppPropertyContactInfoSummary>(PropertyUtil.GetContactInfos(id, false));
        }

        [HttpPost, Route("delete/{id}")]
        [UserActivity(UserActivityType.ChangeState, EntityType.Property, ApplicationType.Haftdong,
            TargetState = PropertyState.Deleted)]
        public IHttpActionResult DeleteProperty(string id)
        {
            UserActivityLogUtils.SetMainActivity(targetId: ObjectId.Parse(id));

            return ValidationResult(PropertyUtil.DeleteProperty(id, false, ApplicationType.Haftdong));
        }

        #endregion

        #region Private helper methods 

        public ValidationResult ValidateForDelete(Property property)
        {
            if (property.State != PropertyState.New)
            {
                return Common.Util.Validation.ValidationResult.Failure("Property.State",
                    PropertyValidationErrors.IsNotValid);
            }
            return Common.Util.Validation.ValidationResult.Success;
        }

        #endregion
    }
}