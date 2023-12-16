using System;
using System.Diagnostics;
using System.Reflection;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.ExceptionHandling;
using System.Web.Mvc;
using Compositional.Composer;
using Compositional.Composer.Factories;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.Composer;
using JahanJooy.Common.Util.Identity;
using JahanJooy.Common.Util.Owin;
using JahanJooy.Common.Util.Serialization;
using JahanJooy.Common.Util.Web;
using JahanJooy.Common.Util.Web.Services;
using JahanJooy.RealEstateAgency.HaftDong.Server.Globalization;
using JahanJooy.RealEstateAgency.HaftDong.Server.Providers;
using JahanJooy.RealEstateAgency.Naroon.Server.Logging;
using log4net;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Startup = JahanJooy.RealEstateAgency.Naroon.Server.Config.Startup;

[assembly: OwinStartup(typeof (Startup))]

namespace JahanJooy.RealEstateAgency.Naroon.Server.Config
{
    public class Startup
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (Startup));
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }
        protected IComposer Composer { get; set; }

        public void Configuration(IAppBuilder app)
        {
                ConfigureLog4Net();

            // After configuration of Log4Net, any startup exception can be logged by it.
            try
            {
                ConfigureCorrelationId(app);
                ConfigureRequestScopeContext(app);
                ConfigureUnhandledExceptions(app, GlobalConfiguration.Configuration);
                ConfigureAccessLog(app);

                ConfigureSerialization(app);
                ConfigureTypeMappings(app);

                ConfigureComposer(app);
                ConfigureHeaders(app);
//                ConfigureAuth(app);

                ConfigureRequestScopeContext(app);
                ConfigurePipelineAfterAuth(app);

                MapWebApi(app);

                ConfigureUnhandledExceptions2(app);
            }
            catch (Exception e)
            {
                Log.Error("Exception during application startup configuration", e);
                throw;
            }
        }

        protected virtual void ConfigureLog4Net()
        {
            if (!HostingEnvironment.IsHosted)
                return;

            var logConfigError = Log4NetConfig.Configure();
            if (!string.IsNullOrWhiteSpace(logConfigError))
                Log.Error("Error configuring Log4Net: " + Environment.NewLine + logConfigError);

            Log.Debug("Application started.");
        }

        protected virtual void ConfigureCorrelationId(IAppBuilder app)
        {
            app.Use(async (context, next) =>
            {
                Log.Debug("Configure Correlation ID starts");
                context.EnsureCorrelationId();
                await next.Invoke();
                Log.Debug("Configure Correlation ID ends");
            });
        }

        protected virtual void ConfigureUnhandledExceptions(IAppBuilder app, HttpConfiguration config)
        {
            config.Services.Replace(typeof (IExceptionLogger), new UnhandledExceptionLogger());

            app.Use(async (context, next) =>
            {
                Log.Debug("Configure Unhandled Exception starts");
                try
                {
                    await next.Invoke();
                }
                catch (Exception e)
                {
                    Log.Error("Unhandled exception in OWIN pipeline, not reported to ExceptionLogger", e);
                    UnhandledExceptionLoggingUtils.Log(e);
                    throw;
                }
                Log.Debug("Configure Unhandled Exception ends");
            });

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Log.Error("Unhandled exception in AppDomain");
                UnhandledExceptionLoggingUtils.Log(e.ExceptionObject as Exception);
            };
        }

        protected virtual void ConfigureUnhandledExceptions2(IAppBuilder app)
        {
            app.Use(async (context, next) =>
            {
                Log.Debug("Configure Unhandled Exception 2 starts");
                try
                {
                    await next.Invoke();
                }
                catch (Exception e)
                {
                    Log.Error("Unhandled exception in OWIN pipeline, not reported to ExceptionLogger", e);
                    UnhandledExceptionLoggingUtils.Log(e);
                    throw;
                }
                Log.Debug("Configure Unhandled Exception 2 ends");
            });
        }

        protected virtual void ConfigureAccessLog(IAppBuilder app)
        {
//            ApplicationStaticLogs.AccessLog.Info("================================ Access log re-configured");
            app.Use(async (context, next) =>
            {
                Log.Debug("Configure Access Log starts");
                try
                {
                    var requestTimer = Stopwatch.StartNew();

                    await next.Invoke();

                    var response = context.Response;
                    var contentLength = response.ContentLength;

                    var userId = context.Authentication.IfNotNull(
                        a => a.User.IfNotNull(
                            u => u.Identity.GetUserId(),
                            "UNULL"),
                        "ANULL");
                    userId = string.IsNullOrWhiteSpace(userId) ? "ANONY" : userId;

                    var statusCode = context.Response.IfNotNull(r => r.StatusCode.ToString(), "???");
                    var uri = context.Request.IfNotNull(
                        r => r.Uri.IfNotNull(
                            u => u.PathAndQuery,
                            "Null-URI"),
                        "Null-Request");
                    var method = context.Request.IfNotNull(r => r.Method, "MTHD?") ?? "MTHD?";
                    var correlationId = context.GetCorrelationId() ?? "----------------------";

                    requestTimer.Stop();
                    var requestTime = requestTimer.ElapsedMilliseconds;

//                    ApplicationStaticLogs.AccessLog.InfoFormat("({0}) [{1,5}] - {2,3} {3,4}ms {4,4}kb - {5,-5} {6}",
//                        correlationId, userId, statusCode, requestTime,
//                        contentLength.IfHasValue(cl => (cl/1024).ToString(), "????"), method, uri);
                }
                catch (Exception e)
                {
//                    ApplicationStaticLogs.AccessLog.Error(
//                        "Could not log API Invocation, because an exception occured in the OWIN pipeline: " + e.Message);
                    throw;
                }
                Log.Debug("Configure Access Log ends");
            });
        }

        protected virtual void ConfigureSerialization(IAppBuilder app)
        {
            Log.Debug("Configure Serialization starts");
            JsSerializationConfigUtil.InitializeDefaultConfiguration();
            FormatterConfig.Setup(GlobalConfiguration.Configuration);
            Log.Debug("Configure Serialization ends");
        }

        protected virtual void ConfigureTypeMappings(IAppBuilder app)
        {
            Log.Debug("Configure Type Mapping starts");
            AutoMapperConfig.ConfigureAllMappers();
            Log.Debug("Configure Type Mapping ends");
        }

        protected virtual void ConfigureComposer(IAppBuilder app)
        {
            Composer = CompositionConfig.Setup(GlobalConfiguration.Configuration);

            app.Use(async (context, next) =>
            {
                Log.Debug("Configure Composer starts");
                context.SetComposer(Composer);
                await next.Invoke();
                Log.Debug("Configure Composer ends");
            });
        }

        protected virtual void MapWebApi(IAppBuilder app)
        {
            app.Map("/api/naroon", apiApp =>
            {
                Log.Debug("Map Web API starts");
                var config = new HttpConfiguration();
                config.Services.Replace(typeof (IAssembliesResolver),
                    new LimitedAssembliesResolver(new[] {Assembly.GetExecutingAssembly()}));
                FormatterConfig.Setup(config);

                ConfigureUnhandledExceptions(apiApp, config);
                ConfigureWebApiPipeline(apiApp);
                ConfigureWebApiHeaders(apiApp);
                ConfigureWebApi(apiApp, config);
                Log.Debug("Map Web API ends");
            });
        }

        protected virtual void ConfigureWebApi(IAppBuilder app, HttpConfiguration config)
        {
            Log.Debug("Configure Web API starts");
            WebApiComposerWebUtil.Setup(Composer, config);
            WebApiConfig.Register(config);
            app.UseWebApi(config);
            Log.Debug("Configure Web API ends");
        }

        protected virtual void ConfigureRequestScopeContext(IAppBuilder app)
        {
            Log.Debug("Configure Request Scope Context starts");
            app.UseRequestScopeContext();
            Log.Debug("Configure Request Scope Context ends");
        }

        protected virtual void ConfigureWebApiRequestScopeContext(IAppBuilder app)
        {
            Log.Debug("Configure Web API Request Scope Context starts");
            app.UseRequestScopeContext();
            Log.Debug("Configure Web API Request Scope Context ends");
        }

        protected virtual void ConfigureHeaders(IAppBuilder app)
        {
            MvcHandler.DisableMvcResponseHeader = true;

            app.Use(async (context, next) =>
            {
                Log.Debug("Configure Headers starts");
                context.Response.Headers.Set("X-Frame-Options", "DENY");
                context.Response.Headers.Set("Server", null);

                await next.Invoke();
                Log.Debug("Configure Headers ends");
            });
        }

        protected virtual void ConfigureWebApiHeaders(IAppBuilder app)
        {
            MvcHandler.DisableMvcResponseHeader = true;

            app.Use(async (context, next) =>
            {
                Log.Debug("Configure Web API Headers starts");
                context.Response.Headers.Set("Cache-Control", "no-cache, no-store, must-revalidate");
                context.Response.Headers.Set("Pragma", "no-cache");
                context.Response.Headers.Set("Expires", "0");

                await next.Invoke();
                Log.Debug("Configure Web API Headers ends");
            });
        }

        protected virtual void ConfigureWebApiPipeline(IAppBuilder app)
        {
            app.Use(async (context, next) =>
            {
                // To make sure the service context is not kept from a previous request,
                // clear it on the request entry.
                Log.Debug("Configure Web API Pipeline starts");

                GlobalizationUtil.SetCurrentCulture();

                await next.Invoke();
                Log.Debug("Configure Web API Pipeline ends");
            });
        }

        protected virtual void ConfigureAuth(IAppBuilder app)
        {
            Log.Debug("Configure Auth starts");
            Composer.GetComponent<IComponentContext>().Register(
                typeof (IDataProtectionProviderContract),
                new PreInitializedComponentFactory(
                    new DataProtectionProviderComponent(app.GetDataProtectionProvider())));

            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/token"),
                Provider = Composer.GetComponent<ApplicationOAuthProvider>(),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                AllowInsecureHttp = true
            };

            app.UseOAuthBearerTokens(OAuthOptions);
            Log.Debug("Configure Auth ends");
        }

        protected virtual void ConfigurePipelineAfterAuth(IAppBuilder app)
        {
            app.Use(async (context, next) =>
            {
                Log.Debug("Configure Pipeline After Auth starts");
                await next.Invoke();
                Log.Debug("Configure Pipeline After Auth ends");
            });
        }
    }
}