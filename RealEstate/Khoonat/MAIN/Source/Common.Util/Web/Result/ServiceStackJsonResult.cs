using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ServiceStack;

namespace JahanJooy.Common.Util.Web.Result
{
	public class ServiceStackJsonResult : ActionResult
	{
	    public const int DefaultSuccessStatusCode = 200;
	    public const int DefaultErrorStatusCode = 400;

	    public Encoding ContentEncoding { get; set; } = null;
	    public string ContentType { get; set; } = null;
	    public int? StatusCode { get; set; } = null;
	    public object Data { get; set; } = null;

		public override void ExecuteResult(ControllerContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			HttpResponseBase response = context.HttpContext.Response;
			response.ContentType = string.IsNullOrEmpty(ContentType) ? "application/json" : ContentType;
		    response.StatusCode = StatusCode ?? DefaultSuccessStatusCode;

			if (ContentEncoding != null)
				response.ContentEncoding = ContentEncoding;

			if (Data == null)
				return;

			response.Write(Data.ToJson());
		}
	}
}