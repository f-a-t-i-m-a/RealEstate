using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using JahanJooy.Common.Util.Log4Net;
using log4net;

namespace JahanJooy.RealEstate.Web.Application.Config
{
	public static class Log4NetConfig
	{
		public static string Configure(HttpServerUtility server)
		{
			var rootLogFolder = ConfigurationManager.AppSettings["log4net.RootLogFolder"];
			if (string.IsNullOrWhiteSpace(rootLogFolder))
				rootLogFolder = Path.GetFullPath(Path.Combine(server.MapPath("~"), "..\\log"));

			GlobalContext.Properties["RootLogFolder"] = rootLogFolder;

			var logConfigError = Log4NetConfiguration.ConfigureFromFiles(
				ConfigurationManager
					.AppSettings["log4net.ConfigFiles"]
					.Split(';')
					.Where(s => !string.IsNullOrWhiteSpace(s))
					.Select(s => new FileInfo(server.MapPath(s.Trim()))));

			return logConfigError;
		}
	}
}