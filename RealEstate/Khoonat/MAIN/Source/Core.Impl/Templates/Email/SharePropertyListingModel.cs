using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Util.Presentation.Property;

namespace JahanJooy.RealEstate.Core.Impl.Templates.Email
{
    public class SharePropertyListingModel
	{
		public PropertyPresentationHelper PropertyPresentationHelper { get; set; }

		public PropertyListingDetails Listing { get; set; }
		public PropertyListingSummary ListingSummary { get; set; }

        public string ReceiverName { get; set; }
        public string Description { get; set; }
        public string SenderDisplayName { get; set; }
        public long? SenderID { get; set; }

	}
}