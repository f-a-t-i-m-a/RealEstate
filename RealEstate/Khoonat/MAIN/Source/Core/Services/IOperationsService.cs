using Compositional.Composer;

namespace JahanJooy.RealEstate.Core.Services
{
	[Contract]
	public interface IOperationsService
	{
		void RebuildAgencyIndex();
		void RebuildPropertyListingIndex();
		void RebuildPropertyRequestIndex();

		void RebuildPropertyListingsVicinityHierarchyString();
		void UpdatePropertyListingsGeoLocationFromVicinities();

		void RecalculateUserTransactions(long userId);
		void RebuildPropertyListingPhotos(long fromId, long toId);

		#region Temporary operations services for upgrade

		// Temporary operations services, only needed during upgrade.
		// These services can safely be removed once the upgrade operation is completed on the operational data.

		#endregion
	}
}