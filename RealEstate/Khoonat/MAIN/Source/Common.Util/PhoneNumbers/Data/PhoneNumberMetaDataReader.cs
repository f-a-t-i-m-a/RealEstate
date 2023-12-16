using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

// ReSharper disable PossibleNullReferenceException
namespace JahanJooy.Common.Util.PhoneNumbers.Data
{
    public class PhoneNumberMetaDataReader
    {

        // String constants used to fetch the XML nodes and attributes.
        private const string CARRIER_CODE_FORMATTING_RULE = "carrierCodeFormattingRule";
        private const string COUNTRY_CODE = "countryCode";
        private const string EMERGENCY = "emergency";
        private const string EXAMPLE_NUMBER = "exampleNumber";
        private const string FIXED_LINE = "fixedLine";
        private const string FORMAT = "format";
        private const string GENERAL_DESC = "generalDesc";
        private const string INTERNATIONAL_PREFIX = "internationalPrefix";
        private const string INTL_FORMAT = "intlFormat";
        private const string LEADING_DIGITS = "leadingDigits";
        private const string LEADING_ZERO_POSSIBLE = "leadingZeroPossible";
        private const string MAIN_COUNTRY_FOR_CODE = "mainCountryForCode";
        private const string MOBILE = "mobile";
        private const string NATIONAL_NUMBER_PATTERN = "nationalNumberPattern";
        private const string NATIONAL_PREFIX = "nationalPrefix";
        private const string NATIONAL_PREFIX_FORMATTING_RULE = "nationalPrefixFormattingRule";
        private const string NATIONAL_PREFIX_OPTIONAL_WHEN_FORMATTING = "nationalPrefixOptionalWhenFormatting";
        private const string NATIONAL_PREFIX_FOR_PARSING = "nationalPrefixForParsing";
        private const string NATIONAL_PREFIX_TRANSFORM_RULE = "nationalPrefixTransformRule";
        private const string NO_INTERNATIONAL_DIALLING = "noInternationalDialling";
        private const string NUMBER_FORMAT = "numberFormat";
        private const string PAGER = "pager";
        private const string PATTERN = "pattern";
        private const string PERSONAL_NUMBER = "personalNumber";
        private const string POSSIBLE_NUMBER_PATTERN = "possibleNumberPattern";
        private const string PREFERRED_EXTN_PREFIX = "preferredExtnPrefix";
        private const string PREFERRED_INTERNATIONAL_PREFIX = "preferredInternationalPrefix";
        private const string PREMIUM_RATE = "premiumRate";
        private const string SHARED_COST = "sharedCost";
        private const string TOLL_FREE = "tollFree";
        private const string UAN = "uan";
        private const string VOICEMAIL = "voicemail";
        private const string VOIP = "voip";

        private static readonly Regex WhitespaceRegex = new Regex("\\s");
        private static readonly Regex NPRegex = new Regex("\\$NP");
        private static readonly Regex FGRegex = new Regex("\\$FG");

        public static PhoneMetaDataCollection BuildMetaDataCollection(Stream stream, bool liteBuild)
        {
            var root = XElement.Load(stream);
            var territories = root.Descendants("territory");

            var result = new PhoneMetaDataCollection();
            foreach (var territory in territories)
            {
                string regionCode = "";

                var idAttribute = territory.Attribute("id");
                if (idAttribute != null) 
                    regionCode = idAttribute.Value;

                PhoneMetaData metaData = LoadCountryMetaData(regionCode, territory, liteBuild);
                result.addMetadata(metaData);
            }

            return result;
        }

        public static IDictionary<int, IList<string>> BuildCountryCodeToRegionCodeMap(PhoneMetaDataCollection metaDataCollection)
        {
            IDictionary<int, IList<string>> countryCodeToRegionCodeMap = new SortedDictionary<int, IList<string>>();
            foreach (PhoneMetaData metaData in metaDataCollection.getMetadataList())
            {
                string regionCode = metaData.ID;
                int countryCode = metaData.CountryCode;

                if (countryCodeToRegionCodeMap.ContainsKey(countryCode))
                {
                    if (metaData.getMainCountryForCode())
                    {
                        countryCodeToRegionCodeMap[countryCode].Insert(0, regionCode);
                    }
                    else
                    {
                        countryCodeToRegionCodeMap[countryCode].Add(regionCode);
                    }
                }
                else
                {
                    // For most countries, there will be only one region code for the country calling code.
                    IList<string> listWithRegionCode = new List<string>(1);
                    if (!string.IsNullOrEmpty(regionCode))
                    {  // For alternate formats, there are no region codes at all.
                        listWithRegionCode.Add(regionCode);
                    }
                    countryCodeToRegionCodeMap[countryCode] = listWithRegionCode;
                }
            }
            return countryCodeToRegionCodeMap;
        }

