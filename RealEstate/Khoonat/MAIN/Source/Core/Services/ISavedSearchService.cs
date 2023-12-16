using Compositional.Composer;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstate.Domain.SavedSearch;

namespace JahanJooy.RealEstate.Core.Services
{
	[Contract]
	public interface ISavedSearchService
	{
		ValidationResult SavePropertySearch(SavedPropertySearch savedPropertySearch);
		ValidationResult Update(SavedPropertySearch savedPropertySearch);
		void DeletePropertySearch(long id);

		int SendEmailToSavedPropertySearchTargets(long listingId);
		int SendSmsToSavedPropertySearchTargets(long listingId);
	    bool BillSavedSearchSmsNotifications();
	    bool FinalizePartialSavedSearchSmsNotificationBillings();
	}
}