using System;
using JahanJooy.RealEstateAgency.Domain.Enums.User;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Users
{
    [TsClass]
    public class SearchApplicationUserInput
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public UserType? Type { get; set; }
        public string ContactValues { get; set; }
        public bool? InActive { get; set; }
        public int StartIndex { get; set; }
        public int PageSize { get; set; }
        public ApplicationUserSortColumn? SortColumn { get; set; }
        public SortDirectionType? SortDirection { get; set; }
    }
}
