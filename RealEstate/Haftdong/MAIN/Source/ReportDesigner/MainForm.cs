using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows.Forms;
using AutoMapper;
using Compositional.Composer;
using JahanJooy.Common.Util;
using JahanJooy.RealEstateAgency.Domain.MasterData;
using JahanJooy.RealEstateAgency.ReportDesigner.Model;
using JahanJooy.RealEstateAgency.Util.Models.Report;
using JahanJooy.RealEstateAgency.Util.Report;
using ServiceStack;
using Stimulsoft.Report;

// ReSharper disable LocalizableElement

namespace JahanJooy.RealEstateAgency.ReportDesigner
{
    public partial class MainForm : Form
    {
        private static readonly ApplicationImplementedReportDataSourceType[] AppImplSrcsSupported =
        {
            ApplicationImplementedReportDataSourceType.Properties,
            ApplicationImplementedReportDataSourceType.Requests,
            ApplicationImplementedReportDataSourceType.Supplies,
            ApplicationImplementedReportDataSourceType.Customers,
            ApplicationImplementedReportDataSourceType.Contracts,
            ApplicationImplementedReportDataSourceType.Users,
            ApplicationImplementedReportDataSourceType.Property,
            ApplicationImplementedReportDataSourceType.Request,
            ApplicationImplementedReportDataSourceType.Supply,
            ApplicationImplementedReportDataSourceType.Customer,
            ApplicationImplementedReportDataSourceType.Contract,
            ApplicationImplementedReportDataSourceType.User,
            ApplicationImplementedReportDataSourceType.Dashboard
        };

        private static readonly ApplicationImplementedReportDataSourceType[] AppImplSrcsWithParams =
        {
            ApplicationImplementedReportDataSourceType.Properties,
            ApplicationImplementedReportDataSourceType.Requests,
            ApplicationImplementedReportDataSourceType.Supplies,
            ApplicationImplementedReportDataSourceType.Customers,
            ApplicationImplementedReportDataSourceType.Contracts,
            ApplicationImplementedReportDataSourceType.Users,
            ApplicationImplementedReportDataSourceType.Property,
            ApplicationImplementedReportDataSourceType.Request,
            ApplicationImplementedReportDataSourceType.Supply,
            ApplicationImplementedReportDataSourceType.Customer,
            ApplicationImplementedReportDataSourceType.Contract,
            ApplicationImplementedReportDataSourceType.User,
            ApplicationImplementedReportDataSourceType.Dashboard
        };

        [ComponentPlug]
        public GeneralReport GeneralReport { get; set; }

        #region Initialization

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            lstDataSourceType.Items.AddRange(DataSourceTypeListItem.GetAll().Cast<object>().ToArray());
            RefreshDataSourceList();
            RefreshTemplateList();
            RefreshCommandStatus();
        }

        #endregion

        #region Populate lists and Enabled / Disabled status

