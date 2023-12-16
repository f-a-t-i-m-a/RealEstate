using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.Common.Util.Web.Captcha;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Core.DomainExtensions;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Search;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Core.Services.Dto;
using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Domain.Property;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Application.Security;
using JahanJooy.RealEstate.Web.Areas;
using JahanJooy.RealEstate.Web.Helpers.Property;
using JahanJooy.RealEstate.Web.Models.Property;
using JahanJooy.RealEstate.Web.Models.Shared.Profile;
using JahanJooy.RealEstate.Web.Resources.Controllers.Property;

namespace JahanJooy.RealEstate.Web.Controllers
{
    public class PropertyController : CustomControllerBase
    {
        #region Injected dependencies

        [ComponentPlug]
        public IAuthenticationService AuthenticationService { get; set; }

        [ComponentPlug]
        public IPropertyService PropertyService { get; set; }

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IVicinityCache VicinityCache { get; set; }

        [ComponentPlug]
        public IEmailNotificationService EmailNotificationService { get; set; }

        #endregion

        #region Action methods

        [HttpGet]
        public ActionResult QuickCreate()
        {
            var model = new PropertySummaryModel();
            model.PublishDuration = PropertyPublishDurationEnum.ThreeMonths;
//            model.UserPoint = new LatLng { Lat = 35.9, Lng = 51.5 };

            PopulateContactInfoSelectItems(model);
            SetContactInfoDefaultValues(model);

            return View("QuickCreate", model);
        }

        [HttpPost]
        [ActionName("QuickCreate")]
        [ValidateCaptcha(validateScriptCaptcha: true)]
        public ActionResult QuickCreatePostback()
        {
            var model = new PropertySummaryModel();
            if (!TryUpdateModel(model))
            {
                PopulateContactInfoSelectItems(model);
                return View("QuickCreate", model);
            }

            var listing = model.ToDomain();

            var validationResult = model.PublishDuration.HasValue
                ? PropertyService.ValidateForPublish(PropertyListingDetails.MakeDetails(listing))
                : PropertyService.ValidateForSave(listing);

            if (!validationResult.IsValid)
            {
                model.AddDomainValidationErrors(validationResult, ModelState);
                PopulateContactInfoSelectItems(model);
                return View("QuickCreate", model);
            }

            PropertyService.Save(listing);
            if (model.PublishDuration.HasValue)
                PropertyService.Publish(listing.ID, (int) model.PublishDuration);

            if (!User.Identity.IsAuthenticated)
            {
                if (SessionInfo.OwnedProperties == null)
                    SessionInfo.OwnedProperties = new HashSet<long>();

                SessionInfo.OwnedProperties.Add(listing.ID);
                OwnedPropertiesCookieUtil.Set(SessionInfo.OwnedProperties);
            }

            return RedirectToAction("QuickCreateCompleted", new {id = listing.ID});
        }

        [HttpGet]
        public ActionResult QuickCreateCompleted(long id)
        {
            var listing = PropertyService.LoadWithAllDetails(id);
            // Loading all details because we need to validate for publish

            if (!PropertyControllerHelper.DetermineIfUserIsOwner(listing) && !User.IsOperator)
                return RedirectToAction("ViewDetails", new {id});

            var listingEditPassword =
                DbManager.Db.PropertyListings.Where(l => l.ID == id).Select(l => l.EditPassword).SingleOrDefault();
            var listingSummary = PropertyListingSummary.Summarize(listing);
            var validationResult = PropertyService.ValidateForPublish(listing);

            var model = new PropertyQuickCreateCompletedModel
            {
                Listing = listing,
                ListingEditPassword = listingEditPassword,
                ListingSummary = listingSummary,
                PublishValidationResult = validationResult
            };
            return View(model);
        }

