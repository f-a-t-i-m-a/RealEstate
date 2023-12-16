using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Web.Configuration;

namespace JahanJooy.Common.Util.Web.UserAgent
{
	public static class HttpBrowserCapabilitiesUtil
	{
		public static HttpBrowserCapabilities BuildHttpBrowserCapabilities(NameValueCollection headers, string userAgent)
		{
			var factory = new BrowserCapabilitiesFactory();
			var browserCaps = new HttpBrowserCapabilities();
			var hashtable = new Hashtable(180, StringComparer.OrdinalIgnoreCase);
			hashtable[string.Empty] = userAgent;
			browserCaps.Capabilities = hashtable;
			factory.ConfigureBrowserCapabilities(headers, browserCaps);
			factory.ConfigureCustomCapabilities(headers, browserCaps);
			return browserCaps;
		}
	}
}