using System.Web.Mvc;
using JahanJooy.RealEstate.Core.Security;
using JahanJooy.RealEstate.Core.Services.Dto;
using JahanJooy.RealEstate.Web.Application.Session;

namespace JahanJooy.RealEstate.Web.Application.Base
{
	public abstract class WebViewPageBase : WebViewPage
	{
		public virtual new CorePrincipal User => base.User as CorePrincipal;

	    public SessionInfo SessionInfo => Session.GetSessionInfo();
	}

	public abstract class WebViewPageBase<TModel> : WebViewPage<TModel>
	{
		public virtual new CorePrincipal User => base.User as CorePrincipal ?? CorePrincipal.Anonymous;

	    public SessionInfo SessionInfo => Session.GetSessionInfo();
	}
}