using System.Linq;
using JahanJooy.RealEstate.Domain;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Models.AdminUserSelector
{
    public class AdminUserSelectorResultItemModel
    {
        public long ID { get; set; }
        public long Code { get; set; }
        public string LoginName { get; set; }
        public string DisplayName { get; set; }

        public bool IsOperator { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsVerified { get; set; }

        // ReSharper disable ReturnTypeCanBeEnumerable.Global
        public static IQueryable<AdminUserSelectorResultItemModel> Map(IQueryable<User> query)
        {
            return query.Select(u => new AdminUserSelectorResultItemModel
                                     {
                                         ID = u.ID,
                                         Code = u.Code,
                                         LoginName = u.LoginName,
                                         DisplayName = u.DisplayName,
                                         IsOperator = u.IsOperator,
                                         IsEnabled = u.IsEnabled,
                                         IsVerified = u.IsVerified
                                     });
        }
        // ReSharper restore ReturnTypeCanBeEnumerable.Global

    }
}