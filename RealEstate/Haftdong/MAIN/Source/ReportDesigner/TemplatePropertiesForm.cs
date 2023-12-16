using System;
using System.Windows.Forms;
using JahanJooy.RealEstateAgency.Util.Models.Report;

namespace JahanJooy.RealEstateAgency.ReportDesigner
{
    public partial class TemplatePropertiesForm : Form
    {
        private readonly ReportTemplateSummary _template;

        public TemplatePropertiesForm(ReportTemplateSummary template)
        {
            InitializeComponent();

            _template = template;

            txtDataSourceType.Text = _template.DataSourceType.ToString();
            txtApplicationImplementedDataSourceType.Text =
                _template.ApplicationImplementedDataSourceType?.ToString() ?? string.Empty;
            txtName.Text = _template.Name ?? string.Empty;
            txtKey.Text = _template.Key ?? string.Empty;
            txtDescription.Text = _template.Description ?? string.Empty;
            txtOrder.Text = _template.Order.ToString();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            _template.Name = txtName.Text;
            _template.Key = txtKey.Text;
            _template.Description = txtDescription.Text;
            _template.Order = long.Parse(txtOrder.Text);
        }
    }
}
