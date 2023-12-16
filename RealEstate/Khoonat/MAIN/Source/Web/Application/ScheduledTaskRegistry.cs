using System;
using Compositional.Composer;
using FluentScheduler;
using JahanJooy.Common.Util.ScheduledTasks;
using JahanJooy.RealEstate.Core;
using log4net;

namespace JahanJooy.RealEstate.Web.Application
{
	[Contract]
	[Component]
	public class ScheduledTaskRegistry : Registry
	{
        private static readonly ILog Log = LogManager.GetLogger(typeof(ScheduledTaskRegistry));
        
        [ComponentPlug]
		public IScheduledTaskManager ScheduledTaskManager { get; set; }

		public ScheduledTaskRegistry()
		{
			DefaultAllTasksAsNonReentrant();
		}

		[OnCompositionComplete]
		public void OnCompositionComplete()
		{
			Schedule(BuildScheduledAction(ScheduledTaskKeys.SendSavedPropertySearchEmailsTask))
				.ToRunEvery(9).Minutes();

			Schedule(BuildScheduledAction(ScheduledTaskKeys.SendSavedPropertySearchSmsTask))
                .ToRunEvery(7).Minutes();

		    Schedule(BuildScheduledAction(ScheduledTaskKeys.BillSavedSearchSmsNotificationsTask))
		        .ToRunEvery(1).Hours();

            Schedule(BuildScheduledAction(ScheduledTaskKeys.FinalizePartialSavedSearchSmsNotificationBillingsTask))
		        .ToRunEvery(1).Days().At(3, 30);

            Schedule(BuildScheduledAction(ScheduledTaskKeys.TransmitSmsMessagesTask))
                .ToRunEvery(1).Minutes();

			Schedule(BuildScheduledAction(ScheduledTaskKeys.CheckSmsMessageDeliveryTask))
                .ToRunEvery(19).Minutes();

            Schedule(BuildScheduledAction(ScheduledTaskKeys.SendPublishTimeWarningEmailTask))
                .ToRunEvery(1).Days().At(9, 00);

            Schedule(BuildScheduledAction(ScheduledTaskKeys.SendEmailToInactiveUsersTask))
               .ToRunEvery(1).Days().At(12, 00);

            Schedule(BuildScheduledAction(ScheduledTaskKeys.RecalculateSponsoredEntitiesTask))
                .ToRunEvery(15).Minutes();

            Schedule(BuildScheduledAction(ScheduledTaskKeys.BillSponsoredEntityImpressionsTask))
              .ToRunEvery(15).Minutes();

            Schedule(BuildScheduledAction(ScheduledTaskKeys.FinalizePartialSponsoredEntityImpressionBillingsTask))
               .ToRunEvery(1).Days().At(3,40); 

            Schedule(BuildScheduledAction(ScheduledTaskKeys.BillSponsoredEntityClicksTask))
               .ToRunEvery(15).Minutes();

            Schedule(BuildScheduledAction(ScheduledTaskKeys.FinalizePartialSponsoredEntityClickBillingsTask))
              .ToRunEvery(1).Days().At(3, 45);

            Schedule(BuildScheduledAction(ScheduledTaskKeys.SendNotificationMessageEmailAndSms))
               .ToRunEvery(1).Hours();

            Schedule(BuildScheduledAction(ScheduledTaskKeys.OptimizeAllIndexes))
               .ToRunEvery(1).Days().At(4, 00);

            Schedule(BuildScheduledAction(ScheduledTaskKeys.IndexAgencies))
               .ToRunEvery(2).Minutes();

            Schedule(BuildScheduledAction(ScheduledTaskKeys.IndexAgencyBranches))
               .ToRunEvery(2).Minutes();

            Schedule(BuildScheduledAction(ScheduledTaskKeys.IndexPropertyListings))
               .ToRunEvery(1).Minutes();

            Schedule(BuildScheduledAction(ScheduledTaskKeys.IndexPropertyRequests))
               .ToRunEvery(3).Minutes();

		}

		private Action BuildScheduledAction(string taskKey)
		{
		    return () =>
		    {
		        Log.DebugFormat("Scheduled task with key {0} is triggered.", taskKey);

		        try
		        {
		            Log.DebugFormat("Invoking ScheduledTaskManager to iterate task '{0}'", taskKey);
		            ScheduledTaskManager.IterateTask(taskKey);
		        }
		        catch (Exception e)
		        {
		            Log.Error("Exception bubbled up in an scheduled action: " + taskKey, e);
		        }
		    };
		}
	}
}