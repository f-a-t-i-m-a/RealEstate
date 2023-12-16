using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.Common.Util.Web.Attributes.MethodSelector;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Audit;

using JahanJooy.RealEstate.Domain.Enums;

using JahanJooy.RealEstate.Domain.Property;

using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Application.Security;
using JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminUsers;
using JahanJooy.RealEstate.Web.Models.Account;
using JahanJooy.RealEstate.Web.Models.Shared.Profile;
using JahanJooy.RealEstate.Web.Resources.Controllers.Account;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Controllers
{
	public class AdminUsersController : AdminControllerBase
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
        public IUserBillingBalanceCache BalanceCache { get; set; }

		#endregion

		[HttpGet]
		public ActionResult List(AdminUsersListModel model)
		{
            if (model == null)
                model = new AdminUsersListModel();

			int pageNum = 1;
			if (!string.IsNullOrWhiteSpace(model.Page))
				int.TryParse(model.Page, out pageNum);

			var db = DbManager.Db;

            var usersQuery = db.Users;

            usersQuery = ApplyFilterQuery(model, usersQuery);

			model.Users = PagedList<User>.BuildUsingPageNumber(usersQuery.Count(), 20, pageNum);
			model.Users.FillFrom(usersQuery.OrderByDescending(h => h.LastLogin ?? h.CreationDate));

			return View(model);
		}

		[HttpGet]
		public ActionResult Details(long id, string activeTab, string page)
		{
			var user = AuthenticationService.LoadUserInfo(id);
			if (user == null)
				return Error(ErrorResult.EntityNotFound);

			var activeTabEnum = ProfileModel.ProfileActiveTab.General;
			if (!string.IsNullOrWhiteSpace(activeTab))
				Enum.TryParse(activeTab, out activeTabEnum);

			int pageNum = 1;
			if (!string.IsNullOrWhiteSpace(page))
				int.TryParse(page, out pageNum);

			var model = new ProfileModel
				            {
					            ActiveTab = activeTabEnum,
					            User = user,
					            PaginationUrlTemplate = Url.Action("Details", new {activeTab = activeTabEnum.ToString()}) + "&page={0}",
					            EnableAdmin = true,
					            EnableEdit = false
				            };

			var db = DbManager.Db;

			switch (activeTabEnum)
			{
				case ProfileModel.ProfileActiveTab.General:
					break;

				case ProfileModel.ProfileActiveTab.AllPropertyListings:
				case ProfileModel.ProfileActiveTab.PublishedPropertyListings:
					model.PropertyListings = PropertyService.ListingsOfUser(id, false, activeTabEnum == ProfileModel.ProfileActiveTab.PublishedPropertyListings, pageNum, 20);
					break;

				case ProfileModel.ProfileActiveTab.PublishHistory:
					var historiesQuery = db.PropertyListingPublishHistories.Where(h => h.UserID == id);
					model.PropertyPublishes = PagedList<PropertyListingPublishHistory>.BuildUsingPageNumber(historiesQuery.Count(), 20, pageNum);
					model.PropertyPublishes.FillFrom(historiesQuery.OrderByDescending(h => h.ID));
					break;

				case ProfileModel.ProfileActiveTab.Searches:
					var searchesQuery = db.PropertySearchHistories.Where(h => h.UserID == id);
					model.Searches = PagedList<PropertySearchHistory>.BuildUsingPageNumber(searchesQuery.Count(), 20, pageNum);
					model.Searches.FillFrom(searchesQuery.OrderByDescending(h => h.ID));
					break;

				case ProfileModel.ProfileActiveTab.SecurityInfo:
					var sessionsQuery = db.HttpSessions.Where(l => l.UserID == id);
					model.Sessions = PagedList<HttpSession>.BuildUsingPageNumber(sessionsQuery.Count(), 20, pageNum);
					model.Sessions.FillFrom(sessionsQuery.OrderByDescending(s => s.ID));
					model.LatestPasswordResets = db.PasswordResetRequests.Where(l => l.TargetUserID == id).Take(3);
					model.LatestLoginNameQueries = db.LoginNameQueries.Where(l => l.TargetUserID == id).Take(3);
					break;

				default:
					return Error(ErrorResult.EntityNotFound);
			}
            model.Balance = BalanceCache[user.ID];
			return View(model);
		}

		[HttpPost]
		[ActionName("Details")]
		[SubmitButton("btnResetFailedLoginAttempts")]
		public ActionResult ResetFailedLoginAttempts(long id)
		{
			AuthenticationService.ResetFailedLoginCount(id);
			PrincipalCache.InvalidateItem(id);
			return RedirectToAction("Details", new {id});
		}

		[HttpPost]
		[ActionName("Details")]
		[SubmitButton("btnMarkAsDisabled")]
		public ActionResult MarkAsDisabled(long id)
		{
			AuthenticationService.MarkEnabled(id, false);
			PrincipalCache.InvalidateItem(id);
			return RedirectToAction("Details", new { id });
		}

		[HttpPost]
		[ActionName("Details")]
		[SubmitButton("btnMarkAsEnabled")]
		public ActionResult MarkAsEnabled(long id)
		{
			AuthenticationService.MarkEnabled(id, true);
			PrincipalCache.InvalidateItem(id);
			return RedirectToAction("Details", new { id });
		}

        [HttpPost]
        public ActionResult ChangePasswordAdministratively(long id)
        {
            var model = new AccountChangePasswordAdministrativelyModel {UserId = id};
            PrincipalCache.InvalidateItem(id);
            return PartialView("ChangePasswordAdministratively", model);
        }

        [HttpPost]
        public ActionResult ChangePasswordAdministrativelyPostback(AccountChangePasswordAdministrativelyModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("ChangePasswordAdministratively", model);   

            var changeResult = AuthenticationService.ResetPassword(model.UserId, model.NewPassword);

            if (changeResult.IsValid)
            {
                return PartialView("ChangePasswordAdministrativelySuccess", model.UserId);
            }
            
            foreach (var error in changeResult.Errors)
                ModelState.AddModelError("",
                    AccountValidationErrors.ResourceManager.GetString(error.FullResourceKey));

            return PartialView("ChangePasswordAdministratively", model);
        }

        private static IQueryable<User> ApplyFilterQuery(AdminUsersListModel model, IQueryable<User> userQueryable)
		{

            if (!string.IsNullOrWhiteSpace(model.ContactName))
            {
                userQueryable = userQueryable.Where(u => u.FullName.Contains(model.ContactName));
            }

            if (!string.IsNullOrWhiteSpace(model.ContactPhone))
            {
                userQueryable = userQueryable.Where(u => u.ContactMethods.Any(uc => uc.ContactMethodType == ContactMethodType.Phone && uc.ContactMethodText.Contains(model.ContactPhone)));
            }

            if (!string.IsNullOrWhiteSpace(model.ContactEmail))
            {
                userQueryable = userQueryable.Where(u => u.ContactMethods.Any(uc => uc.ContactMethodType == ContactMethodType.Email && uc.ContactMethodText.Contains(model.ContactEmail)));
            }

			return userQueryable;
		}

        [HttpPost]
        public ActionResult ContactMethodVerificationPopup(long id)
        {
            var contactMethod = DbManager.Db.UserContactMethods.Include(ucm => ucm.User).SingleOrDefault(ucm => ucm.ID == id);

            if (contactMethod == null)
                return Error(ErrorResult.EntityNotFound);
        
            return PartialView("ContactMethodVerificationPopup", contactMethod);
        }

        [HttpPost]
        [SubmitButton("btnOK")]
        public ActionResult ConfirmContactMethodVerification(long contactMethodId)
        {
            var contactMethod = DbManager.Db.UserContactMethods.SingleOrDefault(cm => cm.ID == contactMethodId);
            if (contactMethod == null)
                return Error(ErrorResult.EntityNotFound);

            AuthenticationService.PerformContactMethodVerificationAdministratively(contactMethodId);
        
            return RedirectToAction("Details", new { id = contactMethod.UserID });
        }

        [HttpPost]
        public ActionResult DeleteContactMethod(long contactMethodId)
        {
            var contactMethod =
                DbManager.Db.UserContactMethods.Include(cm => cm.User).SingleOrDefault(cm => cm.ID == contactMethodId);
            if (contactMethod == null || contactMethod.IsDeleted)
                return Error(ErrorResult.EntityNotFound);

            return PartialView("DeleteContactMethodPopup" ,contactMethod);
        }

        [HttpPost]
        [SubmitButton("btnOK")]
        public ActionResult ConfirmDeleteContactMethod(long contactMethodId)
        {
            var userId =
                DbManager.Db.UserContactMethods.Where(cm => cm.ID == contactMethodId)
                    .Select(cm => (long?) cm.UserID)
                    .SingleOrDefault();

            AuthenticationService.DeleteContactMethod(contactMethodId);

            if (userId == null)
                return Error(ErrorResult.EntityNotFound);

            return RedirectToAction("Details", new { id = userId });
        }
    }

}