using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Web.Http.Controllers;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Routing;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstateAgency.Util.Log4Net;
using log4net;
using ServiceStack;
using ServiceStack.Text;
using HttpHeaders = System.Net.Http.Headers.HttpHeaders;

namespace JahanJooy.RealEstateAgency.HaftDong.Server.Logging
{
    public static class UnhandledExceptionLoggingUtils
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(UnhandledExceptionLoggingUtils));

        #region Public methods

        public static void Log(ExceptionLoggerContext context)
        {
            var message = context.IfNotNull(c => c.Exception.IfNotNull(e => e.Message));
            Logger.Error("Unhandled exception caught: " + message, context.Exception);

            try
            {
                var details = BuildExceptionDetailsObject(context);
                var detailsString = details.Dump();
                ApplicationStaticLogs.ErrorDetails.Info("Details of unhandled exception " + message + ": \r\n" +
                                                        detailsString);
            }
            catch (Exception e)
            {
                ApplicationStaticLogs.ErrorDetails.Error(
                    "Exception occured while generating details dump of exception " + message, e);
            }
        }

        public static void Log(Exception exception)
        {
            var message = exception.Message;
            Logger.Error("Unhandled exception caught: " + message, exception);

            try
            {
                var details = BuildExceptionDetailsObject(exception);
                var detailsString = details.Dump();
                ApplicationStaticLogs.ErrorDetails.Info("Details of unhandled exception " + message + ": \r\n" +
                                                        detailsString);
            }
            catch (Exception e)
            {
                ApplicationStaticLogs.ErrorDetails.Error(
                    "Exception occured while generating details dump of exception " + message, e);
            }
        }

        #endregion

        #region Private helpers

        private static Dictionary<string, object> BuildExceptionDetailsObject(ExceptionLoggerContext context)
        {
            if (context == null)
                return null;

            var result = new Dictionary<string, object>();

            AddToDetailsObject(result, "Request", () => MapRequest(context.ExceptionContext.Request));
            AddToDetailsObject(result, "RequestContext", () => MapRequestContext(context.ExceptionContext.RequestContext));
            AddToDetailsObject(result, "ControllerContext", () => MapControllerContext(context.ExceptionContext.ControllerContext));
            AddToDetailsObject(result, "ActionContext", () => MapActionContext(context.ExceptionContext.ActionContext));
            AddToDetailsObject(result, "Response", () => MapResponse(context.ExceptionContext.Response));
            AddToDetailsObject(result, "StackTrace", () => context.Exception.StackTrace);
            AddToDetailsObject(result, "Exception", () => MapException(context.Exception, 0));

            return result;
        }

        private static Dictionary<string, object> BuildExceptionDetailsObject(Exception exception)
        {
            if (exception == null)
                return null;

            var result = new Dictionary<string, object>();

            AddToDetailsObject(result, "Exception", () => MapException(exception, 0));

            return result;
        }

        private static object MapException(Exception e, int nestingLevel)
        {
            if (e == null)
                return "<NULL>";

            if (nestingLevel >= 8)
                return "<TRUNCATED>";

            var result = new Dictionary<string, object>();

            AddToDetailsObject(result, "Message", () => e.Message);
            AddToDetailsObject(result, "Data", () => MapExceptionData(e.Data));
            AddToDetailsObject(result, "Source", () => e.Source);
            AddToDetailsObject(result, "TargetSite", () => e.TargetSite.ToString());
            AddToDetailsObject(result, "InnerException", () => MapException(e.InnerException, nestingLevel + 1));

            var dbEntityValidationException = e as DbEntityValidationException;
            if (dbEntityValidationException != null)
                AddToDetailsObject(result, "EntityValidationErrors", () => MapEntityValidationErrors(dbEntityValidationException.EntityValidationErrors));

            return result;
        }

        private static object MapEntityValidationErrors(IEnumerable<DbEntityValidationResult> errors)
        {
            // ReSharper disable PossibleMultipleEnumeration
            // ReSharper disable AccessToForEachVariableInClosure

            if (!errors.SafeAny())
                return "<NULL>";

            var result = new List<Dictionary<string, object>>();
            foreach (var error in errors)
            {
                if (error.IsValid || !error.ValidationErrors.SafeAny())
                    continue;

                foreach (var validationError in error.ValidationErrors)
                {
                    var resultItem = new Dictionary<string, object>();
                    AddToDetailsObject(resultItem, "Entity", () => error.Entry.Entity.ToString());
                    AddToDetailsObject(resultItem, "EntityType", () => error.Entry.Entity.GetType().FullName);
                    AddToDetailsObject(resultItem, "State", () => error.Entry.State.ToString());
                    AddToDetailsObject(resultItem, "PropertyName", () => validationError.PropertyName);
                    AddToDetailsObject(resultItem, "ErrorMessage", () => validationError.ErrorMessage);

                    result.Add(resultItem);
                }
            }

            // ReSharper restore PossibleMultipleEnumeration
            // ReSharper restore AccessToForEachVariableInClosure

            return result;
        }

        private static object MapExceptionData(IDictionary data)
        {
            if (data == null)
                return "<NULL>";

            if (data.Count == 0)
                return "<EMPTY>";

            var result = new Dictionary<string, object>();
            foreach (var entry in data.Keys)
            {
                // ReSharper disable once AccessToForEachVariableInClosure
                AddToDetailsObject(result, entry.ToString(), () => data[entry].ToString());
            }

            return result;
        }

        private static object MapControllerContext(HttpControllerContext controllerContext)
        {
            if (controllerContext == null)
                return "<NULL>";

            return new Dictionary<string, string>
            {
                {"Name", controllerContext.ControllerDescriptor.IfNotNull(d => d.ControllerName)},
                {"Type", controllerContext.ControllerDescriptor.IfNotNull(d => d.ControllerType.FullName)}
            };
        }

        private static object MapActionContext(HttpActionContext actionContext)
        {
            if (actionContext == null)
                return "<NULL>";

            return new Dictionary<string, string>
            {
                {"Name", actionContext.ActionDescriptor.IfNotNull(d => d.ActionName)}
            };
        }

        private static object MapResponse(HttpResponseMessage response)
        {
            if (response == null)
                return "<NULL>";

            var result = new Dictionary<string, object>();

            AddToDetailsObject(result, "StatusCode", () => response.StatusCode);
            AddToDetailsObject(result, "Version", () => response.Version.SafeToString());
            AddToDetailsObject(result, "ReasonPhrase", () => response.ReasonPhrase);
            AddToDetailsObject(result, "IsSuccess", () => response.IsSuccessStatusCode);
            AddToDetailsObject(result, "Headers", () => MapHeaders(response.Headers));

            return result;
        }

        private static object MapRequestContext(HttpRequestContext requestContext)
        {
            if (requestContext == null)
                return "<NULL>";

            var result = new Dictionary<string, object>();

            AddToDetailsObject(result, "Principal", () => MapPrincipal(requestContext.Principal));
            AddToDetailsObject(result, "RouteData", () => MapRouteData(requestContext.RouteData));

            return result;
        }

        private static object MapRequest(HttpRequestMessage request)
        {
            if (request == null)
                return "<NULL>";

            var result = new Dictionary<string, object>();

            AddToDetailsObject(result, "Method", () => request.Method.SafeToString());
            AddToDetailsObject(result, "Uri", () => request.RequestUri);
            AddToDetailsObject(result, "Version", () => request.Version.SafeToString());
            AddToDetailsObject(result, "Headers", () => MapHeaders(request.Headers));
            AddToDetailsObject(result, "Properties", () => request.Properties.ToDictionary(p => p.Key, p => p.Value.SafeToString()));

            return result;
        }

        private static object MapRouteData(IHttpRouteData routeData)
        {
            if (routeData == null)
                return "<NULL>";

            return new Dictionary<string, object>
            {
                {"RouteTemplate", routeData.Route.IfNotNull(r => r.RouteTemplate)},
                {"Values", routeData.Values.ToDictionary(v => v.Key, v => v.Value.SafeToString())}
            };
        }

        private static object MapPrincipal(IPrincipal principal)
        {
            return new Dictionary<string, object>
            {
                {"HasPrincipal", principal != null},
                {"HasIdentity", principal.IfNotNull(p => p.Identity != null)},
                {"Principal", principal.SafeToString()},
                {"Identity", principal.IfNotNull(p => p.Identity.SafeToString())},
                {"IsAuthenticated", principal.IfNotNull(p => p.Identity.IfNotNull(i => i.IsAuthenticated))},
                {"Name", principal.IfNotNull(p => p.Identity.IfNotNull(i => i.Name))},
                {"AuthenticationType", principal.IfNotNull(p => p.Identity.IfNotNull(i => i.AuthenticationType))}
            };
        }

        private static Dictionary<string, string> MapHeaders(HttpHeaders headers)
        {
            return headers.ToDictionary(h => h.Key, h => h.Value.Join());
        }

        private static void AddToDetailsObject(IDictionary<string, object> result, string propertyName, Func<object> func)
        {
            try
            {
                result[propertyName] = func();
            }
            catch (Exception e)
            {
                result[propertyName] = "Exception occured: " + e.Message;
            }
        }

        #endregion
    }
}