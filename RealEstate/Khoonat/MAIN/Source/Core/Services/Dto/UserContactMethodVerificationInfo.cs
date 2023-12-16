using System;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Audit;

namespace JahanJooy.RealEstate.Core.Services.Dto
{
	public class UserContactMethodVerificationInfo
	{
        public UserContactMethod ContactMethod { get; set; }
        public UserContactMethodVerification LatestVerification { get; set; }

		public ValidationResult ValidationForVerificationStart { get; set; }

		public bool HasOngoingVerification
		{
			get
			{
			    return LatestVerification != null &&
			           LatestVerification.ExpirationTime > DateTime.Now;
			}
		}
	}
}