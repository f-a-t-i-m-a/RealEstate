using System.Web.Mvc;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Application;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Controllers
{
    [Authorize]
    public class MyProfileController : ApiControllerBase
    {
        [HttpPost]
        public ActionResult AddEmail()
        {
            return Success();
        }

        [HttpPost]
        public ActionResult VerifyEmail()
        {
            return Success();
        }

        [HttpPost]
        public ActionResult AddPhone()
        {
            return Success();
        }

        [HttpPost]
        public ActionResult VerifyPhone()
        {
            return Success();
        }
    }
}