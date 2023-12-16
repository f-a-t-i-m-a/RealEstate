using System;
using System.Collections.Generic;
using Compositional.Composer;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstate.Core.Services.Dto;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Core.Services
{
	[Contract]
	public interface IPropertyRequestService
	{
		void DeserializeContents(PropertyRequest request);

		ValidationResult Save(PropertyRequest request);
		ValidationResult Update(long requestID, Func<PropertyRequest, EntityUpdateResult> updateFunction);
		bool ValidateEditPassword(long requestId, string editPassword);
		void SetOwner(IEnumerable<long> requestIds, long userId);
		void SetApproved(long requestId, bool? approved);

		ValidationResult ValidateForSave(PropertyRequest listing);
		ValidationResult ValidateForPublish(PropertyRequest listing);

		void Delete(long listingID);

	}
}