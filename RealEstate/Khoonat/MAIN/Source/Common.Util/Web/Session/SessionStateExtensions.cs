using System.Configuration;
using System.Web;
using System.Web.Configuration;

namespace JahanJooy.Common.Util.Web.Session
{
	public static class SessionStateExtensions
	{
		public const string PrevSessionIdCookieName = "PSID";

		public static void AbandonAndRenewCookie(this HttpSessionStateBase session, HttpResponseBase response)
		{
			string prevSessionId = session.SessionID;

			session.Abandon();
			var sessionStateSection = (SessionStateSection)ConfigurationManager.GetSection("system.web/sessionState");
			string sessionCookieName = sessionStateSection.CookieName;
			
			response.Cookies.Add(new HttpCookie(sessionCookieName, ""));
			response.Cookies.Add(new HttpCookie(PrevSessionIdCookieName, prevSessionId));
		}
	}
}