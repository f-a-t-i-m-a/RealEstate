using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstate.Domain.Enums;

namespace JahanJooy.RealEstate.Web.Models.Property
{
	public abstract class PropertySectionModelBase
	{
		public long PropertyListingID { get; private set; }
		public long? PropertyListingCode { get; private set; }
		public PropertyType? PropertyType { get; set; }
		public IntentionOfOwner? IntentionOfOwner { get; set; }

		public bool ShowAllAttributes { get; set; }
		public ValidationResult PublishValidationResult { get; set; }

		protected PropertySectionModelBase(long propertyListingID, long? propertyListingCode = null, PropertyType? propertyType = null, IntentionOfOwner? intentionOfOwner = null)
		{
			PropertyListingID = propertyListingID;
			PropertyListingCode = propertyListingCode;
			PropertyType = propertyType;
			IntentionOfOwner = intentionOfOwner;
		}
	}
}