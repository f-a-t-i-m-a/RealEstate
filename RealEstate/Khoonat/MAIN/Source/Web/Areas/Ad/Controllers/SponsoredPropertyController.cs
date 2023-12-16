using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.Common.Util.Web.Extensions;
using JahanJooy.RealEstate.Core.Components;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Core.Services.Ad;
using JahanJooy.RealEstate.Core.Services.Billing;
using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Domain.Ad;
using JahanJooy.RealEstate.Domain.Property;
using JahanJooy.RealEstate.Util.Resources;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Areas.Ad.Models.SponsoredProperty;
using JahanJooy.RealEstate.Web.Helpers.Property;

namespace JahanJooy.RealEstate.Web.Areas.Ad.Controllers
{
	[Authorize]
	public class SponsoredPropertyController : CustomControllerBase
	{
		private const int PageSize = 20;

		[ComponentPlug]
		public DbManager DbManager { get; set; }

		[ComponentPlug]
		public ITarrifService TarrifService { get; set; }

		[ComponentPlug]
		public ISponsoredPropertyService SponsoredPropertyService { get; set; }

		[ComponentPlug]
		public ISponsoredEntityService SponsoredEntityService { get; set; }

		[ComponentPlug]
		public IPropertyService PropertyService { get; set; }

		[HttpGet]
		public ActionResult ViewSponsorshipStatus(long id)
		{
			var listing = DbManager.Db.PropertyListings.Include(p => p.Sponsorships).SingleOrDefault(p => p.ID == id);
			if (listing == null)
				return Error(ErrorResult.EntityNotFound);

			if (!PropertyControllerHelper.CanEditListing(listing))
				return Error(ErrorResult.AccessDenied);

			var model = new SponsoredPropertyViewSponsorshipStatusModel
			{
				PropertyListing = listing,
				Sponsorships = listing.Sponsorships.Where(s => !s.SponsoredEntity.DeleteTime.HasValue).ToList()
			};

			return View(model);
		}

		[HttpGet]
		public ActionResult NewSponsorship(long id)
		{
			var model = new SponsoredPropertyEditSponsorshipModel {PropertyListingID = id};
			PopulateNewSponsorshipOutput(model);

			if (model.PropertyListing == null)
				return Error(ErrorResult.EntityNotFound);

			if (!PropertyControllerHelper.CanEditListing(model.PropertyListing))
				return Error(ErrorResult.AccessDenied);
			var listing = PropertyService.LoadWithAllDetails(id);
			var listingSummary = PropertyListingSummary.Summarize(listing);

			model.PropertyListingID = id;
			model.ShowInAllPages = true;
			model.ShowOnMap = true;
			model.DaysBeforeExpiration = null;
			model.Title = string.Format(SponsoredPropertyControllerResources.NewSponsorship_DefaultTitleFormat,
				model.PropertyListing.Code);
			model.BillingMethod = SponsoredEntityBillingMethod.PerImpression;

			model.ListingSummary = listingSummary;
			model.IgnoreSearchQuery = true;

			var maxBid = GetMaximumBid(null);
			if (maxBid.HasValue)
			{
				model.PerImpressionMaximumBid = maxBid.Value;
				model.PerClickMaximumBid = Math.Round(maxBid.Value/AdConstants.DefaultClicksPerImpression, BillingContants.MoneyPrecision);
			}
			else
			{
				model.PerImpressionMaximumBid = 3 * model.Tarrif.PerImpressionMinimumBid;
				model.PerClickMaximumBid = 3 * model.Tarrif.PerClickMinimumBid;
			}

			model.MaxPayPerImpression = Math.Round((model.PerImpressionMaximumBid)*80/100, BillingContants.MoneyPrecision);
			model.MaxPayPerClick = Math.Round((model.PerClickMaximumBid)*80/100, BillingContants.MoneyPrecision);

			return View(model);
		}

		[HttpPost]
		[ActionName("NewSponsorship")]
		public ActionResult NewSponsorshipPostback(SponsoredPropertyEditSponsorshipModel model)
		{
			if (model == null)
				return RedirectToAction("NewSponsorship");

			PopulateNewSponsorshipOutput(model);
			if (model.PropertyListing == null)
				return Error(ErrorResult.EntityNotFound);

			if (!PropertyControllerHelper.CanEditListing(model.PropertyListing))
				return Error(ErrorResult.AccessDenied);

			if (!ModelState.IsValid)
				return View(model);

			var domain = model.ToDomain();
			domain.SponsoredEntity.BilledUserID = User.CoreIdentity.UserId.GetValueOrDefault();

			var result = SponsoredPropertyService.CreateSponsoredProperty(domain.SponsoredEntity, domain);
			if (result.IsValid)
				return RedirectToAction("ViewDetails", "Property",
					new {area = AreaNames.Main, id = model.PropertyListingID});

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(error.PropertyPath,
					SponsoredPropertyControllerResources.ResourceManager.GetString(error.FullResourceKey));
			}

