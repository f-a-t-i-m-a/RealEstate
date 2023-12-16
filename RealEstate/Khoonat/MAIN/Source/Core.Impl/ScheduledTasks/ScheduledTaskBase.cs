using System;
using Compositional.Composer;
using JahanJooy.Common.Util.ScheduledTasks;
using JahanJooy.RealEstate.Core.Impl.Data;
using log4net;

namespace JahanJooy.RealEstate.Core.Impl.ScheduledTasks
{
    public abstract class ScheduledTaskBase : IScheduledTask
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ScheduledTaskBase));

        public abstract string Key { get; }
        public abstract int MaxIterationsPerSchedule { get; }
        public abstract ScheduledTaskIterationResult IterateInternal(string currentProgress);

        [ComponentPlug]
        public DbManager DbManager { get; set; }

		public virtual void OnBatchStarting()
		{
			// Do nothing
		}

		public virtual void OnBatchCompleted()
		{
			// Do nothing
		}

        public ScheduledTaskIterationResult Iterate(string currentProgress)
        {
            using (DbManager.StartThreadBoundScope())
            {
                ScheduledTaskIterationResult result;
                
                try
                {
                    result = IterateInternal(currentProgress);
                }
                catch (Exception e)
                {
                    Log.Error("Exception occured during iteration of task " + Key, e);
                    return ScheduledTaskIterationResult.Error("Exception: " + e.Message);
                }

                try
                {
                    DbManager.SaveAllChanges();
                }
                catch (Exception e)
                {
                    Log.Error("Exception occured when saving DB changes after iteration of task " + Key, e);
                    return ScheduledTaskIterationResult.Error("Exception: " + e.Message);
                }

                return result;
            }
        }
    }
}