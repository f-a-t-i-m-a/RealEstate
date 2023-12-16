using JahanJooy.Common.Util.PhoneNumbers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JahanJooy.Common.Util.Test.PhoneNumbers
{
    [TestClass]
    public class PhoneNumberTest
    {
        [TestMethod]
        public void testEqualSimpleNumber()
        {
            var numberA = new PhoneNumber();
            numberA.SetCountryCode(1).SetNationalNumber(6502530000L);

            var numberB = new PhoneNumber();
            numberB.SetCountryCode(1).SetNationalNumber(6502530000L);

            Assert.AreEqual(numberA, numberB);
            Assert.AreEqual(numberA.GetHashCode(), numberB.GetHashCode());
        }

        [TestMethod]
        public void testEqualWithItalianLeadingZeroSetToDefault()
        {
            PhoneNumber numberA = new PhoneNumber();
            numberA.SetCountryCode(1).SetNationalNumber(6502530000L).setItalianLeadingZero(false);

            PhoneNumber numberB = new PhoneNumber();
            numberB.SetCountryCode(1).SetNationalNumber(6502530000L);

            // These should still be equal, since the default value for this field is false.
            Assert.AreEqual(numberA, numberB);
            Assert.AreEqual(numberA.GetHashCode(), numberB.GetHashCode());
        }

        [TestMethod]
        public void testEqualWithCountryCodeSourceSet()
        {
            PhoneNumber numberA = new PhoneNumber();
            numberA.SetRawInput("+1 650 253 00 00").
                SetCountryCodeSource(CountryCodeSource.FROM_NUMBER_WITH_PLUS_SIGN);
            PhoneNumber numberB = new PhoneNumber();
            numberB.SetRawInput("+1 650 253 00 00").
                SetCountryCodeSource(CountryCodeSource.FROM_NUMBER_WITH_PLUS_SIGN);
            Assert.AreEqual(numberA, numberB);
            Assert.AreEqual(numberA.GetHashCode(), numberB.GetHashCode());
        }

        [TestMethod]
        public void testNonEqualWithItalianLeadingZeroSetToTrue()
        {
            PhoneNumber numberA = new PhoneNumber();
            numberA.SetCountryCode(1).SetNationalNumber(6502530000L).setItalianLeadingZero(true);

            PhoneNumber numberB = new PhoneNumber();
            numberB.SetCountryCode(1).SetNationalNumber(6502530000L);

            Assert.IsFalse(numberA.Equals(numberB));
            Assert.IsFalse(numberA.GetHashCode() == numberB.GetHashCode());
        }

        [TestMethod]
        public void testNonEqualWithDifferingRawInput()
        {
            PhoneNumber numberA = new PhoneNumber();
            numberA.SetCountryCode(1).SetNationalNumber(6502530000L).SetRawInput("+1 650 253 00 00").
                SetCountryCodeSource(CountryCodeSource.FROM_NUMBER_WITH_PLUS_SIGN);

            PhoneNumber numberB = new PhoneNumber();
            // Although these numbers would pass an isNumberMatch test, they are not considered "equal" as
            // objects, since their raw input is different.
            numberB.SetCountryCode(1).SetNationalNumber(6502530000L).SetRawInput("+1-650-253-00-00").
                SetCountryCodeSource(CountryCodeSource.FROM_NUMBER_WITH_PLUS_SIGN);

            Assert.IsFalse(numberA.Equals(numberB));
            Assert.IsFalse(numberA.GetHashCode() == numberB.GetHashCode());
        }

        [TestMethod]
        public void testNonEqualWithPreferredDomesticCarrierCodeSetToDefault()
        {
            PhoneNumber numberA = new PhoneNumber();
            numberA.SetCountryCode(1).SetNationalNumber(6502530000L).SetPreferredDomesticCarrierCode("");

            PhoneNumber numberB = new PhoneNumber();
            numberB.SetCountryCode(1).SetNationalNumber(6502530000L);

            Assert.IsFalse(numberA.Equals(numberB));
            Assert.IsFalse(numberA.GetHashCode() == numberB.GetHashCode());
        }

        [TestMethod]
        public void testEqualWithPreferredDomesticCarrierCodeSetToDefault()
        {
            PhoneNumber numberA = new PhoneNumber();
            numberA.SetCountryCode(1).SetNationalNumber(6502530000L).SetPreferredDomesticCarrierCode("");

            PhoneNumber numberB = new PhoneNumber();
            numberB.SetCountryCode(1).SetNationalNumber(6502530000L).SetPreferredDomesticCarrierCode("");

            Assert.AreEqual(numberA, numberB);
            Assert.AreEqual(numberA.GetHashCode(), numberB.GetHashCode());
        }
    }
}