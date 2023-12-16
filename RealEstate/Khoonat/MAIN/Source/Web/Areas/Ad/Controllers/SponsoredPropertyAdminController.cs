using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.Common.Util.Web.Attributes.MethodSelector;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Core.Services.Ad;
using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Domain.Property;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Areas.Ad.Models.SponsoredPropertyAdmin;

namespace JahanJooy.RealEstate.Web.Areas.Ad.Controllers
{
    public class SponsoredPropertyAdminController : AdminControllerBase
    {
        private const int PageSize = 20;

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public ISponsoredPropertyService SponsoredPropertyService { get; set; }

        [ComponentPlug]
        public IPropertyService PropertyService { get; set; }

        [ComponentPlug]
        public ISponsoredEntityService SponsoredEntityService { get; set; }

        [HttpGet]
        public ActionResult List(SponsoredPropertyAdminListModel model)
        {
            if (model == null)
                model = new SponsoredPropertyAdminListModel();

            var pageNum = 1;
            if (!string.IsNullOrWhiteSpace(model.Page))
                int.TryParse(model.Page, out pageNum);

            var query1 = DbManager.Db.SponsoredPropertyListings
                .Include(s => s.SponsoredEntity.BilledUser)
                .Include(s => s.Listing.Estate);
            query1 = ApplyFilterQuery(model, query1);

            var query2 = query1.Select(spl => new SponsoredPropertyAdminListItemModel
            {
                ID = spl.ID,
                SponsoredEntity = spl.SponsoredEntity,
                SponsoredEntityID = spl.SponsoredEntityID,
                BilledUser = spl.SponsoredEntity.BilledUser,
                ListingID = spl.ListingID,
                Listing = spl.Listing,
                ImpressionsCount = spl.SponsoredEntity.Impressions.Count,
                ClicksCount = spl.SponsoredEntity.Clicks.Count,
                Approved = spl.Approved,
            });


			model.SponsoredProperties = PagedList<SponsoredPropertyAdminListItemModel>.BuildUsingPageNumber(query2.Count(), PageSize,
                pageNum);
            model.SponsoredProperties.FillFrom(query2.OrderByDescending(c => c.SponsoredEntity.CreationTime));

            return View("List", model);
        }

        [HttpGet]
        public ActionResult SetNextRecalcDue(long sponsoredEntityID)
        {
            SponsoredEntityService.SetNextRecalcDue(sponsoredEntityID);

            return RedirectToAction("List", "SponsoredPropertyAdmin");
        }

        [HttpPost]
        public ActionResult Review(long id)
        {
            var sponsoredPropertyListing = DbManager.Db.SponsoredPropertyListings
                .Include(spl => spl.SponsoredEntity)
                .Include(spl => spl.Listing)
                .SingleOrDefault(spl => spl.ID == id);


            if (sponsoredPropertyListing == null)
                return PartialView("_Errors/EntityNotFound");

            var listingDetails = PropertyService.LoadWithAllDetails(sponsoredPropertyListing.ListingID, true);
            var listingSummary = PropertyListingSummary.Summarize(listingDetails);
            return PartialView(new SponsoredPropertyAdminReviewActionModel
            {
                PropertyListingDetails = listingDetails,
                ListingSummary = listingSummary,
                PropertyListing = sponsoredPropertyListing.Listing,
                SponsoredPropertyListing = sponsoredPropertyListing
            });
        }

        [HttpPost]
        [ActionName("ReviewPostBack")]
        [SubmitButton("btnApprove")]
        public ActionResult Approve(long id)
        {
            SponsoredPropertyService.SetApproved(id, true);
            return RedirectToAction("List", "SponsoredPropertyAdmin");
        }

