using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Audit;

namespace JahanJooy.RealEstate.Core.Impl.Templates.Email
{
	public class ContactMethodVerificationModel
	{
		public User User { get; set; }
		public UserContactMethod ContactMethod { get; set; }
		public UserContactMethodVerification Log { get; set; }
	}
}