using System;
using System.Web.Mvc;

namespace JahanJooy.Common.Util.Web.Attributes
{
	[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
	public class SkipValidationIfValueEqualsAttribute : SkipValidationIfAttribute
	{
		private readonly object _skipValue;

		public SkipValidationIfValueEqualsAttribute(object skipValue)
		{
			_skipValue = skipValue;
		}

		#region Overrides of SkipValidationIfAttribute

		public override bool ShouldSkipValidation(ControllerContext controllerContext, ModelBindingContext bindingContext, ModelMetadata propertyMetadata, ModelValidationResult validationResult)
		{
			object value = propertyMetadata.Model;

			if (_skipValue == null)
			{
				return value == null;
			}

			return _skipValue.Equals(value);
		}

		#endregion
	}
}