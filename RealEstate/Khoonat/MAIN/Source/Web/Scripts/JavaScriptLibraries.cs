using JahanJooy.Common.Util.Web.Extensions;

namespace JahanJooy.RealEstate.Web.Scripts
{
	public static class JavaScriptLibraries
	{
		public static readonly ScriptLibrary JHashtable;

		public static readonly ScriptLibrary JQueryUI;
		public static readonly ScriptLibrary JQueryNumberFormatter;

		public static readonly ScriptLibrary JQueryCalendars;
		public static readonly ScriptLibrary JQueryCalendarsPickerFa;
		public static readonly ScriptLibrary JQueryCalendarsPersian;
		public static readonly ScriptLibrary JQueryCalendarsPerisanFa;

		public static readonly ScriptLibrary JQueryInputMask;
		public static readonly ScriptLibrary JQueryInputMaskExtensions;
		public static readonly ScriptLibrary JQueryInputMaskDateExtensions;
		public static readonly ScriptLibrary JQueryInputMaskNumericExtensions;

        public static readonly ScriptLibrary JQueryFineUploader;
        public static readonly ScriptLibrary JQueryOverscroll;

        public static readonly ScriptLibrary MarkdownDeep;
        public static readonly ScriptLibrary MarkerWithLabel;
        
		static JavaScriptLibraries()
		{
			JHashtable = new ScriptLibrary(
				"~/Scripts/jshashtable-2.1.min.js");

			JQueryUI = new ScriptLibrary(
				"~/Scripts/jquery-ui-1.10.1.min.js");

			JQueryNumberFormatter = new ScriptLibrary(
				"~/Scripts/jquery.numberformatter-1.2.2.min.js",
				new[] {JHashtable});

			JQueryCalendars = new ScriptLibrary(
				"~/Scripts/jquery.calendars.all.1.2.0.min.js");

			JQueryCalendarsPickerFa = new ScriptLibrary(
				"~/Scripts/jquery.calendars.picker-fa.1.2.0.js",
				new[] {JQueryCalendars});

			JQueryCalendarsPersian = new ScriptLibrary(
				"~/Scripts/jquery.calendars.persian.1.2.0.min.js",
				new[] {JQueryCalendars});

			JQueryCalendarsPerisanFa = new ScriptLibrary(
				"~/Scripts/jquery.calendars.persian-fa.1.2.0.js",
				new[] {JQueryCalendarsPersian});

			JQueryInputMask = new ScriptLibrary(
				"~/Scripts/jquery.inputmask/jquery.inputmask.min.js");

			JQueryInputMaskExtensions = new ScriptLibrary(
				"~/Scripts/jquery.inputmask/jquery.inputmask.extensions.min.js",
				new[] {JQueryInputMask});

			JQueryInputMaskDateExtensions = new ScriptLibrary(
				"~/Scripts/jquery.inputmask/jquery.inputmask.date.extensions.min.js",
				new[] {JQueryInputMask});

			JQueryInputMaskNumericExtensions = new ScriptLibrary(
				"~/Scripts/jquery.inputmask/jquery.inputmask.numeric.extensions.min.js",
				new[] {JQueryInputMask});

			JQueryFineUploader = new ScriptLibrary(
				"~/Scripts/jquery.fineuploader-3.6.4.min.js");

			JQueryOverscroll = new ScriptLibrary(
				"~/Scripts/jquery.overscroll.1.7.2.min.js");

            MarkdownDeep = new ScriptLibrary(
                "~/Scripts/MarkdownDeepLib.min.js"); 

            MarkerWithLabel = new ScriptLibrary(
                "~/Scripts/markerwithlabel.1.1.10.min.js");

        }
	}
}