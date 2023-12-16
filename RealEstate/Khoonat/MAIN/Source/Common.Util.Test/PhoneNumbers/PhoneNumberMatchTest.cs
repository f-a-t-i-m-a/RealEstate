using System;
using JahanJooy.Common.Util.PhoneNumbers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JahanJooy.Common.Util.Test.PhoneNumbers
{
    [TestClass]
    public class PhoneNumberMatchTest
    {
        /**
   * Tests the value type semantics. Equality and hash code must be based on the covered range and
   * corresponding phone number. Range and number correctness are tested by
   * {@link PhoneNumberMatcherTest}.
   */

        [TestMethod]
        public void testValueTypeSemantics()
        {
            var number = new PhoneNumber();
            PhoneNumberMatch match1 = new PhoneNumberMatch(10, "1 800 234 45 67", number);
            PhoneNumberMatch match2 = new PhoneNumberMatch(10, "1 800 234 45 67", number);

            Assert.AreEqual(match1, match2);
            Assert.AreEqual(match1.GetHashCode(), match2.GetHashCode());
            Assert.AreEqual(match1.Start, match2.Start);
            Assert.AreEqual(match1.End, match2.End);
            Assert.AreEqual(match1.Number, match2.Number);
            Assert.AreEqual(match1.RawString, match2.RawString);
            Assert.AreEqual("1 800 234 45 67", match1.RawString);
        }

        /**
   * Tests the value type semantics for matches with a null number.
   */

        [TestMethod]
        public void testIllegalArguments()
        {
            try
            {
                new PhoneNumberMatch(-110, "1 800 234 45 67", new PhoneNumber());
                Assert.Fail();
            }
            catch (ArgumentException)
            {
                /* success */
            }

            try
            {
                new PhoneNumberMatch(10, "1 800 234 45 67", null);
                Assert.Fail();
            }
            catch (NullReferenceException)
            {
                /* success */
            }

            try
            {
                new PhoneNumberMatch(10, null, new PhoneNumber());
                Assert.Fail();
            }
            catch (NullReferenceException)
            {
                /* success */
            }

            try
            {
                new PhoneNumberMatch(10, null, null);
                Assert.Fail();
            }
            catch (NullReferenceException)
            {
                /* success */
            }
        }
    }
}