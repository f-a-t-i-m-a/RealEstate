using System.Web.Mvc;
using JahanJooy.RealEstate.Web.Application.Base;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Controllers
{
	// This controller does not extend the API-specific controller base, because it is only
	// used to display welcome help text to the user. (for now)
	public class ApiHomeController : CustomControllerBase
	{
		[HttpGet]
		public ActionResult Index()
		{
			return View();
		}
	}
}