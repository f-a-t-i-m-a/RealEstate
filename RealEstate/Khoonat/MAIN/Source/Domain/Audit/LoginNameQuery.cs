using System;

namespace JahanJooy.RealEstate.Domain.Audit
{
    public class LoginNameQuery
    {
        public long ID { get; set; }
        public long SessionID { get; set; }
        public HttpSession Session { get; set; }

        public User TargetUser { get; set; }
        public long TargetUserID { get; set; }

        public DateTime QueryTime { get; set; }

		public long ContactMethodID { get; set; }
		public UserContactMethod ContactMethod { get; set; }
    }
}