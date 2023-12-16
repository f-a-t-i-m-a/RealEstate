using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using JahanJooy.Common.Util.Text;

namespace JahanJooy.Common.Util.PhoneNumbers
{
    public class AsYouTypeFormatter
    {
        private string currentOutput = "";
        private StringBuilder formattingTemplate = new StringBuilder();
        // The pattern from numberFormat that is currently used to create formattingTemplate.
        private string currentFormattingPattern = "";
        private StringBuilder accruedInput = new StringBuilder();
        private StringBuilder accruedInputWithoutFormatting = new StringBuilder();
        // This indicates whether AsYouTypeFormatter is currently doing the formatting.
        private bool ableToFormat = true;
        // Set to true when users enter their own formatting. AsYouTypeFormatter will do no formatting at
        // all when this is set to true.
        private bool inputHasFormatting = false;
        private bool isInternationalFormatting = false;
        private bool isExpectingCountryCallingCode = false;
        private readonly PhoneNumberUtil phoneUtil = PhoneNumberUtil.getInstance();
        private string defaultCountry;

        private static readonly PhoneMetaData EMPTY_METADATA =
            new PhoneMetaData().setInternationalPrefix("NA");

        private PhoneMetaData defaultMetaData;
        private PhoneMetaData currentMetaData;

        // A pattern that is used to match character classes in regular expressions. An example of a
        // character class is [1-4].
        private static readonly Regex CHARACTER_CLASS_PATTERN = new Regex("\\[([^\\[\\]])*\\]");
        // Any digit in a regular expression that actually denotes a digit. For example, in the regular
        // expression 80[0-2]\d{6,10}, the first 2 digits (8 and 0) are standalone digits, but the rest
        // are not.
        // Two look-aheads are needed because the number following \\d could be a two-digit number, since
        // the phone number can be as long as 15 digits.
        private static readonly Regex STANDALONE_DIGIT_PATTERN = new Regex("\\d(?=[^,}][^,}])");

        // A pattern that is used to determine if a numberFormat under availableFormats is eligible to be
        // used by the AYTF. It is eligible when the format element under numberFormat contains groups of
        // the dollar sign followed by a single digit, separated by valid phone number punctuation. This
        // prevents invalid punctuation (such as the star sign in Israeli star numbers) getting into the
        // output of the AYTF.
        private static readonly Regex ELIGIBLE_FORMAT_PATTERN =
            new Regex("[" + PhoneNumberUtil.VALID_PUNCTUATION + "]*" +
                      "(\\$\\d" + "[" + PhoneNumberUtil.VALID_PUNCTUATION + "]*)+");

        // This is the minimum length of national number accrued that is required to trigger the
        // formatter. The first element of the leadingDigitsPattern of each numberFormat contains a
        // regular expression that matches up to this number of digits.
        private static readonly int MIN_LEADING_DIGITS_LENGTH = 3;

        // The digits that have not been entered yet will be represented by a \u2008, the punctuation
        // space.
        private string digitPlaceholder = "\u2008";
        private Regex digitPattern;
        private int lastMatchPosition = 0;
        // The position of a digit upon which inputDigitAndRememberPosition is most recently invoked, as
        // found in the original sequence of characters the user entered.
        private int originalPosition = 0;
        // The position of a digit upon which inputDigitAndRememberPosition is most recently invoked, as
        // found in accruedInputWithoutFormatting.
        private int positionToRemember = 0;
        // This contains anything that has been entered so far preceding the national significant number,
        // and it is formatted (e.g. with space inserted). For example, this can contain IDD, country
        // code, and/or NDD, etc.
        private StringBuilder prefixBeforeNationalNumber = new StringBuilder();
        // This contains the national prefix that has been extracted. It contains only digits without
        // formatting.
        private string nationalPrefixExtracted = "";
        private StringBuilder nationalNumber = new StringBuilder();
        private IList<PhoneMetaDataNumberFormat> possibleFormats = new List<PhoneMetaDataNumberFormat>();

        // A cache for frequently used country-specific regular expressions.
        private RegexCache regexCache = new RegexCache(64);

        /**
        * Constructs an as-you-type formatter. Should be obtained from {@link
        * PhoneNumberUtil#getAsYouTypeFormatter}.
        *
        * @param regionCode  the country/region where the phone number is being entered
        */

