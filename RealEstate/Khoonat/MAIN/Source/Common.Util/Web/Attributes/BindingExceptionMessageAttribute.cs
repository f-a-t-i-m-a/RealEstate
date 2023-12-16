using System;
using System.Reflection;

namespace JahanJooy.Common.Util.Web.Attributes
{
	[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
	public class BindingExceptionMessageAttribute : Attribute
	{
		public string ErrorMessage { get; set; }
		public string ErrorMessageResourceName { get; set; }
		public Type ErrorMessageResourceType { get; set; }

		public string GetErrorMessage()
		{
			if (string.IsNullOrEmpty(ErrorMessage))
			{
				PropertyInfo prop = ErrorMessageResourceType.GetProperty(ErrorMessageResourceName);
				return prop.GetValue(null, null).ToString();
			}

			return ErrorMessage;
		}
	}
}