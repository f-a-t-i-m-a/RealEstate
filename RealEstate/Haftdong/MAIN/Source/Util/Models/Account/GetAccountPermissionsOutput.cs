using System.Collections.Generic;
using JahanJooy.RealEstateAgency.Domain.Enums.User;
using JahanJooy.RealEstateAgency.Util.Models.Users;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Account
{
    [TsClass]
    public class GetAccountPermissionsOutput
    {
        public ApplicationUserSummary User { get; set; }
        public List<BuiltInRole> Roles { get; set; }
        public List<string> RoleNames { get; set; }
    }
}