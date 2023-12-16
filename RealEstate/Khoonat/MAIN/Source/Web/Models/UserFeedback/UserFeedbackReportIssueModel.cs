using System.ComponentModel.DataAnnotations;
using JahanJooy.RealEstate.Web.Resources.Models.UserFeedback;

namespace JahanJooy.RealEstate.Web.Models.UserFeedback
{
	public class UserFeedbackReportIssueModel
	{
		[StringLength(500)]
		[Display(ResourceType = typeof(UserFeedbackReportIssueModelResources), Name = "Label_ErrorMessage")]
		public string ErrorMessage { get; set; }

		[StringLength(2000)]
		[Display(ResourceType = typeof(UserFeedbackReportIssueModelResources), Name = "Label_ErrorConditions")]
		public string ErrorConditions { get; set; }

		[StringLength(500)]
		[Display(ResourceType = typeof(UserFeedbackReportIssueModelResources), Name = "Label_Where")]
		public string Where { get; set; }

		[StringLength(2000)]
		[Display(ResourceType = typeof(UserFeedbackReportIssueModelResources), Name = "Label_StepsToReproduce")]
		public string StepsToReproduce { get; set; }

		[StringLength(2000)]
		[Display(ResourceType = typeof(UserFeedbackReportIssueModelResources), Name = "Label_Comments")]
		public string Comments { get; set; }

		[StringLength(500)]
		[Display(ResourceType = typeof(UserFeedbackReportIssueModelResources), Name = "Label_Browser")]
		public string Browser { get; set; }

		[StringLength(500)]
		[Display(ResourceType = typeof(UserFeedbackReportIssueModelResources), Name = "Label_BrowserVersion")]
		public string BrowserVersion { get; set; }

		[StringLength(500)]
		[Display(ResourceType = typeof(UserFeedbackReportIssueModelResources), Name = "Label_OperatingSystem")]
		public string OperatingSystem { get; set; }

		[StringLength(500)]
		[Display(ResourceType = typeof(UserFeedbackReportIssueModelResources), Name = "Label_OperatingSystemVersion")]
		public string OperatingSystemVersion { get; set; }

		[StringLength(500)]
		[Display(ResourceType = typeof(UserFeedbackReportIssueModelResources), Name = "Label_DateAndTime")]
		public string DateAndTime { get; set; }

		[StringLength(500)]
		[Display(ResourceType = typeof(UserFeedbackReportIssueModelResources), Name = "Label_LoggedOnUser")]
		public string LoggedOnUser { get; set; }

		[StringLength(500)]
		[Display(ResourceType = typeof(UserFeedbackReportIssueModelResources), Name = "Label_Name")]
		public string Name { get; set; }

		[StringLength(500)]
		[Display(ResourceType = typeof(UserFeedbackReportIssueModelResources), Name = "Label_PhoneNumber")]
		public string PhoneNumber { get; set; }

		[StringLength(500)]
		[Display(ResourceType = typeof(UserFeedbackReportIssueModelResources), Name = "Label_EmailAddress")]
		public string EmailAddress { get; set; }
	}
}