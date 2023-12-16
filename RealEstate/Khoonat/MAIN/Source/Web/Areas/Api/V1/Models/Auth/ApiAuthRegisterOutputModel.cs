using System.Collections.Generic;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Base;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Auth
{
    public class ApiAuthRegisterOutputModel : ApiOutputModelBase
    {
        public long UserID { get; set; }

        public List<ApiAuthRegisterOutputContactMethodModel> VerifyableContactMethods { get; set; }
    }
}