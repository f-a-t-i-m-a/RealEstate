using System;
using System.Linq;
using System.Security.Authentication;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using Compositional.Composer;
using Compositional.Composer.Web;
using Elmah;
using FluentScheduler;
using JahanJooy.Common.Util.Components;
using JahanJooy.Common.Util.Serialization;
using JahanJooy.Common.Util.Web.Robots;
using JahanJooy.Common.Util.Web.Validation;
using JahanJooy.RealEstate.Core;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Security;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Core.Services.Dto;
using JahanJooy.RealEstate.Domain.Audit;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Web.Application;
using JahanJooy.RealEstate.Web.Application.Globalization;
using JahanJooy.RealEstate.Web.Application.Security;
using JahanJooy.RealEstate.Web.Application.Session;
using JahanJooy.RealEstate.Web.Application.Config;
using JahanJooy.RealEstate.Web.Controllers;
using log4net;

namespace JahanJooy.RealEstate.Web
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : HttpApplication
	{
		private const string TrackingCookieName = "JJUT";
		private static readonly ILog Log = LogManager.GetLogger(typeof(MvcApplication));

	    private static IComposer _componentContext;

		#region Application events

		protected void Application_Start()
		{
		    JsSerializationConfigUtil.InitializeDefaultConfiguration();
			AutoMapperConfig.ConfigureAllMappers();

			var logConfigError = Log4NetConfig.Configure(Server);
			if (!string.IsNullOrWhiteSpace(logConfigError))
				Log.Error("Error configuring Log4Net: " + Environment.NewLine + logConfigError);

			Log.Info("Application started.");

			MvcHandler.DisableMvcResponseHeader = true;

			AreaRegistration.RegisterAllAreas();

			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterAllRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			RegisterValidationComponents();

			ComposerWebUtil.Setup();
            ComposerWebUtil.ComponentContext.GetComponent<IAuthenticationService>().CleanupSessionsOnApplicationStartup();
		    _componentContext = ComposerWebUtil.ComponentContext;

            // Instantiate eager components
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            ComposerWebUtil.ComponentContext.GetAllComponents<IEagerComponent>().ToList();
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
			
			TaskManager.UnobservedTaskException += (sender, args) => Log.Error("Unobserved task exception occured in Scheduled Tasks", args.ExceptionObject as Exception);
			TaskManager.Initialize(ComposerWebUtil.ComponentContext.GetComponent<ScheduledTaskRegistry>());
		}

		protected void Application_End()
		{
		    var authenticationService = _componentContext?.GetComponent<IAuthenticationService>();
		    authenticationService?.CleanupSessionsOnApplicationShutdown();

		    Log.Info("Application stopped.");
		}

		protected void Application_Error()
		{
            var exception = Server.GetLastError();
            Log.Error("Unhandled exception in application", exception);

            // Ignore the error handling if the request is from local host, so that we can get errors locally on the development machines.
			if (!Context.Request.IsLocal)
			{
				var httpException = exception as HttpException;

				Response.Clear();

				// If we don't have any error codes, or the error code is not specified here, default to unknown error.
				var action = "Unknown";
				if (httpException != null)
				{
					switch (httpException.GetHttpCode())
					{
						case 400:
							action = "BadRequest";
							break;

						case 401:
							action = "Unauthorized";
							break;

						case 403:
							action = "Forbidden";
							break;

						case 404:
							action = "NotFound";
							break;

						case 500:
							action = "Internal";
							break;
					}
				}

				var routeData = new RouteData();
				routeData.Values.Add("controller", "Error");
				routeData.Values.Add("action", action);
				routeData.Values.Add("error", exception);

				Server.ClearError();
				Response.TrySkipIisCustomErrors = true;

				var controller = DependencyResolver.Current.GetService(typeof (ErrorController));
				var requestContext = new RequestContext(new HttpContextWrapper(Context), routeData);
				((IController)controller).Execute(requestContext);
			}
		}

		#endregion

		#region Session events

		protected void Session_Start()
		{
			if (CrawlerDetectorUtil.IsProbableBot(Request.UserAgent))
			{
				Session.SetSessionInfo(new SessionInfo { IsCrawler = true, OwnedProperties = null, Record = null });
			}
			else
			{
				var authenticationService = ComposerWebUtil.ComponentContext.GetComponent<IAuthenticationService>();
				var visitor = RetrieveUniqueVisitor();

				var initialSessionData = new HttpSession
					                         {
												 Type = SessionType.Web,
						                         HttpSessionID = Session.SessionID,
						                         UserAgent = Request.UserAgent,
						                         StartupUri = Request.Url.AbsoluteUri,
						                         ReferrerUri = Request.UrlReferrer?.AbsoluteUri ?? "",
						                         ClientAddress = Request.UserHostAddress,
						                         UniqueVisitorID = visitor.ID
					                         };

				var principal = HttpContext.Current.User as CorePrincipal;
				if (principal?.CoreIdentity != null && principal.CoreIdentity.IsAuthenticated)
					initialSessionData.UserID = principal.CoreIdentity.UserId;

				var prevSessionIdCookie = Request.Cookies[Common.Util.Web.Session.SessionStateExtensions.PrevSessionIdCookieName];
				if (prevSessionIdCookie != null)
				{
					initialSessionData.PrevHttpSessionID = prevSessionIdCookie.Value;

					var expiredPrevSessionIdCookie = new HttpCookie(Common.Util.Web.Session.SessionStateExtensions.PrevSessionIdCookieName, "");
					expiredPrevSessionIdCookie.Expires = DateTime.Now.AddDays(-1);
					Response.Cookies.Add(expiredPrevSessionIdCookie);
				}

				var sessionInfo = authenticationService.StartSession(initialSessionData);
				sessionInfo.OwnedProperties = OwnedPropertiesCookieUtil.Extract();

				Session.SetSessionInfo(sessionInfo);
			}
		}

		protected void Session_End()
		{
			var sessionInfo = Session.GetSessionInfo();
			if (sessionInfo?.Record != null && !sessionInfo.Record.End.HasValue)
			{
				using (var dbScope = ComposerWebUtil.GetComponentContext(Application).GetComponent<DbManager>().StartThreadBoundScope())
				{
					ComposerWebUtil.GetComponentContext(Application).GetComponent<IAuthenticationService>().EndSession(sessionInfo, SessionEndReason.Timeout);
					dbScope.SaveAllChanges();
				}
			}
		}

		#endregion

		#region Request events

		protected void Application_BeginRequest()
		{
			// To make sure the service context is not kept from a previous request,
			// clear it on the request entry.

			ServiceContext.Reset();
			GlobalizationUtil.SetCurrentCulture();

            if (Request.Url.AbsolutePath.ToLower().StartsWith("/enamadlogo"))
    			Response.Headers["X-Frame-Options"] = "SAMEORIGIN";
            else
                Response.Headers["X-Frame-Options"] = "DENY";

            Response.Headers.Remove("Server");
			Response.Headers.Remove("X-AspNet-Version");
			Response.Headers.Remove("X-AspNetMvc-Version");
		}

		protected void Application_AuthenticateRequest()
		{
			try
			{
				PerformRequestAuthentication(AuthCookieUtil.Extract());
			}
			catch (Exception e)
			{
                Log.Error("Exception while authenticating request", e);

				ErrorSignal.FromCurrentContext().Raise(new AuthenticationException("Exception occured during Application_AuthenticateRequest", e));
				PerformRequestAuthentication(null);
			}
			finally
			{
				if (!(HttpContext.Current.User is CorePrincipal))
				{
					HttpContext.Current.User = CorePrincipal.Anonymous;
				}
			}
		}

		protected void Application_PostAuthenticateRequest()
		{
		}

		protected void Application_AuthorizeRequest()
		{
		}

		protected void Application_EndRequest()
		{
			ComposerWebUtil.ComponentContext.GetComponent<DbManager>().EndRequest();
			ServiceContext.Reset();
		}

		protected void Application_PostAcquireRequestState()
		{
			// Prevent a disabled user to continue browsing in its active session
			
			// Note: can't use "Session" property of HttpApplication directly, because it will throw
			// an exception when the session is null.

			var principal = HttpContext.Current.User as CorePrincipal;
			var sessionInfo = HttpContext.Current.Session.GetSessionInfo();

			if (principal == null || !principal.IsEnabled)
			{
				FormsAuthentication.SignOut();

				if (sessionInfo != null)
					ComposerWebUtil.ComponentContext.GetComponent<IAuthenticationService>().EndSession(Session.GetSessionInfo(), SessionEndReason.UserDisabled);

				if (HttpContext.Current.Session != null)
					HttpContext.Current.Session.Abandon();

				var action = new UrlHelper(Request.RequestContext).Action("UserDisabled", "Error");
				if (action != null)
					Response.Redirect(action);
			}

			// If the user is authenticated but the session is not marked with the user ID
			// (like when the user has persisted the auth cookie but the session is started
			// with HTTP call, so it is not yet authenticated during the Session_Start)
			// mark the session as authenticated and add the User ID to it.

			if (principal != null && principal.CoreIdentity.IsAuthenticated && sessionInfo?.Record != null && !sessionInfo.Record.UserID.HasValue)
			{
				ComposerWebUtil.ComponentContext.GetComponent<IAuthenticationService>().MarkSessionAsAuthenticated(sessionInfo.Record.ID, principal.CoreIdentity.UserId.GetValueOrDefault());
				sessionInfo.Record.UserID = principal.CoreIdentity.UserId.GetValueOrDefault();
			}
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

		#region Elmah events

		protected void ErrorLog_Filtering(object sender, ExceptionFilterEventArgs e)
		{
		}

		protected void ErrorMail_Filtering(object sender, ExceptionFilterEventArgs e)
		{
			// Moved the filtering logic to Web.config.

			//if (e == null)
			//	return;
			//
			//if (e.Exception != null && e.Exception.GetBaseException() != null && e.Exception.GetBaseException() is FileNotFoundException)
			//	e.Dismiss();
		}

		#endregion

		#region Private helper methods

		private static void RegisterValidationComponents()
		{
			ExtendedModelBinder.Setup();
			LocalizedClientDataTypeModelValidatorProvider.Setup();
		}

		private void PerformRequestAuthentication(AuthCookieContents contents)
		{
			if (contents == null || !contents.IsValid)
			{
				HttpContext.Current.User = CorePrincipal.Anonymous;
				return;
			}

			var principal = ComposerWebUtil.ComponentContext.GetComponent<IPrincipalCache>()[contents.UserID];
			if (principal?.CoreIdentity == null || string.Compare(contents.LoginName, principal.CoreIdentity.LoginName, StringComparison.OrdinalIgnoreCase) != 0)
			{
				HttpContext.Current.User = CorePrincipal.Anonymous;
				return;
			}

			HttpContext.Current.User = principal;
		}

		private UniqueVisitor RetrieveUniqueVisitor()
		{
			var authenticationService = ComposerWebUtil.ComponentContext.GetComponent<IAuthenticationService>();

			HttpCookie trackingCookie = Request.Cookies[TrackingCookieName];
			UniqueVisitor currentVisitor = null;

			if (trackingCookie != null)
			{
				currentVisitor = authenticationService.LookupVisitor(trackingCookie.Value);
			}

			if (currentVisitor == null)
			{
				currentVisitor = authenticationService.CreateVisitor();
				trackingCookie = new HttpCookie(TrackingCookieName);
				trackingCookie.Expires = DateTime.Now.AddYears(20);
				trackingCookie.HttpOnly = true;
				trackingCookie.Shareable = false;
				trackingCookie.Value = currentVisitor.UniqueIdentifier;

				Response.Cookies.Add(trackingCookie);
			}

			return currentVisitor;
		}

		#endregion
	}
}