using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Session
{
    public class ApiSessionStartInputModel : ApiInputModelBase
    {
        public string UniqueVisitorIdentifier { get; set; }

        public string UserAgent { get; set; }

        public string DeviceId { get; set; }

        public string Referer { get; set; }
    }
}