        public ActionResult ViewDetails(PropertyViewDetailsParamsModel p)
        {
            if (!p.ID.HasValue)
                return ViewDetailsNotFound();

            var listing = PropertyService.LoadForVisit(p.ID.Value);
            if (listing == null)
                return ViewDetailsNotFound();

            var isOwner = PropertyControllerHelper.DetermineIfUserIsOwner(listing);

            SponsoredPropertyListing sponsoredPropertyListing = null;

            if (isOwner)
            {
                sponsoredPropertyListing = DbManager.Db.SponsoredPropertyListings
                    .Include(s => s.SponsoredEntity)
                    .Where(s => !s.SponsoredEntity.DeleteTime.HasValue)
                    .FirstOrDefault(s => s.ListingID == listing.ID);
            }

            var model = new PropertyViewDetailsModel
            {
                Params = p,
                Listing = listing,
                ListingSummary = PropertyListingSummary.Summarize(listing),
                IsOwner = PropertyControllerHelper.DetermineIfUserIsOwner(listing),
                SponsoredPropertyListing = sponsoredPropertyListing
            };

            FillNextPrevReturnUrls(model);

            if (!PropertyControllerHelper.CanViewListing(listing))
            {
                // Non-owner should not access the details of an unpublished property.
                return ViewDetailsNotAllowed(model);
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult ViewDetailsByCode(long? code)
        {
            if (!code.HasValue)
                return ViewDetailsNotFound();

            var listing = DbManager.Db.PropertyListings.SingleOrDefault(l => l.Code == code.Value);
            if (listing == null)
                return ViewDetailsNotFound();

            return RedirectToAction("ViewDetails", new {id = listing.ID});
        }

        [HttpPost]
        [Authorize]
        public ActionResult ToggleFavorite(long propertyId)
        {
            var result = PropertyService.ToggleFavorite(propertyId);
            return Json(result);
        }

        [HttpPost]
        public ActionResult EnableEdit(long? id)
        {
            if (!id.HasValue)
                return ViewDetailsNotFound();

            var listing = PropertyService.Load(id.Value);
            if (listing == null)
                return ViewDetailsNotFound();

            if (PropertyControllerHelper.DetermineIfUserIsOwner(listing))
                return PartialView("EnableEditSuccessful", id.Value);

            return PartialView("EnableEdit", listing);
        }

        [HttpPost]
        [ValidateCaptcha(validateScriptCaptcha: true)]
        public ActionResult EnableEditPostback(long propertyListingId, string editPassword)
        {
            if (!ModelState.IsValid)
                return EnableEdit(propertyListingId);

            var passwordIsCorrect = PropertyService.ValidateEditPassword(propertyListingId, editPassword.Trim());

            if (passwordIsCorrect)
            {
                if (SessionInfo.OwnedProperties == null)
                    SessionInfo.OwnedProperties = new HashSet<long>();

                SessionInfo.OwnedProperties.Add(propertyListingId);
                OwnedPropertiesCookieUtil.Set(SessionInfo.OwnedProperties);

                return PartialView("EnableEditSuccessful", propertyListingId);
            }

            ModelState.AddModelError("", PropertyControllerResources.EnableEdit_PasswordError);
            return EnableEdit(propertyListingId);
        }

        [HttpPost]
        public ActionResult ContactInfoTab(long? id)
        {
            if (!id.HasValue)
                return Error(ErrorResult.EntityNotFound);

            var listing = PropertyService.LoadForContactInfo(id.Value);
            if (!PropertyControllerHelper.CanViewListing(listing))
                return PartialView("ViewDetailsNotAllowedPartial");

            return PartialView("_Property/Tabs/ContactInfo",
                new ViewPropertyDetailsContactInfoTabModel
                {
                    IsOwner = PropertyControllerHelper.DetermineIfUserIsOwner(listing),
                    Listing = listing
                });
        }

        [HttpPost]
        public ActionResult ContactInfoPopup(long? id)
        {
            if (!id.HasValue)
                return Error(ErrorResult.EntityNotFound);

            var listing = PropertyService.LoadForContactInfo(id.Value);
            if (!PropertyControllerHelper.CanViewListing(listing))
                return PartialView("ViewDetailsNotAllowedPartial");

            return PartialView("_Property/View/ViewPropertyVisitAndContact", PropertyContactModel.CreateFromDto(listing));
        }

        [HttpGet]
        public ActionResult EditBySummary(long id)
        {
            var propertyListing =
                PropertyListingDetails.MakeDetails(DbManager.Db.PropertyListings.Where(l => l.ID == id),
                    includeBuilding: true, includeEstate: true, includeUnit: true, includeSaleAndRentConditions: true,
                    includeContactInfo: true).SingleOrDefault();

            if (propertyListing == null)
                return Error(ErrorResult.EntityNotFound);

            if (!PropertyControllerHelper.CanEditListing(propertyListing))
                return RedirectToAction("ViewDetails", new {id});

            var model = PropertySummaryModel.FromDomain(propertyListing);
            if (propertyListing.VicinityID.HasValue)
                model.Vicinity = VicinityCache[propertyListing.VicinityID.Value];

            PopulateContactInfoSelectItems(model);
            return View(model);
        }

        [HttpPost]
        [ActionName("EditBySummary")]
        public ActionResult EditBySummaryPostback(long id)
        {
            var propertyListing =
                PropertyListingDetails.MakeDetails(DbManager.Db.PropertyListings.Where(l => l.ID == id),
                    includeBuilding: true, includeEstate: true, includeUnit: true, includeSaleAndRentConditions: true,
                    includeContactInfo: true).SingleOrDefault();
            if (!PropertyControllerHelper.CanEditListing(propertyListing))
                return RedirectToAction("ViewDetails", new {id});

            var model = PropertySummaryModel.FromDomain(propertyListing);
            bool modelUpdateSuccessful = TryUpdateModel(model);
            if (!modelUpdateSuccessful)
            {
                PopulateContactInfoSelectItems(model);
                return View(model);
            }

            var result = PropertyService.Update(id, delegate(PropertyListing listing)
            {
                if (PropertyControllerHelper.CanEditListing(listing)) model.UpdateDomain(listing);
                return EntityUpdateResult.Default;
            });

            if (result.IsValid)
                return ViewDetailsActionAfterEdit(id);

            model.PublishValidationResult = result;
            model.AddDomainValidationErrors(result, ModelState);
            PopulateContactInfoSelectItems(model);
            return View(model);
        }

        [HttpGet]
        public ActionResult EditUnit(long id)
        {
            var propertyListing =
                PropertyListingDetails.MakeDetails(DbManager.Db.PropertyListings.Where(l => l.ID == id),
                    includeUnit: true).SingleOrDefault();
            if (!PropertyControllerHelper.CanEditListing(propertyListing))
                return RedirectToAction("ViewDetails", new {id});

            return View(PropertyUnitModel.CreateFromDto(propertyListing));
        }

        [HttpPost]
        [ActionName("EditUnit")]
        public ActionResult EditUnitPostback(long id)
        {
            var model = new PropertyUnitModel(id);
            bool modelUpdateSuccessful = TryUpdateModel(model);
            if (!modelUpdateSuccessful)
                return EditUnit(id);

            var result = PropertyService.Update(id, delegate(PropertyListing listing)
            {
                if (PropertyControllerHelper.CanEditListing(listing)) model.UpdateDomain(listing);
                return EntityUpdateResult.Default;
            });

            if (result.IsValid)
                return RedirectToAction("ViewDetails", new {id});

            model.PublishValidationResult = result;
            return View(model);
        }

        [HttpGet]
        public ActionResult EditBuilding(long id)
        {
            var propertyListing =
                PropertyListingDetails.MakeDetails(DbManager.Db.PropertyListings.Where(l => l.ID == id),
                    includeBuilding: true).SingleOrDefault();
            if (!PropertyControllerHelper.CanEditListing(propertyListing))
                return RedirectToAction("ViewDetails", new {id});

            return View(PropertyBuildingModel.CreateFromDto(propertyListing));
        }

        [HttpPost]
        [ActionName("EditBuilding")]
        public ActionResult EditBuildingPostback(long id)
        {
            var model = new PropertyBuildingModel(id);
            bool modelUpdateSuccessful = TryUpdateModel(model);
            if (!modelUpdateSuccessful)
                return EditBuilding(id);

            var result = PropertyService.Update(id, delegate(PropertyListing listing)
            {
                if (PropertyControllerHelper.CanEditListing(listing)) model.UpdateDomain(listing);
                return EntityUpdateResult.Default;
            });

            if (result.IsValid)
                return RedirectToAction("ViewDetails", new {id});

            model.PublishValidationResult = result;
            return View(model);
        }

        [HttpGet]
        public ActionResult EditEstate(long id)
        {
            var propertyListing =
                PropertyListingDetails.MakeDetails(DbManager.Db.PropertyListings.Where(l => l.ID == id),
                    includeEstate: true).SingleOrDefault();
            if (!PropertyControllerHelper.CanEditListing(propertyListing))
                return RedirectToAction("ViewDetails", new {id});

            return View(PropertyEstateModel.CreateFromDto(propertyListing));
        }

        [HttpPost]
        [ActionName("EditEstate")]
        public ActionResult EditEstatePostback(long id)
        {
            var model = new PropertyEstateModel(id);
            bool modelUpdateSuccessful = TryUpdateModel(model);
            if (!modelUpdateSuccessful)
                return EditEstate(id);

            var result = PropertyService.Update(id, delegate(PropertyListing listing)
            {
                if (PropertyControllerHelper.CanEditListing(listing)) model.UpdateDomain(listing);
                return EntityUpdateResult.Default;
            });

            if (result.IsValid)
                return RedirectToAction("ViewDetails", new {id});

            model.PublishValidationResult = result;
            return View(model);
        }

        [HttpGet]
        public ActionResult EditLuxury(long id)
        {
            var propertyListing =
                PropertyListingDetails.MakeDetails(DbManager.Db.PropertyListings.Where(l => l.ID == id),
                    includeUnit: true).SingleOrDefault();
            if (!PropertyControllerHelper.CanEditListing(propertyListing))
                return RedirectToAction("ViewDetails", new {id});

            return View(PropertyLuxuryModel.CreateFromDto(propertyListing));
        }

        [HttpPost]
        [ActionName("EditLuxury")]
        public ActionResult EditLuxuryPostback(long id)
        {
            var model = new PropertyLuxuryModel(id);
            bool modelUpdateSuccessful = TryUpdateModel(model);
            if (!modelUpdateSuccessful)
                return EditLuxury(id);

            var result = PropertyService.Update(id, delegate(PropertyListing listing)
            {
                if (PropertyControllerHelper.CanEditListing(listing)) model.UpdateDomain(listing);
                return EntityUpdateResult.Default;
            });

            if (result.IsValid)
                return RedirectToAction("ViewDetails", new {id});

            model.PublishValidationResult = result;
            return View(model);
        }

        [HttpGet]
        public ActionResult EditSalePaymentConditions(long id)
        {
            var propertyListing =
                PropertyListingDetails.MakeDetails(DbManager.Db.PropertyListings.Where(l => l.ID == id),
                    includeEstate: true, includeUnit: true, includeSaleAndRentConditions: true).SingleOrDefault();
            if (!PropertyControllerHelper.CanEditListing(propertyListing))
                return RedirectToAction("ViewDetails", new {id});

            return View(PropertySalePaymentModel.CreateFromDto(propertyListing));
        }

        [HttpPost]
        [ActionName("EditSalePaymentConditions")]
        public ActionResult EditSalePaymentConditionsPostback(long id)
        {
            var model = new PropertySalePaymentModel(id);
            bool modelUpdateSuccessful = TryUpdateModel(model);
            if (!modelUpdateSuccessful)
                return EditSalePaymentConditions(id);

            var result = PropertyService.Update(id, delegate(PropertyListing listing)
            {
                if (PropertyControllerHelper.CanEditListing(listing)) model.UpdateDomain(listing);
                return EntityUpdateResult.Default;
            });

            if (result.IsValid)
                return RedirectToAction("ViewDetails", new {id});

            model.PublishValidationResult = result;
            return View(model);
        }

        [HttpGet]
        public ActionResult EditRentPaymentConditions(long id)
        {
            var propertyListing =
                PropertyListingDetails.MakeDetails(DbManager.Db.PropertyListings.Where(l => l.ID == id),
                    includeSaleAndRentConditions: true).SingleOrDefault();
            if (!PropertyControllerHelper.CanEditListing(propertyListing))
                return RedirectToAction("ViewDetails", new {id});

            return View(PropertyRentPaymentModel.CreateFromDto(propertyListing));
        }

        [HttpPost]
        [ActionName("EditRentPaymentConditions")]
        public ActionResult EditRentPaymentConditionsPostback(long id)
        {
            var model = new PropertyRentPaymentModel(id);
            bool modelUpdateSuccessful = TryUpdateModel(model);
            if (!modelUpdateSuccessful)
                return EditRentPaymentConditions(id);

            var result = PropertyService.Update(id, delegate(PropertyListing listing)
            {
                if (PropertyControllerHelper.CanEditListing(listing)) model.UpdateDomain(listing);
                return EntityUpdateResult.Default;
            });

            if (result.IsValid)
                return RedirectToAction("ViewDetails", new {id});

            model.PublishValidationResult = result;
            return View(model);
        }

        [HttpGet]
        public ActionResult EditVisitAndContact(long id)
        {
            var propertyListing =
                PropertyListingDetails.MakeDetails(DbManager.Db.PropertyListings.Where(l => l.ID == id),
                    includeContactInfo: true).SingleOrDefault();
            if (!PropertyControllerHelper.CanEditListing(propertyListing))
                return RedirectToAction("ViewDetails", new {id});

            return View(PropertyContactModel.CreateFromDto(propertyListing));
        }

        [HttpPost]
        [ActionName("EditVisitAndContact")]
        public ActionResult EditVisitAndContactPostback(long id)
        {
            var model = new PropertyContactModel(id);
            bool modelUpdateSuccessful = TryUpdateModel(model);
            if (!modelUpdateSuccessful)
                return EditVisitAndContact(id);

            var result = PropertyService.Update(id, delegate(PropertyListing listing)
            {
                if (PropertyControllerHelper.CanEditListing(listing)) model.UpdateDomain(listing);
                return EntityUpdateResult.Default;
            });

            if (result.IsValid)
                return RedirectToAction("ViewDetails", new {id});

            model.PublishValidationResult = result;
            return View(model);
        }

        [HttpPost]
        public ActionResult Publish(long id)
        {
            // Load all details because we need to validate everything for publishing
            var listing = PropertyService.LoadWithAllDetails(id);

            if (!PropertyControllerHelper.CanPublishListing(listing))
                return PartialView("_AccessDeniedPopupPartial");

            var validationResult = PropertyService.ValidateForPublish(listing);
            PropertyListingSummary listingSummary = PropertyListingSummary.Summarize(listing);

            var model = new PropertyPublishModel {Listing = listingSummary, ValidationResult = validationResult};
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult PublishPostback(long id, int publishDays)
        {
            var propertyListing = DbManager.Db.PropertyListings.SingleOrDefault(l => l.ID == id);
            if (!PropertyControllerHelper.CanPublishListing(propertyListing))
                return RedirectToAction("ViewDetails", new {id});

            if (publishDays < 1 || publishDays > 150)
                return RedirectToAction("ViewDetails", new {id});

            PropertyService.Publish(id, publishDays);

            // TODO: Show success / failure message
            return RedirectToAction("ViewDetails", new {id});
        }

        [HttpPost]
        public ActionResult RePublish(long id)
        {
            var listing = PropertyService.LoadSummary(id);
            if (!PropertyControllerHelper.CanPublishListing(listing))
                return PartialView("_AccessDeniedPopupPartial");

            if (listing.PublishEndDate < DateTime.Now)
                return Content("The listing is already expired.");

            return PartialView(listing);
        }

        [HttpPost]
        public ActionResult UnPublish(long id)
        {
            var listing = PropertyService.LoadSummary(id);
            if (!PropertyControllerHelper.CanPublishListing(listing))
                return PartialView("_AccessDeniedPopupPartial");

            if (listing.PublishEndDate < DateTime.Now)
                return Content("The listing is already expired.");

            return PartialView(listing);
        }

        [HttpPost]
        public ActionResult UnPublishPostback(long id)
        {
            var propertyListing = DbManager.Db.PropertyListings.SingleOrDefault(l => l.ID == id);
            if (!PropertyControllerHelper.CanPublishListing(propertyListing))
                return RedirectToAction("ViewDetails", new {id});

            PropertyService.UnPublish(id);

            // TODO: Show success / failure message
            return RedirectToAction("ViewDetails", new {id});
        }

        [HttpPost]
        public ActionResult Delete(long id)
        {
            var model = PropertyService.LoadSummary(id);
            if (!PropertyControllerHelper.CanEditListing(model))
                return PartialView("_AccessDeniedPopupPartial");

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult DeletePostback(long id)
        {
            var propertyListing = DbManager.Db.PropertyListings.SingleOrDefault(l => l.ID == id);
            if (!PropertyControllerHelper.CanEditListing(propertyListing))
                return RedirectToAction("ViewDetails", new {id});

            PropertyService.Delete(id);
            return RedirectToAction("View", "MyProfile",
                new {activeTab = ProfileModel.ProfileActiveTab.AllPropertyListings.ToString()});
        }

        [HttpPost]
        public ActionResult Map(long? id)
        {
            if (!id.HasValue)
                return Error(ErrorResult.EntityNotFound);

            var listingSummary = PropertyService.LoadSummary(id.Value);
            if (!PropertyControllerHelper.CanViewListing(listingSummary))
                return PartialView("ViewDetailsNotAllowedPartial");

            var vicinity = (listingSummary.VicinityID.HasValue ? VicinityCache[listingSummary.VicinityID.Value] : null);
            LatLng center = null;
            LatLng markerLocation = null;
            string url = "";
            LatLngBounds bounds;
            int zoomLevel = 0;

            if (vicinity != null && vicinity.Boundary != null)
            {
                center = vicinity.Boundary.FindBoundingBox().GetCenter();
                bounds = vicinity.Boundary.FindBoundingBox();
                zoomLevel = GoogleMapsUtil.GetBoundsZoomLevel(bounds, 640, 500);
            }
            else if (vicinity != null && vicinity.Parent != null && vicinity.Parent.Boundary != null)
            {
                center = vicinity.Parent.Boundary.FindBoundingBox().GetCenter();
                bounds = vicinity.Parent.Boundary.FindBoundingBox();
                zoomLevel = GoogleMapsUtil.GetBoundsZoomLevel(bounds, 640, 500) + 1;
            }
            else if (vicinity != null && vicinity.CenterPoint != null)
            {
                center = vicinity.CenterPoint.ToLatLng();
                zoomLevel = 12;
            }
            else if (vicinity != null && vicinity.Parent != null && vicinity.Parent.CenterPoint != null)
            {
                center = vicinity.Parent.CenterPoint.ToLatLng();
                zoomLevel = 12;
            }
            else if (listingSummary.GeographicLocation != null)
            {
                center = listingSummary.GeographicLocation.ToLatLng();
                zoomLevel = 12;
            }

            if (listingSummary.GeographicLocationType == GeographicLocationSpecificationType.UserSpecifiedExact &&
                listingSummary.GeographicLocation != null)
            {
                markerLocation = listingSummary.GeographicLocation.ToLatLng();
            }

            if (center != null)
            {
                url = "http://maps.googleapis.com/maps/api/staticmap?center=" + center.Lat + "," + center.Lng +
                      "&zoom=" + zoomLevel + "&size=640x500&key=AIzaSyDlE2V9O9FzosTGXcX6SPQatAukHsQwP6U";

                if (markerLocation != null)
                {
                    url = url + "&markers=color:red" + "%7C" + markerLocation.Lat + "," + markerLocation.Lng;
                }
            }

            return PartialView("_Property/Tabs/Map",
                new ViewPropertyDetailsMapTabModel
                {
                    Url = url,
                    Vicinity = vicinity,
                    ListingSummary = listingSummary
                });
        }

        [HttpPost]
        public ActionResult SharePropertyListing(long propertyListingID)
        {
            var model = new PropertySharePropertyListingModel
            {
                PropertyListingID = propertyListingID
            };
            return PartialView("SharePropertyListingPartial", model);
        }

        [HttpPost]
        [ValidateCaptcha(validateScriptCaptcha: true)]
        public ActionResult SharePropertyListingPostback(PropertySharePropertyListingModel model)
        {

            if (!User.IsVerified)
                ValidateCaptchaAttribute.AuthorizeImageCaptcha(HttpContext, ModelState);

            if (!ModelState.IsValid)
                return PartialView("SharePropertyListingPartial", model);

            var listing =
                DbManager.Db.PropertyListings.
                    Include(pl => pl.Estate)
                    .Include(pl => pl.Building)
                    .Include(pl => pl.Unit)
                    .Include(pl => pl.SaleConditions)
                    .Include(pl => pl.RentConditions)
                    .SingleOrDefault(pl => pl.ID == model.PropertyListingID
                              && pl.DeleteDate == null
                              && pl.Approved == true)
                ;

            if (listing != null && listing.IsPublished())
            {
                var listingDetails = PropertyListingDetails.MakeDetails(listing);
                var listingSummary = PropertyListingSummary.Summarize(listingDetails);

                EmailNotificationService.SharePropertyListing(model.Email, model.ReceiverName, model.Description,
                    listingDetails, listingSummary);
                return PartialView("SharePropertySuccessfullyPartial");
            }
            return ViewDetailsNotFound();
        }

        #endregion

        #region Private helper methods

        private void PopulateContactInfoSelectItems(PropertySummaryModel model)
        {
            // If the user is authenticated (and not an operator), populate the contact infos dropdown selections.
            // For the operators, since they shouldn't have any associated property listings, we won't be showing the dropdown.

            if (User.Identity.IsAuthenticated && !User.IsOperator)
            {
                model.ExistingContactInfos = DbManager.Db.PropertyListings
                    .Where(
                        pl =>
                            pl.OwnerUserID == User.CoreIdentity.UserId.Value && !pl.DeleteDate.HasValue &&
                            pl.ContactInfo != null)
                    .Select(pl => pl.ContactInfo)
                    .Distinct()
                    .ToList();
            }
        }

        private void SetContactInfoDefaultValues(PropertySummaryModel model)
        {
            if (!User.Identity.IsAuthenticated)
                return;

            // For the operator, we do not set any default since it needs to be filled for every entry.
            if (User.IsOperator)
                return;

            User currentUserInfo = AuthenticationService.LoadCurrentUserInfo();
            if (currentUserInfo == null)
                return;

            // Contact name
            model.ContactName = currentUserInfo.FullName;

            // Phone numbers
            List<UserContactMethod> phoneMatchedList = currentUserInfo.ContactMethods
                .Where(cm => !cm.IsDeleted &&
                             cm.ContactMethodType == ContactMethodType.Phone &&
                             !string.IsNullOrWhiteSpace(cm.ContactMethodText))
                .OrderByDescending(cm => cm.IsVerified)
                .ToList();

            model.ContactPhone1 = phoneMatchedList.ElementAtOrDefault(0)
                .IfNotNull(cm => cm.ContactMethodText, string.Empty);
            model.ContactPhone2 = phoneMatchedList.ElementAtOrDefault(1)
                .IfNotNull(cm => cm.ContactMethodText, string.Empty);

            // Email address
            model.ContactEmail = currentUserInfo.ContactMethods
                .Where(cm => !cm.IsDeleted &&
                             cm.ContactMethodType == ContactMethodType.Email &&
                             !string.IsNullOrWhiteSpace(cm.ContactMethodText))
                .OrderByDescending(cm => cm.IsVerified)
                .FirstOrDefault()
                .IfNotNull(cm => cm.ContactMethodText, string.Empty);

            // Agency properties
            model.AgencyName = "";
            model.AgencyAddress = "";

            if (model.ExistingContactInfos != null && model.ExistingContactInfos.Any())
                model.ContactInfoID = model.ExistingContactInfos.First().ID;
        }

        private ActionResult ViewDetailsNotFound()
        {
            Server.ClearError();
            Response.TrySkipIisCustomErrors = true;
            Response.StatusCode = 404;
            return View("ViewDetailsNotFound");
        }

        private ActionResult ViewDetailsNotAllowed(PropertyViewDetailsModel model)
        {
            // Not returning 401 result, because it will instruct the Forms Authentication to redirect to LogOn page.

            return View("ViewDetailsNotAllowed", model);
        }

        private void FillNextPrevReturnUrls(PropertyViewDetailsModel model)
        {
            if (model.Params == null || model.Params.Origin == null)
                return;

            switch (model.Params.Origin)
            {
                case PropertyViewDetailsOrigin.Search:
                    var search = PropertySearchQueryUtil.ParseQuery(model.Params.OriginQuery);
                    var searchQuery = PropertySearchQueryUtil.GenerateQuery(search);
                    model.ReturnUrl = Url.Action("Browse", "Properties",
                        new {q = searchQuery, index = model.Params.OriginIndex});

                    if (!model.Params.OriginIndex.HasValue || !model.Params.OriginCount.HasValue)
                        break;

                    if (model.Params.OriginIndex.Value > 1)
                        model.PrevUrl = Url.Action("BrowsePrev", "Properties",
                            new
                            {
                                OriginIndex = model.Params.OriginIndex.Value,
                                OriginCount = model.Params.OriginCount.Value,
                                OriginQuery = searchQuery
                            });
                    if (model.Params.OriginIndex.Value < model.Params.OriginCount.Value)
                        model.NextUrl = Url.Action("BrowseNext", "Properties",
                            new
                            {
                                OriginIndex = model.Params.OriginIndex.Value,
                                OriginCount = model.Params.OriginCount.Value,
                                OriginQuery = searchQuery
                            });

                    break;
            }
        }

        private ActionResult ViewDetailsActionAfterEdit(long id)
        {
            if (User.IsOperator)
                return RedirectToAction("Details", "AdminProperties", new {id, area = AreaNames.Admin});

            return RedirectToAction("ViewDetails", new {id});
        }

        #endregion
    }
}