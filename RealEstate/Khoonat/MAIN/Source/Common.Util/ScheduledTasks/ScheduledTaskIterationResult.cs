using System;

namespace JahanJooy.Common.Util.ScheduledTasks
{
	public class ScheduledTaskIterationResult
	{
		public string Progress { get; set; }
		public string ResultText { get; set; }
		public string ErrorText { get; set; }
        public bool ThereIsMoreWorkToDo { get; set; }

		public static ScheduledTaskIterationResult Error(string errorText, string progress = null, string result = null)
		{
			if (string.IsNullOrWhiteSpace(errorText))
				throw new ArgumentNullException("errorText");

			return new ScheduledTaskIterationResult {ErrorText = errorText, Progress = progress, ResultText = result};
		}

		public static ScheduledTaskIterationResult Success(string progress, string result = null, bool thereIsMoreWorkToDo = false)
		{
			if (progress == null)
				throw new ArgumentNullException("progress");

		    return new ScheduledTaskIterationResult {ErrorText = null, Progress = progress, ResultText = result, ThereIsMoreWorkToDo = thereIsMoreWorkToDo};
		}
	}
}