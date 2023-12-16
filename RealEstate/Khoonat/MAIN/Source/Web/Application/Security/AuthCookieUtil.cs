using System;
using System.Globalization;
using System.Web;
using System.Web.Security;

namespace JahanJooy.RealEstate.Web.Application.Security
{
	public static class AuthCookieUtil
	{
		private const int TicketVersion = 1;
	    private const int PersistentCookieValidityDays = 30;

		public static AuthCookieContents Extract()
		{
			try
			{
				var request = HttpContext.Current.Request;
				HttpCookie authCookie = request.Cookies[FormsAuthentication.FormsCookieName];
				if (authCookie == null)
					return null;

                return Extract(authCookie.Value);
			}
			catch (Exception)
			{
				return null;
			}
		}

	    public static AuthCookieContents Extract(string encryptedTicket)
	    {
	        try
	        {
	            var ticket = FormsAuthentication.Decrypt(encryptedTicket);

	            if (ticket == null || ticket.Version != TicketVersion || string.IsNullOrWhiteSpace(ticket.UserData) ||
	                string.IsNullOrWhiteSpace(ticket.Name) || ticket.Expired)
	                return null;

	            long userID;
	            if (!long.TryParse(ticket.UserData, out userID))
	                return null;

	            return new AuthCookieContents {LoginName = ticket.Name, UserID = userID};
	        }
	        catch (Exception)
	        {
	            return null;
	        }
        }

	    public static string BuildTicket(AuthCookieContents contents, bool persistant)
	    {
            var ticket = new FormsAuthenticationTicket(
                TicketVersion,
                contents.LoginName,
                DateTime.Now,
                DateTime.Now.Add(persistant ? TimeSpan.FromDays(PersistentCookieValidityDays) : FormsAuthentication.Timeout),
                persistant,
                contents.UserID.ToString(CultureInfo.InvariantCulture),
                FormsAuthentication.FormsCookiePath);

            return FormsAuthentication.Encrypt(ticket);
        }

        public static void Set(AuthCookieContents contents, bool persistant)
		{
			var response = HttpContext.Current.Response;
            var encryptedTicked = BuildTicket(contents, persistant);
			var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicked)
				                 {
					                 HttpOnly = true,
					                 Secure = FormsAuthentication.RequireSSL,
					                 Path = FormsAuthentication.FormsCookiePath,
					                 Domain = FormsAuthentication.CookieDomain
				                 };

			if (persistant)
				authCookie.Expires = DateTime.Now.Add(TimeSpan.FromDays(PersistentCookieValidityDays));

			response.Cookies.Add(authCookie);
		}
	}
}