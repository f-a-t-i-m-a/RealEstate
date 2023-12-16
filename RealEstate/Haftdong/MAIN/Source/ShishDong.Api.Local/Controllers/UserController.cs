using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using Compositional.Composer;
using JahanJooy.RealEstateAgency.Domain.Audit;
using JahanJooy.RealEstateAgency.Domain.Enums.User;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using JahanJooy.RealEstateAgency.Domain.Users;
using JahanJooy.RealEstateAgency.ShishDong.Api.Local.Base;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Models.Account;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using JahanJooy.RealEstateAgency.Util.Models.Users;
using JahanJooy.RealEstateAgency.Util.UserActivityLog;
using JahanJooy.RealEstateAgency.Util.Utils;
using Microsoft.AspNet.Identity;
using MongoDB.Bson;
using MongoDB.Driver;

namespace JahanJooy.RealEstateAgency.ShishDong.Api.Local.Controllers
{
    [Contract]
    [Component]
    [ComponentCache(null)]
    [RoutePrefix("users")]
    public class UserController : ExtendedApiController
    {
        #region Injected dependencies

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IComposer Composer { get; set; }

        [ComponentPlug]
        public UserUtil UserUtil { get; set; }

        #endregion

        #region Action methods

        [Authorize]
        [HttpGet, Route("get/{id}")]
        public ApplicationUserDetails GetUser(string id)
        {
            return UserUtil.GetUserDetail(id);
        }

        [Authorize]
        [HttpGet, Route("myprofile")]
        public ApplicationUserDetails GetMyProfile()
        {
            return UserUtil.GetMyProfile();
        }

        [Authorize]
        [HttpPost, Route("addnewcontactmethod")]
        [UserActivity(UserActivityType.Edit, EntityType.ApplicationUser, ApplicationType.Sheshdong)]
        public async Task<IHttpActionResult> CompleteRegistrationAddNewContactMethod(NewContactInfoInput input)
        {
            UserActivityLogUtils.SetMainActivity(targetId: ObjectId.Parse(User.Identity.GetUserId()));

            var result = await UserUtil.CompleteRegistrationAddNewContactMethodAsync(User.Identity.GetUserId(), input, ApplicationType.Sheshdong);
            if (!result.IsValid)
                return ValidationResult(result);

            UserActivityLogUtils.ReportAdditionalActivity(new UserActivity
            {
                ApplicationType = ApplicationType.Sheshdong,
                TargetType = EntityType.ApplicationUser,
                TargetID = ObjectId.Parse(User.Identity.GetUserId()),
                ActivityType = UserActivityType.Edit,
                ActivitySubType = "InAddingUserContactMethodVerification"
            });

            return Ok(Mapper.Map<ContactInfoSummary>(result.Result));
        }

        [Authorize]
        [HttpPost, Route("verifycontactmethod")]
        [UserActivity(UserActivityType.Edit, EntityType.ApplicationUser, ApplicationType.Sheshdong)]
        public async Task<IHttpActionResult> CompleteRegistrationVerifyContactMethod(UserApplicationVerifyContactMethodInput input)
        {
            var result = await UserUtil.CompleteRegistrationVerifyContactMethod(input);

            if (!result.IsValid)
                return ValidationResult(result);


            UserActivityLogUtils.ReportAdditionalActivity(new UserActivity
            {
                ApplicationType = ApplicationType.Sheshdong,
                TargetType = EntityType.ApplicationUser,
                TargetID = ObjectId.Parse(input.UserID),
                ActivityType = UserActivityType.Edit,
                ActivitySubType = "VerifyContactMethod"
            });

            var output = new CompeleteRegistrationOutput
            {
                Token = result.Result
            };

            return Ok(output);
        }

        [Authorize]
        [HttpPost, Route("getanothersecret")]
        [UserActivity(UserActivityType.Edit, EntityType.ApplicationUser, ApplicationType.Sheshdong)]
        public IHttpActionResult GetAnotherSecret(NewSecretForUserApplicationInput input)
        {
            var result = UserUtil.GetAnotherSecret(input, ApplicationType.Sheshdong);

            if (!result.IsValid)
                return ValidationResult(result);

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost, Route("ForgotPasswordBySms")]
        [UserActivity(UserActivityType.Edit, EntityType.ApplicationUser, ApplicationType.Haftdong)]
        public IHttpActionResult ForgotPasswordBySms(AccountForgotPasswordInput input)
        {
            var result = UserUtil.StartPasswordRecovery(input, ContactMethodType.Phone, ApplicationType.Sheshdong);

            if (!result.IsValid)
                return ValidationResult(result);

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost, Route("ForgotPasswordByEmail")]
        [UserActivity(UserActivityType.Edit, EntityType.ApplicationUser, ApplicationType.Haftdong)]
        public IHttpActionResult ForgotPasswordByEmail(AccountForgotPasswordInput input)
        {
            var result = UserUtil.StartPasswordRecovery(input, ContactMethodType.Email, ApplicationType.Sheshdong);

            if (!result.IsValid)
                return ValidationResult(result);

            return Ok();
        }

        #endregion

        #region Private helper methods 

        #endregion
    }
}