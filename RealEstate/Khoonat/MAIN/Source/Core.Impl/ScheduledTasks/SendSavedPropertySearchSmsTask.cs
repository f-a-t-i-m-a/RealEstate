using Compositional.Composer;
using JahanJooy.RealEstate.Core.Services;

namespace JahanJooy.RealEstate.Core.Impl.ScheduledTasks
{
	[Component]
	public class SendSavedPropertySearchSmsTask : SendSavedPropertySearchMessageTaskBase
	{
		#region Injected Dependencies

		[ComponentPlug]
		public ISavedSearchService SavedSearchService { get; set; }

		#endregion

		public override string Key { get { return ScheduledTaskKeys.SendSavedPropertySearchSmsTask; } }

	    protected override int SendMessagesToTargets(long listingId)
		{
			return SavedSearchService.SendSmsToSavedPropertySearchTargets(listingId);
		}
	}
}