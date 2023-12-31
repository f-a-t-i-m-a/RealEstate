﻿using System;
using System.Web;

namespace JahanJooy.Common.Util.Web.Extensions
{
    /// <summary>
    /// Copy of AjaxRequestExtensions from System.Web.Mvc, but replaced HttpRequestBase with HttpRequest
    /// </summary>
    public static class AjaxRequestExtensions
    {
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            return (request["X-Requested-With"] == "XMLHttpRequest") || ((request.Headers != null) && (request.Headers["X-Requested-With"] == "XMLHttpRequest"));
        }
    }
}
