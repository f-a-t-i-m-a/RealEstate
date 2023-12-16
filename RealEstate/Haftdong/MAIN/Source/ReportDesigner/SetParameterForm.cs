using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using JahanJooy.RealEstateAgency.Domain.MasterData;
using JahanJooy.RealEstateAgency.Util.Models.Report;

namespace JahanJooy.RealEstateAgency.ReportDesigner
{
    public partial class SetParameterForm : Form
    {
        private readonly ReportTemplateSummary _template;
        private readonly List<ReportTemplateParameter> _templateParams;
        public SetParameterForm(ReportTemplateSummary template, List<ReportTemplateParameter> templateParams, bool dynamicParamsSuport)
        {
            InitializeComponent();
            _template = template;
            _templateParams = templateParams;
            grpParams.Enabled = dynamicParamsSuport;
            if (templateParams != null)
            {
                GenerateParametersPanel(templateParams);
            }
        }

        private void GenerateParametersPanel(List<ReportTemplateParameter> templateParams)
        {
            templateParams.ForEach(rp =>
            {
                var panel = new Panel();
                flowLayoutPanel1.Controls.Add(panel);

                var textBox = new TextBox();
                var label = new Label();

                // Panel
                panel.Controls.Add(textBox);
                panel.Controls.Add(label);
                panel.Location = new Point(22, 3);
                panel.Name = rp.ParameterName;
                panel.Size = new Size(273, 27);
                panel.TabIndex = 0;

                // Label
                label.AutoSize = true;
                label.Location = new Point(202, 6);
                label.Name = "lbl" + rp.ParameterName;
                label.Size = new Size(35, 13);
                label.TabIndex = 0;
                label.Text = rp.DisplayText;

                // TextBox
                textBox.Location = new Point(18, 3);
                textBox.Name = "txt" + rp.ParameterName;
                textBox.Size = new Size(180, 20);
                textBox.TabIndex = 1;
                textBox.Tag = rp.ParameterName;
                textBox.Text = rp.DefaultValue;

                panel.ResumeLayout(false);
                panel.PerformLayout();

            });
        }

        private void btnOK_Click(object sender, EventArgs e)
        {  
        }

        public ParametersOutput ExtractParametersValue()
        {
            var parameters = new List<ParameterValue>();
            var panelControles = flowLayoutPanel1.Controls.OfType<Panel>().Cast<Control>().ToList();

            _templateParams?.ForEach(param =>
            {
                var panelControl = panelControles.Find(control => control.Name == param.ParameterName);
                var inputControl =
                    panelControl.Controls.Cast<Control>()
                        .ToList()
                        .First(ctrl => ctrl.Tag.ToString() == param.ParameterName);
                var parameter = new ParameterValue(param.ParameterName, param.ParameterType,
                    inputControl.Text.ToString());
                parameters.Add(parameter);
            });

            return new ParametersOutput(parameters);
        }
    }
}