			return View(model);
		}

		private void PopulateNewSponsorshipOutput(SponsoredPropertyEditSponsorshipModel model)
		{
			model.PropertyListing =
				DbManager.Db.PropertyListings.Include(p => p.Sponsorships)
					.SingleOrDefault(p => p.ID == model.PropertyListingID);
			model.Tarrif = TarrifService.GetTarrif(User.CoreIdentity.UserId).PropertyListingSponsorshipTarrif;
			model.BillingMethodSelectList = new[]
			{SponsoredEntityBillingMethod.PerImpression, SponsoredEntityBillingMethod.PerClick}
				.AsEnumerable().EnumSelectList(DomainAdEnumResources.ResourceManager);
		}

		[HttpGet]
		public ActionResult List(SponsoredPropertyListModel model)
		{
			if (model == null)
				model = new SponsoredPropertyListModel();

			int pageNum = 1;
			if (!string.IsNullOrWhiteSpace(model.Page))
				int.TryParse(model.Page, out pageNum);

			long userId = User.CoreIdentity.UserId.GetValueOrDefault();


			var query =
				DbManager.Db.SponsoredPropertyListings.Include(s => s.SponsoredEntity)
					.Include(s => s.SponsoredEntity.BilledUser)
					.Include(s => s.Listing)
					.Include(s => s.Listing.Unit)
					.Where(s => s.SponsoredEntity.BilledUserID == userId);


			model.SponsoredProperties = PagedList<SponsoredPropertyListing>.BuildUsingPageNumber(query.Count(), PageSize,
				pageNum);
			model.SponsoredProperties.FillFrom(query.OrderByDescending(c => c.SponsoredEntity.CreationTime));

			return View(model);
		}

		[HttpGet]
		[ActionName("EditSponsorship")]
		public ActionResult EditSponsorship(long sponsoredEntityID, long propertyListingID)
		{
			var sponsoredPropertyListing = DbManager.Db.SponsoredPropertyListings
				.Include(s => s.SponsoredEntity)
				.SingleOrDefault(s => s.SponsoredEntityID == sponsoredEntityID);

			if (sponsoredPropertyListing == null || sponsoredPropertyListing.SponsoredEntity == null)
				return Error(ErrorResult.EntityNotFound);

			var model = new SponsoredPropertyEditSponsorshipModel {PropertyListingID = propertyListingID};
			PopulateNewSponsorshipOutput(model);

			if (model.PropertyListing == null)
				return Error(ErrorResult.EntityNotFound);

			if (!PropertyControllerHelper.CanEditListing(model.PropertyListing))
				return Error(ErrorResult.AccessDenied);

			var listing = PropertyService.LoadWithAllDetails(propertyListingID);
			var listingSummary = PropertyListingSummary.Summarize(listing);

			var maxBid = GetMaximumBid(sponsoredEntityID);
			if (maxBid.HasValue)
			{
				var estimatedClicksPerImpression = Math.Max(sponsoredPropertyListing.SponsoredEntity.EstimatedClicksPerImpression, AdConstants.MinimumClicksPerImpression);
				model.PerImpressionMaximumBid = maxBid.Value;
				model.PerClickMaximumBid = Math.Round(maxBid.Value/estimatedClicksPerImpression, BillingContants.MoneyPrecision);
			}
			else
			{
				model.PerImpressionMaximumBid = 3*model.Tarrif.PerImpressionMinimumBid;
				model.PerClickMaximumBid = 3*model.Tarrif.PerClickMinimumBid;
			}

			model.PropertyListingID = propertyListingID;
			model.ShowInAllPages = sponsoredPropertyListing.ShowInAllPages;
			model.ShowOnMap = sponsoredPropertyListing.ShowOnMap;

			model.IgnoreSearchQuery = sponsoredPropertyListing.IgnoreSearchQuery;

			model.CustomCaption = sponsoredPropertyListing.CustomCaption;
			model.DaysBeforeExpiration = null;
			model.Title = sponsoredPropertyListing.SponsoredEntity.Title;
			model.BillingMethod = sponsoredPropertyListing.SponsoredEntity.BillingMethod;
			model.ExpirationTime = sponsoredPropertyListing.SponsoredEntity.ExpirationTime;
			model.ListingSummary = listingSummary;

			if (sponsoredPropertyListing.SponsoredEntity.BillingMethod == SponsoredEntityBillingMethod.PerImpression)
			{
				model.MaxPayPerImpression = sponsoredPropertyListing.SponsoredEntity.MaxPayPerImpression;
				model.MaxPayPerClick = Math.Round((model.PerClickMaximumBid)*80/100, 0);
			}

			if (sponsoredPropertyListing.SponsoredEntity.BillingMethod == SponsoredEntityBillingMethod.PerClick)
			{
				model.MaxPayPerImpression = Math.Round((model.PerImpressionMaximumBid)*80/100, 0);
				model.MaxPayPerClick = sponsoredPropertyListing.SponsoredEntity.MaxPayPerClick;
			}

			return View("EditSponsorship", model);
		}

