using System;
using System.Windows.Forms;
using JahanJooy.RealEstateAgency.Domain.MasterData;

namespace JahanJooy.RealEstateAgency.ReportDesigner
{
    public partial class ParameterForm : Form
    {
        private readonly ReportTemplateParameter _templateParameter;
        public ParameterForm(ReportTemplateParameter templateParameter)
        {
            InitializeComponent();
            _templateParameter = templateParameter;
            cmbParameterType.DataSource = Enum.GetValues(typeof (ParameterType));

            txtParameterName.Text = _templateParameter.ParameterName;
            txtDisplayText.Text = _templateParameter.DisplayText;
            cmbParameterType.SelectedItem = _templateParameter.ParameterType;
            txtDefaultValue.Text = _templateParameter.DefaultValue;
            txtMinValue.Text = _templateParameter.Min;
            txtMaxValue.Text = _templateParameter.Max;
            chbRequired.Checked = _templateParameter.Required;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            _templateParameter.ParameterName = txtParameterName.Text;
            _templateParameter.DisplayText = txtDisplayText.Text;
            _templateParameter.ParameterType = (ParameterType) cmbParameterType.SelectedItem;
            _templateParameter.DefaultValue = txtDefaultValue.Text;
            _templateParameter.Min = txtMinValue.Text;
            _templateParameter.Max = txtMaxValue.Text;
            _templateParameter.Required = chbRequired.Checked;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
        }
    }
}
