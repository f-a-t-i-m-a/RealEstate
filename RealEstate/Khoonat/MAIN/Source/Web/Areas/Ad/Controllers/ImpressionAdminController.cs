using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Domain.Ad;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Areas.Ad.Models.ImpressionAdmin;

namespace JahanJooy.RealEstate.Web.Areas.Ad.Controllers
{
	public class ImpressionAdminController : AdminControllerBase
    {

        #region Component plugs

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        #endregion
         #region Actions for ClickAdminController

        [HttpGet]
        public ActionResult ListImpressions(ImpressionAdminListImpressionsModel model)
        {
            if (model == null)
                model = new ImpressionAdminListImpressionsModel();

            int pageNum = 1;
            if (!string.IsNullOrWhiteSpace(model.Page))
                int.TryParse(model.Page, out pageNum);

            var query =
                DbManager.Db.SponsoredEntityImpressions
                    .Include(s => s.SponsoredEntity);
                  
                

            query = ApplyFilterQuery(model, query);

			model.Impressions = PagedList<SponsoredEntityImpression>.BuildUsingPageNumber(query.Count(), 20, pageNum);
            model.Impressions.FillFrom(query.OrderByDescending(c => c.CreationTime));
            return View(model);
        }

        #endregion

        #region Private helper methods

        private static IQueryable<SponsoredEntityImpression> ApplyFilterQuery(ImpressionAdminListImpressionsModel model, IQueryable<SponsoredEntityImpression> impressions)
        {
            if (model.SponsoredEntityIDFilter.HasValue)
                impressions = impressions.Where(s => s.SponsoredEntityID == model.SponsoredEntityIDFilter.Value);

            return impressions;
        }

        #endregion

    }
}