		[HttpPost]
		[ActionName("EditSponsorship")]
		public ActionResult EditSponsorshipPostback(SponsoredPropertyEditSponsorshipModel model, long sponsoredEntityID,
			long propertyListingID)
		{
			if (model == null)
				return RedirectToAction("EditSponsorship");

			PopulateNewSponsorshipOutput(model);
			if (model.PropertyListing == null)
				return Error(ErrorResult.EntityNotFound);

			if (!PropertyControllerHelper.CanEditListing(model.PropertyListing))
				return Error(ErrorResult.AccessDenied);

			if (!ModelState.IsValid)
				return View(model);

			var domain = model.ToDomain();
			domain.SponsoredEntity.ID = sponsoredEntityID;
			domain.ListingID = propertyListingID;

			var result = SponsoredPropertyService.UpdateSponsoredProperty(domain.SponsoredEntity, domain);
			if (result.IsValid)
				return RedirectToAction("ViewDetails", "Property",
					new {area = AreaNames.Main, id = model.PropertyListingID});

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(error.PropertyPath,
					SponsoredPropertyControllerResources.ResourceManager.GetString(error.FullResourceKey));
			}

			return View(model);
		}


		[HttpPost]
		[ActionName("ChangeSponsorshipMode")]
		public ActionResult ChangeSponsorshipMode(long sponsoredEntityID, bool enabled, long propertyListingID,
			string referringController)
		{
			SponsoredEntityService.SetEnabled(sponsoredEntityID, enabled);

			if (referringController.ToLower() == "property")
				return RedirectToAction("ViewDetails", "Property", new {id = propertyListingID, area = AreaNames.Main});
			
			//else, referringController == "sponsoredproperty"
			return RedirectToAction("List", "SponsoredProperty");
		}

		[HttpPost]
		public ActionResult ConfirmationPopup(long sponsoredEntityID, bool enabled, long propertyListingID,
			string referringController)
		{
			var listing = PropertyService.LoadWithAllDetails(propertyListingID);
			var listingSummary = PropertyListingSummary.Summarize(listing);
			var model = new SponsoredPropertyConfirmationPopupModel
			{
				ListingSummary = listingSummary,
				SponsoredEntityID = sponsoredEntityID,
				Enabled = enabled,
				ReferringController = referringController
			};
			return PartialView("ConfirmationPopup", model);
		}

		private decimal? GetMaximumBid(long? excludedSponsoredEntityId)
		{
			var todayDate = DateTime.Now;
			var query = DbManager.Db.SponsoredPropertyListings
				.Where(spl =>
					spl.Approved == true && !spl.Listing.DeleteDate.HasValue &&
					(spl.Listing.PublishEndDate >= todayDate || !spl.Listing.PublishDate.HasValue) &&
					spl.SponsoredEntity.Enabled && spl.SponsoredEntity.BlockedForLowCredit == false &&
					spl.SponsoredEntity.BillingMethod == SponsoredEntityBillingMethod.PerImpression);

			if (excludedSponsoredEntityId.HasValue)
				query = query.Where(spl => spl.SponsoredEntityID != excludedSponsoredEntityId.Value);

			return query.Max(spl => (decimal?) spl.SponsoredEntity.ProjectedMaxPayPerImpression);
		}
	}
}