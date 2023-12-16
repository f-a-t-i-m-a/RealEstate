using System;
using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.Common.Util.Web.Attributes.MethodSelector;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Domain.Property;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminProperties;
using JahanJooy.RealEstate.Web.Models.Property;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Controllers
{
	public class AdminPropertiesController : AdminControllerBase
	{
		[ComponentPlug]
		public DbManager DbManager { get; set; }

		[ComponentPlug]
		public IPropertyService PropertyService { get; set; }

		public static string DefaultList(UrlHelper url)
		{
			return url.Action("List", "AdminProperties", new AdminPropertiesListModel  { ApprovalStatusFilter = PropertyListingApprovalStatus.NotApproved, DeletedFilter = false });
		}

		[HttpGet]
		public ActionResult List(AdminPropertiesListModel model)
		{
			if (model == null)
				model = new AdminPropertiesListModel();

			int pageNum = 1;
			if (!string.IsNullOrWhiteSpace(model.Page))
				int.TryParse(model.Page, out pageNum);

			IQueryable<PropertyListing> propsQuery = DbManager.Db.PropertyListingsDbSet;
			propsQuery = ApplyFilterQuery(model, propsQuery);

			var propsSummaryQuery = PropertyListingSummary.Summarize(propsQuery);
			model.PropertyListings = PagedList<PropertyListingSummary>.BuildUsingPageNumber(propsSummaryQuery.Count(), 20, pageNum);
			model.PropertyListings.FillFrom(propsSummaryQuery.OrderByDescending(l => l.ModificationDate));

			return View(model);
		}

		[HttpGet]
		public ActionResult Details(long? id)
		{
			if (!id.HasValue)
				return View("DetailsNotFound");

			// Directly use PropertyListingDbSet so that it includes deleted listings too.
			var listing = PropertyListingDetails.MakeDetails(DbManager.Db.PropertyListingsDbSet.Where(l => l.ID == id.Value), true, true, true, true, true).SingleOrDefault();
			if (listing == null)
				return View("DetailsNotFound");

			var model = new PropertyViewDetailsModel {Listing = listing, ListingSummary = PropertyListingSummary.Summarize(listing), IsOwner = false};
			return View(model);
		}

		[HttpPost]
		[ActionName("ReviewAction")]
		[SubmitButton("btnClearApproval")]
		public ActionResult ClearApproval(long id)
		{
			PropertyService.SetApproved(id, null);
			return RedirectToAction("Details", new { id });
		}

		[HttpPost]
		[ActionName("ReviewAction")]
		[SubmitButton("btnApprove")]
		public ActionResult Approve(long id)
		{
			PropertyService.SetApproved(id, true);
			return NextForReview(id);
		}

		[HttpPost]
		[ActionName("ReviewAction")]
		[SubmitButton("btnReject")]
		public ActionResult Reject(long id)
		{
			PropertyService.SetApproved(id, false);
			return NextForReview(id);
		}

		[HttpPost]
		[ActionName("ReviewAction")]
		[SubmitButton("btnNext")]
		public ActionResult NextForReview(long id)
		{
			var db = DbManager.Db;
			var next = db.PropertyListingsDbSet
				.Where(pl => pl.ModificationDate > db.PropertyListingsDbSet.Where(pl2 => pl2.ID == id).Select(pl2 => pl2.ModificationDate).FirstOrDefault() && !pl.DeleteDate.HasValue && !pl.Approved.HasValue)
				.OrderBy(pl => pl.ModificationDate)
				.Select(pl => (long?)pl.ID)
				.FirstOrDefault();

			return next.HasValue ? (ActionResult)RedirectToAction("Details", new { id = next.Value }) : Redirect(DefaultList(Url));
		}

		[HttpPost]
		[ActionName("ReviewAction")]
		[SubmitButton("btnPrevious")]
		public ActionResult PreviousForReview(long id)
		{
			var db = DbManager.Db;
			var prev = db.PropertyListingsDbSet
				.Where(pl => pl.ModificationDate < db.PropertyListingsDbSet.Where(pl2 => pl2.ID == id).Select(pl2 => pl2.ModificationDate).FirstOrDefault() && !pl.DeleteDate.HasValue && !pl.Approved.HasValue)
				.OrderByDescending(pl => pl.ModificationDate)
				.Select(pl => (long?)pl.ID)
				.FirstOrDefault();

			return prev.HasValue ? (ActionResult)RedirectToAction("Details", new { id = prev.Value }) : Redirect(DefaultList(Url));
		}

		#region Private helper methods

		private static IQueryable<PropertyListing> ApplyFilterQuery(AdminPropertiesListModel model, IQueryable<PropertyListing> propsQuery)
		{
			if (model.PropertyListingIdFilter.HasValue)
				propsQuery = propsQuery.Where(p => p.ID == model.PropertyListingIdFilter.Value);

			if (model.PropertyListingCodeFilter.HasValue)
				propsQuery = propsQuery.Where(p => p.Code == model.PropertyListingCodeFilter.Value);

			if (model.ApplyOwnerUserIdFilter)
			{
				propsQuery = model.OwnerUserIdFilter.HasValue
					             ? propsQuery.Where(p => p.OwnerUserID == model.OwnerUserIdFilter.Value)
					             : propsQuery.Where(p => p.OwnerUserID == null);
			}

			if (model.PublishStatusFilter.HasValue)
			{
				if (model.PublishStatusFilter.Value == PropertyListingPublishStatus.NeverPublished)
					propsQuery = propsQuery.Where(l => l.PublishEndDate == null);
				else if (model.PublishStatusFilter.Value == PropertyListingPublishStatus.PublishedAndPassed)
					propsQuery = propsQuery.Where(l => l.PublishEndDate < DateTime.Now);
				else if (model.PublishStatusFilter.Value == PropertyListingPublishStatus.PublishedAndCurrent)
					propsQuery = propsQuery.Where(l => l.PublishEndDate > DateTime.Now);
			}

			if (model.ApprovalStatusFilter.HasValue)
			{
				if (model.ApprovalStatusFilter.Value == PropertyListingApprovalStatus.NotApproved)
					propsQuery = propsQuery.Where(p => p.Approved == null);
				else if (model.ApprovalStatusFilter.Value == PropertyListingApprovalStatus.Approved)
					propsQuery = propsQuery.Where(p => p.Approved == true);
				else if (model.ApprovalStatusFilter.Value == PropertyListingApprovalStatus.Rejected)
					propsQuery = propsQuery.Where(p => p.Approved == false);
			}

			if (model.DeletedFilter.HasValue)
			{
				propsQuery = model.DeletedFilter.Value ? propsQuery.Where(p => p.DeleteDate.HasValue) : propsQuery.Where(p => !p.DeleteDate.HasValue);
			}

            if (model.GeographicLocationTypeFilter.HasValue)
            {
                if (model.GeographicLocationTypeFilter.Value == GeographicLocationSpecificationType.UserSpecifiedExact)
                    propsQuery = propsQuery.Where(l => l.GeographicLocationType == GeographicLocationSpecificationType.UserSpecifiedExact);
                else if (model.GeographicLocationTypeFilter.Value == GeographicLocationSpecificationType.InferredFromVicinity)
                    propsQuery = propsQuery.Where(l => l.GeographicLocationType == GeographicLocationSpecificationType.InferredFromVicinity);
                else if (model.GeographicLocationTypeFilter.Value == GeographicLocationSpecificationType.InferredFromVicinityAndAddress)
                    propsQuery = propsQuery.Where(l => l.GeographicLocationType == GeographicLocationSpecificationType.InferredFromVicinityAndAddress);
                else if (model.GeographicLocationTypeFilter.Value == GeographicLocationSpecificationType.UserSpecifiedApproximate)
                    propsQuery = propsQuery.Where(l => l.GeographicLocationType == GeographicLocationSpecificationType.UserSpecifiedApproximate);
            }
            if (!string.IsNullOrWhiteSpace(model.ConnectionInfoFilter))
            {
                propsQuery = propsQuery.Where(p => p.ContactInfo.AgencyName.Contains(model.ConnectionInfoFilter) ||
                                                   p.ContactInfo.AgencyAddress.Contains(model.ConnectionInfoFilter) ||
                                                   p.ContactInfo.ContactName.Contains(model.ConnectionInfoFilter) ||
                                                   p.ContactInfo.ContactPhone1.Contains(model.ConnectionInfoFilter) ||
                                                   p.ContactInfo.ContactPhone2.Contains(model.ConnectionInfoFilter) ||
                                                   p.ContactInfo.ContactEmail.Contains(model.ConnectionInfoFilter)
                                            );
            }

			return propsQuery;
		}

		#endregion

	}
}