        internal AsYouTypeFormatter(string regionCode)
        {
            digitPattern = new Regex(digitPlaceholder);
            defaultCountry = regionCode;
            currentMetaData = getMetadataForRegion(defaultCountry);
            defaultMetaData = currentMetaData;
        }

        // The metadata needed by this class is the same for all regions sharing the same country calling
        // code. Therefore, we return the metadata for "main" region for this country calling code.
        private PhoneMetaData getMetadataForRegion(string regionCode)
        {
            int countryCallingCode = phoneUtil.getCountryCodeForRegion(regionCode);
            string mainCountry = phoneUtil.getRegionCodeForCountryCode(countryCallingCode);
            PhoneMetaData metadata = phoneUtil.getMetadataForRegion(mainCountry);
            if (metadata != null)
            {
                return metadata;
            }
            // Set to a default instance of the metadata. This allows us to function with an incorrect
            // region code, even if formatting only works for numbers specified with "+".
            return EMPTY_METADATA;
        }

        // Returns true if a new template is created as opposed to reusing the existing template.
        private bool maybeCreateNewTemplate()
        {
            // When there are multiple available formats, the formatter uses the first format where a
            // formatting template could be created.
            IEnumerator<PhoneMetaDataNumberFormat> it = possibleFormats.GetEnumerator();
            var itemsToRemove = new List<PhoneMetaDataNumberFormat>();

            try
            {
                while (it.MoveNext())
                {
                    PhoneMetaDataNumberFormat numberFormat = it.Current;
                    string pattern = numberFormat.Pattern;
                    if (currentFormattingPattern.Equals(pattern))
                    {
                        return false;
                    }
                    if (createFormattingTemplate(numberFormat))
                    {
                        currentFormattingPattern = pattern;
                        // With a new formatting template, the matched position using the old template needs to be
                        // reset.
                        lastMatchPosition = 0;
                        return true;
                    }

                    // Remove the current number format from possibleFormats.
                    itemsToRemove.Add(numberFormat);
                }
                ableToFormat = false;
                return false;
            }
            finally
            {
                foreach (var itemToRemove in itemsToRemove)
                    possibleFormats.Remove(itemToRemove);
            }
        }

        private void getAvailableFormats(string leadingThreeDigits)
        {
            IList<PhoneMetaDataNumberFormat> formatList =
                (isInternationalFormatting && currentMetaData.intlNumberFormatSize > 0)
                    ? currentMetaData.intlNumberFormats()
                    : currentMetaData.numberFormats();
            foreach (PhoneMetaDataNumberFormat format in formatList)
            {
                if (isFormatEligible(format.Format))
                {
                    possibleFormats.Add(format);
                }
            }
            narrowDownPossibleFormats(leadingThreeDigits);
        }

        private bool isFormatEligible(string format)
        {
            return ELIGIBLE_FORMAT_PATTERN.IsMatchWhole(format);
        }

        private void narrowDownPossibleFormats(string leadingDigits)
        {
            int indexOfLeadingDigitsPattern = leadingDigits.Length - MIN_LEADING_DIGITS_LENGTH;
            IEnumerator<PhoneMetaDataNumberFormat> it = possibleFormats.GetEnumerator();
            var itemsToRemove = new List<PhoneMetaDataNumberFormat>();

            try
            {
                while (it.MoveNext())
                {
                    PhoneMetaDataNumberFormat format = it.Current;
                    if (format.LeadingDigitsPatternSize > indexOfLeadingDigitsPattern)
                    {
                        Regex leadingDigitsPattern =
                            regexCache.getPatternForRegex(
                                format.getLeadingDigitsPattern(indexOfLeadingDigitsPattern));
                        if (!leadingDigitsPattern.IsMatchStart(leadingDigits))
                        {
                            itemsToRemove.Add(format);
                        }
                    } // else the particular format has no more specific leadingDigitsPattern, and it should be
                    // retained.
                }
            }
            finally
            {
                foreach (var itemToRemove in itemsToRemove)
                    possibleFormats.Remove(itemToRemove);
            }
        }

