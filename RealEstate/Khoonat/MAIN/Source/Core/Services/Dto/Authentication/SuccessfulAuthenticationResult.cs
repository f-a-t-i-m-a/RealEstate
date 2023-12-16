using JahanJooy.RealEstate.Core.Security;

namespace JahanJooy.RealEstate.Core.Services.Dto.Authentication
{
	public class SuccessfulAuthenticationResult : AuthenticationResult
	{
		public override bool? IsSuccessful
		{
			get { return true; }
		}

		public CorePrincipal Principal;
	}
}