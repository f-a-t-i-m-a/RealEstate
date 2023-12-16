using System;
using System.Data.Entity;
using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.General;
using JahanJooy.Common.Util.ScheduledTasks;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Domain;

namespace JahanJooy.RealEstate.Core.Impl.ContractImpls
{
	[Component]
	public class ScheduledTaskStore : IScheduledTaskStore
	{
		public void EnsureTaskDefined(string key)
	    {
            // Use Db directly to be independent of any contextual database
            using (var db = new Db())
            {
                if (!db.ScheduledTasksDbSet.Any(t => t.TaskKey == key))
                {
                    var scheduledTask = new ScheduledTask { TaskKey = key };
                    db.ScheduledTasksDbSet.Add(scheduledTask);

                    ((DbContext)db).SaveChanges();
                }
            }
        }

		public RecurringTaskStartupMode GetStartupType(string key)
		{
			// Use Db directly to be independent of any contextual database
			return new Db().Use(db => db.ScheduledTasks.SingleOrDefault(t => t.TaskKey == key).IfNotNull(t => t.StartupMode, RecurringTaskStartupMode.Disabled));
		}

		public void SetStartupType(string key, RecurringTaskStartupMode startupMode)
		{
			using (var db = new Db())
			{
				var scheduledTask = db.ScheduledTasksDbSet.SingleOrDefault(t => t.TaskKey == key);

				if (scheduledTask != null)
				{
					scheduledTask.StartupMode = startupMode;
					((DbContext)db).SaveChanges();
				}
			}
		}

		public string GetProgress(string key)
		{
			// Use Db directly to be independent of any contextual database
			return new Db().Use(db => db.ScheduledTasks.SingleOrDefault(t => t.TaskKey == key).IfNotNull(t => t.TaskProgress));
		}

		public void SetProgress(string key, ScheduledTaskIterationResult iterationResult)
		{
            // Use Db directly to be independent of any contextual database
		    using (var db = new Db())
		    {
                var scheduledTask = db.ScheduledTasksDbSet.SingleOrDefault(t => t.TaskKey == key);

                if (scheduledTask == null)
                {
                    scheduledTask = new ScheduledTask { TaskKey = key };
                    db.ScheduledTasksDbSet.Add(scheduledTask);
                }

                scheduledTask.LastExecutionTime = DateTime.Now;
                scheduledTask.NumberOfExecutions++;

                scheduledTask.LastExecutionResult = iterationResult.ResultText ?? scheduledTask.LastExecutionResult;
                scheduledTask.TaskProgress = iterationResult.Progress ?? scheduledTask.TaskProgress;

                if (!string.IsNullOrWhiteSpace(iterationResult.ErrorText))
                {
                    scheduledTask.LastErrorMessage = iterationResult.ErrorText;
                    scheduledTask.LastErrorTime = DateTime.Now;
                    scheduledTask.NumberOfErrors++;
                }

		        ((DbContext)db).SaveChanges();
		    }
		}
	}
}