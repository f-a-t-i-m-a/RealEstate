using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Domain.Billing;
using JahanJooy.RealEstate.Web.Application.Authorization;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Areas.Billing.Models.BillingAdminHome;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Controllers
{
    [OperatorOnly]
    [RequireHttps]
    public class BillingAdminHomeController : CustomControllerBase
    {
        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IComposer Composer { get; set; }

        [HttpGet]
        public ActionResult Index()
        {
            var model = CalculateStatistics();
            return View(model);
        }

        [ChildActionOnly]
        public ActionResult CategoryPartial()
        {
            var model = CalculateStatistics();
            return PartialView(model);
        }

        private BillingAdminHomeStatisticsModel CalculateStatistics()
        {
            var result = new BillingAdminHomeStatisticsModel
                         {
                             AdministrativeChangesQueueLength =
                                 DbManager.Db.UserBalanceAdministrativeChanges.Count(c => c.BillingState == BillingSourceEntityState.Pending),

                             WireTransferPaymentsQueueLength =
                                 DbManager.Db.UserWireTransferPayments.Count(p => p.BillingState == BillingSourceEntityState.Pending),

//                             ElectronicPaymentsQueueLenth =
//                                 DbManager.Db.UserElectronicPayments.Count(p => !p.ReviewedByUserID.HasValue),

                             RefundRequestQueueLength =
                                 DbManager.Db.UserRefundRequests.Count(
                                     r => !r.ReviewedByUserID.HasValue || (r.BillingState == BillingSourceEntityState.Applied && !r.ClearedByUserID.HasValue))
                         };

            return result;
        }
    }
}