using System;
using System.Threading.Tasks;
using System.Web.Http;
using Compositional.Composer;
using JahanJooy.Common.Util.General;
using JahanJooy.Common.Util.Identity;
using JahanJooy.Common.Util.Owin;
using JahanJooy.Common.Util.Text;
using JahanJooy.Common.Util.Web;
using JahanJooy.RealEstateAgency.Api.App.Base;
using JahanJooy.RealEstateAgency.Api.App.Models.Session;
using JahanJooy.RealEstateAgency.Api.App.Utils;
using JahanJooy.RealEstateAgency.Domain.Audit;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Log4Net;
using JahanJooy.RealEstateAgency.Util.UserActivityLog;
using MongoDB.Bson;
using ServiceStack;
using ServiceStack.Text;

namespace JahanJooy.RealEstateAgency.Api.App.Controllers
{
    [Contract]
    [Component]
    [ComponentCache(null)]
    [RoutePrefix("session")]
    public class AppSessionController : AppExtendedApiController
    {
        private const int MaxCrashReportLength = 20480;

        #region Injected dependencies

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        #endregion

        #region Action methods

        [HttpPost, Route("start")]
        [SkipUserActivityLogging]
        public IHttpActionResult StartSession(StartSessionInput input)
        {
            var sessionId = input.SessionID.IsNullOrEmpty() ? Guid.NewGuid().ToUrlFriendly() : input.SessionID;
            var correlationId = OwinRequestScopeContext.Current.GetCorrelationId() ?? "----------------------";
            var userAgentFullName = OwinRequestScopeContext.Current.GetMobileUserAgentFullName() ?? "----------------------";
            var userId = OwinRequestScopeContext.Current.GetUserId();

            var mobileSession = new MobileSession
            {
                SessionID = sessionId,
                PhoneOperator = input.PhoneOperator,
                PhoneSubscriberID = input.PhoneSubscriberId,
                PhoneSerialNumber = input.PhoneSerialNumber,
                UserAgentString = userAgentFullName,
                UserId = ObjectId.Parse(userId),
                CreationTime = DateTime.Now
            };

            DbManager.MobileSession.InsertOneAsync(mobileSession);

            ApplicationStaticLogs.MobileAccessLog.InfoFormat("({0}) - SID {1} - OP {2} - S/N {3} - SUB {4} - UA {5}",
                correlationId, sessionId, input.PhoneOperator, input.PhoneSerialNumber, input.PhoneSubscriberId,
                userAgentFullName);

            return Ok(new JsonObject());
        }

        [HttpPost, Route("crash")]
        [AllowAnonymous]
        [SkipUserActivityLogging]
        public async Task<IHttpActionResult> CrashReport()
        {
            var payload = await Request.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(payload))
                return NotFound();

            ApplicationStaticLogs.MobileCrashLog.Error(payload.Truncate(MaxCrashReportLength));
            return Ok(new JsonObject());
        }

        #endregion

    }
}