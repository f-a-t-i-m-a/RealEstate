using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using JahanJooy.Common.Util.Web.Attributes;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base;
using JahanJooy.RealEstate.Web.Resources.Models;
using JahanJooy.RealEstate.Web.Resources.Models.Account;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Auth
{
    public class ApiAuthRegisterInputModel : ApiInputModelBase
    {
        [Required(ErrorMessageResourceType = typeof(AccountSignUpResources),
            ErrorMessageResourceName = "Validation_Required_LoginName")]
        [StringLength(50, ErrorMessageResourceType = typeof(AccountSignUpResources),
            ErrorMessageResourceName = "Validation_Length_LoginName", MinimumLength = 4)]
        public string LoginName { get; set; }

        [Required(ErrorMessageResourceType = typeof(AccountSignUpResources),
            ErrorMessageResourceName = "Validation_Required_Password")]
        [StringLength(50, ErrorMessageResourceType = typeof(AccountSignUpResources),
            ErrorMessageResourceName = "Validation_Length_Password", MinimumLength = 4)]
        public string Password { get; set; }

        [Required(ErrorMessageResourceType = typeof(AccountSignUpResources),
            ErrorMessageResourceName = "Validation_Required_FullName")]
        [StringLength(80, ErrorMessageResourceType = typeof(GeneralValidationErrorResources),
            ErrorMessageResourceName = "StringLength")]
        public string FullName { get; set; }

        [Required(ErrorMessageResourceType = typeof(AccountSignUpResources),
            ErrorMessageResourceName = "Validation_Required_DisplayName")]
        [StringLength(80, ErrorMessageResourceType = typeof(GeneralValidationErrorResources),
            ErrorMessageResourceName = "StringLength")]
        public string DisplayName { get; set; }

        public bool Realtor { get; set; }

        public bool IndependentAgent { get; set; }

        public long? AgencyID { get; set; }

        [Common.Util.Web.Attributes.EmailAddress(true, ErrorMessageResourceType = typeof(AccountSignUpResources),
            ErrorMessageResourceName = "Validation_Email_Email")]
        [StringLength(120, ErrorMessageResourceType = typeof(GeneralValidationErrorResources),
            ErrorMessageResourceName = "StringLength")]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(AccountSignUpResources),
            ErrorMessageResourceName = "Validation_Required_Mobile")]
        [StrictPhoneNumber(ErrorMessageResourceType = typeof(AccountSignUpResources),
            ErrorMessageResourceName = "Validation_PhoneNumber_Mobile")]
        [StringLength(25, ErrorMessageResourceType = typeof(GeneralValidationErrorResources),
            ErrorMessageResourceName = "StringLength")]
        public string Phone1 { get; set; }

        [StrictPhoneNumber(ErrorMessageResourceType = typeof(AccountSignUpResources),
            ErrorMessageResourceName = "Validation_PhoneNumber_Phone2")]
        [StringLength(25, ErrorMessageResourceType = typeof(GeneralValidationErrorResources),
            ErrorMessageResourceName = "StringLength")]
        public string Phone2 { get; set; }

        public VisibilityLevel? EmailVisibility { get; set; }

        public VisibilityLevel? Phone1Visibility { get; set; }

        public VisibilityLevel? Phone2Visibility { get; set; }

        #region Convert to/from domain


        public User ToDomainObject()
        {
            var user = new User
            {
                LoginName = LoginName,
                FullName = FullName,
                DisplayName = DisplayName,
                ContactMethods = new Collection<UserContactMethod>()
            };

            if (!string.IsNullOrWhiteSpace(Email))
                user.ContactMethods.Add(new UserContactMethod
                {
                    ContactMethodType = ContactMethodType.Email,
                    ContactMethodText = Email,
                    Visibility = EmailVisibility ?? VisibilityLevel.System,
                    IsActive = true
                });

            if (!string.IsNullOrWhiteSpace(Phone1))
                user.ContactMethods.Add(new UserContactMethod
                {
                    ContactMethodType = ContactMethodType.Phone,
                    ContactMethodText = Phone1,
                    Visibility = Phone1Visibility ?? VisibilityLevel.System,
                    IsActive = true
                });

            if (!string.IsNullOrWhiteSpace(Phone2))
                user.ContactMethods.Add(new UserContactMethod
                {
                    ContactMethodType = ContactMethodType.Phone,
                    ContactMethodText = Phone2,
                    Visibility = Phone2Visibility ?? VisibilityLevel.System,
                    IsActive = true
                });

            if (Realtor)
            {
                if (IndependentAgent)
                {
                    user.Type = UserType.IndependentAgent;
                    user.AgencyID = null;
                }
                else
                {
                    user.Type = UserType.IndependentAgencyMember;
                    user.AgencyID = AgencyID;
                }
            }
            else
            {
                user.Type = UserType.NormalUser;
                user.AgencyID = null;
            }

            return user;
        }

        #endregion
    }
}