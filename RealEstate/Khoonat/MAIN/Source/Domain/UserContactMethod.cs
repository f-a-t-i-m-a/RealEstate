using JahanJooy.RealEstate.Domain.Enums;

namespace JahanJooy.RealEstate.Domain
{
	public class UserContactMethod
	{
		public long ID { get; set; }

		public User User { get; set; }
		public long UserID { get; set; }

		public ContactMethodType ContactMethodType { get; set; }
		public string ContactMethodText { get; set; }
		public VisibilityLevel Visibility { get; set; }

        public bool IsVerifiable { get; set; }
        public bool IsVerified { get; set; }
		public bool IsActive { get; set; }
		public bool IsDeleted { get; set; }
	}
}