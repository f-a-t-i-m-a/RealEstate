using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Property
{
    public class ApiPropertyGetContactInfoInputModel : ApiInputPaginatingModelBase
    {
        public long ListingID { get; set; }
    }
}