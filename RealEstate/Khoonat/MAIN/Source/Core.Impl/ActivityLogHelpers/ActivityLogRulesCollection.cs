using System;
using System.Collections.Generic;
using Compositional.Composer;
using JahanJooy.RealEstate.Domain.Audit;
using System.Linq;

namespace JahanJooy.RealEstate.Core.Impl.ActivityLogHelpers
{
	[Contract]
	[Component]
	public class ActivityLogRulesCollection
	{
		private readonly Dictionary<TargetEntityType, List<Action<ActivityLog>>> _perEntityTypeRules;
		private readonly List<Action<ActivityLog>> _generalRules; 

		public IEnumerable<Action<ActivityLog>> GetRules(TargetEntityType entityType)
		{
			return _perEntityTypeRules[entityType].Concat(_generalRules);
		}

		public ActivityLogRulesCollection()
		{
			_generalRules = new List<Action<ActivityLog>>();
			_perEntityTypeRules = new Dictionary<TargetEntityType, List<Action<ActivityLog>>>();

			foreach (TargetEntityType entityType in Enum.GetValues(typeof(TargetEntityType)))
				_perEntityTypeRules[entityType] = new List<Action<ActivityLog>>();

			AddUserEntityRules();
			AddPropertyListingEntityRules();
			AddAbuseFlagEntityRules();
			AddGeneralRules();
		}

		private void AddUserEntityRules()
		{
			// Review Weight for User entity
			_perEntityTypeRules[TargetEntityType.User].Add(
				delegate(ActivityLog log)
				{
					if (log.Action == ActivityAction.Authenticate)
						log.ReviewWeight = 0;
					else if (new[] { ActivityLogDetails.UserActionDetails.ChangePassword, ActivityLogDetails.UserActionDetails.CancelContactMethodVerification, ActivityLogDetails.UserActionDetails.DeleteContactMethod }.Any(s => log.ActionDetails.Equals(s)))
						log.ReviewWeight = 0;
					if (log.Action == ActivityAction.Create)
						log.ReviewWeight *= 2;
					else if (log.Action == ActivityAction.Other || log.Action == ActivityAction.OtherDetail)
						log.ReviewWeight *= 0.5;
				});
		}

		private void AddPropertyListingEntityRules()
		{
			// Review Weight for PropertyListing entity
			_perEntityTypeRules[TargetEntityType.PropertyListing].Add(
				delegate(ActivityLog log)
				{
					if (log.Action == ActivityAction.Create || log.Action == ActivityAction.Edit)
						log.ReviewWeight *= 4;
					else if (log.Action == ActivityAction.CreateDetail || log.Action == ActivityAction.EditDetail)
						log.ReviewWeight *= 3;
					else if (log.Action == ActivityAction.Unpublish || log.Action == ActivityAction.Delete)
						log.ReviewWeight = 0;
					else if (log.Action == ActivityAction.Authenticate || log.Action == ActivityAction.Other)
						log.ReviewWeight *= 0.3;
				});

			// Need for Approval, for PropertyListing entity
			_perEntityTypeRules[TargetEntityType.PropertyListing].Add(
				delegate(ActivityLog log)
				{
					if (log.Action == ActivityAction.Create || log.Action == ActivityAction.Edit ||
						log.Action == ActivityAction.CreateDetail || log.Action == ActivityAction.EditDetail)
					{
						if (!ServiceContext.Principal.IsOperator)
							log.ApprovalDate = null;
					}
				});

			// Similar to above rules, for PropertyListingPhoto
			_perEntityTypeRules[TargetEntityType.PropertyListingPhoto].Add(
				delegate(ActivityLog log)
				{
					if (log.Action == ActivityAction.Create || log.Action == ActivityAction.Edit)
						log.ReviewWeight *= 3;
					else if (log.Action == ActivityAction.Delete)
						log.ReviewWeight = 0;
				});

			_perEntityTypeRules[TargetEntityType.PropertyListingPhoto].Add(
				delegate(ActivityLog log)
				{
					if (log.Action == ActivityAction.Create || log.Action == ActivityAction.Edit)
					{
						if (!ServiceContext.Principal.IsOperator)
							log.ApprovalDate = null;
					}
				});

		}

		private void AddAbuseFlagEntityRules()
		{
			// Review Weight for AbuseFlag entity
			_perEntityTypeRules[TargetEntityType.AbuseFlag].Add(
				delegate(ActivityLog log)
				{
					if (log.Action == ActivityAction.Create)
						log.ReviewWeight *= 5;
					else if (log.Action == ActivityAction.CreateDetail)
						log.ReviewWeight *= 2;
				});

			// Need for Approval, for AbuseFlag entity
			_perEntityTypeRules[TargetEntityType.AbuseFlag].Add(
				delegate(ActivityLog log)
				{
					if (log.Action == ActivityAction.Create)
						log.ApprovalDate = null;
				});
		}

		private void AddGeneralRules()
		{
			// If the authenticated user is an operator, there's no need to review the action.
			_generalRules.Add(
				delegate(ActivityLog log)
				{
					if (ServiceContext.Principal != null && ServiceContext.Principal.IsOperator)
						log.ReviewWeight = 0;
				});

			// If an authenticated user has performed the action, lower the review requirement.
			// If the authenticated user is also verified, the review requirement is even lower.
			// But if the activity needs an approval, raise the review weight in the above cases
			// rather than lowering, so that their approval is performed faster.
			_generalRules.Add(
				delegate(ActivityLog log)
				{
					double coefficient = 1;

					if (ServiceContext.Principal != null && ServiceContext.Principal.Identity.IsAuthenticated)
						coefficient *= 0.7;
					if (ServiceContext.Principal != null && ServiceContext.Principal.IsVerified)
						coefficient *= 0.7;

					if (!log.ApprovalDate.HasValue)
						coefficient = 1 / coefficient;

					log.ReviewWeight *= coefficient;
				});
		}
	}
}