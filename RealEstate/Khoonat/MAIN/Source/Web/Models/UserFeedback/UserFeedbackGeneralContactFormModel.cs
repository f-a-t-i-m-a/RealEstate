using System.ComponentModel.DataAnnotations;
using JahanJooy.RealEstate.Web.Resources.Models.UserFeedback;

namespace JahanJooy.RealEstate.Web.Models.UserFeedback
{
	public class UserFeedbackGeneralContactFormModel
	{
		[StringLength(2000)]
		[Display(ResourceType = typeof(UserFeedbackGeneralContactFormModelResources), Name = "Label_Comments")]
		public string Comments { get; set; }

		[Display(ResourceType = typeof(UserFeedbackGeneralContactFormModelResources), Name = "Label_Name")]
		public string Name { get; set; }

		[Display(ResourceType = typeof(UserFeedbackGeneralContactFormModelResources), Name = "Label_PhoneNumber")]
		public string PhoneNumber { get; set; }

		[Display(ResourceType = typeof(UserFeedbackGeneralContactFormModelResources), Name = "Label_EmailAddress")]
		public string EmailAddress { get; set; }
	}
}