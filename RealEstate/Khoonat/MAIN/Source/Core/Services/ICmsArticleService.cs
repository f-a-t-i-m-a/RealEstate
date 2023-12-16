using Compositional.Composer;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstate.Domain.Cms;

namespace JahanJooy.RealEstate.Core.Services
{
    [Contract]
    public interface ICmsArticleService
    {
        ValidationResult CreateArticleCategory(ArticleCategory articleCategory);
        ValidationResult CreateArticle(Article article);
        ValidationResult UpdateArticleContent(ArticleRevision articleRevision);
    }
}
