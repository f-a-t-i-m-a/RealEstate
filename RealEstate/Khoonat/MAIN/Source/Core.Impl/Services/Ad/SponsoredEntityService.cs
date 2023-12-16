using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Compositional.Composer;
using JahanJooy.RealEstate.Core.Components;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services.Ad;
using JahanJooy.RealEstate.Core.Services.Billing;
using JahanJooy.RealEstate.Domain.Ad;
using JahanJooy.RealEstate.Domain.Billing;
using JahanJooy.RealEstate.Domain.Messages;

namespace JahanJooy.RealEstate.Core.Impl.Services.Ad
{
    [Component]
    public class SponsoredEntityService : ISponsoredEntityService
    {
        private const int BillImpressionBatchSize = 50;
        private const int FinalizePartialImpressionBillingsBatchSize = 10;
        private const int BillClickBatchSize = 50;
        private const int FinalizePartialClickBillingsBatchSize = 10;

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IUserBillingComponent UserBillingComponent { get; set; }

        [ComponentPlug]
        public IUserBalanceService UserBalanceService { get; set; }

        #region  ISponsoredEntityService implementation

        public void SetEnabled(long sponsoredEntityID, bool enabled)
        {
            var sponsoredEntity = DbManager.Db.SponsoredEntitiesDbSet.SingleOrDefault(s => s.ID == sponsoredEntityID);
            if (sponsoredEntity == null)
                throw new ArgumentException("SponsoredEntity not found.");

            sponsoredEntity.Enabled = enabled;
        }

        public bool BillImpressions()
        {
            var impressions = DbManager.Db.SponsoredEntityImpressionsDbSet
                .Include(i => i.SponsoredEntity)
                .Where(
                    i =>
                        !i.BillingEntityID.HasValue &&
                        i.SponsoredEntity.BillingMethod == SponsoredEntityBillingMethod.PerImpression)
                .OrderBy(i => i.SponsoredEntityID)
                .Take(BillImpressionBatchSize)
                .ToList();

            var billingsToRecalc = new List<SponsoredEntityImpressionBilling>();
            foreach (var impression in impressions)
            {
                var impressionsDate = impression.CreationTime.Date;

                var billing = billingsToRecalc.FirstOrDefault(
                    b => b.SponsoredEntityID == impression.SponsoredEntityID &&
                         b.ImpressionsDate == impressionsDate);

                if (billing == null)
                {
                    billing = DbManager.Db.SponsoredEntityImpressionBillingsDbSet
                        .FirstOrDefault(b => b.SponsoredEntityID == impression.SponsoredEntityID &&
                                             b.ImpressionsDate == impressionsDate &&
                                             (b.BillingState == BillingSourceEntityState.PartiallyApplied ||
                                              b.BillingState == BillingSourceEntityState.Pending));
                }

                if (billing == null)
                {
                    billing = new SponsoredEntityImpressionBilling
                    {
                        CreationTime = DateTime.Now,
                        ImpressionsDate = impressionsDate,
                        NumberOfImpressions = 0,
                        TotalAmount = 0,
                        SponsoredEntityID = impression.SponsoredEntityID,
                        BillingState = BillingSourceEntityState.Pending,
                        TargetUserID = impression.SponsoredEntity.BilledUserID
                    };

                    DbManager.Db.SponsoredEntityImpressionBillingsDbSet.Add(billing);
                }

                impression.BillingEntity = billing;
                billing.NumberOfImpressions++;
                billing.TotalAmount += impression.BidAmount;

                if (!billingsToRecalc.Contains(billing))
                    billingsToRecalc.Add(billing);
            }

            DbManager.SaveDefaultDbChanges();
            var billingResults =
                UserBillingComponent.PartiallyApplyOrRecalculate<SponsoredEntityImpressionBilling>(billingsToRecalc);

            foreach (var billingResult in billingResults)
            {
                var sponsoredEntityId =
                    ((SponsoredEntityImpressionBilling) billingResult.SourceEntity).SponsoredEntityID;
                UserBalanceService.OnUserCostProcessed(billingResult.BalanceAfterEffect, sponsoredEntityId,
                    NotificationSourceEntityType.SponsoredEntity);

                if (!billingResult.BalanceAfterEffect.CanSpend)
                {
                    var sponsoredEntity = DbManager.Db.SponsoredEntitiesDbSet.Find(sponsoredEntityId);
                    sponsoredEntity.BlockedForLowCredit = true;
                }
            }

            return impressions.Count >= BillImpressionBatchSize;
        }

