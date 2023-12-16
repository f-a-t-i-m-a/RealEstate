using System.ComponentModel.DataAnnotations;
using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Domain.Ad;
using JahanJooy.RealEstate.Domain.Enums;

namespace JahanJooy.RealEstate.Web.Areas.Ad.Models.SponsoredPropertyAdmin
{
    public class SponsoredPropertyAdminListModel
    {
        public PagedList<SponsoredPropertyAdminListItemModel> SponsoredProperties { get; set; }
        public string Page { get; set; }

        [Display(ResourceType = typeof(SponsoredPropertyAdminModelResources), Name = "Label_BilledUserIDFilter")]
        public long? BilledUserIDFilter { get; set; }
        public SponsoredEntityApprovalStatus? ApprovalStatusFilter { get; set; }
        public SponsoredEntityBillingMethod? BillingMethodTypeFilter { get; set; }
        public long? PropertyCodeFilter { get; set; }
        public PropertyType? PropertyTypeFilter { get; set; }
        public string TitleFilter { get; set; }
        public bool? BlockedForLowCreditFilter { get; set; }

        public enum SponsoredEntityApprovalStatus
        {
            NotApproved,
            Approved,
            Rejected
        }

    }
}