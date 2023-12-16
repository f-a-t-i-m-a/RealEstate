using System.Web.Mvc;
using JahanJooy.RealEstate.Web.Application.Base;

namespace JahanJooy.RealEstate.Web.Controllers
{
	public class ErrorController : CustomControllerBase
	{
		#region HTTP errors

		public ActionResult Unknown()
		{
			SetCommonResponsePropertiesForError();

			return View("_Errors/GlobalUnknown");
		}
		
		public ActionResult BadRequest()
		{
			SetCommonResponsePropertiesForError();

			Response.StatusCode = 400;
			return View("_Errors/BadRequest");
		}
		
		public ActionResult Unauthorized()
		{
			SetCommonResponsePropertiesForError();

			Response.StatusCode = 401;
			return View("_Errors/Unauthorized");
		}
		
		public ActionResult Forbidden()
		{
			SetCommonResponsePropertiesForError();

			Response.StatusCode = 403;
			return View("_Errors/Forbidden");
		}
	
		public ActionResult NotFound()
		{
			SetCommonResponsePropertiesForError();

			Response.StatusCode = 404;
			return View("_Errors/NotFound");
		}

		public ActionResult Internal()
		{
			SetCommonResponsePropertiesForError();

			return View("_Errors/GlobalInternal");
		}

		#endregion

		#region Application errors

		public ActionResult UserDisabled()
		{
			SetCommonResponsePropertiesForError();

			Response.StatusCode = 401;
			return View("_Errors/UserDisabled");
		}

		public ActionResult AccessDenied()
		{
			SetCommonResponsePropertiesForError();

			Response.StatusCode = 401;

			return View("_Errors/AccessDenied");
		}

		public ActionResult EntityNotFound()
		{
			SetCommonResponsePropertiesForError();

			Response.StatusCode = 404;
			return View("_Errors/EntityNotFound");
		}

		#endregion

		#region Private helper methods

		private void SetCommonResponsePropertiesForError()
		{
			Server.ClearError();
			Response.TrySkipIisCustomErrors = true;
			Response.ContentType = "text/html; charset=utf-8";
		}

		#endregion
	}
}