        private static PhoneMetaData LoadCountryMetaData(string regionCode, XElement territory, bool liteBuild)
        {
            string nationalPrefix = GetNationalPrefix(territory);
            PhoneMetaData result = LoadTerritoryTagMetaData(regionCode, territory, nationalPrefix);
            string nationalPrefixFormattingRule = GetNationalPrefixFormattingRuleFromElement(territory, nationalPrefix);
            LoadAvailableFormats(result, territory, nationalPrefix, nationalPrefixFormattingRule, territory.Attribute(NATIONAL_PREFIX_OPTIONAL_WHEN_FORMATTING) != null);
            LoadGeneralDesc(result, territory, liteBuild);

            return result;
        }

        internal static void LoadGeneralDesc(PhoneMetaData metaData, XElement territory, bool liteBuild)
        {
            var generalDesc = new PhoneNumberDesc();
            generalDesc = ProcessPhoneNumberDescElement(generalDesc, territory, GENERAL_DESC, liteBuild);
            metaData.setGeneralDesc(generalDesc);

            metaData.setFixedLine(ProcessPhoneNumberDescElement(generalDesc, territory, FIXED_LINE, liteBuild));
            metaData.setMobile(ProcessPhoneNumberDescElement(generalDesc, territory, MOBILE, liteBuild));
            metaData.setTollFree(ProcessPhoneNumberDescElement(generalDesc, territory, TOLL_FREE, liteBuild));
            metaData.setPremiumRate(ProcessPhoneNumberDescElement(generalDesc, territory, PREMIUM_RATE, liteBuild));
            metaData.setSharedCost(ProcessPhoneNumberDescElement(generalDesc, territory, SHARED_COST, liteBuild));
            metaData.setVoip(ProcessPhoneNumberDescElement(generalDesc, territory, VOIP, liteBuild));
            metaData.setPersonalNumber(ProcessPhoneNumberDescElement(generalDesc, territory, PERSONAL_NUMBER, liteBuild));
            metaData.setPager(ProcessPhoneNumberDescElement(generalDesc, territory, PAGER, liteBuild));
            metaData.setUan(ProcessPhoneNumberDescElement(generalDesc, territory, UAN, liteBuild));
            metaData.setVoicemail(ProcessPhoneNumberDescElement(generalDesc, territory, VOICEMAIL, liteBuild));
            metaData.setEmergency(ProcessPhoneNumberDescElement(generalDesc, territory, EMERGENCY, liteBuild));
            metaData.setNoInternationalDialling(ProcessPhoneNumberDescElement(generalDesc, territory, NO_INTERNATIONAL_DIALLING, liteBuild));
            metaData.setSameMobileAndFixedLinePattern(metaData.Mobile.NationalNumberPattern.Equals(metaData.FixedLine.NationalNumberPattern));
        }

        internal static PhoneNumberDesc ProcessPhoneNumberDescElement(PhoneNumberDesc generalDesc, XElement territory, string numberType, bool liteBuild)
        {
            var phoneNumberDescList = territory.Descendants(numberType);
            var numberDesc = new PhoneNumberDesc();
            if (!phoneNumberDescList.Any() && !IsValidNumberType(numberType))
            {
                numberDesc.setNationalNumberPattern("NA");
                numberDesc.setPossibleNumberPattern("NA");
                return numberDesc;
            }

            numberDesc.mergeFrom(generalDesc);
            if (phoneNumberDescList.Any())
            {
                var element = phoneNumberDescList.First();
                var possiblePattern = element.Descendants(POSSIBLE_NUMBER_PATTERN);
                if (possiblePattern.Any())
                {
                    numberDesc.setPossibleNumberPattern(validateRE(possiblePattern.First().Value, true));
                }

                var validPattern = element.Descendants(NATIONAL_NUMBER_PATTERN);
                if (validPattern.Any())
                {
                    numberDesc.setNationalNumberPattern(validateRE(validPattern.First().Value, true));
                }

                if (!liteBuild)
                {
                    var exampleNumber = element.Descendants(EXAMPLE_NUMBER);
                    if (exampleNumber.Any())
                    {
                        numberDesc.setExampleNumber(exampleNumber.First().Value);
                    }
                }
            }
            return numberDesc;

        }

        internal static bool IsValidNumberType(string numberType)
        {
            return numberType.Equals(FIXED_LINE) || numberType.Equals(MOBILE) || numberType.Equals(GENERAL_DESC);
        }

