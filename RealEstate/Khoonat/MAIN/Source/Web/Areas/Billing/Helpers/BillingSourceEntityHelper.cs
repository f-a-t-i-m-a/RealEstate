using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Helpers
{
    public static class BillingSourceEntityHelper
    {
        public static string StateBackgroundColor(IBillingSourceEntity entity)
        {
            switch (entity.BillingState)
            {
                case BillingSourceEntityState.Pending:
                    return "info";

                case BillingSourceEntityState.Applied:
                    return "success";

                case BillingSourceEntityState.Cancelled:
                    return "danger";

                case BillingSourceEntityState.Reversed:
                    return "warning";
            }

            return "active";
        }

        public static string StateBackgroundColor(UserRefundRequest entity)
        {
            if (entity.ReviewedByUserID != null && entity.BillingState == BillingSourceEntityState.Applied &&
                entity.ClearedByUserID == null)
            {
                return "success";
            }

            else if (entity.ReviewedByUserID == null && entity.BillingState == BillingSourceEntityState.Applied)
            {
                return "info";
            }
            else if (entity.ReviewedByUserID != null && entity.BillingState == BillingSourceEntityState.Reversed)
            {
                return "danger";
            }
            else if (entity.ReviewedByUserID != null && entity.BillingState == BillingSourceEntityState.Applied &&
                     entity.ClearedByUserID != null)
            {
                return "warning";
            }
            else
            {
                return "active";
            }
        }
    }
}