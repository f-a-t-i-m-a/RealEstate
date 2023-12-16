using System;
using System.Collections.Generic;
using System.Linq;
using JahanJooy.RealEstateAgency.Domain.MasterData;

namespace JahanJooy.RealEstateAgency.ReportDesigner.Model
{
    public class DataSourceTypeListItem
    {
        public ReportDataSourceType Value { get; set; }
        public string DisplayText { get; set; }

        public static IEnumerable<DataSourceTypeListItem> GetAll()
        {
            return
                Enum.GetValues(typeof (ReportDataSourceType))
                    .Cast<ReportDataSourceType>()
                    .Select(v => new DataSourceTypeListItem {Value = v, DisplayText = v.ToString()});
        }
    }
}