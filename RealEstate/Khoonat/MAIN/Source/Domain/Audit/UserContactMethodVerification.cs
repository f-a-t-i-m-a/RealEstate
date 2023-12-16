using System;

namespace JahanJooy.RealEstate.Domain.Audit
{
	public class UserContactMethodVerification
	{
		public long ID { get; set; }
		public long SessionID { get; set; }
		public HttpSession Session { get; set; }

		public User TargetUser { get; set; }
		public long TargetUserID { get; set; }
		public UserContactMethod TargetContactMethod { get; set; }
		public long TargetContactMethodID { get; set; }

		public DateTime StartTime { get; set; }
		public DateTime ExpirationTime { get; set; }
		public DateTime? CompletionTime { get; set; }
		public string VerificationSecret { get; set; }
	}
}