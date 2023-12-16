using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.Collections;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.Common.Util.Web.Attributes.MethodSelector;
using JahanJooy.Common.Util.Web.Upload;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Domain.Directory;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Application.DomainModel;
using JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminAgency;
using JahanJooy.RealEstate.Web.Models.AdminAgency;
using JahanJooy.RealEstate.Web.Models.Agency;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Controllers
{
    public class AdminAgencyController : AdminControllerBase
    {
        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IDirectoryService DirectoryService { get; set; }

        #region public Agency methods

        [HttpGet]
        public ActionResult List(AdminAgencyListModel model)
        {
            if (model == null)
                model = new AdminAgencyListModel();

            int pageNum = 1;
            if (!string.IsNullOrWhiteSpace(model.Page))
                int.TryParse(model.Page, out pageNum);

            IQueryable<Agency> agencyQuery = DbManager.Db.AgenciesDbSet;

            agencyQuery = ApplyFilterQuery(model, agencyQuery);

            model.Agencies = PagedList<Agency>.BuildUsingPageNumber(agencyQuery.Count(), 20, pageNum);
            model.Agencies.FillFrom(agencyQuery.Include(a => a.AgencyBranches).OrderByDescending(a => a.ID));

            return View(model);
        }

        [HttpGet]
        public ActionResult ViewDetails(long id)
        {
            var model = DbManager.Db.AgenciesDbSet.Include(a => a.AgencyBranches).SingleOrDefault(a => a.ID == id);
            if (model == null)
                return Error(ErrorResult.EntityNotFound);

            return View(model);
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

            DirectoryService.SaveAgency(model.Agency, model.MainBranch, null, true);
            return RedirectToAction("List");
        }

        [HttpGet]
        public ActionResult EditAgency(long id)
        {
            var agency = DbManager.Db.Agencies.Include(a => a.AgencyBranches).SingleOrDefault(a => a.ID == id);
            if (agency == null)
                return Error(ErrorResult.EntityNotFound);

            var mainBranch = agency.AgencyBranches.FirstOrDefault(ab => ab.IsMainBranch && !ab.DeleteTime.HasValue);
            var model = new AgencyNewAgencyModel
            {
                AgencyID = agency.ID,
                MainBranchID = mainBranch.IfNotNull(b => b.ID),
                Agency = agency.GetContent(),
                MainBranch = mainBranch.GetContent()
            };

            if (model.MainBranch.GeographicLocation != null)
            {
                model.UserPointLat = model.MainBranch.GeographicLocation.Latitude;
                model.UserPointLng = model.MainBranch.GeographicLocation.Longitude;
            }
            if (model.MainBranch.GeographicLocationType != null)
            {
                model.GeographicLocationType = model.MainBranch.GeographicLocationType;
            }

            return View(model);
        }

        [HttpPost]
        [ActionName("EditAgency")]
        public ActionResult EditAgencyPostback(AgencyNewAgencyModel model)
        {
            if (!model.AgencyID.HasValue)
                return Error(ErrorResult.AccessDenied);

            if (model.GeographicLocationType.HasValue &&
                model.GeographicLocationType == GeographicLocationSpecificationType.UserSpecifiedExact)
            {
                if (model.UserPointLat != null && model.UserPointLng != null)
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
                return EditAgency(model.AgencyID.GetValueOrDefault());

            DirectoryService.UpdateAgency(model.AgencyID.Value, model.Agency, model.MainBranch);
            return RedirectToAction("ViewDetails", new {id = model.AgencyID});
        }

        [HttpGet]
        public ActionResult EditAgencyLogo(long id)
        {
            var agency = DbManager.Db.Agencies.SingleOrDefault(a => a.ID == id);
            if (agency == null)
                return Error(ErrorResult.EntityNotFound);

            var model = new AgencyEditAgencyLogo()
            {
                AgencyID = agency.ID,
                AgencyName=agency.GetContent().Name
                
            };
            if (agency.GetContent().LogoStoreItemID.HasValue)
                model.LogoStoreItemID = agency.GetContent().LogoStoreItemID.Value;

            return View(model);
        }

        [HttpPost]
        [ActionName("DeleteAgency")]
        public ActionResult DeleteAgency(long id)
        {
            var agency = DbManager.Db.AgenciesDbSet.SingleOrDefault(a => a.ID == id);
            if (agency == null)
                return Error(ErrorResult.EntityNotFound);

            return PartialView("ConfirmDeletingAgency", agency);
        }

        [HttpPost]
        [ActionName("ConfirmDeletingAgency")]
        public ActionResult ConfirmDeletingAgency(long id)
        {
            DirectoryService.DeleteAgency(id);
            return RedirectToAction("List");
        }

        [HttpPost]
        public ActionResult Upload(long id, FineUpload file)
        {
            var Agency = DbManager.Db.Agencies.SingleOrDefault(l => l.ID == id);
            if (Agency == null)
                return Json(new { success = false, error = "No such Agency" });

            var result = DirectoryService.SetLogo(id, file.InputStream);
            if (result == null)
                return Json(new { success = false, error = "Invalid file" });

            return Json(new { success = true, Agency = result.ID });
        }

        [HttpPost]
        public ActionResult UploadThumbnailLogo(long id, FineUpload file)
        {
            var agency = DbManager.Db.Agencies.SingleOrDefault(l => l.ID == id);
            if (agency == null)
                return Json(new { success = false, error = "No such Agency" });

            var result = DirectoryService.SetThumbnailLogo(id, file.InputStream);
            if (result == null)
                return Json(new { success = false, error = "Invalid file" });

            return Json(new { success = true, Agency = result.ID });
        }

        [HttpPost]
        public ActionResult ViewLogo(long id)
        {
            var agency = DbManager.Db.Agencies.SingleOrDefault(a => a.ID == id);

            if (agency == null )
                return Error(ErrorResult.AccessDenied);

            var model = new AgencyEditAgencyLogo()
            {
                AgencyID = agency.ID,
                AgencyName = agency.GetContent().Name,
                LogoStoreItemID = agency.GetContent().LogoStoreItemID
            };

            return PartialView("AgencyLogoPartial", model);
        }

        [HttpGet]
        public ActionResult GetAgencyLogo(long id)
        {
            // Use DbSet to allow admin to see deleted images
            var agency = DbManager.Db.AgenciesDbSet.SingleOrDefault(a => a.ID == id);

            if (agency == null|| agency.GetContent().LogoStoreItemID == null)
            {
                // Non-owner should not access the details of an unpublished or unapproved property.
                return Error(ErrorResult.AccessDenied);
            }

            return File(DirectoryService.GetAgencyLogoBytes(agency.GetContent().LogoStoreItemID.Value), "image/jpeg");
        }

        [HttpGet]
        public ActionResult GetThumbnailLogo(long id)
        {
            // Use DbSet to allow admin to see deleted images
            var agency = DbManager.Db.AgenciesDbSet.SingleOrDefault(a => a.ID == id);

            if (agency == null || agency.GetContent().LogoStoreItemID == null)
            {
                // Non-owner should not access the details of an unpublished or unapproved property.
                return Error(ErrorResult.AccessDenied);
            }

            return File(DirectoryService.GetThumbnailLogoBytes(agency.GetContent().LogoStoreItemID.Value), "image/jpeg");
        }

        #endregion

        #region public AgencyBranch methods

        [HttpGet]
        public ActionResult NewAgencyBranch(long? agencyId)
        {
            if (agencyId == null)
                return Error(ErrorResult.EntityNotFound);

            var model = new AdminAgencyBranchModel
            {
                AgencyID = agencyId,
                Branch = new AgencyBranchContent()
            };

            return View(model);
        }

        [HttpPost]
        [ActionName("NewAgencyBranch")]
        public ActionResult NewAgencyBranchPostback(AdminAgencyBranchModel model)
        {
            if (!model.AgencyID.HasValue)
                return Error(ErrorResult.EntityNotFound);

            if (!model.Branch.GeographicLocationType.HasValue ||
                model.Branch.GeographicLocationType != GeographicLocationSpecificationType.UserSpecifiedExact)
            {
                model.Branch.GeographicLocationType = null;
                model.Branch.GeographicLocation = null;
            }

            ValidateForSave(model.Branch, ".Branch");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            DirectoryService.SaveAgencyBranch(model.AgencyID.Value, model.Branch);
            return RedirectToAction("ViewDetails", new {ID = model.AgencyID});
        }

        [HttpGet]
        public ActionResult EditAgencyBranch(long id)
        {
            var agencyBranch = DbManager.Db.AgencyBranchesDbSet.Single(ab => ab.ID == id);
            if (agencyBranch == null)
                return Error(ErrorResult.EntityNotFound);

            var model = new AdminAgencyBranchModel
            {
                AgencyID = agencyBranch.AgencyID,
                BranchID = id,
                Branch = agencyBranch.GetContent()
            };

            return View(model);
        }

        [HttpPost]
        [ActionName("EditAgencyBranch")]
        public ActionResult EditAgencyBranchPostback(AdminAgencyBranchModel model)
        {
            if (!model.AgencyID.HasValue)
                return Error(ErrorResult.EntityNotFound);
            if (!model.BranchID.HasValue)
                return Error(ErrorResult.EntityNotFound);

            if (!model.Branch.GeographicLocationType.HasValue ||
                model.Branch.GeographicLocationType != GeographicLocationSpecificationType.UserSpecifiedExact)
            {
                model.Branch.GeographicLocationType = null;
                model.Branch.GeographicLocation = null;
            }

            ValidateForSave(model.Branch, "Branch.");

            if (!ModelState.IsValid)
                return EditAgencyBranch(model.BranchID.GetValueOrDefault());

            DirectoryService.UpdateAgencyBranch(model.BranchID.Value, model.Branch);

            return RedirectToAction("ViewDetails", new {ID = model.AgencyID});
        }

        [HttpPost]
        [ActionName("DeleteAgencyBranch")]
        public ActionResult DeleteAgencyBranch(long id)
        {
            var agencyBranch = DbManager.Db.AgencyBranchesDbSet.SingleOrDefault(ab => ab.ID == id);
            if (agencyBranch == null)
                return Error(ErrorResult.EntityNotFound);

            return PartialView("ConfirmDeletingAgencyBranch", agencyBranch);
        }

        [HttpPost]
        [ActionName("ConfirmDeletingAgencyBranch")]
        public ActionResult ConfirmDeletingAgencyBranch(long branchId, long agencyId)
        {
            DirectoryService.DeleteAgencyBranch(branchId);
            return RedirectToAction("ViewDetails", new {id = agencyId});
        }

        [HttpPost]
        public ActionResult Review(long id)
        {
            var agency = DbManager.Db.Agencies
                .SingleOrDefault(a => a.ID == id);


            if (agency == null)
                return PartialView("_Errors/EntityNotFound");


            return PartialView(new AdminAgencyReviewActionModel
            {
                Agency = agency
            });
        }

        [HttpPost]
        [ActionName("ReviewPostBack")]
        [SubmitButton("btnApprove")]
        public ActionResult Approve(long id)
        {
            DirectoryService.SetApproved(id, true);
            return RedirectToAction("List", "AdminAgency");
        }

        [HttpPost]
        [ActionName("ReviewPostBack")]
        [SubmitButton("btnReject")]
        public ActionResult Reject(long id)
        {
            DirectoryService.SetApproved(id, false);
            return RedirectToAction("List", "AdminAgency");
        }

        #endregion

        #region Private methods

        private void ValidateForSave(AgencyNewAgencyModel model)
        {
            ValidateForSave(model.Agency);
            ValidateForSave(model.MainBranch, "MainBranch.");
        }


        private void ValidateForSave(AgencyContent agencyContent)
        {
            if (string.IsNullOrWhiteSpace(agencyContent.Name))
            {
                ModelState.AddModelError("", AdminAgencyControllerResources.ValidationError_AgencyName);
            }
        }

        private void ValidateForSave(AgencyBranchContent model, string fieldKeyPrefix)
        {
            if (string.IsNullOrWhiteSpace(model.BranchName))
            {
                ModelState.AddModelError(fieldKeyPrefix + "BranchName",
                    AdminAgencyControllerResources.ValidationError_AgencyBranchName);
            }

            if (model.VicinityID == 0)
            {
                if (ModelState.Keys.Contains(fieldKeyPrefix + "VicinityID"))
                    ModelState.Remove(fieldKeyPrefix + "VicinityID");

                ModelState.AddModelError(fieldKeyPrefix + "VicinityID",
                    AdminAgencyControllerResources.ValidationError_VicinityOfBranch);
            }
        }

        private static IQueryable<Agency> ApplyFilterQuery(AdminAgencyListModel model,
            IQueryable<Agency> agencyQueryable)
        {
            agencyQueryable = (model.IsIncludeDeletedAgencies
                ? agencyQueryable
                : agencyQueryable.Where(a => !a.DeleteTime.HasValue));

            return agencyQueryable;
        }

        #endregion
    }
}