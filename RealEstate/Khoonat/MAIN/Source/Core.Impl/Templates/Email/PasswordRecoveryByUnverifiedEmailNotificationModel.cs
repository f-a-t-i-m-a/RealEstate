namespace JahanJooy.RealEstate.Core.Impl.Templates.Email
{
	public class PasswordRecoveryByUnverifiedEmailNotificationModel
	{
		public string TargetEmail { get; set; }
		public string RequestedLoginName { get; set; }
	}
}