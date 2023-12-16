using System.Web.Http;
using JahanJooy.RealEstateAgency.Util.UserActivityLog;
using Microsoft.Owin.Security.OAuth;

namespace JahanJooy.RealEstateAgency.HaftDong.Server.Config
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
            config.Filters.Add(new UserActivityLogFilter());

            // Web API routes
            config.MapHttpAttributeRoutes();
        }
    }
}
