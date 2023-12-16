using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Property
{
    public class ApiPropertyGetPhotosInputModel : ApiInputPaginatingModelBase
    {
        public long ListingID { get; set; }
		public bool IncludeThumbnailFile { get; set; }
		public bool IncludeMediumSizeFile { get; set; }
    }
}