using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Spatial;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using JahanJooy.Common.Util.Web.Attributes;

namespace JahanJooy.Common.Util.Web.Validation
{
	public class ExtendedModelBinder : DefaultModelBinder
	{
		public static void Setup()
		{
			ModelBinders.Binders.DefaultBinder = new ExtendedModelBinder();
			ModelBinders.Binders.Add(typeof(DateTime), new DateTimeModelBinder());
			ModelBinders.Binders.Add(typeof(DateTime?), new DateTimeModelBinder());
            ModelBinders.Binders.Add(typeof(DbGeography), new DbGeographyModelBinder());

            ModelBinders.Binders.Add(typeof(Byte), new NumericModelBinder(s => Convert.ToByte(s, CultureInfo.InvariantCulture)));
            ModelBinders.Binders.Add(typeof(Byte?), new NumericModelBinder(s => Convert.ToByte(s, CultureInfo.InvariantCulture)));
            ModelBinders.Binders.Add(typeof(Decimal), new NumericModelBinder(s => Convert.ToDecimal(s, CultureInfo.InvariantCulture)));
            ModelBinders.Binders.Add(typeof(Decimal?), new NumericModelBinder(s => Convert.ToDecimal(s, CultureInfo.InvariantCulture)));
            ModelBinders.Binders.Add(typeof(Double), new NumericModelBinder(s => Convert.ToDouble(s, CultureInfo.InvariantCulture)));
            ModelBinders.Binders.Add(typeof(Double?), new NumericModelBinder(s => Convert.ToDouble(s, CultureInfo.InvariantCulture)));
            ModelBinders.Binders.Add(typeof(Int16), new NumericModelBinder(s => Convert.ToInt16(s, CultureInfo.InvariantCulture)));
            ModelBinders.Binders.Add(typeof(Int16?), new NumericModelBinder(s => Convert.ToInt16(s, CultureInfo.InvariantCulture)));
            ModelBinders.Binders.Add(typeof(Int32), new NumericModelBinder(s => Convert.ToInt32(s, CultureInfo.InvariantCulture)));
            ModelBinders.Binders.Add(typeof(Int32?), new NumericModelBinder(s => Convert.ToInt32(s, CultureInfo.InvariantCulture)));
            ModelBinders.Binders.Add(typeof(Int64), new NumericModelBinder(s => Convert.ToInt64(s, CultureInfo.InvariantCulture)));
            ModelBinders.Binders.Add(typeof(Int64?), new NumericModelBinder(s => Convert.ToInt64(s, CultureInfo.InvariantCulture)));
            ModelBinders.Binders.Add(typeof(SByte), new NumericModelBinder(s => Convert.ToSByte(s, CultureInfo.InvariantCulture)));
            ModelBinders.Binders.Add(typeof(SByte?), new NumericModelBinder(s => Convert.ToSByte(s, CultureInfo.InvariantCulture)));
            ModelBinders.Binders.Add(typeof(Single), new NumericModelBinder(s => Convert.ToSingle(s, CultureInfo.InvariantCulture)));
            ModelBinders.Binders.Add(typeof(Single?), new NumericModelBinder(s => Convert.ToSingle(s, CultureInfo.InvariantCulture)));
            ModelBinders.Binders.Add(typeof(UInt16), new NumericModelBinder(s => Convert.ToUInt16(s, CultureInfo.InvariantCulture)));
            ModelBinders.Binders.Add(typeof(UInt16?), new NumericModelBinder(s => Convert.ToUInt16(s, CultureInfo.InvariantCulture)));
            ModelBinders.Binders.Add(typeof(UInt32), new NumericModelBinder(s => Convert.ToUInt32(s, CultureInfo.InvariantCulture)));
            ModelBinders.Binders.Add(typeof(UInt32?), new NumericModelBinder(s => Convert.ToUInt32(s, CultureInfo.InvariantCulture)));
            ModelBinders.Binders.Add(typeof(UInt64), new NumericModelBinder(s => Convert.ToUInt64(s, CultureInfo.InvariantCulture)));
            ModelBinders.Binders.Add(typeof(UInt64?), new NumericModelBinder(s => Convert.ToUInt64(s, CultureInfo.InvariantCulture)));
		}

		protected override void SetProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, object value)
		{
			base.SetProperty(controllerContext, bindingContext, propertyDescriptor, value);

            ModelMetadata propertyMetadata = bindingContext.PropertyMetadata[propertyDescriptor.Name];
			string propertyName = CreateSubPropertyName(bindingContext.ModelName, propertyMetadata.PropertyName);

			if (bindingContext.ModelState[propertyName] == null)
				return;

			foreach (var error in bindingContext.ModelState[propertyName].Errors.Where(e => e != null))
			{
				if (propertyDescriptor.Attributes[typeof (BindingExceptionMessageAttribute)] == null) continue;

				string errorMessage = ((BindingExceptionMessageAttribute) propertyDescriptor.Attributes[typeof (BindingExceptionMessageAttribute)]).GetErrorMessage();
				bindingContext.ModelState[propertyName].Errors.Remove(error);
				bindingContext.ModelState[propertyName].Errors.Add(errorMessage);
				break;
			}
		}

		protected override void OnPropertyValidated(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, object value)
		{
			base.OnPropertyValidated(controllerContext, bindingContext, propertyDescriptor, value);

			foreach (var attribute in propertyDescriptor.Attributes.OfType<ValidationExtensionAttribute>())
				attribute.OnPropertyValidated(controllerContext, bindingContext, propertyDescriptor, value);
		}

		protected override bool OnPropertyValidating(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, object value)
		{
			if (!base.OnPropertyValidating(controllerContext, bindingContext, propertyDescriptor, value))
				return false;

			return propertyDescriptor.Attributes.OfType<ValidationExtensionAttribute>().All(a => a.OnPropertyValidating(controllerContext, bindingContext, propertyDescriptor, value));
		}

		/// <summary>
		/// The method is copied from base class (base.OnModelUpdated(controllerContext, bindingContext))
		/// and modified. Modified region is commented in the method source code.
		/// </summary>
		protected override void OnModelUpdated(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			var startedValid = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

			foreach (ModelValidationResult validationResult in ModelValidator.GetModelValidator(bindingContext.ModelMetadata, controllerContext).Validate(null))
			{
				// START added code

				var propertyMetadata = bindingContext.PropertyMetadata[validationResult.MemberName];
				var customAttributes = bindingContext.ModelType.GetProperty(propertyMetadata.PropertyName)
					.GetCustomAttributes(typeof (SkipValidationIfAttribute), true)
					.Cast<SkipValidationIfAttribute>();
				
				if (customAttributes.Any(a => a.ShouldSkipValidation(controllerContext, bindingContext, propertyMetadata, validationResult)))
					continue;

				// END added code

				string subPropertyName = CreateSubPropertyName(bindingContext.ModelName, validationResult.MemberName);

				if (!startedValid.ContainsKey(subPropertyName))
				{
					startedValid[subPropertyName] = bindingContext.ModelState.IsValidField(subPropertyName);
				}

				if (startedValid[subPropertyName])
				{
					bindingContext.ModelState.AddModelError(subPropertyName, validationResult.Message);
				}
			}
		}

	}
}