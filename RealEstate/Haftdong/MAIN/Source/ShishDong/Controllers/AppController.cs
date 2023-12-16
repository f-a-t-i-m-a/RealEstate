using System.Web.Mvc;
using JahanJooy.RealEstateAgency.Util.Resources;

namespace JahanJooy.RealEstateAgency.ShishDong.Controllers
{
    public class AppController : Controller
	{
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
		{
			return RedirectToRoute("App");
		}

        [HttpGet]
        [AllowAnonymous]
        public ActionResult InitApp()
        {
			return View();
		}

        [HttpGet]
        [AllowAnonymous]
        public ActionResult TranslationsJs()
        {
            return View(StaticEnumResourcesMetadata.TranslatedEnumTypes);
        }
    }
}