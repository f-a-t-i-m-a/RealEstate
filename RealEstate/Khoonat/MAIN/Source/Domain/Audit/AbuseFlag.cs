using System;

namespace JahanJooy.RealEstate.Domain.Audit
{
	public class AbuseFlag
	{
		public long ID { get; set; }

		public AbuseFlagEntityType EntityType { get; set; }
		public long EntityID { get; set; }
		public AbuseFlagReason Reason { get; set; }
		public string Comments { get; set; }

		public DateTime ReportDate { get; set; }
		public User ReportedBy { get; set; }
		public long? ReportedByID { get; set; }
		public HttpSession ReportedInSession { get; set; }
		public long? ReportedInSessionID { get; set; }

		public bool? Approved { get; set; }
		public DateTime? ApprovalDate { get; set; }
		public User ApprovedBy { get; set; }
		public long? ApprovedByID { get; set; }
	}
}