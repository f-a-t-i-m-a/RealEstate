using System;
using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services.Ad;
using JahanJooy.RealEstate.Core.Services.Billing;
using JahanJooy.RealEstate.Domain.Ad;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Core.Impl.Services.Ad
{
    [Component]
    public class SponsoredPropertyService : ISponsoredPropertyService
    {
        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public ITarrifService TarrifService { get; set; }

        #region ISponsoredPropertyService implementation

        public ValidationResult CreateSponsoredProperty(SponsoredEntity sponsoredEntity,
            SponsoredPropertyListing sponsoredPropertyListing)
        {
            if (!ServiceContext.Principal.IsAuthenticated || ServiceContext.CurrentSession.IsCrawler)
                throw new InvalidOperationException("Unknown user or bot");

            var billedUserId = sponsoredEntity.BilledUserID;
            var tarrif = TarrifService.GetTarrif(billedUserId).PropertyListingSponsorshipTarrif;

            if (sponsoredEntity.BillingMethod != SponsoredEntityBillingMethod.PerImpression &&
                sponsoredEntity.BillingMethod != SponsoredEntityBillingMethod.PerClick)
            {
                return ValidationResult.Failure("BillingMethod", AdValidationErrors.UnsupportedPaymentMethod);
            }

            if (sponsoredEntity.BillingMethod == SponsoredEntityBillingMethod.PerImpression &&
                (sponsoredEntity.MaxPayPerImpression < tarrif.PerImpressionMinimumBid))
            {
                return ValidationResult.Failure("MaxPayPerImpression",
                    AdValidationErrors.SpecifiedMaxPayPerImpressionLessThanMinimum);
            }

            if (sponsoredEntity.BillingMethod == SponsoredEntityBillingMethod.PerClick &&
                (sponsoredEntity.MaxPayPerClick < tarrif.PerClickMinimumBid))
            {
                return ValidationResult.Failure("MaxPayPerClick",
                    AdValidationErrors.SpecifiedMaxPayPerClickLessThanMinimum);
            }

            var newSponsoredEntity = new SponsoredEntity
            {
                EntityType = SponsoredEntityType.PropertyListing,
                BilledUserID = billedUserId,
                CreationTime = DateTime.Now,
                LastImpressionTime = null,
                ExpirationTime = sponsoredEntity.ExpirationTime,
                DeleteTime = null,
                Title = sponsoredEntity.Title,
                Enabled = sponsoredEntity.Enabled,
                BillingMethod = sponsoredEntity.BillingMethod,
                MaxPayPerImpression = sponsoredEntity.MaxPayPerImpression,
                MaxPayPerClick = sponsoredEntity.MaxPayPerClick,
                BlockedForLowCredit = true,
                NextRecalcDue = DateTime.Now,
            };

            var newSponsoredPropertyListing = new SponsoredPropertyListing
            {
                SponsoredEntity = newSponsoredEntity,
                ListingID = sponsoredPropertyListing.ListingID,
                ShowInAllPages = sponsoredPropertyListing.ShowInAllPages,
                ShowOnMap = sponsoredPropertyListing.ShowOnMap,
                CustomCaption = sponsoredPropertyListing.CustomCaption,
                IgnoreSearchQuery = sponsoredPropertyListing.IgnoreSearchQuery,
                Approved = null
            };


            DbManager.Db.SponsoredPropertyListingsDbSet.Add(newSponsoredPropertyListing);
            return ValidationResult.Success;
        }

        public ValidationResult UpdateSponsoredProperty(SponsoredEntity sponsoredEntity,
            SponsoredPropertyListing sponsoredPropertyListing)
        {
            if (!ServiceContext.Principal.IsAuthenticated || ServiceContext.CurrentSession.IsCrawler)
                throw new InvalidOperationException("Unknown user or bot");

            var editedSponsoredEntity =
                DbManager.Db.SponsoredEntitiesDbSet.SingleOrDefault(s => s.ID == sponsoredEntity.ID);
            var editedSponsoredPropertyListing =
                DbManager.Db.SponsoredPropertyListingsDbSet.SingleOrDefault(s => s.ListingID == sponsoredPropertyListing.ListingID);
            if (editedSponsoredEntity == null)
                throw new ArgumentException("SponsoredEntity not found.");

            if (editedSponsoredPropertyListing == null)
                throw new ArgumentException("SponsoredPropertyListings not found.");

            if(editedSponsoredPropertyListing.SponsoredEntityID != editedSponsoredEntity.ID)
                throw new ArgumentException("sponsoredEntity and sponsoredPropertyListing dont match.");

            var billedUserId = editedSponsoredEntity.BilledUserID;
            var tarrif = TarrifService.GetTarrif(billedUserId).PropertyListingSponsorshipTarrif;

            if (sponsoredEntity.BillingMethod != SponsoredEntityBillingMethod.PerImpression &&
                sponsoredEntity.BillingMethod != SponsoredEntityBillingMethod.PerClick)
            {
                return ValidationResult.Failure("BillingMethod", AdValidationErrors.UnsupportedPaymentMethod);
            }

            if (sponsoredEntity.BillingMethod == SponsoredEntityBillingMethod.PerImpression &&
                (sponsoredEntity.MaxPayPerImpression < tarrif.PerImpressionMinimumBid))
            {
                return ValidationResult.Failure("MaxPayPerImpression",
                    AdValidationErrors.SpecifiedMaxPayPerImpressionLessThanMinimum);
            }

            if (sponsoredEntity.BillingMethod == SponsoredEntityBillingMethod.PerClick &&
                (sponsoredEntity.MaxPayPerClick < tarrif.PerClickMinimumBid))
            {
                return ValidationResult.Failure("MaxPayPerClick",
                    AdValidationErrors.SpecifiedMaxPayPerClickLessThanMinimum);
            }

            editedSponsoredEntity.ExpirationTime = sponsoredEntity.ExpirationTime;
            editedSponsoredEntity.Title = sponsoredEntity.Title;
            editedSponsoredEntity.BillingMethod = sponsoredEntity.BillingMethod;
            editedSponsoredEntity.MaxPayPerImpression = sponsoredEntity.MaxPayPerImpression;
            editedSponsoredEntity.MaxPayPerClick = sponsoredEntity.MaxPayPerClick;

            editedSponsoredPropertyListing.ShowOnMap = sponsoredPropertyListing.ShowOnMap;
            editedSponsoredPropertyListing.ShowInAllPages = sponsoredPropertyListing.ShowInAllPages;
            editedSponsoredPropertyListing.CustomCaption = sponsoredPropertyListing.CustomCaption;
            editedSponsoredPropertyListing.IgnoreSearchQuery = sponsoredPropertyListing.IgnoreSearchQuery;
            editedSponsoredPropertyListing.Approved = null;

            return ValidationResult.Success;
        }

        public void SetApproved(long sponsoredPropertylistingId, bool? approved)
        {
            var sponsoredPropertyListing = DbManager.Db.SponsoredPropertyListingsDbSet.SingleOrDefault(l => l.ID == sponsoredPropertylistingId);
            if (sponsoredPropertyListing == null)
                throw new ArgumentException("SponsoredPropertyListing not found.");

            sponsoredPropertyListing.Approved = approved;

        }

        public void EditCustomCaption(long sponsoredPropertylistingId, string customCaption)
        {
            var sponsoredPropertyListing = DbManager.Db.SponsoredPropertyListingsDbSet.SingleOrDefault(l => l.ID == sponsoredPropertylistingId);
            if (sponsoredPropertyListing == null)
                throw new ArgumentException("SponsoredPropertyListing not found.");

            sponsoredPropertyListing.CustomCaption = customCaption;
        }

        #endregion
    }
}