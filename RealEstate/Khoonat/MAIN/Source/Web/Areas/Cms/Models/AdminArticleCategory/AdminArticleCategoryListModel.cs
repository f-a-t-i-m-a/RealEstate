using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Domain.Cms;

namespace JahanJooy.RealEstate.Web.Areas.Cms.Models.AdminArticleCategory
{
    public class AdminArticleCategoryListModel
    {
        public PagedList<ArticleCategory> ArticleCategories { get; set; }
        public string Page { get; set; }
    }
}