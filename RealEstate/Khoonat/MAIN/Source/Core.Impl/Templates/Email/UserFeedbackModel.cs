using System.Collections.Generic;
using JahanJooy.RealEstate.Core.Security;
using JahanJooy.RealEstate.Core.Services.Dto;
using JahanJooy.RealEstate.Domain;

namespace JahanJooy.RealEstate.Core.Impl.Templates.Email
{
	public class UserFeedbackModel
	{
		public string Subject { get; set; }
		public IDictionary<string, string> Contents { get; set; }

		public CorePrincipal AuthenticatedUser { get; set; }
		public SessionInfo SessionInfo { get; set; }
	}
}