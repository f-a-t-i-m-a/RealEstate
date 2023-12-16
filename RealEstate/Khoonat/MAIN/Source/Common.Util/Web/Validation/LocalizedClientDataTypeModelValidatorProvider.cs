using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using JahanJooy.Common.Util.Web.Attributes;

namespace JahanJooy.Common.Util.Web.Validation
{
	public class LocalizedClientDataTypeModelValidatorProvider : AssociatedValidatorProvider
	{
		private static readonly HashSet<Type> numericTypes = new HashSet<Type>(new[] {
			typeof (byte), typeof (sbyte),
			typeof (short), typeof (ushort),
			typeof (int), typeof (uint),
			typeof (long), typeof (ulong),
			typeof (float), typeof (double), typeof (decimal)
		});

		public static void Setup()
		{
			var cdProvider = ModelValidatorProviders.Providers.SingleOrDefault(p => p.GetType().Equals(typeof(ClientDataTypeModelValidatorProvider)));
			if (cdProvider != null)
			{
				ModelValidatorProviders.Providers.Remove(cdProvider);
				ModelValidatorProviders.Providers.Add(
						new LocalizedClientDataTypeModelValidatorProvider());
			}
		}

		protected override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context, IEnumerable<Attribute> attributes)
		{
			if (metadata == null)
			{
				throw new ArgumentNullException("metadata");
			}
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}

			Type type = metadata.ModelType;
			NumericAttribute numericAttribute = attributes.OfType<NumericAttribute>().SingleOrDefault();
			if (IsNumericType(type))
			{
				yield return new NumericModelValidator(metadata, context, numericAttribute);
			}
		}

		private static bool IsNumericType(Type type)
		{
			Type underlyingType = Nullable.GetUnderlyingType(type);
			return numericTypes.Contains(underlyingType ?? type);
		}

		#region Nested type: NumericModelValidator

		internal sealed class NumericModelValidator : ModelValidator
		{
			private readonly NumericAttribute _numericAttribute;

			public NumericModelValidator(ModelMetadata metadata, ControllerContext controllerContext, NumericAttribute numericAttribute)
				: base(metadata, controllerContext)
			{
				_numericAttribute = numericAttribute;
			}

			public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
			{
				var rule = new ModelClientValidationRule {
					ValidationType = "number",
					ErrorMessage = MakeErrorString(Metadata.GetDisplayName())
				};

				return new[] {rule};
			}

			private string MakeErrorString(string displayName)
			{
				if (_numericAttribute != null)
					return _numericAttribute.GetErrorMessage();

				return "Value of " + displayName + " field should be number.";
			}

			public override IEnumerable<ModelValidationResult> Validate(object container)
			{
				return Enumerable.Empty<ModelValidationResult>();
			}
		}

		#endregion
	}
}

