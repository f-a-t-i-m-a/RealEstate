namespace JahanJooy.RealEstate.Core.Services.Dto.Authentication
{
	public class PasswordAuthenticationRequest : AuthenticationRequest
	{
		public string LoginName { get; set; }
		public string Password { get; set; }
	}
}