﻿using System;

namespace JahanJooy.Common.Util.PhoneNumbers
{
	public class PhoneNumberParseException : Exception
	{
		public enum ParseErrorType
		{
			INVALID_COUNTRY_CODE,
			// This generally indicates the string passed in had less than 3 digits in it. More
			// specifically, the number failed to match the regular expression VALID_PHONE_NUMBER in
			// PhoneNumberUtil.java.
			NOT_A_NUMBER,
			// This indicates the string started with an international dialing prefix, but after this was
			// stripped from the number, had less digits than any valid phone number (including country
			// code) could have.
			TOO_SHORT_AFTER_IDD,
			// This indicates the string, after any country code has been stripped, had less digits than any
			// valid phone number could have.
			TOO_SHORT_NSN,
			// This indicates the string had more digits than any valid phone number could have.
			TOO_LONG,
		}

		private ParseErrorType errorType;
		private string message;

		public PhoneNumberParseException(ParseErrorType errorType, String message)
			: base(message)
		{
			this.message = message;
			this.errorType = errorType;
		}

		/**
	     * Returns the error type of the exception that has been thrown.
	     */
		public ParseErrorType ErrorType
		{
			get { return errorType; }
		}

		public override string ToString()
		{
			return "Error type: " + errorType + ". " + message;
		}
	}
}