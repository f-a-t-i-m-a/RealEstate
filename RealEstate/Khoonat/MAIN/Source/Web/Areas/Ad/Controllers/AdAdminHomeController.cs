using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Areas.Ad.Models.AdAdminHome;

namespace JahanJooy.RealEstate.Web.Areas.Ad.Controllers
{
    public class AdAdminHomeController : AdminControllerBase
    {
        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IComposer Composer { get; set; }


        [ChildActionOnly]
        public ActionResult CategoryPartial()
        {
            var model = CalculateStatistics();
            return PartialView(model);
        }


        [HttpGet]
        public ActionResult Index()
        {
            var model = CalculateStatistics();
            return View(model);
        }

        private AdAdminHomeStatisticsModel CalculateStatistics()
        {
            var result = new AdAdminHomeStatisticsModel
            {
                SponsoredPropertiesQueueLength =
                    DbManager.Db.SponsoredPropertyListings.Count( spl =>spl.Approved == null)
            };

            return result;
        }
    }
}