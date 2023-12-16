using System.ComponentModel.DataAnnotations;
using JahanJooy.RealEstate.Domain.Cms;

namespace JahanJooy.RealEstate.Web.Areas.Cms.Models.AdminArticleCategory
{
    public class AdminArticleCategoryModel
    {
        [Required]
        public string Name { get; set; }

        public ArticleDisplayType DefaultDisplayType { get; set; }
    }
}