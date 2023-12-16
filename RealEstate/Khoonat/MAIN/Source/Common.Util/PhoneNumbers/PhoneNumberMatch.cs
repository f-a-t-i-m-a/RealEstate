using System;

namespace JahanJooy.Common.Util.PhoneNumbers
{
	public class PhoneNumberMatch
	{
		/** The start index into the text. */
		private readonly int start;
		/** The raw substring matched. */
		private readonly string rawString;
		/** The matched phone number. */
		private readonly PhoneNumber number;

		/**
	     * Creates a new match.
	     *
	     * @param start  the start index into the target text
	     * @param rawString  the matched substring of the target text
	     * @param number  the matched phone number
	     */
		internal PhoneNumberMatch(int start, string rawString, PhoneNumber number)
		{
			if (start < 0)
			{
				throw new ArgumentException("Start index must be >= 0.");
			}
			if (rawString == null || number == null)
			{
				throw new NullReferenceException();
			}
			this.start = start;
			this.rawString = rawString;
			this.number = number;
		}

		/** Returns the phone number matched by the receiver. */

		public PhoneNumber Number
		{
			get { return number; }
		}

		/** Returns the start index of the matched phone number within the searched text. */

		public int Start
		{
			get { return start; }
		}

		/** Returns the exclusive end index of the matched phone number within the searched text. */

		public int End
		{
			get { return start + rawString.Length; }
		}

		/** Returns the raw string matched as a phone number in the searched text. */

		public string RawString
		{
			get { return rawString; }
		}

		protected bool Equals(PhoneNumberMatch other)
		{
			return start == other.start && string.Equals(rawString, other.rawString) && Equals(number, other.number);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((PhoneNumberMatch) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = start;
				hashCode = (hashCode*397) ^ (rawString != null ? rawString.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ (number != null ? number.GetHashCode() : 0);
				return hashCode;
			}
		}

		public override string ToString()
		{
			return "PhoneNumberMatch [" + Start + "," + End + ") " + rawString;
		}
	}
}