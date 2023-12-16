using System;
using Compositional.Composer;
using JahanJooy.Common.Util.Components;
using JahanJooy.Common.Util.Configuration;
using JahanJooy.RealEstateAgency.Api.App.Utils;
using Microsoft.Owin;

namespace JahanJooy.RealEstateAgency.Api.App.Components
{
    [Contract]
    [Component]
    public class MobileAppVersionChecker : IEagerComponent
    {
        public const string SuggestedVersionKey = "UserAgent.App.Version.Suggested";
        public const string RequiredVersionKey = "UserAgent.App.Version.Required";

        private const string SuggestedVersionResponseHeaderKey = "X-JJ-Suggested-UserAgent-Version";
        private const string RequiredVersionResponseHeaderKey = "X-JJ-Required-UserAgent-Version";

        [ComponentPlug]
        public static IApplicationSettings ApplicationSettings { get; set; }

        static MobileAppVersionChecker()
        {
            ApplicationSettingKeys.RegisterKey(SuggestedVersionKey);
            ApplicationSettingKeys.RegisterKey(RequiredVersionKey);
        }

        public bool? CheckAppVersion(IOwinContext context)
        {
            Version currentVersion;
            if (!Version.TryParse(context.GetMobileUserAgentVersion(), out currentVersion))
                return null;

            Version suggestedVersion;
            if (Version.TryParse(ApplicationSettings[SuggestedVersionKey], out suggestedVersion))
            {
                if (currentVersion.CompareTo(suggestedVersion) < 0)
                {
                    context.Response.Headers.Set(SuggestedVersionResponseHeaderKey,
                        ApplicationSettings[SuggestedVersionKey]);
                }
            }

            Version requiredVersion;
            if (Version.TryParse(ApplicationSettings[RequiredVersionKey], out requiredVersion))
            {
                if (currentVersion.CompareTo(requiredVersion) < 0)
                {
                    context.Response.Headers.Set(RequiredVersionResponseHeaderKey,
                        ApplicationSettings[RequiredVersionKey]);

                    return false;
                }
            }

            if (suggestedVersion == null || requiredVersion == null)
                return null;

            return true;
        }
    }

}