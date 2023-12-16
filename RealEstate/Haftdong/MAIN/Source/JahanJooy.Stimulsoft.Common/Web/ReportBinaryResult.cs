using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Stimulsoft.Report;

namespace JahanJooy.Stimulsoft.Common.Web
{
    public class ReportBinaryResult : IHttpActionResult
    {
        private readonly StiExportFormat _format;
        private readonly bool _rendered;
        private readonly StiReport _report;

        public ReportBinaryResult(StiReport report, StiExportFormat format = StiExportFormat.Pdf, bool rendered = false)
        {
            if (report == null)
                throw new ArgumentNullException(nameof(report));

            _report = report;
            _format = format;
            _rendered = rendered;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                if (!_rendered)
                    _report.Render();

                var response = new HttpResponseMessage(HttpStatusCode.OK);
                using (var ms = new MemoryStream())
                {
                    _report.ExportDocument(_format, ms);
                    response.Content = new ByteArrayContent(ms.ToArray());
                }

                response.Content.Headers.ContentType = new MediaTypeHeaderValue(GetContentType());
                return response;
            }, cancellationToken);
        }

        private string GetContentType()
        {
            switch (_format)
            {
                case StiExportFormat.Pdf:
                    return "application/pdf";
                case StiExportFormat.Xps:
                    return "application/vnd.ms-xpsdocument";

                case StiExportFormat.HtmlTable:
                case StiExportFormat.HtmlSpan:
                case StiExportFormat.HtmlDiv:
                case StiExportFormat.Html:
                case StiExportFormat.Html5:
                    return "text/html";
                case StiExportFormat.Mht:
                    return "message/rfc822";

                case StiExportFormat.Rtf:
                case StiExportFormat.RtfTable:
                case StiExportFormat.RtfFrame:
                case StiExportFormat.RtfWinWord:
                case StiExportFormat.RtfTabbedText:
                    return "text/rtf";
                case StiExportFormat.Text:
                    return "text/plain";

                case StiExportFormat.Excel:
                    return "application/vnd.ms-excel";
                case StiExportFormat.ExcelXml:
                case StiExportFormat.Excel2007:
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case StiExportFormat.Word2007:
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case StiExportFormat.Ppt2007:
                    return "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                case StiExportFormat.Ods:
                    return "application/vnd.oasis.opendocument.spreadsheet";
                case StiExportFormat.Odt:
                    return "application/vnd.oasis.opendocument.text";
                case StiExportFormat.Xml:
                    return "text/xml";
                case StiExportFormat.Csv:
                    return "text/csv";

                case StiExportFormat.Image:
                    return "image";
                case StiExportFormat.ImageGif:
                    return "image/gif";
                case StiExportFormat.ImageBmp:
                    return "image/bmp";
                case StiExportFormat.ImagePng:
                    return "image/png";
                case StiExportFormat.ImageTiff:
                    return "image/tiff";
                case StiExportFormat.ImageJpeg:
                    return "image/jpeg";
                case StiExportFormat.ImageSvg:
                    return "image/svg+xml";

                case StiExportFormat.Data:
                case StiExportFormat.Document:
                case StiExportFormat.Dif:
                case StiExportFormat.Sylk:
                case StiExportFormat.ImagePcx:
                case StiExportFormat.ImageEmf:
                case StiExportFormat.ImageSvgz:
                case StiExportFormat.Dbf:
                    return "application/octet-stream";
                default:
                    return "application/octet-stream";
            }
        }
    }
}