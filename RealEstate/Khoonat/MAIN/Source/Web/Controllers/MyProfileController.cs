using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.WebPages;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.Common.Util.Web.Captcha;
using JahanJooy.Common.Util.Web.Upload;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Core.Services.Dto;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Audit;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Domain.Property;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Application.Security;
using JahanJooy.RealEstate.Web.Models.MyProfile;
using JahanJooy.RealEstate.Web.Models.Shared.Profile;
using JahanJooy.RealEstate.Web.Resources.Controllers.Account;

namespace JahanJooy.RealEstate.Web.Controllers
{
    [Authorize]
    [RequireHttps]
    public class MyProfileController : CustomControllerBase
    {
        #region Injected dependencies

        [ComponentPlug]
        public IAuthenticationService AuthenticationService { get; set; }

        [ComponentPlug]
        public IPropertyService PropertyService { get; set; }

        [ComponentPlug]
        public IPrincipalCache PrincipalCache { get; set; }

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IUserService UserService { get; set; }

        #endregion

        [HttpGet]
        [ActionName("View")]
        public ActionResult ViewAction(string activeTab, string page)
        {
            var activeTabEnum = ProfileModel.ProfileActiveTab.General;
            if (!string.IsNullOrWhiteSpace(activeTab))
                Enum.TryParse(activeTab, out activeTabEnum);

            int pageNum = 1;
            if (!string.IsNullOrWhiteSpace(page))
                int.TryParse(page, out pageNum);

            var model = new ProfileModel
            {
                ActiveTab = activeTabEnum,
                User = AuthenticationService.LoadCurrentUserInfo(),
                PaginationUrlTemplate = Url.Action("View", new {activeTab = activeTabEnum.ToString()}) + "&page={0}",
                EnableAdmin = false,
                EnableEdit = true
            };

            switch (activeTabEnum)
            {
                case ProfileModel.ProfileActiveTab.General:
                    break;

                case ProfileModel.ProfileActiveTab.AllPropertyListings:
                case ProfileModel.ProfileActiveTab.PublishedPropertyListings:
                    model.PropertyListings = PropertyService.ListingsOfUser(User.CoreIdentity.UserId.Value, false,
                        activeTabEnum == ProfileModel.ProfileActiveTab.PublishedPropertyListings, pageNum, 20);
                    break;

                case ProfileModel.ProfileActiveTab.PublishHistory:
                    var historiesQuery =
                        DbManager.Db.PropertyListingPublishHistories.Where(
                            h => h.UserID == User.CoreIdentity.UserId.Value);
                    model.PropertyPublishes =
                        PagedList<PropertyListingPublishHistory>.BuildUsingPageNumber(historiesQuery.Count(), 20,
                            pageNum);
                    model.PropertyPublishes.FillFrom(historiesQuery.OrderByDescending(h => h.ID));
                    break;

                case ProfileModel.ProfileActiveTab.SecurityInfo:
                    var sessionsQuery = DbManager.Db.HttpSessions.Where(l => l.UserID == User.CoreIdentity.UserId);
                    model.Sessions = PagedList<HttpSession>.BuildUsingPageNumber(sessionsQuery.Count(), 20, pageNum);
                    model.Sessions.FillFrom(sessionsQuery.OrderByDescending(s => s.ID));
                    model.LatestPasswordResets =
                        DbManager.Db.PasswordResetRequests.Where(l => l.TargetUserID == User.CoreIdentity.UserId.Value)
                            .Take(3);
                    model.LatestLoginNameQueries =
                        DbManager.Db.LoginNameQueries.Where(l => l.TargetUserID == User.CoreIdentity.UserId.Value)
                            .Take(3);
                    break;

                default:
                    return Error(ErrorResult.EntityNotFound);
            }

            return View(model);
        }

