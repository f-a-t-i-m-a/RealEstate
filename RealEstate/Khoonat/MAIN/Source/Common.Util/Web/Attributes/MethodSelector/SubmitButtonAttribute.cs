using System.Reflection;
using System.Web.Mvc;

namespace JahanJooy.Common.Util.Web.Attributes.MethodSelector
{
	public class SubmitButtonAttribute : ActionMethodSelectorAttribute
	{
		private readonly string _buttonName;

		public SubmitButtonAttribute(string buttonName)
		{
			_buttonName = buttonName;
		}

		public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
		{
			var request = controllerContext.RequestContext.HttpContext.Request;
			var buttonPostbackValue = request.Form[_buttonName];

			return !string.IsNullOrWhiteSpace(buttonPostbackValue);
		}
	}
}