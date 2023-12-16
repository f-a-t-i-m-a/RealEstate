using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Web;
using JahanJooy.Common.Util.Log4Net;
using JahanJooy.Common.Util.Web.UserAgent;

namespace JahanJooy.Common.Util.Web.Robots
{
	public static class CrawlerDetectorUtil
	{
		private static readonly HashSet<string> DetectedUserAgents = new HashSet<string>();
		private static readonly object DetectedUserAgentsLock = new object();

		// Regex tests for the word endings in user agent string, and looks for the following:
		//   - Has a word ending in "bot" (like SurveyBot, ScreenerBot, Googlebot, YandexBot)
		//   - Has a word ending in "agent" (like ips-agent)
		//   - Has a word ending in "monitor" (like ipMonitor)
		//   - Has a word enging in "crawler" (like ip-web-crawler.com)
		//   - Has a word ending in "spider" (like Baiduspider)
		private static readonly Regex GenericBotRegex = new Regex(@"[bB][oO][tT]\W|[aA][gG][eE][nN][tT]\W|[mM][oO][nN][iI][tT][oO][rR]\W|[cC][rR][aA][wW][lL][eE][rR]\W|[sS][pP][iI][dD][eE][rR]\W");

		// Regex tests for specificly identified bots that does not fit into the generic category:
		//   - smarterstats
		//   - panscient.com
		private static readonly Regex SpecificBotRegex = new Regex(@"smarterstats|panscient\.com|WinInet");

		static CrawlerDetectorUtil()
		{
			CommonStaticLogs.UserAgent.Info("================================================================================");
			CommonStaticLogs.UserAgent.Info("Detected user agents cleared.");
		}

		public static bool IsWellKnownBot(HttpRequest request)
		{
			return request.Browser.Crawler;
		}

		public static bool IsWellKnownBot(string userAgent)
		{
			return HttpBrowserCapabilitiesUtil.BuildHttpBrowserCapabilities(new NameValueCollection(), userAgent).Crawler;
		}

		public static bool IsProbableBot(string userAgent)
		{
			if (string.IsNullOrWhiteSpace(userAgent))
				return true;

			// Example terms in the user agent that can be interpreted as a robot:

			// SurveyBot, ScreenerBot, Statsbot, YandexBot
			// ips-agent
			// ipMonitor
			// ip-web-crawler.com
			// Baiduspider

			var result = GenericBotRegex.IsMatch(userAgent) || SpecificBotRegex.IsMatch(userAgent);

			//
			// Log the result if it's a newly-seen user agent

			bool isPreviouslyDetected;
			lock (DetectedUserAgentsLock)
			{
				isPreviouslyDetected = DetectedUserAgents.Contains(userAgent);
				if (!isPreviouslyDetected)
				{
					DetectedUserAgents.Add(userAgent);
				}
			}

			if (!isPreviouslyDetected)
			{
				CommonStaticLogs.UserAgent.InfoFormat("Detected '{0}' - Probable bot: {1}", LogUtils.SanitizeUserInput(userAgent), result);
			}

			return result;
		}
	}
}