using JahanJooy.RealEstate.Domain.Enums;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Auth
{
    public class ApiAuthRegisterOutputContactMethodModel
    {
        public long ID { get; set; }
        public ContactMethodType ContactMethodType { get; set; }
        public string ContactMethodText { get; set; }
    }
}