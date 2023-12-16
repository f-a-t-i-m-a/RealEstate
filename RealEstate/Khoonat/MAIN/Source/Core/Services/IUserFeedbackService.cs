using System.Collections.Generic;
using Compositional.Composer;
using JahanJooy.RealEstate.Domain.Audit;

namespace JahanJooy.RealEstate.Core.Services
{
	[Contract]
	public interface IUserFeedbackService
	{
		AbuseFlag ReportAbuse(AbuseFlagEntityType entityType, long entityId, AbuseFlagReason reason, string comments);
		void ReviewAbuse(long abuseFlagId, bool? approved);

		void SubmitFeedback(string subject, Dictionary<string, string> contents);
	}
}