        private bool createFormattingTemplate(PhoneMetaDataNumberFormat format)
        {
            string numberPattern = format.Pattern;

            // The formatter doesn't format numbers when numberPattern contains "|", e.g.
            // (20|3)\d{4}. In those cases we quickly return.
            if (numberPattern.IndexOf('|') != -1)
            {
                return false;
            }

            // Replace anything in the form of [..] with \d
            numberPattern = CHARACTER_CLASS_PATTERN.Replace(numberPattern, "\\d");

            // Replace any standalone digit (not the one in d{}) with \d
            numberPattern = STANDALONE_DIGIT_PATTERN.Replace(numberPattern, "\\d");
            formattingTemplate.Clear();
            string tempTemplate = getFormattingTemplate(numberPattern, format.Format);
            if (tempTemplate.Length > 0)
            {
                formattingTemplate.Append(tempTemplate);
                return true;
            }
            return false;
        }

        // Gets a formatting template which can be used to efficiently format a partial number where
        // digits are added one by one.
        private string getFormattingTemplate(string numberPattern, string numberFormat)
        {
            // Creates a phone number consisting only of the digit 9 that matches the
            // numberPattern by applying the pattern to the longestPhoneNumber string.
            string longestPhoneNumber = "999999999999999";
            Match m = regexCache.getPatternForRegex(numberPattern).Match(longestPhoneNumber);
            string aPhoneNumber = m.Value;
            // No formatting template can be created if the number of digits entered so far is longer than
            // the maximum the current formatting rule can accommodate.
            if (aPhoneNumber.Length < nationalNumber.Length)
            {
                return "";
            }
            // Formats the number according to numberFormat
            string template = regexCache.getPatternForRegex(numberPattern).Replace(aPhoneNumber, numberFormat);
            // Replaces each digit with character digitPlaceholder
            template = template.Replace("9", digitPlaceholder);
            return template;
        }

        /**
   * Clears the internal state of the formatter, so it can be reused.
   */

        public void clear()
        {
            currentOutput = "";
            accruedInput.Clear();
            accruedInputWithoutFormatting.Clear();
            formattingTemplate.Clear();
            lastMatchPosition = 0;
            currentFormattingPattern = "";
            prefixBeforeNationalNumber.Clear();
            nationalPrefixExtracted = "";
            nationalNumber.Clear();
            ableToFormat = true;
            inputHasFormatting = false;
            positionToRemember = 0;
            originalPosition = 0;
            isInternationalFormatting = false;
            isExpectingCountryCallingCode = false;
            possibleFormats.Clear();
            if (!currentMetaData.Equals(defaultMetaData))
            {
                currentMetaData = getMetadataForRegion(defaultCountry);
            }
        }

        /**
   * Formats a phone number on-the-fly as each digit is entered.
   *
   * @param nextChar  the most recently entered digit of a phone number. Formatting characters are
   *     allowed, but as soon as they are encountered this method formats the number as entered and
   *     not "as you type" anymore. Full width digits and Arabic-indic digits are allowed, and will
   *     be shown as they are.
   * @return  the partially formatted phone number.
   */

        public string inputDigit(char nextChar)
        {
            currentOutput = inputDigitWithOptionToRememberPosition(nextChar, false);
            return currentOutput;
        }

        /**
   * Same as {@link #inputDigit}, but remembers the position where {@code nextChar} is inserted, so
   * that it can be retrieved later by using {@link #getRememberedPosition}. The remembered
   * position will be automatically adjusted if additional formatting characters are later
   * inserted/removed in front of {@code nextChar}.
   */

        public string inputDigitAndRememberPosition(char nextChar)
        {
            currentOutput = inputDigitWithOptionToRememberPosition(nextChar, true);
            return currentOutput;
        }

