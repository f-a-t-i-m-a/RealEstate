using System;
using System.IO;
using System.Text;
using System.Xml.Linq;
using JahanJooy.Common.Util.PhoneNumbers;
using JahanJooy.Common.Util.PhoneNumbers.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JahanJooy.Common.Util.Test.PhoneNumbers
{
    [TestClass]
    public class PhoneNumberMetaDataReaderTest
    {
        // Tests validateRE().
        [TestMethod]
        public void testValidateRERemovesWhiteSpaces()
        {
            string input = " hello world ";
            // Should remove all the white spaces contained in the provided string.
            Assert.AreEqual("helloworld", PhoneNumberMetaDataReader.validateRE(input, true));
            // Make sure it only happens when the last parameter is set to true.
            Assert.AreEqual(" hello world ", PhoneNumberMetaDataReader.validateRE(input, false));
        }

        [TestMethod]
        public void testValidateREThrowsException()
        {
            string invalidPattern = "[";
            // Should throw an exception when an invalid pattern is provided independently of the last
            // parameter (remove white spaces).
            try
            {
                PhoneNumberMetaDataReader.validateRE(invalidPattern, false);
                Assert.Fail();
            }
            catch (ArgumentException)
            {
                // Test passed.
            }
            try
            {
                PhoneNumberMetaDataReader.validateRE(invalidPattern, true);
                Assert.Fail();
            }
            catch (ArgumentException)
            {
                // Test passed.
            }
        }

        [TestMethod]
        public void testValidateRE()
        {
            string validPattern = "[a-zA-Z]d{1,9}";
            // The provided pattern should be left unchanged.
            Assert.AreEqual(validPattern, PhoneNumberMetaDataReader.validateRE(validPattern, false));
        }

        // Tests getNationalPrefix().
        [TestMethod]
        public void testGetNationalPrefix()
        {
            string xmlInput = "<territory nationalPrefix='00'/>";
            XElement territoryElement = parseXmlString(xmlInput);
            Assert.AreEqual("00", PhoneNumberMetaDataReader.GetNationalPrefix(territoryElement));
        }

        // Tests LoadTerritoriTagMetaData().
        [TestMethod]
        public void testLoadTerritoryTagMetadata()
        {
            string xmlInput =
                "<territory countryCode='33' leadingDigits='2' internationalPrefix='00'" +
                "           preferredInternationalPrefix='0011' nationalPrefixForParsing='0'" +
                "           nationalPrefixTransformRule='9$1'" + // nationalPrefix manually injected.
                "           preferredExtnPrefix=' x' mainCountryForCode='true'" +
                "           leadingZeroPossible='true'>" +
                "</territory>";
            XElement territoryElement = parseXmlString(xmlInput);
            PhoneMetaData phoneMetadata = PhoneNumberMetaDataReader.LoadTerritoryTagMetaData("33", territoryElement, "0");
            Assert.AreEqual(33, phoneMetadata.CountryCode);
            Assert.AreEqual("2", phoneMetadata.LeadingDigits);
            Assert.AreEqual("00", phoneMetadata.InternationalPrefix);
            Assert.AreEqual("0011", phoneMetadata.PreferredInternationalPrefix);
            Assert.AreEqual("0", phoneMetadata.NationalPrefixForParsing);
            Assert.AreEqual("9$1", phoneMetadata.NationalPrefixTransformRule);
            Assert.AreEqual("0", phoneMetadata.NationalPrefix);
            Assert.AreEqual(" x", phoneMetadata.PreferredExtnPrefix);
            Assert.IsTrue(phoneMetadata.getMainCountryForCode());
            Assert.IsTrue(phoneMetadata.LeadingZeroPossible);
        }

        [TestMethod]
        public void testLoadTerritoryTagMetadataSetsBooleanFieldsToFalseByDefault()
        {
            string xmlInput = "<territory countryCode='33'/>";
            XElement territoryElement = parseXmlString(xmlInput);
            PhoneMetaData phoneMetadata = PhoneNumberMetaDataReader.LoadTerritoryTagMetaData("33", territoryElement, "");
            Assert.IsFalse(phoneMetadata.getMainCountryForCode());
            Assert.IsFalse(phoneMetadata.LeadingZeroPossible);
        }

        [TestMethod]
        public void testLoadTerritoryTagMetadataSetsNationalPrefixForParsingByDefault()
        {
            string xmlInput = "<territory countryCode='33'/>";
            XElement territoryElement = parseXmlString(xmlInput);
            PhoneMetaData phoneMetadata = PhoneNumberMetaDataReader.LoadTerritoryTagMetaData("33", territoryElement, "00");
            // When unspecified, nationalPrefixForParsing defaults to nationalPrefix.
            Assert.AreEqual("00", phoneMetadata.NationalPrefix);
            Assert.AreEqual(phoneMetadata.NationalPrefix, phoneMetadata.NationalPrefixForParsing);
        }

        [TestMethod]
        public void testLoadTerritoryTagMetadataWithRequiredAttributesOnly()
        {
            string xmlInput = "<territory countryCode='33' internationalPrefix='00'/>";
            XElement territoryElement = parseXmlString(xmlInput);
            // Should not throw any exception.
            PhoneNumberMetaDataReader.LoadTerritoryTagMetaData("33", territoryElement, "");
        }

        // Tests loadInternationalFormat().
        [TestMethod]
        public void testLoadInternationalFormat()
        {
            string intlFormat = "$1 $2";
            string xmlInput = "<numberFormat><intlFormat>" + intlFormat + "</intlFormat></numberFormat>";
            XElement numberFormatElement = parseXmlString(xmlInput);
            PhoneMetaData metadata = new PhoneMetaData();
            string nationalFormat = "";

            Assert.IsTrue(PhoneNumberMetaDataReader.LoadInternationalFormat(metadata, numberFormatElement,
                                                                    nationalFormat));
            Assert.AreEqual(intlFormat, metadata.getIntlNumberFormat(0).Format);
        }

        [TestMethod]
        public void testLoadInternationalFormatWithBothNationalAndIntlFormatsDefined()
        {
            string intlFormat = "$1 $2";
            string xmlInput = "<numberFormat><intlFormat>" + intlFormat + "</intlFormat></numberFormat>";
            XElement numberFormatElement = parseXmlString(xmlInput);
            PhoneMetaData metadata = new PhoneMetaData();
            string nationalFormat = "$1";

            Assert.IsTrue(PhoneNumberMetaDataReader.LoadInternationalFormat(metadata, numberFormatElement,
                                                                    nationalFormat));
            Assert.AreEqual(intlFormat, metadata.getIntlNumberFormat(0).Format);
        }

        [TestMethod]
        public void testLoadInternationalFormatExpectsOnlyOnePattern()
        {
            string xmlInput = "<numberFormat><intlFormat/><intlFormat/></numberFormat>";
            XElement numberFormatElement = parseXmlString(xmlInput);
            var metadata = new PhoneMetaData();

            // Should throw an exception as multiple intlFormats are provided.
            try
            {
                PhoneNumberMetaDataReader.LoadInternationalFormat(metadata, numberFormatElement, "");
                Assert.Fail();
            }
            catch (Exception e)
            {
                // Test passed.
            }
        }

        [TestMethod]
        public void testLoadInternationalFormatUsesNationalFormatByDefault()
        {
            string xmlInput = "<numberFormat></numberFormat>";
            XElement numberFormatElement = parseXmlString(xmlInput);
            var metadata = new PhoneMetaData();
            string nationalFormat = "$1 $2 $3";

            Assert.IsFalse(PhoneNumberMetaDataReader.LoadInternationalFormat(metadata, numberFormatElement,
                                                                     nationalFormat));
            Assert.AreEqual(nationalFormat, metadata.getIntlNumberFormat(0).Format);
        }

        // Tests loadNationalFormat().
        [TestMethod]
        public void testLoadNationalFormat()
        {
            string nationalFormat = "$1 $2";
            string xmlInput = string.Format("<numberFormat><format>{0}</format></numberFormat>", nationalFormat);
            XElement numberFormatElement = parseXmlString(xmlInput);
            var metadata = new PhoneMetaData();
            var numberFormat = new PhoneMetaDataNumberFormat();

            Assert.AreEqual(nationalFormat, PhoneNumberMetaDataReader.LoadNationalFormat(metadata, numberFormatElement, numberFormat));
        }

        [TestMethod]
        public void testLoadNationalFormatRequiresFormat()
        {
            string xmlInput = "<numberFormat></numberFormat>";
            XElement numberFormatElement = parseXmlString(xmlInput);
            var metadata = new PhoneMetaData();
            var numberFormat = new PhoneMetaDataNumberFormat();

            try
            {
                PhoneNumberMetaDataReader.LoadNationalFormat(metadata, numberFormatElement, numberFormat);
                Assert.Fail();
            }
            catch (Exception e)
            {
                // Test passed.
            }
        }

        [TestMethod]
        public void testLoadNationalFormatExpectsExactlyOneFormat()
        {
            string xmlInput = "<numberFormat><format/><format/></numberFormat>";
            XElement numberFormatElement = parseXmlString(xmlInput);
            var metadata = new PhoneMetaData();
            var numberFormat = new PhoneMetaDataNumberFormat();

            try
            {
                PhoneNumberMetaDataReader.LoadNationalFormat(metadata, numberFormatElement, numberFormat);
                Assert.Fail();
            }
            catch (Exception e)
            {
                // Test passed.
            }
        }

        // Tests loadAvailableFormats().
        [TestMethod]
        public void testLoadAvailableFormats()
        {
            string xmlInput =
                "<territory >" +
                "  <availableFormats>" +
                "    <numberFormat nationalPrefixFormattingRule='($FG)'" +
                "                  carrierCodeFormattingRule='$NP $CC ($FG)'>" +
                "      <format>$1 $2 $3</format>" +
                "    </numberFormat>" +
                "  </availableFormats>" +
                "</territory>";
            XElement element = parseXmlString(xmlInput);
            var metadata = new PhoneMetaData();
            PhoneNumberMetaDataReader.LoadAvailableFormats(metadata, element, "0", "", false /* NP not optional */);
            Assert.AreEqual("($1)", metadata.getNumberFormat(0).NationalPrefixFormattingRule);
            Assert.AreEqual("0 $CC ($1)", metadata.getNumberFormat(0).DomesticCarrierCodeFormattingRule);
            Assert.AreEqual("$1 $2 $3", metadata.getNumberFormat(0).Format);
        }

        [TestMethod]
        public void testLoadAvailableFormatsPropagatesCarrierCodeFormattingRule()
        {
            string xmlInput =
                "<territory carrierCodeFormattingRule='$NP $CC ($FG)'>" +
                "  <availableFormats>" +
                "    <numberFormat nationalPrefixFormattingRule='($FG)'>" +
                "      <format>$1 $2 $3</format>" +
                "    </numberFormat>" +
                "  </availableFormats>" +
                "</territory>";
            XElement element = parseXmlString(xmlInput);
            var metadata = new PhoneMetaData();
            PhoneNumberMetaDataReader.LoadAvailableFormats(
                metadata, element, "0", "", false /* NP not optional */);
            Assert.AreEqual("($1)", metadata.getNumberFormat(0).NationalPrefixFormattingRule);
            Assert.AreEqual("0 $CC ($1)", metadata.getNumberFormat(0).DomesticCarrierCodeFormattingRule);
            Assert.AreEqual("$1 $2 $3", metadata.getNumberFormat(0).Format);
        }

        [TestMethod]
        public void testLoadAvailableFormatsSetsProvidedNationalPrefixFormattingRule()
        {
            string xmlInput =
                "<territory>" +
                "  <availableFormats>" +
                "    <numberFormat><format>$1 $2 $3</format></numberFormat>" +
                "  </availableFormats>" +
                "</territory>";
            XElement element = parseXmlString(xmlInput);
            var metadata = new PhoneMetaData();
            PhoneNumberMetaDataReader.LoadAvailableFormats(metadata, element, "", "($1)", false /* NP not optional */);
            Assert.AreEqual("($1)", metadata.getNumberFormat(0).NationalPrefixFormattingRule);
        }

        [TestMethod]
        public void testLoadAvailableFormatsClearsIntlFormat()
        {
            string xmlInput =
                "<territory>" +
                "  <availableFormats>" +
                "    <numberFormat><format>$1 $2 $3</format></numberFormat>" +
                "  </availableFormats>" +
                "</territory>";
            XElement element = parseXmlString(xmlInput);
            var metadata = new PhoneMetaData();
            PhoneNumberMetaDataReader.LoadAvailableFormats(metadata, element, "0", "($1)", false /* NP not optional */);
            Assert.AreEqual(0, metadata.intlNumberFormatSize);
        }

        [TestMethod]
        public void testLoadAvailableFormatsHandlesMultipleNumberFormats()
        {
            string xmlInput =
                "<territory>" +
                "  <availableFormats>" +
                "    <numberFormat><format>$1 $2 $3</format></numberFormat>" +
                "    <numberFormat><format>$1-$2</format></numberFormat>" +
                "  </availableFormats>" +
                "</territory>";
            XElement element = parseXmlString(xmlInput);
            var metadata = new PhoneMetaData();
            PhoneNumberMetaDataReader.LoadAvailableFormats(metadata, element, "0", "($1)", false /* NP not optional */);
            Assert.AreEqual("$1 $2 $3", metadata.getNumberFormat(0).Format);
            Assert.AreEqual("$1-$2", metadata.getNumberFormat(1).Format);
        }

        [TestMethod]
        public void testLoadInternationalFormatDoesNotSetIntlFormatWhenNA()
        {
            string xmlInput = "<numberFormat><intlFormat>NA</intlFormat></numberFormat>";
            XElement numberFormatElement = parseXmlString(xmlInput);
            var metadata = new PhoneMetaData();
            string nationalFormat = "$1 $2";

            PhoneNumberMetaDataReader.LoadInternationalFormat(metadata, numberFormatElement, nationalFormat);
            Assert.AreEqual(0, metadata.intlNumberFormatSize);
        }

        // Tests setLeadingDigitsPatterns().
        [TestMethod]
        public void testSetLeadingDigitsPatterns()
        {
            string xmlInput =
                "<numberFormat>" +
                "<leadingDigits>1</leadingDigits><leadingDigits>2</leadingDigits>" +
                "</numberFormat>";
            XElement numberFormatElement = parseXmlString(xmlInput);
            var numberFormat = new PhoneMetaDataNumberFormat();
            PhoneNumberMetaDataReader.SetLeadingDigitsPatterns(numberFormatElement, numberFormat);

            Assert.AreEqual("1", numberFormat.getLeadingDigitsPattern(0));
            Assert.AreEqual("2", numberFormat.getLeadingDigitsPattern(1));
        }

        // Tests getNationalPrefixFormattingRuleFromElement().
        [TestMethod]
        public void testGetNationalPrefixFormattingRuleFromElement()
        {
            string xmlInput = "<territory nationalPrefixFormattingRule='$NP$FG'/>";
            XElement element = parseXmlString(xmlInput);
            Assert.AreEqual("0$1", PhoneNumberMetaDataReader.GetNationalPrefixFormattingRuleFromElement(element, "0"));
        }

        // Tests getDomesticCarrierCodeFormattingRuleFromElement().
        [TestMethod]
        public void testGetDomesticCarrierCodeFormattingRuleFromElement()
        {
            string xmlInput = "<territory carrierCodeFormattingRule='$NP$CC $FG'/>";
            XElement element = parseXmlString(xmlInput);
            Assert.AreEqual("0$CC $1", PhoneNumberMetaDataReader.GetDomesticCarrierCodeFormattingRuleFromElement(element, "0"));
        }

        // Tests isValidNumberType().
        [TestMethod]
        public void testIsValidNumberTypeWithInvalidInput()
        {
            Assert.IsFalse(PhoneNumberMetaDataReader.IsValidNumberType("invalidType"));
        }

        // Tests processPhoneNumberDescElement().
        [TestMethod]
        public void testProcessPhoneNumberDescElementWithInvalidInput()
        
    {
        var generalDesc = new PhoneNumberDesc();
        XElement territoryElement = parseXmlString("<territory/>");
        PhoneNumberDesc phoneNumberDesc;

        phoneNumberDesc = PhoneNumberMetaDataReader.ProcessPhoneNumberDescElement(generalDesc, territoryElement, "invalidType", false);
        Assert.AreEqual("NA", phoneNumberDesc.PossibleNumberPattern);
        Assert.AreEqual("NA", phoneNumberDesc.NationalNumberPattern);
    }

        [TestMethod]
        public void testProcessPhoneNumberDescElementMergesWithGeneralDesc()
        {
            PhoneNumberDesc generalDesc = PhoneNumberDesc.newBuilder();
            generalDesc.setPossibleNumberPattern("\\d{6}");
            XElement territoryElement = parseXmlString("<territory><fixedLine/></territory>");
            PhoneNumberDesc phoneNumberDesc;

            phoneNumberDesc = PhoneNumberMetaDataReader.ProcessPhoneNumberDescElement(generalDesc, territoryElement, "fixedLine", false);
            Assert.AreEqual("\\d{6}", phoneNumberDesc.PossibleNumberPattern);
        }

        [TestMethod]
        public void testProcessPhoneNumberDescElementOverridesGeneralDesc()
        {
            PhoneNumberDesc generalDesc = PhoneNumberDesc.newBuilder();
            generalDesc.setPossibleNumberPattern("\\d{8}");
            string xmlInput =
                "<territory><fixedLine>" +
                "  <possibleNumberPattern>\\d{6}</possibleNumberPattern>" +
                "</fixedLine></territory>";
            XElement territoryElement = parseXmlString(xmlInput);
            PhoneNumberDesc phoneNumberDesc;

            phoneNumberDesc = PhoneNumberMetaDataReader.ProcessPhoneNumberDescElement(generalDesc, territoryElement, "fixedLine", false);
            Assert.AreEqual("\\d{6}", phoneNumberDesc.PossibleNumberPattern);
        }

        [TestMethod]
        public void testProcessPhoneNumberDescElementHandlesLiteBuild()
        {
            var generalDesc = new PhoneNumberDesc();
            string xmlInput =
                "<territory><fixedLine>" +
                "  <exampleNumber>01 01 01 01</exampleNumber>" +
                "</fixedLine></territory>";
            XElement territoryElement = parseXmlString(xmlInput);
            PhoneNumberDesc phoneNumberDesc;

            phoneNumberDesc = PhoneNumberMetaDataReader.ProcessPhoneNumberDescElement(generalDesc, territoryElement, "fixedLine", true);
            Assert.AreEqual("", phoneNumberDesc.ExampleNumber);
        }

        [TestMethod]
        public void testProcessPhoneNumberDescOutputsExampleNumberByDefault()
        {
            var generalDesc = new PhoneNumberDesc();
            string xmlInput =
                "<territory><fixedLine>" +
                "  <exampleNumber>01 01 01 01</exampleNumber>" +
                "</fixedLine></territory>";
            XElement territoryElement = parseXmlString(xmlInput);
            PhoneNumberDesc phoneNumberDesc;

            phoneNumberDesc = PhoneNumberMetaDataReader.ProcessPhoneNumberDescElement(generalDesc, territoryElement, "fixedLine", false);
            Assert.AreEqual("01 01 01 01", phoneNumberDesc.ExampleNumber);
        }

        [TestMethod]
        public void testProcessPhoneNumberDescRemovesWhiteSpacesInPatterns()
        {
            var generalDesc = new PhoneNumberDesc();
            string xmlInput =
                "<territory><fixedLine>" +
                "  <possibleNumberPattern>\t \\d { 6 } </possibleNumberPattern>" +
                "</fixedLine></territory>";
            XElement countryElement = parseXmlString(xmlInput);
            PhoneNumberDesc phoneNumberDesc;

            phoneNumberDesc = PhoneNumberMetaDataReader.ProcessPhoneNumberDescElement(generalDesc, countryElement, "fixedLine", false);
            Assert.AreEqual("\\d{6}", phoneNumberDesc.PossibleNumberPattern);
        }

        // Tests loadGeneralDesc().
        [TestMethod]
        public void testLoadGeneralDescSetsSameMobileAndFixedLinePattern()
        {
            string xmlInput =
                "<territory countryCode=\"33\">" +
                "  <fixedLine><nationalNumberPattern>\\d{6}</nationalNumberPattern></fixedLine>" +
                "  <mobile><nationalNumberPattern>\\d{6}</nationalNumberPattern></mobile>" +
                "</territory>";
            XElement territoryElement = parseXmlString(xmlInput);
            var metadata = new PhoneMetaData();
            // Should set sameMobileAndFixedPattern to true.
            PhoneNumberMetaDataReader.LoadGeneralDesc(metadata, territoryElement, false);
            Assert.IsTrue(metadata.SameMobileAndFixedLinePattern);
        }

        [TestMethod]
        public void testLoadGeneralDescSetsAllDescriptions()
        {
            string xmlInput =
                "<territory countryCode=\"33\">" +
                "  <fixedLine><nationalNumberPattern>\\d{1}</nationalNumberPattern></fixedLine>" +
                "  <mobile><nationalNumberPattern>\\d{2}</nationalNumberPattern></mobile>" +
                "  <pager><nationalNumberPattern>\\d{3}</nationalNumberPattern></pager>" +
                "  <tollFree><nationalNumberPattern>\\d{4}</nationalNumberPattern></tollFree>" +
                "  <premiumRate><nationalNumberPattern>\\d{5}</nationalNumberPattern></premiumRate>" +
                "  <sharedCost><nationalNumberPattern>\\d{6}</nationalNumberPattern></sharedCost>" +
                "  <personalNumber><nationalNumberPattern>\\d{7}</nationalNumberPattern></personalNumber>" +
                "  <voip><nationalNumberPattern>\\d{8}</nationalNumberPattern></voip>" +
                "  <uan><nationalNumberPattern>\\d{9}</nationalNumberPattern></uan>" +
                "  <shortCode><nationalNumberPattern>\\d{10}</nationalNumberPattern></shortCode>" +
                "</territory>";
            XElement territoryElement = parseXmlString(xmlInput);
            var metadata = new PhoneMetaData();
            PhoneNumberMetaDataReader.LoadGeneralDesc(metadata, territoryElement, false);
            Assert.AreEqual("\\d{1}", metadata.FixedLine.NationalNumberPattern);
            Assert.AreEqual("\\d{2}", metadata.Mobile.NationalNumberPattern);
            Assert.AreEqual("\\d{3}", metadata.Pager.NationalNumberPattern);
            Assert.AreEqual("\\d{4}", metadata.TollFree.NationalNumberPattern);
            Assert.AreEqual("\\d{5}", metadata.PremiumRate.NationalNumberPattern);
            Assert.AreEqual("\\d{6}", metadata.SharedCost.NationalNumberPattern);
            Assert.AreEqual("\\d{7}", metadata.PersonalNumber.NationalNumberPattern);
            Assert.AreEqual("\\d{8}", metadata.Voip.NationalNumberPattern);
            Assert.AreEqual("\\d{9}", metadata.Uan.NationalNumberPattern);
        }

        #region Helper methods

        private XElement parseXmlString(string xmlString)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(xmlString)))
            {
                return XElement.Load(ms);
            }
        }

        #endregion
    }
}