using JahanJooy.RealEstateAgency.Util.Models.Report;

namespace JahanJooy.RealEstateAgency.ReportDesigner.Model
{
    public class TemplateListItem
    {
        public ReportTemplateSummary Value { get; private set; }
        public string DisplayText { get; private set; }

        public TemplateListItem(ReportTemplateSummary value)
        {
            Value = value;
            DisplayText = value.Name;
        }
    }
}