using JahanJooy.Common.Util.PhoneNumbers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JahanJooy.Common.Util.Test.PhoneNumbers
{
    /**
     * Unit tests for ShortNumberUtil.java
     *
     * @author Shaopeng Jia
     */
    [TestClass]
    public class ShortPhoneNumberUtilTest : TestMetaDataBase
    {
        private readonly ShortPhoneNumberUtil shortUtil;

        public ShortPhoneNumberUtilTest()
        {
            shortUtil = new ShortPhoneNumberUtil(phoneUtil);
        }

        [TestMethod]
        public void testConnectsToEmergencyNumber_US()
        {
            Assert.IsTrue(shortUtil.connectsToEmergencyNumber("911", RegionCode.US));
            Assert.IsTrue(shortUtil.connectsToEmergencyNumber("119", RegionCode.US));
            Assert.IsFalse(shortUtil.connectsToEmergencyNumber("999", RegionCode.US));
        }

        [TestMethod]
        public void testConnectsToEmergencyNumberLongNumber_US()
        {
            Assert.IsTrue(shortUtil.connectsToEmergencyNumber("9116666666", RegionCode.US));
            Assert.IsTrue(shortUtil.connectsToEmergencyNumber("1196666666", RegionCode.US));
            Assert.IsFalse(shortUtil.connectsToEmergencyNumber("9996666666", RegionCode.US));
        }

        [TestMethod]
        public void testConnectsToEmergencyNumberWithFormatting_US()
        {
            Assert.IsTrue(shortUtil.connectsToEmergencyNumber("9-1-1", RegionCode.US));
            Assert.IsTrue(shortUtil.connectsToEmergencyNumber("1-1-9", RegionCode.US));
            Assert.IsFalse(shortUtil.connectsToEmergencyNumber("9-9-9", RegionCode.US));
        }

        [TestMethod]
        public void testConnectsToEmergencyNumberWithPlusSign_US()
        {
            Assert.IsFalse(shortUtil.connectsToEmergencyNumber("+911", RegionCode.US));
            Assert.IsFalse(shortUtil.connectsToEmergencyNumber("\uFF0B911", RegionCode.US));
            Assert.IsFalse(shortUtil.connectsToEmergencyNumber(" +911", RegionCode.US));
            Assert.IsFalse(shortUtil.connectsToEmergencyNumber("+119", RegionCode.US));
            Assert.IsFalse(shortUtil.connectsToEmergencyNumber("+999", RegionCode.US));
        }

        [TestMethod]
        public void testConnectsToEmergencyNumber_BR()
        {
            Assert.IsTrue(shortUtil.connectsToEmergencyNumber("911", RegionCode.BR));
            Assert.IsTrue(shortUtil.connectsToEmergencyNumber("190", RegionCode.BR));
            Assert.IsFalse(shortUtil.connectsToEmergencyNumber("999", RegionCode.BR));
        }

        [TestMethod]
        public void testConnectsToEmergencyNumberLongNumber_BR()
        {
            // Brazilian emergency numbers don't work when additional digits are appended.
            Assert.IsFalse(shortUtil.connectsToEmergencyNumber("9111", RegionCode.BR));
            Assert.IsFalse(shortUtil.connectsToEmergencyNumber("1900", RegionCode.BR));
            Assert.IsFalse(shortUtil.connectsToEmergencyNumber("9996", RegionCode.BR));
        }

        [TestMethod]
        public void testConnectsToEmergencyNumber_AO()
        {
            // Angola doesn't have any metadata for emergency numbers in the test metadata.
            Assert.IsFalse(shortUtil.connectsToEmergencyNumber("911", RegionCode.AO));
            Assert.IsFalse(shortUtil.connectsToEmergencyNumber("222123456", RegionCode.AO));
            Assert.IsFalse(shortUtil.connectsToEmergencyNumber("923123456", RegionCode.AO));
        }

        [TestMethod]
        public void testConnectsToEmergencyNumber_ZW()
        {
            // Zimbabwe doesn't have any metadata in the test metadata.
            Assert.IsFalse(shortUtil.connectsToEmergencyNumber("911", RegionCode.ZW));
            Assert.IsFalse(shortUtil.connectsToEmergencyNumber("01312345", RegionCode.ZW));
            Assert.IsFalse(shortUtil.connectsToEmergencyNumber("0711234567", RegionCode.ZW));
        }

        [TestMethod]
        public void testIsEmergencyNumber_US()
        {
            Assert.IsTrue(shortUtil.isEmergencyNumber("911", RegionCode.US));
            Assert.IsTrue(shortUtil.isEmergencyNumber("119", RegionCode.US));
            Assert.IsFalse(shortUtil.isEmergencyNumber("999", RegionCode.US));
        }

        [TestMethod]
        public void testIsEmergencyNumberLongNumber_US()
        {
            Assert.IsFalse(shortUtil.isEmergencyNumber("9116666666", RegionCode.US));
            Assert.IsFalse(shortUtil.isEmergencyNumber("1196666666", RegionCode.US));
            Assert.IsFalse(shortUtil.isEmergencyNumber("9996666666", RegionCode.US));
        }

        [TestMethod]
        public void testIsEmergencyNumberWithFormatting_US()
        {
            Assert.IsTrue(shortUtil.isEmergencyNumber("9-1-1", RegionCode.US));
            Assert.IsTrue(shortUtil.isEmergencyNumber("*911", RegionCode.US));
            Assert.IsTrue(shortUtil.isEmergencyNumber("1-1-9", RegionCode.US));
            Assert.IsTrue(shortUtil.isEmergencyNumber("*119", RegionCode.US));
            Assert.IsFalse(shortUtil.isEmergencyNumber("9-9-9", RegionCode.US));
            Assert.IsFalse(shortUtil.isEmergencyNumber("*999", RegionCode.US));
        }

        [TestMethod]
        public void testIsEmergencyNumberWithPlusSign_US()
        {
            Assert.IsFalse(shortUtil.isEmergencyNumber("+911", RegionCode.US));
            Assert.IsFalse(shortUtil.isEmergencyNumber("\uFF0B911", RegionCode.US));
            Assert.IsFalse(shortUtil.isEmergencyNumber(" +911", RegionCode.US));
            Assert.IsFalse(shortUtil.isEmergencyNumber("+119", RegionCode.US));
            Assert.IsFalse(shortUtil.isEmergencyNumber("+999", RegionCode.US));
        }

        [TestMethod]
        public void testIsEmergencyNumber_BR()
        {
            Assert.IsTrue(shortUtil.isEmergencyNumber("911", RegionCode.BR));
            Assert.IsTrue(shortUtil.isEmergencyNumber("190", RegionCode.BR));
            Assert.IsFalse(shortUtil.isEmergencyNumber("999", RegionCode.BR));
        }

        [TestMethod]
        public void testIsEmergencyNumberLongNumber_BR()
        {
            Assert.IsFalse(shortUtil.isEmergencyNumber("9111", RegionCode.BR));
            Assert.IsFalse(shortUtil.isEmergencyNumber("1900", RegionCode.BR));
            Assert.IsFalse(shortUtil.isEmergencyNumber("9996", RegionCode.BR));
        }

        [TestMethod]
        public void testIsEmergencyNumber_AO()
        {
            // Angola doesn't have any metadata for emergency numbers in the test metadata.
            Assert.IsFalse(shortUtil.isEmergencyNumber("911", RegionCode.AO));
            Assert.IsFalse(shortUtil.isEmergencyNumber("222123456", RegionCode.AO));
            Assert.IsFalse(shortUtil.isEmergencyNumber("923123456", RegionCode.AO));
        }

        [TestMethod]
        public void testIsEmergencyNumber_ZW()
        {
            // Zimbabwe doesn't have any metadata in the test metadata.
            Assert.IsFalse(shortUtil.isEmergencyNumber("911", RegionCode.ZW));
            Assert.IsFalse(shortUtil.isEmergencyNumber("01312345", RegionCode.ZW));
            Assert.IsFalse(shortUtil.isEmergencyNumber("0711234567", RegionCode.ZW));
        }
    }
}