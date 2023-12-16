using JahanJooy.Common.Util;
using JahanJooy.Common.Util.Owin;
using JahanJooy.Common.Util.Text;
using Microsoft.Owin;

namespace JahanJooy.RealEstateAgency.Api.App.Utils
{
    public static class MobileUserAgentUtils
    {
        public const string FullNameOwinSessionKey = "jj.mobile.UserAgent.FullName";
        public const string NameOwinSessionKey = "jj.mobile.UserAgent.Name";
        public const string VersionOwinSessionKey = "jj.mobile.UserAgent.Version";

        public const string HeaderName = "X-JJ-UserAgent";

        public static void SetMobileUserAgentFullName(this IOwinContext context, string fullName)
        {
            context.Set(FullNameOwinSessionKey, fullName);
        }

        public static void SetMobileUserAgentName(this IOwinContext context, string name)
        {
            context.Set(NameOwinSessionKey, name);
        }

        public static void SetMobileUserAgentVersion(this IOwinContext context, string version)
        {
            context.Set(VersionOwinSessionKey, version);
        }

        public static string GetMobileUserAgentFullName(this IOwinContext context)
        {
            return context.Get<string>(FullNameOwinSessionKey);
        }

        public static string GetMobileUserAgentName(this IOwinContext context)
        {
            return context.Get<string>(NameOwinSessionKey);
        }

        public static string GetMobileUserAgentVersion(this IOwinContext context)
        {
            return context.Get<string>(VersionOwinSessionKey);
        }

        public static string GetMobileUserAgentFullName(this OwinRequestScopeContext context)
        {
            return context.OwinContext.Get<string>(FullNameOwinSessionKey);
        }

        public static string GetMobileUserAgentName(this OwinRequestScopeContext context)
        {
            return context.OwinContext.Get<string>(NameOwinSessionKey);
        }

        public static string GetMobileUserAgentVersion(this OwinRequestScopeContext context)
        {
            return context.OwinContext.Get<string>(VersionOwinSessionKey);
        }

        public static string CurrentFullName
        {
            get
            {
                return OwinRequestScopeContext.Current.IfNotNull(c => c.GetMobileUserAgentFullName());
            }
        }

        public static string CurrentName
        {
            get
            {
                return OwinRequestScopeContext.Current.IfNotNull(c => c.GetMobileUserAgentName());
            }
        }
        public static string CurrentVersion
        {
            get
            {
                return OwinRequestScopeContext.Current.IfNotNull(c => c.GetMobileUserAgentVersion());
            }
        }

        public static void SetFromContext(IOwinContext context)
        {
            if (!context.Request.Headers.Keys.Contains(HeaderName))
                return;

            var fullName = context.Request.Headers[HeaderName];
            context.SetMobileUserAgentFullName(fullName);

            if (fullName.IsNullOrWhitespace())
                return;

            var fullNameSplit = fullName.Split(';');
            if (fullNameSplit.Length > 0 && !fullNameSplit[0].IsNullOrWhitespace())
            {
                context.SetMobileUserAgentName(fullNameSplit[0].Trim());
            }

            if (fullNameSplit.Length > 1 && !fullNameSplit[1].IsNullOrWhitespace())
            {
                context.SetMobileUserAgentVersion(fullNameSplit[1].Trim());
            }
        }
    }

}