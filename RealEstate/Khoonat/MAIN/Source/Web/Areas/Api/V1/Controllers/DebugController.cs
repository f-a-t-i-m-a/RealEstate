using System.Web.Mvc;
using JahanJooy.Common.Util;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Application;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Debug;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Controllers
{
	public class DebugController : ApiControllerBase
	{
		[HttpPost]
		public ActionResult Echo(ApiDebugEchoInputModel input)
		{
			var output = new ApiDebugEchoOutputModel
			             {
				             Response = "ECHO of message: " + input.IfNotNull(m => m.Message ?? "<NULL>", "<NULL INPUT>")
			             };
			return Json(output);
		}
	}
}