        private void lstDataSourceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshDataSourceList();
            RefreshTemplateList();
            RefreshCommandStatus();
        }

        private void lstDataSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshTemplateList();
            RefreshCommandStatus();
        }

        private void lstReportTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectedSourceType.HasValue && SelectedSourceType.Value == ReportDataSourceType.DirectDbConnection)
                RefreshParameterList();
            RefreshCommandStatus();
        }

        private void RefreshDataSourceList()
        {
            lstDataSource.Items.Clear();
            lsbParameters.Items.Clear();

            var selectedType = SelectedSourceType;
            if (!selectedType.HasValue || selectedType.Value == ReportDataSourceType.DirectDbConnection)
            {
                lstDataSource.Enabled = false;
                lstDataSource.BackColor = Color.LightGray;
                return;
            }

            lstDataSource.Enabled = true;
            lstDataSource.BackColor = Color.White;
            lstDataSource.Items.AddRange(ApplicationImplementedDataSourceListItem.GetAll().Cast<object>().ToArray());
        }

        private void RefreshTemplateList()
        {
            var prevSelectedIndex = lstReportTemplate.SelectedIndex;
            lstReportTemplate.Items.Clear();

            if (!SelectedSourceType.HasValue || (
                SelectedSourceType.Value == ReportDataSourceType.ApplicationImplemented &&
                !SelectedAppImplSourceType.HasValue))
            {
                lstReportTemplate.Enabled = false;
                lstReportTemplate.BackColor = Color.LightGray;
                return;
            }

            lstReportTemplate.Enabled = true;
            lstReportTemplate.BackColor = Color.White;

            var searchInput = new SearchReportTemplateInput
            {
                DataSourceType = SelectedSourceType.Value,
                ApplicationImplementedDataSourceType = SelectedAppImplSourceType
            };

            var content = new StringContent(searchInput.ToJson(), Encoding.UTF8, "application/json");
            var response = SendPostRequest(content, "/report/search");
            if (response.IsSuccessStatusCode)
            {
                var body = response.Content.ReadAsStringAsync().Result;
                var output = body.FromJson<ReportListOutput>();
                lstReportTemplate.Items.AddRange(
                    output.Templates.Select(t => new TemplateListItem(t))
                        .Cast<object>()
                        .ToArray());

                if (lstReportTemplate.Items.Count > 0)
                    lstReportTemplate.SelectedIndex = Math.Min(prevSelectedIndex, lstReportTemplate.Items.Count - 1);
            }
            else
            {
                MessageBox.Show("Error!");
            }
        }

        private void RefreshCommandStatus()
        {
            bool templateListValid = SelectedSourceType.HasValue &&
                                     (SelectedSourceType.Value == ReportDataSourceType.DirectDbConnection ||
                                      SelectedAppImplSourceType.HasValue);
            bool templateSelected = templateListValid && lstReportTemplate.SelectedItem != null;

            bool supported = templateListValid && (
                SelectedSourceType.Value == ReportDataSourceType.DirectDbConnection ||
                SelectedAppImplSourceType.HasValue && AppImplSrcsSupported.Contains(SelectedAppImplSourceType.Value)
                );

            btnNewTemplate.Enabled = supported;
            btnEditTemplate.Enabled = supported && templateSelected;
            btnEditProperties.Enabled = supported && templateSelected;
            btnViewReport.Enabled = supported && templateSelected;
            btnDeleteTemplate.Enabled = supported && templateSelected;
            btnCopy.Enabled = supported && templateSelected;
            btnImpot.Enabled = supported;
            btnExport.Enabled = supported && templateSelected;
            grpParams.Enabled = DynamicParameters &&
                                (SelectedSourceType.HasValue &&
                                 SelectedSourceType.Value == ReportDataSourceType.DirectDbConnection);
        }

        private void RefreshParameterList()
        {
            lsbParameters.Items.Clear();

            var response = SendGetRequest(SelectedTemplate.ID.ToString(), "/report/get");
            if (response.IsSuccessStatusCode)
            {
                var body = response.Content.ReadAsStringAsync().Result;
                var template = body.FromJson<ReportTemplateSummary>();
                var parameters = template.Parameters;

                if (parameters != null)
                    lsbParameters.Items.AddRange(parameters.Cast<object>().ToArray());
                lsbParameters.DisplayMember = "ParameterName";
            }
            else
            {
                MessageBox.Show("Error!");
            }
        }

        #endregion

        private void btnNewTemplate_Click(object sender, EventArgs e)
        {
            var newTemplate = new NewReportTemplateInput
            {
                DataSourceType = SelectedSourceType ?? 0,
                ApplicationImplementedDataSourceType = SelectedAppImplSourceType
            };

            var template = Mapper.Map<ReportTemplateSummary>(newTemplate);
            var propForm = new TemplatePropertiesForm(template) {Text = "ایجاد گزارش جدید"};
            if (propForm.ShowDialog() != DialogResult.OK)
                return;

            newTemplate = Mapper.Map<NewReportTemplateInput>(template);
            var report = new StiReport {ReportName = newTemplate.Name};
            using (var ms = new MemoryStream())
            {
                report.Save(ms);
                newTemplate.Definition = ms.ToArray();
            }

            var content = new StringContent(newTemplate.ToJson(), Encoding.UTF8, "application/json");
            var response = SendPostRequest(content, "/report/save");
            if (response.IsSuccessStatusCode)
            {
                RefreshTemplateList();
                RefreshCommandStatus();
            }
            else
            {
                MessageBox.Show("Error!");
            }
        }

        private void btnEditTemplate_Click(object sender, EventArgs e)
        {
            var response = SendGetRequest(SelectedTemplate.ID.ToString(), "/report/get");
            if (response.IsSuccessStatusCode)
            {
                var body = response.Content.ReadAsStringAsync().Result;
                var template = body.FromJson<ReportTemplateSummary>();
                if (template != null)
                {
                    var templateParams = template.Parameters ?? new List<ReportTemplateParameter>();

                    ParametersOutput parameters;
                    if (templateParams.Count != 0)
                    {
                        var paramForm = new SetParameterForm(template, templateParams, DynamicParameters);
                        if (paramForm.ShowDialog() != DialogResult.OK)
                            return;

                        parameters = paramForm.ExtractParametersValue();
                    }
                    else
                    {
                        parameters = new ParametersOutput(new List<ParameterValue>());
                    }

                    var designer = new ReportDesigner(ParseDesignerParameters(parameters, txtId.Text), SelectedTemplate,
                        templateParams);
                    designer.Run(this);
                }

                RefreshTemplateList();
                RefreshCommandStatus();
            }
            else
            {
                MessageBox.Show("Error!");
            }
        }

        private void btnEditProperties_Click(object sender, EventArgs e)
        {
            var response = SendGetRequest(SelectedTemplate.ID.ToString(), "/report/get/");
            if (response.IsSuccessStatusCode)
            {
                var body = response.Content.ReadAsStringAsync().Result;
                var template = body.FromJson<ReportTemplateSummary>();

                var propForm = new TemplatePropertiesForm(template) {Text = "ویرایش مشخصات گزارش"};
                if (propForm.ShowDialog() != DialogResult.OK)
                    return;

                var content = new StringContent(template.ToJson(), Encoding.UTF8, "application/json");
                response = SendPostRequest(content, "/report/update");
                if (response.IsSuccessStatusCode)
                {
                    RefreshTemplateList();
                    RefreshCommandStatus();
                }
                else
                {
                    MessageBox.Show("Error!");
                }
            }
            else
            {
                MessageBox.Show("Error!");
            }
        }

        private void btnViewReport_Click(object sender, EventArgs e)
        {
            var response = SendGetRequest(SelectedTemplate.ID.ToString(), "/report/get");
            if (response.IsSuccessStatusCode)
            {
                var body = response.Content.ReadAsStringAsync().Result;
                var template = body.FromJson<ReportTemplateSummary>();
                var templateParams = template.Parameters;
                ParametersOutput parameters = null;

                if (templateParams != null && templateParams.Count != 0)
                {
                    var paramForm = new SetParameterForm(template, templateParams, DynamicParameters);
                    if (paramForm.ShowDialog() != DialogResult.OK)
                        return;
                    parameters = paramForm.ExtractParametersValue();
                }

                var report = Program.Composer.GetComponent<ReportRepository>().NewEmptyReport();
                if (SelectedTemplate.Definition != null)
                {
                    report.Load(SelectedTemplate.Definition);
                    if (parameters != null)
                    {
                        GeneralReport.PopulateVariables(report, templateParams, parameters);
                    }

//                    ReportDictionaryFiller.Fill(SelectedTemplate.ID.ToString(), ParseDesignerParameters(parameters, txtId.Text));
                    ReportDictionaryFiller.Fill(report, ParseDesignerParameters(parameters, txtId.Text));
                    report.Show();
                }
            }
            else
            {
                MessageBox.Show("Error!");
            }
        }

        private void btnDeleteTemplate_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("آیا مطمئنید که قالب گزارش حذف شود؟ این عمل برگشت پذیر نیست.", "اخطار",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2,
                MessageBoxOptions.RtlReading) == DialogResult.Yes)
            {
                var response = SendGetRequest(SelectedTemplate.ID.ToString(), "/report/delete");
                if (response.IsSuccessStatusCode)
                {
                    RefreshTemplateList();
                    RefreshCommandStatus();
                }
                else
                {
                    MessageBox.Show("Error!");
                }
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            var original = SelectedTemplate;
            var template = new ReportTemplateSummary
            {
                DataSourceType = SelectedSourceType.GetValueOrDefault(),
                ApplicationImplementedDataSourceType = SelectedAppImplSourceType,
                Name = original.Name,
                Key = original.Key,
                Description = original.Description,
                Order = original.Order
            };

            var propForm = new TemplatePropertiesForm(template) {Text = "کپی قالب گزارش"};
            if (propForm.ShowDialog() != DialogResult.OK)
                return;

            StiReport report = new StiReport();
            report.Load(SelectedTemplate.Definition);
            report.ReportName = template.Name;
            using (var ms = new MemoryStream())
            {
                report.Save(ms);
                template.Definition = ms.ToArray();
            }

            var content = new StringContent(template.ToJson(), Encoding.UTF8, "application/json");
            var response = SendPostRequest(content, "/report/save");
            if (response.IsSuccessStatusCode)
            {
                RefreshTemplateList();
                RefreshCommandStatus();
            }
            else
            {
                MessageBox.Show("Error!");
            }
        }

        private void btnImpot_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "mrt",
                Filter = "Stimulreport templates (*.mrt)|*.mrt|All files (*.*)|*.*",
                FilterIndex = 1
            };

            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            StiReport report = new StiReport();
            report.Load(dialog.FileName);

            // TODO: Check (or maybe rebuild) report's data sources

            var template = new ReportTemplateSummary
            {
                DataSourceType = SelectedSourceType.GetValueOrDefault(),
                ApplicationImplementedDataSourceType = SelectedAppImplSourceType,
                Name = report.ReportName,
                Key = string.Empty,
                Description = string.Empty,
                Order = DateTime.Now.Ticks
            };

            var propForm = new TemplatePropertiesForm(template) {Text = "مشخصات قالب ورودی"};
            if (propForm.ShowDialog() != DialogResult.OK)
                return;

            report.ReportName = template.Name;
            using (var ms = new MemoryStream())
            {
                report.Save(ms);
                template.Definition = ms.ToArray();
            }

            var content = new StringContent(template.ToJson(), Encoding.UTF8, "application/json");
            var response = SendPostRequest(content, "/report/save");
            if (response.IsSuccessStatusCode)
            {
                RefreshTemplateList();
                RefreshCommandStatus();
            }
            else
            {
                MessageBox.Show("Error!");
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                AddExtension = true,
                CheckPathExists = true,
                DefaultExt = "mrt",
                Filter = "Stimulreport templates (*.mrt)|*.mrt|All files (*.*)|*.*",
                FilterIndex = 1
            };

            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            var report = new StiReport();
            report.Load(SelectedTemplate.Definition);
            report.Save(dialog.FileName);
        }

        private ReportDesignerParams ParseDesignerParameters(ParametersOutput parameters, string id)
        {
            return new ReportDesignerParams
            {
                DataSourceType = SelectedSourceType.GetValueOrDefault(),
                ApplicationImplementedDataSourceType = SelectedAppImplSourceType,
                ReportParameters = parameters,
                ID = id
            };
        }

        private void btnAddParameter_Click(object sender, EventArgs e)
        {
            //TODO test beshe
            var response = SendGetRequest(SelectedTemplate.ID.ToString(), "/report/get");
            if (response.IsSuccessStatusCode)
            {
                var body = response.Content.ReadAsStringAsync().Result;
                var template = body.FromJson<ReportTemplateSummary>();
                if (template != null)
                {
                    var reportTemplateParameter = new ReportTemplateParameter();

                    var paramForm = new ParameterForm(reportTemplateParameter);
                    if (paramForm.ShowDialog() != DialogResult.OK)
                        return;

                    var input = new AddParameterInput
                    {
                        ID = SelectedTemplate.ID,
                        Parameter = reportTemplateParameter
                    };
                    var content = new StringContent(input.ToJson(), Encoding.UTF8, "application/json");
                    SendPostRequest(content, "/report/addparameter");
                }

                RefreshParameterList();
                RefreshTemplateList();
                RefreshCommandStatus();
            }
            else
            {
                MessageBox.Show("Error!");
            }
        }

        private void btnEditParameter_Click(object sender, EventArgs e)
        {
            //TODO test beshe
            if (SelectedTemplateParameter != null)
            {
                var response = SendGetRequest(SelectedTemplate.ID.ToString(), "/report/get");
//                var response = SendGetRequest(SelectedTemplateParameter.ID.ToString(), "/report/getByParameterId");
                if (response.IsSuccessStatusCode)
                {
                    var body = response.Content.ReadAsStringAsync().Result;
                    var template = body.FromJson<ReportTemplateSummary>();
                    var templateParameter =
                        template.Parameters.SingleOrDefault(p => p.ID == SelectedTemplateParameter.ID);
                    var index = template.Parameters.IndexOf(templateParameter);

                    if (templateParameter != null && index != -1)
                    {
                        var paramForm = new ParameterForm(templateParameter);
                        if (paramForm.ShowDialog() != DialogResult.OK)
                            return;

                        template.Parameters[index] = templateParameter;
                        var input = new SetParametersInput
                        {
                            ID = SelectedTemplate.ID,
                            Parameters = template.Parameters
                        };
                        var content = new StringContent(input.ToJson(), Encoding.UTF8, "application/json");
                        SendPostRequest(content, "/report/setparameters");
                    }

                    RefreshParameterList();
                    RefreshTemplateList();
                    RefreshCommandStatus();
                }
            }
            else
            {
                MessageBox.Show("Error!");
            }
        }

        private void btnRemoveParameter_Click(object sender, EventArgs e)
        {
            //TODO test beshe
            if (SelectedTemplateParameter != null)
            {
                if (MessageBox.Show("آیا مطمئنید که پارامتر گزارش حذف شود؟ این عمل برگشت پذیر نیست.", "اخطار",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2,
                    MessageBoxOptions.RtlReading) == DialogResult.Yes)
                {
                    var input = new RemoveParameterInput
                    {
                        ReportTemplateID = SelectedTemplate.ID,
                        ParameterID = SelectedTemplateParameter.ID
                    };
                    var content = new StringContent(input.ToJson(), Encoding.UTF8, "application/json");
                    var response = SendPostRequest(content, "/report/removeparameter");
                    if (response.IsSuccessStatusCode)
                    {
                        RefreshParameterList();
                        RefreshTemplateList();
                        RefreshCommandStatus();
                    }
                    else
                    {
                        MessageBox.Show("Error!");
                    }
                }
            }
        }

        #region Helper properties

        private ReportDataSourceType? SelectedSourceType
        {
            get
            {
                return (lstDataSourceType.SelectedItem as DataSourceTypeListItem)
                    .IfNotNull(i => (ReportDataSourceType?) i.Value);
            }
        }

        private ApplicationImplementedReportDataSourceType? SelectedAppImplSourceType
        {
            get
            {
                return (lstDataSource.SelectedItem as ApplicationImplementedDataSourceListItem)
                    .IfNotNull(i => (ApplicationImplementedReportDataSourceType?) i.Value);
            }
        }

        private ReportTemplateSummary SelectedTemplate
        {
            get { return (lstReportTemplate.SelectedItem as TemplateListItem).IfNotNull(i => i.Value); }
        }

        private ReportTemplateParameter SelectedTemplateParameter
            => (lsbParameters.SelectedItem as ReportTemplateParameter);

        private bool DynamicParameters => (lstReportTemplate.SelectedItem != null);

        private bool TestParameter
            => ((SelectedSourceType.HasValue && (SelectedSourceType.Value == ReportDataSourceType.DirectDbConnection ||
                                                 SelectedAppImplSourceType.HasValue)) &&
                (SelectedSourceType.Value == ReportDataSourceType.DirectDbConnection ||
                 SelectedAppImplSourceType.HasValue && AppImplSrcsSupported.Contains(SelectedAppImplSourceType.Value))
                && SelectedAppImplSourceType.HasValue && AppImplSrcsWithParams.Contains(SelectedAppImplSourceType.Value))
            ;

        #endregion

        private void lsbParameters_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshCommandStatus();
        }

        private HttpResponseMessage SendPostRequest(StringContent content, string url)
        {
            HttpClient client = new HttpClient {BaseAddress = new Uri(LoginForm.BaseAddress)};
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LoginForm.OathToken);
            return client.PostAsync(LoginForm.UrlPrefix + url, content).Result;
        }

        private HttpResponseMessage SendGetRequest(string id, string url)
        {
            HttpClient client = new HttpClient {BaseAddress = new Uri(LoginForm.BaseAddress)};
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LoginForm.OathToken);
            var extraSlash = !url.Last().Equals('/') ? "/" : "";
            return client.GetAsync(LoginForm.UrlPrefix + url + extraSlash + id).Result;
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();
            Hide();
            var loginForm = new LoginForm();
            loginForm.ShowDialog();
            Close();
        }
    }
}