        private string inputDigitWithOptionToRememberPosition(char nextChar, bool rememberPosition)
        {
            accruedInput.Append(nextChar);
            if (rememberPosition)
            {
                originalPosition = accruedInput.Length;
            }
            // We do formatting on-the-fly only when each character entered is either a digit, or a plus
            // sign (accepted at the start of the number only).
            if (!isDigitOrLeadingPlusSign(nextChar))
            {
                ableToFormat = false;
                inputHasFormatting = true;
            }
            else
            {
                nextChar = normalizeAndAccrueDigitsAndPlusSign(nextChar, rememberPosition);
            }
            if (!ableToFormat)
            {
                // When we are unable to format because of reasons other than that formatting chars have been
                // entered, it can be due to really long IDDs or NDDs. If that is the case, we might be able
                // to do formatting again after extracting them.
                if (inputHasFormatting)
                {
                    return accruedInput.ToString();
                }
                else if (attemptToExtractIdd())
                {
                    if (attemptToExtractCountryCallingCode())
                    {
                        return attemptToChoosePatternWithPrefixExtracted();
                    }
                }
                else if (ableToExtractLongerNdd())
                {
                    // Add an additional space to separate long NDD and national significant number for
                    // readability.
                    prefixBeforeNationalNumber.Append(" ");
                    return attemptToChoosePatternWithPrefixExtracted();
                }
                return accruedInput.ToString();
            }

            // We start to attempt to format only when at least MIN_LEADING_DIGITS_LENGTH digits (the plus
            // sign is counted as a digit as well for this purpose) have been entered.
            switch (accruedInputWithoutFormatting.Length)
            {
                case 0:
                case 1:
                case 2:
                    return accruedInput.ToString();
                case 3:
                    if (attemptToExtractIdd())
                    {
                        isExpectingCountryCallingCode = true;
                    }
                    else
                    {
                        // No IDD or plus sign is found, might be entering in national format.
                        nationalPrefixExtracted = removeNationalPrefixFromNationalNumber();
                        return attemptToChooseFormattingPattern();
                    }
                    break;
            }

            if (isExpectingCountryCallingCode)
            {
                if (attemptToExtractCountryCallingCode())
                {
                    isExpectingCountryCallingCode = false;
                }
                return prefixBeforeNationalNumber + nationalNumber.ToString();
            }
            if (possibleFormats.Count > 0)
            {
                // The formatting pattern is already chosen.
                string tempNationalNumber = inputDigitHelper(nextChar);
                // See if the accrued digits can be formatted properly already. If not, use the results
                // from inputDigitHelper, which does formatting based on the formatting pattern chosen.
                string formattedNumber = attemptToFormatAccruedDigits();
                if (formattedNumber.Length > 0)
                {
                    return formattedNumber;
                }
                narrowDownPossibleFormats(nationalNumber.ToString());
                if (maybeCreateNewTemplate())
                {
                    return inputAccruedNationalNumber();
                }
                return ableToFormat
                           ? prefixBeforeNationalNumber + tempNationalNumber
                           : accruedInput.ToString();
            }
            else
            {
                return attemptToChooseFormattingPattern();
            }
        }

        private string attemptToChoosePatternWithPrefixExtracted()
        {
            ableToFormat = true;
            isExpectingCountryCallingCode = false;
            possibleFormats.Clear();
            return attemptToChooseFormattingPattern();
        }

        // Some national prefixes are a substring of others. If extracting the shorter NDD doesn't result
        // in a number we can format, we try to see if we can extract a longer version here.
        private bool ableToExtractLongerNdd()
        {
            if (nationalPrefixExtracted.Length > 0)
            {
                // Put the extracted NDD back to the national number before attempting to extract a new NDD.
                nationalNumber.Insert(0, nationalPrefixExtracted);
                // Remove the previously extracted NDD from prefixBeforeNationalNumber. We cannot simply set
                // it to empty string because people sometimes enter national prefix after country code, e.g
                // +44 (0)20-1234-5678.
                int indexOfPreviousNdd = prefixBeforeNationalNumber.ToString().LastIndexOf(nationalPrefixExtracted,
                                                                                           System.StringComparison.
                                                                                               Ordinal);
                prefixBeforeNationalNumber.Length = indexOfPreviousNdd;
            }
            return !nationalPrefixExtracted.Equals(removeNationalPrefixFromNationalNumber());
        }

        private bool isDigitOrLeadingPlusSign(char nextChar)
        {
            return char.IsDigit(nextChar) ||
                   (accruedInput.Length == 1 &&
                    PhoneNumberUtil.PLUS_CHARS_PATTERN.IsMatchWhole(nextChar.ToString(CultureInfo.InvariantCulture)));
        }

