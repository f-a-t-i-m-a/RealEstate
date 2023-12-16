using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Domain.Cms;

namespace JahanJooy.RealEstate.Web.Areas.Cms.Models.AdminArticle
{
    public class AdminArticleListModel
    {
        public PagedList<Article> Articles { get; set; }
        public string Page { get; set; }
        public long ArticleCategoryID { get; set; }
    }

}