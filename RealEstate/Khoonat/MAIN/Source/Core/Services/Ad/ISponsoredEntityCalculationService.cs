using Compositional.Composer;

namespace JahanJooy.RealEstate.Core.Services.Ad
{
	[Contract]
	public interface ISponsoredEntityCalculationService
	{
		bool RecalculateNextBatch();
		void RecalculateEntity(long sponsoredEntityId);
	}
}