using System.Web;
using JahanJooy.RealEstate.Core.Services.Dto;

namespace JahanJooy.RealEstate.Web.Application.Session
{
	public static class SessionUtil
	{
		public static SessionInfo Current
		{
			get { return HttpContext.Current.Session.GetSessionInfo(); }
		}
	}
}