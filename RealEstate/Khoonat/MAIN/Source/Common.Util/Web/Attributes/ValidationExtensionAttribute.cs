using System;
using System.ComponentModel;
using System.Web.Mvc;

namespace JahanJooy.Common.Util.Web.Attributes
{
	[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
	public abstract class ValidationExtensionAttribute : Attribute
	{
		public abstract void OnPropertyValidated(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, object value);
		public abstract bool OnPropertyValidating(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, object value);
	}
}