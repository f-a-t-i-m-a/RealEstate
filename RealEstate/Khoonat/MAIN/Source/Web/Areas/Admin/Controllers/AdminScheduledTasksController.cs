using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.Common.Util.ScheduledTasks;
using JahanJooy.Common.Util.Web.Attributes.MethodSelector;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminScheduledTasks;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Controllers
{
    public class AdminScheduledTasksController : AdminControllerBase
    {
        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IScheduledTaskManager ScheduledTaskManager { get; set; }

        public ActionResult List()
        {
            var model = new AdminScheduledTasksListModel
            {
                ScheduledTasks = DbManager.Db.ScheduledTasks.OrderBy(t => t.TaskKey).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [SubmitButton("btnRunScheduler")]
		[ActionName("PerformOperationOnScheduledTask")]
		public ActionResult RunTask(string taskKey)
        {
            ScheduledTaskManager.IterateTask(taskKey, true);
            return RedirectToAction("List");

        }

        [HttpPost]
        [SubmitButton("btnDisable")]
		[ActionName("PerformOperationOnScheduledTask")]
		public ActionResult SetTaskToDisabled(string taskKey)
        {
			ScheduledTaskManager.ChangeTaskStartup(taskKey, RecurringTaskStartupMode.Disabled);
            return RedirectToAction("List");

        }

        [HttpPost]
        [SubmitButton("btnSetManual")]
		[ActionName("PerformOperationOnScheduledTask")]
		public ActionResult SetTaskToManual(string taskKey)
        {
			ScheduledTaskManager.ChangeTaskStartup(taskKey, RecurringTaskStartupMode.Manual);
            return RedirectToAction("List");

        }

        [HttpPost]
        [SubmitButton("btnSetAutomatic")]
		[ActionName("PerformOperationOnScheduledTask")]
		public ActionResult SetTaskToAutomatic(string taskKey)
        {
			ScheduledTaskManager.ChangeTaskStartup(taskKey, RecurringTaskStartupMode.Automatic);
			return RedirectToAction("List");
        }

        #region Static helper methods called from the view

        public static string GetRowBkgColor(ScheduledTask task)
        {
            if (task == null || task.StartupMode == RecurringTaskStartupMode.Disabled)
                return "#888";

			if (task.StartupMode == RecurringTaskStartupMode.Manual)
				return "#bbb";

            return "inherit";
        }

        public static string GetExecutionBkgColor(ScheduledTask task)
        {
            if (task == null || !task.LastExecutionTime.HasValue || Math.Abs((task.LastExecutionTime.Value - DateTime.Now).TotalHours) > 24)
                return "#fbb";

            return "inherit";
        }

        public static string GetErrorsBkgColor(ScheduledTask task)
        {
            if (task == null || !task.LastErrorTime.HasValue || Math.Abs((task.LastErrorTime.Value - DateTime.Now).TotalHours) > 48)
                return "inherit";

            return "#fbb";
        }

        #endregion
    }
}