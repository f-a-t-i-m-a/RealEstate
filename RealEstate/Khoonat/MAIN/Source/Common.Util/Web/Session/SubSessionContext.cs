using System;
using System.Collections.Generic;
using System.Web;

namespace JahanJooy.Common.Util.Web.Session
{
	public static class SubSessionContext
	{
		private const string SessionKey = "SubSessions";

		private static IDictionary<string, object> SubSessions
		{
			get
			{
				if (HttpContext.Current.Session[SessionKey] == null)
					HttpContext.Current.Session[SessionKey] = new Dictionary<string, object>();

				return HttpContext.Current.Session[SessionKey] as IDictionary<string, object>;
			}
		}

		public static string Start(object content)
		{
			string id = Guid.NewGuid().ToString();

			SubSessions[id] = content;
			return id;
		}

		public static object Get(string id)
		{
			var subSessions = SubSessions;

			if (subSessions.ContainsKey(id))
				return subSessions[id];

			return null;
		}

		public static bool End(string id)
		{
			return SubSessions.Remove(id);
		}

		public static bool Exists(string id)
		{
			return SubSessions.ContainsKey(id);
		}
	}
}