using JahanJooy.RealEstate.Domain;


namespace JahanJooy.RealEstate.Core.Impl.Templates.Email
{
    public class BalanceNotificationModel
	{
		// Data
		public User User { get; set; }
		public bool IsBalanceDepleted { get; set; }
		public decimal UserTotalBalance { get; set; }
	}
}