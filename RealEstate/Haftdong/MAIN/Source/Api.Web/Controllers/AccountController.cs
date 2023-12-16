using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using AspNet.Identity.MongoDB;
using AutoMapper;
using Compositional.Composer;
using JahanJooy.Common.Util.Identity;
using JahanJooy.Common.Util.Owin;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstateAgency.Api.Web.Base;
using JahanJooy.RealEstateAgency.Domain.Enums.User;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using JahanJooy.RealEstateAgency.Domain.Users;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Enums;
using JahanJooy.RealEstateAgency.Util.Identity;
using JahanJooy.RealEstateAgency.Util.Models.Account;
using JahanJooy.RealEstateAgency.Util.Models.Users;
using JahanJooy.RealEstateAgency.Util.UserActivityLog;
using JahanJooy.RealEstateAgency.Util.Utils;
using JahanJooy.RealEstateAgency.Util.Utils.ContactMethods;
using Microsoft.AspNet.Identity;
using MongoDB.Bson;

namespace JahanJooy.RealEstateAgency.Api.Web.Controllers
{
    [Contract]
    [Component]
    [ComponentCache(null)]
    [RoutePrefix("account")]
    public class AccountController : ExtendedApiController
    {
        #region Injected dependencies

        [ComponentPlug]
        public AccountUtil AccountUtil { get; set; }

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public ElasticManager ElasticManager { get; set; }

        [ComponentPlug]
        public LocalContactMethodUtil LocalContactMethodUtil { get; set; }

        [ComponentPlug]
        public IComposer Composer { get; set; }

        #endregion

        #region Action methods

        [HttpPost, Route("changepassword")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.Edit, EntityType.ApplicationUser, ApplicationType.Haftdong,
            ActivitySubType = "ChangingPassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordInputModel model)
        {
            UserActivityLogUtils.SetMainActivity(targetId: ObjectId.Parse(OwinRequestScopeContext.Current.GetUserId()));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var store = new UserStore<ApplicationUser>(DbManager.ApplicationUser);
            var userManager = new UserManager<ApplicationUser>(store);
            var result = await userManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        [HttpPost, Route("resetpasswordbyadmin")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.Administrator})]
        [UserActivity(UserActivityType.Edit, EntityType.ApplicationUser, ApplicationType.Haftdong,
            ActivitySubType = "ResettingPassword")]
        public async Task<IHttpActionResult> ResetPassword(ResetPasswordInput model)
        {
            UserActivityLogUtils.SetMainActivity(targetId: ObjectId.Parse(OwinRequestScopeContext.Current.GetUserId()));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var store = new UserStore<ApplicationUser>(DbManager.ApplicationUser);
            var userManager = new UserManager<ApplicationUser>(store);
            userManager.RemovePassword(model.UserID);

            var result = await userManager.AddPasswordAsync(model.UserID, model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        [HttpGet, Route("permissions")]
        public IHttpActionResult GetAccountPermissions()
        {
            var store = new UserStore<ApplicationUser>(DbManager.ApplicationUser);
            var userManager = new UserManager<ApplicationUser>(store);

            var user = userManager.FindByIdAsync(User.Identity.GetUserId()).Result;
            var roleNames = userManager.GetRolesAsync(User.Identity.GetUserId()).Result;

            var output = new GetAccountPermissionsOutput
            {
                User = Mapper.Map<ApplicationUserSummary>(user),
                Roles = new List<BuiltInRole>(),
                RoleNames = roleNames.ToList()
            };

            foreach (var roleName in roleNames)
            {
                BuiltInRole role;
                if (Enum.TryParse(roleName, true, out role))
                    output.Roles.Add(role);
            }

            return Ok(output);
        }

        [AllowAnonymous]
        [HttpPost, Route("signup")]
        [UserActivity(UserActivityType.Create, EntityType.ApplicationUser, ApplicationType.Haftdong,
            ActivitySubType = "SigningUp")]
        public IHttpActionResult Signup(SignupInput input)
        {
            var user = Mapper.Map<ApplicationUser>(input);
            user.Contact = LocalContactMethodUtil.MapContactMethods(input.ContactInfos, input.DisplayName);
            var contactResult = LocalContactMethodUtil.PrepareContactMethods(user.Contact, false, false, false);
            if (!contactResult.IsValid)
                return ValidationResult(contactResult);

            var errors = ValidateUserPhoneNumberForSignup(user);

            if (errors.Count > 0)
                return ValidationResult(new ValidationResult
                {
                    Errors = errors
                });

            var result = AccountUtil.Signup(input, user, ApplicationType.Haftdong);

            if (!result.IsValid)
                return ValidationResult(result);

            return Ok(Mapper.Map<ApplicationUserSummary>(user));
        }

        [AllowAnonymous]
        [HttpPost, Route("resetpassword")]
        [UserActivity(UserActivityType.Edit, EntityType.ApplicationUser, ApplicationType.Haftdong,
            ActivitySubType = "ResetPassword")]
        public IHttpActionResult ResetPasswordWithPasswordResetToken(ResetPasswordInput input)
        {
            var performRecoveryResult = AccountUtil.PerformPasswordRecovery(input.UserName, input.PasswordResetToken,
                input.NewPassword, ApplicationType.Haftdong);
            if (!performRecoveryResult.IsValid)
                return ValidationResult(performRecoveryResult);

            return Ok();
        }

        #endregion

        #region Private helper methods 

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private List<ValidationError> ValidateUserPhoneNumberForSignup(ApplicationUser user)
        {
            var errors = new List<ValidationError>();
            if (string.IsNullOrWhiteSpace(user.DisplayName))
                errors.Add(new ValidationError("User.DisplayName", GeneralValidationErrors.ValueNotSpecified));
            if (user.Contact?.Phones?.Count > 0 && (bool) !user.Contact?.Phones[0].CanReceiveSms)
                errors.Add(new ValidationError("User.Contact.Phones",
                    AuthenticationValidationErrors.ContactMethodShouldBeVerifyable));
            if (user.Contact?.Phones?.Count <= 0)
                errors.Add(new ValidationError("User.Contact.Phones", GeneralValidationErrors.ValueNotSpecified));

            return errors;
        }

        #endregion
    }
}