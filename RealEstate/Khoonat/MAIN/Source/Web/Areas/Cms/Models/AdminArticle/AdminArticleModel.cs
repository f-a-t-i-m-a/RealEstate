using System.ComponentModel.DataAnnotations;
using JahanJooy.RealEstate.Domain.Cms;

namespace JahanJooy.RealEstate.Web.Areas.Cms.Models.AdminArticle
{
    public class AdminArticleModel
    {
        [Required]
        public string Name { get; set; }
        public long ArticleCategoryID { get; set; }

        public ArticleDisplayType DisplayType { get; set; }
       
    }
}