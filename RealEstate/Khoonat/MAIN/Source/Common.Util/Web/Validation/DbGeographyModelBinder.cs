using System;
using System.Data.Entity.Spatial;
using System.Web.Mvc;

namespace JahanJooy.Common.Util.Web.Validation
{
	public class DbGeographyModelBinder : IModelBinder
	{
		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			ValueProviderResult valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
			var modelState = new ModelState { Value = valueResult };

			if (string.IsNullOrWhiteSpace(valueResult.AttemptedValue))
				return null;

			object actualValue = null;
			try
			{
				actualValue = DbGeography.FromText(valueResult.AttemptedValue);
			}
			catch (Exception e)
			{
				modelState.Errors.Add(e);
			}

			bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
			return actualValue;
		}
	}
}