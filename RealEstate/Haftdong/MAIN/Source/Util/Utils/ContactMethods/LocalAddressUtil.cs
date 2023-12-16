using Compositional.Composer;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstateAgency.Domain.Base;
using JahanJooy.RealEstateAgency.Domain.Users;
using JahanJooy.RealEstateAgency.Util.Enums;
using MongoDB.Bson;
using ServiceStack;

namespace JahanJooy.RealEstateAgency.Util.Utils.ContactMethods
{
    [Contract]
    [Component]
    public class LocalAddressUtil
    {
        #region Injected dependencies

        #endregion

        #region Action methods

        public ValidationResult PrepareAddressInfo(AddressInfo address, bool isVerified, bool isDeleted, bool isActive)
        {
            address.IsVerifiable = false;
            address.IsVerified = address.ID != ObjectId.Empty ? address.IsVerified : isVerified;
            address.IsDeleted = address.ID != ObjectId.Empty ? address.IsDeleted : isDeleted;
            address.IsActive = address.ID != ObjectId.Empty ? address.IsActive : isActive;
            address.UserContactMethodVerification = address.ID != ObjectId.Empty
                ? address.UserContactMethodVerification
                : new UserContactMethodVerification();
            address.ID = address.ID != ObjectId.Empty ? address.ID : ObjectId.GenerateNewId();

            return NormalizeAddress(address);
        }

        private ValidationResult NormalizeAddress(AddressInfo address)
        {
            if (!address.Value.IsNullOrEmpty())
            {
                address.NormalizedValue = address.Value.Trim();
                return ValidationResult.Success;
            }
            return ValidationResult.Failure("Address", GeneralValidationErrors.NotValid);
        }

        #endregion
    }
}