using System;
using System.Collections.Generic;
using System.Linq;
using JahanJooy.RealEstateAgency.Domain.MasterData;

namespace JahanJooy.RealEstateAgency.ReportDesigner.Model
{
    public class ApplicationImplementedDataSourceListItem
    {
        public ApplicationImplementedReportDataSourceType Value { get; set; }
        public string DisplayText { get; set; }

        public static IEnumerable<ApplicationImplementedDataSourceListItem> GetAll()
        {
            return
                Enum.GetValues(typeof(ApplicationImplementedReportDataSourceType))
                    .Cast<ApplicationImplementedReportDataSourceType>()
                    .Select(v => new ApplicationImplementedDataSourceListItem { Value = v, DisplayText = v.ToString() });
        }
    }
}