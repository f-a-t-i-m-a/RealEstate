using System;
using System.Linq;
using System.Linq.Expressions;
using JahanJooy.RealEstate.Domain.Audit;

namespace JahanJooy.RealEstate.Core.Services.Dto.Audit
{
	public class ActivityLogDisplayInfo
	{
		public long ID { get; set; }
		public DateTime LogDate { get; set; }
		public DateTime? ApprovalDate { get; set; }
		public DateTime? ReviewDate { get; set; }
		public double ReviewWeight { get; set; }

		public ActivityAction Action { get; set; }
		public string ActionDetails { get; set; }
		public bool? ActionSucceeded { get; set; }

		public long? SessionID { get; set; }
		public long? AuthenticatedUserID { get; set; }
		public string AuthenticatedUserLoginName { get; set; }
		public string AuthenticatedUserDisplayName { get; set; }
		public string AuthenticatedUserFullName { get; set; }

		public TargetEntityType? ParentEntity { get; set; }
		public TargetEntityType TargetEntity { get; set; }
		public DetailEntityType? DetailEntity { get; set; }
		public AuditEntityType? AuditEntity { get; set; }
		public long? ParentEntityID { get; set; }
		public long? TargetEntityID { get; set; }
		public long? DetailEntityID { get; set; }
		public long? AuditEntityID { get; set; }

		#region Static fields

		public static readonly Expression<Func<ActivityLog, ActivityLogDisplayInfo>> DisplayInfoExpression;
		public static readonly Func<ActivityLog, ActivityLogDisplayInfo> DisplayInfoDelegate;

		#endregion

		#region Initialization

		static ActivityLogDisplayInfo()
		{
			DisplayInfoExpression = log => new ActivityLogDisplayInfo
				                               {
					                               ID = log.ID,
					                               LogDate = log.LogDate,
					                               ApprovalDate = log.ApprovalDate,
					                               ReviewDate = log.ReviewDate,
					                               ReviewWeight = log.ReviewWeight,
					                               Action = log.Action,
					                               ActionDetails = log.ActionDetails,
					                               ActionSucceeded = log.ActionSucceeded,
					                               SessionID = log.SessionID,
					                               AuthenticatedUserID = log.AuthenticatedUserID,
					                               AuthenticatedUserLoginName = log.AuthenticatedUser.LoginName,
					                               AuthenticatedUserDisplayName = log.AuthenticatedUser.DisplayName,
					                               AuthenticatedUserFullName = log.AuthenticatedUser.FullName,
					                               ParentEntity = log.ParentEntity,
					                               TargetEntity = log.TargetEntity,
					                               DetailEntity = log.DetailEntity,
					                               AuditEntity = log.AuditEntity,
					                               ParentEntityID = log.ParentEntityID,
					                               TargetEntityID = log.TargetEntityID,
					                               DetailEntityID = log.DetailEntityID,
					                               AuditEntityID = log.AuditEntityID
				                               };

			DisplayInfoDelegate = log => new ActivityLogDisplayInfo
				                             {
					                             ID = log.ID,
					                             LogDate = log.LogDate,
					                             ApprovalDate = log.ApprovalDate,
					                             ReviewDate = log.ReviewDate,
					                             ReviewWeight = log.ReviewWeight,
					                             Action = log.Action,
					                             ActionDetails = log.ActionDetails,
					                             ActionSucceeded = log.ActionSucceeded,
					                             SessionID = log.SessionID,
					                             AuthenticatedUserID = log.AuthenticatedUserID,
					                             AuthenticatedUserLoginName = log.AuthenticatedUser != null ? log.AuthenticatedUser.LoginName : null,
					                             AuthenticatedUserDisplayName = log.AuthenticatedUser != null ? log.AuthenticatedUser.DisplayName : null,
												 AuthenticatedUserFullName = log.AuthenticatedUser != null ? log.AuthenticatedUser.FullName : null,
					                             ParentEntity = log.ParentEntity,
					                             TargetEntity = log.TargetEntity,
					                             DetailEntity = log.DetailEntity,
					                             AuditEntity = log.AuditEntity,
					                             ParentEntityID = log.ParentEntityID,
					                             TargetEntityID = log.TargetEntityID,
					                             DetailEntityID = log.DetailEntityID,
					                             AuditEntityID = log.AuditEntityID
				                             };
		}

		#endregion

		#region Public helper methods

		public static ActivityLogDisplayInfo MakeDetails(ActivityLog log)
		{
			return DisplayInfoDelegate(log);
		}

		public static IQueryable<ActivityLogDisplayInfo> MakeDetails(IQueryable<ActivityLog> query)
		{
			return query.Select(DisplayInfoExpression);
		}

		#endregion
	}
}