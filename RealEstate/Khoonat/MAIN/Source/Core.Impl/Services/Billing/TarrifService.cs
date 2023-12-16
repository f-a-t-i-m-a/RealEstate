using System;
using System.Collections.Generic;
using Compositional.Composer;
using JahanJooy.Common.Util.Tarrif;
using JahanJooy.RealEstate.Core.Services.Billing;
using JahanJooy.RealEstate.Core.Services.Dto.Billing;

namespace JahanJooy.RealEstate.Core.Impl.Services.Billing
{
    [Component]
    public class TarrifService : ITarrifService
    {
        private readonly Tarrif _baseTarrif;

        public TarrifService()
        {
            _baseTarrif = new Tarrif
                          {
                              PromotionalSmsBase = 195m,
                              PromotionalSmsAdditionalPart = 115m,
                              
                              SavedPropertySearchSmsNotificationBase = 179m,

                              NewAccountVerificationPromotionalBonus = 20000m,

                              PropertyListingSponsorshipTarrif = new SponsorshipTarrif
                                                                 {
                                                                     PerImpressionMinimumBid = 8m,
                                                                     PerImpressionBidIncrement = 1m,
                                                                     PerClickMinimumBid = 289m,
                                                                     PerClickBidIncrement = 5m
                                                                 },

                              UserPaymentBonusCoefficients = new List<SteppingTarrifRow>
                                                             {
                                                                 new SteppingTarrifRow {MinimumInput = 10000000m, Result = 0.4m},
                                                                 new SteppingTarrifRow {MinimumInput = 5000000m, Result = 0.35m},
                                                                 new SteppingTarrifRow {MinimumInput = 4000000m, Result = 0.3m},
                                                                 new SteppingTarrifRow {MinimumInput = 3000000m, Result = 0.25m},
                                                                 new SteppingTarrifRow {MinimumInput = 2000000m, Result = 0.2m},
                                                                 new SteppingTarrifRow {MinimumInput = 1000000m, Result = 0.1m},
                                                                 new SteppingTarrifRow {MinimumInput = 0m, Result = 0m},
                                                             }
                          };
        }

        public Tarrif GetTarrif(long? userId, DateTime? asOf = null)
        {
            return _baseTarrif;
        }
    }
}