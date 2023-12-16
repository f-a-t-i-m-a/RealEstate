using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows.Forms;
using JahanJooy.RealEstateAgency.Domain.MasterData;
using JahanJooy.RealEstateAgency.Util.Models.Report;
using JahanJooy.RealEstateAgency.Util.Report;
using ServiceStack;
using Stimulsoft.Report;
using Stimulsoft.Report.Design;

// ReSharper disable LocalizableElement

namespace JahanJooy.RealEstateAgency.ReportDesigner
{
    public class ReportDesigner
    {
        private readonly ReportDesignerParams _parameters;
        private ReportTemplateSummary _template;
        private StiReport _report;
        private readonly List<ReportTemplateParameter> _templateParameters;

        private static ReportDesigner _current;

        static ReportDesigner()
        {
            StiDesigner.SavingReport += StiDesignerOnSavingReport;
            StiDesigner.DontAskSaveReport = false;
        }

        private static void StiDesignerOnSavingReport(object sender, StiSavingObjectEventArgs ea)
        {
            _current?.Save();
        }

        public ReportDesigner(ReportDesignerParams parameters, ReportTemplateSummary template,
            List<ReportTemplateParameter> templateParameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            _parameters = parameters;
            _template = template;
            _templateParameters = templateParameters;
        }

        public void Run(IWin32Window parent)
        {
            if (_template != null)
            {
                _report = new StiReport();
                if (_template.Definition != null)
                {
                    _report.Load(_template.Definition);
                    _report.ReportName = _template.Name;
                }

                Program.Composer.GetComponent<GeneralReport>()
                    .PopulateVariables(_report, _templateParameters, _parameters.ReportParameters);
                Program.Composer.GetComponent<ReportRepository>().CanonicalizeReport(_report);
            }
            else
            {
                _report = Program.Composer.GetComponent<ReportRepository>().NewEmptyReport();
            }

            if (_template != null)
//                ReportDictionaryFiller.Fill(_template.ID.ToString(), _parameters);
            ReportDictionaryFiller.Fill(_report, _parameters);

            try
            {
                _current = this;
                var designer = new StiDesigner(_report);
                designer.ShowDialogDesigner(parent);
            }
            finally
            {
                _current = null;
            }
        }

        public void Save()
        {
            if (_template == null)
            {
                SaveAs();
                return;
            }

            if (MessageBox.Show("آیا می خواهید قالب گزارش را تغییر دهید؟ قالب قبلی از دست خواهد رفت.",
                "ذخیره قالب گزارش", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            var template = new ReportTemplateSummary
            {
                ID = _template.ID,
                Name = _report.ReportName,
                Key = string.Empty,
                Description = string.Empty,
                Order = DateTime.Now.Ticks
            };

            var ms = new MemoryStream();
            _report.Save(ms);
            template.Definition = ms.ToArray();

            var content = new StringContent(template.ToJson(), Encoding.UTF8, "application/json");
            var response = SendPostRequest(content, "/report/update");
            if (response.IsSuccessStatusCode)
            {
                _template = template;
                MessageBox.Show("Report template saved to the database.");
            }
            else
            {
                MessageBox.Show("Error!");
            }
        }

        public void SaveAs()
        {
            var template = new ReportTemplateSummary
            {
                DataSourceType = _parameters.DataSourceType,
                ApplicationImplementedDataSourceType = _parameters.ApplicationImplementedDataSourceType,
                Name = _report.ReportName,
                Key = string.Empty,
                Description = string.Empty,
                Order = DateTime.Now.Ticks
            };

            var propForm = new TemplatePropertiesForm(template) {Text = "ذخیره قالب گزارش"};
            if (propForm.ShowDialog() != DialogResult.OK)
                return;

            _report.ReportName = template.Name;
            using (var ms = new MemoryStream())
            {
                _report.Save(ms);
                template.Definition = ms.ToArray();
            }
            var content = new StringContent(template.ToJson(), Encoding.UTF8, "application/json");
            var response = SendPostRequest(content, "/report/save");
            if (response.IsSuccessStatusCode)
            {
                _template = template;
                MessageBox.Show("Report template saved to the database.");
            }
            else
            {
                MessageBox.Show("Error!");
            }
        }

        private HttpResponseMessage SendPostRequest(StringContent content, string url)
        {
            HttpClient client = new HttpClient {BaseAddress = new Uri(LoginForm.BaseAddress)};
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LoginForm.OathToken);
            return client.PostAsync(LoginForm.UrlPrefix + url, content).Result;
        }
    }
}