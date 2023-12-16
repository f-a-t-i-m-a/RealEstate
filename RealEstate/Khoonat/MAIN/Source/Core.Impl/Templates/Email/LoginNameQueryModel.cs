using JahanJooy.RealEstate.Domain;

namespace JahanJooy.RealEstate.Core.Impl.Templates.Email
{
	public class LoginNameQueryModel
	{
		public User User { get; set; }
		public UserContactMethod ContactMethod { get; set; }
	}
}