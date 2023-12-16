using System;
using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Domain.Cms;

namespace JahanJooy.RealEstate.Core.Impl.Services
{
    [Component]
    public class CmsArticleService : ICmsArticleService
    {
        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IActivityLogService ActivityLogService { get; set; }

        #region Implementation of ICmsArticleService

        public ValidationResult CreateArticleCategory(ArticleCategory articleCategory)
        {
            articleCategory.CreationTime = DateTime.Now;
            articleCategory.LastModificationTime = DateTime.Now;


            DbManager.Db.ArticleCategoriesDbSet.Add(articleCategory);
            DbManager.SaveDefaultDbChanges();

            return ValidationResult.Success;
        }

        public ValidationResult CreateArticle(Article article)
        {
            article.CreationTime = DateTime.Now;
            article.LastModificationTime = DateTime.Now;


            DbManager.Db.ArticlesDbSet.Add(article);
            DbManager.SaveDefaultDbChanges();

            return ValidationResult.Success;
        }

        public ValidationResult UpdateArticleContent(ArticleRevision model)
        {
            var articleRevision = DbManager.Db.ArticleRevisionsDbSet.SingleOrDefault(ar => ar.ID == model.ID);

            if (articleRevision != null)
            {
                articleRevision.LastModificationTime = DateTime.Now;
                articleRevision.CreationTime = DateTime.Now;
                articleRevision.CreatedByUserID = ServiceContext.Principal.CoreIdentity.UserId.Value;
                articleRevision.LastModifiedByUserID = ServiceContext.Principal.CoreIdentity.UserId.Value;
                articleRevision.LinkText = model.LinkText;
                articleRevision.Markdown = model.Markdown;
                articleRevision.Title = model.Title;
                articleRevision.Description = model.Description;

               
            }
            else
            {
                var revision = new ArticleRevision
                {
                    CreationTime = DateTime.Now,
                    LastModificationTime = DateTime.Now,
                    CreatedByUserID = ServiceContext.Principal.CoreIdentity.UserId.Value,
                    LastModifiedByUserID = ServiceContext.Principal.CoreIdentity.UserId.Value,
                    LinkText = model.LinkText,
                    Markdown = model.Markdown,
                    Title = model.Title,
                    Description = model.Description
                };

                DbManager.Db.ArticleRevisionsDbSet.Add(revision);
            }
     
            DbManager.SaveDefaultDbChanges();

            return ValidationResult.Success;
        }

        #endregion
    }
}