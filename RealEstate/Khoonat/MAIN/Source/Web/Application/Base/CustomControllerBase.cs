using System;
using System.Text;
using System.Web.Mvc;
using JahanJooy.Common.Util.Web.Result;
using JahanJooy.RealEstate.Core.Security;
using JahanJooy.RealEstate.Core.Services.Dto;
using JahanJooy.RealEstate.Web.Application.Session;

namespace JahanJooy.RealEstate.Web.Application.Base
{
	public class CustomControllerBase : Controller
	{
		protected virtual new CorePrincipal User => HttpContext == null ? CorePrincipal.Anonymous : ((HttpContext.User as CorePrincipal) ?? CorePrincipal.Anonymous);

	    protected SessionInfo SessionInfo => Session.GetSessionInfo();

	    protected virtual ActionResult Error(ErrorResult error)
		{
			switch (error)
			{
				case ErrorResult.AccessDenied:
					if (Request.IsAjaxRequest())
						return PartialView("_Errors/AccessDenied");

					return new TransferResult(Url.Action("AccessDenied", "Error", new { Area = "", isAjax = Request.IsAjaxRequest() }));

				case ErrorResult.EntityNotFound:
					if (Request.IsAjaxRequest())
						return PartialView("_Errors/EntityNotFound");

					return new TransferResult(Url.Action("EntityNotFound", "Error", new { Area = "", isAjax = Request.IsAjaxRequest() }));
			}

			throw new ArgumentException("error type not implemented.");
		}

		protected new ServiceStackJsonResult Json(object data)
		{
			return Json(data, null, null);
		}

		protected new ServiceStackJsonResult Json(object data, JsonRequestBehavior behavior)
		{
			return Json(data, null, null);
		}

		protected new ServiceStackJsonResult Json(object data, string contentType)
		{
			return Json(data, contentType, null);
		}

		protected new ServiceStackJsonResult Json(object data, string contentType, JsonRequestBehavior behavior)
		{
			return Json(data, contentType, null);
		}

		protected new ServiceStackJsonResult Json(object data, string contentType, Encoding encoding)
		{
			return new ServiceStackJsonResult
			{
				Data = data,
				ContentEncoding = encoding,
				ContentType = contentType
			};
		}

		protected new ServiceStackJsonResult Json(object data, string contentType, Encoding encoding, JsonRequestBehavior behavior)
		{
			return Json(data, contentType, encoding);
		}
		
	}

	public enum ErrorResult
	{
		AccessDenied,
		EntityNotFound
	}
}