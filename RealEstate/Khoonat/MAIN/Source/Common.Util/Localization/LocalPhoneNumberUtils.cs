using JahanJooy.Common.Util.PhoneNumbers;
using JahanJooy.Common.Util.Validation;

namespace JahanJooy.Common.Util.Localization
{
	public static class LocalPhoneNumberUtils
	{
		private const string NationalLocaleSymbol = "IR";
        private const int NationalLocaleCode = 98;
        private const string DefaultCityPrefix = "021";

	    public const string ValidationErrorInputIsNullOrEmpty = "InputIsNullOrEmpty";
        public const string ValidationErrorInputIsNotAPossiblePhoneNumber = "InputIsNotAPossiblePhoneNumber";
        public const string ValidationErrorInputIsNotAValidPhoneNumber = "InputIsNotAValidPhoneNumber";
        public const string ValidationErrorInputIsNotANationalNumber = "InputIsNotANationalNumber";
        public const string ValidationErrorInputIsNotAValidSmsTarget = "InputIsNotAValidSmsTarget";
		
		public static bool IsPossibleNumber(string input)
		{
			try
			{
				var phoneNumberUtil = PhoneNumberUtil.getInstance();
				if (phoneNumberUtil.isPossibleNumber(input, NationalLocaleSymbol))
					return true;

				return phoneNumberUtil.isPossibleNumber(DefaultCityPrefix + input, NationalLocaleSymbol);
			}
			catch (PhoneNumberParseException)
			{
				return false;
			}
		}

		public static PhoneNumber Parse(string input)
		{
			try
			{
				var phoneNumberUtil = PhoneNumberUtil.getInstance();
				var originalResult = phoneNumberUtil.parse(input, NationalLocaleSymbol);

				if (phoneNumberUtil.isValidNumber(originalResult))
					return originalResult;

				var prefixedResult = phoneNumberUtil.parse(DefaultCityPrefix + input, NationalLocaleSymbol);
				if (phoneNumberUtil.isValidNumber(prefixedResult))
					return prefixedResult;

				return originalResult;
			}
			catch (PhoneNumberParseException)
			{
				return null;
			}
		}

		public static PhoneNumber ParseAndValidate(string input)
		{
			try
			{
				var phoneNumberUtil = PhoneNumberUtil.getInstance();
				var originalResult = phoneNumberUtil.parse(input, NationalLocaleSymbol);

				if (phoneNumberUtil.isValidNumber(originalResult))
					return originalResult;

				var prefixedResult = phoneNumberUtil.parse(DefaultCityPrefix + input, NationalLocaleSymbol);
				if (phoneNumberUtil.isValidNumber(prefixedResult))
					return prefixedResult;

				return null;
			}
			catch (PhoneNumberParseException)
			{
				return null;
			}
		}

		public static string Format(PhoneNumber input)
		{
			if (input == null)
				return null;

			var phoneNumberUtil = PhoneNumberUtil.getInstance();
			return phoneNumberUtil.format(input, PhoneNumberFormat.INTERNATIONAL);
		}

		public static PhoneNumberType GetNumberType(PhoneNumber input)
		{
			if (input == null)
				return PhoneNumberType.UNKNOWN;

			var phoneNumberUtil = PhoneNumberUtil.getInstance();
			return phoneNumberUtil.getNumberType(input);
		}

	    public static ValidatedResult<string> ValidateAndFormat(string number, bool allowNationalNumberOnly, bool allowSmsTargetOnly)
	    {
	        if (string.IsNullOrWhiteSpace(number))
	            return ValidatedResult<string>.Failure(ValidationErrorInputIsNullOrEmpty);

            if (!IsPossibleNumber(number))
                return ValidatedResult<string>.Failure(ValidationErrorInputIsNotAPossiblePhoneNumber);

            var parsedNumber = ParseAndValidate(number);
            if (parsedNumber == null)
                return ValidatedResult<string>.Failure(ValidationErrorInputIsNotAValidPhoneNumber);

	        if (allowNationalNumberOnly && !IsNationalNumber(parsedNumber))
	            return ValidatedResult<string>.Failure(ValidationErrorInputIsNotANationalNumber);

            if (allowSmsTargetOnly && !CanReceiveSms(parsedNumber))
                return ValidatedResult<string>.Failure(ValidationErrorInputIsNotAValidSmsTarget);

	        return ValidatedResult<string>.Success(Format(parsedNumber));
	    }

	    public static bool IsNationalNumber(PhoneNumber phoneNumber)
	    {
	        return phoneNumber.CountryCode == NationalLocaleCode;
	    }

	    public static bool CanReceiveSms(PhoneNumber phoneNumber)
	    {
	        var phoneNumberType = GetNumberType(phoneNumber);

	        return phoneNumberType == PhoneNumberType.MOBILE ||
	               phoneNumberType == PhoneNumberType.FIXED_LINE_OR_MOBILE;
	    }
	}
}