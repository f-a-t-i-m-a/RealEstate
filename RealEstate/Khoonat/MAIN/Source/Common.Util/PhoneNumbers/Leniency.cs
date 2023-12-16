using System.Text;

namespace JahanJooy.Common.Util.PhoneNumbers
{
    public abstract class Leniency
    {
        public static readonly Leniency POSSIBLE = new PossibleLeniency(1);
        public static readonly Leniency VALID = new ValidLeniency(2);
        public static readonly Leniency STRICT_GROUPING = new StrictGroupingLeniency(3);
        public static readonly Leniency EXACT_GROUPING = new ExactGroupingLeniency(4);

        /** Returns true if {@code number} is a verified number according to this leniency. */
        public abstract bool verify(PhoneNumber number, string candidate, PhoneNumberUtil util);

        private readonly int _ordinal;
        protected internal Leniency(int ordinal)
        {
            _ordinal = ordinal;
        }

        public int compareTo(Leniency other)
        {
            return _ordinal - other._ordinal;
        }

        /**
         * Phone numbers accepted are {@linkplain PhoneNumberUtil#isPossibleNumber(PhoneNumber)
         * possible}, but not necessarily {@linkplain PhoneNumberUtil#isValidNumber(PhoneNumber) valid}.
         */
        private class PossibleLeniency : Leniency
        {
            public PossibleLeniency(int ordinal) : base(ordinal)
            {
            }

            public override bool verify(PhoneNumber number, string candidate, PhoneNumberUtil util)
            {
                return util.isPossibleNumber(number);
            }
        }

        /**
         * Phone numbers accepted are {@linkplain PhoneNumberUtil#isPossibleNumber(PhoneNumber)
         * possible} and {@linkplain PhoneNumberUtil#isValidNumber(PhoneNumber) valid}. Numbers written
         * in national format must have their national-prefix present if it is usually written for a
         * number of this type.
         */
        private class ValidLeniency : Leniency
        {
            public ValidLeniency(int ordinal) : base(ordinal)
            {
            }

            public override bool verify(PhoneNumber number, string candidate, PhoneNumberUtil util)
            {
                if (!util.isValidNumber(number) ||
                    !PhoneNumberMatchEnumerator.containsOnlyValidXChars(number, candidate, util))
                {
                    return false;
                }

                return PhoneNumberMatchEnumerator.isNationalPrefixPresentIfRequired(number, util);
            }
        }

        /**
         * Phone numbers accepted are {@linkplain PhoneNumberUtil#isValidNumber(PhoneNumber) valid} and
         * are grouped in a possible way for this locale. For example, a US number written as
         * "65 02 53 00 00" and "650253 0000" are not accepted at this leniency level, whereas
         * "650 253 0000", "650 2530000" or "6502530000" are.
         * Numbers with more than one '/' symbol are also dropped at this level.
         * <p>
         * Warning: This level might result in lower coverage especially for regions outside of country
         * code "+1". If you are not sure about which level to use, email the discussion group
         * libphonenumber-discuss@googlegroups.com.
         */
        private class StrictGroupingLeniency : Leniency
        {
            public StrictGroupingLeniency(int ordinal) : base(ordinal)
            {
            }

            public override bool verify(PhoneNumber number, string candidate, PhoneNumberUtil util)
            {
                if (!util.isValidNumber(number) ||
                    !PhoneNumberMatchEnumerator.containsOnlyValidXChars(number, candidate, util) ||
                    PhoneNumberMatchEnumerator.containsMoreThanOneSlash(candidate) ||
                    !PhoneNumberMatchEnumerator.isNationalPrefixPresentIfRequired(number, util))
                {
                    return false;
                }

                return PhoneNumberMatchEnumerator.checkNumberGroupingIsValid(number, candidate, util, PhoneNumberMatchEnumerator.allNumberGroupsRemainGrouped);
            }
        }

        /**
         * Phone numbers accepted are {@linkplain PhoneNumberUtil#isValidNumber(PhoneNumber) valid} and
         * are grouped in the same way that we would have formatted it, or as a single block. For
         * example, a US number written as "650 2530000" is not accepted at this leniency level, whereas
         * "650 253 0000" or "6502530000" are.
         * Numbers with more than one '/' symbol are also dropped at this level.
         * <p>
         * Warning: This level might result in lower coverage especially for regions outside of country
         * code "+1". If you are not sure about which level to use, email the discussion group
         * libphonenumber-discuss@googlegroups.com.
         */
        private class ExactGroupingLeniency : Leniency
        {
            public ExactGroupingLeniency(int ordinal) : base(ordinal)
            {
            }

            public override bool verify(PhoneNumber number, string candidate, PhoneNumberUtil util)
            {
                if (!util.isValidNumber(number) ||
                    !PhoneNumberMatchEnumerator.containsOnlyValidXChars(number, candidate, util) ||
                    PhoneNumberMatchEnumerator.containsMoreThanOneSlash(candidate) ||
                    !PhoneNumberMatchEnumerator.isNationalPrefixPresentIfRequired(number, util))
                {
                    return false;
                }

                return PhoneNumberMatchEnumerator.checkNumberGroupingIsValid(number, candidate, util, PhoneNumberMatchEnumerator.allNumberGroupsAreExactlyPresent);
            }
        }

    }
}