using System.Reflection;
using System.Web.Mvc;

namespace JahanJooy.Common.Util.Web.Attributes.MethodSelector
{
	/// <summary>
	/// Inspired by:
	/// http://weblogs.asp.net/dfindley/archive/2009/05/31/asp-net-mvc-multiple-buttons-in-the-same-form.aspx
	/// </summary>
	public class AcceptParameterAttribute : ActionMethodSelectorAttribute
	{
		private readonly string _name;
		private readonly string _value;

		public AcceptParameterAttribute(string name, string value)
		{
			_name = name;
			_value = value;
		}

		public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
		{
			var request = controllerContext.RequestContext.HttpContext.Request;

			if (_value == null)
				return request.Form[_name] == null;

			return _value.Equals(request.Form[_name]);
		}
	}
}
