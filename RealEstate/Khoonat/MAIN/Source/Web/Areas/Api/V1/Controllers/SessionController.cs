using System;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util.Text;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Domain.Audit;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Web.Application.Security;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Application;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Session;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Controllers
{
    public class SessionController : ApiControllerBase
    {
        [ComponentPlug]
        public IAuthenticationService AuthenticationService { get; set; }

        [ComponentPlug]
        public IHttpSessionCache HttpSessionCache { get; set; }

        [HttpPost]
        public ActionResult Start(ApiSessionStartInputModel input)
        {
            var output = new ApiSessionStartOutputModel();

            var visitor = AuthenticationService.LookupVisitor(input.UniqueVisitorIdentifier) ??
                          AuthenticationService.CreateVisitor();

            output.UniqueVisitorIdentifier = visitor.UniqueIdentifier;
            output.HttpSessionIdentifier = $"{Base32Url.ToBase32String(Guid.NewGuid().ToByteArray()).ToLower()}/apiv1";

            var initialSessionData = new HttpSession
            {
                Type = SessionType.Api,
                HttpSessionID = output.HttpSessionIdentifier,
                UserAgent = input.UserAgent,
                StartupUri = input.DeviceId, // URGH. Where do I record DeviceID?
                ReferrerUri = input.Referer,
                ClientAddress = Request.UserHostAddress,
                UniqueVisitorID = visitor.ID
            };

            var principal = ApiCallContext.Current.EndUser;
            if (principal?.CoreIdentity != null && principal.CoreIdentity.IsAuthenticated)
                initialSessionData.UserID = principal.CoreIdentity.UserId;

            AuthenticationService.StartSession(initialSessionData);
            return Json(output);
        }

        [HttpPost]
        public ActionResult End(ApiEmptyInputModel input)
        {
            if (ApiCallContext.Current.Session == null ||
                !ApiCallContext.Current.Session.IsValidSession ||
                ApiCallContext.Current.Session.Record == null ||
                ApiCallContext.Current.Session.Record.End.HasValue)
            {
                return Error(ApiErrorCode.SessionIdIsInvalid);
            }

            AuthenticationService.EndSession(ApiCallContext.Current.Session, SessionEndReason.Logout);
            HttpSessionCache.InvalidateItem(ApiCallContext.Current.Session.Record.HttpSessionID);
            return Success();
        }

    }
}