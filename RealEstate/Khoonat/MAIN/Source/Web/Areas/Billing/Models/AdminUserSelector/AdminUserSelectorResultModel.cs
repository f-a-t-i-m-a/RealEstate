using System.Collections.Generic;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Models.AdminUserSelector
{
    public class AdminUserSelectorResultModel
    {
        public bool More { get; set; }
        public List<AdminUserSelectorResultItemModel> Items { get; set; }
    }
}