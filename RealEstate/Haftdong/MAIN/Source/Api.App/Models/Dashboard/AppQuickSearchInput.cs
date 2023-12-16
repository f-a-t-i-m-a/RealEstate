using System.Collections.Generic;
using AutoMapper;
using JahanJooy.Common.Util.AutoMapper;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using JahanJooy.RealEstateAgency.Util.Models.Dashboard;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Api.App.Models.Dashboard
{
    [TsClass]
    [AutoMapperConfig]
    public class AppQuickSearchInput
    {
        public string Text { get; set; }
        public int StartIndex { get; set; }
        public int PageSize { get; set; }
        public List<EntityType> DataTypes { get; set; }
        public DashboardSortColumn? SortColumn { get; set; }
        public SortDirectionType? SortDirection { get; set; }


        public static void ConfigureAutoMapper()
        {
            Mapper.CreateMap<AppQuickSearchInput, QuickSearchInput>()
                .IgnoreUnmappedProperties();
        }
    }
}
