using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using JahanJooy.Common.Util.Web.Attributes;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Web.Resources.Models;
using JahanJooy.RealEstate.Web.Resources.Models.Account;

namespace JahanJooy.RealEstate.Web.Models.Account
{
    public class AccountSignUpModel
    {
        [Required(ErrorMessageResourceType = typeof (AccountSignUpResources),
            ErrorMessageResourceName = "Validation_Required_LoginName")]
        [StringLength(50, ErrorMessageResourceType = typeof (AccountSignUpResources),
            ErrorMessageResourceName = "Validation_Length_LoginName", MinimumLength = 4)]
        [Display(ResourceType = typeof (AccountSignUpResources), Name = "Label_LoginName")]
        public string LoginName { get; set; }

        [Required(ErrorMessageResourceType = typeof (AccountSignUpResources),
            ErrorMessageResourceName = "Validation_Required_Password")]
        [StringLength(50, ErrorMessageResourceType = typeof (AccountSignUpResources),
            ErrorMessageResourceName = "Validation_Length_Password", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof (AccountSignUpResources), Name = "Label_Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessageResourceType = typeof (AccountSignUpResources),
            ErrorMessageResourceName = "Validation_Compare_ConfirmPassword")]
        [Display(ResourceType = typeof (AccountSignUpResources), Name = "Label_ConfirmPassword")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessageResourceType = typeof (AccountSignUpResources),
            ErrorMessageResourceName = "Validation_Required_FullName")]
        [Display(ResourceType = typeof (AccountSignUpResources), Name = "Label_FullName")]
        [StringLength(80, ErrorMessageResourceType = typeof (GeneralValidationErrorResources),
            ErrorMessageResourceName = "StringLength")]
        public string FullName { get; set; }

        [Required(ErrorMessageResourceType = typeof (AccountSignUpResources),
            ErrorMessageResourceName = "Validation_Required_DisplayName")]
        [Display(ResourceType = typeof (AccountSignUpResources), Name = "Label_DisplayName")]
        [StringLength(80, ErrorMessageResourceType = typeof (GeneralValidationErrorResources),
            ErrorMessageResourceName = "StringLength")]
        public string DisplayName { get; set; }

        [Display(ResourceType = typeof (AccountSignUpResources), Name = "Label_DisplayNameSameAsFullName")]
        public bool DisplayNameSameAsFullName { get; set; }

        [Display(ResourceType = typeof (AccountSignUpResources), Name = "Lable_Realtor")]
        public bool Realtor { get; set; }

        public bool IndependentAgent { get; set; }

        public long? AgencyID { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(ResourceType = typeof (AccountSignUpResources), Name = "Label_Email")]
        [Common.Util.Web.Attributes.EmailAddress(true, ErrorMessageResourceType = typeof (AccountSignUpResources),
            ErrorMessageResourceName = "Validation_Email_Email")]
        [StringLength(120, ErrorMessageResourceType = typeof (GeneralValidationErrorResources),
            ErrorMessageResourceName = "StringLength")]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(AccountSignUpResources),
            ErrorMessageResourceName = "Validation_Required_Mobile")]
        [Display(ResourceType = typeof (AccountSignUpResources), Name = "Label_Mobile")]
        [StrictPhoneNumber(ErrorMessageResourceType = typeof (AccountSignUpResources),
            ErrorMessageResourceName = "Validation_PhoneNumber_Mobile")]
        [StringLength(25, ErrorMessageResourceType = typeof (GeneralValidationErrorResources),
            ErrorMessageResourceName = "StringLength")]
        public string Mobile { get; set; }

        [Display(ResourceType = typeof (AccountSignUpResources), Name = "Label_Phone2")]
        [StrictPhoneNumber(ErrorMessageResourceType = typeof (AccountSignUpResources),
            ErrorMessageResourceName = "Validation_PhoneNumber_Phone2")]
        [StringLength(25, ErrorMessageResourceType = typeof (GeneralValidationErrorResources),
            ErrorMessageResourceName = "StringLength")]
        public string Phone2 { get; set; }

        [Display(ResourceType = typeof (AccountSignUpResources), Name = "Label_EmailVisibility")]
        public VisibilityLevel EmailVisibility { get; set; }

        [Display(ResourceType = typeof (AccountSignUpResources), Name = "Label_Phone1Visibility")]
        public VisibilityLevel Phone1Visibility { get; set; }

        [Display(ResourceType = typeof (AccountSignUpResources), Name = "Label_Phone2Visibility")]
        public VisibilityLevel Phone2Visibility { get; set; }

        [Display(ResourceType = typeof (AccountSignUpResources), Name = "Label_AcquireOwnedProperties")]
        public bool AcquireOwnedProperties { get; set; }

        

        #region Convert to/from domain

        public User ToDomainObject()
        {
            var user = new User
            {
                LoginName = LoginName,
                FullName = FullName,
                DisplayName = DisplayNameSameAsFullName ? FullName : DisplayName,
                ContactMethods = new Collection<UserContactMethod>()
            };

            if (!string.IsNullOrWhiteSpace(Email))
                user.ContactMethods.Add(new UserContactMethod
                {
                    ContactMethodType = ContactMethodType.Email,
                    ContactMethodText = Email,
                    Visibility = EmailVisibility,
                    IsActive = true
                });

            if (!string.IsNullOrWhiteSpace(Mobile))
                user.ContactMethods.Add(new UserContactMethod
                {
                    ContactMethodType = ContactMethodType.Phone,
                    ContactMethodText = Mobile,
                    Visibility = Phone1Visibility,
                    IsActive = true
                });

            if (!string.IsNullOrWhiteSpace(Phone2))
                user.ContactMethods.Add(new UserContactMethod
                {
                    ContactMethodType = ContactMethodType.Phone,
                    ContactMethodText = Phone2,
                    Visibility = Phone2Visibility,
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