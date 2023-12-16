using System;
using System.Text.RegularExpressions;
using JahanJooy.Common.Util.Text;

namespace JahanJooy.Common.Util.PhoneNumbers
{
    public class ShortPhoneNumberUtil
    {
        private readonly PhoneNumberUtil phoneUtil;

        public ShortPhoneNumberUtil()
        {
            phoneUtil = PhoneNumberUtil.getInstance();
        }

        // @VisibleForTesting
        internal ShortPhoneNumberUtil(PhoneNumberUtil util)
        {
            phoneUtil = util;
        }

        /**
        * Returns true if the number might be used to connect to an emergency service in the given
        * region.
        *
        * This method takes into account cases where the number might contain formatting, or might have
        * additional digits appended (when it is okay to do that in the region specified).
        *
        * @param number  the phone number to test
        * @param regionCode  the region where the phone number is being dialed
        * @return  if the number might be used to connect to an emergency service in the given region.
        */

        public bool connectsToEmergencyNumber(String number, String regionCode)
        {
            return matchesEmergencyNumberHelper(number, regionCode, true /* allows prefix match */);
        }

        /**
        * Returns true if the number exactly matches an emergency service number in the given region.
        *
        * This method takes into account cases where the number might contain formatting, but doesn't
        * allow additional digits to be appended.
        *
        * @param number  the phone number to test
        * @param regionCode  the region where the phone number is being dialed
        * @return  if the number exactly matches an emergency services number in the given region.
        */

        public bool isEmergencyNumber(String number, String regionCode)
        {
            return matchesEmergencyNumberHelper(number, regionCode, false /* doesn't allow prefix match */);
        }

        private bool matchesEmergencyNumberHelper(String number, String regionCode,
                                                  bool allowPrefixMatch)
        {
            number = PhoneNumberUtil.extractPossibleNumber(number);
            if (PhoneNumberUtil.PLUS_CHARS_PATTERN.Match(number).SuccessInputStart())
            {
                // Returns false if the number starts with a plus sign. We don't believe dialing the country
                // code before emergency numbers (e.g. +1911) works, but later, if that proves to work, we can
                // add additional logic here to handle it.
                return false;
            }

            PhoneMetaData metadata = phoneUtil.getMetadataForRegion(regionCode);
            if (metadata == null || !metadata.HasEmergency)
            {
                return false;
            }

            var emergencyNumberPattern = new Regex(metadata.Emergency.NationalNumberPattern);
            String normalizedNumber = PhoneNumberUtil.normalizeDigitsOnly(number);
            // In Brazil, it is impossible to append additional digits to an emergency number to dial the
            // number.
            return (!allowPrefixMatch || regionCode.Equals("BR"))
                       ? emergencyNumberPattern.IsMatchWhole(normalizedNumber)
                       : emergencyNumberPattern.IsMatchStart(normalizedNumber);
        }
    }
}