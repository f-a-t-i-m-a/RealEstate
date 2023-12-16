using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using JahanJooy.Common.Util.Log4Net;
using log4net;

namespace JahanJooy.RealEstateAgency.HaftDong.Server.Config
{
	public static class Log4NetConfig
	{
	    public static string Configure()
		{
			var rootLogFolder = ConfigurationManager.AppSettings["log4net.RootLogFolder"];
			if (string.IsNullOrWhiteSpace(rootLogFolder))
			{
			    var appRootPath = HostingEnvironment.MapPath("~");
			    if (appRootPath == null)
			        throw new InvalidOperationException("Cannot configure log4net because HostingEnvironment.MapPath returns null");
			    
                rootLogFolder = Path.GetFullPath(Path.Combine(appRootPath, "..\\log"));
			}

	        GlobalContext.Properties["RootLogFolder"] = rootLogFolder;

	        var configFileInfos = ConfigurationManager
	            .AppSettings["log4net.ConfigFiles"]
	            .Split(';')
	            .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
	            .Select(HostingEnvironment.MapPath)
	            .Select(p => new FileInfo(p));

	        var logConfigError = Log4NetConfiguration.ConfigureFromFiles(configFileInfos);

			return logConfigError;
		}
	}
}