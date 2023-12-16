using JahanJooy.Common.Util;
using JahanJooy.Common.Util.Owin;
using Microsoft.Owin;

namespace JahanJooy.RealEstateAgency.Api.App.Utils
{
    public static class MobileSessionIdUtils
    {
        public const string OwinSessionKey = "jj.mobile.SessionId";
        public const string HeaderName = "X-JJ-SessionId";

        public static void SetMobileSessionId(this IOwinContext context, string sessionId)
        {
            context.Set(OwinSessionKey, sessionId);
        }

        public static string GetMobileSessionId(this IOwinContext context)
        {
            return context.Get<string>(OwinSessionKey);
        }

        public static string GetMobileSessionId(this OwinRequestScopeContext context)
        {
            return context.OwinContext.Get<string>(OwinSessionKey);
        }

        public static string Current
        {
            get
            {
                return OwinRequestScopeContext.Current.IfNotNull(c => c.GetMobileSessionId());
            }
        }

        public static void SetFromContext(IOwinContext context)
        {
            if (!context.Request.Headers.Keys.Contains(HeaderName))
                return;

            var sessionId = context.Request.Headers[HeaderName];
            context.SetMobileSessionId(sessionId);
        }
    }

}