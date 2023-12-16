using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Compositional.Composer;
using JahanJooy.Common.Util;
using JahanJooy.RealEstate.Core.Impl.ActivityLogHelpers;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Impl.Templates;
using JahanJooy.RealEstate.Core.Impl.Templates.Email;
using JahanJooy.RealEstate.Core.Notification;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Domain.Audit;

namespace JahanJooy.RealEstate.Core.Impl.Services
{
	[Component]
	public class UserFeedbackService : IUserFeedbackService
	{
		[ComponentPlug]
		public DbManager DbManager { get; set; }

		[ComponentPlug]
		public IActivityLogService ActivityLogService { get; set; }

		[ComponentPlug]
		public IEmailTransmitter EmailTransmitter { get; set; }

		[ComponentPlug]
		public RazorTemplateRunner TemplateRunner { get; set; }

		private const string FeedbackEmailAddress = "feedback@khoonat.com";
		private readonly IRazorTemplate _feedbackTemplate = new EmbeddedResourceRazorTemplate(Assembly.GetExecutingAssembly(), "JahanJooy.RealEstate.Core.Impl.Templates.Email.UserFeedback.cshtml");

		#region IUserFeedbackService implementation

		public AbuseFlag ReportAbuse(AbuseFlagEntityType entityType, long entityId, AbuseFlagReason reason, string comments)
		{
			if (ServiceContext.CurrentSession.IsCrawler)
				throw new InvalidOperationException("Not a user-interactive session");

			var flag = new AbuseFlag
			{
				EntityType = entityType,
				EntityID = entityId,
				Reason = reason,
				Comments = comments,
				ReportDate = DateTime.Now,
				ReportedByID = (ServiceContext.Principal != null && ServiceContext.Principal.CoreIdentity.IsAuthenticated) ? ServiceContext.Principal.CoreIdentity.UserId : null,
				ReportedInSessionID = ServiceContext.CurrentSession.IfNotNull(cs => cs.Record.IfNotNull(r => (long?)r.ID))
			};

			DbManager.Db.AbuseFlagsDbSet.Add(flag);
			DbManager.SaveDefaultDbChanges();

			TargetEntityType? activityLogEntityType = null;
			switch (entityType)
			{
				case AbuseFlagEntityType.PropertyListing:
					activityLogEntityType = TargetEntityType.PropertyListing;
					break;

				case AbuseFlagEntityType.User:
					activityLogEntityType = TargetEntityType.User;
					break;
			}

			ActivityLogService.ReportActivity(TargetEntityType.AbuseFlag, flag.ID, ActivityAction.Create,
											  parentEntity: activityLogEntityType, parentEntityID: entityId);

			return flag;
		}

		public void ReviewAbuse(long abuseFlagId, bool? approved)
		{
			var abuseFlag = DbManager.Db.AbuseFlagsDbSet.SingleOrDefault(l => l.ID == abuseFlagId);

			if (abuseFlag == null)
				throw new ArgumentException("AbuseFlag not found.");

			if (abuseFlag.Approved == approved)
				return;

			if (approved == null)
			{
				abuseFlag.Approved = null;
				abuseFlag.ApprovalDate = null;
				abuseFlag.ApprovedByID = null;
			}
			else
			{
				abuseFlag.Approved = approved;
				abuseFlag.ApprovalDate = DateTime.Now;
				abuseFlag.ApprovedByID = ServiceContext.Principal.CoreIdentity.UserId;
			}


			if (!approved.HasValue)
			{
				ActivityLogService.ReportActivity(TargetEntityType.AbuseFlag, abuseFlagId, ActivityAction.Edit,
					ActivityLogDetails.PropertyListingActionDetails.ClearApproval);
			}
			else
			{
				ActivityLogService.ReportActivity(TargetEntityType.AbuseFlag, abuseFlagId,
					approved.Value ? ActivityAction.Approve : ActivityAction.Reject);
				ActivityLogService.MarkTaskComplete(TargetEntityType.AbuseFlag, abuseFlagId, ActivityAction.Create);
				ActivityLogService.MarkTaskComplete(TargetEntityType.AbuseFlag, abuseFlagId, ActivityAction.Edit);
			}
		}

		public void SubmitFeedback(string subject, Dictionary<string, string> contents)
		{
			string body = TemplateRunner.ApplyTemplate(_feedbackTemplate,
				new UserFeedbackModel
				{
					Subject = subject,
					Contents = contents,
					AuthenticatedUser = ServiceContext.Principal,
					SessionInfo = ServiceContext.CurrentSession
				});

			subject = "User feedback: " + subject;
			EmailTransmitter.Send(subject, body, new[] {FeedbackEmailAddress}, Enumerable.Empty<string>(), Enumerable.Empty<string>());
		}

		#endregion
	}
}