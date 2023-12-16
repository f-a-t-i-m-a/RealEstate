using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using log4net;

namespace JahanJooy.RealEstate.Web.Application.Security
{
	public class OwnedPropertiesCookieUtil
	{
	    private static readonly ILog Log = LogManager.GetLogger(typeof (OwnedPropertiesCookieUtil));

		private const char OwnedPropertiesSeparator = ',';
		private const string OwnedPropertiesCookieName = "JJOP";
		private const string OwnedPropertiesMachineKeyPurpose = "OwnedProperties";

		public static HashSet<long> Extract()
		{
			var request = HttpContext.Current.Request;

			var cookie = request.Cookies[OwnedPropertiesCookieName];
			if (cookie == null)
				return null;

			try
			{
				var protectedCookieString = cookie.Value;
				if (string.IsNullOrWhiteSpace(protectedCookieString))
					return null;

				var protectedCookieBytes = Convert.FromBase64String(protectedCookieString);
				var cookieBytes = MachineKey.Unprotect(protectedCookieBytes, OwnedPropertiesMachineKeyPurpose);

				// Return null if the bytes are tampered with
				if (cookieBytes == null)
					return null;

				var cookieText = Encoding.UTF8.GetString(cookieBytes);

				return new HashSet<long>(cookieText.Split(OwnedPropertiesSeparator).Select(long.Parse));
			}
			catch (Exception e)
			{
                Log.Error("Exception while extracting OwnedProperties protected content", e);
				return null;
			}
		}

		public static void Set(HashSet<long> ownedProperties)
		{
			var request = HttpContext.Current.Request;
			var response = HttpContext.Current.Response;

			if (ownedProperties == null || ownedProperties.Count == 0)
			{
				// Clear cookie and return
				if (request.Cookies[OwnedPropertiesCookieName] != null)
					response.Cookies.Add(new HttpCookie(OwnedPropertiesCookieName) {Expires = DateTime.Now.AddDays(-1)});

				return;
			}

			var cookieText = string.Join(new string(OwnedPropertiesSeparator, 1), ownedProperties.Select(id => id.ToString(CultureInfo.InvariantCulture)));
			var cookieBytes = Encoding.UTF8.GetBytes(cookieText);
			var protectedCookieBytes = MachineKey.Protect(cookieBytes, OwnedPropertiesMachineKeyPurpose);
			var protectedCookieString = Convert.ToBase64String(protectedCookieBytes);

			var cookie = new HttpCookie(OwnedPropertiesCookieName, protectedCookieString)
			{
				HttpOnly = true
			};
	
			response.Cookies.Add(cookie);
		}
	}
}