using System.Web;
using System.Web.SessionState;
using JahanJooy.RealEstate.Core.Services.Dto;
using JahanJooy.RealEstate.Domain.Audit;

namespace JahanJooy.RealEstate.Web.Application.Session
{
	public static class SessionStateExtensions
	{
		public static SessionInfo GetSessionInfo(this HttpSessionState session)
		{
			if (session == null)
				return SessionInfo.Default;

			return (SessionInfo) session[SessionKeys.SessionInfo];
		}

		public static SessionInfo GetSessionInfo(this HttpSessionStateBase session)
		{
			if (session == null)
				return SessionInfo.Default;

			return (SessionInfo)session[SessionKeys.SessionInfo];
		}

		public static void SetSessionInfo(this HttpSessionState session, SessionInfo sessionInfo)
		{
			session[SessionKeys.SessionInfo] = sessionInfo;
		}

		public static void SetSessionInfo(this HttpSessionStateBase session, SessionInfo sessionInfo)
		{
			session[SessionKeys.SessionInfo] = sessionInfo;
		}
	}
}