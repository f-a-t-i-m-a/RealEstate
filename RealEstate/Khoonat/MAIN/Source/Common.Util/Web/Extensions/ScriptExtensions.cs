using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace JahanJooy.Common.Util.Web.Extensions
{
	public static class ScriptExtensions
	{
		private const string ScriptContext = "JahanJooy.Common.Util.Web.Extensions.ScriptExtensions.ScriptContext";

		#region Public extension methods for WebViewPage

		public static HtmlString ScriptBlock(this WebViewPage page, Func<dynamic, HelperResult> block)
		{
			if (page.IsAjax)
				return new HtmlString(block(null).ToHtmlString());

			GetContextScripts(page).AddBlock(block(null).ToHtmlString());
			return new HtmlString(string.Empty);
		}

		public static HtmlString ScriptText(this WebViewPage page, string scriptText)
		{
			if (page.IsAjax)
				return new HtmlString(scriptText);

			GetContextScripts(page).AddBlock(scriptText);
			return new HtmlString(string.Empty);
		}

		public static HtmlString ScriptText(this WebViewPage page, IHtmlString scriptText)
		{
			return ScriptText(page, scriptText.ToString());
		}

		public static HtmlString ScriptLibrary(this WebViewPage page, ScriptLibrary library)
		{
			if (page.IsAjax)
				return new HtmlString(string.Empty);

			GetContextScripts(page).AddLibrary(library);

			return new HtmlString(string.Empty);
		}

		public static HtmlString OutputScripts(this WebViewPage page)
		{
			var contextScripts = GetContextScripts(page);
			if (contextScripts == null)
				return new HtmlString(string.Empty);

			var result = new StringBuilder();

			foreach (var library in contextScripts.GetLibraries())
			{
				result.Append("<script src=\"")
					.Append(page.Url.Content(library.Path))
					.Append("\" type=\"text/javascript\"></script>")
					.Append(Environment.NewLine);
			}

			foreach (var block in contextScripts.GetBlocks())
			{
				result.Append(block)
					.Append(Environment.NewLine);
			}

			return new HtmlString(result.ToString());
		}

		#endregion

		#region Private helper methods

		private static HttpContextScripts GetContextScripts(WebViewPage page)
		{
			var contextScripts = page.Context.Items[ScriptContext] as HttpContextScripts;

			if (contextScripts == null)
			{
				contextScripts = new HttpContextScripts();
				page.Context.Items[ScriptContext] = contextScripts;
			}

			return contextScripts;
		}

		#endregion
	}

	#region Helper classes

	public class HttpContextScripts
	{
		public HttpContextScripts()
		{
			Libraries = null;
			LibraryOrder = null;
			Blocks = null;
		}

		private HashSet<ScriptLibrary> Libraries { get; set; } 
		private List<ScriptLibrary> LibraryOrder { get; set; }
		private List<string> Blocks { get; set; }

		public void AddLibrary(ScriptLibrary library)
		{
			if (Libraries == null)
				Libraries = new HashSet<ScriptLibrary>();

			if (LibraryOrder == null)
				LibraryOrder = new List<ScriptLibrary>();

			AddLibraryInternal(library);
		}

		public void AddBlock(string block)
		{
			if (Blocks == null)
				Blocks = new List<string>();

			Blocks.Add(block);
		}

		public IEnumerable<ScriptLibrary> GetLibraries()
		{
			return LibraryOrder ?? Enumerable.Empty<ScriptLibrary>();
		}

		public IEnumerable<string> GetBlocks()
		{
			return Blocks ?? Enumerable.Empty<string>();
		}

		private void AddLibraryInternal(ScriptLibrary library)
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

	public class ScriptLibrary
	{
		public ScriptLibrary(string path, ScriptLibrary[] dependencies = null)
		{
			Path = path;
			Dependencies = dependencies;
		}

		public string Path { get; private set; }
		public ScriptLibrary[] Dependencies { get; private set; }
	}

	#endregion
}