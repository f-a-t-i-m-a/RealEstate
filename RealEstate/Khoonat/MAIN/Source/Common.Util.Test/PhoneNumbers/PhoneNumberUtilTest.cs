using System;
using System.Collections.Generic;
using System.Text;
using JahanJooy.Common.Util.PhoneNumbers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JahanJooy.Common.Util.Test.PhoneNumbers
{
    /**
     * Unit tests for PhoneNumberUtil.java
     *
     * Note that these tests use the test metadata, not the normal metadata file, so should not be used
     * for regression test purposes - these tests are illustrative only and test functionality.
     *
     * @author Shaopeng Jia
     * @author Lara Rennie
     */

    [TestClass]
    public class PhoneNumberUtilTest : TestMetaDataBase
    {
        // Set up some test numbers to re-use.
        private static readonly PhoneNumber ALPHA_NUMERIC_NUMBER =
            new PhoneNumber().SetCountryCode(1).SetNationalNumber(80074935247L);

        private static readonly PhoneNumber AR_MOBILE =
            new PhoneNumber().SetCountryCode(54).SetNationalNumber(91187654321L);

        private static readonly PhoneNumber AR_NUMBER =
            new PhoneNumber().SetCountryCode(54).SetNationalNumber(1187654321);

        private static readonly PhoneNumber AU_NUMBER =
            new PhoneNumber().SetCountryCode(61).SetNationalNumber(236618300L);

        private static readonly PhoneNumber BS_MOBILE =
            new PhoneNumber().SetCountryCode(1).SetNationalNumber(2423570000L);

        private static readonly PhoneNumber BS_NUMBER =
            new PhoneNumber().SetCountryCode(1).SetNationalNumber(2423651234L);

        // Note that this is the same as the example number for DE in the metadata.
        private static readonly PhoneNumber DE_NUMBER =
            new PhoneNumber().SetCountryCode(49).SetNationalNumber(30123456L);

        private static readonly PhoneNumber DE_SHORT_NUMBER =
            new PhoneNumber().SetCountryCode(49).SetNationalNumber(1234L);

        private static readonly PhoneNumber GB_MOBILE =
            new PhoneNumber().SetCountryCode(44).SetNationalNumber(7912345678L);

        private static readonly PhoneNumber GB_NUMBER =
            new PhoneNumber().SetCountryCode(44).SetNationalNumber(2070313000L);

        private static readonly PhoneNumber IT_MOBILE =
            new PhoneNumber().SetCountryCode(39).SetNationalNumber(345678901L);

        private static readonly PhoneNumber IT_NUMBER =
            new PhoneNumber().SetCountryCode(39).SetNationalNumber(236618300L).
                setItalianLeadingZero(true);

        private static readonly PhoneNumber JP_STAR_NUMBER =
            new PhoneNumber().SetCountryCode(81).SetNationalNumber(2345);

        // Numbers to test the formatting rules from Mexico.
        private static readonly PhoneNumber MX_MOBILE1 =
            new PhoneNumber().SetCountryCode(52).SetNationalNumber(12345678900L);

        private static readonly PhoneNumber MX_MOBILE2 =
            new PhoneNumber().SetCountryCode(52).SetNationalNumber(15512345678L);

        private static readonly PhoneNumber MX_NUMBER1 =
            new PhoneNumber().SetCountryCode(52).SetNationalNumber(3312345678L);

        private static readonly PhoneNumber MX_NUMBER2 =
            new PhoneNumber().SetCountryCode(52).SetNationalNumber(8211234567L);

        private static readonly PhoneNumber NZ_NUMBER =
            new PhoneNumber().SetCountryCode(64).SetNationalNumber(33316005L);

        private static readonly PhoneNumber SG_NUMBER =
            new PhoneNumber().SetCountryCode(65).SetNationalNumber(65218000L);

        // A too-long and hence invalid US number.
        private static readonly PhoneNumber US_LONG_NUMBER =
            new PhoneNumber().SetCountryCode(1).SetNationalNumber(65025300001L);

        private static readonly PhoneNumber US_NUMBER =
            new PhoneNumber().SetCountryCode(1).SetNationalNumber(6502530000L);

        private static readonly PhoneNumber US_PREMIUM =
            new PhoneNumber().SetCountryCode(1).SetNationalNumber(9002530000L);

        // Too short, but still possible US numbers.
        private static readonly PhoneNumber US_LOCAL_NUMBER =
            new PhoneNumber().SetCountryCode(1).SetNationalNumber(2530000L);

        private static readonly PhoneNumber US_SHORT_BY_ONE_NUMBER =
            new PhoneNumber().SetCountryCode(1).SetNationalNumber(650253000L);

        private static readonly PhoneNumber US_TOLLFREE =
            new PhoneNumber().SetCountryCode(1).SetNationalNumber(8002530000L);

        private static readonly PhoneNumber US_SPOOF =
            new PhoneNumber().SetCountryCode(1).SetNationalNumber(0L);

        private static readonly PhoneNumber US_SPOOF_WITH_RAW_INPUT =
            new PhoneNumber().SetCountryCode(1).SetNationalNumber(0L)
                .SetRawInput("000-000-0000");

        private static readonly PhoneNumber INTERNATIONAL_TOLL_FREE =
            new PhoneNumber().SetCountryCode(800).SetNationalNumber(12345678L);

        // We set this to be the same length as numbers for the other non-geographical country prefix that
        // we have in our test metadata. However, this is not considered valid because they differ in
        // their country calling code.
        private static readonly PhoneNumber INTERNATIONAL_TOLL_FREE_TOO_LONG =
            new PhoneNumber().SetCountryCode(800).SetNationalNumber(123456789L);

        private static readonly PhoneNumber UNIVERSAL_PREMIUM_RATE =
            new PhoneNumber().SetCountryCode(979).SetNationalNumber(123456789L);

        [TestMethod]
        public void testGetSupportedRegions()
        {
            Assert.IsTrue(phoneUtil.getSupportedRegions().Count > 0);
        }

        [TestMethod]
        public void testGetInstanceLoadUSMetadata()
        {
            PhoneMetaData metadata = phoneUtil.getMetadataForRegion(RegionCode.US);
            Assert.AreEqual("US", metadata.ID);
            Assert.AreEqual(1, metadata.CountryCode);
            Assert.AreEqual("011", metadata.InternationalPrefix);
            Assert.IsTrue(metadata.HasNationalPrefix);
            Assert.AreEqual(2, metadata.numberFormatSize);
            Assert.AreEqual("(\\d{3})(\\d{3})(\\d{4})",
                         metadata.getNumberFormat(1).Pattern);
            Assert.AreEqual("$1 $2 $3", metadata.getNumberFormat(1).Format);
            Assert.AreEqual("[13-689]\\d{9}|2[0-35-9]\\d{8}",
                         metadata.GeneralDesc.NationalNumberPattern);
            Assert.AreEqual("\\d{7}(?:\\d{3})?", metadata.GeneralDesc.PossibleNumberPattern);
            Assert.IsTrue(metadata.GeneralDesc.exactlySameAs(metadata.FixedLine));
            Assert.AreEqual("\\d{10}", metadata.TollFree.PossibleNumberPattern);
            Assert.AreEqual("900\\d{7}", metadata.PremiumRate.NationalNumberPattern);
            // No shared-cost data is available, so it should be initialised to "NA".
            Assert.AreEqual("NA", metadata.SharedCost.NationalNumberPattern);
            Assert.AreEqual("NA", metadata.SharedCost.PossibleNumberPattern);
        }

        [TestMethod]
        public void testGetInstanceLoadDEMetadata()
        {
            PhoneMetaData metadata = phoneUtil.getMetadataForRegion(RegionCode.DE);
            Assert.AreEqual("DE", metadata.ID);
            Assert.AreEqual(49, metadata.CountryCode);
            Assert.AreEqual("00", metadata.InternationalPrefix);
            Assert.AreEqual("0", metadata.NationalPrefix);
            Assert.AreEqual(6, metadata.numberFormatSize);
            Assert.AreEqual(1, metadata.getNumberFormat(5).LeadingDigitsPatternSize);
            Assert.AreEqual("900", metadata.getNumberFormat(5).getLeadingDigitsPattern(0));
            Assert.AreEqual("(\\d{3})(\\d{3,4})(\\d{4})",
                         metadata.getNumberFormat(5).Pattern);
            Assert.AreEqual("$1 $2 $3", metadata.getNumberFormat(5).Format);
            Assert.AreEqual("(?:[24-6]\\d{2}|3[03-9]\\d|[789](?:[1-9]\\d|0[2-9]))\\d{1,8}",
                         metadata.FixedLine.NationalNumberPattern);
            Assert.AreEqual("\\d{2,14}", metadata.FixedLine.PossibleNumberPattern);
            Assert.AreEqual("30123456", metadata.FixedLine.ExampleNumber);
            Assert.AreEqual("\\d{10}", metadata.TollFree.PossibleNumberPattern);
            Assert.AreEqual("900([135]\\d{6}|9\\d{7})", metadata.PremiumRate.NationalNumberPattern);
        }

        [TestMethod]
        public void testGetInstanceLoadARMetadata()
        {
            PhoneMetaData metadata = phoneUtil.getMetadataForRegion(RegionCode.AR);
            Assert.AreEqual("AR", metadata.ID);
            Assert.AreEqual(54, metadata.CountryCode);
            Assert.AreEqual("00", metadata.InternationalPrefix);
            Assert.AreEqual("0", metadata.NationalPrefix);
            Assert.AreEqual("0(?:(11|343|3715)15)?", metadata.NationalPrefixForParsing);
            Assert.AreEqual("9$1", metadata.NationalPrefixTransformRule);
            Assert.AreEqual("$2 15 $3-$4", metadata.getNumberFormat(2).Format);
            Assert.AreEqual("(9)(\\d{4})(\\d{2})(\\d{4})",
                         metadata.getNumberFormat(3).Pattern);
            Assert.AreEqual("(9)(\\d{4})(\\d{2})(\\d{4})",
                         metadata.getIntlNumberFormat(3).Pattern);
            Assert.AreEqual("$1 $2 $3 $4", metadata.getIntlNumberFormat(3).Format);
        }

        [TestMethod]
        public void testGetInstanceLoadInternationalTollFreeMetadata()
        {
            PhoneMetaData metadata = phoneUtil.getMetadataForNonGeographicalRegion(800);
            Assert.AreEqual("001", metadata.ID);
            Assert.AreEqual(800, metadata.CountryCode);
            Assert.AreEqual("$1 $2", metadata.getNumberFormat(0).Format);
            Assert.AreEqual("(\\d{4})(\\d{4})", metadata.getNumberFormat(0).Pattern);
            Assert.AreEqual("12345678", metadata.GeneralDesc.ExampleNumber);
            Assert.AreEqual("12345678", metadata.TollFree.ExampleNumber);
        }

        [TestMethod]
        public void testIsLeadingZeroPossible()
        {
            Assert.IsTrue(phoneUtil.isLeadingZeroPossible(39)); // Italy
            Assert.IsFalse(phoneUtil.isLeadingZeroPossible(1)); // USA
            Assert.IsFalse(phoneUtil.isLeadingZeroPossible(800)); // International toll free numbers
            Assert.IsFalse(phoneUtil.isLeadingZeroPossible(888)); // Not in metadata file, just default to
            // false.
        }

        [TestMethod]
        public void testGetLengthOfGeographicalAreaCode()
        {
            // Google MTV, which has area code "650".
            Assert.AreEqual(3, phoneUtil.getLengthOfGeographicalAreaCode(US_NUMBER));

            // A North America toll-free number, which has no area code.
            Assert.AreEqual(0, phoneUtil.getLengthOfGeographicalAreaCode(US_TOLLFREE));

            // Google London, which has area code "20".
            Assert.AreEqual(2, phoneUtil.getLengthOfGeographicalAreaCode(GB_NUMBER));

            // A UK mobile phone, which has no area code.
            Assert.AreEqual(0, phoneUtil.getLengthOfGeographicalAreaCode(GB_MOBILE));

            // Google Buenos Aires, which has area code "11".
            Assert.AreEqual(2, phoneUtil.getLengthOfGeographicalAreaCode(AR_NUMBER));

            // Google Sydney, which has area code "2".
            Assert.AreEqual(1, phoneUtil.getLengthOfGeographicalAreaCode(AU_NUMBER));

            // Italian numbers - there is no national prefix, but it still has an area code.
            Assert.AreEqual(2, phoneUtil.getLengthOfGeographicalAreaCode(IT_NUMBER));

            // Google Singapore. Singapore has no area code and no national prefix.
            Assert.AreEqual(0, phoneUtil.getLengthOfGeographicalAreaCode(SG_NUMBER));

            // An invalid US number (1 digit shorter), which has no area code.
            Assert.AreEqual(0, phoneUtil.getLengthOfGeographicalAreaCode(US_SHORT_BY_ONE_NUMBER));

            // An international toll free number, which has no area code.
            Assert.AreEqual(0, phoneUtil.getLengthOfGeographicalAreaCode(INTERNATIONAL_TOLL_FREE));
        }

        [TestMethod]
        public void testGetLengthOfNationalDestinationCode()
        {
            // Google MTV, which has national destination code (NDC) "650".
            Assert.AreEqual(3, phoneUtil.getLengthOfNationalDestinationCode(US_NUMBER));

            // A North America toll-free number, which has NDC "800".
            Assert.AreEqual(3, phoneUtil.getLengthOfNationalDestinationCode(US_TOLLFREE));

            // Google London, which has NDC "20".
            Assert.AreEqual(2, phoneUtil.getLengthOfNationalDestinationCode(GB_NUMBER));

            // A UK mobile phone, which has NDC "7912".
            Assert.AreEqual(4, phoneUtil.getLengthOfNationalDestinationCode(GB_MOBILE));

            // Google Buenos Aires, which has NDC "11".
            Assert.AreEqual(2, phoneUtil.getLengthOfNationalDestinationCode(AR_NUMBER));

            // An Argentinian mobile which has NDC "911".
            Assert.AreEqual(3, phoneUtil.getLengthOfNationalDestinationCode(AR_MOBILE));

            // Google Sydney, which has NDC "2".
            Assert.AreEqual(1, phoneUtil.getLengthOfNationalDestinationCode(AU_NUMBER));

            // Google Singapore, which has NDC "6521".
            Assert.AreEqual(4, phoneUtil.getLengthOfNationalDestinationCode(SG_NUMBER));

            // An invalid US number (1 digit shorter), which has no NDC.
            Assert.AreEqual(0, phoneUtil.getLengthOfNationalDestinationCode(US_SHORT_BY_ONE_NUMBER));

            // A number containing an invalid country calling code, which shouldn't have any NDC.
            PhoneNumber number = new PhoneNumber().SetCountryCode(123).SetNationalNumber(6502530000L);
            Assert.AreEqual(0, phoneUtil.getLengthOfNationalDestinationCode(number));

            // An international toll free number, which has NDC "1234".
            Assert.AreEqual(4, phoneUtil.getLengthOfNationalDestinationCode(INTERNATIONAL_TOLL_FREE));
        }

        [TestMethod]
        public void testGetNationalSignificantNumber()
        {
            Assert.AreEqual("6502530000", phoneUtil.getNationalSignificantNumber(US_NUMBER));

            // An Italian mobile number.
            Assert.AreEqual("345678901", phoneUtil.getNationalSignificantNumber(IT_MOBILE));

            // An Italian fixed line number.
            Assert.AreEqual("0236618300", phoneUtil.getNationalSignificantNumber(IT_NUMBER));

            Assert.AreEqual("12345678", phoneUtil.getNationalSignificantNumber(INTERNATIONAL_TOLL_FREE));
        }

        [TestMethod]
        public void testGetExampleNumber()
        {
            Assert.AreEqual(DE_NUMBER, phoneUtil.getExampleNumber(RegionCode.DE));

            Assert.AreEqual(DE_NUMBER,
                         phoneUtil.getExampleNumberForType(RegionCode.DE,
                                                           PhoneNumberType.FIXED_LINE));
            Assert.AreEqual(null,
                         phoneUtil.getExampleNumberForType(RegionCode.DE,
                                                           PhoneNumberType.MOBILE));
            // For the US, the example number is placed under general description, and hence should be used
            // for both fixed line and mobile, so neither of these should return null.
            Assert.IsNotNull(phoneUtil.getExampleNumberForType(RegionCode.US,
                                                           PhoneNumberType.FIXED_LINE));
            Assert.IsNotNull(phoneUtil.getExampleNumberForType(RegionCode.US,
                                                           PhoneNumberType.MOBILE));
            // CS is an invalid region, so we have no data for it.
            Assert.IsNull(phoneUtil.getExampleNumberForType(RegionCode.CS,
                                                        PhoneNumberType.MOBILE));
            // RegionCode 001 is reserved for supporting non-geographical country calling code. We don't
            // support getting an example number for it with this method.
            Assert.AreEqual(null, phoneUtil.getExampleNumber(RegionCode.UN001));
        }

        [TestMethod]
        public void testGetExampleNumberForNonGeoEntity()
        {
            Assert.AreEqual(INTERNATIONAL_TOLL_FREE, phoneUtil.getExampleNumberForNonGeoEntity(800));
            Assert.AreEqual(UNIVERSAL_PREMIUM_RATE, phoneUtil.getExampleNumberForNonGeoEntity(979));
        }

        [TestMethod]
        public void testConvertAlphaCharactersInNumber()
        {
            string input = "1800-ABC-DEF";
            // Alpha chars are converted to digits; everything else is left untouched.
            string expectedOutput = "1800-222-333";
            Assert.AreEqual(expectedOutput, PhoneNumberUtil.convertAlphaCharactersInNumber(input));
        }

        [TestMethod]
        public void testNormaliseRemovePunctuation()
        {
            const string inputNumber = "034-56&+#2\u00AD34";
            const string expectedOutput = "03456234";
            Assert.AreEqual(expectedOutput, PhoneNumberUtil.normalize(inputNumber), "Conversion did not correctly remove punctuation");
        }

        [TestMethod]
        public void testNormaliseReplaceAlphaCharacters()
        {
            const string inputNumber = "034-I-am-HUNGRY";
            const string expectedOutput = "034426486479";
            Assert.AreEqual(expectedOutput, PhoneNumberUtil.normalize(inputNumber), "Conversion did not correctly replace alpha characters");
        }

        [TestMethod]
        public void testNormaliseOtherDigits()
        {
            string inputNumber = "\uFF125\u0665";
            string expectedOutput = "255";
            Assert.AreEqual(expectedOutput, PhoneNumberUtil.normalize(inputNumber), "Conversion did not correctly replace non-latin digits");
            // Eastern-Arabic digits.
            inputNumber = "\u06F52\u06F0";
            expectedOutput = "520";
            Assert.AreEqual(expectedOutput, PhoneNumberUtil.normalize(inputNumber), "Conversion did not correctly replace non-latin digits");
        }

        [TestMethod]
        public void testNormaliseStripAlphaCharacters()
        {
            const string inputNumber = "034-56&+a#234";
            const string expectedOutput = "03456234";
            Assert.AreEqual(expectedOutput, PhoneNumberUtil.normalizeDigitsOnly(inputNumber), "Conversion did not correctly remove alpha character");
        }

        [TestMethod]
        public void testFormatUSNumber()
        {
            Assert.AreEqual("650 253 0000", phoneUtil.format(US_NUMBER, PhoneNumberFormat.NATIONAL));
            Assert.AreEqual("+1 650 253 0000", phoneUtil.format(US_NUMBER, PhoneNumberFormat.INTERNATIONAL));

            Assert.AreEqual("800 253 0000", phoneUtil.format(US_TOLLFREE, PhoneNumberFormat.NATIONAL));
            Assert.AreEqual("+1 800 253 0000", phoneUtil.format(US_TOLLFREE, PhoneNumberFormat.INTERNATIONAL));

            Assert.AreEqual("900 253 0000", phoneUtil.format(US_PREMIUM, PhoneNumberFormat.NATIONAL));
            Assert.AreEqual("+1 900 253 0000", phoneUtil.format(US_PREMIUM, PhoneNumberFormat.INTERNATIONAL));
            Assert.AreEqual("tel:+1-900-253-0000", phoneUtil.format(US_PREMIUM, PhoneNumberFormat.RFC3966));
            // Numbers with all zeros in the national number part will be formatted by using the raw_input
            // if that is available no matter which format is specified.
            Assert.AreEqual("000-000-0000",
                         phoneUtil.format(US_SPOOF_WITH_RAW_INPUT, PhoneNumberFormat.NATIONAL));
            Assert.AreEqual("0", phoneUtil.format(US_SPOOF, PhoneNumberFormat.NATIONAL));
        }

        [TestMethod]
        public void testFormatBSNumber()
        {
            Assert.AreEqual("242 365 1234", phoneUtil.format(BS_NUMBER, PhoneNumberFormat.NATIONAL));
            Assert.AreEqual("+1 242 365 1234", phoneUtil.format(BS_NUMBER, PhoneNumberFormat.INTERNATIONAL));
        }

        [TestMethod]
        public void testFormatGBNumber()
        {
            Assert.AreEqual("(020) 7031 3000", phoneUtil.format(GB_NUMBER, PhoneNumberFormat.NATIONAL));
            Assert.AreEqual("+44 20 7031 3000", phoneUtil.format(GB_NUMBER, PhoneNumberFormat.INTERNATIONAL));

            Assert.AreEqual("(07912) 345 678", phoneUtil.format(GB_MOBILE, PhoneNumberFormat.NATIONAL));
            Assert.AreEqual("+44 7912 345 678", phoneUtil.format(GB_MOBILE, PhoneNumberFormat.INTERNATIONAL));
        }

        [TestMethod]
        public void testFormatDENumber()
        {
            PhoneNumber deNumber = new PhoneNumber();
            deNumber.SetCountryCode(49).SetNationalNumber(301234L);
            Assert.AreEqual("030/1234", phoneUtil.format(deNumber, PhoneNumberFormat.NATIONAL));
            Assert.AreEqual("+49 30/1234", phoneUtil.format(deNumber, PhoneNumberFormat.INTERNATIONAL));
            Assert.AreEqual("tel:+49-30-1234", phoneUtil.format(deNumber, PhoneNumberFormat.RFC3966));

            deNumber.Clear();
            deNumber.SetCountryCode(49).SetNationalNumber(291123L);
            Assert.AreEqual("0291 123", phoneUtil.format(deNumber, PhoneNumberFormat.NATIONAL));
            Assert.AreEqual("+49 291 123", phoneUtil.format(deNumber, PhoneNumberFormat.INTERNATIONAL));

            deNumber.Clear();
            deNumber.SetCountryCode(49).SetNationalNumber(29112345678L);
            Assert.AreEqual("0291 12345678", phoneUtil.format(deNumber, PhoneNumberFormat.NATIONAL));
            Assert.AreEqual("+49 291 12345678", phoneUtil.format(deNumber, PhoneNumberFormat.INTERNATIONAL));

            deNumber.Clear();
            deNumber.SetCountryCode(49).SetNationalNumber(912312345L);
            Assert.AreEqual("09123 12345", phoneUtil.format(deNumber, PhoneNumberFormat.NATIONAL));
            Assert.AreEqual("+49 9123 12345", phoneUtil.format(deNumber, PhoneNumberFormat.INTERNATIONAL));
            deNumber.Clear();
            deNumber.SetCountryCode(49).SetNationalNumber(80212345L);
            Assert.AreEqual("08021 2345", phoneUtil.format(deNumber, PhoneNumberFormat.NATIONAL));
            Assert.AreEqual("+49 8021 2345", phoneUtil.format(deNumber, PhoneNumberFormat.INTERNATIONAL));
            // Note this number is correctly formatted without national prefix. Most of the numbers that
            // are treated as invalid numbers by the library are short numbers, and they are usually not
            // dialed with national prefix.
            Assert.AreEqual("1234", phoneUtil.format(DE_SHORT_NUMBER, PhoneNumberFormat.NATIONAL));
            Assert.AreEqual("+49 1234", phoneUtil.format(DE_SHORT_NUMBER, PhoneNumberFormat.INTERNATIONAL));

            deNumber.Clear();
            deNumber.SetCountryCode(49).SetNationalNumber(41341234);
            Assert.AreEqual("04134 1234", phoneUtil.format(deNumber, PhoneNumberFormat.NATIONAL));
        }

        [TestMethod]
        public void testFormatITNumber()
        {
            Assert.AreEqual("02 3661 8300", phoneUtil.format(IT_NUMBER, PhoneNumberFormat.NATIONAL));
            Assert.AreEqual("+39 02 3661 8300", phoneUtil.format(IT_NUMBER, PhoneNumberFormat.INTERNATIONAL));
            Assert.AreEqual("+390236618300", phoneUtil.format(IT_NUMBER, PhoneNumberFormat.E164));

            Assert.AreEqual("345 678 901", phoneUtil.format(IT_MOBILE, PhoneNumberFormat.NATIONAL));
            Assert.AreEqual("+39 345 678 901", phoneUtil.format(IT_MOBILE, PhoneNumberFormat.INTERNATIONAL));
            Assert.AreEqual("+39345678901", phoneUtil.format(IT_MOBILE, PhoneNumberFormat.E164));
        }

        [TestMethod]
        public void testFormatAUNumber()
        {
            Assert.AreEqual("02 3661 8300", phoneUtil.format(AU_NUMBER, PhoneNumberFormat.NATIONAL));
            Assert.AreEqual("+61 2 3661 8300", phoneUtil.format(AU_NUMBER, PhoneNumberFormat.INTERNATIONAL));
            Assert.AreEqual("+61236618300", phoneUtil.format(AU_NUMBER, PhoneNumberFormat.E164));

            PhoneNumber auNumber = new PhoneNumber().SetCountryCode(61).SetNationalNumber(1800123456L);
            Assert.AreEqual("1800 123 456", phoneUtil.format(auNumber, PhoneNumberFormat.NATIONAL));
            Assert.AreEqual("+61 1800 123 456", phoneUtil.format(auNumber, PhoneNumberFormat.INTERNATIONAL));
            Assert.AreEqual("+611800123456", phoneUtil.format(auNumber, PhoneNumberFormat.E164));
        }

        [TestMethod]
        public void testFormatARNumber()
        {
            Assert.AreEqual("011 8765-4321", phoneUtil.format(AR_NUMBER, PhoneNumberFormat.NATIONAL));
            Assert.AreEqual("+54 11 8765-4321", phoneUtil.format(AR_NUMBER, PhoneNumberFormat.INTERNATIONAL));
            Assert.AreEqual("+541187654321", phoneUtil.format(AR_NUMBER, PhoneNumberFormat.E164));

            Assert.AreEqual("011 15 8765-4321", phoneUtil.format(AR_MOBILE, PhoneNumberFormat.NATIONAL));
            Assert.AreEqual("+54 9 11 8765 4321", phoneUtil.format(AR_MOBILE,
                                                                PhoneNumberFormat.INTERNATIONAL));
            Assert.AreEqual("+5491187654321", phoneUtil.format(AR_MOBILE, PhoneNumberFormat.E164));
        }

        [TestMethod]
        public void testFormatMXNumber()
        {
            Assert.AreEqual("045 234 567 8900", phoneUtil.format(MX_MOBILE1, PhoneNumberFormat.NATIONAL));
            Assert.AreEqual("+52 1 234 567 8900", phoneUtil.format(
                MX_MOBILE1, PhoneNumberFormat.INTERNATIONAL));
            Assert.AreEqual("+5212345678900", phoneUtil.format(MX_MOBILE1, PhoneNumberFormat.E164));

            Assert.AreEqual("045 55 1234 5678", phoneUtil.format(MX_MOBILE2, PhoneNumberFormat.NATIONAL));
            Assert.AreEqual("+52 1 55 1234 5678", phoneUtil.format(
                MX_MOBILE2, PhoneNumberFormat.INTERNATIONAL));
            Assert.AreEqual("+5215512345678", phoneUtil.format(MX_MOBILE2, PhoneNumberFormat.E164));

            Assert.AreEqual("01 33 1234 5678", phoneUtil.format(MX_NUMBER1, PhoneNumberFormat.NATIONAL));
            Assert.AreEqual("+52 33 1234 5678", phoneUtil.format(MX_NUMBER1, PhoneNumberFormat.INTERNATIONAL));
            Assert.AreEqual("+523312345678", phoneUtil.format(MX_NUMBER1, PhoneNumberFormat.E164));

            Assert.AreEqual("01 821 123 4567", phoneUtil.format(MX_NUMBER2, PhoneNumberFormat.NATIONAL));
            Assert.AreEqual("+52 821 123 4567", phoneUtil.format(MX_NUMBER2, PhoneNumberFormat.INTERNATIONAL));
            Assert.AreEqual("+528211234567", phoneUtil.format(MX_NUMBER2, PhoneNumberFormat.E164));
        }

        [TestMethod]
        public void testFormatOutOfCountryCallingNumber()
        {
            Assert.AreEqual("00 1 900 253 0000",
                         phoneUtil.formatOutOfCountryCallingNumber(US_PREMIUM, RegionCode.DE));

            Assert.AreEqual("1 650 253 0000",
                         phoneUtil.formatOutOfCountryCallingNumber(US_NUMBER, RegionCode.BS));

            Assert.AreEqual("00 1 650 253 0000",
                         phoneUtil.formatOutOfCountryCallingNumber(US_NUMBER, RegionCode.PL));

            Assert.AreEqual("011 44 7912 345 678",
                         phoneUtil.formatOutOfCountryCallingNumber(GB_MOBILE, RegionCode.US));

            Assert.AreEqual("00 49 1234",
                         phoneUtil.formatOutOfCountryCallingNumber(DE_SHORT_NUMBER, RegionCode.GB));
            // Note this number is correctly formatted without national prefix. Most of the numbers that
            // are treated as invalid numbers by the library are short numbers, and they are usually not
            // dialed with national prefix.
            Assert.AreEqual("1234", phoneUtil.formatOutOfCountryCallingNumber(DE_SHORT_NUMBER, RegionCode.DE));

            Assert.AreEqual("011 39 02 3661 8300",
                         phoneUtil.formatOutOfCountryCallingNumber(IT_NUMBER, RegionCode.US));
            Assert.AreEqual("02 3661 8300",
                         phoneUtil.formatOutOfCountryCallingNumber(IT_NUMBER, RegionCode.IT));
            Assert.AreEqual("+39 02 3661 8300",
                         phoneUtil.formatOutOfCountryCallingNumber(IT_NUMBER, RegionCode.SG));

            Assert.AreEqual("6521 8000",
                         phoneUtil.formatOutOfCountryCallingNumber(SG_NUMBER, RegionCode.SG));

            Assert.AreEqual("011 54 9 11 8765 4321",
                         phoneUtil.formatOutOfCountryCallingNumber(AR_MOBILE, RegionCode.US));
            Assert.AreEqual("011 800 1234 5678",
                         phoneUtil.formatOutOfCountryCallingNumber(INTERNATIONAL_TOLL_FREE, RegionCode.US));

            PhoneNumber arNumberWithExtn = new PhoneNumber().MergeFrom(AR_MOBILE).SetExtension("1234");
            Assert.AreEqual("011 54 9 11 8765 4321 ext. 1234",
                         phoneUtil.formatOutOfCountryCallingNumber(arNumberWithExtn, RegionCode.US));
            Assert.AreEqual("0011 54 9 11 8765 4321 ext. 1234",
                         phoneUtil.formatOutOfCountryCallingNumber(arNumberWithExtn, RegionCode.AU));
            Assert.AreEqual("011 15 8765-4321 ext. 1234",
                         phoneUtil.formatOutOfCountryCallingNumber(arNumberWithExtn, RegionCode.AR));
        }

        [TestMethod]
        public void testFormatOutOfCountryWithInvalidRegion()
        {
            // AQ/Antarctica isn't a valid region code for phone number formatting,
            // so this falls back to intl formatting.
            Assert.AreEqual("+1 650 253 0000",
                         phoneUtil.formatOutOfCountryCallingNumber(US_NUMBER, RegionCode.AQ));
            // For region code 001, the out-of-country format always turns into the international format.
            Assert.AreEqual("+1 650 253 0000",
                         phoneUtil.formatOutOfCountryCallingNumber(US_NUMBER, RegionCode.UN001));
        }

        [TestMethod]
        public void testFormatOutOfCountryWithPreferredIntlPrefix()
        {
            // This should use 0011, since that is the preferred international prefix (both 0011 and 0012
            // are accepted as possible international prefixes in our test metadta.)
            Assert.AreEqual("0011 39 02 3661 8300",
                         phoneUtil.formatOutOfCountryCallingNumber(IT_NUMBER, RegionCode.AU));
        }

        [TestMethod]
        public void testFormatOutOfCountryKeepingAlphaChars()
        {
            PhoneNumber alphaNumericNumber = new PhoneNumber();
            alphaNumericNumber.SetCountryCode(1).SetNationalNumber(8007493524L)
                .SetRawInput("1800 six-flag");
            Assert.AreEqual("0011 1 800 SIX-FLAG",
                         phoneUtil.formatOutOfCountryKeepingAlphaChars(alphaNumericNumber, RegionCode.AU));

            alphaNumericNumber.SetRawInput("1-800-SIX-flag");
            Assert.AreEqual("0011 1 800-SIX-FLAG",
                         phoneUtil.formatOutOfCountryKeepingAlphaChars(alphaNumericNumber, RegionCode.AU));

            alphaNumericNumber.SetRawInput("Call us from UK: 00 1 800 SIX-flag");
            Assert.AreEqual("0011 1 800 SIX-FLAG",
                         phoneUtil.formatOutOfCountryKeepingAlphaChars(alphaNumericNumber, RegionCode.AU));

            alphaNumericNumber.SetRawInput("800 SIX-flag");
            Assert.AreEqual("0011 1 800 SIX-FLAG",
                         phoneUtil.formatOutOfCountryKeepingAlphaChars(alphaNumericNumber, RegionCode.AU));

            // Formatting from within the NANPA region.
            Assert.AreEqual("1 800 SIX-FLAG",
                         phoneUtil.formatOutOfCountryKeepingAlphaChars(alphaNumericNumber, RegionCode.US));

            Assert.AreEqual("1 800 SIX-FLAG",
                         phoneUtil.formatOutOfCountryKeepingAlphaChars(alphaNumericNumber, RegionCode.BS));

            // Testing that if the raw input doesn't exist, it is formatted using
            // formatOutOfCountryCallingNumber.
            alphaNumericNumber.ClearRawInput();
            Assert.AreEqual("00 1 800 749 3524",
                         phoneUtil.formatOutOfCountryKeepingAlphaChars(alphaNumericNumber, RegionCode.DE));

            // Testing AU alpha number formatted from Australia.
            alphaNumericNumber.SetCountryCode(61).SetNationalNumber(827493524L)
                .SetRawInput("+61 82749-FLAG");
            // This number should have the national prefix fixed.
            Assert.AreEqual("082749-FLAG",
                         phoneUtil.formatOutOfCountryKeepingAlphaChars(alphaNumericNumber, RegionCode.AU));

            alphaNumericNumber.SetRawInput("082749-FLAG");
            Assert.AreEqual("082749-FLAG",
                         phoneUtil.formatOutOfCountryKeepingAlphaChars(alphaNumericNumber, RegionCode.AU));

            alphaNumericNumber.SetNationalNumber(18007493524L).SetRawInput("1-800-SIX-flag");
            // This number should not have the national prefix prefixed, in accordance with the override for
            // this specific formatting rule.
            Assert.AreEqual("1-800-SIX-FLAG",
                         phoneUtil.formatOutOfCountryKeepingAlphaChars(alphaNumericNumber, RegionCode.AU));

            // The metadata should not be permanently changed, since we copied it before modifying patterns.
            // Here we check this.
            alphaNumericNumber.SetNationalNumber(1800749352L);
            Assert.AreEqual("1800 749 352",
                         phoneUtil.formatOutOfCountryCallingNumber(alphaNumericNumber, RegionCode.AU));

            // Testing a region with multiple international prefixes.
            Assert.AreEqual("+61 1-800-SIX-FLAG",
                         phoneUtil.formatOutOfCountryKeepingAlphaChars(alphaNumericNumber, RegionCode.SG));
            // Testing the case of calling from a non-supported region.
            Assert.AreEqual("+61 1-800-SIX-FLAG",
                         phoneUtil.formatOutOfCountryKeepingAlphaChars(alphaNumericNumber, RegionCode.AQ));

            // Testing the case with an invalid country calling code.
            alphaNumericNumber.SetCountryCode(0).SetNationalNumber(18007493524L)
                .SetRawInput("1-800-SIX-flag");
            // Uses the raw input only.
            Assert.AreEqual("1-800-SIX-flag",
                         phoneUtil.formatOutOfCountryKeepingAlphaChars(alphaNumericNumber, RegionCode.DE));

            // Testing the case of an invalid alpha number.
            alphaNumericNumber.SetCountryCode(1).SetNationalNumber(80749L).SetRawInput("180-SIX");
            // No country-code stripping can be done.
            Assert.AreEqual("00 1 180-SIX",
                         phoneUtil.formatOutOfCountryKeepingAlphaChars(alphaNumericNumber, RegionCode.DE));

            // Testing the case of calling from a non-supported region.
            alphaNumericNumber.SetCountryCode(1).SetNationalNumber(80749L).SetRawInput("180-SIX");
            // No country-code stripping can be done since the number is invalid.
            Assert.AreEqual("+1 180-SIX",
                         phoneUtil.formatOutOfCountryKeepingAlphaChars(alphaNumericNumber, RegionCode.AQ));
        }

        [TestMethod]
        public void testFormatWithCarrierCode()
        {
            // We only support this for AR in our test metadata, and only for mobile numbers starting with
            // certain values.
            PhoneNumber arMobile = new PhoneNumber().SetCountryCode(54).SetNationalNumber(92234654321L);
            Assert.AreEqual("02234 65-4321", phoneUtil.format(arMobile, PhoneNumberFormat.NATIONAL));
            // Here we force 14 as the carrier code.
            Assert.AreEqual("02234 14 65-4321",
                         phoneUtil.formatNationalNumberWithCarrierCode(arMobile, "14"));
            // Here we force the number to be shown with no carrier code.
            Assert.AreEqual("02234 65-4321",
                         phoneUtil.formatNationalNumberWithCarrierCode(arMobile, ""));
            // Here the international rule is used, so no carrier code should be present.
            Assert.AreEqual("+5492234654321", phoneUtil.format(arMobile, PhoneNumberFormat.E164));
            // We don't support this for the US so there should be no change.
            Assert.AreEqual("650 253 0000", phoneUtil.formatNationalNumberWithCarrierCode(US_NUMBER, "15"));
        }

        [TestMethod]
        public void testFormatWithPreferredCarrierCode()
        {
            // We only support this for AR in our test metadata.
            var arNumber = new PhoneNumber();
            arNumber.SetCountryCode(54).SetNationalNumber(91234125678L);
            // Test formatting with no preferred carrier code stored in the number itself.
            Assert.AreEqual("01234 15 12-5678",
                         phoneUtil.formatNationalNumberWithPreferredCarrierCode(arNumber, "15"));
            Assert.AreEqual("01234 12-5678",
                         phoneUtil.formatNationalNumberWithPreferredCarrierCode(arNumber, ""));
            // Test formatting with preferred carrier code present.
            arNumber.SetPreferredDomesticCarrierCode("19");
            Assert.AreEqual("01234 12-5678", phoneUtil.format(arNumber, PhoneNumberFormat.NATIONAL));
            Assert.AreEqual("01234 19 12-5678",
                         phoneUtil.formatNationalNumberWithPreferredCarrierCode(arNumber, "15"));
            Assert.AreEqual("01234 19 12-5678",
                         phoneUtil.formatNationalNumberWithPreferredCarrierCode(arNumber, ""));
            // When the preferred_domestic_carrier_code is present (even when it contains an empty string),
            // use it instead of the default carrier code passed in.
            arNumber.SetPreferredDomesticCarrierCode("");
            Assert.AreEqual("01234 12-5678",
                         phoneUtil.formatNationalNumberWithPreferredCarrierCode(arNumber, "15"));
            // We don't support this for the US so there should be no change.
            var usNumber = new PhoneNumber();
            usNumber.SetCountryCode(1).SetNationalNumber(4241231234L).SetPreferredDomesticCarrierCode("99");
            Assert.AreEqual("424 123 1234", phoneUtil.format(usNumber, PhoneNumberFormat.NATIONAL));
            Assert.AreEqual("424 123 1234",
                         phoneUtil.formatNationalNumberWithPreferredCarrierCode(usNumber, "15"));
        }

        [TestMethod]
        public void testFormatNumberForMobileDialing()
        {
            // US toll free numbers are marked as noInternationalDialling in the test metadata for testing
            // purposes.
            Assert.AreEqual("800 253 0000",
                         phoneUtil.formatNumberForMobileDialing(US_TOLLFREE, RegionCode.US,
                                                                true /*  keep formatting */));
            Assert.AreEqual("", phoneUtil.formatNumberForMobileDialing(US_TOLLFREE, RegionCode.CN, true));
            Assert.AreEqual("+1 650 253 0000",
                         phoneUtil.formatNumberForMobileDialing(US_NUMBER, RegionCode.US, true));
            PhoneNumber usNumberWithExtn = new PhoneNumber().MergeFrom(US_NUMBER).SetExtension("1234");
            Assert.AreEqual("+1 650 253 0000",
                         phoneUtil.formatNumberForMobileDialing(usNumberWithExtn, RegionCode.US, true));

            Assert.AreEqual("8002530000",
                         phoneUtil.formatNumberForMobileDialing(US_TOLLFREE, RegionCode.US,
                                                                false /* remove formatting */));
            Assert.AreEqual("", phoneUtil.formatNumberForMobileDialing(US_TOLLFREE, RegionCode.CN, false));
            Assert.AreEqual("+16502530000",
                         phoneUtil.formatNumberForMobileDialing(US_NUMBER, RegionCode.US, false));
            Assert.AreEqual("+16502530000",
                         phoneUtil.formatNumberForMobileDialing(usNumberWithExtn, RegionCode.US, false));

            // An invalid US number, which is one digit too long.
            Assert.AreEqual("+165025300001",
                         phoneUtil.formatNumberForMobileDialing(US_LONG_NUMBER, RegionCode.US, false));
            Assert.AreEqual("+1 65025300001",
                         phoneUtil.formatNumberForMobileDialing(US_LONG_NUMBER, RegionCode.US, true));

            // Star numbers. In real life they appear in Israel, but we have them in JP in our test
            // metadata.
            Assert.AreEqual("*2345",
                         phoneUtil.formatNumberForMobileDialing(JP_STAR_NUMBER, RegionCode.JP, false));
            Assert.AreEqual("*2345",
                         phoneUtil.formatNumberForMobileDialing(JP_STAR_NUMBER, RegionCode.JP, true));

            Assert.AreEqual("+80012345678",
                         phoneUtil.formatNumberForMobileDialing(INTERNATIONAL_TOLL_FREE, RegionCode.JP, false));
            Assert.AreEqual("+800 1234 5678",
                         phoneUtil.formatNumberForMobileDialing(INTERNATIONAL_TOLL_FREE, RegionCode.JP, true));
        }

        [TestMethod]
        public void testFormatByPattern()
        {
            PhoneMetaDataNumberFormat newNumFormat = new PhoneMetaDataNumberFormat();
            newNumFormat.SetPattern("(\\d{3})(\\d{3})(\\d{4})");
            newNumFormat.SetFormat("($1) $2-$3");
            IList<PhoneMetaDataNumberFormat> newNumberFormats = new List<PhoneMetaDataNumberFormat>();
            newNumberFormats.Add(newNumFormat);

            Assert.AreEqual("(650) 253-0000", phoneUtil.formatByPattern(US_NUMBER, PhoneNumberFormat.NATIONAL,
                                                                     newNumberFormats));
            Assert.AreEqual("+1 (650) 253-0000", phoneUtil.formatByPattern(US_NUMBER,
                                                                        PhoneNumberFormat.INTERNATIONAL,
                                                                        newNumberFormats));
            Assert.AreEqual("tel:+1-650-253-0000", phoneUtil.formatByPattern(US_NUMBER,
                                                                          PhoneNumberFormat.RFC3966,
                                                                          newNumberFormats));

            // $NP is set to '1' for the US. Here we check that for other NANPA countries the US rules are
            // followed.
            newNumFormat.setNationalPrefixFormattingRule("$NP ($FG)");
            newNumFormat.SetFormat("$1 $2-$3");
            Assert.AreEqual("1 (242) 365-1234",
                         phoneUtil.formatByPattern(BS_NUMBER, PhoneNumberFormat.NATIONAL,
                                                   newNumberFormats));
            Assert.AreEqual("+1 242 365-1234",
                         phoneUtil.formatByPattern(BS_NUMBER, PhoneNumberFormat.INTERNATIONAL,
                                                   newNumberFormats));

            newNumFormat.SetPattern("(\\d{2})(\\d{5})(\\d{3})");
            newNumFormat.SetFormat("$1-$2 $3");
            newNumberFormats[0] = newNumFormat;

            Assert.AreEqual("02-36618 300",
                         phoneUtil.formatByPattern(IT_NUMBER, PhoneNumberFormat.NATIONAL,
                                                   newNumberFormats));
            Assert.AreEqual("+39 02-36618 300",
                         phoneUtil.formatByPattern(IT_NUMBER, PhoneNumberFormat.INTERNATIONAL,
                                                   newNumberFormats));

            newNumFormat.setNationalPrefixFormattingRule("$NP$FG");
            newNumFormat.SetPattern("(\\d{2})(\\d{4})(\\d{4})");
            newNumFormat.SetFormat("$1 $2 $3");
            newNumberFormats[0] = newNumFormat;
            Assert.AreEqual("020 7031 3000",
                         phoneUtil.formatByPattern(GB_NUMBER, PhoneNumberFormat.NATIONAL,
                                                   newNumberFormats));

            newNumFormat.setNationalPrefixFormattingRule("($NP$FG)");
            Assert.AreEqual("(020) 7031 3000",
                         phoneUtil.formatByPattern(GB_NUMBER, PhoneNumberFormat.NATIONAL,
                                                   newNumberFormats));

            newNumFormat.setNationalPrefixFormattingRule("");
            Assert.AreEqual("20 7031 3000",
                         phoneUtil.formatByPattern(GB_NUMBER, PhoneNumberFormat.NATIONAL,
                                                   newNumberFormats));

            Assert.AreEqual("+44 20 7031 3000",
                         phoneUtil.formatByPattern(GB_NUMBER, PhoneNumberFormat.INTERNATIONAL,
                                                   newNumberFormats));
        }

        [TestMethod]
        public void testFormatE164Number()
        {
            Assert.AreEqual("+16502530000", phoneUtil.format(US_NUMBER, PhoneNumberFormat.E164));
            Assert.AreEqual("+4930123456", phoneUtil.format(DE_NUMBER, PhoneNumberFormat.E164));
            Assert.AreEqual("+80012345678", phoneUtil.format(INTERNATIONAL_TOLL_FREE, PhoneNumberFormat.E164));
        }

        [TestMethod]
        public void testFormatNumberWithExtension()
        {
            PhoneNumber nzNumber = new PhoneNumber().MergeFrom(NZ_NUMBER).SetExtension("1234");
            // Uses default extension prefix:
            Assert.AreEqual("03-331 6005 ext. 1234", phoneUtil.format(nzNumber, PhoneNumberFormat.NATIONAL));
            // Uses RFC 3966 syntax.
            Assert.AreEqual("tel:+64-3-331-6005;ext=1234",
                         phoneUtil.format(nzNumber, PhoneNumberFormat.RFC3966));
            // Extension prefix overridden in the territory information for the US:
            PhoneNumber usNumberWithExtension = new PhoneNumber().MergeFrom(US_NUMBER).SetExtension("4567");
            Assert.AreEqual("650 253 0000 extn. 4567", phoneUtil.format(usNumberWithExtension,
                                                                     PhoneNumberFormat.NATIONAL));
        }

        [TestMethod]
        public void testFormatInOriginalFormat()
        {
            PhoneNumber number1 = phoneUtil.parseAndKeepRawInput("+442087654321", RegionCode.GB);
            Assert.AreEqual("+44 20 8765 4321", phoneUtil.formatInOriginalFormat(number1, RegionCode.GB));

            PhoneNumber number2 = phoneUtil.parseAndKeepRawInput("02087654321", RegionCode.GB);
            Assert.AreEqual("(020) 8765 4321", phoneUtil.formatInOriginalFormat(number2, RegionCode.GB));

            PhoneNumber number3 = phoneUtil.parseAndKeepRawInput("011442087654321", RegionCode.US);
            Assert.AreEqual("011 44 20 8765 4321", phoneUtil.formatInOriginalFormat(number3, RegionCode.US));

            PhoneNumber number4 = phoneUtil.parseAndKeepRawInput("442087654321", RegionCode.GB);
            Assert.AreEqual("44 20 8765 4321", phoneUtil.formatInOriginalFormat(number4, RegionCode.GB));

            PhoneNumber number5 = phoneUtil.parse("+442087654321", RegionCode.GB);
            Assert.AreEqual("(020) 8765 4321", phoneUtil.formatInOriginalFormat(number5, RegionCode.GB));

            // Invalid numbers that we have a formatting pattern for should be formatted properly. Note area
            // codes starting with 7 are intentionally excluded in the test metadata for testing purposes.
            PhoneNumber number6 = phoneUtil.parseAndKeepRawInput("7345678901", RegionCode.US);
            Assert.AreEqual("734 567 8901", phoneUtil.formatInOriginalFormat(number6, RegionCode.US));

            // US is not a leading zero country, and the presence of the leading zero leads us to format the
            // number using raw_input.
            PhoneNumber number7 = phoneUtil.parseAndKeepRawInput("0734567 8901", RegionCode.US);
            Assert.AreEqual("0734567 8901", phoneUtil.formatInOriginalFormat(number7, RegionCode.US));

            // This number is valid, but we don't have a formatting pattern for it. Fall back to the raw
            // input.
            PhoneNumber number8 = phoneUtil.parseAndKeepRawInput("02-4567-8900", RegionCode.KR);
            Assert.AreEqual("02-4567-8900", phoneUtil.formatInOriginalFormat(number8, RegionCode.KR));

            PhoneNumber number9 = phoneUtil.parseAndKeepRawInput("01180012345678", RegionCode.US);
            Assert.AreEqual("011 800 1234 5678", phoneUtil.formatInOriginalFormat(number9, RegionCode.US));

            PhoneNumber number10 = phoneUtil.parseAndKeepRawInput("+80012345678", RegionCode.KR);
            Assert.AreEqual("+800 1234 5678", phoneUtil.formatInOriginalFormat(number10, RegionCode.KR));

            // US local numbers are formatted correctly, as we have formatting patterns for them.
            PhoneNumber localNumberUS = phoneUtil.parseAndKeepRawInput("2530000", RegionCode.US);
            Assert.AreEqual("253 0000", phoneUtil.formatInOriginalFormat(localNumberUS, RegionCode.US));

            PhoneNumber numberWithNationalPrefixUS =
                phoneUtil.parseAndKeepRawInput("18003456789", RegionCode.US);
            Assert.AreEqual("1 800 345 6789",
                         phoneUtil.formatInOriginalFormat(numberWithNationalPrefixUS, RegionCode.US));

            PhoneNumber numberWithoutNationalPrefixGB =
                phoneUtil.parseAndKeepRawInput("2087654321", RegionCode.GB);
            Assert.AreEqual("20 8765 4321",
                         phoneUtil.formatInOriginalFormat(numberWithoutNationalPrefixGB, RegionCode.GB));
            // Make sure no metadata is modified as a result of the previous function call.
            Assert.AreEqual("(020) 8765 4321", phoneUtil.formatInOriginalFormat(number5, RegionCode.GB));

            PhoneNumber numberWithNationalPrefixMX =
                phoneUtil.parseAndKeepRawInput("013312345678", RegionCode.MX);
            Assert.AreEqual("01 33 1234 5678",
                         phoneUtil.formatInOriginalFormat(numberWithNationalPrefixMX, RegionCode.MX));

            PhoneNumber numberWithoutNationalPrefixMX =
                phoneUtil.parseAndKeepRawInput("3312345678", RegionCode.MX);
            Assert.AreEqual("33 1234 5678",
                         phoneUtil.formatInOriginalFormat(numberWithoutNationalPrefixMX, RegionCode.MX));

            PhoneNumber italianFixedLineNumber =
                phoneUtil.parseAndKeepRawInput("0212345678", RegionCode.IT);
            Assert.AreEqual("02 1234 5678",
                         phoneUtil.formatInOriginalFormat(italianFixedLineNumber, RegionCode.IT));

            PhoneNumber numberWithNationalPrefixJP =
                phoneUtil.parseAndKeepRawInput("00777012", RegionCode.JP);
            Assert.AreEqual("0077-7012",
                         phoneUtil.formatInOriginalFormat(numberWithNationalPrefixJP, RegionCode.JP));

            PhoneNumber numberWithoutNationalPrefixJP =
                phoneUtil.parseAndKeepRawInput("0777012", RegionCode.JP);
            Assert.AreEqual("0777012",
                         phoneUtil.formatInOriginalFormat(numberWithoutNationalPrefixJP, RegionCode.JP));

            PhoneNumber numberWithCarrierCodeBR =
                phoneUtil.parseAndKeepRawInput("012 3121286979", RegionCode.BR);
            Assert.AreEqual("012 3121286979",
                         phoneUtil.formatInOriginalFormat(numberWithCarrierCodeBR, RegionCode.BR));

            // The default national prefix used in this case is 045. When a number with national prefix 044
            // is entered, we return the raw input as we don't want to change the number entered.
            PhoneNumber numberWithNationalPrefixMX1 =
                phoneUtil.parseAndKeepRawInput("044(33)1234-5678", RegionCode.MX);
            Assert.AreEqual("044(33)1234-5678",
                         phoneUtil.formatInOriginalFormat(numberWithNationalPrefixMX1, RegionCode.MX));

            PhoneNumber numberWithNationalPrefixMX2 =
                phoneUtil.parseAndKeepRawInput("045(33)1234-5678", RegionCode.MX);
            Assert.AreEqual("045 33 1234 5678",
                         phoneUtil.formatInOriginalFormat(numberWithNationalPrefixMX2, RegionCode.MX));

            // The default international prefix used in this case is 0011. When a number with international
            // prefix 0012 is entered, we return the raw input as we don't want to change the number
            // entered.
            PhoneNumber outOfCountryNumberFromAU1 =
                phoneUtil.parseAndKeepRawInput("0012 16502530000", RegionCode.AU);
            Assert.AreEqual("0012 16502530000",
                         phoneUtil.formatInOriginalFormat(outOfCountryNumberFromAU1, RegionCode.AU));

            PhoneNumber outOfCountryNumberFromAU2 =
                phoneUtil.parseAndKeepRawInput("0011 16502530000", RegionCode.AU);
            Assert.AreEqual("0011 1 650 253 0000",
                         phoneUtil.formatInOriginalFormat(outOfCountryNumberFromAU2, RegionCode.AU));

            // Test the star sign is not removed from or added to the original input by this method.
            PhoneNumber starNumber = phoneUtil.parseAndKeepRawInput("*1234", RegionCode.JP);
            Assert.AreEqual("*1234", phoneUtil.formatInOriginalFormat(starNumber, RegionCode.JP));
            PhoneNumber numberWithoutStar = phoneUtil.parseAndKeepRawInput("1234", RegionCode.JP);
            Assert.AreEqual("1234", phoneUtil.formatInOriginalFormat(numberWithoutStar, RegionCode.JP));
        }

        [TestMethod]
        public void testIsPremiumRate()
        {
            Assert.AreEqual(PhoneNumberType.PREMIUM_RATE, phoneUtil.getNumberType(US_PREMIUM));

            PhoneNumber premiumRateNumber = new PhoneNumber();
            premiumRateNumber.SetCountryCode(39).SetNationalNumber(892123L);
            Assert.AreEqual(PhoneNumberType.PREMIUM_RATE,
                         phoneUtil.getNumberType(premiumRateNumber));

            premiumRateNumber.Clear();
            premiumRateNumber.SetCountryCode(44).SetNationalNumber(9187654321L);
            Assert.AreEqual(PhoneNumberType.PREMIUM_RATE,
                         phoneUtil.getNumberType(premiumRateNumber));

            premiumRateNumber.Clear();
            premiumRateNumber.SetCountryCode(49).SetNationalNumber(9001654321L);
            Assert.AreEqual(PhoneNumberType.PREMIUM_RATE,
                         phoneUtil.getNumberType(premiumRateNumber));

            premiumRateNumber.Clear();
            premiumRateNumber.SetCountryCode(49).SetNationalNumber(90091234567L);
            Assert.AreEqual(PhoneNumberType.PREMIUM_RATE,
                         phoneUtil.getNumberType(premiumRateNumber));

            Assert.AreEqual(PhoneNumberType.PREMIUM_RATE,
                         phoneUtil.getNumberType(UNIVERSAL_PREMIUM_RATE));
        }

        [TestMethod]
        public void testIsTollFree()
        {
            var tollFreeNumber = new PhoneNumber();

            tollFreeNumber.SetCountryCode(1).SetNationalNumber(8881234567L);
            Assert.AreEqual(PhoneNumberType.TOLL_FREE,
                         phoneUtil.getNumberType(tollFreeNumber));

            tollFreeNumber.Clear();
            tollFreeNumber.SetCountryCode(39).SetNationalNumber(803123L);
            Assert.AreEqual(PhoneNumberType.TOLL_FREE,
                         phoneUtil.getNumberType(tollFreeNumber));

            tollFreeNumber.Clear();
            tollFreeNumber.SetCountryCode(44).SetNationalNumber(8012345678L);
            Assert.AreEqual(PhoneNumberType.TOLL_FREE,
                         phoneUtil.getNumberType(tollFreeNumber));

            tollFreeNumber.Clear();
            tollFreeNumber.SetCountryCode(49).SetNationalNumber(8001234567L);
            Assert.AreEqual(PhoneNumberType.TOLL_FREE,
                         phoneUtil.getNumberType(tollFreeNumber));

            Assert.AreEqual(PhoneNumberType.TOLL_FREE,
                         phoneUtil.getNumberType(INTERNATIONAL_TOLL_FREE));
        }

        [TestMethod]
        public void testIsMobile()
        {
            Assert.AreEqual(PhoneNumberType.MOBILE, phoneUtil.getNumberType(BS_MOBILE));
            Assert.AreEqual(PhoneNumberType.MOBILE, phoneUtil.getNumberType(GB_MOBILE));
            Assert.AreEqual(PhoneNumberType.MOBILE, phoneUtil.getNumberType(IT_MOBILE));
            Assert.AreEqual(PhoneNumberType.MOBILE, phoneUtil.getNumberType(AR_MOBILE));

            PhoneNumber mobileNumber = new PhoneNumber();
            mobileNumber.SetCountryCode(49).SetNationalNumber(15123456789L);
            Assert.AreEqual(PhoneNumberType.MOBILE, phoneUtil.getNumberType(mobileNumber));
        }

        [TestMethod]
        public void testIsFixedLine()
        {
            Assert.AreEqual(PhoneNumberType.FIXED_LINE, phoneUtil.getNumberType(BS_NUMBER));
            Assert.AreEqual(PhoneNumberType.FIXED_LINE, phoneUtil.getNumberType(IT_NUMBER));
            Assert.AreEqual(PhoneNumberType.FIXED_LINE, phoneUtil.getNumberType(GB_NUMBER));
            Assert.AreEqual(PhoneNumberType.FIXED_LINE, phoneUtil.getNumberType(DE_NUMBER));
        }

        [TestMethod]
        public void testIsFixedLineAndMobile()
        {
            Assert.AreEqual(PhoneNumberType.FIXED_LINE_OR_MOBILE,
                         phoneUtil.getNumberType(US_NUMBER));

            PhoneNumber fixedLineAndMobileNumber = new PhoneNumber().
                SetCountryCode(54).SetNationalNumber(1987654321L);
            Assert.AreEqual(PhoneNumberType.FIXED_LINE_OR_MOBILE,
                         phoneUtil.getNumberType(fixedLineAndMobileNumber));
        }

        [TestMethod]
        public void testIsSharedCost()
        {
            PhoneNumber gbNumber = new PhoneNumber();
            gbNumber.SetCountryCode(44).SetNationalNumber(8431231234L);
            Assert.AreEqual(PhoneNumberType.SHARED_COST, phoneUtil.getNumberType(gbNumber));
        }

        [TestMethod]
        public void testIsVoip()
        {
            PhoneNumber gbNumber = new PhoneNumber();
            gbNumber.SetCountryCode(44).SetNationalNumber(5631231234L);
            Assert.AreEqual(PhoneNumberType.VOIP, phoneUtil.getNumberType(gbNumber));
        }

        [TestMethod]
        public void testIsPersonalNumber()
        {
            PhoneNumber gbNumber = new PhoneNumber();
            gbNumber.SetCountryCode(44).SetNationalNumber(7031231234L);
            Assert.AreEqual(PhoneNumberType.PERSONAL_NUMBER,
                         phoneUtil.getNumberType(gbNumber));
        }

        [TestMethod]
        public void testIsUnknown()
        {
            // Invalid numbers should be of type UNKNOWN.
            Assert.AreEqual(PhoneNumberType.UNKNOWN, phoneUtil.getNumberType(US_LOCAL_NUMBER));
        }

        [TestMethod]
        public void testIsValidNumber()
        {
            Assert.IsTrue(phoneUtil.isValidNumber(US_NUMBER));
            Assert.IsTrue(phoneUtil.isValidNumber(IT_NUMBER));
            Assert.IsTrue(phoneUtil.isValidNumber(GB_MOBILE));
            Assert.IsTrue(phoneUtil.isValidNumber(INTERNATIONAL_TOLL_FREE));
            Assert.IsTrue(phoneUtil.isValidNumber(UNIVERSAL_PREMIUM_RATE));

            PhoneNumber nzNumber = new PhoneNumber().SetCountryCode(64).SetNationalNumber(21387835L);
            Assert.IsTrue(phoneUtil.isValidNumber(nzNumber));
        }

        [TestMethod]
        public void testIsValidForRegion()
        {
            // This number is valid for the Bahamas, but is not a valid US number.
            Assert.IsTrue(phoneUtil.isValidNumber(BS_NUMBER));
            Assert.IsTrue(phoneUtil.isValidNumberForRegion(BS_NUMBER, RegionCode.BS));
            Assert.IsFalse(phoneUtil.isValidNumberForRegion(BS_NUMBER, RegionCode.US));
            PhoneNumber bsInvalidNumber =
                new PhoneNumber().SetCountryCode(1).SetNationalNumber(2421232345L);
            // This number is no longer valid.
            Assert.IsFalse(phoneUtil.isValidNumber(bsInvalidNumber));

            // La Mayotte and Reunion use 'leadingDigits' to differentiate them.
            PhoneNumber reNumber = new PhoneNumber();
            reNumber.SetCountryCode(262).SetNationalNumber(262123456L);
            Assert.IsTrue(phoneUtil.isValidNumber(reNumber));
            Assert.IsTrue(phoneUtil.isValidNumberForRegion(reNumber, RegionCode.RE));
            Assert.IsFalse(phoneUtil.isValidNumberForRegion(reNumber, RegionCode.YT));
            // Now change the number to be a number for La Mayotte.
            reNumber.SetNationalNumber(269601234L);
            Assert.IsTrue(phoneUtil.isValidNumberForRegion(reNumber, RegionCode.YT));
            Assert.IsFalse(phoneUtil.isValidNumberForRegion(reNumber, RegionCode.RE));
            // This number is no longer valid for La Reunion.
            reNumber.SetNationalNumber(269123456L);
            Assert.IsFalse(phoneUtil.isValidNumberForRegion(reNumber, RegionCode.YT));
            Assert.IsFalse(phoneUtil.isValidNumberForRegion(reNumber, RegionCode.RE));
            Assert.IsFalse(phoneUtil.isValidNumber(reNumber));
            // However, it should be recognised as from La Mayotte, since it is valid for this region.
            Assert.AreEqual(RegionCode.YT, phoneUtil.getRegionCodeForNumber(reNumber));
            // This number is valid in both places.
            reNumber.SetNationalNumber(800123456L);
            Assert.IsTrue(phoneUtil.isValidNumberForRegion(reNumber, RegionCode.YT));
            Assert.IsTrue(phoneUtil.isValidNumberForRegion(reNumber, RegionCode.RE));
            Assert.IsTrue(phoneUtil.isValidNumberForRegion(INTERNATIONAL_TOLL_FREE, RegionCode.UN001));
            Assert.IsFalse(phoneUtil.isValidNumberForRegion(INTERNATIONAL_TOLL_FREE, RegionCode.US));
            Assert.IsFalse(phoneUtil.isValidNumberForRegion(INTERNATIONAL_TOLL_FREE, RegionCode.ZZ));

            PhoneNumber invalidNumber = new PhoneNumber();
            // Invalid country calling codes.
            invalidNumber.SetCountryCode(3923).SetNationalNumber(2366L);
            Assert.IsFalse(phoneUtil.isValidNumberForRegion(invalidNumber, RegionCode.ZZ));
            Assert.IsFalse(phoneUtil.isValidNumberForRegion(invalidNumber, RegionCode.UN001));
            invalidNumber.SetCountryCode(0);
            Assert.IsFalse(phoneUtil.isValidNumberForRegion(invalidNumber, RegionCode.UN001));
            Assert.IsFalse(phoneUtil.isValidNumberForRegion(invalidNumber, RegionCode.ZZ));
        }

        [TestMethod]
        public void testIsNotValidNumber()
        {
            Assert.IsFalse(phoneUtil.isValidNumber(US_LOCAL_NUMBER));

            PhoneNumber invalidNumber = new PhoneNumber();
            invalidNumber.SetCountryCode(39).SetNationalNumber(23661830000L).setItalianLeadingZero(true);
            Assert.IsFalse(phoneUtil.isValidNumber(invalidNumber));

            invalidNumber.Clear();
            invalidNumber.SetCountryCode(44).SetNationalNumber(791234567L);
            Assert.IsFalse(phoneUtil.isValidNumber(invalidNumber));

            invalidNumber.Clear();
            invalidNumber.SetCountryCode(49).SetNationalNumber(1234L);
            Assert.IsFalse(phoneUtil.isValidNumber(invalidNumber));

            invalidNumber.Clear();
            invalidNumber.SetCountryCode(64).SetNationalNumber(3316005L);
            Assert.IsFalse(phoneUtil.isValidNumber(invalidNumber));

            invalidNumber.Clear();
            // Invalid country calling codes.
            invalidNumber.SetCountryCode(3923).SetNationalNumber(2366L);
            Assert.IsFalse(phoneUtil.isValidNumber(invalidNumber));
            invalidNumber.SetCountryCode(0);
            Assert.IsFalse(phoneUtil.isValidNumber(invalidNumber));

            Assert.IsFalse(phoneUtil.isValidNumber(INTERNATIONAL_TOLL_FREE_TOO_LONG));
        }

        [TestMethod]
        public void testGetRegionCodeForCountryCode()
        {
            Assert.AreEqual(RegionCode.US, phoneUtil.getRegionCodeForCountryCode(1));
            Assert.AreEqual(RegionCode.GB, phoneUtil.getRegionCodeForCountryCode(44));
            Assert.AreEqual(RegionCode.DE, phoneUtil.getRegionCodeForCountryCode(49));
            Assert.AreEqual(RegionCode.UN001, phoneUtil.getRegionCodeForCountryCode(800));
            Assert.AreEqual(RegionCode.UN001, phoneUtil.getRegionCodeForCountryCode(979));
        }

        [TestMethod]
        public void testGetRegionCodeForNumber()
        {
            Assert.AreEqual(RegionCode.BS, phoneUtil.getRegionCodeForNumber(BS_NUMBER));
            Assert.AreEqual(RegionCode.US, phoneUtil.getRegionCodeForNumber(US_NUMBER));
            Assert.AreEqual(RegionCode.GB, phoneUtil.getRegionCodeForNumber(GB_MOBILE));
            Assert.AreEqual(RegionCode.UN001, phoneUtil.getRegionCodeForNumber(INTERNATIONAL_TOLL_FREE));
            Assert.AreEqual(RegionCode.UN001, phoneUtil.getRegionCodeForNumber(UNIVERSAL_PREMIUM_RATE));
        }

        [TestMethod]
        public void testGetCountryCodeForRegion()
        {
            Assert.AreEqual(1, phoneUtil.getCountryCodeForRegion(RegionCode.US));
            Assert.AreEqual(64, phoneUtil.getCountryCodeForRegion(RegionCode.NZ));
            Assert.AreEqual(0, phoneUtil.getCountryCodeForRegion(null));
            Assert.AreEqual(0, phoneUtil.getCountryCodeForRegion(RegionCode.ZZ));
            Assert.AreEqual(0, phoneUtil.getCountryCodeForRegion(RegionCode.UN001));
            // CS is already deprecated so the library doesn't support it.
            Assert.AreEqual(0, phoneUtil.getCountryCodeForRegion(RegionCode.CS));
        }

        [TestMethod]
        public void testGetNationalDiallingPrefixForRegion()
        {
            Assert.AreEqual("1", phoneUtil.getNddPrefixForRegion(RegionCode.US, false));
            // Test non-main country to see it gets the national dialling prefix for the main country with
            // that country calling code.
            Assert.AreEqual("1", phoneUtil.getNddPrefixForRegion(RegionCode.BS, false));
            Assert.AreEqual("0", phoneUtil.getNddPrefixForRegion(RegionCode.NZ, false));
            // Test case with non digit in the national prefix.
            Assert.AreEqual("0~0", phoneUtil.getNddPrefixForRegion(RegionCode.AO, false));
            Assert.AreEqual("00", phoneUtil.getNddPrefixForRegion(RegionCode.AO, true));
            // Test cases with invalid regions.
            Assert.AreEqual(null, phoneUtil.getNddPrefixForRegion(null, false));
            Assert.AreEqual(null, phoneUtil.getNddPrefixForRegion(RegionCode.ZZ, false));
            Assert.AreEqual(null, phoneUtil.getNddPrefixForRegion(RegionCode.UN001, false));
            // CS is already deprecated so the library doesn't support it.
            Assert.AreEqual(null, phoneUtil.getNddPrefixForRegion(RegionCode.CS, false));
        }

        [TestMethod]
        public void testIsNANPACountry()
        {
            Assert.IsTrue(phoneUtil.isNANPACountry(RegionCode.US));
            Assert.IsTrue(phoneUtil.isNANPACountry(RegionCode.BS));
            Assert.IsFalse(phoneUtil.isNANPACountry(RegionCode.DE));
            Assert.IsFalse(phoneUtil.isNANPACountry(RegionCode.ZZ));
            Assert.IsFalse(phoneUtil.isNANPACountry(RegionCode.UN001));
            Assert.IsFalse(phoneUtil.isNANPACountry(null));
        }

        [TestMethod]
        public void testIsPossibleNumber()
        {
            Assert.IsTrue(phoneUtil.isPossibleNumber(US_NUMBER));
            Assert.IsTrue(phoneUtil.isPossibleNumber(US_LOCAL_NUMBER));
            Assert.IsTrue(phoneUtil.isPossibleNumber(GB_NUMBER));
            Assert.IsTrue(phoneUtil.isPossibleNumber(INTERNATIONAL_TOLL_FREE));

            Assert.IsTrue(phoneUtil.isPossibleNumber("+1 650 253 0000", RegionCode.US));
            Assert.IsTrue(phoneUtil.isPossibleNumber("+1 650 GOO OGLE", RegionCode.US));
            Assert.IsTrue(phoneUtil.isPossibleNumber("(650) 253-0000", RegionCode.US));
            Assert.IsTrue(phoneUtil.isPossibleNumber("253-0000", RegionCode.US));
            Assert.IsTrue(phoneUtil.isPossibleNumber("+1 650 253 0000", RegionCode.GB));
            Assert.IsTrue(phoneUtil.isPossibleNumber("+44 20 7031 3000", RegionCode.GB));
            Assert.IsTrue(phoneUtil.isPossibleNumber("(020) 7031 3000", RegionCode.GB));
            Assert.IsTrue(phoneUtil.isPossibleNumber("7031 3000", RegionCode.GB));
            Assert.IsTrue(phoneUtil.isPossibleNumber("3331 6005", RegionCode.NZ));
            Assert.IsTrue(phoneUtil.isPossibleNumber("+800 1234 5678", RegionCode.UN001));
        }

        [TestMethod]
        public void testIsPossibleNumberWithReason()
        {
            // National numbers for country calling code +1 that are within 7 to 10 digits are possible.
            Assert.AreEqual(PhoneNumberValidationResult.IS_POSSIBLE,
                         phoneUtil.isPossibleNumberWithReason(US_NUMBER));

            Assert.AreEqual(PhoneNumberValidationResult.IS_POSSIBLE,
                         phoneUtil.isPossibleNumberWithReason(US_LOCAL_NUMBER));

            Assert.AreEqual(PhoneNumberValidationResult.TOO_LONG,
                         phoneUtil.isPossibleNumberWithReason(US_LONG_NUMBER));

            PhoneNumber number = new PhoneNumber();
            number.SetCountryCode(0).SetNationalNumber(2530000L);
            Assert.AreEqual(PhoneNumberValidationResult.INVALID_COUNTRY_CODE,
                         phoneUtil.isPossibleNumberWithReason(number));

            number.Clear();
            number.SetCountryCode(1).SetNationalNumber(253000L);
            Assert.AreEqual(PhoneNumberValidationResult.TOO_SHORT,
                         phoneUtil.isPossibleNumberWithReason(number));

            number.Clear();
            number.SetCountryCode(65).SetNationalNumber(1234567890L);
            Assert.AreEqual(PhoneNumberValidationResult.IS_POSSIBLE,
                         phoneUtil.isPossibleNumberWithReason(number));

            Assert.AreEqual(PhoneNumberValidationResult.TOO_LONG,
                         phoneUtil.isPossibleNumberWithReason(INTERNATIONAL_TOLL_FREE_TOO_LONG));

            // Try with number that we don't have metadata for.
            PhoneNumber adNumber = new PhoneNumber();
            adNumber.SetCountryCode(376).SetNationalNumber(12345L);
            Assert.AreEqual(PhoneNumberValidationResult.IS_POSSIBLE,
                         phoneUtil.isPossibleNumberWithReason(adNumber));
            adNumber.SetCountryCode(376).SetNationalNumber(1L);
            Assert.AreEqual(PhoneNumberValidationResult.TOO_SHORT,
                         phoneUtil.isPossibleNumberWithReason(adNumber));
            adNumber.SetCountryCode(376).SetNationalNumber(12345678901234567L);
            Assert.AreEqual(PhoneNumberValidationResult.TOO_LONG,
                         phoneUtil.isPossibleNumberWithReason(adNumber));
        }

        [TestMethod]
        public void testIsNotPossibleNumber()
        {
            Assert.IsFalse(phoneUtil.isPossibleNumber(US_LONG_NUMBER));
            Assert.IsFalse(phoneUtil.isPossibleNumber(INTERNATIONAL_TOLL_FREE_TOO_LONG));

            PhoneNumber number = new PhoneNumber();
            number.SetCountryCode(1).SetNationalNumber(253000L);
            Assert.IsFalse(phoneUtil.isPossibleNumber(number));

            number.Clear();
            number.SetCountryCode(44).SetNationalNumber(300L);
            Assert.IsFalse(phoneUtil.isPossibleNumber(number));
            Assert.IsFalse(phoneUtil.isPossibleNumber("+1 650 253 00000", RegionCode.US));
            Assert.IsFalse(phoneUtil.isPossibleNumber("(650) 253-00000", RegionCode.US));
            Assert.IsFalse(phoneUtil.isPossibleNumber("I want a Pizza", RegionCode.US));
            Assert.IsFalse(phoneUtil.isPossibleNumber("253-000", RegionCode.US));
            Assert.IsFalse(phoneUtil.isPossibleNumber("1 3000", RegionCode.GB));
            Assert.IsFalse(phoneUtil.isPossibleNumber("+44 300", RegionCode.GB));
            Assert.IsFalse(phoneUtil.isPossibleNumber("+800 1234 5678 9", RegionCode.UN001));
        }

        [TestMethod]
        public void testTruncateTooLongNumber()
        {
            // GB number 080 1234 5678, but entered with 4 extra digits at the end.
            PhoneNumber tooLongNumber = new PhoneNumber();
            tooLongNumber.SetCountryCode(44).SetNationalNumber(80123456780123L);
            PhoneNumber validNumber = new PhoneNumber();
            validNumber.SetCountryCode(44).SetNationalNumber(8012345678L);
            Assert.IsTrue(phoneUtil.truncateTooLongNumber(tooLongNumber));
            Assert.AreEqual(validNumber, tooLongNumber);

            // IT number 022 3456 7890, but entered with 3 extra digits at the end.
            tooLongNumber.Clear();
            tooLongNumber.SetCountryCode(39).SetNationalNumber(2234567890123L).setItalianLeadingZero(true);
            validNumber.Clear();
            validNumber.SetCountryCode(39).SetNationalNumber(2234567890L).setItalianLeadingZero(true);
            Assert.IsTrue(phoneUtil.truncateTooLongNumber(tooLongNumber));
            Assert.AreEqual(validNumber, tooLongNumber);

            // US number 650-253-0000, but entered with one additional digit at the end.
            tooLongNumber.Clear();
            tooLongNumber.MergeFrom(US_LONG_NUMBER);
            Assert.IsTrue(phoneUtil.truncateTooLongNumber(tooLongNumber));
            Assert.AreEqual(US_NUMBER, tooLongNumber);

            tooLongNumber.Clear();
            tooLongNumber.MergeFrom(INTERNATIONAL_TOLL_FREE_TOO_LONG);
            Assert.IsTrue(phoneUtil.truncateTooLongNumber(tooLongNumber));
            Assert.AreEqual(INTERNATIONAL_TOLL_FREE, tooLongNumber);

            // Tests what happens when a valid number is passed in.
            PhoneNumber validNumberCopy = new PhoneNumber().MergeFrom(validNumber);
            Assert.IsTrue(phoneUtil.truncateTooLongNumber(validNumber));
            // Tests the number is not modified.
            Assert.AreEqual(validNumberCopy, validNumber);

            // Tests what happens when a number with invalid prefix is passed in.
            PhoneNumber numberWithInvalidPrefix = new PhoneNumber();
            // The test metadata says US numbers cannot have prefix 240.
            numberWithInvalidPrefix.SetCountryCode(1).SetNationalNumber(2401234567L);
            PhoneNumber invalidNumberCopy = new PhoneNumber().MergeFrom(numberWithInvalidPrefix);
            Assert.IsFalse(phoneUtil.truncateTooLongNumber(numberWithInvalidPrefix));
            // Tests the number is not modified.
            Assert.AreEqual(invalidNumberCopy, numberWithInvalidPrefix);

            // Tests what happens when a too short number is passed in.
            PhoneNumber tooShortNumber = new PhoneNumber().SetCountryCode(1).SetNationalNumber(1234L);
            PhoneNumber tooShortNumberCopy = new PhoneNumber().MergeFrom(tooShortNumber);
            Assert.IsFalse(phoneUtil.truncateTooLongNumber(tooShortNumber));
            // Tests the number is not modified.
            Assert.AreEqual(tooShortNumberCopy, tooShortNumber);
        }

        [TestMethod]
        public void testIsViablePhoneNumber()
        {
            Assert.IsFalse(PhoneNumberUtil.isViablePhoneNumber("1"));
            // Only one or two digits before strange non-possible punctuation.
            Assert.IsFalse(PhoneNumberUtil.isViablePhoneNumber("1+1+1"));
            Assert.IsFalse(PhoneNumberUtil.isViablePhoneNumber("80+0"));
            // Two digits is viable.
            Assert.IsTrue(PhoneNumberUtil.isViablePhoneNumber("00"));
            Assert.IsTrue(PhoneNumberUtil.isViablePhoneNumber("111"));
            // Alpha numbers.
            Assert.IsTrue(PhoneNumberUtil.isViablePhoneNumber("0800-4-pizza"));
            Assert.IsTrue(PhoneNumberUtil.isViablePhoneNumber("0800-4-PIZZA"));
            // We need at least three digits before any alpha characters.
            Assert.IsFalse(PhoneNumberUtil.isViablePhoneNumber("08-PIZZA"));
            Assert.IsFalse(PhoneNumberUtil.isViablePhoneNumber("8-PIZZA"));
            Assert.IsFalse(PhoneNumberUtil.isViablePhoneNumber("12. March"));
        }

        [TestMethod]
        public void testIsViablePhoneNumberNonAscii()
        {
            // Only one or two digits before possible punctuation followed by more digits.
            Assert.IsTrue(PhoneNumberUtil.isViablePhoneNumber("1\u300034"));
            Assert.IsFalse(PhoneNumberUtil.isViablePhoneNumber("1\u30003+4"));
            // Unicode variants of possible starting character and other allowed punctuation/digits.
            Assert.IsTrue(PhoneNumberUtil.isViablePhoneNumber("\uFF081\uFF09\u30003456789"));
            // Testing a leading + is okay.
            Assert.IsTrue(PhoneNumberUtil.isViablePhoneNumber("+1\uFF09\u30003456789"));
        }

        [TestMethod]
        public void testExtractPossibleNumber()
        {
            // Removes preceding funky punctuation and letters but leaves the rest untouched.
            Assert.AreEqual("0800-345-600", PhoneNumberUtil.extractPossibleNumber("Tel:0800-345-600"));
            Assert.AreEqual("0800 FOR PIZZA", PhoneNumberUtil.extractPossibleNumber("Tel:0800 FOR PIZZA"));
            // Should not remove plus sign
            Assert.AreEqual("+800-345-600", PhoneNumberUtil.extractPossibleNumber("Tel:+800-345-600"));
            // Should recognise wide digits as possible start values.
            Assert.AreEqual("\uFF10\uFF12\uFF13", PhoneNumberUtil.extractPossibleNumber("\uFF10\uFF12\uFF13"));
            // Dashes are not possible start values and should be removed.
            Assert.AreEqual("\uFF11\uFF12\uFF13", PhoneNumberUtil.extractPossibleNumber("Num-\uFF11\uFF12\uFF13"));
            // If not possible number present, return empty string.
            Assert.AreEqual("", PhoneNumberUtil.extractPossibleNumber("Num-...."));
            // Leading brackets are stripped - these are not used when parsing.
            Assert.AreEqual("650) 253-0000", PhoneNumberUtil.extractPossibleNumber("(650) 253-0000"));

            // Trailing non-alpha-numeric characters should be removed.
            Assert.AreEqual("650) 253-0000", PhoneNumberUtil.extractPossibleNumber("(650) 253-0000..- .."));
            Assert.AreEqual("650) 253-0000", PhoneNumberUtil.extractPossibleNumber("(650) 253-0000."));
            // This case has a trailing RTL char.
            Assert.AreEqual("650) 253-0000", PhoneNumberUtil.extractPossibleNumber("(650) 253-0000\u200F"));
        }

        [TestMethod]
        public void testMaybeStripNationalPrefix()
        {
            PhoneMetaData metadata = new PhoneMetaData();
            metadata.setNationalPrefixForParsing("34");
            metadata.setGeneralDesc(new PhoneNumberDesc().setNationalNumberPattern("\\d{4,8}"));
            StringBuilder numberToStrip = new StringBuilder("34356778");
            string strippedNumber = "356778";
            Assert.IsTrue(phoneUtil.maybeStripNationalPrefixAndCarrierCode(numberToStrip, metadata, null));
            Assert.AreEqual(strippedNumber, numberToStrip.ToString(), "Should have had national prefix stripped.");
            // Retry stripping - now the number should not start with the national prefix, so no more
            // stripping should occur.
            Assert.IsFalse(phoneUtil.maybeStripNationalPrefixAndCarrierCode(numberToStrip, metadata, null));
            Assert.AreEqual(strippedNumber, numberToStrip.ToString(), "Should have had no change - no national prefix present.");
            // Some countries have no national prefix. Repeat test with none specified.
            metadata.setNationalPrefixForParsing("");
            Assert.IsFalse(phoneUtil.maybeStripNationalPrefixAndCarrierCode(numberToStrip, metadata, null));
            Assert.AreEqual(strippedNumber, numberToStrip.ToString(), "Should not strip anything with empty national prefix.");
            // If the resultant number doesn't match the national rule, it shouldn't be stripped.
            metadata.setNationalPrefixForParsing("3");
            numberToStrip = new StringBuilder("3123");
            strippedNumber = "3123";
            Assert.IsFalse(phoneUtil.maybeStripNationalPrefixAndCarrierCode(numberToStrip, metadata, null));
            Assert.AreEqual(strippedNumber, numberToStrip.ToString(), "Should have had no change - after stripping, it wouldn't have matched the national rule.");
            // Test extracting carrier selection code.
            metadata.setNationalPrefixForParsing("0(81)?");
            numberToStrip = new StringBuilder("08122123456");
            strippedNumber = "22123456";
            StringBuilder carrierCode = new StringBuilder();
            Assert.IsTrue(phoneUtil.maybeStripNationalPrefixAndCarrierCode(
                numberToStrip, metadata, carrierCode));
            Assert.AreEqual("81", carrierCode.ToString());
            Assert.AreEqual(strippedNumber, numberToStrip.ToString(), "Should have had national prefix and carrier code stripped.");
            // If there was a transform rule, check it was applied.
            metadata.setNationalPrefixTransformRule("5${1}5");
            // Note that a capturing group is present here.
            metadata.setNationalPrefixForParsing("0(\\d{2})");
            numberToStrip = new StringBuilder("031123");
            string transformedNumber = "5315123";
            Assert.IsTrue(phoneUtil.maybeStripNationalPrefixAndCarrierCode(numberToStrip, metadata, null));
            Assert.AreEqual(transformedNumber, numberToStrip.ToString(), "Should transform the 031 to a 5315.");
        }

        [TestMethod]
        public void testMaybeStripInternationalPrefix()
        {
            string internationalPrefix = "00[39]";
            StringBuilder numberToStrip = new StringBuilder("0034567700-3898003");
            // Note the dash is removed as part of the normalization.
            StringBuilder strippedNumber = new StringBuilder("45677003898003");
            Assert.AreEqual(CountryCodeSource.FROM_NUMBER_WITH_IDD,
                         phoneUtil.maybeStripInternationalPrefixAndNormalize(numberToStrip,
                                                                             internationalPrefix));
            Assert.AreEqual(strippedNumber.ToString(), numberToStrip.ToString(), "The number supplied was not stripped of its international prefix.");
            // Now the number no longer starts with an IDD prefix, so it should now report
            // FROM_DEFAULT_COUNTRY.
            Assert.AreEqual(CountryCodeSource.FROM_DEFAULT_COUNTRY,
                         phoneUtil.maybeStripInternationalPrefixAndNormalize(numberToStrip,
                                                                             internationalPrefix));

            numberToStrip = new StringBuilder("00945677003898003");
            Assert.AreEqual(CountryCodeSource.FROM_NUMBER_WITH_IDD,
                         phoneUtil.maybeStripInternationalPrefixAndNormalize(numberToStrip,
                                                                             internationalPrefix));
            Assert.AreEqual(strippedNumber.ToString(), numberToStrip.ToString(), "The number supplied was not stripped of its international prefix.");
            // Test it works when the international prefix is broken up by spaces.
            numberToStrip = new StringBuilder("00 9 45677003898003");
            Assert.AreEqual(CountryCodeSource.FROM_NUMBER_WITH_IDD,
                         phoneUtil.maybeStripInternationalPrefixAndNormalize(numberToStrip,
                                                                             internationalPrefix));
            Assert.AreEqual(strippedNumber.ToString(), numberToStrip.ToString(), "The number supplied was not stripped of its international prefix.");
            // Now the number no longer starts with an IDD prefix, so it should now report
            // FROM_DEFAULT_COUNTRY.
            Assert.AreEqual(CountryCodeSource.FROM_DEFAULT_COUNTRY,
                         phoneUtil.maybeStripInternationalPrefixAndNormalize(numberToStrip,
                                                                             internationalPrefix));

            // Test the + symbol is also recognised and stripped.
            numberToStrip = new StringBuilder("+45677003898003");
            strippedNumber = new StringBuilder("45677003898003");
            Assert.AreEqual(CountryCodeSource.FROM_NUMBER_WITH_PLUS_SIGN,
                         phoneUtil.maybeStripInternationalPrefixAndNormalize(numberToStrip,
                                                                             internationalPrefix));
            Assert.AreEqual(strippedNumber.ToString(), numberToStrip.ToString(), "The number supplied was not stripped of the plus symbol.");

            // If the number afterwards is a zero, we should not strip this - no country calling code begins
            // with 0.
            numberToStrip = new StringBuilder("0090112-3123");
            strippedNumber = new StringBuilder("00901123123");
            Assert.AreEqual(CountryCodeSource.FROM_DEFAULT_COUNTRY,
                         phoneUtil.maybeStripInternationalPrefixAndNormalize(numberToStrip,
                                                                             internationalPrefix));
            Assert.AreEqual(strippedNumber.ToString(), numberToStrip.ToString(), "The number supplied had a 0 after the match so shouldn't be stripped.");
            // Here the 0 is separated by a space from the IDD.
            numberToStrip = new StringBuilder("009 0-112-3123");
            Assert.AreEqual(CountryCodeSource.FROM_DEFAULT_COUNTRY,
                         phoneUtil.maybeStripInternationalPrefixAndNormalize(numberToStrip,
                                                                             internationalPrefix));
        }

        [TestMethod]
        public void testMaybeExtractCountryCode()
        {
            PhoneNumber number = new PhoneNumber();
            PhoneMetaData metadata = phoneUtil.getMetadataForRegion(RegionCode.US);
            // Note that for the US, the IDD is 011.
            try
            {
                string phoneNumber = "011112-3456789";
                string strippedNumber = "123456789";
                int countryCallingCode = 1;
                StringBuilder numberToFill = new StringBuilder();
                Assert.AreEqual(countryCallingCode,
                             phoneUtil.maybeExtractCountryCode(phoneNumber, metadata, numberToFill, true, number),
                             "Did not extract country calling code " + countryCallingCode + " correctly.");
                Assert.AreEqual(CountryCodeSource.FROM_NUMBER_WITH_IDD, number.CountryCodeSource, "Did not figure out CountryCodeSource correctly");
                // Should strip and normalize national significant number.
                Assert.AreEqual(strippedNumber, numberToFill.ToString(), "Did not strip off the country calling code correctly.");
            }
            catch (PhoneNumberParseException e)
            {
                Assert.Fail("Should not have thrown an exception: " + e);
            }
            number.Clear();
            try
            {
                string phoneNumber = "+6423456789";
                int countryCallingCode = 64;
                StringBuilder numberToFill = new StringBuilder();
                Assert.AreEqual(countryCallingCode,
                             phoneUtil.maybeExtractCountryCode(phoneNumber, metadata, numberToFill, true, number),
                                                               "Did not extract country calling code " + countryCallingCode + " correctly.");
                Assert.AreEqual(CountryCodeSource.FROM_NUMBER_WITH_PLUS_SIGN, number.CountryCodeSource, "Did not figure out CountryCodeSource correctly");
            }
            catch (PhoneNumberParseException e)
            {
                Assert.Fail("Should not have thrown an exception: " + e);
            }
            number.Clear();
            try
            {
                string phoneNumber = "+80012345678";
                int countryCallingCode = 800;
                StringBuilder numberToFill = new StringBuilder();
                Assert.AreEqual(countryCallingCode,
                             phoneUtil.maybeExtractCountryCode(phoneNumber, metadata, numberToFill, true, number), "Did not extract country calling code " + countryCallingCode + " correctly.");
                Assert.AreEqual(CountryCodeSource.FROM_NUMBER_WITH_PLUS_SIGN, number.CountryCodeSource, "Did not figure out CountryCodeSource correctly");
            }
            catch (PhoneNumberParseException e)
            {
                Assert.Fail("Should not have thrown an exception: " + e.ToString());
            }
            number.Clear();
            try
            {
                string phoneNumber = "2345-6789";
                var numberToFill = new StringBuilder();
                Assert.AreEqual(0, phoneUtil.maybeExtractCountryCode(phoneNumber, metadata, numberToFill, true, number),
                    "Should not have extracted a country calling code - no international prefix present.");
                Assert.AreEqual(CountryCodeSource.FROM_DEFAULT_COUNTRY, number.CountryCodeSource, "Did not figure out CountryCodeSource correctly");
            }
            catch (PhoneNumberParseException e)
            {
                Assert.Fail("Should not have thrown an exception: " + e.ToString());
            }
            number.Clear();
            try
            {
                string phoneNumber = "0119991123456789";
                StringBuilder numberToFill = new StringBuilder();
                phoneUtil.maybeExtractCountryCode(phoneNumber, metadata, numberToFill, true, number);
                Assert.Fail("Should have thrown an exception, no valid country calling code present.");
            }
            catch (PhoneNumberParseException e)
            {
                // Expected.
                Assert.AreEqual(PhoneNumberParseException.ParseErrorType.INVALID_COUNTRY_CODE,
                             e.ErrorType, "Wrong error type stored in exception.");
            }
            number.Clear();
            try
            {
                string phoneNumber = "(1 610) 619 4466";
                int countryCallingCode = 1;
                StringBuilder numberToFill = new StringBuilder();
                Assert.AreEqual(countryCallingCode,
                             phoneUtil.maybeExtractCountryCode(phoneNumber, metadata, numberToFill, true, number), "Should have extracted the country calling code of the region passed in");
                Assert.AreEqual(CountryCodeSource.FROM_NUMBER_WITHOUT_PLUS_SIGN, number.CountryCodeSource, "Did not figure out CountryCodeSource correctly");
            }
            catch (PhoneNumberParseException e)
            {
                Assert.Fail("Should not have thrown an exception: " + e);
            }
            number.Clear();
            try
            {
                string phoneNumber = "(1 610) 619 4466";
                int countryCallingCode = 1;
                StringBuilder numberToFill = new StringBuilder();
                Assert.AreEqual(countryCallingCode,
                             phoneUtil.maybeExtractCountryCode(phoneNumber, metadata, numberToFill, false, number), "Should have extracted the country calling code of the region passed in");
                Assert.IsFalse(number.HasCountryCodeSource, "Should not contain CountryCodeSource.");
            }
            catch (PhoneNumberParseException e)
            {
                Assert.Fail("Should not have thrown an exception: " + e);
            }
            number.Clear();
            try
            {
                string phoneNumber = "(1 610) 619 446";
                var numberToFill = new StringBuilder();
                Assert.AreEqual(0, phoneUtil.maybeExtractCountryCode(phoneNumber, metadata, numberToFill, false, number), "Should not have extracted a country calling code - invalid number after extraction of uncertain country calling code.");
                Assert.IsFalse(number.HasCountryCodeSource, "Should not contain CountryCodeSource.");
            }
            catch (PhoneNumberParseException e)
            {
                Assert.Fail("Should not have thrown an exception: " + e);
            }
            number.Clear();
            try
            {
                string phoneNumber = "(1 610) 619";
                var numberToFill = new StringBuilder();
                Assert.AreEqual(0, phoneUtil.maybeExtractCountryCode(phoneNumber, metadata, numberToFill, true, number), "Should not have extracted a country calling code - too short number both before and after extraction of uncertain country calling code.");
                Assert.AreEqual(CountryCodeSource.FROM_DEFAULT_COUNTRY, number.CountryCodeSource, "Did not figure out CountryCodeSource correctly");
            }
            catch (PhoneNumberParseException e)
            {
                Assert.Fail("Should not have thrown an exception: " + e);
            }
        }

        [TestMethod]
        public void testParseNationalNumber()
        {
            // National prefix attached.
            Assert.AreEqual(NZ_NUMBER, phoneUtil.parse("033316005", RegionCode.NZ));
            Assert.AreEqual(NZ_NUMBER, phoneUtil.parse("33316005", RegionCode.NZ));
            // National prefix attached and some formatting present.
            Assert.AreEqual(NZ_NUMBER, phoneUtil.parse("03-331 6005", RegionCode.NZ));
            Assert.AreEqual(NZ_NUMBER, phoneUtil.parse("03 331 6005", RegionCode.NZ));
            // Test parsing RFC3966 format with a phone context.
            Assert.AreEqual(NZ_NUMBER, phoneUtil.parse("tel:03-331-6005;phone-context=+64", RegionCode.NZ));
            Assert.AreEqual(NZ_NUMBER, phoneUtil.parse("tel:331-6005;phone-context=+64-3", RegionCode.NZ));
            Assert.AreEqual(NZ_NUMBER, phoneUtil.parse("tel:331-6005;phone-context=+64-3", RegionCode.US));
            // Test parsing RFC3966 format with optional user-defined parameters. The parameters will appear
            // after the context if present.
            Assert.AreEqual(NZ_NUMBER, phoneUtil.parse("tel:03-331-6005;phone-context=+64;a=%A1",
                                                    RegionCode.NZ));
            // Test parsing RFC3966 with an ISDN subaddress.
            Assert.AreEqual(NZ_NUMBER, phoneUtil.parse("tel:03-331-6005;isub=12345;phone-context=+64",
                                                    RegionCode.NZ));
            Assert.AreEqual(NZ_NUMBER, phoneUtil.parse("tel:+64-3-331-6005;isub=12345", RegionCode.NZ));
            // Testing international prefixes.
            // Should strip country calling code.
            Assert.AreEqual(NZ_NUMBER, phoneUtil.parse("0064 3 331 6005", RegionCode.NZ));
            // Try again, but this time we have an international number with Region Code US. It should
            // recognise the country calling code and parse accordingly.
            Assert.AreEqual(NZ_NUMBER, phoneUtil.parse("01164 3 331 6005", RegionCode.US));
            Assert.AreEqual(NZ_NUMBER, phoneUtil.parse("+64 3 331 6005", RegionCode.US));
            // We should ignore the leading plus here, since it is not followed by a valid country code but
            // instead is followed by the IDD for the US.
            Assert.AreEqual(NZ_NUMBER, phoneUtil.parse("+01164 3 331 6005", RegionCode.US));
            Assert.AreEqual(NZ_NUMBER, phoneUtil.parse("+0064 3 331 6005", RegionCode.NZ));
            Assert.AreEqual(NZ_NUMBER, phoneUtil.parse("+ 00 64 3 331 6005", RegionCode.NZ));

            Assert.AreEqual(US_LOCAL_NUMBER,
                         phoneUtil.parse("tel:253-0000;phone-context=www.google.com", RegionCode.US));
            Assert.AreEqual(US_LOCAL_NUMBER,
                         phoneUtil.parse("tel:253-0000;isub=12345;phone-context=www.google.com", RegionCode.US));
            // This is invalid because no "+" sign is present as part of phone-context. The phone context
            // is simply ignored in this case just as if it contains a domain.
            Assert.AreEqual(US_LOCAL_NUMBER,
                         phoneUtil.parse("tel:2530000;isub=12345;phone-context=1-650", RegionCode.US));
            Assert.AreEqual(US_LOCAL_NUMBER,
                         phoneUtil.parse("tel:2530000;isub=12345;phone-context=1234.com", RegionCode.US));

            PhoneNumber nzNumber = new PhoneNumber();
            nzNumber.SetCountryCode(64).SetNationalNumber(64123456L);
            Assert.AreEqual(nzNumber, phoneUtil.parse("64(0)64123456", RegionCode.NZ));
            // Check that using a "/" is fine in a phone number.
            Assert.AreEqual(DE_NUMBER, phoneUtil.parse("301/23456", RegionCode.DE));

            PhoneNumber usNumber = new PhoneNumber();
            // Check it doesn't use the '1' as a country calling code when parsing if the phone number was
            // already possible.
            usNumber.SetCountryCode(1).SetNationalNumber(1234567890L);
            Assert.AreEqual(usNumber, phoneUtil.parse("123-456-7890", RegionCode.US));

            // Test star numbers. Although this is not strictly valid, we would like to make sure we can
            // parse the output we produce when formatting the number.
            Assert.AreEqual(JP_STAR_NUMBER, phoneUtil.parse("+81 *2345", RegionCode.JP));

            PhoneNumber shortNumber = new PhoneNumber();
            shortNumber.SetCountryCode(64).SetNationalNumber(12L);
            Assert.AreEqual(shortNumber, phoneUtil.parse("12", RegionCode.NZ));
        }

        [TestMethod]
        public void testParseNumberWithAlphaCharacters()
        {
            // Test case with alpha characters.
            PhoneNumber tollfreeNumber = new PhoneNumber();
            tollfreeNumber.SetCountryCode(64).SetNationalNumber(800332005L);
            Assert.AreEqual(tollfreeNumber, phoneUtil.parse("0800 DDA 005", RegionCode.NZ));
            PhoneNumber premiumNumber = new PhoneNumber();
            premiumNumber.SetCountryCode(64).SetNationalNumber(9003326005L);
            Assert.AreEqual(premiumNumber, phoneUtil.parse("0900 DDA 6005", RegionCode.NZ));
            // Not enough alpha characters for them to be considered intentional, so they are stripped.
            Assert.AreEqual(premiumNumber, phoneUtil.parse("0900 332 6005a", RegionCode.NZ));
            Assert.AreEqual(premiumNumber, phoneUtil.parse("0900 332 600a5", RegionCode.NZ));
            Assert.AreEqual(premiumNumber, phoneUtil.parse("0900 332 600A5", RegionCode.NZ));
            Assert.AreEqual(premiumNumber, phoneUtil.parse("0900 a332 600A5", RegionCode.NZ));
        }

        [TestMethod]
        public void testParseMaliciousInput()
        {
            // Lots of leading + signs before the possible number.
            StringBuilder maliciousNumber = new StringBuilder(6000);
            for (int i = 0; i < 6000; i++)
            {
                maliciousNumber.Append('+');
            }
            maliciousNumber.Append("12222-33-244 extensioB 343+");
            try
            {
                phoneUtil.parse(maliciousNumber.ToString(), RegionCode.US);
                Assert.Fail("This should not parse without throwing an exception " + maliciousNumber);
            }
            catch (PhoneNumberParseException e)
            {
                // Expected this exception.
                Assert.AreEqual(PhoneNumberParseException.ParseErrorType.TOO_LONG, e.ErrorType, "Wrong error type stored in exception.");
            }
            StringBuilder maliciousNumberWithAlmostExt = new StringBuilder(6000);
            for (int i = 0; i < 350; i++)
            {
                maliciousNumberWithAlmostExt.Append("200");
            }
            maliciousNumberWithAlmostExt.Append(" extensiOB 345");
            try
            {
                phoneUtil.parse(maliciousNumberWithAlmostExt.ToString(), RegionCode.US);
                Assert.Fail("This should not parse without throwing an exception " + maliciousNumberWithAlmostExt);
            }
            catch (PhoneNumberParseException e)
            {
                // Expected this exception.
                Assert.AreEqual(PhoneNumberParseException.ParseErrorType.TOO_LONG,
                             e.ErrorType,
                             "Wrong error type stored in exception.");
            }
        }

        [TestMethod]
        public void testParseWithInternationalPrefixes()
        {
            Assert.AreEqual(US_NUMBER, phoneUtil.parse("+1 (650) 253-0000", RegionCode.NZ));
            Assert.AreEqual(INTERNATIONAL_TOLL_FREE, phoneUtil.parse("011 800 1234 5678", RegionCode.US));
            Assert.AreEqual(US_NUMBER, phoneUtil.parse("1-650-253-0000", RegionCode.US));
            // Calling the US number from Singapore by using different service providers
            // 1st test: calling using SingTel IDD service (IDD is 001)
            Assert.AreEqual(US_NUMBER, phoneUtil.parse("0011-650-253-0000", RegionCode.SG));
            // 2nd test: calling using StarHub IDD service (IDD is 008)
            Assert.AreEqual(US_NUMBER, phoneUtil.parse("0081-650-253-0000", RegionCode.SG));
            // 3rd test: calling using SingTel V019 service (IDD is 019)
            Assert.AreEqual(US_NUMBER, phoneUtil.parse("0191-650-253-0000", RegionCode.SG));
            // Calling the US number from Poland
            Assert.AreEqual(US_NUMBER, phoneUtil.parse("0~01-650-253-0000", RegionCode.PL));
            // Using "++" at the start.
            Assert.AreEqual(US_NUMBER, phoneUtil.parse("++1 (650) 253-0000", RegionCode.PL));
        }

        [TestMethod]
        public void testParseNonAscii()
        {
            // Using a full-width plus sign.
            Assert.AreEqual(US_NUMBER, phoneUtil.parse("\uFF0B1 (650) 253-0000", RegionCode.SG));
            // Using a soft hyphen U+00AD.
            Assert.AreEqual(US_NUMBER, phoneUtil.parse("1 (650) 253\u00AD-0000", RegionCode.US));
            // The whole number, including punctuation, is here represented in full-width form.
            Assert.AreEqual(US_NUMBER, phoneUtil.parse("\uFF0B\uFF11\u3000\uFF08\uFF16\uFF15\uFF10\uFF09" +
                                                    "\u3000\uFF12\uFF15\uFF13\uFF0D\uFF10\uFF10\uFF10" +
                                                    "\uFF10",
                                                    RegionCode.SG));
            // Using U+30FC dash instead.
            Assert.AreEqual(US_NUMBER, phoneUtil.parse("\uFF0B\uFF11\u3000\uFF08\uFF16\uFF15\uFF10\uFF09" +
                                                    "\u3000\uFF12\uFF15\uFF13\u30FC\uFF10\uFF10\uFF10" +
                                                    "\uFF10",
                                                    RegionCode.SG));

            // Using a very strange decimal digit range (Mongolian digits).
            Assert.AreEqual(US_NUMBER, phoneUtil.parse("\u1811 \u1816\u1815\u1810 " +
                                                    "\u1812\u1815\u1813 \u1810\u1810\u1810\u1810",
                                                    RegionCode.US));
        }

        [TestMethod]
        public void testParseWithLeadingZero()
        {
            Assert.AreEqual(IT_NUMBER, phoneUtil.parse("+39 02-36618 300", RegionCode.NZ));
            Assert.AreEqual(IT_NUMBER, phoneUtil.parse("02-36618 300", RegionCode.IT));

            Assert.AreEqual(IT_MOBILE, phoneUtil.parse("345 678 901", RegionCode.IT));
        }

        [TestMethod]
        public void testParseNationalNumberArgentina()
        {
            // Test parsing mobile numbers of Argentina.
            PhoneNumber arNumber = new PhoneNumber();
            arNumber.SetCountryCode(54).SetNationalNumber(93435551212L);
            Assert.AreEqual(arNumber, phoneUtil.parse("+54 9 343 555 1212", RegionCode.AR));
            Assert.AreEqual(arNumber, phoneUtil.parse("0343 15 555 1212", RegionCode.AR));

            arNumber.Clear();
            arNumber.SetCountryCode(54).SetNationalNumber(93715654320L);
            Assert.AreEqual(arNumber, phoneUtil.parse("+54 9 3715 65 4320", RegionCode.AR));
            Assert.AreEqual(arNumber, phoneUtil.parse("03715 15 65 4320", RegionCode.AR));
            Assert.AreEqual(AR_MOBILE, phoneUtil.parse("911 876 54321", RegionCode.AR));

            // Test parsing fixed-line numbers of Argentina.
            Assert.AreEqual(AR_NUMBER, phoneUtil.parse("+54 11 8765 4321", RegionCode.AR));
            Assert.AreEqual(AR_NUMBER, phoneUtil.parse("011 8765 4321", RegionCode.AR));

            arNumber.Clear();
            arNumber.SetCountryCode(54).SetNationalNumber(3715654321L);
            Assert.AreEqual(arNumber, phoneUtil.parse("+54 3715 65 4321", RegionCode.AR));
            Assert.AreEqual(arNumber, phoneUtil.parse("03715 65 4321", RegionCode.AR));

            arNumber.Clear();
            arNumber.SetCountryCode(54).SetNationalNumber(2312340000L);
            Assert.AreEqual(arNumber, phoneUtil.parse("+54 23 1234 0000", RegionCode.AR));
            Assert.AreEqual(arNumber, phoneUtil.parse("023 1234 0000", RegionCode.AR));
        }

        [TestMethod]
        public void testParseWithXInNumber()
        {
            // Test that having an 'x' in the phone number at the start is ok and that it just gets removed.
            Assert.AreEqual(AR_NUMBER, phoneUtil.parse("01187654321", RegionCode.AR));
            Assert.AreEqual(AR_NUMBER, phoneUtil.parse("(0) 1187654321", RegionCode.AR));
            Assert.AreEqual(AR_NUMBER, phoneUtil.parse("0 1187654321", RegionCode.AR));
            Assert.AreEqual(AR_NUMBER, phoneUtil.parse("(0xx) 1187654321", RegionCode.AR));
            PhoneNumber arFromUs = new PhoneNumber();
            arFromUs.SetCountryCode(54).SetNationalNumber(81429712L);
            // This test is intentionally constructed such that the number of digit after xx is larger than
            // 7, so that the number won't be mistakenly treated as an extension, as we allow extensions up
            // to 7 digits. This assumption is okay for now as all the countries where a carrier selection
            // code is written in the form of xx have a national significant number of length larger than 7.
            Assert.AreEqual(arFromUs, phoneUtil.parse("011xx5481429712", RegionCode.US));
        }

        [TestMethod]
        public void testParseNumbersMexico()
        {
            // Test parsing fixed-line numbers of Mexico.
            PhoneNumber mxNumber = new PhoneNumber();
            mxNumber.SetCountryCode(52).SetNationalNumber(4499780001L);
            Assert.AreEqual(mxNumber, phoneUtil.parse("+52 (449)978-0001", RegionCode.MX));
            Assert.AreEqual(mxNumber, phoneUtil.parse("01 (449)978-0001", RegionCode.MX));
            Assert.AreEqual(mxNumber, phoneUtil.parse("(449)978-0001", RegionCode.MX));

            // Test parsing mobile numbers of Mexico.
            mxNumber.Clear();
            mxNumber.SetCountryCode(52).SetNationalNumber(13312345678L);
            Assert.AreEqual(mxNumber, phoneUtil.parse("+52 1 33 1234-5678", RegionCode.MX));
            Assert.AreEqual(mxNumber, phoneUtil.parse("044 (33) 1234-5678", RegionCode.MX));
            Assert.AreEqual(mxNumber, phoneUtil.parse("045 33 1234-5678", RegionCode.MX));
        }

        [TestMethod]
        public void testFailedParseOnInvalidNumbers()
        {
            try
            {
                string sentencePhoneNumber = "This is not a phone number";
                phoneUtil.parse(sentencePhoneNumber, RegionCode.NZ);
                Assert.Fail("This should not parse without throwing an exception " + sentencePhoneNumber);
            }
            catch (PhoneNumberParseException e)
            {
                // Expected this exception.
                Assert.AreEqual(PhoneNumberParseException.ParseErrorType.NOT_A_NUMBER,
                             e.ErrorType,
                             "Wrong error type stored in exception.");
            }
            try
            {
                string sentencePhoneNumber = "1 Still not a number";
                phoneUtil.parse(sentencePhoneNumber, RegionCode.NZ);
                Assert.Fail("This should not parse without throwing an exception " + sentencePhoneNumber);
            }
            catch (PhoneNumberParseException e)
            {
                // Expected this exception.
                Assert.AreEqual(PhoneNumberParseException.ParseErrorType.NOT_A_NUMBER,
                             e.ErrorType,
                             "Wrong error type stored in exception.");
            }
            try
            {
                string sentencePhoneNumber = "1 MICROSOFT";
                phoneUtil.parse(sentencePhoneNumber, RegionCode.NZ);
                Assert.Fail("This should not parse without throwing an exception " + sentencePhoneNumber);
            }
            catch (PhoneNumberParseException e)
            {
                // Expected this exception.
                Assert.AreEqual(PhoneNumberParseException.ParseErrorType.NOT_A_NUMBER,
                             e.ErrorType,
                             "Wrong error type stored in exception.");
            }
            try
            {
                string sentencePhoneNumber = "12 MICROSOFT";
                phoneUtil.parse(sentencePhoneNumber, RegionCode.NZ);
                Assert.Fail("This should not parse without throwing an exception " + sentencePhoneNumber);
            }
            catch (PhoneNumberParseException e)
            {
                // Expected this exception.
                Assert.AreEqual(PhoneNumberParseException.ParseErrorType.NOT_A_NUMBER,
                             e.ErrorType,
                             "Wrong error type stored in exception.");
            }
            try
            {
                string tooLongPhoneNumber = "01495 72553301873 810104";
                phoneUtil.parse(tooLongPhoneNumber, RegionCode.GB);
                Assert.Fail("This should not parse without throwing an exception " + tooLongPhoneNumber);
            }
            catch (PhoneNumberParseException e)
            {
                // Expected this exception.
                Assert.AreEqual(PhoneNumberParseException.ParseErrorType.TOO_LONG,
                             e.ErrorType,
                             "Wrong error type stored in exception.");
            }
            try
            {
                string plusMinusPhoneNumber = "+---";
                phoneUtil.parse(plusMinusPhoneNumber, RegionCode.DE);
                Assert.Fail("This should not parse without throwing an exception " + plusMinusPhoneNumber);
            }
            catch (PhoneNumberParseException e)
            {
                // Expected this exception.
                Assert.AreEqual(PhoneNumberParseException.ParseErrorType.NOT_A_NUMBER,
                             e.ErrorType,
                             "Wrong error type stored in exception.");
            }
            try
            {
                string plusStar = "+***";
                phoneUtil.parse(plusStar, RegionCode.DE);
                Assert.Fail("This should not parse without throwing an exception " + plusStar);
            }
            catch (PhoneNumberParseException e)
            {
                // Expected this exception.
                Assert.AreEqual(PhoneNumberParseException.ParseErrorType.NOT_A_NUMBER,
                             e.ErrorType,
                             "Wrong error type stored in exception.");
            }
            try
            {
                string plusStarPhoneNumber = "+*******91";
                phoneUtil.parse(plusStarPhoneNumber, RegionCode.DE);
                Assert.Fail("This should not parse without throwing an exception " + plusStarPhoneNumber);
            }
            catch (PhoneNumberParseException e)
            {
                // Expected this exception.
                Assert.AreEqual(PhoneNumberParseException.ParseErrorType.NOT_A_NUMBER,
                             e.ErrorType,
                             "Wrong error type stored in exception.");
            }
            try
            {
                string tooShortPhoneNumber = "+49 0";
                phoneUtil.parse(tooShortPhoneNumber, RegionCode.DE);
                Assert.Fail("This should not parse without throwing an exception " + tooShortPhoneNumber);
            }
            catch (PhoneNumberParseException e)
            {
                // Expected this exception.
                Assert.AreEqual(PhoneNumberParseException.ParseErrorType.TOO_SHORT_NSN,
                             e.ErrorType,
                             "Wrong error type stored in exception.");
            }
            try
            {
                string invalidCountryCode = "+210 3456 56789";
                phoneUtil.parse(invalidCountryCode, RegionCode.NZ);
                Assert.Fail("This is not a recognised region code: should fail: " + invalidCountryCode);
            }
            catch (PhoneNumberParseException e)
            {
                // Expected this exception.
                Assert.AreEqual(PhoneNumberParseException.ParseErrorType.INVALID_COUNTRY_CODE,
                             e.ErrorType,
                             "Wrong error type stored in exception.");
            }
            try
            {
                string plusAndIddAndInvalidCountryCode = "+ 00 210 3 331 6005";
                phoneUtil.parse(plusAndIddAndInvalidCountryCode, RegionCode.NZ);
                Assert.Fail("This should not parse without throwing an exception.");
            }
            catch (PhoneNumberParseException e)
            {
                // Expected this exception. 00 is a correct IDD, but 210 is not a valid country code.
                Assert.AreEqual(PhoneNumberParseException.ParseErrorType.INVALID_COUNTRY_CODE,
                             e.ErrorType,
                             "Wrong error type stored in exception.");
            }
            try
            {
                string someNumber = "123 456 7890";
                phoneUtil.parse(someNumber, RegionCode.ZZ);
                Assert.Fail("'Unknown' region code not allowed: should fail.");
            }
            catch (PhoneNumberParseException e)
            {
                // Expected this exception.
                Assert.AreEqual(PhoneNumberParseException.ParseErrorType.INVALID_COUNTRY_CODE,
                             e.ErrorType,
                             "Wrong error type stored in exception.");
            }
            try
            {
                string someNumber = "123 456 7890";
                phoneUtil.parse(someNumber, RegionCode.CS);
                Assert.Fail("Deprecated region code not allowed: should fail.");
            }
            catch (PhoneNumberParseException e)
            {
                // Expected this exception.
                Assert.AreEqual(PhoneNumberParseException.ParseErrorType.INVALID_COUNTRY_CODE,
                             e.ErrorType,
                             "Wrong error type stored in exception.");
            }
            try
            {
                string someNumber = "123 456 7890";
                phoneUtil.parse(someNumber, null);
                Assert.Fail("Null region code not allowed: should fail.");
            }
            catch (PhoneNumberParseException e)
            {
                // Expected this exception.
                Assert.AreEqual(PhoneNumberParseException.ParseErrorType.INVALID_COUNTRY_CODE,
                             e.ErrorType,
                             "Wrong error type stored in exception.");
            }
            try
            {
                string someNumber = "0044------";
                phoneUtil.parse(someNumber, RegionCode.GB);
                Assert.Fail("No number provided, only region code: should fail");
            }
            catch (PhoneNumberParseException e)
            {
                // Expected this exception.
                Assert.AreEqual(PhoneNumberParseException.ParseErrorType.TOO_SHORT_AFTER_IDD,
                             e.ErrorType,
                             "Wrong error type stored in exception.");
            }
            try
            {
                string someNumber = "0044";
                phoneUtil.parse(someNumber, RegionCode.GB);
                Assert.Fail("No number provided, only region code: should fail");
            }
            catch (PhoneNumberParseException e)
            {
                // Expected this exception.
                Assert.AreEqual(PhoneNumberParseException.ParseErrorType.TOO_SHORT_AFTER_IDD,
                             e.ErrorType,
                             "Wrong error type stored in exception.");
            }
            try
            {
                string someNumber = "011";
                phoneUtil.parse(someNumber, RegionCode.US);
                Assert.Fail("Only IDD provided - should fail.");
            }
            catch (PhoneNumberParseException e)
            {
                // Expected this exception.
                Assert.AreEqual(PhoneNumberParseException.ParseErrorType.TOO_SHORT_AFTER_IDD,
                             e.ErrorType,
                             "Wrong error type stored in exception.");
            }
            try
            {
                string someNumber = "0119";
                phoneUtil.parse(someNumber, RegionCode.US);
                Assert.Fail("Only IDD provided and then 9 - should fail.");
            }
            catch (PhoneNumberParseException e)
            {
                // Expected this exception.
                Assert.AreEqual(PhoneNumberParseException.ParseErrorType.TOO_SHORT_AFTER_IDD,
                             e.ErrorType,
                             "Wrong error type stored in exception.");
            }
            try
            {
                string emptyNumber = "";
                // Invalid region.
                phoneUtil.parse(emptyNumber, RegionCode.ZZ);
                Assert.Fail("Empty string - should fail.");
            }
            catch (PhoneNumberParseException e)
            {
                // Expected this exception.
                Assert.AreEqual(PhoneNumberParseException.ParseErrorType.NOT_A_NUMBER,
                             e.ErrorType,
                             "Wrong error type stored in exception.");
            }
            try
            {
                string nullNumber = null;
                // Invalid region.
                phoneUtil.parse(nullNumber, RegionCode.ZZ);
                Assert.Fail("Null string - should fail.");
            }
            catch (PhoneNumberParseException e)
            {
                // Expected this exception.
                Assert.AreEqual(PhoneNumberParseException.ParseErrorType.NOT_A_NUMBER,
                             e.ErrorType,
                             "Wrong error type stored in exception.");
            }
            catch (NullReferenceException)
            {
                Assert.Fail("Null string - but should not throw a null pointer exception.");
            }
            try
            {
                string nullNumber = null;
                phoneUtil.parse(nullNumber, RegionCode.US);
                Assert.Fail("Null string - should fail.");
            }
            catch (PhoneNumberParseException e)
            {
                // Expected this exception.
                Assert.AreEqual(PhoneNumberParseException.ParseErrorType.NOT_A_NUMBER,
                             e.ErrorType,
                             "Wrong error type stored in exception.");
            }
            catch (NullReferenceException)
            {
                Assert.Fail("Null string - but should not throw a null pointer exception.");
            }
            try
            {
                string domainRfcPhoneContext = "tel:555-1234;phone-context=www.google.com";
                phoneUtil.parse(domainRfcPhoneContext, RegionCode.ZZ);
                Assert.Fail("'Unknown' region code not allowed: should fail.");
            }
            catch (PhoneNumberParseException e)
            {
                // Expected this exception.
                Assert.AreEqual(PhoneNumberParseException.ParseErrorType.INVALID_COUNTRY_CODE,
                             e.ErrorType,
                             "Wrong error type stored in exception.");
            }
            try
            {
                // This is invalid because no "+" sign is present as part of phone-context. This should not
                // succeed in being parsed.
                string invalidRfcPhoneContext = "tel:555-1234;phone-context=1-331";
                phoneUtil.parse(invalidRfcPhoneContext, RegionCode.ZZ);
                Assert.Fail("'Unknown' region code not allowed: should fail.");
            }
            catch (PhoneNumberParseException e)
            {
                // Expected this exception.
                Assert.AreEqual(PhoneNumberParseException.ParseErrorType.INVALID_COUNTRY_CODE,
                             e.ErrorType,
                             "Wrong error type stored in exception.");
            }
        }

        [TestMethod]
        public void testParseNumbersWithPlusWithNoRegion()
        {
            // RegionCode.ZZ is allowed only if the number starts with a '+' - then the country calling code
            // can be calculated.
            Assert.AreEqual(NZ_NUMBER, phoneUtil.parse("+64 3 331 6005", RegionCode.ZZ));
            // Test with full-width plus.
            Assert.AreEqual(NZ_NUMBER, phoneUtil.parse("\uFF0B64 3 331 6005", RegionCode.ZZ));
            // Test with normal plus but leading characters that need to be stripped.
            Assert.AreEqual(NZ_NUMBER, phoneUtil.parse("Tel: +64 3 331 6005", RegionCode.ZZ));
            Assert.AreEqual(NZ_NUMBER, phoneUtil.parse("+64 3 331 6005", null));
            Assert.AreEqual(INTERNATIONAL_TOLL_FREE, phoneUtil.parse("+800 1234 5678", null));
            Assert.AreEqual(UNIVERSAL_PREMIUM_RATE, phoneUtil.parse("+979 123 456 789", null));

            // Test parsing RFC3966 format with a phone context.
            Assert.AreEqual(NZ_NUMBER, phoneUtil.parse("tel:03-331-6005;phone-context=+64", RegionCode.ZZ));
            Assert.AreEqual(NZ_NUMBER, phoneUtil.parse("  tel:03-331-6005;phone-context=+64", RegionCode.ZZ));
            Assert.AreEqual(NZ_NUMBER, phoneUtil.parse("tel:03-331-6005;isub=12345;phone-context=+64",
                                                    RegionCode.ZZ));

            // It is important that we set the carrier code to an empty string, since we used
            // ParseAndKeepRawInput and no carrier code was found.
            PhoneNumber nzNumberWithRawInput = new PhoneNumber().MergeFrom(NZ_NUMBER).
                SetRawInput("+64 3 331 6005").
                SetCountryCodeSource(CountryCodeSource.FROM_NUMBER_WITH_PLUS_SIGN).
                SetPreferredDomesticCarrierCode("");
            Assert.AreEqual(nzNumberWithRawInput, phoneUtil.parseAndKeepRawInput("+64 3 331 6005",
                                                                              RegionCode.ZZ));
            // Null is also allowed for the region code in these cases.
            Assert.AreEqual(nzNumberWithRawInput, phoneUtil.parseAndKeepRawInput("+64 3 331 6005", null));
        }

        [TestMethod]
        public void testParseExtensions()
        {
            PhoneNumber nzNumber = new PhoneNumber();
            nzNumber.SetCountryCode(64).SetNationalNumber(33316005L).SetExtension("3456");
            Assert.AreEqual(nzNumber, phoneUtil.parse("03 331 6005 ext 3456", RegionCode.NZ));
            Assert.AreEqual(nzNumber, phoneUtil.parse("03-3316005x3456", RegionCode.NZ));
            Assert.AreEqual(nzNumber, phoneUtil.parse("03-3316005 int.3456", RegionCode.NZ));
            Assert.AreEqual(nzNumber, phoneUtil.parse("03 3316005 #3456", RegionCode.NZ));
            // Test the following do not extract extensions:
            Assert.AreEqual(ALPHA_NUMERIC_NUMBER, phoneUtil.parse("1800 six-flags", RegionCode.US));
            Assert.AreEqual(ALPHA_NUMERIC_NUMBER, phoneUtil.parse("1800 SIX FLAGS", RegionCode.US));
            Assert.AreEqual(ALPHA_NUMERIC_NUMBER, phoneUtil.parse("0~0 1800 7493 5247", RegionCode.PL));
            Assert.AreEqual(ALPHA_NUMERIC_NUMBER, phoneUtil.parse("(1800) 7493.5247", RegionCode.US));
            // Check that the last instance of an extension token is matched.
            PhoneNumber extnNumber = new PhoneNumber().MergeFrom(ALPHA_NUMERIC_NUMBER).SetExtension("1234");
            Assert.AreEqual(extnNumber, phoneUtil.parse("0~0 1800 7493 5247 ~1234", RegionCode.PL));
            // Verifying bug-fix where the last digit of a number was previously omitted if it was a 0 when
            // extracting the extension. Also verifying a few different cases of extensions.
            PhoneNumber ukNumber = new PhoneNumber();
            ukNumber.SetCountryCode(44).SetNationalNumber(2034567890L).SetExtension("456");
            Assert.AreEqual(ukNumber, phoneUtil.parse("+44 2034567890x456", RegionCode.NZ));
            Assert.AreEqual(ukNumber, phoneUtil.parse("+44 2034567890x456", RegionCode.GB));
            Assert.AreEqual(ukNumber, phoneUtil.parse("+44 2034567890 x456", RegionCode.GB));
            Assert.AreEqual(ukNumber, phoneUtil.parse("+44 2034567890 X456", RegionCode.GB));
            Assert.AreEqual(ukNumber, phoneUtil.parse("+44 2034567890 X 456", RegionCode.GB));
            Assert.AreEqual(ukNumber, phoneUtil.parse("+44 2034567890 X  456", RegionCode.GB));
            Assert.AreEqual(ukNumber, phoneUtil.parse("+44 2034567890 x 456  ", RegionCode.GB));
            Assert.AreEqual(ukNumber, phoneUtil.parse("+44 2034567890  X 456", RegionCode.GB));
            Assert.AreEqual(ukNumber, phoneUtil.parse("+44-2034567890;ext=456", RegionCode.GB));
            Assert.AreEqual(ukNumber, phoneUtil.parse("tel:2034567890;ext=456;phone-context=+44",
                                                   RegionCode.ZZ));
            // Full-width extension, "extn" only.
            Assert.AreEqual(ukNumber, phoneUtil.parse("+442034567890\uFF45\uFF58\uFF54\uFF4E456",
                                                   RegionCode.GB));
            // "xtn" only.
            Assert.AreEqual(ukNumber, phoneUtil.parse("+442034567890\uFF58\uFF54\uFF4E456",
                                                   RegionCode.GB));
            // "xt" only.
            Assert.AreEqual(ukNumber, phoneUtil.parse("+442034567890\uFF58\uFF54456",
                                                   RegionCode.GB));

            PhoneNumber usWithExtension = new PhoneNumber();
            usWithExtension.SetCountryCode(1).SetNationalNumber(8009013355L).SetExtension("7246433");
            Assert.AreEqual(usWithExtension, phoneUtil.parse("(800) 901-3355 x 7246433", RegionCode.US));
            Assert.AreEqual(usWithExtension, phoneUtil.parse("(800) 901-3355 , ext 7246433", RegionCode.US));
            Assert.AreEqual(usWithExtension,
                         phoneUtil.parse("(800) 901-3355 ,extension 7246433", RegionCode.US));
            Assert.AreEqual(usWithExtension,
                         phoneUtil.parse("(800) 901-3355 ,extensi\u00F3n 7246433", RegionCode.US));
            // Repeat with the small letter o with acute accent created by combining characters.
            Assert.AreEqual(usWithExtension,
                         phoneUtil.parse("(800) 901-3355 ,extensio\u0301n 7246433", RegionCode.US));
            Assert.AreEqual(usWithExtension, phoneUtil.parse("(800) 901-3355 , 7246433", RegionCode.US));
            Assert.AreEqual(usWithExtension, phoneUtil.parse("(800) 901-3355 ext: 7246433", RegionCode.US));

            // Test that if a number has two extensions specified, we ignore the second.
            PhoneNumber usWithTwoExtensionsNumber = new PhoneNumber();
            usWithTwoExtensionsNumber.SetCountryCode(1).SetNationalNumber(2121231234L).SetExtension("508");
            Assert.AreEqual(usWithTwoExtensionsNumber, phoneUtil.parse("(212)123-1234 x508/x1234",
                                                                    RegionCode.US));
            Assert.AreEqual(usWithTwoExtensionsNumber, phoneUtil.parse("(212)123-1234 x508/ x1234",
                                                                    RegionCode.US));
            Assert.AreEqual(usWithTwoExtensionsNumber, phoneUtil.parse("(212)123-1234 x508\\x1234",
                                                                    RegionCode.US));

            // Test parsing numbers in the form (645) 123-1234-910# works, where the last 3 digits before
            // the # are an extension.
            usWithExtension.Clear();
            usWithExtension.SetCountryCode(1).SetNationalNumber(6451231234L).SetExtension("910");
            Assert.AreEqual(usWithExtension, phoneUtil.parse("+1 (645) 123 1234-910#", RegionCode.US));
            // Retry with the same number in a slightly different format.
            Assert.AreEqual(usWithExtension, phoneUtil.parse("+1 (645) 123 1234 ext. 910#", RegionCode.US));
        }

        [TestMethod]
        public void testParseAndKeepRaw()
        {
            PhoneNumber alphaNumericNumber = new PhoneNumber().MergeFrom(ALPHA_NUMERIC_NUMBER).
                SetRawInput("800 six-flags").
                SetCountryCodeSource(CountryCodeSource.FROM_DEFAULT_COUNTRY).
                SetPreferredDomesticCarrierCode("");
            Assert.AreEqual(alphaNumericNumber,
                         phoneUtil.parseAndKeepRawInput("800 six-flags", RegionCode.US));

            PhoneNumber shorterAlphaNumber = new PhoneNumber().
                SetCountryCode(1).SetNationalNumber(8007493524L).
                SetRawInput("1800 six-flag").
                SetCountryCodeSource(CountryCodeSource.FROM_NUMBER_WITHOUT_PLUS_SIGN).
                SetPreferredDomesticCarrierCode("");
            Assert.AreEqual(shorterAlphaNumber,
                         phoneUtil.parseAndKeepRawInput("1800 six-flag", RegionCode.US));

            shorterAlphaNumber.SetRawInput("+1800 six-flag").
                SetCountryCodeSource(CountryCodeSource.FROM_NUMBER_WITH_PLUS_SIGN);
            Assert.AreEqual(shorterAlphaNumber,
                         phoneUtil.parseAndKeepRawInput("+1800 six-flag", RegionCode.NZ));

            shorterAlphaNumber.SetRawInput("001800 six-flag").
                SetCountryCodeSource(CountryCodeSource.FROM_NUMBER_WITH_IDD);
            Assert.AreEqual(shorterAlphaNumber,
                         phoneUtil.parseAndKeepRawInput("001800 six-flag", RegionCode.NZ));

            // Invalid region code supplied.
            try
            {
                phoneUtil.parseAndKeepRawInput("123 456 7890", RegionCode.CS);
                Assert.Fail("Deprecated region code not allowed: should fail.");
            }
            catch (PhoneNumberParseException e)
            {
                // Expected this exception.
                Assert.AreEqual(PhoneNumberParseException.ParseErrorType.INVALID_COUNTRY_CODE,
                             e.ErrorType,
                             "Wrong error type stored in exception.");
            }

            PhoneNumber koreanNumber = new PhoneNumber();
            koreanNumber.SetCountryCode(82).SetNationalNumber(22123456).SetRawInput("08122123456").
                SetCountryCodeSource(CountryCodeSource.FROM_DEFAULT_COUNTRY).
                SetPreferredDomesticCarrierCode("81");
            Assert.AreEqual(koreanNumber, phoneUtil.parseAndKeepRawInput("08122123456", RegionCode.KR));
        }

        [TestMethod]
        public void testCountryWithNoNumberDesc()
        {
            // Andorra is a country where we don't have PhoneNumberDesc info in the metadata.
            PhoneNumber adNumber = new PhoneNumber();
            adNumber.SetCountryCode(376).SetNationalNumber(12345L);
            Assert.AreEqual("+376 12345", phoneUtil.format(adNumber, PhoneNumberFormat.INTERNATIONAL));
            Assert.AreEqual("+37612345", phoneUtil.format(adNumber, PhoneNumberFormat.E164));
            Assert.AreEqual("12345", phoneUtil.format(adNumber, PhoneNumberFormat.NATIONAL));
            Assert.AreEqual(PhoneNumberType.UNKNOWN, phoneUtil.getNumberType(adNumber));
            Assert.IsTrue(phoneUtil.isValidNumber(adNumber));

            // Test dialing a US number from within Andorra.
            Assert.AreEqual("00 1 650 253 0000",
                         phoneUtil.formatOutOfCountryCallingNumber(US_NUMBER, RegionCode.AD));
        }

        [TestMethod]
        public void testUnknownCountryCallingCodeForValidation()
        {
            PhoneNumber invalidNumber = new PhoneNumber();
            invalidNumber.SetCountryCode(0).SetNationalNumber(1234L);
            Assert.IsFalse(phoneUtil.isValidNumber(invalidNumber));
        }

        [TestMethod]
        public void testIsNumberMatchMatches()
        {
            // Test simple matches where formatting is different, or leading zeroes, or country calling code
            // has been specified.
            Assert.AreEqual(PhoneNumberMatchType.EXACT_MATCH,
                         phoneUtil.isNumberMatch("+64 3 331 6005", "+64 03 331 6005"));
            Assert.AreEqual(PhoneNumberMatchType.EXACT_MATCH,
                         phoneUtil.isNumberMatch("+800 1234 5678", "+80012345678"));
            Assert.AreEqual(PhoneNumberMatchType.EXACT_MATCH,
                         phoneUtil.isNumberMatch("+64 03 331-6005", "+64 03331 6005"));
            Assert.AreEqual(PhoneNumberMatchType.EXACT_MATCH,
                         phoneUtil.isNumberMatch("+643 331-6005", "+64033316005"));
            Assert.AreEqual(PhoneNumberMatchType.EXACT_MATCH,
                         phoneUtil.isNumberMatch("+643 331-6005", "+6433316005"));
            Assert.AreEqual(PhoneNumberMatchType.EXACT_MATCH,
                         phoneUtil.isNumberMatch("+64 3 331-6005", "+6433316005"));
            Assert.AreEqual(PhoneNumberMatchType.EXACT_MATCH,
                         phoneUtil.isNumberMatch("+64 3 331-6005", "tel:+64-3-331-6005;isub=123"));
            // Test alpha numbers.
            Assert.AreEqual(PhoneNumberMatchType.EXACT_MATCH,
                         phoneUtil.isNumberMatch("+1800 siX-Flags", "+1 800 7493 5247"));
            // Test numbers with extensions.
            Assert.AreEqual(PhoneNumberMatchType.EXACT_MATCH,
                         phoneUtil.isNumberMatch("+64 3 331-6005 extn 1234", "+6433316005#1234"));
            // Test proto buffers.
            Assert.AreEqual(PhoneNumberMatchType.EXACT_MATCH,
                         phoneUtil.isNumberMatch(NZ_NUMBER, "+6403 331 6005"));

            PhoneNumber nzNumber = new PhoneNumber().MergeFrom(NZ_NUMBER).SetExtension("3456");
            Assert.AreEqual(PhoneNumberMatchType.EXACT_MATCH,
                         phoneUtil.isNumberMatch(nzNumber, "+643 331 6005 ext 3456"));
            // Check empty extensions are ignored.
            nzNumber.SetExtension("");
            Assert.AreEqual(PhoneNumberMatchType.EXACT_MATCH,
                         phoneUtil.isNumberMatch(nzNumber, "+6403 331 6005"));
            // Check variant with two proto buffers.
            Assert.AreEqual(PhoneNumberMatchType.EXACT_MATCH,
                         phoneUtil.isNumberMatch(nzNumber, NZ_NUMBER),
                         "Number " + nzNumber + " did not match " + NZ_NUMBER);

            // Check raw_input, country_code_source and preferred_domestic_carrier_code are ignored.
            var brNumberOne = new PhoneNumber();
            var brNumberTwo = new PhoneNumber();
            brNumberOne.SetCountryCode(55).SetNationalNumber(3121286979L)
                .SetCountryCodeSource(CountryCodeSource.FROM_NUMBER_WITH_PLUS_SIGN)
                .SetPreferredDomesticCarrierCode("12").SetRawInput("012 3121286979");
            brNumberTwo.SetCountryCode(55).SetNationalNumber(3121286979L)
                .SetCountryCodeSource(CountryCodeSource.FROM_DEFAULT_COUNTRY)
                .SetPreferredDomesticCarrierCode("14").SetRawInput("143121286979");
            Assert.AreEqual(PhoneNumberMatchType.EXACT_MATCH,
                         phoneUtil.isNumberMatch(brNumberOne, brNumberTwo));
        }

        [TestMethod]
        public void testIsNumberMatchNonMatches()
        {
            // Non-matches.
            Assert.AreEqual(PhoneNumberMatchType.NO_MATCH,
                         phoneUtil.isNumberMatch("03 331 6005", "03 331 6006"));
            Assert.AreEqual(PhoneNumberMatchType.NO_MATCH,
                         phoneUtil.isNumberMatch("+800 1234 5678", "+1 800 1234 5678"));
            // Different country calling code, partial number match.
            Assert.AreEqual(PhoneNumberMatchType.NO_MATCH,
                         phoneUtil.isNumberMatch("+64 3 331-6005", "+16433316005"));
            // Different country calling code, same number.
            Assert.AreEqual(PhoneNumberMatchType.NO_MATCH,
                         phoneUtil.isNumberMatch("+64 3 331-6005", "+6133316005"));
            // Extension different, all else the same.
            Assert.AreEqual(PhoneNumberMatchType.NO_MATCH,
                         phoneUtil.isNumberMatch("+64 3 331-6005 extn 1234", "0116433316005#1235"));
            Assert.AreEqual(PhoneNumberMatchType.NO_MATCH,
                         phoneUtil.isNumberMatch(
                             "+64 3 331-6005 extn 1234", "tel:+64-3-331-6005;ext=1235"));
            // NSN matches, but extension is different - not the same number.
            Assert.AreEqual(PhoneNumberMatchType.NO_MATCH,
                         phoneUtil.isNumberMatch("+64 3 331-6005 ext.1235", "3 331 6005#1234"));

            // Invalid numbers that can't be parsed.
            Assert.AreEqual(PhoneNumberMatchType.NOT_A_NUMBER,
                         phoneUtil.isNumberMatch("4", "3 331 6043"));
            Assert.AreEqual(PhoneNumberMatchType.NOT_A_NUMBER,
                         phoneUtil.isNumberMatch("+43", "+64 3 331 6005"));
            Assert.AreEqual(PhoneNumberMatchType.NOT_A_NUMBER,
                         phoneUtil.isNumberMatch("+43", "64 3 331 6005"));
            Assert.AreEqual(PhoneNumberMatchType.NOT_A_NUMBER,
                         phoneUtil.isNumberMatch("Dog", "64 3 331 6005"));
        }

        [TestMethod]
        public void testIsNumberMatchNsnMatches()
        {
            // NSN matches.
            Assert.AreEqual(PhoneNumberMatchType.NSN_MATCH,
                         phoneUtil.isNumberMatch("+64 3 331-6005", "03 331 6005"));
            Assert.AreEqual(PhoneNumberMatchType.NSN_MATCH,
                         phoneUtil.isNumberMatch(
                             "+64 3 331-6005", "tel:03-331-6005;isub=1234;phone-context=abc.nz"));
            Assert.AreEqual(PhoneNumberMatchType.NSN_MATCH,
                         phoneUtil.isNumberMatch(NZ_NUMBER, "03 331 6005"));
            // Here the second number possibly starts with the country calling code for New Zealand,
            // although we are unsure.
            PhoneNumber unchangedNzNumber = new PhoneNumber().MergeFrom(NZ_NUMBER);
            Assert.AreEqual(PhoneNumberMatchType.NSN_MATCH,
                         phoneUtil.isNumberMatch(unchangedNzNumber, "(64-3) 331 6005"));
            // Check the phone number proto was not edited during the method call.
            Assert.AreEqual(NZ_NUMBER, unchangedNzNumber);

            // Here, the 1 might be a national prefix, if we compare it to the US number, so the resultant
            // match is an NSN match.
            Assert.AreEqual(PhoneNumberMatchType.NSN_MATCH,
                         phoneUtil.isNumberMatch(US_NUMBER, "1-650-253-0000"));
            Assert.AreEqual(PhoneNumberMatchType.NSN_MATCH,
                         phoneUtil.isNumberMatch(US_NUMBER, "6502530000"));
            Assert.AreEqual(PhoneNumberMatchType.NSN_MATCH,
                         phoneUtil.isNumberMatch("+1 650-253 0000", "1 650 253 0000"));
            Assert.AreEqual(PhoneNumberMatchType.NSN_MATCH,
                         phoneUtil.isNumberMatch("1 650-253 0000", "1 650 253 0000"));
            Assert.AreEqual(PhoneNumberMatchType.NSN_MATCH,
                         phoneUtil.isNumberMatch("1 650-253 0000", "+1 650 253 0000"));
            // For this case, the match will be a short NSN match, because we cannot assume that the 1 might
            // be a national prefix, so don't remove it when parsing.
            PhoneNumber randomNumber = new PhoneNumber();
            randomNumber.SetCountryCode(41).SetNationalNumber(6502530000L);
            Assert.AreEqual(PhoneNumberMatchType.SHORT_NSN_MATCH,
                         phoneUtil.isNumberMatch(randomNumber, "1-650-253-0000"));
        }

        [TestMethod]
        public void testIsNumberMatchShortNsnMatches()
        {
            // Short NSN matches with the country not specified for either one or both numbers.
            Assert.AreEqual(PhoneNumberMatchType.SHORT_NSN_MATCH,
                         phoneUtil.isNumberMatch("+64 3 331-6005", "331 6005"));
            Assert.AreEqual(PhoneNumberMatchType.SHORT_NSN_MATCH,
                         phoneUtil.isNumberMatch("+64 3 331-6005", "tel:331-6005;phone-context=abc.nz"));
            Assert.AreEqual(PhoneNumberMatchType.SHORT_NSN_MATCH,
                         phoneUtil.isNumberMatch("+64 3 331-6005",
                                                 "tel:331-6005;isub=1234;phone-context=abc.nz"));
            Assert.AreEqual(PhoneNumberMatchType.SHORT_NSN_MATCH,
                         phoneUtil.isNumberMatch("+64 3 331-6005",
                                                 "tel:331-6005;isub=1234;phone-context=abc.nz;a=%A1"));
            // We did not know that the "0" was a national prefix since neither number has a country code,
            // so this is considered a SHORT_NSN_MATCH.
            Assert.AreEqual(PhoneNumberMatchType.SHORT_NSN_MATCH,
                         phoneUtil.isNumberMatch("3 331-6005", "03 331 6005"));
            Assert.AreEqual(PhoneNumberMatchType.SHORT_NSN_MATCH,
                         phoneUtil.isNumberMatch("3 331-6005", "331 6005"));
            Assert.AreEqual(PhoneNumberMatchType.SHORT_NSN_MATCH,
                         phoneUtil.isNumberMatch("3 331-6005", "tel:331-6005;phone-context=abc.nz"));
            Assert.AreEqual(PhoneNumberMatchType.SHORT_NSN_MATCH,
                         phoneUtil.isNumberMatch("3 331-6005", "+64 331 6005"));
            // Short NSN match with the country specified.
            Assert.AreEqual(PhoneNumberMatchType.SHORT_NSN_MATCH,
                         phoneUtil.isNumberMatch("03 331-6005", "331 6005"));
            Assert.AreEqual(PhoneNumberMatchType.SHORT_NSN_MATCH,
                         phoneUtil.isNumberMatch("1 234 345 6789", "345 6789"));
            Assert.AreEqual(PhoneNumberMatchType.SHORT_NSN_MATCH,
                         phoneUtil.isNumberMatch("+1 (234) 345 6789", "345 6789"));
            // NSN matches, country calling code omitted for one number, extension missing for one.
            Assert.AreEqual(PhoneNumberMatchType.SHORT_NSN_MATCH,
                         phoneUtil.isNumberMatch("+64 3 331-6005", "3 331 6005#1234"));
            // One has Italian leading zero, one does not.
            var italianNumberOne = new PhoneNumber();
            italianNumberOne.SetCountryCode(39).SetNationalNumber(1234L).setItalianLeadingZero(true);

            var italianNumberTwo = new PhoneNumber();
            italianNumberTwo.SetCountryCode(39).SetNationalNumber(1234L);

            Assert.AreEqual(PhoneNumberMatchType.SHORT_NSN_MATCH,
                         phoneUtil.isNumberMatch(italianNumberOne, italianNumberTwo));
            // One has an extension, the other has an extension of "".
            italianNumberOne.SetExtension("1234").ClearItalianLeadingZero();
            italianNumberTwo.SetExtension("");
            Assert.AreEqual(PhoneNumberMatchType.SHORT_NSN_MATCH,
                         phoneUtil.isNumberMatch(italianNumberOne, italianNumberTwo));
        }

        [TestMethod]
        public void testCanBeInternationallyDialled()
        {
            // We have no-international-dialling rules for the US in our test metadata that say that
            // toll-free numbers cannot be dialled internationally.
            Assert.IsFalse(phoneUtil.canBeInternationallyDialled(US_TOLLFREE));

            // Normal US numbers can be internationally dialled.
            Assert.IsTrue(phoneUtil.canBeInternationallyDialled(US_NUMBER));

            // Invalid number.
            Assert.IsTrue(phoneUtil.canBeInternationallyDialled(US_LOCAL_NUMBER));

            // We have no data for NZ - should return true.
            Assert.IsTrue(phoneUtil.canBeInternationallyDialled(NZ_NUMBER));
            Assert.IsTrue(phoneUtil.canBeInternationallyDialled(INTERNATIONAL_TOLL_FREE));
        }

        [TestMethod]
        public void testIsAlphaNumber()
        {
            Assert.IsTrue(phoneUtil.isAlphaNumber("1800 six-flags"));
            Assert.IsTrue(phoneUtil.isAlphaNumber("1800 six-flags ext. 1234"));
            Assert.IsTrue(phoneUtil.isAlphaNumber("+800 six-flags"));
            Assert.IsTrue(phoneUtil.isAlphaNumber("180 six-flags"));
            Assert.IsFalse(phoneUtil.isAlphaNumber("1800 123-1234"));
            Assert.IsFalse(phoneUtil.isAlphaNumber("1 six-flags"));
            Assert.IsFalse(phoneUtil.isAlphaNumber("18 six-flags"));
            Assert.IsFalse(phoneUtil.isAlphaNumber("1800 123-1234 extension: 1234"));
            Assert.IsFalse(phoneUtil.isAlphaNumber("+800 1234-1234"));
        }
    }
}