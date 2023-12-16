using System.ComponentModel.DataAnnotations;
using JahanJooy.RealEstate.Web.Resources.Models.UserFeedback;

namespace JahanJooy.RealEstate.Web.Models.UserFeedback
{
	public class UserFeedbackRequestNeighborhoodModel
	{
		[Display(ResourceType = typeof(UserFeedbackRequestNeighborhoodModelResources), Name = "Label_NeighborhoodName")]
		public string NeighborhoodName { get; set; }

		[Display(ResourceType = typeof(UserFeedbackRequestNeighborhoodModelResources), Name = "Label_CityRegionName")]
		public string CityRegionName { get; set; }

		[Display(ResourceType = typeof(UserFeedbackRequestNeighborhoodModelResources), Name = "Label_CityName")]
		public string CityName { get; set; }

		[StringLength(2000)]
		[Display(ResourceType = typeof(UserFeedbackRequestNeighborhoodModelResources), Name = "Label_Comments")]
		public string Comments { get; set; }

		[Display(ResourceType = typeof(UserFeedbackRequestNeighborhoodModelResources), Name = "Label_Name")]
		public string Name { get; set; }

		[Display(ResourceType = typeof(UserFeedbackRequestNeighborhoodModelResources), Name = "Label_PhoneNumber")]
		public string PhoneNumber { get; set; }

		[Display(ResourceType = typeof(UserFeedbackRequestNeighborhoodModelResources), Name = "Label_EmailAddress")]
		public string EmailAddress { get; set; }
	}
}