using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Domain.Ad;

namespace JahanJooy.RealEstate.Web.Areas.Ad.Models.ClickAdmin
{
    public class ClickAdminListClicksModel
    {
        public PagedList<SponsoredEntityClick> Clicks { get; set; }
        public string Page { get; set; }

        public long? SponsoredEntityIDFilter { get; set; }
    }
}