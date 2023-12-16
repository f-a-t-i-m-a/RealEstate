using System;
using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util;
using JahanJooy.RealEstate.Core.Impl.ActivityLogHelpers;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Domain.Audit;

namespace JahanJooy.RealEstate.Core.Impl.Services
{
	[Component]
	public class ActivityLogService : IActivityLogService
	{
		[ComponentPlug]
		public DbManager DbManager { get; set; }

		[ComponentPlug]
		public ActivityLogRulesCollection RulesCollection { get; set; }

		public void ReportActivity(TargetEntityType targetEntity, long? targetEntityID, ActivityAction action,
		                           string actionDetails = null, bool? succeeded = null, TargetEntityType? parentEntity = null, long? parentEntityID = null,
		                           DetailEntityType? detailEntity = null, long? detailEntityID = null, AuditEntityType? auditEntity = null, long? auditEntityID = null)
		{
			if (string.IsNullOrEmpty(actionDetails))
				actionDetails = action.ToString();

			var log = new ActivityLog
				          {
					          LogDate = DateTime.Now,
							  ReviewWeight = 1000,
							  Action = action,
							  ActionDetails = actionDetails,
							  ActionSucceeded = succeeded,
							  ParentEntity = parentEntity,
							  TargetEntity = targetEntity,
							  DetailEntity = detailEntity,
							  AuditEntity = auditEntity,
							  ParentEntityID = parentEntityID,
							  TargetEntityID = targetEntityID,
							  DetailEntityID = detailEntityID,
							  AuditEntityID = auditEntityID
				          };

			log.SessionID = ServiceContext.CurrentSession.IfNotNull(cs => cs.Record.IfNotNull(r => (long?)r.ID));
			log.AuthenticatedUserID = ServiceContext.Principal.CoreIdentity.UserId;

			// By default, none of the logs are treated as a "task" unless specified by rules
			log.ApprovalDate = log.LogDate;

			foreach (var rule in RulesCollection.GetRules(log.TargetEntity))
				rule(log);

			if (log.ReviewWeight <= 0)
			{
				log.ReviewWeight = 0;
				log.ReviewDate = log.LogDate;
			}

			DbManager.Db.ActivityLogsDbSet.Add(log);
		}

		public void MarkTaskComplete(TargetEntityType targetEntityType, long targetEntityID, ActivityAction action)
		{
			var logs = DbManager.Db.ActivityLogsDbSet.Where(l => l.TargetEntity == targetEntityType && l.TargetEntityID == targetEntityID && l.Action == action && !l.ApprovalDate.HasValue).ToList();

			foreach (var log in logs)
				log.ApprovalDate = DateTime.Now;
		}
	}
}