        private string attemptToFormatAccruedDigits()
        {
            foreach (PhoneMetaDataNumberFormat numFormat in possibleFormats)
            {
                Regex regex = regexCache.getPatternForRegex(numFormat.Pattern);
                if (regex.IsMatchWhole(nationalNumber.ToString()))
                {
                    string formattedNumber = regex.Replace(nationalNumber.ToString(), numFormat.Format);
                    return prefixBeforeNationalNumber + formattedNumber;
                }
            }
            return "";
        }

        /**
   * Returns the current position in the partially formatted phone number of the character which was
   * previously passed in as the parameter of {@link #inputDigitAndRememberPosition}.
   */

        public int getRememberedPosition()
        {
            if (!ableToFormat)
            {
                return originalPosition;
            }
            int accruedInputIndex = 0, currentOutputIndex = 0;
            while (accruedInputIndex < positionToRemember && currentOutputIndex < currentOutput.Length)
            {
                if (accruedInputWithoutFormatting[accruedInputIndex] == currentOutput[currentOutputIndex])
                {
                    accruedInputIndex++;
                }
                currentOutputIndex++;
            }
            return currentOutputIndex;
        }

        // Attempts to set the formatting template and returns a string which contains the formatted
        // version of the digits entered so far.
        private string attemptToChooseFormattingPattern()
        {
            // We start to attempt to format only when as least MIN_LEADING_DIGITS_LENGTH digits of national
            // number (excluding national prefix) have been entered.
            if (nationalNumber.Length >= MIN_LEADING_DIGITS_LENGTH)
            {
                getAvailableFormats(nationalNumber.ToString().Substring(0, MIN_LEADING_DIGITS_LENGTH));
                return maybeCreateNewTemplate() ? inputAccruedNationalNumber() : accruedInput.ToString();
            }
            else
            {
                return prefixBeforeNationalNumber + nationalNumber.ToString();
            }
        }

        // Invokes inputDigitHelper on each digit of the national number accrued, and returns a formatted
        // string in the end.
        private string inputAccruedNationalNumber()
        {
            int lengthOfNationalNumber = nationalNumber.Length;
            if (lengthOfNationalNumber > 0)
            {
                string tempNationalNumber = "";
                for (int i = 0; i < lengthOfNationalNumber; i++)
                {
                    tempNationalNumber = inputDigitHelper(nationalNumber[i]);
                }
                return ableToFormat
                           ? prefixBeforeNationalNumber + tempNationalNumber
                           : accruedInput.ToString();
            }
            else
            {
                return prefixBeforeNationalNumber.ToString();
            }
        }

        // Returns the national prefix extracted, or an empty string if it is not present.
        private string removeNationalPrefixFromNationalNumber()
        {
            int startOfNationalNumber = 0;
            if (currentMetaData.CountryCode == 1 && nationalNumber[0] == '1')
            {
                startOfNationalNumber = 1;
                prefixBeforeNationalNumber.Append("1 ");
                isInternationalFormatting = true;
            }
            else if (currentMetaData.HasNationalPrefixForParsing)
            {
                Regex nationalPrefixForParsing =
                    regexCache.getPatternForRegex(currentMetaData.NationalPrefixForParsing);
                Match m = nationalPrefixForParsing.Match(nationalNumber.ToString());
                if (m.SuccessInputStart())
                {
                    // When the national prefix is detected, we use international formatting rules instead of
                    // national ones, because national formatting rules could contain local formatting rules
                    // for numbers entered without area code.
                    isInternationalFormatting = true;
                    startOfNationalNumber = m.Index + m.Length;
                    prefixBeforeNationalNumber.Append(nationalNumber.ToString().Substring(0, startOfNationalNumber));
                }
            }
            string nationalPrefix = nationalNumber.ToString().Substring(0, startOfNationalNumber);
            nationalNumber.Remove(0, startOfNationalNumber);
            return nationalPrefix;
        }

        /**
   * Extracts IDD and plus sign to prefixBeforeNationalNumber when they are available, and places
   * the remaining input into nationalNumber.
   *
   * @return  true when accruedInputWithoutFormatting begins with the plus sign or valid IDD for
   *     defaultCountry.
   */

