using System;
using System.Reflection;
using System.Web.Mvc;

namespace JahanJooy.Common.Util.Web.Attributes
{
	[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
	public class SkipValidationIfPropertyAttribute : SkipValidationIfAttribute
	{
		private readonly string _propertyName;
		private readonly PropertyValidationComparisonUtil.ComparisonType _comparisonType;
		private readonly object _targetValue;

		public SkipValidationIfPropertyAttribute(string propertyName, PropertyValidationComparisonUtil.ComparisonType comparisonType, object targetValue)
		{
			_propertyName = propertyName;
			_comparisonType = comparisonType;
			_targetValue = targetValue;

			if (propertyName == null)
				throw new ArgumentNullException("propertyName");
		}

		#region Overrides of SkipValidationIfAttribute

		public override bool ShouldSkipValidation(ControllerContext controllerContext, ModelBindingContext bindingContext, ModelMetadata propertyMetadata, ModelValidationResult validationResult)
		{
			PropertyInfo modelPropInfo = bindingContext.ModelType.GetProperty(_propertyName);
			if (modelPropInfo == null)
				throw new InvalidOperationException("Could not find property " + _propertyName + " on type " + bindingContext.ModelType.Name);

			object modelPropValue = modelPropInfo.GetValue(bindingContext.Model, null);
			var result = PropertyValidationComparisonUtil.Compare(modelPropValue, _comparisonType, _targetValue);
			return !result.HasValue || result.Value;
		}

		#endregion
	}

}