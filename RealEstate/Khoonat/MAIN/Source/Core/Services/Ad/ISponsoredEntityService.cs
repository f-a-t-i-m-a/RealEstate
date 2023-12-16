using System;
using Compositional.Composer;
using JahanJooy.RealEstate.Domain.Ad;

namespace JahanJooy.RealEstate.Core.Services.Ad
{
    [Contract]
    public interface ISponsoredEntityService
    {
        void SetEnabled(long sponsoredEntityID, bool enabled);
        bool BillImpressions();
        bool FinalizePartialImpressionBillings();
        SponsoredEntityImpression CreateClick(long impressionId, Guid impressionGuid);
        bool BillClicks();
        bool FinalizePartialClickBillings();
        void SetNextRecalcDue(long sponsoredEntityID);
    }
}