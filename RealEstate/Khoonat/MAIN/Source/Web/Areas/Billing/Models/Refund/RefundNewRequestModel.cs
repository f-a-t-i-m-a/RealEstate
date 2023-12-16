using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using JahanJooy.Common.Util.Web.Attributes;
using JahanJooy.RealEstate.Core.Components.Dto;
using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Web.Areas.Billing.Models.Refund
{
    [Bind(Exclude = "Balance")]
    public class RefundNewRequestModel
    {
        // Output to view

        public UserBillingBalance Balance { get; set; }

        // Input from user

        [Display(ResourceType = typeof(RefundModelResources), Name = "CheckBox_RequestedMaximumAmount")]
        public bool RequestedMaximumAmount { get; set; }


        [SkipValidationIfProperty("RequestedMaximumAmount", PropertyValidationComparisonUtil.ComparisonType.NotEquals, true)]
        [Numeric(ErrorMessageResourceType = typeof(RefundModelResources), ErrorMessageResourceName = "Validation_Amount_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof(RefundModelResources), ErrorMessageResourceName = "Validation_Amount_Numeric")]
        [Display(ResourceType = typeof(RefundModelResources), Name = "Label_Amount")]
        public decimal? Amount { get; set; }


        [StringLength(25, ErrorMessageResourceType = typeof(RefundModelResources), ErrorMessageResourceName = "Validation_TargetCardNumber_Length")]
        [Display(ResourceType = typeof(RefundModelResources), Name = "Label_TargetCardNumber")]
        public string TargetCardNumber { get; set; }


        [StringLength(80, ErrorMessageResourceType = typeof(RefundModelResources), ErrorMessageResourceName = "Validation_TargetAccountHolderName_Length")]
        [Display(ResourceType = typeof(RefundModelResources), Name = "Label_TargetAccountHolderName")]
        [Required(ErrorMessageResourceType = typeof(RefundModelResources), ErrorMessageResourceName = "Validation_TargetAccountHolderName_Required")]
        public string TargetAccountHolderName { get; set; }


        [Display(ResourceType = typeof(RefundModelResources), Name = "Label_TargetBank")]
        [Required(ErrorMessageResourceType = typeof(RefundModelResources), ErrorMessageResourceName = "Validation_TargetBank_Required")]
        public IranianBank? TargetBank { get; set; }


        [StringLength(35, ErrorMessageResourceType = typeof(RefundModelResources), ErrorMessageResourceName = "Validation_TargetShebaNumber_Length")]
        [Display(ResourceType = typeof(RefundModelResources), Name = "Label_TargetShebaNumber")]
        public string TargetShebaNumber { get; set; }


        [StringLength(500, ErrorMessageResourceType = typeof(RefundModelResources), ErrorMessageResourceName = "Validation_UserEnteredReason_Length")]
        [Display(ResourceType = typeof(RefundModelResources), Name = "Label_UserEnteredReason")]
        public string UserEnteredReason { get; set; }


        [StringLength(500, ErrorMessageResourceType = typeof(RefundModelResources), ErrorMessageResourceName = "Validation_UserEnteredDescription_Length")]
        [Display(ResourceType = typeof(RefundModelResources), Name = "Label_UserEnteredDescription")]
        public string UserEnteredDescription { get; set; }
     
    }
}