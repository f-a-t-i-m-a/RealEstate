using System;

namespace JahanJooy.RealEstate.Domain.Audit
{
	public class ActivityLog
	{
		public long ID { get; set; }
		public DateTime LogDate { get; set; }
		public DateTime? ApprovalDate { get; set; }
		public DateTime? ReviewDate { get; set; }
		public double ReviewWeight { get; set; }

		public ActivityAction Action { get; set; }
		public string ActionDetails { get; set; }
		public bool? ActionSucceeded { get; set; }

		public HttpSession Session { get; set; }
		public long? SessionID { get; set; }

		public User AuthenticatedUser { get; set; }
		public long? AuthenticatedUserID { get; set; }

		public User ReviewedBy { get; set; }
		public long? ReviewedByID { get; set; }

		public TargetEntityType? ParentEntity { get; set; }
		public TargetEntityType TargetEntity { get; set; }
		public DetailEntityType? DetailEntity { get; set; }
		public AuditEntityType? AuditEntity { get; set; }
		public long? ParentEntityID { get; set; }
		public long? TargetEntityID { get; set; }
		public long? DetailEntityID { get; set; }
		public long? AuditEntityID { get; set; }
	}
}