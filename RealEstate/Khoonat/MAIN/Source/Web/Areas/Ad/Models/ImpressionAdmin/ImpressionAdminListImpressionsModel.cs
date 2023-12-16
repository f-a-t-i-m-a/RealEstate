using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Domain.Ad;

namespace JahanJooy.RealEstate.Web.Areas.Ad.Models.ImpressionAdmin
{
    public class ImpressionAdminListImpressionsModel
    {
        public PagedList<SponsoredEntityImpression> Impressions { get; set; }
        public string Page { get; set; }

        public long? SponsoredEntityIDFilter { get; set; }
    }
}