using System;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util.Web.Captcha;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Models.PropertyRequest;

namespace JahanJooy.RealEstate.Web.Controllers
{
	public class PropertyRequestController : CustomControllerBase
	{
		#region Injected dependencies

		[ComponentPlug]
		public IAuthenticationService AuthenticationService { get; set; }

		[ComponentPlug]
		public IPropertyRequestService PropertyRequestService { get; set; }

		[ComponentPlug]
		public DbManager DbManager { get; set; }

		[ComponentPlug]
		public IVicinityCache VicinityCache { get; set; }

		#endregion

		[HttpGet]
		public ActionResult Create()
		{
			var model = new PropertyRequestEditModel();
			return View("Create", model);
		}

		[HttpPost]
		[ActionName("Create")]
		[ValidateCaptcha(validateScriptCaptcha: true)]
		public ActionResult CreatePostback()
		{
			var model = new PropertyRequestEditModel();
			if (!TryUpdateModel(model))
			{
				return View("Create", model);
			}
            return View("Create", model);
//			var requestContent = Mapper.Map(model, new PropertyRequestContent());

			
			
//			PropertyRequestService.ValidateForSave(new PropertyRequest());

//			var listing = model.ToDomain();
//			ValidationResult validationResult = model.PublishDuration.HasValue
//				? PropertyService.ValidateForPublish(PropertyListingDetails.MakeDetails(listing))
//				: PropertyService.ValidateForSave(listing);

//			if (!validationResult.IsValid)
//			{
//				model.AddDomainValidationErrors(validationResult, ModelState);
//				PopulateContactInfoSelectItems(model);
//				return View("QuickCreate", model);
//			}

//			PropertyService.Save(listing);
//			if (model.PublishDuration.HasValue)
//				PropertyService.Publish(listing.ID, (int)model.PublishDuration);

//			if (!User.Identity.IsAuthenticated)
//			{
//				if (SessionInfo.OwnedProperties == null)
//					SessionInfo.OwnedProperties = new HashSet<long>();

//				SessionInfo.OwnedProperties.Add(listing.ID);
//				OwnedPropertiesCookieUtil.Set(SessionInfo.OwnedProperties);
//			}

//			return RedirectToAction("QuickCreateCompleted", new { id = listing.ID });
		}

		[HttpGet]
		public ActionResult CreateCompleted(long id)
		{
//			var listing = PropertyRequestService.LoadWithAllDetails(id);
			// Loading all details because we need to validate for publish

//			if (!PropertyControllerHelper.DetermineIfUserIsOwner(listing) && !User.IsOperator)
//				return RedirectToAction("ViewDetails", new { id });

//			var listingEditPassword =
//				DbManager.Db.PropertyListings.Where(l => l.ID == id).Select(l => l.EditPassword).SingleOrDefault();
//			var listingSummary = PropertyListingSummary.Summarize(listing);
//			var validationResult = PropertyService.ValidateForPublish(listing);

//			var model = new PropertyQuickCreateCompletedModel
//			{
//				Listing = listing,
//				ListingEditPassword = listingEditPassword,
//				ListingSummary = listingSummary,
//				PublishValidationResult = validationResult
//			};
//			return View(model);
			throw new NotImplementedException();
		}
	}
}