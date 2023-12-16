using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace JahanJooy.Common.Util.Web.Extensions
{
	public static class CssExtensions
	{
		private const string CssContext = "JahanJooy.Common.Util.Web.Extensions.CssExtensions.CssContext";

		#region Public extension methods for WebViewPage

		public static HtmlString CssBlock(this WebViewPage page, Func<dynamic, HelperResult> block)
		{
			if (page.IsAjax)
				return new HtmlString(block(null).ToHtmlString());

			GetContextStyles(page).AddBlock(block(null).ToHtmlString());
			return new HtmlString(string.Empty);
		}

		public static HtmlString StyleLibrary(this WebViewPage page, StyleLibrary library)
		{
			if (page.IsAjax)
				return new HtmlString(string.Empty);

			GetContextStyles(page).AddLibrary(library);

			return new HtmlString(string.Empty);
		}

		public static HtmlString OutputStyles(this WebViewPage page)
		{
			var contextStyles = GetContextStyles(page);
			if (contextStyles == null)
				return new HtmlString(string.Empty);

			var result = new StringBuilder();

			foreach (var library in contextStyles.GetLibraries())
			{
				result.Append("<link href=\"")
					.Append(page.Url.Content(library.Path))
					.Append("\" rel=\"stylesheet\" type=\"text/css\"/>")
					.Append(Environment.NewLine);
			}

			foreach (var block in contextStyles.GetBlocks())
			{
				result.Append(block)
					.Append(Environment.NewLine);
			}

			return new HtmlString(result.ToString());
		}

		#endregion

		#region Private helper methods

		private static HttpContextStyles GetContextStyles(WebViewPage page)
		{
			var contextStyles = page.Context.Items[CssContext] as HttpContextStyles;

			if (contextStyles == null)
			{
				contextStyles = new HttpContextStyles();
				page.Context.Items[CssContext] = contextStyles;
			}

			return contextStyles;
		}

		#endregion
	}
	#region Helper classes

	public class HttpContextStyles
	{
		public HttpContextStyles()
		{
			Libraries = null;
			LibraryOrder = null;
			Blocks = null;
		}

		private HashSet<StyleLibrary> Libraries { get; set; }
		private List<StyleLibrary> LibraryOrder { get; set; }
		private List<string> Blocks { get; set; }

		public void AddLibrary(StyleLibrary library)
		{
			if (Libraries == null)
				Libraries = new HashSet<StyleLibrary>();

			if (LibraryOrder == null)
				LibraryOrder = new List<StyleLibrary>();

			AddLibraryInternal(library);
		}

		public void AddBlock(string block)
		{
			if (Blocks == null)
				Blocks = new List<string>();

			Blocks.Add(block);
		}

		public IEnumerable<StyleLibrary> GetLibraries()
		{
			return LibraryOrder ?? Enumerable.Empty<StyleLibrary>();
		}

		public IEnumerable<string> GetBlocks()
		{
			return Blocks ?? Enumerable.Empty<string>();
		}

		private void AddLibraryInternal(StyleLibrary library)
		{
			if (Libraries.Contains(library))
				return;

			if (library.Dependencies != null)
				foreach (var dependentLibrary in library.Dependencies)
					AddLibraryInternal(dependentLibrary);

			Libraries.Add(library);
			LibraryOrder.Add(library);
		}
	}

	public class StyleLibrary
	{
		public StyleLibrary(string path, StyleLibrary[] dependencies)
		{
			Path = path;
			Dependencies = dependencies;
		}

		public string Path { get; private set; }
		public StyleLibrary[] Dependencies { get; private set; }
	}

	#endregion
}