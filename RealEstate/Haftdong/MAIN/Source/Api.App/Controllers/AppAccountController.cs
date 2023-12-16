using System.Web.Http;
using AutoMapper;
using Compositional.Composer;
using JahanJooy.RealEstateAgency.Api.App.Base;
using JahanJooy.RealEstateAgency.Api.App.Models.Users;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using JahanJooy.RealEstateAgency.Domain.Users;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Models.Users;
using JahanJooy.RealEstateAgency.Util.UserActivityLog;
using JahanJooy.RealEstateAgency.Util.Utils;
using JahanJooy.RealEstateAgency.Util.Utils.ContactMethods;
using MongoDB.Bson;
using MongoDB.Driver;
using ServiceStack.Text;

namespace JahanJooy.RealEstateAgency.Api.App.Controllers
{
    [Contract]
    [Component]
    [ComponentCache(null)]
    [RoutePrefix("account")]
    public class AppAccountController : AppExtendedApiController
    {
        #region Injected dependencies

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public AccountUtil AccountUtil { get; set; }

        [ComponentPlug]
        public ElasticManager ElasticManager { get; set; }

        [ComponentPlug]
        public IComposer Composer { get; set; }

        [ComponentPlug]
        public UserUtil UserUtil { get; set; }


        [ComponentPlug]
        public LocalContactMethodUtil LocalContactMethodUtil { get; set; }

        #endregion

        #region Action methods

        [AllowAnonymous]
        [HttpPost, Route("signup")]
        [UserActivity(UserActivityType.Create, EntityType.ApplicationUser, ApplicationType.Haftdong,
            ActivitySubType = "SigningUp")]
        public IHttpActionResult Signup(AppSignupInput input)
        {
            var user = Mapper.Map<ApplicationUser>(input);

            user.Contact = LocalContactMethodUtil.MapContactMethods(input.ContactInfos, input.DisplayName);
            var appInput = Mapper.Map<SignupInput>(input);

            var result = AccountUtil.Signup(appInput, user, ApplicationType.Haftdong);

            if (!result.IsValid)
                return ValidationResult(result);

            var filter = Builders<ApplicationUser>.Filter.Eq("_id", ObjectId.Parse(user.Id));
            var updatedUser = DbManager.ApplicationUser.Find(filter).SingleOrDefaultAsync().Result;

            return Ok(Mapper.Map<ApplicationUserSummary>(updatedUser));
        }


        [AllowAnonymous]
        [HttpPost, Route("resetpassword")]
        [UserActivity(UserActivityType.Edit, EntityType.ApplicationUser, ApplicationType.Haftdong,
            ActivitySubType = "ResetPassword")]
        public IHttpActionResult ResetPassword(AppResetPasswordInput input)
        {
            var performRecoveryResult = AccountUtil.PerformPasswordRecovery(input.UserName, input.PasswordResetToken,
                input.NewPassword, ApplicationType.Haftdong);
            if (!performRecoveryResult.IsValid)
                return ValidationResult(performRecoveryResult);

            return Ok(new JsonObject());
        }

        #endregion

        #region Private helper methods 

        #endregion
    }
}