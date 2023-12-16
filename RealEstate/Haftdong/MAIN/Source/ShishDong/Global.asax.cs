using System.Web;
using log4net;

namespace JahanJooy.RealEstateAgency.ShishDong
{
    public class WebApiApplication : HttpApplication
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(WebApiApplication));

        #region Application events

        protected void Application_Start()
        {
        }

        protected void Application_End()
        {
            Log.Info("Application stopped.");
        }

        protected void Application_Error()
        {
        }

        #endregion

        #region Session events

        protected void Session_Start()
        {
        }

        protected void Session_End()
        {
        }

        #endregion

        #region Request events

        protected void Application_BeginRequest()
        {
        }

        protected void Application_AuthenticateRequest()
        {
        }

        protected void Application_PostAuthenticateRequest()
        {
        }

        protected void Application_AuthorizeRequest()
        {
        }

        protected void Application_EndRequest()
        {
        }

        protected void Application_PostAcquireRequestState()
        {
        }

        protected void Application_PreRequestHandlerExecute()
        {
        }

        protected void Application_PostReleaseRequestState()
        {
        }

        protected void Application_ReleaseRequestState()
        {
        }

        #endregion
    }
}