        private bool attemptToExtractIdd()
        {
            Regex internationalPrefix = regexCache.getPatternForRegex("\\" + PhoneNumberUtil.PLUS_SIGN + "|" + currentMetaData.InternationalPrefix);
            Match iddMatcher = internationalPrefix.Match(accruedInputWithoutFormatting.ToString());
            if (iddMatcher.SuccessInputStart())
            {
                isInternationalFormatting = true;
                int startOfCountryCallingCode = iddMatcher.Index + iddMatcher.Length;
                nationalNumber.Clear();
                nationalNumber.Append(accruedInputWithoutFormatting.ToString().Substring(startOfCountryCallingCode));
                prefixBeforeNationalNumber.Clear();
                prefixBeforeNationalNumber.Append(
                    accruedInputWithoutFormatting.ToString().Substring(0, startOfCountryCallingCode));
                if (accruedInputWithoutFormatting[0] != PhoneNumberUtil.PLUS_SIGN)
                {
                    prefixBeforeNationalNumber.Append(" ");
                }
                return true;
            }
            return false;
        }

        /**
   * Extracts the country calling code from the beginning of nationalNumber to
   * prefixBeforeNationalNumber when they are available, and places the remaining input into
   * nationalNumber.
   *
   * @return  true when a valid country calling code can be found.
   */

        private bool attemptToExtractCountryCallingCode()
        {
            if (nationalNumber.Length == 0)
            {
                return false;
            }
            StringBuilder numberWithoutCountryCallingCode = new StringBuilder();
            int countryCode = phoneUtil.extractCountryCode(nationalNumber, numberWithoutCountryCallingCode);
            if (countryCode == 0)
            {
                return false;
            }
            nationalNumber.Clear();
            nationalNumber.Append(numberWithoutCountryCallingCode);
            string newRegionCode = phoneUtil.getRegionCodeForCountryCode(countryCode);
            if (PhoneNumberUtil.REGION_CODE_FOR_NON_GEO_ENTITY.Equals(newRegionCode))
            {
                currentMetaData = phoneUtil.getMetadataForNonGeographicalRegion(countryCode);
            }
            else if (!newRegionCode.Equals(defaultCountry))
            {
                currentMetaData = getMetadataForRegion(newRegionCode);
            }
            string countryCodeString = countryCode.ToString(CultureInfo.InvariantCulture);
            prefixBeforeNationalNumber.Append(countryCodeString).Append(" ");
            return true;
        }

        // Accrues digits and the plus sign to accruedInputWithoutFormatting for later use. If nextChar
        // contains a digit in non-ASCII format (e.g. the full-width version of digits), it is first
        // normalized to the ASCII version. The return value is nextChar itself, or its normalized
        // version, if nextChar is a digit in non-ASCII format. This method assumes its input is either a
        // digit or the plus sign.
        private char normalizeAndAccrueDigitsAndPlusSign(char nextChar, bool rememberPosition)
        {
            char normalizedChar;
            if (nextChar == PhoneNumberUtil.PLUS_SIGN)
            {
                normalizedChar = nextChar;
                accruedInputWithoutFormatting.Append(nextChar);
            }
            else
            {
                normalizedChar = ((int)(char.GetNumericValue(nextChar))).ToString(CultureInfo.InvariantCulture)[0];
                accruedInputWithoutFormatting.Append(normalizedChar);
                nationalNumber.Append(normalizedChar);
            }
            if (rememberPosition)
            {
                positionToRemember = accruedInputWithoutFormatting.Length;
            }
            return normalizedChar;
        }

        private string inputDigitHelper(char nextChar)
        {
            Match digitMatcher = digitPattern.Match(formattingTemplate.ToString(), lastMatchPosition);
            if (digitMatcher.Success)
            {
                string tempTemplate = digitPattern.Replace(formattingTemplate.ToString(), nextChar.ToString(CultureInfo.InvariantCulture), 1, lastMatchPosition);
                formattingTemplate.Remove(0, tempTemplate.Length).Insert(0, tempTemplate);
                lastMatchPosition = digitMatcher.Index;
                return formattingTemplate.ToString().Substring(0, lastMatchPosition + 1);
            }
            
            if (possibleFormats.Count == 1)
            {
                // More digits are entered than we could handle, and there are no other valid patterns to
                // try.
                ableToFormat = false;
            } // else, we just reset the formatting pattern.
            currentFormattingPattern = "";
            return accruedInput.ToString();
        }
    }
}