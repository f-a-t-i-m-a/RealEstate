using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace JahanJooy.Common.Util.Web.Routing
{
	/// <summary>
	/// Copied from:
	/// http://stackoverflow.com/questions/2378222/asp-net-mvc-route-class-that-supports-catch-all-parameter-anywhere-in-the-url
	/// 
	/// Sample usage:
	/// routes.Add(new RegexRoute("Show/(?<topics>.*)/(?<id>[\\d]+)/(?<title>.*)", new { controller = "Home", action = "Index" }));
	/// </summary>
	public class RegexRoute : Route
	{
		#region Fields

		private readonly Regex _regEx;
		private readonly RouteValueDictionary _defaultValues;

		#endregion

		#region Initialization

		public RegexRoute(string pattern, object defaultValues)
			: this(pattern, new RouteValueDictionary(defaultValues))
		{
		}

		public RegexRoute(string pattern, RouteValueDictionary defaultValues)
			: this(pattern, defaultValues, new MvcRouteHandler())
		{
		}

		public RegexRoute(string pattern, RouteValueDictionary defaultValues, IRouteHandler routeHandler)
			: base(null, routeHandler)
		{
			_regEx = new Regex(pattern);
			_defaultValues = defaultValues;
		}

		#endregion

		#region Overrides

		public override RouteData GetRouteData(HttpContextBase httpContext)
		{
			string requestedUrl = httpContext.Request.AppRelativeCurrentExecutionFilePath.Substring(2) + httpContext.Request.PathInfo;
			Match match = _regEx.Match(requestedUrl);

			if (match.Success)
			{
				var routeData = new RouteData(this, RouteHandler);
				AddDefaultValues(routeData);

				for (int i = 0; i < match.Groups.Count; i++)
				{
					string key = _regEx.GroupNameFromNumber(i);

					if (string.IsNullOrWhiteSpace(key))
						continue;

					key = key.Trim();
					if (!char.IsLetter(key[0]))
						continue;

					Group group = match.Groups[i];
					if (!string.IsNullOrEmpty(group.Value))
						routeData.Values[key] = group.Value;
				}

				return routeData;
			}

			return null;
		}

		public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
		{
			return null;
//			var virtualPathData = base.GetVirtualPath(requestContext, values);
//			return virtualPathData;
		}

		#endregion

		#region Private helper methods

		private void AddDefaultValues(RouteData routeData)
		{
			if (_defaultValues != null)
			{
				foreach (KeyValuePair<string, object> pair in _defaultValues)
				{
					routeData.Values[pair.Key] = pair.Value;
				}
			}
		}

		#endregion

	}
}