        public bool FinalizePartialImpressionBillings()
        {
            var maxImpressionsDate = DateTime.Now.Date.AddDays(-1);

            var billings = DbManager.Db.SponsoredEntityImpressionBillingsDbSet
                .Where(b => b.BillingState == BillingSourceEntityState.PartiallyApplied &&
                            b.ImpressionsDate <= maxImpressionsDate)
                .OrderBy(b => b.ImpressionsDate)
                .ThenBy(b => b.SponsoredEntityID)
                .Take(FinalizePartialImpressionBillingsBatchSize)
                .ToList();

            foreach (var billing in billings)
            {
                // Load all related Impressions

                var billingId = billing.ID;
                var sponsoredEntityID = billing.SponsoredEntityID;
                var impressionTimeMin = billing.ImpressionsDate;
                var impressionTimeMax = billing.ImpressionsDate.AddDays(1);

                var impressions = DbManager.Db.SponsoredEntityImpressionsDbSet
                    .Where(i => (i.BillingEntityID == billingId) ||
                                (i.CreationTime >= impressionTimeMin && i.CreationTime < impressionTimeMax &&
                                 i.SponsoredEntityID == sponsoredEntityID))
                    .ToList();

                // Iterate over the Impressions and make corrections

                foreach (var impression in impressions)
                {
                    if (impression.CreationTime.Date != billing.ImpressionsDate)
                    {
                        // If the impressions is assigned to this billing by mistake, remove it.
                        // The billing scheduler will pick it up and process it later.
                        impression.BillingEntityID = null;
                    }
                    else
                    {
                        // If the impressions is assigned to another billing by mistake, 
                        // re-assign it to this billing.
                        impression.BillingEntityID = billingId;
                    }
                }

                // Recalculate the totals
                billing.NumberOfImpressions = impressions.Count(i => i.BillingEntityID == billingId);
                billing.TotalAmount =
                    impressions.Where(n => n.BillingEntityID == billingId).Sum(n => n.BidAmount);
                billing.CompletionTime = DateTime.Now;
            }

            UserBillingComponent.ApplyOrFinalizePartiallyApplied<SponsoredEntityImpressionBilling>(billings);
            return billings.Count >= FinalizePartialImpressionBillingsBatchSize;
        }

        public SponsoredEntityImpression CreateClick(long impressionId, Guid impressionGuid)
        {
            if (ServiceContext.CurrentSession.IsCrawler)
                throw new InvalidOperationException("Not a user-interactive session.");

            var maxCreationTime = DateTime.Now.AddSeconds(-1);
            var minCreationTime = DateTime.Now.AddMinutes(-30);

            var sponsoredeEntityImpression = DbManager.Db.SponsoredEntityImpressions
                .Include(s => s.SponsoredEntity).SingleOrDefault(s =>
                    s.GUID == impressionGuid &&
                    s.ID == impressionId &&
                    s.CreationTime < maxCreationTime &&
                    s.CreationTime > minCreationTime);

            if (sponsoredeEntityImpression == null)
                return null;

            if (DbManager.Db.SponsoredEntityClicks.Any(s => s.ImpressionID == sponsoredeEntityImpression.ID))
                return sponsoredeEntityImpression;

            var sponsoredEntityClick = new SponsoredEntityClick
            {
                CreationTime = DateTime.Now,
                SponsoredEntityID = sponsoredeEntityImpression.SponsoredEntityID,
                ImpressionID = sponsoredeEntityImpression.ID,
                HttpSessionID = ServiceContext.CurrentSession.Record.ID,
            };

            DbManager.Db.SponsoredEntityClicksDbSet.Add(sponsoredEntityClick);

            return sponsoredeEntityImpression;
        }

