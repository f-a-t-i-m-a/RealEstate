using System.Web.Optimization;
using BundleTransformer.Core.Bundles;
using BundleTransformer.Core.Orderers;
using BundleTransformer.Core.Resolvers;
using JahanJooy.Common.Util.Configuration;

namespace JahanJooy.RealEstate.Web.Application.Config
{
	public class BundleConfig
	{
		public static void RegisterBundles(BundleCollection bundles)
		{
			BundleTable.EnableOptimizations = ApplicationEnvironmentUtil.Type != ApplicationEnvironmentType.Development;
			bundles.UseCdn = false;
			BundleResolver.Current = new CustomBundleResolver();

			var nullOrderer = new NullOrderer();

			var baseJsBundle = new CustomScriptBundle("~/bundles/js");
			baseJsBundle.Orderer = nullOrderer;

			var angularJsBundle = new CustomScriptBundle("~/bundles/js-angular");
			angularJsBundle.Orderer = nullOrderer;

			var adminJsBundle = new CustomScriptBundle("~/bundles/js-admin");
			adminJsBundle.Orderer = nullOrderer;

			var cssBundle = new CustomStyleBundle("~/bundles/css");
			cssBundle.Orderer = nullOrderer;

			baseJsBundle.Include(
				"~/Scripts/jquery-1.9.1.min.js",
				"~/Scripts/khoonat.common.js", // TODO: Remove
				"~/Scripts/jquery.validate.js",
				"~/Scripts/jquery.validate.fixed.js",
				"~/Scripts/jquery.validate.unobtrusive.min.js",
				"~/Scripts/jquery.qtip.2.2.0.min.js", // TODO: Remove
				"~/Scripts/jquery.qtip.unobtrusive.js", // TODO: Remove
				"~/Scripts/jquery.qtip.2.2.0.imagesloaded.min.js", // TODO: Remove
				"~/Scripts/jquery.autonumeric.1.9.34.js",
				"~/Scripts/select2.3.4.5.min.js",
				"~/Scripts/select2.3.4.5.locale.fa.js",
				"~/Scripts/jquery.unobtrusive-ajax.js",
				"~/Scripts/jquery-migrate-1.1.1.min.js", // TODO: Remove (Used by Chosen)
				"~/Scripts/jquery.chosen.0.9.14.min.js", // TODO: Remove
				"~/Scripts/bootstrap.min.js",
				"~/Scripts/site-base/*.js",
				"~/Scripts/site-common/*.js"
				);

			angularJsBundle.Include(
				"~/Scripts/angular.min.js"
				);

			adminJsBundle.Include(
				"~/Scripts/site-admin/*.js"
				);

			cssBundle.Include(
				"~/Content/jquery.qtip.2.2.0.min.css", // TODO: Remove
				"~/Content/chosen/jquery.chosen.css", // TODO: Remove
				"~/Content/select2/select2.css",
				"~/Content/select2/select2-bootstrap.css",
				"~/Content/bootstrap-theme/bootstrap-theme.less",
				"~/Content/bootstrap-rtl/bootstrap-rtl.less",
				"~/Content/Site.fa-IR.css" // TODO: Remove
				);

			bundles.Add(baseJsBundle);
			bundles.Add(angularJsBundle);
			bundles.Add(adminJsBundle);
			bundles.Add(cssBundle);
		}
	}
}