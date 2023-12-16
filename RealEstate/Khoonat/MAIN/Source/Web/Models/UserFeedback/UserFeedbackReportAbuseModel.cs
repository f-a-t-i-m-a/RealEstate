using System.ComponentModel.DataAnnotations;
using JahanJooy.RealEstate.Domain.Audit;
using JahanJooy.RealEstate.Web.Resources.Models.UserFeedback;

namespace JahanJooy.RealEstate.Web.Models.UserFeedback
{
	public class UserFeedbackReportAbuseModel
	{
		[Required]
		public AbuseFlagEntityType? EntityType { get; set; }

		[Required]
		public long? EntityID { get; set; }

		[Required(ErrorMessageResourceType = typeof(UserFeedbackReportAbuseModelResources), ErrorMessageResourceName = "Validation_Reason_Required")]
		[Display(ResourceType = typeof(UserFeedbackReportAbuseModelResources), Name = "Label_Reason")]
		public AbuseFlagReason? Reason { get; set; }

		[StringLength(500)]
		[Display(ResourceType = typeof(UserFeedbackReportAbuseModelResources), Name = "Label_Comments")]
		public string Comments { get; set; }
	}
}