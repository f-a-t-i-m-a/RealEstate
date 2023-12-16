using System.Web.Optimization;
using BundleTransformer.Core.Bundles;
using BundleTransformer.Core.Orderers;
using BundleTransformer.Core.Resolvers;
using JahanJooy.Common.Util.Configuration;

namespace JahanJooy.RealEstateAgency.ShishDong.Server.Config
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = ApplicationEnvironmentUtil.Type != ApplicationEnvironmentType.Development;
            bundles.UseCdn = false;
            BundleResolver.Current = new CustomBundleResolver();

            var nullOrderer = new NullOrderer();

            var jsBundle = new CustomScriptBundle("~/bundles/js") {Orderer = nullOrderer};

            var userBundle = new CustomScriptBundle("~/bundles/user") {Orderer = nullOrderer};

            var modernizrBundle = new CustomScriptBundle("~/bundles/modernizr") {Orderer = nullOrderer};

            var cssBundle = new CustomStyleBundle("~/bundles/css") {Orderer = nullOrderer};

            jsBundle.Include(
                "~/Scripts/jquery-2.2.3.min.js",
                "~/Scripts/jquery.caret-1.5.2.js",
                "~/Scripts/linq.min.js",
                "~/Scripts/lodash.min.js",
                "~/Scripts/angular.min.js",
                "~/Scripts/angular-resource.min.js",
                "~/Scripts/angular-animate.min.js",
                "~/Scripts/angular-sanitize.min.js",
                "~/Scripts/AngularUI/ui-router.min.js",
                "~/Scripts/angular-ui/ui-utils.min.js",
                "~/Scripts/angular-ui/ui-bootstrap.min.js",
                "~/Scripts/angular-ui/ui-bootstrap-tpls.min.js",
                "~/Scripts/select.min.js",
                "~/Scripts/ng-file-upload.js",
                "~/Scripts/angular-bootstrap-persian-datepicker/persiandate.js",
                "~/Scripts/angular-bootstrap-persian-datepicker/persian-datepicker-tpls.js",
                "~/Scripts/angular-toastr.tpls.js",
                "~/Scripts/moment.min.js",
                "~/Scripts/moment-jalaali.js",
                "~/Scripts/Enums.js",
                "~/Scripts/d3/d3.js",
                "~/Scripts/nv.d3.js",
                "~/Scripts/angular-nvd3.js",
                "~/Scripts/respond.js",
                "~/Scripts/angular-simple-logger.min.js"
                );
            jsBundle.IncludeDirectory("~/Application/commonApplication/base", "*.js", true);
            jsBundle.IncludeDirectory("~/Application/commonApplication/jj", "*.js", true);
            jsBundle.IncludeDirectory("~/Application/commonApplication/app", "*.js", true);
            jsBundle.IncludeDirectory("~/Application/commonApplication/api", "*.js", true);
            jsBundle.IncludeDirectory("~/Application/commonApplication/components", "*.js", true);
            jsBundle.IncludeDirectory("~/Application/commonApplication/views", "*.js", true);
            jsBundle.IncludeDirectory("~/Application/commonApplication/translations", "*.js", true);

            userBundle.IncludeDirectory("~/Application/userApplication/base", "*.js", true);
            userBundle.IncludeDirectory("~/Application/userApplication/views", "*.js", true);

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            modernizrBundle.Include(
                "~/Scripts/modernizr-*"
                );

            cssBundle.Include(
                "~/Content/nv.d3.css",
                "~/Content/bootstrap-theme/bootstrap-theme.less",
                "~/Content/bootstrap-rtl/bootstrap-rtl.less",
                "~/Content/angular-toastr.css"
                );

            bundles.Add(jsBundle);
            bundles.Add(userBundle);
            bundles.Add(modernizrBundle);
            bundles.Add(cssBundle);
        }
    }
}