using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstate.Core.Services.Dto.Property;

namespace JahanJooy.RealEstate.Web.Models.Property
{
	public class PropertyQuickCreateCompletedModel
	{
		public PropertyListingDetails Listing { get; set; }
		public string ListingEditPassword { get; set; }
		public PropertyListingSummary ListingSummary { get; set; }
		public ValidationResult PublishValidationResult { get; set; }
	}
}