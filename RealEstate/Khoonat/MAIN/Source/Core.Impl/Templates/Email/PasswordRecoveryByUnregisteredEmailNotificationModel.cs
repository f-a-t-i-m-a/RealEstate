namespace JahanJooy.RealEstate.Core.Impl.Templates.Email
{
	public class PasswordRecoveryByUnregisteredEmailNotificationModel
	{
		public string TargetEmail { get; set; }
		public string RequestedLoginName { get; set; }
	}
}