        internal static void LoadAvailableFormats(PhoneMetaData metaData, XElement territory, string nationalPrefix, 
            string nationalPrefixFormattingRule, bool nationalPrefixOptionalWhenFormatting)
        {
            string carrierCodeFormattingRule = "";
            if (territory.Attribute(CARRIER_CODE_FORMATTING_RULE) != null)
            {
                carrierCodeFormattingRule = validateRE(
                    GetDomesticCarrierCodeFormattingRuleFromElement(territory, nationalPrefix));
            }
            var numberFormatElements = territory.Descendants(NUMBER_FORMAT);
            bool hasExplicitIntlFormatDefined = false;
            bool hasNumberFormatElements = false;

            foreach (var numberFormatElement in numberFormatElements)
            {
                hasNumberFormatElements = true;

                var format = new PhoneMetaDataNumberFormat();

                if (numberFormatElement.Attribute(NATIONAL_PREFIX_FORMATTING_RULE) != null)
                {
                    format.setNationalPrefixFormattingRule(
                        GetNationalPrefixFormattingRuleFromElement(numberFormatElement, nationalPrefix));
                    format.setNationalPrefixOptionalWhenFormatting(
                        numberFormatElement.Attribute(NATIONAL_PREFIX_OPTIONAL_WHEN_FORMATTING) != null);
                }
                else
                {
                    format.setNationalPrefixFormattingRule(nationalPrefixFormattingRule);
                    format.setNationalPrefixOptionalWhenFormatting(nationalPrefixOptionalWhenFormatting);
                }
                if (numberFormatElement.Attribute(CARRIER_CODE_FORMATTING_RULE) != null)
                {
                    format.setDomesticCarrierCodeFormattingRule(validateRE(
                        GetDomesticCarrierCodeFormattingRuleFromElement(numberFormatElement,
                                                                        nationalPrefix)));
                }
                else
                {
                    format.setDomesticCarrierCodeFormattingRule(carrierCodeFormattingRule);
                }

                string nationalFormat = LoadNationalFormat(metaData, numberFormatElement, format);
                metaData.addNumberFormat(format);

                if (LoadInternationalFormat(metaData, numberFormatElement, nationalFormat))
                {
                    hasExplicitIntlFormatDefined = true;
                }
            }

            if (hasNumberFormatElements)
            {
                // Only a small number of regions need to specify the intlFormats in the xml. For the majority
                // of countries the intlNumberFormat metadata is an exact copy of the national NumberFormat
                // metadata. To minimize the size of the metadata file, we only keep intlNumberFormats that
                // actually differ in some way to the national formats.
                if (!hasExplicitIntlFormatDefined)
                {
                    metaData.clearIntlNumberFormat();
                }
            }
        }

        internal static bool LoadInternationalFormat(PhoneMetaData metaData, XElement numberFormatElement, string nationalFormat)
        {
            var intlFormat = new PhoneMetaDataNumberFormat();
            SetLeadingDigitsPatterns(numberFormatElement, intlFormat);
            intlFormat.SetPattern(numberFormatElement.Attribute(PATTERN) != null ? numberFormatElement.Attribute(PATTERN).Value : "");

            var intlFormatPattern = numberFormatElement.Descendants(INTL_FORMAT);
            bool hasExplicitIntlFormatDefined = false;

            if (intlFormatPattern.Count() > 1)
            {
//                LOGGER.log(Level.SEVERE,
//                           "A maximum of one intlFormat pattern for a numberFormat element should be " +
//                           "defined."); TODO
                throw new Exception("Invalid number of intlFormat patterns for country: " + metaData.ID);
            }
            
            if (!intlFormatPattern.Any())
            {
                // Default to use the same as the national pattern if none is defined.
                intlFormat.SetFormat(nationalFormat);
            }
            else
            {
                String intlFormatPatternValue = intlFormatPattern.First().Value;
                if (!intlFormatPatternValue.Equals("NA"))
                {
                    intlFormat.SetFormat(intlFormatPatternValue);
                }
                hasExplicitIntlFormatDefined = true;
            }

            if (intlFormat.HasFormat)
            {
                metaData.addIntlNumberFormat(intlFormat);
            }

            return hasExplicitIntlFormatDefined;
        }

        internal static string LoadNationalFormat(PhoneMetaData metaData, XElement numberFormatElement, PhoneMetaDataNumberFormat format)
        {
            SetLeadingDigitsPatterns(numberFormatElement, format);
            format.SetPattern(validateRE(numberFormatElement.Attribute(PATTERN) != null ? numberFormatElement.Attribute(PATTERN).Value : ""));

            var formatPattern = numberFormatElement.Descendants(FORMAT).SingleOrDefault();
            if (formatPattern == null)
            {
//                LOGGER.log(Level.SEVERE,
//                           "Only one format pattern for a numberFormat element should be defined."); TODO
                throw new Exception("Invalid number of format patterns for country: " + metaData.ID);
            }

            string nationalFormat = formatPattern.Value;
            format.SetFormat(nationalFormat);
            return nationalFormat;

        }