        public bool BillClicks()
        {
            var clicks = DbManager.Db.SponsoredEntityClicksDbSet
                .Include(i => i.SponsoredEntity)
                .Include(i => i.Impression)
                .Where(
                    i =>
                        !i.BillingEntityID.HasValue &&
                        i.SponsoredEntity.BillingMethod == SponsoredEntityBillingMethod.PerClick)
                .OrderBy(i => i.SponsoredEntityID)
                .Take(BillClickBatchSize)
                .ToList();

            var billingsToRecalc = new List<SponsoredEntityClickBilling>();
            foreach (var click in clicks)
            {
                var clicksDate = click.CreationTime.Date;

                var billing = billingsToRecalc.FirstOrDefault(
                    b => b.SponsoredEntityID == click.SponsoredEntityID &&
                         b.ClicksDate == clicksDate);

                if (billing == null)
                {
                    billing = DbManager.Db.SponsoredEntityClickBillingsDbSet
                        .FirstOrDefault(b => b.SponsoredEntityID == click.SponsoredEntityID &&
                                             b.ClicksDate == clicksDate &&
                                             (b.BillingState == BillingSourceEntityState.PartiallyApplied ||
                                              b.BillingState == BillingSourceEntityState.Pending));
                }

                if (billing == null)
                {
                    billing = new SponsoredEntityClickBilling
                    {
                        CreationTime = DateTime.Now,
                        ClicksDate = clicksDate,
                        NumberOfClicks = 0,
                        TotalAmount = 0,
                        SponsoredEntityID = click.SponsoredEntityID,
                        BillingState = BillingSourceEntityState.Pending,
                        TargetUserID = click.SponsoredEntity.BilledUserID
                    };

                    DbManager.Db.SponsoredEntityClickBillingsDbSet.Add(billing);
                }

                click.BillingEntity = billing;
                billing.NumberOfClicks++;
                billing.TotalAmount += click.Impression.BidAmount;

                if (!billingsToRecalc.Contains(billing))
                    billingsToRecalc.Add(billing);
            }

            DbManager.SaveDefaultDbChanges();
            var billingResults =
                UserBillingComponent.PartiallyApplyOrRecalculate<SponsoredEntityClickBilling>(billingsToRecalc);

            foreach (var billingResult in billingResults)
            {
                var sponsoredEntityId = ((SponsoredEntityClickBilling) billingResult.SourceEntity).SponsoredEntityID;
                UserBalanceService.OnUserCostProcessed(billingResult.BalanceAfterEffect, sponsoredEntityId,
                    NotificationSourceEntityType.SponsoredEntity);

                if (!billingResult.BalanceAfterEffect.CanSpend)
                {
                    var sponsoredEntity = DbManager.Db.SponsoredEntitiesDbSet.Find(sponsoredEntityId);
                    sponsoredEntity.BlockedForLowCredit = true;
                }
            }

            return clicks.Count >= BillClickBatchSize;
        }

        public bool FinalizePartialClickBillings()
        {
            var maxClicksDate = DateTime.Now.Date.AddDays(-1);

            var billings = DbManager.Db.SponsoredEntityClickBillingsDbSet
                .Where(b => b.BillingState == BillingSourceEntityState.PartiallyApplied &&
                            b.ClicksDate <= maxClicksDate)
                .OrderBy(b => b.ClicksDate)
                .ThenBy(b => b.SponsoredEntityID)
                .Take(FinalizePartialClickBillingsBatchSize)
                .ToList();

            foreach (var billing in billings)
            {
                // Load all related Click

                var billingId = billing.ID;
                var sponsoredEntityID = billing.SponsoredEntityID;
                var clickTimeMin = billing.ClicksDate;
                var clickTimeMax = billing.ClicksDate.AddDays(1);

                var clicks = DbManager.Db.SponsoredEntityClicksDbSet
                    .Include(c => c.Impression)
                    .Where(i => (i.BillingEntityID == billingId) ||
                                (i.CreationTime >= clickTimeMin && i.CreationTime < clickTimeMax &&
                                 i.SponsoredEntityID == sponsoredEntityID))
                    .ToList();

                // Iterate over the Clicks and make corrections

                foreach (var click in clicks)
                {
                    if (click.CreationTime.Date != billing.ClicksDate)
                    {
                        // If the clicks is assigned to this billing by mistake, remove it.
                        // The billing scheduler will pick it up and process it later.
                        click.BillingEntityID = null;
                    }
                    else
                    {
                        // If the clicks is assigned to another billing by mistake, 
                        // re-assign it to this billing.
                        click.BillingEntityID = billingId;
                    }
                }

                // Recalculate the totals
                billing.NumberOfClicks = clicks.Count(c => c.BillingEntityID == billingId);
                billing.TotalAmount = clicks.Where(c => c.BillingEntityID == billingId).Sum(c => c.Impression.BidAmount);
                billing.CompletionTime = DateTime.Now;
            }

            UserBillingComponent.ApplyOrFinalizePartiallyApplied<SponsoredEntityClickBilling>(billings);
            return billings.Count >= FinalizePartialClickBillingsBatchSize;
        }

        public void SetNextRecalcDue(long sponsoredEntityID)
        {
            var sponsoredEntity = DbManager.Db.SponsoredEntitiesDbSet.SingleOrDefault(s => s.ID == sponsoredEntityID);
            if (sponsoredEntity == null)
                throw new ArgumentException("SponsoredEntity not found.");
            sponsoredEntity.NextRecalcDue = DateTime.Now;
        }

        #endregion
    }
}