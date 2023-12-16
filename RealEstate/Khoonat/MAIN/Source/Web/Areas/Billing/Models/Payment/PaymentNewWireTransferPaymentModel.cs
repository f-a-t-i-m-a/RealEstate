using System;
using System.ComponentModel.DataAnnotations;
using JahanJooy.Common.Util.Web.Attributes;
using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Models.Payment
{
    public class PaymentNewWireTransferPaymentModel
    {
        [Numeric(ErrorMessageResourceType = typeof(PaymentModelResources), ErrorMessageResourceName = "Validation_Amount_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof(PaymentModelResources), ErrorMessageResourceName = "Validation_Amount_Numeric")]
        [Range(1, 999999999, ErrorMessageResourceType = typeof(PaymentModelResources), ErrorMessageResourceName = "Validation_Amount_Range")]
        [Required(ErrorMessageResourceType = typeof(PaymentModelResources), ErrorMessageResourceName = "Validation_Amount_Required")]
        [Display(ResourceType = typeof(PaymentModelResources), Name = "Label_Amount")]
        public decimal? Amount { get; set; }

        [Display(ResourceType = typeof(PaymentModelResources), Name = "Label_SourceBank")]
        [Required(ErrorMessageResourceType = typeof(PaymentModelResources), ErrorMessageResourceName = "Validation_SourceBank_Required")]
        public IranianBank? SourceBank { get; set; }

        [StringLength(10, ErrorMessageResourceType = typeof(PaymentModelResources), ErrorMessageResourceName = "Validation_SourceCardNumberLastDigits_Length")]
        [Display(ResourceType = typeof(PaymentModelResources), Name = "Label_SourceCardNumberLastDigits")]
        public string SourceCardNumberLastDigits { get; set; }

        [StringLength(80, ErrorMessageResourceType = typeof(PaymentModelResources), ErrorMessageResourceName = "Validation_SourceAccountHolderName_Length")]
        [Display(ResourceType = typeof(PaymentModelResources), Name = "Label_SourceAccountHolderName")]
        public string SourceAccountHolderName { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(PaymentModelResources), ErrorMessageResourceName = "Validation_FollowUpNumber_Length")]
        [Display(ResourceType = typeof(PaymentModelResources), Name = "Label_FollowUpNumber")]
        public string FollowUpNumber { get; set; }

        [BindingExceptionMessage(ErrorMessageResourceType = typeof(PaymentModelResources), ErrorMessageResourceName = "Validation_UserEnteredDate_Date")]
        [DynamicDateRange(double.NaN, 0, ErrorMessageResourceType = typeof(PaymentModelResources), ErrorMessageResourceName = "Validation_UserEnteredDate_Range")]
        [Required(ErrorMessageResourceType = typeof(PaymentModelResources), ErrorMessageResourceName = "Validation_UserEnteredDate_Required")]
        [Display(ResourceType = typeof(PaymentModelResources), Name = "Label_UserEnteredDate")]
        public DateTime? UserEnteredDate { get; set; }

        [Numeric(ErrorMessageResourceType = typeof(PaymentModelResources), ErrorMessageResourceName = "Validation_UserEnteredTimeOfDay_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof(PaymentModelResources), ErrorMessageResourceName = "Validation_UserEnteredTimeOfDay_Numeric")]
        [Range(0, 1440, ErrorMessageResourceType = typeof(PaymentModelResources), ErrorMessageResourceName = "Validation_UserEnteredTimeOfDay_Range")]
        [Required(ErrorMessageResourceType = typeof(PaymentModelResources), ErrorMessageResourceName = "Validation_UserEnteredTimeOfDay_Required")]
        [Display(ResourceType = typeof(PaymentModelResources), Name = "Label_UserEnteredTimeOfDay")]
        public int? UserEnteredTimeOfDay { get; set; }

        [StringLength(500, ErrorMessageResourceType = typeof(PaymentModelResources), ErrorMessageResourceName = "Validation_UserEnteredDescription_Length")]
        [Display(ResourceType = typeof(PaymentModelResources), Name = "Label_UserEnteredDescription")]
        public string UserEnteredDescription { get; set; }


        public UserWireTransferPayment ToDomain()
        {
            var result = new UserWireTransferPayment();

            result.Amount = Amount.GetValueOrDefault();
            result.SourceBank = SourceBank.GetValueOrDefault();
            result.SourceCardNumberLastDigits = SourceCardNumberLastDigits;
            result.SourceAccountHolderName = SourceAccountHolderName;
            result.FollowUpNumber = FollowUpNumber;
            result.UserEnteredDate = UserEnteredDate.GetValueOrDefault().Date.AddMinutes(UserEnteredTimeOfDay.GetValueOrDefault());
            result.UserEnteredDescription = UserEnteredDescription;

            return result;
        }

    }
}