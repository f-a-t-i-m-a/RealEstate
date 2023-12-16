﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using JahanJooy.Common.Util.Collections;
using JahanJooy.Common.Util.PhoneNumbers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace JahanJooy.Common.Util.Test.PhoneNumbers
{
    [TestClass]
    public class PhoneNumberMatchEnumeratorTest : TestMetaDataBase
    {
        /** See {@link PhoneNumberUtilTest#testParseNationalNumber()}. */

        [TestMethod]
        public void testFindNationalNumber()
        {
            // same cases as in testParseNationalNumber
            doTestFindInContext("033316005", RegionCode.NZ);
            // ("33316005", RegionCode.NZ) is omitted since the national prefix is obligatory for these
            // types of numbers in New Zealand.
            // National prefix attached and some formatting present.
            doTestFindInContext("03-331 6005", RegionCode.NZ);
            doTestFindInContext("03 331 6005", RegionCode.NZ);
            // Testing international prefixes.
            // Should strip country code.
            doTestFindInContext("0064 3 331 6005", RegionCode.NZ);
            // Try again, but this time we have an international number with Region Code US. It should
            // recognize the country code and parse accordingly.
            doTestFindInContext("01164 3 331 6005", RegionCode.US);
            doTestFindInContext("+64 3 331 6005", RegionCode.US);

            doTestFindInContext("64(0)64123456", RegionCode.NZ);
            // Check that using a "/" is fine in a phone number.
            doTestFindInContext("123/45678", RegionCode.DE);
            doTestFindInContext("123-456-7890", RegionCode.US);
        }
        /** See {@link PhoneNumberUtilTest#testParseWithInternationalPrefixes()}. */

        [TestMethod]
        public void testFindWithInternationalPrefixes()
        {
            doTestFindInContext("+1 (650) 333-6000", RegionCode.NZ);
            doTestFindInContext("1-650-333-6000", RegionCode.US);
            // Calling the US number from Singapore by using different service providers
            // 1st test: calling using SingTel IDD service (IDD is 001)
            doTestFindInContext("0011-650-333-6000", RegionCode.SG);
            // 2nd test: calling using StarHub IDD service (IDD is 008)
            doTestFindInContext("0081-650-333-6000", RegionCode.SG);
            // 3rd test: calling using SingTel V019 service (IDD is 019)
            doTestFindInContext("0191-650-333-6000", RegionCode.SG);
            // Calling the US number from Poland
            doTestFindInContext("0~01-650-333-6000", RegionCode.PL);
            // Using "++" at the start.
            doTestFindInContext("++1 (650) 333-6000", RegionCode.PL);
            // Using a full-width plus sign.
            doTestFindInContext("\uFF0B1 (650) 333-6000", RegionCode.SG);
            // The whole number, including punctuation, is here represented in full-width form.
            doTestFindInContext("\uFF0B\uFF11\u3000\uFF08\uFF16\uFF15\uFF10\uFF09" +
                                "\u3000\uFF13\uFF13\uFF13\uFF0D\uFF16\uFF10\uFF10\uFF10",
                                RegionCode.SG);
        }

        /** See {@link PhoneNumberUtilTest#testParseWithLeadingZero()}. */
        [TestMethod]
        public void testFindWithLeadingZero()
        {
            doTestFindInContext("+39 02-36618 300", RegionCode.NZ);
            doTestFindInContext("02-36618 300", RegionCode.IT);
            doTestFindInContext("312 345 678", RegionCode.IT);
        }

        /** See {@link PhoneNumberUtilTest#testParseNationalNumberArgentina()}. */
        [TestMethod]
        public void testFindNationalNumberArgentina()
        {
            // Test parsing mobile numbers of Argentina.
            doTestFindInContext("+54 9 343 555 1212", RegionCode.AR);
            doTestFindInContext("0343 15 555 1212", RegionCode.AR);

            doTestFindInContext("+54 9 3715 65 4320", RegionCode.AR);
            doTestFindInContext("03715 15 65 4320", RegionCode.AR);

            // Test parsing fixed-line numbers of Argentina.
            doTestFindInContext("+54 11 3797 0000", RegionCode.AR);
            doTestFindInContext("011 3797 0000", RegionCode.AR);

            doTestFindInContext("+54 3715 65 4321", RegionCode.AR);
            doTestFindInContext("03715 65 4321", RegionCode.AR);

            doTestFindInContext("+54 23 1234 0000", RegionCode.AR);
            doTestFindInContext("023 1234 0000", RegionCode.AR);
        }

        /** See {@link PhoneNumberUtilTest#testParseWithXInNumber()}. */
        [TestMethod]
        public void testFindWithXInNumber()
        {
            doTestFindInContext("(0xx) 123456789", RegionCode.AR);
            // A case where x denotes both carrier codes and extension symbol.
            doTestFindInContext("(0xx) 123456789 x 1234", RegionCode.AR);

            // This test is intentionally constructed such that the number of digit after xx is larger than
            // 7, so that the number won't be mistakenly treated as an extension, as we allow extensions up
            // to 7 digits. This assumption is okay for now as all the countries where a carrier selection
            // code is written in the form of xx have a national significant number of length larger than 7.
            doTestFindInContext("011xx5481429712", RegionCode.US);
        }

        /** See {@link PhoneNumberUtilTest#testParseNumbersMexico()}. */
        [TestMethod]
        public void testFindNumbersMexico()
        {
            // Test parsing fixed-line numbers of Mexico.
            doTestFindInContext("+52 (449)978-0001", RegionCode.MX);
            doTestFindInContext("01 (449)978-0001", RegionCode.MX);
            doTestFindInContext("(449)978-0001", RegionCode.MX);

            // Test parsing mobile numbers of Mexico.
            doTestFindInContext("+52 1 33 1234-5678", RegionCode.MX);
            doTestFindInContext("044 (33) 1234-5678", RegionCode.MX);
            doTestFindInContext("045 33 1234-5678", RegionCode.MX);
        }

        /** See {@link PhoneNumberUtilTest#testParseNumbersWithPlusWithNoRegion()}. */
        [TestMethod]
        public void testFindNumbersWithPlusWithNoRegion()
        {
            // RegionCode.ZZ is allowed only if the number starts with a '+' - then the country code can be
            // calculated.
            doTestFindInContext("+64 3 331 6005", RegionCode.ZZ);
            // Null is also allowed for the region code in these cases.
            doTestFindInContext("+64 3 331 6005", null);
        }

        /** See {@link PhoneNumberUtilTest#testParseExtensions()}. */
        [TestMethod]
        public void testFindExtensions()
        {
            doTestFindInContext("03 331 6005 ext 3456", RegionCode.NZ);
            doTestFindInContext("03-3316005x3456", RegionCode.NZ);
            doTestFindInContext("03-3316005 int.3456", RegionCode.NZ);
            doTestFindInContext("03 3316005 #3456", RegionCode.NZ);
            doTestFindInContext("0~0 1800 7493 524", RegionCode.PL);
            doTestFindInContext("(1800) 7493.524", RegionCode.US);
            // Check that the last instance of an extension token is matched.
            doTestFindInContext("0~0 1800 7493 524 ~1234", RegionCode.PL);
            // Verifying bug-fix where the last digit of a number was previously omitted if it was a 0 when
            // extracting the extension. Also verifying a few different cases of extensions.
            doTestFindInContext("+44 2034567890x456", RegionCode.NZ);
            doTestFindInContext("+44 2034567890x456", RegionCode.GB);
            doTestFindInContext("+44 2034567890 x456", RegionCode.GB);
            doTestFindInContext("+44 2034567890 X456", RegionCode.GB);
            doTestFindInContext("+44 2034567890 X 456", RegionCode.GB);
            doTestFindInContext("+44 2034567890 X  456", RegionCode.GB);
            doTestFindInContext("+44 2034567890  X 456", RegionCode.GB);

            doTestFindInContext("(800) 901-3355 x 7246433", RegionCode.US);
            doTestFindInContext("(800) 901-3355 , ext 7246433", RegionCode.US);
            doTestFindInContext("(800) 901-3355 ,extension 7246433", RegionCode.US);
            // The next test differs from PhoneNumberUtil -> when matching we don't consider a lone comma to
            // indicate an extension, although we accept it when parsing.
            doTestFindInContext("(800) 901-3355 ,x 7246433", RegionCode.US);
            doTestFindInContext("(800) 901-3355 ext: 7246433", RegionCode.US);
        }

        [TestMethod]
        public void testFindInterspersedWithSpace()
        {
            doTestFindInContext("0 3   3 3 1   6 0 0 5", RegionCode.NZ);
        }

        /**
        * Test matching behavior when starting in the middle of a phone number.
        */
        [TestMethod]
        public void testIntermediateParsePositions()
        {
            String text = "Call 033316005  or 032316005!";
            //             |    |    |    |    |    |
            //             0    5   10   15   20   25

            // Iterate over all possible indices.
            for (int i = 0; i <= 5; i++)
            {
                assertEqualRange(text, i, 5, 14);
            }
            // 7 and 8 digits in a row are still parsed as number.
            assertEqualRange(text, 6, 6, 14);
            assertEqualRange(text, 7, 7, 14);
            // Anything smaller is skipped to the second instance.
            for (int i = 8; i <= 19; i++)
            {
                assertEqualRange(text, i, 19, 28);
            }
        }

        [TestMethod]
        public void testMatchWithSurroundingZipcodes()
        {
            String number = "415-666-7777";
            String zipPreceding = "My address is CA 34215 - " + number + " is my number.";
            PhoneNumber expectedResult = phoneUtil.parse(number, RegionCode.US);

            IEnumerator<PhoneNumberMatch> iterator = phoneUtil.findNumbers(zipPreceding, RegionCode.US).GetEnumerator();
            PhoneNumberMatch match = iterator.MoveNext() ? iterator.Current : null;
            Assert.IsNotNull(match, "Did not find a number in '" + zipPreceding + "'; expected " + number);
            Assert.AreEqual(expectedResult, match.Number);
            Assert.AreEqual(number, match.RawString);

            // Now repeat, but this time the phone number has spaces in it. It should still be found.
            number = "(415) 666 7777";

            String zipFollowing = "My number is " + number + ". 34215 is my zip-code.";
            iterator = phoneUtil.findNumbers(zipFollowing, RegionCode.US).GetEnumerator();

            PhoneNumberMatch matchWithSpaces = iterator.MoveNext() ? iterator.Current : null;
            Assert.IsNotNull(matchWithSpaces, "Did not find a number in '" + zipFollowing + "'; expected " + number);
            Assert.AreEqual(expectedResult, matchWithSpaces.Number);
            Assert.AreEqual(number, matchWithSpaces.RawString);
        }

        [TestMethod]
        public void testIsLatinLetter()
        {
            Assert.IsTrue(PhoneNumberMatchEnumerator.isLatinLetter('c'));
            Assert.IsTrue(PhoneNumberMatchEnumerator.isLatinLetter('C'));
            Assert.IsTrue(PhoneNumberMatchEnumerator.isLatinLetter('\u00C9'));
            Assert.IsTrue(PhoneNumberMatchEnumerator.isLatinLetter('\u0301')); // Combining acute accent
            // Punctuation, digits and white-space are not considered "latin letters".
            Assert.IsFalse(PhoneNumberMatchEnumerator.isLatinLetter(':'));
            Assert.IsFalse(PhoneNumberMatchEnumerator.isLatinLetter('5'));
            Assert.IsFalse(PhoneNumberMatchEnumerator.isLatinLetter('-'));
            Assert.IsFalse(PhoneNumberMatchEnumerator.isLatinLetter('.'));
            Assert.IsFalse(PhoneNumberMatchEnumerator.isLatinLetter(' '));
            Assert.IsFalse(PhoneNumberMatchEnumerator.isLatinLetter('\u6211')); // Chinese character
            Assert.IsFalse(PhoneNumberMatchEnumerator.isLatinLetter('\u306E')); // Hiragana letter no
        }

        [TestMethod]
        public void testMatchesWithSurroundingLatinChars()
        {
            List<NumberContext> possibleOnlyContexts = new List<NumberContext>();
            possibleOnlyContexts.Add(new NumberContext("abc", "def"));
            possibleOnlyContexts.Add(new NumberContext("abc", ""));
            possibleOnlyContexts.Add(new NumberContext("", "def"));
            // Latin capital letter e with an acute accent.
            possibleOnlyContexts.Add(new NumberContext("\u00C9", ""));
            // e with an acute accent decomposed (with combining mark).
            possibleOnlyContexts.Add(new NumberContext("e\u0301", ""));

            // Numbers should not be considered valid, if they are surrounded by Latin characters, but
            // should be considered possible.
            findMatchesInContexts(possibleOnlyContexts, false, true);
        }

        [TestMethod]
        public void testMoneyNotSeenAsPhoneNumber()
        {
            List<NumberContext> possibleOnlyContexts = new List<NumberContext>();
            possibleOnlyContexts.Add(new NumberContext("$", ""));
            possibleOnlyContexts.Add(new NumberContext("", "$"));
            possibleOnlyContexts.Add(new NumberContext("\u00A3", "")); // Pound sign
            possibleOnlyContexts.Add(new NumberContext("\u00A5", "")); // Yen sign
            findMatchesInContexts(possibleOnlyContexts, false, true);
        }

        [TestMethod]
        public void testPercentageNotSeenAsPhoneNumber()
        {
            List<NumberContext> possibleOnlyContexts = new List<NumberContext>();
            possibleOnlyContexts.Add(new NumberContext("", "%"));
            // Numbers followed by % should be dropped.
            findMatchesInContexts(possibleOnlyContexts, false, true);
        }

        [TestMethod]
        public void testPhoneNumberWithLeadingOrTrailingMoneyMatches()
        {
            // Because of the space after the 20 (or before the 100) these dollar amounts should not stop
            // the actual number from being found.
            List<NumberContext> contexts = new List<NumberContext>();
            contexts.Add(new NumberContext("$20 ", ""));
            contexts.Add(new NumberContext("", " 100$"));
            findMatchesInContexts(contexts, true, true);
        }

        [TestMethod]
        public void testMatchesWithSurroundingLatinCharsAndLeadingPunctuation()
        {
            // Contexts with trailing characters. Leading characters are okay here since the numbers we will
            // insert start with punctuation, but trailing characters are still not allowed.
            List<NumberContext> possibleOnlyContexts = new List<NumberContext>();
            possibleOnlyContexts.Add(new NumberContext("abc", "def"));
            possibleOnlyContexts.Add(new NumberContext("", "def"));
            possibleOnlyContexts.Add(new NumberContext("", "\u00C9"));

            // Numbers should not be considered valid, if they have trailing Latin characters, but should be
            // considered possible.
            String numberWithPlus = "+14156667777";
            String numberWithBrackets = "(415)6667777";
            findMatchesInContexts(possibleOnlyContexts, false, true, RegionCode.US, numberWithPlus);
            findMatchesInContexts(possibleOnlyContexts, false, true, RegionCode.US, numberWithBrackets);

            List<NumberContext> validContexts = new List<NumberContext>();
            validContexts.Add(new NumberContext("abc", ""));
            validContexts.Add(new NumberContext("\u00C9", ""));
            validContexts.Add(new NumberContext("\u00C9", ".")); // Trailing punctuation.
            validContexts.Add(new NumberContext("\u00C9", " def")); // Trailing white-space.

            // Numbers should be considered valid, since they start with punctuation.
            findMatchesInContexts(validContexts, true, true, RegionCode.US, numberWithPlus);
            findMatchesInContexts(validContexts, true, true, RegionCode.US, numberWithBrackets);
        }

        [TestMethod]
        public void testMatchesWithSurroundingChineseChars()
        {
            List<NumberContext> validContexts = new List<NumberContext>();
            validContexts.Add(new NumberContext("\u6211\u7684\u7535\u8BDD\u53F7\u7801\u662F", ""));
            validContexts.Add(new NumberContext("", "\u662F\u6211\u7684\u7535\u8BDD\u53F7\u7801"));
            validContexts.Add(new NumberContext("\u8BF7\u62E8\u6253", "\u6211\u5728\u660E\u5929"));

            // Numbers should be considered valid, since they are surrounded by Chinese.
            findMatchesInContexts(validContexts, true, true);
        }

        [TestMethod]
        public void testMatchesWithSurroundingPunctuation()
        {
            List<NumberContext> validContexts = new List<NumberContext>();
            validContexts.Add(new NumberContext("My number-", "")); // At end of text.
            validContexts.Add(new NumberContext("", ".Nice day.")); // At start of text.
            validContexts.Add(new NumberContext("Tel:", ".")); // Punctuation surrounds number.
            validContexts.Add(new NumberContext("Tel: ", " on Saturdays.")); // White-space is also fine.

            // Numbers should be considered valid, since they are surrounded by punctuation.
            findMatchesInContexts(validContexts, true, true);
        }

        [TestMethod]
        public void testMatchesMultiplePhoneNumbersSeparatedByPhoneNumberPunctuation()
        {
            String text = "Call 650-253-4561 -- 455-234-3451";
            String region = RegionCode.US;

            PhoneNumber number1 = new PhoneNumber();
            number1.SetCountryCode(phoneUtil.getCountryCodeForRegion(region));
            number1.SetNationalNumber(6502534561L);
            PhoneNumberMatch match1 = new PhoneNumberMatch(5, "650-253-4561", number1);

            PhoneNumber number2 = new PhoneNumber();
            number2.SetCountryCode(phoneUtil.getCountryCodeForRegion(region));
            number2.SetNationalNumber(4552343451L);
            PhoneNumberMatch match2 = new PhoneNumberMatch(21, "455-234-3451", number2);

            IEnumerator<PhoneNumberMatch> matches = phoneUtil.findNumbers(text, region).GetEnumerator();
            Assert.IsTrue(matches.MoveNext());
            Assert.AreEqual(match1, matches.Current);
            Assert.IsTrue(matches.MoveNext());
            Assert.AreEqual(match2, matches.Current);
        }

        [TestMethod]
        public void testDoesNotMatchMultiplePhoneNumbersSeparatedWithNoWhiteSpace()
        {
            // No white-space found between numbers - neither is found.
            String text = "Call 650-253-4561--455-234-3451";
            String region = RegionCode.US;

            Assert.IsTrue(hasNoMatches(phoneUtil.findNumbers(text, region)));
        }

        /**
        * Strings with number-like things that shouldn't be found under any level.
        */
        private static readonly NumberTest[] IMPOSSIBLE_CASES = new[]
                                                                    {
                                                                        new NumberTest("12345", RegionCode.US),
                                                                        new NumberTest("23456789", RegionCode.US),
                                                                        new NumberTest("234567890112", RegionCode.US),
                                                                        new NumberTest("650+253+1234", RegionCode.US),
                                                                        new NumberTest("3/10/1984", RegionCode.CA),
                                                                        new NumberTest("03/27/2011", RegionCode.US),
                                                                        new NumberTest("31/8/2011", RegionCode.US),
                                                                        new NumberTest("1/12/2011", RegionCode.US),
                                                                        new NumberTest("10/12/82", RegionCode.DE),
                                                                        new NumberTest("650x2531234", RegionCode.US),
                                                                        new NumberTest("2012-01-02 08:00", RegionCode.US),
                                                                        new NumberTest("2012/01/02 08:00", RegionCode.US),
                                                                        new NumberTest("20120102 08:00", RegionCode.US),
                                                                    };

        /**
        * Strings with number-like things that should only be found under "possible".
        */
        private static readonly NumberTest[] POSSIBLE_ONLY_CASES = new[]
                                                                       {
                                                                           // US numbers cannot start with 7 in the test metadata to be valid.
                                                                           new NumberTest("7121115678", RegionCode.US),
                                                                           // 'X' should not be found in numbers at leniencies stricter than POSSIBLE, unless it represents
                                                                           // a carrier code or extension.
                                                                           new NumberTest("1650 x 253 - 1234", RegionCode.US),
                                                                           new NumberTest("650 x 253 - 1234", RegionCode.US),
                                                                           new NumberTest("6502531x234", RegionCode.US),
                                                                           new NumberTest("(20) 3346 1234", RegionCode.GB), // Non-optional NP omitted
                                                                       };

        /**
        * Strings with number-like things that should only be found up to and including the "valid"
        * leniency level.
        */
        private static readonly NumberTest[] VALID_CASES =
            {
                new NumberTest("65 02 53 00 00", RegionCode.US),
                new NumberTest("6502 538365", RegionCode.US),
                new NumberTest("650//253-1234", RegionCode.US), // 2 slashes are illegal at higher levels
                new NumberTest("650/253/1234", RegionCode.US),
                new NumberTest("9002309. 158", RegionCode.US),
                new NumberTest("12 7/8 - 14 12/34 - 5", RegionCode.US),
                new NumberTest("12.1 - 23.71 - 23.45", RegionCode.US),
                new NumberTest("800 234 1 111x1111", RegionCode.US),
                new NumberTest("1979-2011 100", RegionCode.US),
                new NumberTest("+494949-4-94", RegionCode.DE), // National number in wrong format
                new NumberTest("\uFF14\uFF11\uFF15\uFF16\uFF16\uFF16\uFF16-\uFF17\uFF17\uFF17", RegionCode.US),
                new NumberTest("2012-0102 08", RegionCode.US), // Very strange formatting.
                new NumberTest("2012-01-02 08", RegionCode.US),
                // Breakdown assistance number with unexpected formatting.
                new NumberTest("1800-1-0-10 22", RegionCode.AU),
                new NumberTest("030-3-2 23 12 34", RegionCode.DE),
                new NumberTest("03 0 -3 2 23 12 34", RegionCode.DE),
                new NumberTest("(0)3 0 -3 2 23 12 34", RegionCode.DE),
                new NumberTest("0 3 0 -3 2 23 12 34", RegionCode.DE),
            };

        /**
        * Strings with number-like things that should only be found up to and including the
        * "strict_grouping" leniency level.
        */
        private static readonly NumberTest[] STRICT_GROUPING_CASES =
            {
                new NumberTest("(415) 6667777", RegionCode.US),
                new NumberTest("415-6667777", RegionCode.US),
                // Should be found by strict grouping but not exact grouping, as the last two groups are
                // formatted together as a block.
                new NumberTest("0800-2491234", RegionCode.DE),
                // Doesn't match any formatting in the test file, but almost matches an alternate format (the
                // last two groups have been squashed together here).
                new NumberTest("0900-1 123123", RegionCode.DE),
                new NumberTest("(0)900-1 123123", RegionCode.DE),
                new NumberTest("0 900-1 123123", RegionCode.DE),
            };

        /**
        * Strings with number-like things that should be found at all levels.
        */
        private static readonly NumberTest[] EXACT_GROUPING_CASES = new[]
                                                                        {
                                                                            new NumberTest("\uFF14\uFF11\uFF15\uFF16\uFF16\uFF16\uFF17\uFF17\uFF17\uFF17", RegionCode.US),
                                                                            new NumberTest("\uFF14\uFF11\uFF15-\uFF16\uFF16\uFF16-\uFF17\uFF17\uFF17\uFF17", RegionCode.US),
                                                                            new NumberTest("4156667777", RegionCode.US),
                                                                            new NumberTest("4156667777 x 123", RegionCode.US),
                                                                            new NumberTest("415-666-7777", RegionCode.US),
                                                                            new NumberTest("415/666-7777", RegionCode.US),
                                                                            new NumberTest("415-666-7777 ext. 503", RegionCode.US),
                                                                            new NumberTest("1 415 666 7777 x 123", RegionCode.US),
                                                                            new NumberTest("+1 415-666-7777", RegionCode.US),
                                                                            new NumberTest("+494949 49", RegionCode.DE),
                                                                            new NumberTest("+49-49-34", RegionCode.DE),
                                                                            new NumberTest("+49-4931-49", RegionCode.DE),
                                                                            new NumberTest("04931-49", RegionCode.DE), // With National Prefix
                                                                            new NumberTest("+49-494949", RegionCode.DE), // One group with country code
                                                                            new NumberTest("+49-494949 ext. 49", RegionCode.DE),
                                                                            new NumberTest("+49494949 ext. 49", RegionCode.DE),
                                                                            new NumberTest("0494949", RegionCode.DE),
                                                                            new NumberTest("0494949 ext. 49", RegionCode.DE),
                                                                            new NumberTest("01 (33) 3461 2234", RegionCode.MX), // Optional NP present
                                                                            new NumberTest("(33) 3461 2234", RegionCode.MX), // Optional NP omitted
                                                                            new NumberTest("1800-10-10 22", RegionCode.AU), // Breakdown assistance number.
                                                                            // Doesn't match any formatting in the test file, but matches an alternate format exactly.
                                                                            new NumberTest("0900-1 123 123", RegionCode.DE),
                                                                            new NumberTest("(0)900-1 123 123", RegionCode.DE),
                                                                            new NumberTest("0 900-1 123 123", RegionCode.DE),
                                                                        };

        [TestMethod]
        public void testMatchesWithPossibleLeniency()
        {
            IList<NumberTest> testCases = new List<NumberTest>();
            testCases.AddAll(STRICT_GROUPING_CASES);
            testCases.AddAll(EXACT_GROUPING_CASES);
            testCases.AddAll(VALID_CASES);
            testCases.AddAll(POSSIBLE_ONLY_CASES);
            doTestNumberMatchesForLeniency(testCases, Leniency.POSSIBLE);
        }

        [TestMethod]
        public void testNonMatchesWithPossibleLeniency()
        {
            IList<NumberTest> testCases = new List<NumberTest>();
            testCases.AddAll(IMPOSSIBLE_CASES);
            doTestNumberNonMatchesForLeniency(testCases, Leniency.POSSIBLE);
        }

        [TestMethod]
        public void testMatchesWithValidLeniency()
        {
            IList<NumberTest> testCases = new List<NumberTest>();
            testCases.AddAll(STRICT_GROUPING_CASES);
            testCases.AddAll(EXACT_GROUPING_CASES);
            testCases.AddAll(VALID_CASES);
            doTestNumberMatchesForLeniency(testCases, Leniency.VALID);
        }

        [TestMethod]
        public void testNonMatchesWithValidLeniency()
        {
            IList<NumberTest> testCases = new List<NumberTest>();
            testCases.AddAll(IMPOSSIBLE_CASES);
            testCases.AddAll(POSSIBLE_ONLY_CASES);
            doTestNumberNonMatchesForLeniency(testCases, Leniency.VALID);
        }

        [TestMethod]
        public void testMatchesWithStrictGroupingLeniency()
        {
            IList<NumberTest> testCases = new List<NumberTest>();
            testCases.AddAll(STRICT_GROUPING_CASES);
            testCases.AddAll(EXACT_GROUPING_CASES);
            doTestNumberMatchesForLeniency(testCases, Leniency.STRICT_GROUPING);
        }

        [TestMethod]
        public void testNonMatchesWithStrictGroupLeniency()
        {
            IList<NumberTest> testCases = new List<NumberTest>();
            testCases.AddAll(IMPOSSIBLE_CASES);
            testCases.AddAll(POSSIBLE_ONLY_CASES);
            testCases.AddAll(VALID_CASES);
            doTestNumberNonMatchesForLeniency(testCases, Leniency.STRICT_GROUPING);
        }

        [TestMethod]
        public void testMatchesWithExactGroupingLeniency()
        {
            IList<NumberTest> testCases = new List<NumberTest>();
            testCases.AddAll(EXACT_GROUPING_CASES);
            doTestNumberMatchesForLeniency(testCases, Leniency.EXACT_GROUPING);
        }

        [TestMethod]
        public void testNonMatchesExactGroupLeniency()
        {
            IList<NumberTest> testCases = new List<NumberTest>();
            testCases.AddAll(IMPOSSIBLE_CASES);
            testCases.AddAll(POSSIBLE_ONLY_CASES);
            testCases.AddAll(VALID_CASES);
            testCases.AddAll(STRICT_GROUPING_CASES);
            doTestNumberNonMatchesForLeniency(testCases, Leniency.EXACT_GROUPING);
        }

        private void doTestNumberMatchesForLeniency(IList<NumberTest> testCases, Leniency leniency)
        {
            int noMatchFoundCount = 0;
            int wrongMatchFoundCount = 0;
            foreach (NumberTest test in testCases)
            {
                IEnumerator<PhoneNumberMatch> iterator = findNumbersForLeniency(test.rawString, test.region, leniency);
                PhoneNumberMatch match = iterator.MoveNext() ? iterator.Current : null;
                if (match == null)
                {
                    noMatchFoundCount++;
                    Debug.WriteLine("No match found in " + test + " for leniency: " + leniency);
                }
                else
                {
                    if (!test.rawString.Equals(match.RawString))
                    {
                        wrongMatchFoundCount++;
                        Debug.WriteLine("Found wrong match in test " + test + ". Found " + match.RawString);
                    }
                }
            }
            Assert.AreEqual(0, noMatchFoundCount);
            Assert.AreEqual(0, wrongMatchFoundCount);
        }

        private void doTestNumberNonMatchesForLeniency(IList<NumberTest> testCases, Leniency leniency)
        {
            int matchFoundCount = 0;
            foreach (NumberTest test in testCases)
            {
                IEnumerator<PhoneNumberMatch> iterator = findNumbersForLeniency(test.rawString, test.region, leniency);
                PhoneNumberMatch match = iterator.MoveNext() ? iterator.Current : null;
                if (match != null)
                {
                    matchFoundCount++;
                    Debug.WriteLine("Match found in " + test + " for leniency: " + leniency);
                }
            }
            Assert.AreEqual(0, matchFoundCount);
        }

        /**
   * Helper method which tests the contexts provided and ensures that:
   * -- if isValid is true, they all find a test number inserted in the middle when leniency of
   *  matching is set to VALID; else no test number should be extracted at that leniency level
   * -- if isPossible is true, they all find a test number inserted in the middle when leniency of
   *  matching is set to POSSIBLE; else no test number should be extracted at that leniency level
   */

        private void findMatchesInContexts(IList<NumberContext> contexts, bool isValid,
                                           bool isPossible, String region, String number)
        {
            if (isValid)
            {
                doTestInContext(number, region, contexts, Leniency.VALID);
            }
            else
            {
                foreach (NumberContext context in contexts)
                {
                    String text = context.leadingText + number + context.trailingText;
                    Assert.IsTrue(hasNoMatches(phoneUtil.findNumbers(text, region)), "Should not have found a number in " + text);
                }
            }
            if (isPossible)
            {
                doTestInContext(number, region, contexts, Leniency.POSSIBLE);
            }
            else
            {
                foreach (NumberContext context in contexts)
                {
                    String text = context.leadingText + number + context.trailingText;
                    Assert.IsTrue(hasNoMatches(phoneUtil.findNumbers(text, region, Leniency.POSSIBLE, long.MaxValue)),
                                  "Should not have found a number in " + text);
                }
            }
        }

        /**
   * Variant of findMatchesInContexts that uses a default number and region.
   */

        private void findMatchesInContexts(IList<NumberContext> contexts, bool isValid,
                                           bool isPossible)
        {
            String region = RegionCode.US;
            String number = "415-666-7777";

            findMatchesInContexts(contexts, isValid, isPossible, region, number);
        }

        [TestMethod]
        public void testNonMatchingBracketsAreInvalid()
        {
            // The digits up to the ", " form a valid US number, but it shouldn't be matched as one since
            // there was a non-matching bracket present.
            Assert.IsTrue(hasNoMatches(phoneUtil.findNumbers(
                "80.585 [79.964, 81.191]", RegionCode.US)));

            // The trailing "]" is thrown away before parsing, so the resultant number, while a valid US
            // number, does not have matching brackets.
            Assert.IsTrue(hasNoMatches(phoneUtil.findNumbers(
                "80.585 [79.964]", RegionCode.US)));

            Assert.IsTrue(hasNoMatches(phoneUtil.findNumbers(
                "80.585 ((79.964)", RegionCode.US)));

            // This case has too many sets of brackets to be valid.
            Assert.IsTrue(hasNoMatches(phoneUtil.findNumbers(
                "(80).(585) (79).(9)64", RegionCode.US)));
        }

        [TestMethod]
        public void testNoMatchIfRegionIsNull()
        {
            // Fail on non-international prefix if region code is null.
            Assert.IsTrue(hasNoMatches(phoneUtil.findNumbers(
                "Random text body - number is 0331 6005, see you there", null)));
        }

        [TestMethod]
        public void testNoMatchInEmptyString()
        {
            Assert.IsTrue(hasNoMatches(phoneUtil.findNumbers("", RegionCode.US)));
            Assert.IsTrue(hasNoMatches(phoneUtil.findNumbers("  ", RegionCode.US)));
        }

        [TestMethod]
        public void testNoMatchIfNoNumber()
        {
            Assert.IsTrue(hasNoMatches(phoneUtil.findNumbers(
                "Random text body - number is foobar, see you there", RegionCode.US)));
        }

        [TestMethod]
        public void testSequences()
        {
            // Test multiple occurrences.
            String text = "Call 033316005  or 032316005!";
            String region = RegionCode.NZ;

            PhoneNumber number1 = new PhoneNumber();
            number1.SetCountryCode(phoneUtil.getCountryCodeForRegion(region));
            number1.SetNationalNumber(33316005);
            PhoneNumberMatch match1 = new PhoneNumberMatch(5, "033316005", number1);

            PhoneNumber number2 = new PhoneNumber();
            number2.SetCountryCode(phoneUtil.getCountryCodeForRegion(region));
            number2.SetNationalNumber(32316005);
            PhoneNumberMatch match2 = new PhoneNumberMatch(19, "032316005", number2);

            IEnumerator<PhoneNumberMatch> matches = phoneUtil.findNumbers(text, region, Leniency.POSSIBLE, long.MaxValue).GetEnumerator();

            Assert.IsTrue(matches.MoveNext());
            Assert.AreEqual(match1, matches.Current);
            Assert.IsTrue(matches.MoveNext());
            Assert.AreEqual(match2, matches.Current);
        }

        [TestMethod]
        public void testNullInput()
        {
            Assert.IsTrue(hasNoMatches(phoneUtil.findNumbers(null, RegionCode.US)));
            Assert.IsTrue(hasNoMatches(phoneUtil.findNumbers(null, null)));
        }

        [TestMethod]
        public void testMaxMatches()
        {
            // Set up text with 100 valid phone numbers.
            StringBuilder numbers = new StringBuilder();
            for (int i = 0; i < 100; i++)
            {
                numbers.Append("My info: 415-666-7777,");
            }

            // Matches all 100. Max only applies to failed cases.
            IList<PhoneNumber> expected = new List<PhoneNumber>(100);
            PhoneNumber number = phoneUtil.parse("+14156667777", null);
            for (int i = 0; i < 100; i++)
            {
                expected.Add(number);
            }

            IEnumerable<PhoneNumberMatch> iterable = phoneUtil.findNumbers(numbers.ToString(), RegionCode.US, Leniency.VALID, 10);
            IList<PhoneNumber> actual = new List<PhoneNumber>(100);
            foreach (PhoneNumberMatch match in iterable)
            {
                actual.Add(match.Number);
            }
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [TestMethod]
        public void testMaxMatchesInvalid()
        {
            // Set up text with 10 invalid phone numbers followed by 100 valid.
            StringBuilder numbers = new StringBuilder();
            for (int i = 0; i < 10; i++)
            {
                numbers.Append("My address 949-8945-0");
            }
            for (int i = 0; i < 100; i++)
            {
                numbers.Append("My info: 415-666-7777,");
            }

            IEnumerable<PhoneNumberMatch> iterable = phoneUtil.findNumbers(numbers.ToString(), RegionCode.US, Leniency.VALID, 10);
            Assert.IsFalse(iterable.GetEnumerator().MoveNext());
        }

        [TestMethod]
        public void testMaxMatchesMixed()
        {
            // Set up text with 100 valid numbers inside an invalid number.
            StringBuilder numbers = new StringBuilder();
            for (int i = 0; i < 100; i++)
            {
                numbers.Append("My info: 415-666-7777 123 fake street");
            }

            // Only matches the first 10 despite there being 100 numbers due to max matches.
            IList<PhoneNumber> expected = new List<PhoneNumber>(100);
            PhoneNumber number = phoneUtil.parse("+14156667777", null);
            for (int i = 0; i < 10; i++)
            {
                expected.Add(number);
            }

            IEnumerable<PhoneNumberMatch> iterable = phoneUtil.findNumbers(numbers.ToString(), RegionCode.US, Leniency.VALID, 10);
            IList<PhoneNumber> actual = new List<PhoneNumber>(100);
            foreach (PhoneNumberMatch match in iterable)
            {
                actual.Add(match.Number);
            }
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [TestMethod]
        public void testNonPlusPrefixedNumbersNotFoundForInvalidRegion()
        {
            // Does not start with a "+", we won't match it.
            IEnumerable<PhoneNumberMatch> iterable = phoneUtil.findNumbers("1 456 764 156", RegionCode.ZZ);
            IEnumerator<PhoneNumberMatch> iterator = iterable.GetEnumerator();

            Assert.IsFalse(iterator.MoveNext());
            try
            {
                var phoneNumberMatch = iterator.Current;
                Assert.Fail("Violation of the Iterator contract.");
            }
            catch (InvalidOperationException)
            {
                /* Success */
            }
            Assert.IsFalse(iterator.MoveNext());
        }

        [TestMethod]
        public void testEmptyIteration()
        {
            IEnumerable<PhoneNumberMatch> iterable = phoneUtil.findNumbers("", RegionCode.ZZ);
            IEnumerator<PhoneNumberMatch> iterator = iterable.GetEnumerator();

            Assert.IsFalse(iterator.MoveNext());
            Assert.IsFalse(iterator.MoveNext());

            try
            {
                PhoneNumberMatch phoneNumberMatch = iterator.Current;
                Assert.Fail("Violation of the Iterator contract.");
            }
            catch (InvalidOperationException)
            {
                /* Success */
            }

            Assert.IsFalse(iterator.MoveNext());
        }

        [TestMethod]
        public void testSingleIteration()
        {
            IEnumerable<PhoneNumberMatch> iterable = phoneUtil.findNumbers("+14156667777", RegionCode.ZZ);

            // With hasNext() -> next().
            IEnumerator<PhoneNumberMatch> iterator = iterable.GetEnumerator();

            Assert.IsTrue(iterator.MoveNext());
            Assert.IsNotNull(iterator.Current);
            Assert.IsFalse(iterator.MoveNext());

            try
            {
                var phoneNumberMatch = iterator.Current;
                Assert.Fail("Violation of the Iterator contract.");
            }
            catch (InvalidOperationException)
            {
                /* Success */
            }
            
            Assert.IsFalse(iterator.MoveNext());
        }

        [TestMethod]
        public void testDoubleIteration()
        {
            IEnumerable<PhoneNumberMatch> iterable = phoneUtil.findNumbers("+14156667777 foobar +14156667777 ", RegionCode.ZZ);

            IEnumerator<PhoneNumberMatch> iterator = iterable.GetEnumerator();
            Assert.IsTrue(iterator.MoveNext());
            Assert.IsNotNull(iterator.Current);
            Assert.IsTrue(iterator.MoveNext());
            Assert.IsNotNull(iterator.Current);
            Assert.IsFalse(iterator.MoveNext());

            try
            {
                var phoneNumberMatch = iterator.Current;
                Assert.Fail("Violation of the Iterator contract.");
            }
            catch (InvalidOperationException)
            {
                /* Success */
            }

            Assert.IsFalse(iterator.MoveNext());
        }

        /**
        * Asserts that another number can be found in {@code text} starting at {@code index}, and that
        * its corresponding range is {@code [start, end)}.
        */
        private void assertEqualRange(string text, int index, int start, int end)
        {
            string sub = text.Substring(index, text.Length - index);
            IEnumerator<PhoneNumberMatch> matches = phoneUtil.findNumbers(sub, RegionCode.NZ, Leniency.POSSIBLE, long.MaxValue).GetEnumerator();
            Assert.IsTrue(matches.MoveNext());
            PhoneNumberMatch match = matches.Current;
            Assert.AreEqual(start - index, match.Start);
            Assert.AreEqual(end - index, match.End);
            Assert.AreEqual(sub.Substring(match.Start, match.End - match.Start), match.RawString);
        }

        /**
        * Tests numbers found by {@link PhoneNumberUtil#findNumbers(CharSequence, String)} in various
        * textual contexts.
        *
        * @param number the number to test and the corresponding region code to use
        */
        private void doTestFindInContext(String number, String defaultCountry)
        {
            findPossibleInContext(number, defaultCountry);

            PhoneNumber parsed = phoneUtil.parse(number, defaultCountry);
            if (phoneUtil.isValidNumber(parsed))
            {
                findValidInContext(number, defaultCountry);
            }
        }

        /**
        * Tests valid numbers in contexts that should pass for {@link Leniency#POSSIBLE}.
        */
        private void findPossibleInContext(String number, String defaultCountry)
        {
            List<NumberContext> contextPairs = new List<NumberContext>();
            contextPairs.Add(new NumberContext("", "")); // no context
            contextPairs.Add(new NumberContext("   ", "\t")); // whitespace only
            contextPairs.Add(new NumberContext("Hello ", "")); // no context at end
            contextPairs.Add(new NumberContext("", " to call me!")); // no context at start
            contextPairs.Add(new NumberContext("Hi there, call ", " to reach me!")); // no context at start
            contextPairs.Add(new NumberContext("Hi there, call ", ", or don't")); // with commas
            // Three examples without whitespace around the number.
            contextPairs.Add(new NumberContext("Hi call", ""));
            contextPairs.Add(new NumberContext("", "forme"));
            contextPairs.Add(new NumberContext("Hi call", "forme"));
            // With other small numbers.
            contextPairs.Add(new NumberContext("It's cheap! Call ", " before 6:30"));
            // With a second number later.
            contextPairs.Add(new NumberContext("Call ", " or +1800-123-4567!"));
            contextPairs.Add(new NumberContext("Call me on June 2 at", "")); // with a Month-Day date
            // With publication pages.
            contextPairs.Add(new NumberContext(
                                 "As quoted by Alfonso 12-15 (2009), you may call me at ", ""));
            contextPairs.Add(new NumberContext(
                                 "As quoted by Alfonso et al. 12-15 (2009), you may call me at ", ""));
            // With dates, written in the American style.
            contextPairs.Add(new NumberContext(
                                 "As I said on 03/10/2011, you may call me at ", ""));
            // With trailing numbers after a comma. The 45 should not be considered an extension.
            contextPairs.Add(new NumberContext("", ", 45 days a year"));
            // With a postfix stripped off as it looks like the start of another number.
            contextPairs.Add(new NumberContext("Call ", "/x12 more"));

            doTestInContext(number, defaultCountry, contextPairs, Leniency.POSSIBLE);
        }

        /**
        * Tests valid numbers in contexts that fail for {@link Leniency#POSSIBLE} but are valid for
        * {@link Leniency#VALID}.
        */
        private void findValidInContext(String number, String defaultCountry)
        {
            List<NumberContext> contextPairs = new List<NumberContext>();
            // With other small numbers.
            contextPairs.Add(new NumberContext("It's only 9.99! Call ", " to buy"));
            // With a number Day.Month.Year date.
            contextPairs.Add(new NumberContext("Call me on 21.6.1984 at ", ""));
            // With a number Month/Day date.
            contextPairs.Add(new NumberContext("Call me on 06/21 at ", ""));
            // With a number Day.Month date.
            contextPairs.Add(new NumberContext("Call me on 21.6. at ", ""));
            // With a number Month/Day/Year date.
            contextPairs.Add(new NumberContext("Call me on 06/21/84 at ", ""));

            doTestInContext(number, defaultCountry, contextPairs, Leniency.VALID);
        }

        private void doTestInContext(String number, String defaultCountry, IEnumerable<NumberContext> contextPairs, Leniency leniency)
        {
            foreach (NumberContext context in contextPairs)
            {
                String prefix = context.leadingText;
                String text = prefix + number + context.trailingText;

                int start = prefix.Length;
                int end = start + number.Length;
                IEnumerator<PhoneNumberMatch> iterator = phoneUtil.findNumbers(text, defaultCountry, leniency, long.MaxValue).GetEnumerator();

                PhoneNumberMatch match = iterator.MoveNext() ? iterator.Current : null;
                Assert.IsNotNull(match, "Did not find a number in '" + text + "'; expected '" + number + "'");

                string extracted = text.Substring(match.Start, match.End - match.Start);
                Assert.IsTrue(start == match.Start && end == match.End, "Unexpected phone region in '" + text + "'; extracted '" + extracted + "'");
                Assert.IsTrue(number.Equals(extracted));
                Assert.IsTrue(match.RawString.Equals(extracted));

                ensureTermination(text, defaultCountry, leniency);
            }
        }

        /**
        * Exhaustively searches for phone numbers from each index within {@code text} to test that
        * finding matches always terminates.
        */
        private void ensureTermination(String text, String defaultCountry, Leniency leniency)
        {
            for (int index = 0; index <= text.Length; index++)
            {
                String sub = text.Substring(index);
                var matches = new StringBuilder();
                // Iterates over all matches.
                foreach (PhoneNumberMatch match in phoneUtil.findNumbers(sub, defaultCountry, leniency, long.MaxValue))
                {
                    matches.Append(", ").Append(match);
                }
            }
        }

        private IEnumerator<PhoneNumberMatch> findNumbersForLeniency(String text, String defaultCountry, Leniency leniency)
        {
            return phoneUtil.findNumbers(text, defaultCountry, leniency, long.MaxValue).GetEnumerator();
        }

        private bool hasNoMatches(IEnumerable<PhoneNumberMatch> iterable)
        {
            return !iterable.GetEnumerator().MoveNext();
        }

        /**
        * Small class that holds the context of the number we are testing against. The test will
        * insert the phone number to be found between leadingText and trailingText.
        */
        private class NumberContext
        {
            internal readonly String leadingText;
            internal readonly String trailingText;

            internal NumberContext(String leadingText, String trailingText)
            {
                this.leadingText = leadingText;
                this.trailingText = trailingText;
            }
        }

        /**
        * Small class that holds the number we want to test and the region for which it should be valid.
        */
        private class NumberTest
        {
            internal readonly String rawString;
            internal readonly String region;

            internal NumberTest(String rawString, String regionCode)
            {
                this.rawString = rawString;
                this.region = regionCode;
            }

            public override string ToString()
            {
                return rawString + " (" + region + ")";
            }
        }
    }
}