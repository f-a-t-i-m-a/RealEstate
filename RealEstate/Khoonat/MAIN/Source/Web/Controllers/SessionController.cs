using System;
using System.IO;
using System.Web.Mvc;
using JahanJooy.Common.Util.Text;
using JahanJooy.RealEstate.Util.Log4Net;
using JahanJooy.RealEstate.Web.Application.Base;

namespace JahanJooy.RealEstate.Web.Controllers
{
    public class SessionController : CustomControllerBase
    {
        private const int MaxCrashReportLength = 20480;

        [HttpPost]
        public ActionResult Start()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public ActionResult End()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public ActionResult Beat()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public ActionResult CrashReport()
        {
            var inStream = Request.InputStream;
            inStream.Seek(0, SeekOrigin.Begin);

            using (var reader = new StreamReader(inStream))
            {
                var body = reader.ReadToEnd();
                body = body.Substring(1, body.Length - 2);
                if (string.IsNullOrWhiteSpace(body))
                    return Error(ErrorResult.EntityNotFound);

                RealEstateStaticLogs.MobileCrashLog.Error(body.Truncate(MaxCrashReportLength));
            }

            return Content("");
        }
    }
}