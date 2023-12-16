using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Billing;


namespace JahanJooy.RealEstate.Core.Impl.Templates.Email
{
	public class NotifyBonusAwardedModel
	{
		// Data
		public User User { get; set; }
		public PromotionalBonus Bonus { get; set; }
	}
}