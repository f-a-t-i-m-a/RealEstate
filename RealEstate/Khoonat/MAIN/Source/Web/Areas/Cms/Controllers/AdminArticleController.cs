using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Domain.Cms;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Areas.Cms.Models.AdminArticle;

namespace JahanJooy.RealEstate.Web.Areas.Cms.Controllers
{
    public class AdminArticleController : CustomControllerBase
    {
        private const int PageSize = 20;

        #region Component plugs

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public ICmsArticleService CmsArticleService { get; set; }

        #endregion

        [HttpGet]
        public ActionResult List(long articleCategoryID)
        {
            List<Article> articles = null;

            if (articleCategoryID != 0)
                articles = DbManager.Db.Articles.Where(a => a.CategoryID == articleCategoryID).ToList();

            if (articleCategoryID != 0 && articles == null)
                return Error(ErrorResult.EntityNotFound);

            var model = new AdminArticleListModel();
            int pageNum = 1;
            if (!string.IsNullOrWhiteSpace(model.Page))
                int.TryParse(model.Page, out pageNum);

			model.Articles = PagedList<Article>.BuildUsingPageNumber(articles.Count(), PageSize, pageNum);
            model.Articles.FillFrom(articles);
            model.ArticleCategoryID = articleCategoryID;
            return View(model);
        }

        [HttpGet]
        public ActionResult NewArticle(long articleCategoryID)
        {
            var model = new AdminArticleModel();
            model.ArticleCategoryID = articleCategoryID;
            return View(model);
        }

        [HttpPost]
        [ActionName("NewArticle")]
        public ActionResult NewArticlePostback(AdminArticleModel model)
        {
            if (model == null)
                return RedirectToAction("NewArticle");

            if (!ModelState.IsValid)
                return View(model);


            long userId = User.CoreIdentity.UserId.GetValueOrDefault();

            var article = new Article()
            {
                CategoryID = model.ArticleCategoryID,
                Name = model.Name,
                Order = 1,
                DisplayType = model.DisplayType,
                IsLocked = true,
                CreatedByUserID = userId,
                LastModifiedByUserID = userId
            };

            CmsArticleService.CreateArticle(article);

            return RedirectToAction("List", new {ArticleCategoryID = model.ArticleCategoryID});
        }

        [HttpGet]
        public ActionResult Edit(long articleID)
        {
            var article = DbManager.Db.Articles.SingleOrDefault(a => a.ID == articleID);
            var articleRevision = DbManager.Db.ArticleRevisions.Where(ar => ar.ArticleID == articleID)
                .OrderByDescending(ar => ar.RevisionNumber).Include( ar => ar.Article)
                .FirstOrDefault() ?? new ArticleRevision()
                {
                    RevisionNumber = 1,
                    ArticleID = articleID,
                    Article = article,
                };

            return View(articleRevision);
        }

        [HttpPost]
        [ActionName("Edit")]
        public ActionResult EditPostback(ArticleRevision model)
        {
            if (!ModelState.IsValid)
                return Edit(model.ArticleID);

            CmsArticleService.UpdateArticleContent(model);

            var article = DbManager.Db.Articles.SingleOrDefault(a => a.ID == model.ArticleID);
            if (article == null)
                return Error(ErrorResult.EntityNotFound);

            return RedirectToAction("List", "AdminArticle", new {ArticleCategoryID = article.CategoryID});
        }

        [HttpGet]
        public ActionResult ViewDetails(long articleID)
        {
            var articleRevision = DbManager.Db.ArticleRevisions.Where(ar => ar.ArticleID == articleID)
                .OrderByDescending(ar => ar.RevisionNumber)
                .Include(ar => ar.Article)
                .FirstOrDefault();
            if (articleRevision == null)
                return Error(ErrorResult.EntityNotFound);

            var md = new MarkdownDeep.Markdown();
            md.ExtraMode = true;
            md.SafeMode = false;

            var model = new AdminArticleViewDetailsModel();
            model.Html = md.Transform(articleRevision.Markdown);
            model.ArticleCategoryID = articleRevision.Article.CategoryID;


            return View(model);
        }
    }
}