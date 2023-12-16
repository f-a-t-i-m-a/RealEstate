using JahanJooy.Common.Util.Web.Extensions;

namespace JahanJooy.RealEstate.Web.Content
{
	public class StyleLibraries
	{
		public static readonly StyleLibrary JQueryUiAll;
		public static readonly StyleLibrary JQueryCalendarsPicker;
		public static readonly StyleLibrary JQueryFineUploader;
        public static readonly StyleLibrary MarkdownDeep;

		static StyleLibraries()
		{
			JQueryUiAll = new StyleLibrary("~/Content/themes/base/jquery.ui.all.css", null);
			JQueryCalendarsPicker = new StyleLibrary("~/Content/jquery.calendars.picker-1.2.0.css", null);
			JQueryFineUploader = new StyleLibrary("~/Content/fineuploader/fineuploader-3.6.4.css", null);
            MarkdownDeep = new StyleLibrary("~/Scripts/mdd_styles.css", null);
		}
	}
}