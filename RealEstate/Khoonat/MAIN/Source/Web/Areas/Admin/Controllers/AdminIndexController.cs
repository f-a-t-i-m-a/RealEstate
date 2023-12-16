using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util.Search;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminIndex;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Controllers
{
    public class AdminIndexController : AdminControllerBase
    {
        #region Injected dependencies

        [ComponentPlug]
        public LuceneIndexManager LuceneIndexManager { get; set; }

        #endregion

        #region public AdminIndex methods

        public ActionResult Index()
        {
            var model = new AdminIndexIndexModel
            {
                LuceneIndexStatistics = LuceneIndexManager.GetAllStatistics()
            };
            return View(model);
        }

        [HttpGet]
        public ActionResult IndexDetails(string indexId)
        {
            var model = new AdminIndexIndexDetailsModel
            {
                LuceneIndexStatistics = LuceneIndexManager.GetStatistics(indexId),
                Errors = LuceneIndexManager.GetErrors(indexId)
            };
            return View(model);
        }

		#endregion
	}
}