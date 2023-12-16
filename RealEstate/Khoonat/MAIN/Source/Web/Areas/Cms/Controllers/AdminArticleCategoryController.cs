using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Domain.Cms;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Areas.Cms.Models.AdminArticleCategory;

namespace JahanJooy.RealEstate.Web.Areas.Cms.Controllers
{
    public class AdminArticleCategoryController : CustomControllerBase
    {
        private const int PageSize = 20;

        #region Component plugs

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public ICmsArticleService CmsArticleService { get; set; }

        #endregion

        [HttpGet]
        public ActionResult List(AdminArticleCategoryListModel model)
        {
            if (model == null)
                model = new AdminArticleCategoryListModel();

            int pageNum = 1;
            if (!string.IsNullOrWhiteSpace(model.Page))
                int.TryParse(model.Page, out pageNum);

            var query = DbManager.Db.ArticleCategories.OrderBy(ac => ac.ID);

			model.ArticleCategories = PagedList<ArticleCategory>.BuildUsingPageNumber(query.Count(), PageSize, pageNum);
            model.ArticleCategories.FillFrom(query);

            return View(model);
        }

        [HttpGet]
        public ActionResult NewArticleCategory()
        {

            var model = new AdminArticleCategoryModel();
            return View(model);
        }

        [HttpPost]
        [ActionName("NewArticleCategory")]
        public ActionResult NewArticleCategoryPostback(AdminArticleCategoryModel model)
        {
            if (model == null)
                return RedirectToAction("NewArticleCategory");

            if (!ModelState.IsValid)
                return View(model);

            long userId = User.CoreIdentity.UserId.GetValueOrDefault();
            var articleCategory = new ArticleCategory
            {
                DefaultDisplayType = model.DefaultDisplayType,
                Name = model.Name,
                LastModifiedByUserID = userId,
                CreatedByUserID = userId
            };

            CmsArticleService.CreateArticleCategory(articleCategory);


            return RedirectToAction("List");
        }

    }
}
