using System;
using System.Web.Mvc;
using JahanJooy.Common.Util.Localization;

namespace JahanJooy.Common.Util.Web.Validation
{
	public class DateTimeModelBinder : IModelBinder
	{
		#region Implementation of IModelBinder

		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
			if (value == null)
				return null;

			bindingContext.ModelState.SetModelValue(bindingContext.ModelName, value);

			var stringValue = value.AttemptedValue;
			if (string.IsNullOrEmpty(stringValue))
				return null;

			DateTime? result = DateTimeLocalizationUtils.FromLocalizedDateString(stringValue);

			if (!result.HasValue)
				bindingContext.ModelState.AddModelError(bindingContext.ModelName, "The input string '" + stringValue + "' is not a valid date value.");

			return result;
		}

		#endregion
	}
}