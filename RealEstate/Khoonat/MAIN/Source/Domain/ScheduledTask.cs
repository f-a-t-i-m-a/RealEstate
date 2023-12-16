using System;
using JahanJooy.Common.Util.ScheduledTasks;

namespace JahanJooy.RealEstate.Domain
{
	public class ScheduledTask
	{
		public long ID { get; set; }
		public RecurringTaskStartupMode StartupMode { get; set; }

		public string TaskKey { get; set; }
		public string TaskProgress { get; set; }

		public DateTime? LastExecutionTime { get; set; }
		public DateTime? LastErrorTime { get; set; }
		
		public long NumberOfExecutions { get; set; }
		public long NumberOfErrors { get; set; }

		public string LastExecutionResult { get; set; }
		public string LastErrorMessage { get; set; }
	}
}