using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Session
{
    public class ApiSessionStartOutputModel : ApiOutputModelBase
    {
        public string UniqueVisitorIdentifier { get; set; }
        public string HttpSessionIdentifier { get; set; }

    }
}