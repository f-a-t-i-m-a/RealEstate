using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstate.Core.Services.Dto.Property;

namespace JahanJooy.RealEstate.Web.Models.Property
{
	public class PropertyPublishModel
	{
		public PropertyListingSummary Listing { get; set; }
		public ValidationResult ValidationResult { get; set; }
	}
}