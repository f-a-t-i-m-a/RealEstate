using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Http;
using Compositional.Composer;
using JahanJooy.Common.Util.Log4Net;
using JahanJooy.RealEstateAgency.Api.Web.Base;
using JahanJooy.RealEstateAgency.Domain.Enums.User;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Identity;
using JahanJooy.RealEstateAgency.Util.Models.Logs;
using JahanJooy.RealEstateAgency.Util.UserActivityLog;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Repository.Hierarchy;

namespace JahanJooy.RealEstateAgency.Api.Web.Controllers
{
    [Contract]
    [Component]
    [ComponentCache(null)]
    [RoutePrefix("log")]
    public class LogController : ExtendedApiController
    {
        #region Injected dependencies

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        private const long MaxReturnedLogLength = 100 * 1024;

        private static readonly Regex ArchivableLogFileRegex = new Regex(@".*\d{8}\.log$");
        private static readonly Regex DeleteableLogFileRegex = new Regex(@"^debug\.log$");

        #endregion

        #region Action methods

        [HttpGet, Route("all")]
        [ApplicationAuthorize(BuiltInRoles = new[] { BuiltInRole.Administrator })]
        public GetAllOutput GetAllFeeds()
        {
            return new GetAllOutput
            {
                LogFiles = BuildListOfLogFiles(),
                Loggers = BuildListOfLoggers()
            };
        }

        [HttpPost, Route("debug")]
        [ApplicationAuthorize(BuiltInRoles = new[] { BuiltInRole.Administrator })]
        [SkipUserActivityLogging]
        public IHttpActionResult SetLoggerDebug(ChangeDebugModeInput input)
        {
            var loggerObject = GetLogger(input.Logger);
            var debugAppender = GetDebugAppender();

            if (loggerObject != null)
            {
                loggerObject.Level = input.DebugEnabled ? Level.Debug : Level.Info;

                // Setting the level to Debug is not useful unless the output is included
                // in the debug appender. So, if this logger is not connected, connect it.

                if (input.DebugEnabled && debugAppender != null && !loggerObject.Appenders.Contains(debugAppender))
                {
                    loggerObject.AddAppender(debugAppender);
                }

                GetLog4NetRepository().RaiseConfigurationChanged(EventArgs.Empty);
            }
            return Ok();
        }

        [HttpPost, Route("append")]
        [ApplicationAuthorize(BuiltInRoles = new[] { BuiltInRole.Administrator })]
        [SkipUserActivityLogging]
        public IHttpActionResult SetLoggerAssociation(AppendToDebugAppenderInput input)
        {
            var loggerObject = GetLogger(input.Logger);
            var debugAppender = GetDebugAppender();

            if (loggerObject != null && debugAppender != null)
            {
                if (input.Append && !loggerObject.Appenders.Contains(debugAppender))
                {
                    loggerObject.AddAppender(debugAppender);
                }

                if (!input.Append && loggerObject.Appenders.Contains(debugAppender))
                {
                    loggerObject.RemoveAppender(debugAppender);

                    // If the appender level is set to Debug, it is not useful unless the output is included
                    // in the debug appender. So, if this logger is set to debug level, change it back.
                    loggerObject.Level = null;
                }

                GetLog4NetRepository().RaiseConfigurationChanged(EventArgs.Empty);
            }

            return Ok();
        }

        [HttpPost, Route("view")]
        [ApplicationAuthorize(BuiltInRoles = new[] { BuiltInRole.Administrator })]
        [SkipUserActivityLogging]
        public string ViewLogFile(ViewLogInput input)
        {
            var filePath = GetLogFilePath(input.LogName);
            if (filePath == null)
                return "";

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                if (fs.Length > MaxReturnedLogLength)
                {
                    fs.Seek(-MaxReturnedLogLength, SeekOrigin.End);
                }

                using (var sr = new StreamReader(fs))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        #endregion

        #region Private helper methods

        private Hierarchy GetLog4NetRepository()
        {
            return LogManager.GetRepository() as Hierarchy;
        }

        private IEnumerable<Logger> GetLoggers()
        {
            return GetLog4NetRepository().GetCurrentLoggers().Cast<Logger>();
        }

        private Logger GetLogger(string name)
        {
            return GetLoggers().SingleOrDefault(l => l.Name == name);
        }

        private IAppender GetDebugAppender()
        {
            return GetLog4NetRepository().GetAppenders().SingleOrDefault(a => a.Name == "DebugAppender");
        }

        private List<LogFilesOutput> BuildListOfLogFiles()
        {
            var result = new List<LogFilesOutput>();
            var rootLogFolder = GlobalContext.Properties["RootLogFolder"] as string;

            if (string.IsNullOrWhiteSpace(rootLogFolder))
            {
                ModelState.AddModelError("", @"Root log folder is not specified.");
                return result;
            }

            foreach (var file in Directory.GetFiles(rootLogFolder).OrderBy(s => s))
            {
                var fileInfo = new FileInfo(file);

                result.Add(new LogFilesOutput
                {
                    Name = fileInfo.Name,
                    Size = fileInfo.Length,
                    CreationTime = fileInfo.CreationTime,
                    LastWriteTime = fileInfo.LastWriteTime,
                    CanBeArchived = ArchivableLogFileRegex.IsMatch(fileInfo.Name),
                    CanBeDeleted = DeleteableLogFileRegex.IsMatch(fileInfo.Name)
                });
            }

            result = result.OrderByDescending(r => r.LastWriteTime).ToList();

            return result;
        }

        private List<LoggersOutput> BuildListOfLoggers()
        {
            var debugAppender = GetDebugAppender();
            return GetLoggers().OrderBy(l => l.Name)
                .Select(l => BuildLoggerModel(l, debugAppender))
                .ToList();
        }

        private LoggersOutput BuildLoggerModel(Logger logger, IAppender debugAppender)
        {
            return new LoggersOutput
            {
                Name = logger.Name,
                Additivity = logger.Additivity,
                EffectiveLevel = logger.EffectiveLevel.DisplayName,
                IsDebugEnabled = logger.IsEnabledFor(Level.Debug),
                AssociatedToDebugAppender = logger.Appenders.Contains(debugAppender),
                CanChangeLevelAndAssociation = logger.Name != CommonStaticLogs.Debug.Logger.Name
            };
        }

        private string GetLogFilePath(string name)
        {
            var rootLogFolder = GlobalContext.Properties["RootLogFolder"] as string;
            if (string.IsNullOrWhiteSpace(rootLogFolder))
                return null;

            rootLogFolder = Path.GetFullPath(rootLogFolder);
            var filePath = Path.GetFullPath(Path.Combine(rootLogFolder, name));

            if (!filePath.StartsWith(rootLogFolder))
                return null;

            return filePath;
        }

        #endregion
    }
}