        public ActionResult CompleteRegistration(string verifiedEmail, bool? isSuccessful)
        {
            if (User.IsVerified && !verifiedEmail.IsEmpty() && isSuccessful != null)
                return RedirectToAction("View");

            UserContactMethodVerificationInfo phoneVerificationInfo = null;
            UserContactMethodVerificationInfo emailVerificationInfo = null;

            var phoneId = DbManager.Db.UserContactMethods
                .Where(
                    cm =>
                        cm.UserID == User.CoreIdentity.UserId && cm.IsVerifiable && !cm.IsDeleted &&
                        cm.ContactMethodType == ContactMethodType.Phone)
                .OrderByDescending(cm => cm.IsVerified)
                .ThenBy(cm => cm.ID)
                .Select(cm => (long?) cm.ID)
                .FirstOrDefault();

            var emailId = DbManager.Db.UserContactMethods
                .Where(
                    cm =>
                        cm.UserID == User.CoreIdentity.UserId && cm.IsVerifiable && !cm.IsDeleted &&
                        cm.ContactMethodType == ContactMethodType.Email)
                .OrderByDescending(cm => cm.IsVerified)
                .ThenBy(cm => cm.ID)
                .Select(cm => (long?) cm.ID)
                .FirstOrDefault();

            if (phoneId.HasValue)
                phoneVerificationInfo = AuthenticationService.LoadContactMethodVerificationInfo(phoneId.Value);

            if (emailId.HasValue)
                emailVerificationInfo = AuthenticationService.LoadContactMethodVerificationInfo(emailId.Value);

            var model = new MyProfileVerifyModel
            {
                PhoneVerificationInfo = phoneVerificationInfo,
                EmailVerificationInfo = emailVerificationInfo
            };
            model.IsSuccessful = isSuccessful;
            model.VerifiedEmail = verifiedEmail;
            return View(model);
        }

        [HttpPost]
        [ValidateCaptcha(validateScriptCaptcha: true)]
        public ActionResult CompleteRegistrationAddPhoneNumber(MyProfileAddPhoneNumberModel model)
        {
            return AddPhoneNumberInternal(model, "CompleteRegistrationPartials/AddPhoneNumber",
                "CompleteRegistrationPartials/VerifyPhoneNumber", true);
        }

        [HttpPost]
        [ValidateCaptcha(validateScriptCaptcha: true)]
        public ActionResult CompleteRegistrationAddEmailAddress(MyProfileAddEmailModel model)
        {
            return AddEmailAddressInternal(model, "CompleteRegistrationPartials/AddEmailAddress",
                "CompleteRegistrationPartials/VerifyEmailAddress", true);
        }

        [HttpPost]
        [ValidateCaptcha(validateScriptCaptcha: true)]
        public ActionResult CompleteRegistrationVerifyPhoneNumber(MyProfileContactMethodVerificationModel model)
        {
            return PerformVerificationInternal(model, "CompleteRegistrationPartials/VerifyPhoneNumber",
                "CompleteRegistrationPartials/PhoneNumberOkay");
        }

        [HttpPost]
        [ValidateCaptcha(validateScriptCaptcha: true)]
        public ActionResult CompleteRegistrationVerifyEmailAddress(MyProfileContactMethodVerificationModel model)
        {
            return PerformVerificationInternal(model, "CompleteRegistrationPartials/VerifyEmailAddress",
                "CompleteRegistrationPartials/EmailAddressOkay");
        }

        [HttpPost]
        public ActionResult CompleteRegistrationRestartPhoneNumberVerification(long id)
        {
            return RestartVerificationInternal(id, "CompleteRegistrationPartials/VerifyPhoneNumber",
                "CompleteRegistrationPartials/VerifyPhoneNumber", "CompleteRegistrationPartials/PhoneNumberOkay");
        }

        [HttpPost]
        public ActionResult CompleteRegistrationRestartEmailAddressVerification(long id)
        {
            return RestartVerificationInternal(id, "CompleteRegistrationPartials/VerifyEmailAddress",
                "CompleteRegistrationPartials/VerifyEmailAddress", "CompleteRegistrationPartials/EmailAddressOkay");
        }

