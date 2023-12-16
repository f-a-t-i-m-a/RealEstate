using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.Common.Util.Web.Attributes.MethodSelector;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Index;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Directory;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Application.Security;
using JahanJooy.RealEstate.Web.Models.Agency;
using JahanJooy.RealEstate.Web.Models.Shared;
using JahanJooy.RealEstate.Web.Models.Shared.Profile;

namespace JahanJooy.RealEstate.Web.Controllers
{
    public class AgencyController : CustomControllerBase
    {
        #region Injected dependencies

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IDirectoryService DirectoryService { get; set; }

        [ComponentPlug]
        public IAgencyIndex AgencyIndex { get; set; }

        [ComponentPlug]
        public IUserService UserService { get; set; }

		[ComponentPlug]
		public IPrincipalCache PrincipalCache { get; set; }

        #endregion

        #region Action methods

        [HttpGet]
        public ActionResult List(AgencyListModel model)
        {
            if (model == null)
                model = new AgencyListModel();

            int pageNum = 1;
            if (!string.IsNullOrWhiteSpace(model.Page))
                int.TryParse(model.Page, out pageNum);

            return View(model);
        }

        [HttpGet]
        public ActionResult ViewDetails(long id)
        {
            var agency = DbManager.Db.Agencies
                .Include(a => a.MemberUsers)
                .Include(a => a.AgencyBranches)
                .SingleOrDefault(a => a.ID == id);
            if (agency == null || agency.DeleteTime.HasValue)
                return Error(ErrorResult.EntityNotFound);

            return View(agency);
        }

        [HttpGet]
        public ActionResult NewAgency()
        {
            var model = new AgencyNewAgencyModel
            {
                Agency = new AgencyContent(),
                MainBranch = new AgencyBranchContent()
            };
            return View(model);
        }

        [HttpPost]
        [ActionName("NewAgency")]
        public ActionResult NewAgencyPostback(AgencyNewAgencyModel model)
        {
            if (model.GeographicLocationType.HasValue &&
                model.GeographicLocationType == GeographicLocationSpecificationType.UserSpecifiedExact)
            {
                if (model.UserPointLat.HasValue && model.UserPointLng.HasValue)
                {
                    var userpoint = new LatLng
                    {
                        Lat = model.UserPointLat.Value,
                        Lng = model.UserPointLng.Value
                    };
                    model.MainBranch.GeographicLocation = userpoint.ToDbGeography();
                    model.MainBranch.GeographicLocationType = model.GeographicLocationType;
                }
                else
                {
                    model.MainBranch.GeographicLocation = null;
                    model.MainBranch.GeographicLocationType = null;
                }
            }
            else
            {
                model.MainBranch.GeographicLocationType = null;
                model.MainBranch.GeographicLocation = null;
            }

            ValidateForSave(model);

            if (!ModelState.IsValid)
                return View(model);

            DirectoryService.SaveAgency(model.Agency, model.MainBranch, null, null);
            return RedirectToAction("AddNewAgencySuccess");
        }

        [HttpGet]
        public ActionResult AddNewAgencySuccess()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Search(long? vicinityID, int? pageNum)
        {
            var indexResult = AgencyIndex.Search(vicinityID, 1000, pageNum);

            var result = PagedList<Agency>.BuildUsingPagedList(indexResult);
            if (indexResult.Any())
            {
                result.PageItems = DbManager.Db.Agencies.Include(a => a.AgencyBranches)
                    .Where(a => indexResult.PageItems.Contains(a.ID))
                    .ToList();
            }

            return PartialView("AgencyListPartial", new AgencyListPartialModel {Agencies = result});
        }

        [HttpPost]
        public ActionResult AgencyDetails(long agencyId)
        {
            var agency = DbManager.Db.Agencies
                .Include(a => a.AgencyBranches)
                .Include(a => a.MemberUsers)
                .SingleOrDefault(a => a.ID == agencyId);
            if (agency == null || agency.DeleteTime.HasValue)
                return Error(ErrorResult.EntityNotFound);

            return PartialView("AgencyViewDetailsPartial", agency);
        }

        [HttpPost]
		[Authorize]
        public ActionResult MembershipConfirmationPopup(long agencyID)
        {
            return PartialView(agencyID);
        }

        [HttpPost]
		[Authorize]
		[SubmitButton("btnConfirm")]
        public ActionResult MembershipConfirmationPostBack(long agencyId)
        {
	        if (User.CoreIdentity.UserId != null)
	        {
				UserService.SetUserTypeAndAgencyMembership(User.CoreIdentity.UserId.Value, UserType.IndependentAgencyMember, agencyId);
				PrincipalCache.InvalidateItem(User.CoreIdentity.UserId.Value);
			}

            return RedirectToAction("View", "MyProfile",
                new {activeTab = ProfileModel.ProfileActiveTab.General.ToString()});
        }

        [HttpPost]
		[Authorize]
		public ActionResult UnsubscribeFromAgencyPopup()
        {
	        var user = DbManager.Db.Users.Include(u => u.Agency).SingleOrDefault(u => u.ID == User.CoreIdentity.UserId);
            if (user == null || user.Agency == null)
				return RedirectToAction("View", "MyProfile");

            var model = new AgencyUnSubscribeFromAgencyPopupModel
            {
                Agency = user.Agency
            };

            return PartialView("_Agency/UnsubscribeFromAgencyPopup", model);
        }

		[HttpPost]
		[Authorize]
		[SubmitButton("btnConfirm")]
		public ActionResult UnsubscribeFromAgencyPostback()
		{
			var user = DbManager.Db.Users.Include(u => u.Agency).SingleOrDefault(u => u.ID == User.CoreIdentity.UserId);
			if (user == null || user.Agency == null || user.Type != UserType.IndependentAgencyMember)
				return RedirectToAction("View", "MyProfile");

			UserService.SetUserTypeAndAgencyMembership(User.CoreIdentity.UserId.GetValueOrDefault(), UserType.IndependentAgent, null);
			PrincipalCache.InvalidateItem(User.CoreIdentity.UserId.Value);
			return RedirectToAction("View", "MyProfile");
		}

        #endregion

        #region private methods

        private void ValidateForSave(AgencyNewAgencyModel model)
        {
            ValidateForSave(model.Agency);
            ValidateForSave(model.MainBranch, "MainBranch.");
        }

        private void ValidateForSave(AgencyContent agencyContent)
        {
            if (string.IsNullOrWhiteSpace(agencyContent.Name))
            {
                ModelState.AddModelError("", AgencyControllerResources.ValidationError_AgencyName);
            }
        }

        private void ValidateForSave(AgencyBranchContent model, string fieldKeyPrefix)
        {
            if (string.IsNullOrWhiteSpace(model.BranchName))
            {
                ModelState.AddModelError(fieldKeyPrefix + "BranchName",
                    AgencyControllerResources.ValidationError_AgencyBranchName);
            }

            if (model.VicinityID == 0)
            {
                if (ModelState.Keys.Contains(fieldKeyPrefix + "VicinityID"))
                    ModelState.Remove(fieldKeyPrefix + "VicinityID");

                ModelState.AddModelError(fieldKeyPrefix + "VicinityID",
                    AgencyControllerResources.ValidationError_VicinityOfBranch);
            }
        }

        #endregion

    }
}