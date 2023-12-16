using System;
using System.Web;
using System.Web.Mvc;

namespace JahanJooy.RealEstate.Web.Helpers
{
	public class IconsHelper
	{
		public static IHtmlString Tristate(UrlHelper url, bool? value)
		{
			if (!value.HasValue)
				return Unknown16(url);

			return value.Value ? Tick16(url) : Cross16(url);
		}

		public static IHtmlString Tristate32(UrlHelper url, bool? value)
		{
			if (!value.HasValue)
				return Unknown32(url);

			return value.Value ? Tick32(url) : Cross32(url);
		}

		public static IHtmlString Add16(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "add", "16").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Add24(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "add", "24").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Add32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "add", "32").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Add16White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "add", "16", "white").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Add24White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "add", "24", "white").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Add32White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "add", "32", "white").ToString(TagRenderMode.SelfClosing));
		}

        public static IHtmlString Announcement32(UrlHelper url)
        {
            return new HtmlString(CreateIconImgTag(url, "announcement", "32").ToString(TagRenderMode.SelfClosing));
        }
        
        public static IHtmlString Attachment16(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "attachment", "16").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Attachment32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "attachment", "32").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Attachment48(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "attachment", "48").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Blueprint32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "blueprint", "32").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Blueprint48(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "blueprint", "48").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Close16(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "close", "16").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Close32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "close", "32").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Close16White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "close", "16", "white").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Close32White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "close", "32", "white").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Community32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "community", "32").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Compare32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "compare", "32").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Compare48(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "compare", "48").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString ContactInfo32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "contactInfo", "32").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString ContactInfo48(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "contactInfo", "48").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString ContactInfoBad32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "contactInfoBad", "32").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString ContactInfoBad32On(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "contactInfoBad", "32", "on").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString ContactInfoGood32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "contactInfoGood", "32").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString ContactInfoGood32On(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "contactInfoGood", "32", "on").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Cross16(UrlHelper url)
		{
			return new HtmlString("<span class='tristate-cross'></span>");
		}
		public static IHtmlString Cross32(UrlHelper url)
		{
			return new HtmlString("<span class='tristate-cross'></span>");
		}

		public static IHtmlString Delete24(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "delete", "24").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Delete32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "delete", "32").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Delete24White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "delete", "24", "white").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Delete32White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "delete", "32", "white").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Details16(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "details", "16").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Details32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "details", "32").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Details48(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "details", "48").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Discussion16(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "discussion", "16").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Discussion32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "discussion", "32").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Discussion48(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "discussion", "48").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Edit16(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "edit", "16").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Edit24(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "edit", "24").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Edit32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "edit", "32").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Edit16White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "edit", "16", "white").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Edit24White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "edit", "24", "white").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Edit32White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "edit", "32", "white").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString EditHistory32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "editHistory", "32").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString EditHistory48(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "editHistory", "48").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Error32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "error", "32").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Error16Red(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "error", "16", "red").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Error32Red(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "error", "32", "red").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString ExtenderCollapsed32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "extender-collapsed", "32").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString ExtenderExtended32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "extender-extended", "32").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString ExtenderCollapsed32White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "extender-collapsed", "32", "white").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString ExtenderExtended32White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "extender-extended", "32", "white").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Flag16(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "flag", "16").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Flag16Yellow(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "flag", "16", "yellow").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Flag32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "flag", "32").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Flag32Yellow(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "flag", "32", "yellow").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Forbidden16(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "forbidden", "16").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Forbidden24(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "forbidden", "24").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Forbidden32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "forbidden", "32").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Forbidden48(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "forbidden", "48").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Forbidden64(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "forbidden", "64").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Forgot32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "forgot", "32").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Forgot64(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "forgot", "64").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString GoDown16(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "godown", "16").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString GoDown16White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "godown", "16", "white").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString GoDown32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "godown", "32").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString GoDown32White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "godown", "32", "white").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString GoLeft16(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "goleft", "16").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString GoLeft16White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "goleft", "16", "white").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString GoLeft32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "goleft", "32").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString GoLeft32White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "goleft", "32", "white").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString GoRight16(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "goright", "16").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString GoRight16White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "goright", "16", "white").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString GoRight32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "goright", "32").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString GoRight32White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "goright", "32", "white").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString GoUp16(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "goup", "16").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString GoUp16White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "goup", "16", "white").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString GoUp32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "goup", "32").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString GoUp32White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "goup", "32", "white").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Idea16(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "idea", "16").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Idea32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "idea", "32").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Idea48(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "idea", "48").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Idea64(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "idea", "64").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Idea16White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "idea", "16", "white").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Idea32White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "idea", "32", "white").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Idea48White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "idea", "48", "white").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Idea64White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "idea", "64", "white").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Login16(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "login", "16").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Login32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "login", "32").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Map32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "map", "32").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Map48(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "map", "48").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Note16(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "note", "16").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Note32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "note", "32").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Note48(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "note", "48").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Photos16(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "photos", "16").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Photos32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "photos", "32").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Photos48(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "photos", "48").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Photos16White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "photos", "16", "white").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Photos32White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "photos", "32", "white").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Photos48White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "photos", "48", "white").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Public24(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "public", "24").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Public32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "public", "32").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Public48(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "public", "48").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Public64(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "public", "64").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString PublicForbidden24(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "publicForbidden", "24").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString PublicForbidden32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "publicForbidden", "32").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString PublicForbidden48(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "publicForbidden", "48").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString PublicForbidden64(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "publicForbidden", "64").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString PublicTime24(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "publicTime", "24").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString PublicTime32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "publicTime", "32").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString PublicTime48(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "publicTime", "48").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString PublicTime64(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "publicTime", "64").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Refresh12(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "refresh", "12").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Refresh16(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "refresh", "16").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Refresh24(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "refresh", "24").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Refresh32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "refresh", "32").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Refresh48(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "refresh", "48").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Register16(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "register", "16").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Register32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "register", "32").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString RequiredOne16(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "required1", "16").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString RequiredOne32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "required1", "32").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString RequiredTwo16(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "required2", "16").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString RequiredTwo32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "required2", "32").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Return16(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "return", "16").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Return24(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "return", "24").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Return32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "return", "32").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Return48(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "return", "48").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Return16White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "return", "16", "white").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Return24White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "return", "24", "white").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Return32White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "return", "32", "white").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Return48White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "return", "48", "white").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Review32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "review", "32").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Review48(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "review", "48").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Save16(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "save", "16").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Save24(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "save", "24").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Save32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "save", "32").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Save16White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "save", "16", "white").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Save24White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "save", "24", "white").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Save32White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "save", "32", "white").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Search16(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "search", "16").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Search32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "search", "32").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Search16White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "search", "16", "white").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Search32White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "search", "32", "white").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Send16(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "send", "16").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Send32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "send", "32").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Send48(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "send", "48").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Share16(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "share", "16").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Share32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "share", "32").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Share48(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "share", "48").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Star32Off(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "star", "32", "off").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Star32Red(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "star", "32", "red").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Star32Green(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "star", "32", "green").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Star32LightBlue(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "star", "32", "lightblue").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Star32LightOrange(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "star", "32", "lightorange").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Star32LightRed(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "star", "32", "lightred").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Star32Blue(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "star", "32", "blue").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Star32Orange(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "star", "32", "orange").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Star32Purple(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "star", "32", "purple").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Star32White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "star", "32", "white").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Star32Yellow(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "star", "32", "yellow").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Star16Off(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "star", "16", "off").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Star16Red(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "star", "16", "red").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Star16Green(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "star", "16", "green").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Star16LightBlue(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "star", "16", "lightblue").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Star16LightOrange(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "star", "16", "lightorange").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Star16LightRed(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "star", "16", "lightred").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Star16Blue(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "star", "16", "blue").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Star16Orange(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "star", "16", "orange").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Star16Purple(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "star", "16", "purple").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Star16White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "star", "16", "white").ToString(TagRenderMode.SelfClosing));
		}
		public static IHtmlString Star16Yellow(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "star", "16", "yellow").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Tick16(UrlHelper url)
		{
			return new HtmlString("<span class='tristate-tick'></span>");
		}
		public static IHtmlString Tick32(UrlHelper url)
		{
			return new HtmlString("<span class='tristate-tick'></span>");
		}

		public static IHtmlString TriangleDown12(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "triangle-down", "12").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString TriangleDown12White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "triangle-down", "12", "white").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString TriangleLeft12(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "triangle-left", "12").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString TriangleLeft12White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "triangle-left", "12", "white").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString TriangleLeft16(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "triangle-left", "16").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString TriangleLeft16White(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "triangle-left", "16", "white").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Unknown16(UrlHelper url)
		{
			return new HtmlString("<span class='tristate-unknown'></span>");
		}

		public static IHtmlString Unknown32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "unknown", "32").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString User16(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "user", "16").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString User32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "user", "32").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString VoteDown32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "voteDown", "32").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString VoteDown32On(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "voteDown", "32", "on").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString VoteUp32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "voteUp", "32").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString VoteUp32On(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "voteUp", "32", "on").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Warning16(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "warning", "16").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Warning32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "warning", "32").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString WarningTime32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "warning-time", "32").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Xlink16(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "xlink", "16").ToString(TagRenderMode.SelfClosing));
		}

		public static IHtmlString Xlink32(UrlHelper url)
		{
			return new HtmlString(CreateIconImgTag(url, "xlink", "32").ToString(TagRenderMode.SelfClosing));
		}

		#region Helpers creating TagBuilder result

		private static TagBuilder CreateIconImgTag(UrlHelper urlHelper, string iconName, string dimension, string variation = null)
		{
			var result = new TagBuilder("img");
			string iconFileName = iconName + "-" + dimension + (String.IsNullOrWhiteSpace(variation) ? "" : ("-" + variation)) + ".png";
			result.MergeAttribute("src", urlHelper.Content("~/Content/icons/" + iconFileName));
			result.MergeAttribute("class", "icon" + dimension);
			return result;
		}

		#endregion
	}
}