using System.Collections.Generic;
using JahanJooy.Common.Util.Tarrif;

namespace JahanJooy.RealEstate.Core.Services.Dto.Billing
{
    public class Tarrif
    {
        public decimal SavedPropertySearchSmsNotificationBase { get; set; }

        public decimal PromotionalSmsBase { get; set; }
        public decimal PromotionalSmsAdditionalPart { get; set; }

        public decimal NewAccountVerificationPromotionalBonus { get; set; }

        public SponsorshipTarrif PropertyListingSponsorshipTarrif { get; set; }

        public List<SteppingTarrifRow> UserPaymentBonusCoefficients { get; set; }
    }
}