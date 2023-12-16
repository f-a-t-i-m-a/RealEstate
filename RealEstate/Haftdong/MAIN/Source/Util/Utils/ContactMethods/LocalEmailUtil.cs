using System.Text.RegularExpressions;
using Compositional.Composer;
using JahanJooy.Common.Util.Text;
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
    public class LocalEmailUtil
    {
        #region Injected dependencies

        private const string EmailRegexString = @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?";

        private static readonly Regex EmailRegex = new Regex(EmailRegexString);
        private static readonly Regex WholeEmailRegex = new Regex(RegexUtils.CreateWholeInputRegex(EmailRegexString));
       
        #endregion

        #region Action methods

        public ValidationResult PrepareEmailInfo(EmailInfo email, bool isVerified, bool isDeleted, bool isActive)
        {
            email.IsVerifiable = true;
            email.IsVerified = email.ID != ObjectId.Empty ? email.IsVerified : isVerified;
            email.IsDeleted = email.ID != ObjectId.Empty ? email.IsDeleted : isDeleted;
            email.IsActive = email.ID != ObjectId.Empty ? email.IsActive : isActive;
            email.UserContactMethodVerification = email.ID != ObjectId.Empty
                ? email.UserContactMethodVerification
                : new UserContactMethodVerification();
            email.ID = email.ID != ObjectId.Empty ? email.ID : ObjectId.GenerateNewId();

            return NormalizeEmail(email);
        }

        private ValidationResult NormalizeEmail(EmailInfo email)
        {
            if (!email.Value.IsNullOrEmpty())
            {
                email.NormalizedValue = email.Value.Trim();
                return ValidationResult.Success;
            }
                return ValidationResult.Failure("Email", GeneralValidationErrors.NotValid);
        }

        public bool IsValidEmail(string email, bool allowPartial)
        {
            return (allowPartial ? EmailRegex : WholeEmailRegex).IsMatch(email);
        }

        public bool IsValidEmail(string email)
        {
            return IsValidEmail(email, false);
        }

        #endregion
    }
}