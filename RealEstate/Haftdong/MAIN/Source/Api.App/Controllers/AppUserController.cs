using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using Compositional.Composer;
using JahanJooy.RealEstateAgency.Api.App.Base;
using JahanJooy.RealEstateAgency.Api.App.Models.Base;
using JahanJooy.RealEstateAgency.Api.App.Models.Users;
using JahanJooy.RealEstateAgency.Domain.Audit;
using JahanJooy.RealEstateAgency.Domain.Enums.User;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Identity;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using JahanJooy.RealEstateAgency.Util.Models.Users;
using JahanJooy.RealEstateAgency.Util.Notification;
using JahanJooy.RealEstateAgency.Util.UserActivityLog;
using JahanJooy.RealEstateAgency.Util.Utils;
using Microsoft.AspNet.Identity;
using MongoDB.Bson;
using ServiceStack.Text;

namespace JahanJooy.RealEstateAgency.Api.App.Controllers
{
    [Contract]
    [Component]
    [ComponentCache(null)]
    [RoutePrefix("users")]
    public class AppUserController : AppExtendedApiController
    {
        #region Injected dependencies

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public EmailNotificationUtils EmailNotificationUtils { get; set; }

        [ComponentPlug]
        public SmsNotificationUtils SmsNotificationService { get; set; }

        [ComponentPlug]
        public UserUtil UserUtil { get; set; }

        #endregion

        #region Action methods

        [HttpPost, Route("addnewcontactmethod")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.Edit, EntityType.ApplicationUser, ApplicationType.Haftdong)]
        public async Task<IHttpActionResult> CompleteRegistrationAddNewContactMethod(AppNewContactInfoInput input)
        {
            UserActivityLogUtils.SetMainActivity(targetId: ObjectId.Parse(User.Identity.GetUserId()));

            var appInput = Mapper.Map<NewContactInfoInput>(input);
            var result = await UserUtil.CompleteRegistrationAddNewContactMethodAsync(User.Identity.GetUserId(), appInput, ApplicationType.Haftdong);

            if (!result.IsValid)
                return ValidationResult(result);

            UserActivityLogUtils.ReportAdditionalActivity(new UserActivity
            {
                ApplicationType = ApplicationType.Haftdong,
                TargetType = EntityType.ApplicationUser,
                TargetID = ObjectId.Parse(User.Identity.GetUserId()),
                ActivityType = UserActivityType.Edit,
                ActivitySubType = "InAddingUserContactMethodVerification"
            });

            return Ok(Mapper.Map<AppContactInfoSummary>(result.Result));
        }

        [HttpPost, Route("verifycontactmethod")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.Edit, EntityType.ApplicationUser, ApplicationType.Haftdong)]
        public async Task<IHttpActionResult> CompleteRegistrationVerifyContactMethod(AppUserApplicationVerifyContactMethodInput input)
        {
            var appInput = Mapper.Map<UserApplicationVerifyContactMethodInput>(input);
            appInput.UserID = User.Identity.GetUserId();

            var result = await UserUtil.CompleteRegistrationVerifyContactMethod(appInput);
            if (!result.IsValid)
                return ValidationResult(result);

            UserActivityLogUtils.ReportAdditionalActivity(new UserActivity
            {
                ApplicationType = ApplicationType.Haftdong,
                TargetType = EntityType.ApplicationUser,
                TargetID = ObjectId.Parse(User.Identity.GetUserId()),
                ActivityType = UserActivityType.Edit,
                ActivitySubType = "VerifyContactMethod"
            });
            return Ok();
        }

        [HttpPost, Route("getanothersecret")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.Edit, EntityType.ApplicationUser, ApplicationType.Haftdong)]
        public IHttpActionResult GetAnotherSecret(AppNewSecretForUserApplicationInput input)
        {
            var appInput = Mapper.Map<NewSecretForUserApplicationInput>(input);
            var result = UserUtil.GetAnotherSecret(appInput, ApplicationType.Haftdong);

            if (!result.IsValid)
                return ValidationResult(result);

            return Ok(new JsonObject());
        }

        [AllowAnonymous]
        [HttpPost, Route("ForgotPasswordBySms")]
        [UserActivity(UserActivityType.Edit, EntityType.ApplicationUser, ApplicationType.Haftdong)]
        public IHttpActionResult ForgotPasswordBySms(AppAccountForgotPasswordInput input)
        {
            var appInput = Mapper.Map<AccountForgotPasswordInput>(input);
            var result = UserUtil.StartPasswordRecovery(appInput, ContactMethodType.Phone, ApplicationType.Haftdong);

            if (!result.IsValid)
                return ValidationResult(result);

            return Ok(new JsonObject());
        }

        #endregion

        #region Private helper methods 

        #endregion
    }
}