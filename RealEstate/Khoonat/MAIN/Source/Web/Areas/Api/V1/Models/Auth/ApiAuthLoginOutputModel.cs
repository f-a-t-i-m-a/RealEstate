using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Auth
{
    public class ApiAuthLoginOutputModel : ApiOutputModelBase
    {
        public string Token { get; set; }
    }
}