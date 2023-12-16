namespace JahanJooy.RealEstate.Core.Services.Dto.Authentication
{
	public class FailedAuthenticationResult : AuthenticationResult
	{
		public override bool? IsSuccessful
		{
			get { return false; }
		}

		public string ErrorKey { get; set; }
		public bool AccountIsInactive { get; set; }
		public bool AccountIsLocked { get; set; }
	}
}