        [HttpPost]
        [ActionName("ReviewPostBack")]
        [SubmitButton("btnReject")]
        public ActionResult Reject(long id)
        {
            SponsoredPropertyService.SetApproved(id, false);
            return RedirectToAction("List", "SponsoredPropertyAdmin");
        }

       
        [HttpPost]
        public ActionResult Edit(long id)
        {
               var sponsoredPropertyListing = DbManager.Db.SponsoredPropertyListings
                .Include(spl => spl.SponsoredEntity)
                .Include(spl => spl.Listing)
                .SingleOrDefault(spl => spl.ID == id);


            if (sponsoredPropertyListing == null)
                return PartialView("_Errors/EntityNotFound");

            return PartialView(new SponsoredPropertyListing
            {
               ListingID = id,
               CustomCaption = sponsoredPropertyListing.CustomCaption
            });
          
        }

        [HttpPost]
        [ActionName("EditPostBack")]
        [SubmitButton("EditCustomCaption")]
        public ActionResult EditCustomCaption(long id, String  customCaption)
        {
            SponsoredPropertyService.EditCustomCaption(id, customCaption);
            return RedirectToAction("List", "SponsoredPropertyAdmin");
        }

        #region Private helper methods

        private static IQueryable<SponsoredPropertyListing> ApplyFilterQuery(SponsoredPropertyAdminListModel model,
            IQueryable<SponsoredPropertyListing> sponsoredPropertyListings)
        {
            if (model.BilledUserIDFilter.HasValue)
                sponsoredPropertyListings =
                    sponsoredPropertyListings.Where(
                        sp => sp.SponsoredEntity.BilledUserID == model.BilledUserIDFilter.Value);

            if (model.ApprovalStatusFilter.HasValue)
            {
                if (model.ApprovalStatusFilter.Value ==
                    SponsoredPropertyAdminListModel.SponsoredEntityApprovalStatus.NotApproved)
                    sponsoredPropertyListings = sponsoredPropertyListings.Where(spl => spl.Approved == null);
                else if (model.ApprovalStatusFilter.Value ==
                         SponsoredPropertyAdminListModel.SponsoredEntityApprovalStatus.Approved)
                    sponsoredPropertyListings = sponsoredPropertyListings.Where(spl => spl.Approved == true);
                else if (model.ApprovalStatusFilter.Value ==
                         SponsoredPropertyAdminListModel.SponsoredEntityApprovalStatus.Rejected)
                    sponsoredPropertyListings = sponsoredPropertyListings.Where(spl => spl.Approved == false);
            }

            if (model.BillingMethodTypeFilter.HasValue)
                sponsoredPropertyListings =
                    sponsoredPropertyListings.Where(
                        sp => sp.SponsoredEntity.BillingMethod == model.BillingMethodTypeFilter.Value);

            if (model.PropertyCodeFilter.HasValue)
                sponsoredPropertyListings =
                    sponsoredPropertyListings.Where(sp => sp.Listing.Code == model.PropertyCodeFilter.Value);

            if (model.TitleFilter != null)
                sponsoredPropertyListings =
                    sponsoredPropertyListings.Where(sp => sp.SponsoredEntity.Title.Contains(model.TitleFilter));

            if (model.PropertyTypeFilter.HasValue)
                sponsoredPropertyListings =
                    sponsoredPropertyListings.Where(sp => sp.Listing.PropertyType == model.PropertyTypeFilter.Value);

            if (model.BlockedForLowCreditFilter.HasValue && model.BlockedForLowCreditFilter.Value )
                
            {
                sponsoredPropertyListings =
                    sponsoredPropertyListings.Where(
                        sp => sp.SponsoredEntity.BlockedForLowCredit == true);
            }
            if (model.BlockedForLowCreditFilter.HasValue && model.BlockedForLowCreditFilter.Value == false)
            {
                sponsoredPropertyListings =
                    sponsoredPropertyListings.Where(
                        sp => sp.SponsoredEntity.BlockedForLowCredit == false);
            }

            return sponsoredPropertyListings;
        }

        #endregion
    }
}