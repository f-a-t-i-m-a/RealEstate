using System;
using System.Collections.Generic;
using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util.Log4Net;
using log4net;

namespace JahanJooy.Common.Util.ScheduledTasks
{
	[Component]
	public class DefaultScheduledTaskManager : IScheduledTaskManager
	{
	    private const int GlobalMaxIterationsPerSchedule = 50;

	    private static readonly ILog Log = LogManager.GetLogger(typeof (DefaultScheduledTaskManager));

		[ComponentPlug]
		public IComposer Composer { get; set; }

		[ComponentPlug]
		public IScheduledTaskStore Store { get; set; }

		private Dictionary<string, IScheduledTask> _scheduledTasks;
	
		[OnCompositionComplete]
		public void OnCompositionComplete()
		{
            var scheduledTasks = Composer.GetAllComponents<IScheduledTask>() ?? Enumerable.Empty<IScheduledTask>();
			_scheduledTasks = scheduledTasks.ToDictionary(st => st.Key);

		    foreach (var taskKey in _scheduledTasks.Keys)
		    {
		        Store.EnsureTaskDefined(taskKey);
		    }

            Log.DebugFormat("Scheduled Task manager initialized, {0} scheduled tasks are defined.", _scheduledTasks.Count);
            Log.DebugFormat("Names of scheduled tasks: {0}", string.Join(", ", _scheduledTasks.Keys.Select(k => k ?? "<null>")));
		}

	    #region Implementation of IScheduledTaskManager

		public void IterateTask(string key, bool isManuallyTriggered = false)
		{
			if (!_scheduledTasks.ContainsKey(key))
				throw new ArgumentException("The scheduled task key " + key + " is not registered.");

			var startupMode = Store.GetStartupType(key);
			if (startupMode == RecurringTaskStartupMode.Disabled)
				return;

			if (!(startupMode == RecurringTaskStartupMode.Automatic || isManuallyTriggered))
				return;

			var scheduledTask = _scheduledTasks[key];

		    var maxIterations = scheduledTask.MaxIterationsPerSchedule;
		    maxIterations = Math.Max(1, maxIterations);
		    maxIterations = Math.Min(GlobalMaxIterationsPerSchedule, maxIterations);

			scheduledTask.OnBatchStarting();

            Log.DebugFormat("Triggered task key {0} for a maximum of {1} iterations.", key, maxIterations);

			try
			{
				for (int i = 0; i < maxIterations; i++)
				{
					string progress;
					ScheduledTaskIterationResult iterationResult;

					Log.DebugFormat("Task {0} iteration #{1} starting.", key, i);

					try
					{
						progress = Store.GetProgress(key);
					}
					catch (Exception e)
					{
						Log.Error("Could not load current progress for task key " + key, e);
						return;
					}

					Log.DebugFormat("Task {0} iteration #{1} has progress {2} before start.", key, i, progress);

					try
					{
						CommonStaticLogs.ScheduledTask.InfoFormat("Start '{0}' iteration #{1}", key, i);
						iterationResult = scheduledTask.Iterate(progress);
					}
					catch (Exception ex)
					{
						iterationResult = ScheduledTaskIterationResult.Error("Exception occured: " + ex.Message);
					}

					if (iterationResult == null)
						iterationResult = ScheduledTaskIterationResult.Error("Error: NULL result returned by the task.");

					Log.DebugFormat(
						"Task {0} iteration #{1} finished. ErrorText: '{2}'; ResultText: '{3}'; Progress: '{4}'; ThereIsMoreWorkToDo: '{5}'.", key, i,
						iterationResult.ErrorText, iterationResult.ResultText, iterationResult.Progress, iterationResult.ThereIsMoreWorkToDo);

					if (string.IsNullOrWhiteSpace(iterationResult.ErrorText))
						CommonStaticLogs.ScheduledTask.InfoFormat("Task '{0}' iteration #{1} completed without error.", key, i);
					else
						CommonStaticLogs.ScheduledTask.WarnFormat("Error in '{0}' iteration #{1}: {2}", key, i, iterationResult.ErrorText);

					try
					{
						Store.SetProgress(key, iterationResult);
					}
					catch (Exception e)
					{
						Log.Error("Could not set progress for task key " + key, e);
						return;
					}

					if (!string.IsNullOrWhiteSpace(iterationResult.ErrorText))
						break;

					if (!iterationResult.ThereIsMoreWorkToDo)
						break;
				}
			}
			finally
			{
				scheduledTask.OnBatchCompleted();
			}
		}

		public void ChangeTaskStartup(string key, RecurringTaskStartupMode startupMode)
		{
			if (!_scheduledTasks.ContainsKey(key))
				throw new ArgumentException("The scheduled task key " + key + " is not registered.");

			Store.SetStartupType(key, startupMode);
		}

		#endregion
	}
}