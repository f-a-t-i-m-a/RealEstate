using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using JahanJooy.Common.Util.PhoneNumbers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JahanJooy.Common.Util.Text;

namespace JahanJooy.Common.Util.Test.PhoneNumbers
{
    [TestClass]
    public class ExampleNumbersTest
    {
        private PhoneNumberUtil phoneNumberUtil;
        private IList<PhoneNumber> invalidCases = new List<PhoneNumber>();
        private IList<PhoneNumber> wrongTypeCases = new List<PhoneNumber>();

        public ExampleNumbersTest()
        {
            PhoneNumberUtil.resetInstance();
            phoneNumberUtil = PhoneNumberUtil.getInstance();
        }

        [TestInitialize]
        protected void setUp()
        {
            invalidCases.Clear();
            wrongTypeCases.Clear();
        }

        [TestCleanup]
        protected void tearDown()
        {
        }

        /**
        * @param exampleNumberRequestedType  type we are requesting an example number for
        * @param possibleExpectedTypes       acceptable types that this number should match, such as
        *     FIXED_LINE and FIXED_LINE_OR_MOBILE for a fixed line example number.
        */
        private void checkNumbersValidAndCorrectType(PhoneNumberType exampleNumberRequestedType,
                                                     ISet<PhoneNumberType> possibleExpectedTypes)
        {
            foreach (String regionCode in phoneNumberUtil.getSupportedRegions())
            {
                PhoneNumber exampleNumber =
                    phoneNumberUtil.getExampleNumberForType(regionCode, exampleNumberRequestedType);
                if (exampleNumber != null)
                {
                    if (!phoneNumberUtil.isValidNumber(exampleNumber))
                    {
                        invalidCases.Add(exampleNumber);
                        Debug.WriteLine("Failed validation for " + exampleNumber);
                    }
                    else
                    {
                        // We know the number is valid, now we check the type.
                        PhoneNumberType exampleNumberType = phoneNumberUtil.getNumberType(exampleNumber);
                        if (!possibleExpectedTypes.Contains(exampleNumberType))
                        {
                            wrongTypeCases.Add(exampleNumber);
                            Debug.WriteLine("Wrong type for " + exampleNumber + ": got " + exampleNumberType);
                            Debug.WriteLine("Expected types: ");
                            foreach (PhoneNumberType type in possibleExpectedTypes)
                            {
                                Debug.WriteLine(type.ToString());
                            }
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void testFixedLine()
        {
            ISet<PhoneNumberType> fixedLineTypes = new HashSet<PhoneNumberType> {PhoneNumberType.FIXED_LINE, PhoneNumberType.FIXED_LINE_OR_MOBILE};
            checkNumbersValidAndCorrectType(PhoneNumberType.FIXED_LINE, fixedLineTypes);
            Assert.AreEqual(0, invalidCases.Count);
            Assert.AreEqual(0, wrongTypeCases.Count);
        }

        [TestMethod]
        public void testMobile()
        {
            ISet<PhoneNumberType> mobileTypes = new HashSet<PhoneNumberType> {PhoneNumberType.MOBILE, PhoneNumberType.FIXED_LINE_OR_MOBILE};
            checkNumbersValidAndCorrectType(PhoneNumberType.MOBILE, mobileTypes);
            Assert.AreEqual(0, invalidCases.Count);
            Assert.AreEqual(0, wrongTypeCases.Count);
        }

        [TestMethod]
        public void testTollFree()
        {
            ISet<PhoneNumberType> tollFreeTypes = new HashSet<PhoneNumberType> {PhoneNumberType.TOLL_FREE};
            checkNumbersValidAndCorrectType(PhoneNumberType.TOLL_FREE, tollFreeTypes);
            Assert.AreEqual(0, invalidCases.Count);
            Assert.AreEqual(0, wrongTypeCases.Count);
        }

        [TestMethod]
        public void testPremiumRate()
        {
            ISet<PhoneNumberType> premiumRateTypes = new HashSet<PhoneNumberType> {PhoneNumberType.PREMIUM_RATE};
            checkNumbersValidAndCorrectType(PhoneNumberType.PREMIUM_RATE, premiumRateTypes);
            Assert.AreEqual(0, invalidCases.Count);
            Assert.AreEqual(0, wrongTypeCases.Count);
        }

        [TestMethod]
        public void testVoip()
        {
            ISet<PhoneNumberType> voipTypes = new HashSet<PhoneNumberType> {PhoneNumberType.VOIP};
            checkNumbersValidAndCorrectType(PhoneNumberType.VOIP, voipTypes);
            Assert.AreEqual(0, invalidCases.Count);
            Assert.AreEqual(0, wrongTypeCases.Count);
        }

        [TestMethod]
        public void testPager()
        {
            ISet<PhoneNumberType> pagerTypes = new HashSet<PhoneNumberType> {PhoneNumberType.PAGER};
            checkNumbersValidAndCorrectType(PhoneNumberType.PAGER, pagerTypes);
            Assert.AreEqual(0, invalidCases.Count);
            Assert.AreEqual(0, wrongTypeCases.Count);
        }

        [TestMethod]
        public void testUan()
        {
            ISet<PhoneNumberType> uanTypes = new HashSet<PhoneNumberType> {PhoneNumberType.UAN};
            checkNumbersValidAndCorrectType(PhoneNumberType.UAN, uanTypes);
            Assert.AreEqual(0, invalidCases.Count);
            Assert.AreEqual(0, wrongTypeCases.Count);
        }

        [TestMethod]
        public void testVoicemail()
        {
            ISet<PhoneNumberType> voicemailTypes = new HashSet<PhoneNumberType> {PhoneNumberType.VOICEMAIL};
            checkNumbersValidAndCorrectType(PhoneNumberType.VOICEMAIL, voicemailTypes);
            Assert.AreEqual(0, invalidCases.Count);
            Assert.AreEqual(0, wrongTypeCases.Count);
        }

        [TestMethod]
        public void testSharedCost()
        {
            ISet<PhoneNumberType> sharedCostTypes = new HashSet<PhoneNumberType> {PhoneNumberType.SHARED_COST};
            checkNumbersValidAndCorrectType(PhoneNumberType.SHARED_COST, sharedCostTypes);
            Assert.AreEqual(0, invalidCases.Count);
            Assert.AreEqual(0, wrongTypeCases.Count);
        }

        [TestMethod]
        public void testCanBeInternationallyDialled()
        {
            foreach (String regionCode in phoneNumberUtil.getSupportedRegions())
            {
                PhoneNumber exampleNumber = null;
                PhoneNumberDesc desc = phoneNumberUtil.getMetadataForRegion(regionCode).NoInternationalDialling;
                try
                {
                    if (desc.HasExampleNumber)
                    {
                        exampleNumber = phoneNumberUtil.parse(desc.ExampleNumber, regionCode);
                    }
                }
                catch (PhoneNumberParseException e)
                {
                    Debug.WriteLine(e.ToString());
                }
                if (exampleNumber != null && phoneNumberUtil.canBeInternationallyDialled(exampleNumber))
                {
                    wrongTypeCases.Add(exampleNumber);
                    Debug.WriteLine("Number " + exampleNumber + " should not be internationally diallable");
                }
            }
            Assert.AreEqual(0, wrongTypeCases.Count);
        }

        [TestMethod]
        public void testEmergency()
        {
            ShortPhoneNumberUtil shortUtil = new ShortPhoneNumberUtil(phoneNumberUtil);
            int wrongTypeCounter = 0;
            foreach (String regionCode in phoneNumberUtil.getSupportedRegions())
            {
                PhoneNumberDesc desc =
                    phoneNumberUtil.getMetadataForRegion(regionCode).Emergency;
                if (desc.HasExampleNumber)
                {
                    String exampleNumber = desc.ExampleNumber;
                    if (!new Regex(desc.PossibleNumberPattern).IsMatchWhole(exampleNumber) ||
                        !shortUtil.isEmergencyNumber(exampleNumber, regionCode))
                    {
                        wrongTypeCounter++;
                        Debug.WriteLine("Emergency example number test failed for " + regionCode);
                    }
                }
            }
            Assert.AreEqual(0, wrongTypeCounter);
        }

        [TestMethod]
        public void testGlobalNetworkNumbers()
        {
            foreach (int callingCode in phoneNumberUtil.getSupportedGlobalNetworkCallingCodes())
            {
                PhoneNumber exampleNumber = phoneNumberUtil.getExampleNumberForNonGeoEntity(callingCode);
                Assert.IsNotNull(exampleNumber, "No example phone number for calling code " + callingCode);

                if (!phoneNumberUtil.isValidNumber(exampleNumber))
                {
                    invalidCases.Add(exampleNumber);
                    Debug.WriteLine("Failed validation for " + exampleNumber);
                }
            }
        }

        [TestMethod]
        public void testEveryRegionHasAnExampleNumber()
        {
            foreach (String regionCode in phoneNumberUtil.getSupportedRegions())
            {
                PhoneNumber exampleNumber = phoneNumberUtil.getExampleNumber(regionCode);
                Assert.IsNotNull(exampleNumber, "None found for region " + regionCode);
            }
        }
    }
}