        [HttpGet]
        public ActionResult Edit()
        {
            var currentUserInfo = AuthenticationService.LoadCurrentUserInfo();
            var model = new MyProfileEditModel
            {
                DisplayName = currentUserInfo.DisplayName.Trim(),
                FullName = currentUserInfo.FullName.Trim()
            };
            model.DisplayNameSameAsFullName = model.DisplayName.Equals(model.FullName);

            return View(model);
        }

        [HttpPost]
        [ActionName("Edit")]
        public ActionResult EditPostback(MyProfileEditModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result =
                AuthenticationService.UpdateUser(new User
                {
                    ID = User.CoreIdentity.UserId.Value,
                    FullName = model.FullName,
                    DisplayName = model.DisplayName
                });
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.FullResourceKey);

                return View();
            }

            PrincipalCache.InvalidateItem(User.CoreIdentity.UserId.Value);
            return RedirectToAction("View");
        }

        [HttpPost]
        public ActionResult AddEmailPopup()
        {
            var model = new MyProfileAddEmailModel {Visibility = VisibilityLevel.Everybody};
            return PartialView(model);
        }

        [HttpPost]
        [ValidateCaptcha(validateScriptCaptcha: true)]
        public ActionResult AddEmailPostback(MyProfileAddEmailModel model)
        {
            return AddEmailAddressInternal(model, "AddEmailPopup", "AddContactMethodComplete", false);
        }

        [HttpPost]
        public ActionResult AddPhoneNumberPopup()
        {
            var model = new MyProfileAddPhoneNumberModel {Visibility = VisibilityLevel.Everybody};
            return PartialView(model);
        }

        [HttpPost]
        [ValidateCaptcha(validateScriptCaptcha: true)]
        public ActionResult AddPhoneNumberPostback(MyProfileAddPhoneNumberModel model)
        {
            return AddPhoneNumberInternal(model, "AddPhoneNumberPopup", "AddContactMethodComplete", false);
        }

        [HttpPost]
        public ActionResult VerificationPopup(long id)
        {
            var verificationInfo = AuthenticationService.LoadContactMethodVerificationInfo(id);
            if (verificationInfo == null || verificationInfo.ContactMethod == null ||
                verificationInfo.ContactMethod.IsDeleted)
                return Error(ErrorResult.EntityNotFound);

            if (verificationInfo.ContactMethod.UserID != User.CoreIdentity.UserId ||
                !verificationInfo.ContactMethod.IsVerifiable)
                return Error(ErrorResult.AccessDenied);

            if (verificationInfo.ContactMethod.IsVerified)
                return PartialView("VerificationSuccessful", verificationInfo);

            var model = new MyProfileContactMethodVerificationModel
            {
                ContactMethodID = id,
                VerificationInfo = verificationInfo,
            };

            return PartialView(model);
        }

        [HttpPost]
        [ValidateCaptcha(validateScriptCaptcha: true)]
        public ActionResult PerformVerification(MyProfileContactMethodVerificationModel model)
        {
            return PerformVerificationInternal(model,
                "ContactMethodVerificationPartials/ContactPopupVerificationPartial", "VerificationSuccessful");
        }

        [HttpPost]
        public ActionResult RestartVerification(long id)
        {
            return RestartVerificationInternal(id, "ContactMethodVerificationPartials/ContactPopupVerificationPartial",
                "ContactMethodVerificationPartials/UnableToVerifyPartial", "VerificationSuccessful");
        }

        [HttpPost]
        public ActionResult DeleteContactMethod(long id)
        {
            var contactMethod = DbManager.Db.UserContactMethods.SingleOrDefault(cm => cm.ID == id);
            if (contactMethod == null || contactMethod.UserID != User.CoreIdentity.UserId.Value ||
                contactMethod.IsDeleted)
                return Error(ErrorResult.EntityNotFound);

            return PartialView(contactMethod);
        }

        [HttpPost]
        public ActionResult DeleteContactMethodConfirmed(long id)
        {
            var contactMethod = DbManager.Db.UserContactMethods.SingleOrDefault(cm => cm.ID == id);
            if (User.CoreIdentity.UserId != null && (contactMethod == null || contactMethod.UserID != User.CoreIdentity.UserId.Value ||
                                                     contactMethod.IsDeleted))
                return Error(ErrorResult.EntityNotFound);

            AuthenticationService.DeleteContactMethod(id);
            PrincipalCache.InvalidateItem(User.CoreIdentity.UserId.Value);

            return RedirectToAction("View");
        }

        [HttpGet]
        public ActionResult AboutVerification(string returnurl)
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadProfilePicture(long id, FineUpload file)
        {
//            var listing = DbManager.Db.Users.SingleOrDefault(l => l.ID == id);
//            if (listing == null)
//                return Json(new { success = false, error = "No such property listing" });
//
//            if (!PropertyControllerHelper.CanEditListing(listing))
//                return Json(new { success = false, error = "Access denied" });
//
//            var photo = PropertyPhotoService.AddPhoto(id, file.InputStream);
//            if (photo == null)
//                return Json(new { success = false, error = "Invalid file" });
//
//            return Json(new { success = true, PropertyListingPhotoID = photo.ID });
            return null;
        }

        #region Private helper methods

        private ActionResult AddPhoneNumberInternal(MyProfileAddPhoneNumberModel model, string errorViewName,
            string successViewName, bool shouldBeVerifyable)
        {
            if (!ModelState.IsValid)
                return PartialView(errorViewName, model);

            var addResult = AuthenticationService.AddContactMethod(
                User.CoreIdentity.UserId.Value,
                new UserContactMethod
                {
                    ContactMethodType = ContactMethodType.Phone,
                    ContactMethodText = model.PhoneNumber,
                    Visibility = model.Visibility
                },
                shouldBeVerifyable);

            if (!addResult.IsValid || addResult.Result == null)
            {
                foreach (var error in addResult.Errors)
                {
                    ModelState.AddModelError(string.Empty,
                        AccountValidationErrors.ResourceManager.GetString(error.ErrorKey));
                }

                return PartialView(errorViewName, model);
            }

            var contactMethod = addResult.Result;

            UserContactMethodVerificationInfo verificationInfo = contactMethod.IsVerifiable
                ? AuthenticationService.StartContactMethodVerification(contactMethod.ID)
                : AuthenticationService.LoadContactMethodVerificationInfo(contactMethod.ID);

            return PartialView(successViewName, new MyProfileContactMethodVerificationModel
            {
                VerificationInfo = verificationInfo,
                ContactMethodID = contactMethod.ID
            });
        }

        private ActionResult AddEmailAddressInternal(MyProfileAddEmailModel model, string errorViewName,
            string successViewName, bool shouldBeVerifyable)
        {
            if (!ModelState.IsValid)
                return PartialView(errorViewName, model);

            var addResult = AuthenticationService.AddContactMethod(
                User.CoreIdentity.UserId.Value,
                new UserContactMethod
                {
                    ContactMethodType = ContactMethodType.Email,
                    ContactMethodText = model.Email,
                    Visibility = model.Visibility
                },
                shouldBeVerifyable);

            if (!addResult.IsValid || addResult.Result == null)
            {
                foreach (var error in addResult.Errors)
                {
                    ModelState.AddModelError(string.Empty,
                        AccountValidationErrors.ResourceManager.GetString(error.ErrorKey));
                }

                return PartialView(errorViewName, model);
            }

            var contactMethod = addResult.Result;

            UserContactMethodVerificationInfo verificationInfo = contactMethod.IsVerifiable
                ? AuthenticationService.StartContactMethodVerification(contactMethod.ID)
                : AuthenticationService.LoadContactMethodVerificationInfo(contactMethod.ID);

            return PartialView(successViewName, new MyProfileContactMethodVerificationModel
            {
                VerificationInfo = verificationInfo,
                ContactMethodID = contactMethod.ID
            });
        }

        private ActionResult PerformVerificationInternal(MyProfileContactMethodVerificationModel model,
            string errorViewName, string successViewName)
        {
            var verificationInfo = AuthenticationService.LoadContactMethodVerificationInfo(model.ContactMethodID);
            if (verificationInfo == null || verificationInfo.ContactMethod == null ||
                verificationInfo.ContactMethod.IsDeleted)
                return Error(ErrorResult.EntityNotFound);

            if (verificationInfo.ContactMethod.UserID != User.CoreIdentity.UserId ||
                !verificationInfo.ContactMethod.IsVerifiable)
                return Error(ErrorResult.AccessDenied);

            model.VerificationInfo = verificationInfo;

            if (!ModelState.IsValid)
                return PartialView(errorViewName, model);

            if (verificationInfo.ContactMethod.IsVerified)
                return PartialView(successViewName, verificationInfo);

            var performVerificationResult = AuthenticationService.PerformContactMethodVerification(
                model.ContactMethodID, model.VerificationSecret);
            if (performVerificationResult.IsValid)
            {
                PrincipalCache.InvalidateItem(User.CoreIdentity.UserId.Value);
                return PartialView(successViewName, verificationInfo);
            }

            foreach (var error in performVerificationResult.Errors)
                ModelState.AddModelError("VerificationSecret",
                    AccountValidationErrors.ResourceManager.GetString(error.ErrorKey));
           
            return PartialView(errorViewName, model);
        }

        private ActionResult RestartVerificationInternal(long id, string restartedAndValidViewName,
            string restartedAndErrorViewName, string alreadyVerifiedViewName)
        {
            var verificationInfo = AuthenticationService.LoadContactMethodVerificationInfo(id);
            if (verificationInfo?.ContactMethod == null || verificationInfo.ContactMethod.IsDeleted)
                return Error(ErrorResult.EntityNotFound);

            if (verificationInfo.ContactMethod.UserID != User.CoreIdentity.UserId ||
                !verificationInfo.ContactMethod.IsVerifiable)
                return Error(ErrorResult.AccessDenied);

            if (verificationInfo.ContactMethod.IsVerified)
                return PartialView(alreadyVerifiedViewName, verificationInfo);

            AuthenticationService.CancelContactMethodVerification(id);
            verificationInfo = AuthenticationService.StartContactMethodVerification(id);

            var model = new MyProfileContactMethodVerificationModel
            {
                ContactMethodID = id,
                VerificationInfo = verificationInfo,
                VerificationRestarted = verificationInfo.ValidationForVerificationStart.IsValid
            };

            return verificationInfo.ValidationForVerificationStart.IsValid
                ? PartialView(restartedAndValidViewName, model)
                : PartialView(restartedAndErrorViewName, model);
        }

        public ActionResult VerifyEmailDirectLink(string verificationSecret, long contactMethodID,
            string contactMethodText)
        {
            if (verificationSecret != null)
            {
                var performVerificationResult = AuthenticationService.PerformContactMethodVerification(contactMethodID,
                    verificationSecret);
                if (performVerificationResult.IsValid)
                {
                    PrincipalCache.InvalidateItem(User.CoreIdentity.UserId.Value);
                    var userContactMethod = DbManager.Db.UserContactMethods.SingleOrDefault(c => c.ID == contactMethodID);
                    if (userContactMethod != null)
                        return RedirectToAction("CompleteRegistration",
                            new {verifiedEmail = userContactMethod.ContactMethodText, isSuccessful = true});
                    return RedirectToAction("CompleteRegistration");
                }
                return RedirectToAction("CompleteRegistration",
                    new {isSuccessful = false});
            }
            return null;
        }

        #endregion
    }
}