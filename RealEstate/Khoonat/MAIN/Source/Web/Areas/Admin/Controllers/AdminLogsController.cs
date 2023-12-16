using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util.Log4Net;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminLogs;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Repository.Hierarchy;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Controllers
{
    public class AdminLogsController : AdminControllerBase
    {
        private const long MaxReturnedLogLength = 100 * 1024;

        private static readonly Regex ArchivableLogFileRegex = new Regex(@".*\d{8}\.log$");
        private static readonly Regex DeleteableLogFileRegex = new Regex(@"^debug\.log$");

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        public ActionResult Index()
        {
            var model = new AdminLogsIndexModel
            {
                LogFiles = BuildListOfLogFiles(),
                Loggers = BuildListOfLoggers()
            };

            return View(model);
        }

        public ActionResult ViewLogFile(string id)
        {
            var filePath = GetLogFilePath(id);
            if (filePath == null)
                return Error(ErrorResult.AccessDenied);

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                if (fs.Length > MaxReturnedLogLength)
                {
                    fs.Seek(-MaxReturnedLogLength, SeekOrigin.End);
                }

                using (var sr = new StreamReader(fs))
                {
                    return Content(sr.ReadToEnd(), "text/plain");
                }
            }
        }

        [HttpPost]
        public ActionResult SetLoggerDebug(string logger, bool enabled)
        {
            var loggerObject = GetLogger(logger);
            var debugAppender = GetDebugAppender();

            if (loggerObject != null)
            {
                loggerObject.Level = enabled ? Level.Debug : null;

                // Setting the level to Debug is not useful unless the output is included
                // in the debug appender. So, if this logger is not connected, connect it.

                if (enabled && debugAppender != null && !loggerObject.Appenders.Contains(debugAppender))
                {
                    loggerObject.AddAppender(debugAppender);
                }

                GetLog4NetRepository().RaiseConfigurationChanged(EventArgs.Empty);
            }

            return Json(true);
        }

        public ActionResult SetLoggerAssociation(string logger, bool enabled)
        {
            var loggerObject = GetLogger(logger);
            var debugAppender = GetDebugAppender();

            if (loggerObject != null && debugAppender != null)
            {
                if (enabled && !loggerObject.Appenders.Contains(debugAppender))
                {
                    loggerObject.AddAppender(debugAppender);
                }

                if (!enabled && loggerObject.Appenders.Contains(debugAppender))
                {
                    loggerObject.RemoveAppender(debugAppender);

                    // If the appender level is set to Debug, it is not useful unless the output is included
                    // in the debug appender. So, if this logger is set to debug level, change it back.
                    loggerObject.Level = null;
                }

                GetLog4NetRepository().RaiseConfigurationChanged(EventArgs.Empty);
            }

            return Json(true);
        }

        [HttpPost]
        public ActionResult DeleteLogFile(string id)
        {
            return PartialView((object)id);
        }

        [HttpPost]
        public ActionResult DeleteLogFileConfirmed(string id)
        {
            var filePath = GetLogFilePath(id);
            if (filePath == null)
                return Error(ErrorResult.AccessDenied);

            var fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists || !DeleteableLogFileRegex.IsMatch(fileInfo.Name))
                return Error(ErrorResult.AccessDenied);

            fileInfo.Delete();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult ArchiveLogFile(string id)
        {
            return PartialView((object)id);
        }

        [HttpPost]
        public ActionResult ArchiveLogFileConfirmed(string id)
        {
            var filePath = GetLogFilePath(id);
            if (filePath == null)
                return Error(ErrorResult.AccessDenied);

            var newFilePath = GetArchivedLogFilePath(id);
            if (newFilePath == null)
                return Error(ErrorResult.AccessDenied);

            var fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists || !ArchivableLogFileRegex.IsMatch(fileInfo.Name))
                return Error(ErrorResult.AccessDenied);

            var newFileInfo = new FileInfo(newFilePath);
            if (newFileInfo.Exists)
                return Error(ErrorResult.AccessDenied);

            if (!newFileInfo.Directory.Exists)
                newFileInfo.Directory.Create();

            fileInfo.MoveTo(newFilePath);
            return RedirectToAction("Index");
        }

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

        private List<AdminLogsLogFileModel> BuildListOfLogFiles()
        {
            var result = new List<AdminLogsLogFileModel>();
            var rootLogFolder = GlobalContext.Properties["RootLogFolder"] as string;

            if (string.IsNullOrWhiteSpace(rootLogFolder))
            {
                ModelState.AddModelError("", @"Root log folder is not specified.");
                return result;
            }

            foreach (var file in Directory.GetFiles(rootLogFolder).OrderBy(s => s))
            {
                var fileInfo = new FileInfo(file);

                result.Add(new AdminLogsLogFileModel
                {
                    Name = fileInfo.Name,
                    Size = fileInfo.Length,
                    CreationTime = fileInfo.CreationTime,
                    LastWriteTime = fileInfo.LastWriteTime,
                    CanBeArchived = ArchivableLogFileRegex.IsMatch(fileInfo.Name),
                    CanBeDeleted = DeleteableLogFileRegex.IsMatch(fileInfo.Name)
                });
            }

            return result;
        }

        private List<AdminLogsLoggerModel> BuildListOfLoggers()
        {
            var debugAppender = GetDebugAppender();
            return GetLoggers().OrderBy(l => l.Name)
                .Select(l => BuildLoggerModel(l, debugAppender))
                .ToList();
        }

        private AdminLogsLoggerModel BuildLoggerModel(Logger logger, IAppender debugAppender)
        {
            return new AdminLogsLoggerModel
            {
                Name = logger.Name,
                Additivity = logger.Additivity,
                EffectiveLevel = logger.EffectiveLevel.DisplayName,
                IsDebugEnabled = logger.IsEnabledFor(Level.Debug),
                AssociatedToDebugAppender = logger.Appenders.Contains(debugAppender),
                CanChangeLevelAndAssociation = logger.Name != CommonStaticLogs.Debug.Logger.Name
            };
        }

        private string GetLogFilePath(string id)
        {
            var rootLogFolder = GlobalContext.Properties["RootLogFolder"] as string;
            if (string.IsNullOrWhiteSpace(rootLogFolder))
                return null;

            rootLogFolder = Path.GetFullPath(rootLogFolder);
            var filePath = Path.GetFullPath(Path.Combine(rootLogFolder, id));

            if (!filePath.StartsWith(rootLogFolder))
                return null;

            return filePath;
        }

        private string GetArchivedLogFilePath(string id)
        {
            var rootLogFolder = GlobalContext.Properties["RootLogFolder"] as string;
            if (string.IsNullOrWhiteSpace(rootLogFolder))
                return null;

            rootLogFolder = Path.GetFullPath(Path.Combine(rootLogFolder, "archive"));
            var filePath = Path.GetFullPath(Path.Combine(rootLogFolder, id));

            if (!filePath.StartsWith(rootLogFolder))
                return null;

            return filePath;
        }

        #endregion
    }
}