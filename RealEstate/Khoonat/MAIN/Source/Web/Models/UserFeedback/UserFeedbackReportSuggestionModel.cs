using System.ComponentModel.DataAnnotations;
using JahanJooy.RealEstate.Web.Resources.Models.UserFeedback;

namespace JahanJooy.RealEstate.Web.Models.UserFeedback
{
	public class UserFeedbackReportSuggestionModel
	{
		[StringLength(2000)]
		[Display(ResourceType = typeof(UserFeedbackReportSuggestionModelResources), Name = "Label_SuggestionDetails")]
		public string SuggestionDetails { get; set; }

		[StringLength(2000)]
		[Display(ResourceType = typeof(UserFeedbackReportSuggestionModelResources), Name = "Label_AffectedUsers")]
		public string AffectedUsers { get; set; }

		[StringLength(2000)]
		[Display(ResourceType = typeof(UserFeedbackReportSuggestionModelResources), Name = "Label_EffectsOnUsers")]
		public string EffectsOnUsers { get; set; }

		[StringLength(500)]
		[Display(ResourceType = typeof(UserFeedbackReportSuggestionModelResources), Name = "Label_Name")]
		public string Name { get; set; }

		[StringLength(500)]
		[Display(ResourceType = typeof(UserFeedbackReportSuggestionModelResources), Name = "Label_PhoneNumber")]
		public string PhoneNumber { get; set; }

		[StringLength(500)]
		[Display(ResourceType = typeof(UserFeedbackReportSuggestionModelResources), Name = "Label_EmailAddress")]
		public string EmailAddress { get; set; }
	}
}