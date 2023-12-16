using System;
using System.Web;
using JahanJooy.RealEstate.Util.Resources;

namespace JahanJooy.RealEstate.Util.Presentation
{
    public static class BillingPresentationHelper
    {
        public static HtmlString Amount(decimal? amount)
        {
            if (!amount.HasValue)
                return HtmlPresentationHelper.Disabled(GeneralResources.Unknown);

            return Amount(amount.Value);
        }

        public static HtmlString Amount(decimal amount)
        {
            var format = amount == 0m
                ? BillingPresentationResources.ZeroAmount
                : amount > 0m ? BillingPresentationResources.PositiveAmountFormat : BillingPresentationResources.NegativeAmountFormat;

            return new HtmlString("<span>" + HtmlPresentationHelper.NoBreaking(string.Format(format, Math.Abs(amount))).ToHtmlString() + "</span>");
        }

        public static HtmlString Delta(decimal? delta)
        {
            if (!delta.HasValue)
                return HtmlPresentationHelper.Disabled(GeneralResources.Unknown);

            return Delta(delta.Value);
        }

        public static HtmlString Delta(decimal delta)
        {
            var htmlClass = delta == 0m
                ? "billing-delta-zero"
                : delta > 0m ? "billing-delta-pos" : "billing-delta-neg";

            var format = delta == 0m
                ? BillingPresentationResources.ZeroDelta
                : delta > 0m ? BillingPresentationResources.PositiveDeltaFormat : BillingPresentationResources.NegativeDeltaFormat;

            return new HtmlString("<span class=\"" + htmlClass + "\">" + HtmlPresentationHelper.NoBreaking(string.Format(format, Math.Abs(delta))).ToHtmlString() + "</span>");
        }

        public static HtmlString Balance(decimal? balance)
        {
            if (!balance.HasValue)
                return HtmlPresentationHelper.Disabled(GeneralResources.Unknown);

            return Delta(balance.Value);
        }

        public static HtmlString Balance(decimal balance)
        {
            var htmlClass = balance == 0m
                ? "billing-balance-zero"
                : balance > 0m ? "billing-balance-pos" : "billing-balance-neg";

            var format = balance == 0m
                ? BillingPresentationResources.ZeroBalance
                : balance > 0m ? BillingPresentationResources.PositiveBalanceFormat : BillingPresentationResources.NegativeBalanceFormat;

            return new HtmlString("<span class=\"" + htmlClass + "\">" + HtmlPresentationHelper.NoBreaking(string.Format(format, Math.Abs(balance))) + "</span>");
        }

        public static HtmlString Turnover(decimal turnover)
        {
            var htmlClass = turnover == 0m
                ? "billing-turnover-zero"
                : "billing-turnover-pos";

            var format = turnover == 0m
                ? BillingPresentationResources.ZeroTurnover
                : BillingPresentationResources.PositiveTurnoverFormat;

            return new HtmlString("<span class=\"" + htmlClass + "\">" + HtmlPresentationHelper.NoBreaking(string.Format(format, turnover)) + "</span>");
        }
    }
}