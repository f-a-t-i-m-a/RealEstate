using System;
using System.Collections.Generic;
using Compositional.Composer;
using JahanJooy.Common.Util.DomainServiceContracts;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Core.Services.Dto;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Core.Impl.Services
{
	[Component]
	public class PropertyRequestService : IPropertyRequestService
	{
		[ComponentPlug]
		public IEntityContentSerializer<PropertyRequestContent> Serializer { get; set; }

		#region IPropertyRequestService implementation

		public void DeserializeContents(PropertyRequest request)
		{
			throw new NotImplementedException();
		}

		public ValidationResult Save(PropertyRequest request)
		{
			throw new NotImplementedException();
		}

		public ValidationResult Update(long requestID, Func<PropertyRequest, EntityUpdateResult> updateFunction)
		{
			throw new NotImplementedException();
		}

		public bool ValidateEditPassword(long requestId, string editPassword)
		{
			throw new NotImplementedException();
		}

		public void SetOwner(IEnumerable<long> requestIds, long userId)
		{
			throw new NotImplementedException();
		}

		public void SetApproved(long requestId, bool? approved)
		{
			throw new NotImplementedException();
		}

		public ValidationResult ValidateForSave(PropertyRequest listing)
		{
			throw new NotImplementedException();
		}

		public ValidationResult ValidateForPublish(PropertyRequest listing)
		{
			throw new NotImplementedException();
		}

		public void Delete(long listingID)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region PropertyRequest content serialization
		#endregion
	}
}