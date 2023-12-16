using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Domain.Ad;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Areas.Ad.Models.ClickAdmin;

namespace JahanJooy.RealEstate.Web.Areas.Ad.Controllers
{
	public class ClickAdminController : AdminControllerBase
    {
        #region Component plugs

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        #endregion

        #region Actions for ClickAdminController

        [HttpGet]
        public ActionResult ListClicks(ClickAdminListClicksModel model)
        {
            if (model == null)
                model = new ClickAdminListClicksModel();

            int pageNum = 1;
            if (!string.IsNullOrWhiteSpace(model.Page))
                int.TryParse(model.Page, out pageNum);

            var query =
                DbManager.Db.SponsoredEntityClicks
                .Include( s => s.SponsoredEntity);
                  
                

            query = ApplyFilterQuery(model, query);

			model.Clicks = PagedList<SponsoredEntityClick>.BuildUsingPageNumber(query.Count(), 20, pageNum);
            model.Clicks.FillFrom(query.OrderByDescending(c => c.CreationTime));
            return View(model);
        }

        #endregion

        #region Private helper methods

        private static IQueryable<SponsoredEntityClick> ApplyFilterQuery(ClickAdminListClicksModel model, IQueryable<SponsoredEntityClick> clicks)
        {
            if (model.SponsoredEntityIDFilter.HasValue)
                clicks = clicks.Where(s => s.SponsoredEntityID == model.SponsoredEntityIDFilter.Value);

            return clicks;
        }

        #endregion

    }
}
