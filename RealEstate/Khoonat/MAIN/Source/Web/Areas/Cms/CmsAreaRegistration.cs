using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JahanJooy.RealEstate.Web.Areas.Cms.Controllers;

namespace JahanJooy.RealEstate.Web.Areas.Cms
{
    public class CmsAreaRegistration:AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "cms";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Cms_default",
                "cms/{controller}/{action}/{id}",
                new { controller = "AdminArticleCategory", action = "List", id = UrlParameter.Optional },
                new[] { typeof(AdminArticleCategoryController).Namespace }
            );
        }
    }
}