        internal static void SetLeadingDigitsPatterns(XElement numberFormatElement, PhoneMetaDataNumberFormat format)
        {
            var leadingDigitsPatternNodes = numberFormatElement.Descendants(LEADING_DIGITS);
            foreach (var leadingDigitsPatternNode in leadingDigitsPatternNodes)
            {
                format.addLeadingDigitsPattern(validateRE(leadingDigitsPatternNode.Value, true));
            }
        }

        internal static string GetDomesticCarrierCodeFormattingRuleFromElement(XElement territory, string nationalPrefix)
        {
            string carrierCodeFormattingRule = territory.Attribute(CARRIER_CODE_FORMATTING_RULE) != null ? territory.Attribute(CARRIER_CODE_FORMATTING_RULE).Value : "";
            // Replace $FG with the first group ($1) and $NP with the national prefix.
            carrierCodeFormattingRule = NPRegex.Replace(FGRegex.Replace(carrierCodeFormattingRule, "$1", 1), nationalPrefix, 1);
            return carrierCodeFormattingRule;
        }

        internal static string GetNationalPrefixFormattingRuleFromElement(XElement territory, string nationalPrefix)
        {
            string nationalPrefixFormattingRule = territory.Attribute(NATIONAL_PREFIX_FORMATTING_RULE) != null ? territory.Attribute(NATIONAL_PREFIX_FORMATTING_RULE).Value : "";
            // Replace $NP with national prefix and $FG with the first group ($1).
            nationalPrefixFormattingRule = FGRegex.Replace(NPRegex.Replace(nationalPrefixFormattingRule, nationalPrefix, 1), "$1", 1);
            return nationalPrefixFormattingRule;

        }

        internal static PhoneMetaData LoadTerritoryTagMetaData(string regionCode, XElement territory, string nationalPrefix)
        {
            var result = new PhoneMetaData();

            result.setId(regionCode);
            result.setCountryCode(int.Parse(territory.Attribute(COUNTRY_CODE).Value));

            if (territory.Attribute(LEADING_DIGITS) != null)
            {
                result.setLeadingDigits(validateRE(territory.Attribute(LEADING_DIGITS).Value));
            }

            result.setInternationalPrefix(validateRE(territory.Attribute(INTERNATIONAL_PREFIX) != null ? territory.Attribute(INTERNATIONAL_PREFIX).Value : ""));
            if (territory.Attribute(PREFERRED_INTERNATIONAL_PREFIX) != null)
            {
                string preferredInternationalPrefix = territory.Attribute(PREFERRED_INTERNATIONAL_PREFIX).Value;
                result.setPreferredInternationalPrefix(preferredInternationalPrefix);
            }

            if (territory.Attribute(NATIONAL_PREFIX_FOR_PARSING) != null)
            {
                result.setNationalPrefixForParsing(validateRE(territory.Attribute(NATIONAL_PREFIX_FOR_PARSING).Value, true));
                if (territory.Attribute(NATIONAL_PREFIX_TRANSFORM_RULE) != null)
                    result.setNationalPrefixTransformRule(validateRE(territory.Attribute(NATIONAL_PREFIX_TRANSFORM_RULE).Value));
            }

            if (!string.IsNullOrEmpty(nationalPrefix))
            {
                result.setNationalPrefix(nationalPrefix);
                if (!result.HasNationalPrefixForParsing)
                    result.setNationalPrefixForParsing(nationalPrefix);
            }
            if (territory.Attribute(PREFERRED_EXTN_PREFIX) != null)
            {
                result.setPreferredExtnPrefix(territory.Attribute(PREFERRED_EXTN_PREFIX).Value);
            }
            if (territory.Attribute(MAIN_COUNTRY_FOR_CODE) != null)
            {
                result.setMainCountryForCode(true);
            }
            if (territory.Attribute(LEADING_ZERO_POSSIBLE) != null)
            {
                result.setLeadingZeroPossible(true);
            }
            return result;
        }

        internal static string GetNationalPrefix(XElement territory)
        {
            var nationalPrefixAttribute = territory.Attribute(NATIONAL_PREFIX);
            return nationalPrefixAttribute != null ? nationalPrefixAttribute.Value : "";
        }

        private static string validateRE(string regex)
        {
            return validateRE(regex, false);
        }

        internal static string validateRE(string regex, bool removeWhitespace)
        {
            // Removes all the whitespace and newline from the regexp. Not using pattern compile options to
            // make it work across programming languages.
            if (removeWhitespace)
            {
                regex = WhitespaceRegex.Replace(regex, "");
            }

            // ReSharper disable ObjectCreationAsStatement
#if DEBUG
            new Regex(regex);
#endif
            // ReSharper restore ObjectCreationAsStatement

            // return regex itself if it is of correct regex syntax
            // i.e. compile did not fail with a PatternSyntaxException.
            return regex;
        }
 
    }
}