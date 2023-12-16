﻿using System;
using JahanJooy.Common.Util.PhoneNumbers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JahanJooy.Common.Util.Test.PhoneNumbers
{
    [TestClass]
    public class AsYouTypeFormatterTest : TestMetaDataBase
    {
        [TestMethod]
        public void testInvalidRegion()
        {
            AsYouTypeFormatter formatter = phoneUtil.getAsYouTypeFormatter(RegionCode.ZZ);
            Assert.AreEqual("+", formatter.inputDigit('+'));
            Assert.AreEqual("+4", formatter.inputDigit('4'));
            Assert.AreEqual("+48 ", formatter.inputDigit('8'));
            Assert.AreEqual("+48 8", formatter.inputDigit('8'));
            Assert.AreEqual("+48 88", formatter.inputDigit('8'));
            Assert.AreEqual("+48 88 1", formatter.inputDigit('1'));
            Assert.AreEqual("+48 88 12", formatter.inputDigit('2'));
            Assert.AreEqual("+48 88 123", formatter.inputDigit('3'));
            Assert.AreEqual("+48 88 123 1", formatter.inputDigit('1'));
            Assert.AreEqual("+48 88 123 12", formatter.inputDigit('2'));

            formatter.clear();
            Assert.AreEqual("6", formatter.inputDigit('6'));
            Assert.AreEqual("65", formatter.inputDigit('5'));
            Assert.AreEqual("650", formatter.inputDigit('0'));
            Assert.AreEqual("6502", formatter.inputDigit('2'));
            Assert.AreEqual("65025", formatter.inputDigit('5'));
            Assert.AreEqual("650253", formatter.inputDigit('3'));
        }

        [TestMethod]
        public void testInvalidPlusSign()
        {
            AsYouTypeFormatter formatter = phoneUtil.getAsYouTypeFormatter(RegionCode.ZZ);
            Assert.AreEqual("+", formatter.inputDigit('+'));
            Assert.AreEqual("+4", formatter.inputDigit('4'));
            Assert.AreEqual("+48 ", formatter.inputDigit('8'));
            Assert.AreEqual("+48 8", formatter.inputDigit('8'));
            Assert.AreEqual("+48 88", formatter.inputDigit('8'));
            Assert.AreEqual("+48 88 1", formatter.inputDigit('1'));
            Assert.AreEqual("+48 88 12", formatter.inputDigit('2'));
            Assert.AreEqual("+48 88 123", formatter.inputDigit('3'));
            Assert.AreEqual("+48 88 123 1", formatter.inputDigit('1'));
            // A plus sign can only appear at the beginning of the number; otherwise, no formatting is
            // applied.
            Assert.AreEqual("+48881231+", formatter.inputDigit('+'));
            Assert.AreEqual("+48881231+2", formatter.inputDigit('2'));
        }

        [TestMethod]
        public void testTooLongNumberMatchingMultipleLeadingDigits()
        {
            // See http://code.google.com/p/libphonenumber/issues/detail?id=36
            // The bug occurred last time for countries which have two formatting rules with exactly the
            // same leading digits pattern but differ in length.
            AsYouTypeFormatter formatter = phoneUtil.getAsYouTypeFormatter(RegionCode.ZZ);
            Assert.AreEqual("+", formatter.inputDigit('+'));
            Assert.AreEqual("+8", formatter.inputDigit('8'));
            Assert.AreEqual("+81 ", formatter.inputDigit('1'));
            Assert.AreEqual("+81 9", formatter.inputDigit('9'));
            Assert.AreEqual("+81 90", formatter.inputDigit('0'));
            Assert.AreEqual("+81 90 1", formatter.inputDigit('1'));
            Assert.AreEqual("+81 90 12", formatter.inputDigit('2'));
            Assert.AreEqual("+81 90 123", formatter.inputDigit('3'));
            Assert.AreEqual("+81 90 1234", formatter.inputDigit('4'));
            Assert.AreEqual("+81 90 1234 5", formatter.inputDigit('5'));
            Assert.AreEqual("+81 90 1234 56", formatter.inputDigit('6'));
            Assert.AreEqual("+81 90 1234 567", formatter.inputDigit('7'));
            Assert.AreEqual("+81 90 1234 5678", formatter.inputDigit('8'));
            Assert.AreEqual("+81 90 12 345 6789", formatter.inputDigit('9'));
            Assert.AreEqual("+81901234567890", formatter.inputDigit('0'));
            Assert.AreEqual("+819012345678901", formatter.inputDigit('1'));
        }

        [TestMethod]
        public void testAYTFUS()
        {
            AsYouTypeFormatter formatter = phoneUtil.getAsYouTypeFormatter(RegionCode.US);
            Assert.AreEqual("6", formatter.inputDigit('6'));
            Assert.AreEqual("65", formatter.inputDigit('5'));
            Assert.AreEqual("650", formatter.inputDigit('0'));
            Assert.AreEqual("650 2", formatter.inputDigit('2'));
            Assert.AreEqual("650 25", formatter.inputDigit('5'));
            Assert.AreEqual("650 253", formatter.inputDigit('3'));
            // Note this is how a US local number (without area code) should be formatted.
            Assert.AreEqual("650 2532", formatter.inputDigit('2'));
            Assert.AreEqual("650 253 22", formatter.inputDigit('2'));
            Assert.AreEqual("650 253 222", formatter.inputDigit('2'));
            Assert.AreEqual("650 253 2222", formatter.inputDigit('2'));

            formatter.clear();
            Assert.AreEqual("1", formatter.inputDigit('1'));
            Assert.AreEqual("16", formatter.inputDigit('6'));
            Assert.AreEqual("1 65", formatter.inputDigit('5'));
            Assert.AreEqual("1 650", formatter.inputDigit('0'));
            Assert.AreEqual("1 650 2", formatter.inputDigit('2'));
            Assert.AreEqual("1 650 25", formatter.inputDigit('5'));
            Assert.AreEqual("1 650 253", formatter.inputDigit('3'));
            Assert.AreEqual("1 650 253 2", formatter.inputDigit('2'));
            Assert.AreEqual("1 650 253 22", formatter.inputDigit('2'));
            Assert.AreEqual("1 650 253 222", formatter.inputDigit('2'));
            Assert.AreEqual("1 650 253 2222", formatter.inputDigit('2'));

            formatter.clear();
            Assert.AreEqual("0", formatter.inputDigit('0'));
            Assert.AreEqual("01", formatter.inputDigit('1'));
            Assert.AreEqual("011 ", formatter.inputDigit('1'));
            Assert.AreEqual("011 4", formatter.inputDigit('4'));
            Assert.AreEqual("011 44 ", formatter.inputDigit('4'));
            Assert.AreEqual("011 44 6", formatter.inputDigit('6'));
            Assert.AreEqual("011 44 61", formatter.inputDigit('1'));
            Assert.AreEqual("011 44 6 12", formatter.inputDigit('2'));
            Assert.AreEqual("011 44 6 123", formatter.inputDigit('3'));
            Assert.AreEqual("011 44 6 123 1", formatter.inputDigit('1'));
            Assert.AreEqual("011 44 6 123 12", formatter.inputDigit('2'));
            Assert.AreEqual("011 44 6 123 123", formatter.inputDigit('3'));
            Assert.AreEqual("011 44 6 123 123 1", formatter.inputDigit('1'));
            Assert.AreEqual("011 44 6 123 123 12", formatter.inputDigit('2'));
            Assert.AreEqual("011 44 6 123 123 123", formatter.inputDigit('3'));

            formatter.clear();
            Assert.AreEqual("0", formatter.inputDigit('0'));
            Assert.AreEqual("01", formatter.inputDigit('1'));
            Assert.AreEqual("011 ", formatter.inputDigit('1'));
            Assert.AreEqual("011 5", formatter.inputDigit('5'));
            Assert.AreEqual("011 54 ", formatter.inputDigit('4'));
            Assert.AreEqual("011 54 9", formatter.inputDigit('9'));
            Assert.AreEqual("011 54 91", formatter.inputDigit('1'));
            Assert.AreEqual("011 54 9 11", formatter.inputDigit('1'));
            Assert.AreEqual("011 54 9 11 2", formatter.inputDigit('2'));
            Assert.AreEqual("011 54 9 11 23", formatter.inputDigit('3'));
            Assert.AreEqual("011 54 9 11 231", formatter.inputDigit('1'));
            Assert.AreEqual("011 54 9 11 2312", formatter.inputDigit('2'));
            Assert.AreEqual("011 54 9 11 2312 1", formatter.inputDigit('1'));
            Assert.AreEqual("011 54 9 11 2312 12", formatter.inputDigit('2'));
            Assert.AreEqual("011 54 9 11 2312 123", formatter.inputDigit('3'));
            Assert.AreEqual("011 54 9 11 2312 1234", formatter.inputDigit('4'));

            formatter.clear();
            Assert.AreEqual("0", formatter.inputDigit('0'));
            Assert.AreEqual("01", formatter.inputDigit('1'));
            Assert.AreEqual("011 ", formatter.inputDigit('1'));
            Assert.AreEqual("011 2", formatter.inputDigit('2'));
            Assert.AreEqual("011 24", formatter.inputDigit('4'));
            Assert.AreEqual("011 244 ", formatter.inputDigit('4'));
            Assert.AreEqual("011 244 2", formatter.inputDigit('2'));
            Assert.AreEqual("011 244 28", formatter.inputDigit('8'));
            Assert.AreEqual("011 244 280", formatter.inputDigit('0'));
            Assert.AreEqual("011 244 280 0", formatter.inputDigit('0'));
            Assert.AreEqual("011 244 280 00", formatter.inputDigit('0'));
            Assert.AreEqual("011 244 280 000", formatter.inputDigit('0'));
            Assert.AreEqual("011 244 280 000 0", formatter.inputDigit('0'));
            Assert.AreEqual("011 244 280 000 00", formatter.inputDigit('0'));
            Assert.AreEqual("011 244 280 000 000", formatter.inputDigit('0'));

            formatter.clear();
            Assert.AreEqual("+", formatter.inputDigit('+'));
            Assert.AreEqual("+4", formatter.inputDigit('4'));
            Assert.AreEqual("+48 ", formatter.inputDigit('8'));
            Assert.AreEqual("+48 8", formatter.inputDigit('8'));
            Assert.AreEqual("+48 88", formatter.inputDigit('8'));
            Assert.AreEqual("+48 88 1", formatter.inputDigit('1'));
            Assert.AreEqual("+48 88 12", formatter.inputDigit('2'));
            Assert.AreEqual("+48 88 123", formatter.inputDigit('3'));
            Assert.AreEqual("+48 88 123 1", formatter.inputDigit('1'));
            Assert.AreEqual("+48 88 123 12", formatter.inputDigit('2'));
            Assert.AreEqual("+48 88 123 12 1", formatter.inputDigit('1'));
            Assert.AreEqual("+48 88 123 12 12", formatter.inputDigit('2'));
        }

        [TestMethod]
        public void testAYTFUSFullWidthCharacters()
        {
            AsYouTypeFormatter formatter = phoneUtil.getAsYouTypeFormatter(RegionCode.US);
            Assert.AreEqual("\uFF16", formatter.inputDigit('\uFF16'));
            Assert.AreEqual("\uFF16\uFF15", formatter.inputDigit('\uFF15'));
            Assert.AreEqual("650", formatter.inputDigit('\uFF10'));
            Assert.AreEqual("650 2", formatter.inputDigit('\uFF12'));
            Assert.AreEqual("650 25", formatter.inputDigit('\uFF15'));
            Assert.AreEqual("650 253", formatter.inputDigit('\uFF13'));
            Assert.AreEqual("650 2532", formatter.inputDigit('\uFF12'));
            Assert.AreEqual("650 253 22", formatter.inputDigit('\uFF12'));
            Assert.AreEqual("650 253 222", formatter.inputDigit('\uFF12'));
            Assert.AreEqual("650 253 2222", formatter.inputDigit('\uFF12'));
        }

        [TestMethod]
        public void testAYTFUSMobileShortCode()
        {
            AsYouTypeFormatter formatter = phoneUtil.getAsYouTypeFormatter(RegionCode.US);
            Assert.AreEqual("*", formatter.inputDigit('*'));
            Assert.AreEqual("*1", formatter.inputDigit('1'));
            Assert.AreEqual("*12", formatter.inputDigit('2'));
            Assert.AreEqual("*121", formatter.inputDigit('1'));
            Assert.AreEqual("*121#", formatter.inputDigit('#'));
        }

        [TestMethod]
        public void testAYTFUSVanityNumber()
        {
            AsYouTypeFormatter formatter = phoneUtil.getAsYouTypeFormatter(RegionCode.US);
            Assert.AreEqual("8", formatter.inputDigit('8'));
            Assert.AreEqual("80", formatter.inputDigit('0'));
            Assert.AreEqual("800", formatter.inputDigit('0'));
            Assert.AreEqual("800 ", formatter.inputDigit(' '));
            Assert.AreEqual("800 M", formatter.inputDigit('M'));
            Assert.AreEqual("800 MY", formatter.inputDigit('Y'));
            Assert.AreEqual("800 MY ", formatter.inputDigit(' '));
            Assert.AreEqual("800 MY A", formatter.inputDigit('A'));
            Assert.AreEqual("800 MY AP", formatter.inputDigit('P'));
            Assert.AreEqual("800 MY APP", formatter.inputDigit('P'));
            Assert.AreEqual("800 MY APPL", formatter.inputDigit('L'));
            Assert.AreEqual("800 MY APPLE", formatter.inputDigit('E'));
        }

        [TestMethod]
        public void testAYTFAndRememberPositionUS()
        {
            AsYouTypeFormatter formatter = phoneUtil.getAsYouTypeFormatter(RegionCode.US);
            Assert.AreEqual("1", formatter.inputDigitAndRememberPosition('1'));
            Assert.AreEqual(1, formatter.getRememberedPosition());
            Assert.AreEqual("16", formatter.inputDigit('6'));
            Assert.AreEqual("1 65", formatter.inputDigit('5'));
            Assert.AreEqual(1, formatter.getRememberedPosition());
            Assert.AreEqual("1 650", formatter.inputDigitAndRememberPosition('0'));
            Assert.AreEqual(5, formatter.getRememberedPosition());
            Assert.AreEqual("1 650 2", formatter.inputDigit('2'));
            Assert.AreEqual("1 650 25", formatter.inputDigit('5'));
            // Note the remembered position for digit "0" changes from 4 to 5, because a space is now
            // inserted in the front.
            Assert.AreEqual(5, formatter.getRememberedPosition());
            Assert.AreEqual("1 650 253", formatter.inputDigit('3'));
            Assert.AreEqual("1 650 253 2", formatter.inputDigit('2'));
            Assert.AreEqual("1 650 253 22", formatter.inputDigit('2'));
            Assert.AreEqual(5, formatter.getRememberedPosition());
            Assert.AreEqual("1 650 253 222", formatter.inputDigitAndRememberPosition('2'));
            Assert.AreEqual(13, formatter.getRememberedPosition());
            Assert.AreEqual("1 650 253 2222", formatter.inputDigit('2'));
            Assert.AreEqual(13, formatter.getRememberedPosition());
            Assert.AreEqual("165025322222", formatter.inputDigit('2'));
            Assert.AreEqual(10, formatter.getRememberedPosition());
            Assert.AreEqual("1650253222222", formatter.inputDigit('2'));
            Assert.AreEqual(10, formatter.getRememberedPosition());

            formatter.clear();
            Assert.AreEqual("1", formatter.inputDigit('1'));
            Assert.AreEqual("16", formatter.inputDigitAndRememberPosition('6'));
            Assert.AreEqual(2, formatter.getRememberedPosition());
            Assert.AreEqual("1 65", formatter.inputDigit('5'));
            Assert.AreEqual("1 650", formatter.inputDigit('0'));
            Assert.AreEqual(3, formatter.getRememberedPosition());
            Assert.AreEqual("1 650 2", formatter.inputDigit('2'));
            Assert.AreEqual("1 650 25", formatter.inputDigit('5'));
            Assert.AreEqual(3, formatter.getRememberedPosition());
            Assert.AreEqual("1 650 253", formatter.inputDigit('3'));
            Assert.AreEqual("1 650 253 2", formatter.inputDigit('2'));
            Assert.AreEqual("1 650 253 22", formatter.inputDigit('2'));
            Assert.AreEqual(3, formatter.getRememberedPosition());
            Assert.AreEqual("1 650 253 222", formatter.inputDigit('2'));
            Assert.AreEqual("1 650 253 2222", formatter.inputDigit('2'));
            Assert.AreEqual("165025322222", formatter.inputDigit('2'));
            Assert.AreEqual(2, formatter.getRememberedPosition());
            Assert.AreEqual("1650253222222", formatter.inputDigit('2'));
            Assert.AreEqual(2, formatter.getRememberedPosition());

            formatter.clear();
            Assert.AreEqual("6", formatter.inputDigit('6'));
            Assert.AreEqual("65", formatter.inputDigit('5'));
            Assert.AreEqual("650", formatter.inputDigit('0'));
            Assert.AreEqual("650 2", formatter.inputDigit('2'));
            Assert.AreEqual("650 25", formatter.inputDigit('5'));
            Assert.AreEqual("650 253", formatter.inputDigit('3'));
            Assert.AreEqual("650 2532", formatter.inputDigitAndRememberPosition('2'));
            Assert.AreEqual(8, formatter.getRememberedPosition());
            Assert.AreEqual("650 253 22", formatter.inputDigit('2'));
            Assert.AreEqual(9, formatter.getRememberedPosition());
            Assert.AreEqual("650 253 222", formatter.inputDigit('2'));
            // No more formatting when semicolon is entered.
            Assert.AreEqual("650253222;", formatter.inputDigit(';'));
            Assert.AreEqual(7, formatter.getRememberedPosition());
            Assert.AreEqual("650253222;2", formatter.inputDigit('2'));

            formatter.clear();
            Assert.AreEqual("6", formatter.inputDigit('6'));
            Assert.AreEqual("65", formatter.inputDigit('5'));
            Assert.AreEqual("650", formatter.inputDigit('0'));
            // No more formatting when users choose to do their own formatting.
            Assert.AreEqual("650-", formatter.inputDigit('-'));
            Assert.AreEqual("650-2", formatter.inputDigitAndRememberPosition('2'));
            Assert.AreEqual(5, formatter.getRememberedPosition());
            Assert.AreEqual("650-25", formatter.inputDigit('5'));
            Assert.AreEqual(5, formatter.getRememberedPosition());
            Assert.AreEqual("650-253", formatter.inputDigit('3'));
            Assert.AreEqual(5, formatter.getRememberedPosition());
            Assert.AreEqual("650-253-", formatter.inputDigit('-'));
            Assert.AreEqual("650-253-2", formatter.inputDigit('2'));
            Assert.AreEqual("650-253-22", formatter.inputDigit('2'));
            Assert.AreEqual("650-253-222", formatter.inputDigit('2'));
            Assert.AreEqual("650-253-2222", formatter.inputDigit('2'));

            formatter.clear();
            Assert.AreEqual("0", formatter.inputDigit('0'));
            Assert.AreEqual("01", formatter.inputDigit('1'));
            Assert.AreEqual("011 ", formatter.inputDigit('1'));
            Assert.AreEqual("011 4", formatter.inputDigitAndRememberPosition('4'));
            Assert.AreEqual("011 48 ", formatter.inputDigit('8'));
            Assert.AreEqual(5, formatter.getRememberedPosition());
            Assert.AreEqual("011 48 8", formatter.inputDigit('8'));
            Assert.AreEqual(5, formatter.getRememberedPosition());
            Assert.AreEqual("011 48 88", formatter.inputDigit('8'));
            Assert.AreEqual("011 48 88 1", formatter.inputDigit('1'));
            Assert.AreEqual("011 48 88 12", formatter.inputDigit('2'));
            Assert.AreEqual(5, formatter.getRememberedPosition());
            Assert.AreEqual("011 48 88 123", formatter.inputDigit('3'));
            Assert.AreEqual("011 48 88 123 1", formatter.inputDigit('1'));
            Assert.AreEqual("011 48 88 123 12", formatter.inputDigit('2'));
            Assert.AreEqual("011 48 88 123 12 1", formatter.inputDigit('1'));
            Assert.AreEqual("011 48 88 123 12 12", formatter.inputDigit('2'));

            formatter.clear();
            Assert.AreEqual("+", formatter.inputDigit('+'));
            Assert.AreEqual("+1", formatter.inputDigit('1'));
            Assert.AreEqual("+1 6", formatter.inputDigitAndRememberPosition('6'));
            Assert.AreEqual("+1 65", formatter.inputDigit('5'));
            Assert.AreEqual("+1 650", formatter.inputDigit('0'));
            Assert.AreEqual(4, formatter.getRememberedPosition());
            Assert.AreEqual("+1 650 2", formatter.inputDigit('2'));
            Assert.AreEqual(4, formatter.getRememberedPosition());
            Assert.AreEqual("+1 650 25", formatter.inputDigit('5'));
            Assert.AreEqual("+1 650 253", formatter.inputDigitAndRememberPosition('3'));
            Assert.AreEqual("+1 650 253 2", formatter.inputDigit('2'));
            Assert.AreEqual("+1 650 253 22", formatter.inputDigit('2'));
            Assert.AreEqual("+1 650 253 222", formatter.inputDigit('2'));
            Assert.AreEqual(10, formatter.getRememberedPosition());

            formatter.clear();
            Assert.AreEqual("+", formatter.inputDigit('+'));
            Assert.AreEqual("+1", formatter.inputDigit('1'));
            Assert.AreEqual("+1 6", formatter.inputDigitAndRememberPosition('6'));
            Assert.AreEqual("+1 65", formatter.inputDigit('5'));
            Assert.AreEqual("+1 650", formatter.inputDigit('0'));
            Assert.AreEqual(4, formatter.getRememberedPosition());
            Assert.AreEqual("+1 650 2", formatter.inputDigit('2'));
            Assert.AreEqual(4, formatter.getRememberedPosition());
            Assert.AreEqual("+1 650 25", formatter.inputDigit('5'));
            Assert.AreEqual("+1 650 253", formatter.inputDigit('3'));
            Assert.AreEqual("+1 650 253 2", formatter.inputDigit('2'));
            Assert.AreEqual("+1 650 253 22", formatter.inputDigit('2'));
            Assert.AreEqual("+1 650 253 222", formatter.inputDigit('2'));
            Assert.AreEqual("+1650253222;", formatter.inputDigit(';'));
            Assert.AreEqual(3, formatter.getRememberedPosition());
        }

        [TestMethod]
        public void testAYTFGBFixedLine()
        {
            AsYouTypeFormatter formatter = phoneUtil.getAsYouTypeFormatter(RegionCode.GB);
            Assert.AreEqual("0", formatter.inputDigit('0'));
            Assert.AreEqual("02", formatter.inputDigit('2'));
            Assert.AreEqual("020", formatter.inputDigit('0'));
            Assert.AreEqual("020 7", formatter.inputDigitAndRememberPosition('7'));
            Assert.AreEqual(5, formatter.getRememberedPosition());
            Assert.AreEqual("020 70", formatter.inputDigit('0'));
            Assert.AreEqual("020 703", formatter.inputDigit('3'));
            Assert.AreEqual(5, formatter.getRememberedPosition());
            Assert.AreEqual("020 7031", formatter.inputDigit('1'));
            Assert.AreEqual("020 7031 3", formatter.inputDigit('3'));
            Assert.AreEqual("020 7031 30", formatter.inputDigit('0'));
            Assert.AreEqual("020 7031 300", formatter.inputDigit('0'));
            Assert.AreEqual("020 7031 3000", formatter.inputDigit('0'));
        }

        [TestMethod]
        public void testAYTFGBTollFree()
        {
            AsYouTypeFormatter formatter = phoneUtil.getAsYouTypeFormatter(RegionCode.GB);
            Assert.AreEqual("0", formatter.inputDigit('0'));
            Assert.AreEqual("08", formatter.inputDigit('8'));
            Assert.AreEqual("080", formatter.inputDigit('0'));
            Assert.AreEqual("080 7", formatter.inputDigit('7'));
            Assert.AreEqual("080 70", formatter.inputDigit('0'));
            Assert.AreEqual("080 703", formatter.inputDigit('3'));
            Assert.AreEqual("080 7031", formatter.inputDigit('1'));
            Assert.AreEqual("080 7031 3", formatter.inputDigit('3'));
            Assert.AreEqual("080 7031 30", formatter.inputDigit('0'));
            Assert.AreEqual("080 7031 300", formatter.inputDigit('0'));
            Assert.AreEqual("080 7031 3000", formatter.inputDigit('0'));
        }

        [TestMethod]
        public void testAYTFGBPremiumRate()
        {
            AsYouTypeFormatter formatter = phoneUtil.getAsYouTypeFormatter(RegionCode.GB);
            Assert.AreEqual("0", formatter.inputDigit('0'));
            Assert.AreEqual("09", formatter.inputDigit('9'));
            Assert.AreEqual("090", formatter.inputDigit('0'));
            Assert.AreEqual("090 7", formatter.inputDigit('7'));
            Assert.AreEqual("090 70", formatter.inputDigit('0'));
            Assert.AreEqual("090 703", formatter.inputDigit('3'));
            Assert.AreEqual("090 7031", formatter.inputDigit('1'));
            Assert.AreEqual("090 7031 3", formatter.inputDigit('3'));
            Assert.AreEqual("090 7031 30", formatter.inputDigit('0'));
            Assert.AreEqual("090 7031 300", formatter.inputDigit('0'));
            Assert.AreEqual("090 7031 3000", formatter.inputDigit('0'));
        }

        [TestMethod]
        public void testAYTFNZMobile()
        {
            AsYouTypeFormatter formatter = phoneUtil.getAsYouTypeFormatter(RegionCode.NZ);
            Assert.AreEqual("0", formatter.inputDigit('0'));
            Assert.AreEqual("02", formatter.inputDigit('2'));
            Assert.AreEqual("021", formatter.inputDigit('1'));
            Assert.AreEqual("02-11", formatter.inputDigit('1'));
            Assert.AreEqual("02-112", formatter.inputDigit('2'));
            // Note the unittest is using fake metadata which might produce non-ideal results.
            Assert.AreEqual("02-112 3", formatter.inputDigit('3'));
            Assert.AreEqual("02-112 34", formatter.inputDigit('4'));
            Assert.AreEqual("02-112 345", formatter.inputDigit('5'));
            Assert.AreEqual("02-112 3456", formatter.inputDigit('6'));
        }

        [TestMethod]
        public void testAYTFDE()
        {
            AsYouTypeFormatter formatter = phoneUtil.getAsYouTypeFormatter(RegionCode.DE);
            Assert.AreEqual("0", formatter.inputDigit('0'));
            Assert.AreEqual("03", formatter.inputDigit('3'));
            Assert.AreEqual("030", formatter.inputDigit('0'));
            Assert.AreEqual("030/1", formatter.inputDigit('1'));
            Assert.AreEqual("030/12", formatter.inputDigit('2'));
            Assert.AreEqual("030/123", formatter.inputDigit('3'));
            Assert.AreEqual("030/1234", formatter.inputDigit('4'));

            // 04134 1234
            formatter.clear();
            Assert.AreEqual("0", formatter.inputDigit('0'));
            Assert.AreEqual("04", formatter.inputDigit('4'));
            Assert.AreEqual("041", formatter.inputDigit('1'));
            Assert.AreEqual("041 3", formatter.inputDigit('3'));
            Assert.AreEqual("041 34", formatter.inputDigit('4'));
            Assert.AreEqual("04134 1", formatter.inputDigit('1'));
            Assert.AreEqual("04134 12", formatter.inputDigit('2'));
            Assert.AreEqual("04134 123", formatter.inputDigit('3'));
            Assert.AreEqual("04134 1234", formatter.inputDigit('4'));

            // 08021 2345
            formatter.clear();
            Assert.AreEqual("0", formatter.inputDigit('0'));
            Assert.AreEqual("08", formatter.inputDigit('8'));
            Assert.AreEqual("080", formatter.inputDigit('0'));
            Assert.AreEqual("080 2", formatter.inputDigit('2'));
            Assert.AreEqual("080 21", formatter.inputDigit('1'));
            Assert.AreEqual("08021 2", formatter.inputDigit('2'));
            Assert.AreEqual("08021 23", formatter.inputDigit('3'));
            Assert.AreEqual("08021 234", formatter.inputDigit('4'));
            Assert.AreEqual("08021 2345", formatter.inputDigit('5'));

            // 00 1 650 253 2250
            formatter.clear();
            Assert.AreEqual("0", formatter.inputDigit('0'));
            Assert.AreEqual("00", formatter.inputDigit('0'));
            Assert.AreEqual("00 1 ", formatter.inputDigit('1'));
            Assert.AreEqual("00 1 6", formatter.inputDigit('6'));
            Assert.AreEqual("00 1 65", formatter.inputDigit('5'));
            Assert.AreEqual("00 1 650", formatter.inputDigit('0'));
            Assert.AreEqual("00 1 650 2", formatter.inputDigit('2'));
            Assert.AreEqual("00 1 650 25", formatter.inputDigit('5'));
            Assert.AreEqual("00 1 650 253", formatter.inputDigit('3'));
            Assert.AreEqual("00 1 650 253 2", formatter.inputDigit('2'));
            Assert.AreEqual("00 1 650 253 22", formatter.inputDigit('2'));
            Assert.AreEqual("00 1 650 253 222", formatter.inputDigit('2'));
            Assert.AreEqual("00 1 650 253 2222", formatter.inputDigit('2'));
        }

        [TestMethod]
        public void testAYTFAR()
        {
            AsYouTypeFormatter formatter = phoneUtil.getAsYouTypeFormatter(RegionCode.AR);
            Assert.AreEqual("0", formatter.inputDigit('0'));
            Assert.AreEqual("01", formatter.inputDigit('1'));
            Assert.AreEqual("011", formatter.inputDigit('1'));
            Assert.AreEqual("011 7", formatter.inputDigit('7'));
            Assert.AreEqual("011 70", formatter.inputDigit('0'));
            Assert.AreEqual("011 703", formatter.inputDigit('3'));
            Assert.AreEqual("011 7031", formatter.inputDigit('1'));
            Assert.AreEqual("011 7031-3", formatter.inputDigit('3'));
            Assert.AreEqual("011 7031-30", formatter.inputDigit('0'));
            Assert.AreEqual("011 7031-300", formatter.inputDigit('0'));
            Assert.AreEqual("011 7031-3000", formatter.inputDigit('0'));
        }

        [TestMethod]
        public void testAYTFARMobile()
        {
            AsYouTypeFormatter formatter = phoneUtil.getAsYouTypeFormatter(RegionCode.AR);
            Assert.AreEqual("+", formatter.inputDigit('+'));
            Assert.AreEqual("+5", formatter.inputDigit('5'));
            Assert.AreEqual("+54 ", formatter.inputDigit('4'));
            Assert.AreEqual("+54 9", formatter.inputDigit('9'));
            Assert.AreEqual("+54 91", formatter.inputDigit('1'));
            Assert.AreEqual("+54 9 11", formatter.inputDigit('1'));
            Assert.AreEqual("+54 9 11 2", formatter.inputDigit('2'));
            Assert.AreEqual("+54 9 11 23", formatter.inputDigit('3'));
            Assert.AreEqual("+54 9 11 231", formatter.inputDigit('1'));
            Assert.AreEqual("+54 9 11 2312", formatter.inputDigit('2'));
            Assert.AreEqual("+54 9 11 2312 1", formatter.inputDigit('1'));
            Assert.AreEqual("+54 9 11 2312 12", formatter.inputDigit('2'));
            Assert.AreEqual("+54 9 11 2312 123", formatter.inputDigit('3'));
            Assert.AreEqual("+54 9 11 2312 1234", formatter.inputDigit('4'));
        }

        [TestMethod]
        public void testAYTFKR()
        {
            // +82 51 234 5678
            AsYouTypeFormatter formatter = phoneUtil.getAsYouTypeFormatter(RegionCode.KR);
            Assert.AreEqual("+", formatter.inputDigit('+'));
            Assert.AreEqual("+8", formatter.inputDigit('8'));
            Assert.AreEqual("+82 ", formatter.inputDigit('2'));
            Assert.AreEqual("+82 5", formatter.inputDigit('5'));
            Assert.AreEqual("+82 51", formatter.inputDigit('1'));
            Assert.AreEqual("+82 51-2", formatter.inputDigit('2'));
            Assert.AreEqual("+82 51-23", formatter.inputDigit('3'));
            Assert.AreEqual("+82 51-234", formatter.inputDigit('4'));
            Assert.AreEqual("+82 51-234-5", formatter.inputDigit('5'));
            Assert.AreEqual("+82 51-234-56", formatter.inputDigit('6'));
            Assert.AreEqual("+82 51-234-567", formatter.inputDigit('7'));
            Assert.AreEqual("+82 51-234-5678", formatter.inputDigit('8'));

            // +82 2 531 5678
            formatter.clear();
            Assert.AreEqual("+", formatter.inputDigit('+'));
            Assert.AreEqual("+8", formatter.inputDigit('8'));
            Assert.AreEqual("+82 ", formatter.inputDigit('2'));
            Assert.AreEqual("+82 2", formatter.inputDigit('2'));
            Assert.AreEqual("+82 25", formatter.inputDigit('5'));
            Assert.AreEqual("+82 2-53", formatter.inputDigit('3'));
            Assert.AreEqual("+82 2-531", formatter.inputDigit('1'));
            Assert.AreEqual("+82 2-531-5", formatter.inputDigit('5'));
            Assert.AreEqual("+82 2-531-56", formatter.inputDigit('6'));
            Assert.AreEqual("+82 2-531-567", formatter.inputDigit('7'));
            Assert.AreEqual("+82 2-531-5678", formatter.inputDigit('8'));

            // +82 2 3665 5678
            formatter.clear();
            Assert.AreEqual("+", formatter.inputDigit('+'));
            Assert.AreEqual("+8", formatter.inputDigit('8'));
            Assert.AreEqual("+82 ", formatter.inputDigit('2'));
            Assert.AreEqual("+82 2", formatter.inputDigit('2'));
            Assert.AreEqual("+82 23", formatter.inputDigit('3'));
            Assert.AreEqual("+82 2-36", formatter.inputDigit('6'));
            Assert.AreEqual("+82 2-366", formatter.inputDigit('6'));
            Assert.AreEqual("+82 2-3665", formatter.inputDigit('5'));
            Assert.AreEqual("+82 2-3665-5", formatter.inputDigit('5'));
            Assert.AreEqual("+82 2-3665-56", formatter.inputDigit('6'));
            Assert.AreEqual("+82 2-3665-567", formatter.inputDigit('7'));
            Assert.AreEqual("+82 2-3665-5678", formatter.inputDigit('8'));

            // 02-114
            formatter.clear();
            Assert.AreEqual("0", formatter.inputDigit('0'));
            Assert.AreEqual("02", formatter.inputDigit('2'));
            Assert.AreEqual("021", formatter.inputDigit('1'));
            Assert.AreEqual("02-11", formatter.inputDigit('1'));
            Assert.AreEqual("02-114", formatter.inputDigit('4'));

            // 02-1300
            formatter.clear();
            Assert.AreEqual("0", formatter.inputDigit('0'));
            Assert.AreEqual("02", formatter.inputDigit('2'));
            Assert.AreEqual("021", formatter.inputDigit('1'));
            Assert.AreEqual("02-13", formatter.inputDigit('3'));
            Assert.AreEqual("02-130", formatter.inputDigit('0'));
            Assert.AreEqual("02-1300", formatter.inputDigit('0'));

            // 011-456-7890
            formatter.clear();
            Assert.AreEqual("0", formatter.inputDigit('0'));
            Assert.AreEqual("01", formatter.inputDigit('1'));
            Assert.AreEqual("011", formatter.inputDigit('1'));
            Assert.AreEqual("011-4", formatter.inputDigit('4'));
            Assert.AreEqual("011-45", formatter.inputDigit('5'));
            Assert.AreEqual("011-456", formatter.inputDigit('6'));
            Assert.AreEqual("011-456-7", formatter.inputDigit('7'));
            Assert.AreEqual("011-456-78", formatter.inputDigit('8'));
            Assert.AreEqual("011-456-789", formatter.inputDigit('9'));
            Assert.AreEqual("011-456-7890", formatter.inputDigit('0'));

            // 011-9876-7890
            formatter.clear();
            Assert.AreEqual("0", formatter.inputDigit('0'));
            Assert.AreEqual("01", formatter.inputDigit('1'));
            Assert.AreEqual("011", formatter.inputDigit('1'));
            Assert.AreEqual("011-9", formatter.inputDigit('9'));
            Assert.AreEqual("011-98", formatter.inputDigit('8'));
            Assert.AreEqual("011-987", formatter.inputDigit('7'));
            Assert.AreEqual("011-9876", formatter.inputDigit('6'));
            Assert.AreEqual("011-9876-7", formatter.inputDigit('7'));
            Assert.AreEqual("011-9876-78", formatter.inputDigit('8'));
            Assert.AreEqual("011-9876-789", formatter.inputDigit('9'));
            Assert.AreEqual("011-9876-7890", formatter.inputDigit('0'));
        }

        [TestMethod]
        public void testAYTF_MX()
        {
            AsYouTypeFormatter formatter = phoneUtil.getAsYouTypeFormatter(RegionCode.MX);

            // +52 800 123 4567
            Assert.AreEqual("+", formatter.inputDigit('+'));
            Assert.AreEqual("+5", formatter.inputDigit('5'));
            Assert.AreEqual("+52 ", formatter.inputDigit('2'));
            Assert.AreEqual("+52 8", formatter.inputDigit('8'));
            Assert.AreEqual("+52 80", formatter.inputDigit('0'));
            Assert.AreEqual("+52 800", formatter.inputDigit('0'));
            Assert.AreEqual("+52 800 1", formatter.inputDigit('1'));
            Assert.AreEqual("+52 800 12", formatter.inputDigit('2'));
            Assert.AreEqual("+52 800 123", formatter.inputDigit('3'));
            Assert.AreEqual("+52 800 123 4", formatter.inputDigit('4'));
            Assert.AreEqual("+52 800 123 45", formatter.inputDigit('5'));
            Assert.AreEqual("+52 800 123 456", formatter.inputDigit('6'));
            Assert.AreEqual("+52 800 123 4567", formatter.inputDigit('7'));

            // +52 55 1234 5678
            formatter.clear();
            Assert.AreEqual("+", formatter.inputDigit('+'));
            Assert.AreEqual("+5", formatter.inputDigit('5'));
            Assert.AreEqual("+52 ", formatter.inputDigit('2'));
            Assert.AreEqual("+52 5", formatter.inputDigit('5'));
            Assert.AreEqual("+52 55", formatter.inputDigit('5'));
            Assert.AreEqual("+52 55 1", formatter.inputDigit('1'));
            Assert.AreEqual("+52 55 12", formatter.inputDigit('2'));
            Assert.AreEqual("+52 55 123", formatter.inputDigit('3'));
            Assert.AreEqual("+52 55 1234", formatter.inputDigit('4'));
            Assert.AreEqual("+52 55 1234 5", formatter.inputDigit('5'));
            Assert.AreEqual("+52 55 1234 56", formatter.inputDigit('6'));
            Assert.AreEqual("+52 55 1234 567", formatter.inputDigit('7'));
            Assert.AreEqual("+52 55 1234 5678", formatter.inputDigit('8'));

            // +52 212 345 6789
            formatter.clear();
            Assert.AreEqual("+", formatter.inputDigit('+'));
            Assert.AreEqual("+5", formatter.inputDigit('5'));
            Assert.AreEqual("+52 ", formatter.inputDigit('2'));
            Assert.AreEqual("+52 2", formatter.inputDigit('2'));
            Assert.AreEqual("+52 21", formatter.inputDigit('1'));
            Assert.AreEqual("+52 212", formatter.inputDigit('2'));
            Assert.AreEqual("+52 212 3", formatter.inputDigit('3'));
            Assert.AreEqual("+52 212 34", formatter.inputDigit('4'));
            Assert.AreEqual("+52 212 345", formatter.inputDigit('5'));
            Assert.AreEqual("+52 212 345 6", formatter.inputDigit('6'));
            Assert.AreEqual("+52 212 345 67", formatter.inputDigit('7'));
            Assert.AreEqual("+52 212 345 678", formatter.inputDigit('8'));
            Assert.AreEqual("+52 212 345 6789", formatter.inputDigit('9'));

            // +52 1 55 1234 5678
            formatter.clear();
            Assert.AreEqual("+", formatter.inputDigit('+'));
            Assert.AreEqual("+5", formatter.inputDigit('5'));
            Assert.AreEqual("+52 ", formatter.inputDigit('2'));
            Assert.AreEqual("+52 1", formatter.inputDigit('1'));
            Assert.AreEqual("+52 15", formatter.inputDigit('5'));
            Assert.AreEqual("+52 1 55", formatter.inputDigit('5'));
            Assert.AreEqual("+52 1 55 1", formatter.inputDigit('1'));
            Assert.AreEqual("+52 1 55 12", formatter.inputDigit('2'));
            Assert.AreEqual("+52 1 55 123", formatter.inputDigit('3'));
            Assert.AreEqual("+52 1 55 1234", formatter.inputDigit('4'));
            Assert.AreEqual("+52 1 55 1234 5", formatter.inputDigit('5'));
            Assert.AreEqual("+52 1 55 1234 56", formatter.inputDigit('6'));
            Assert.AreEqual("+52 1 55 1234 567", formatter.inputDigit('7'));
            Assert.AreEqual("+52 1 55 1234 5678", formatter.inputDigit('8'));

            // +52 1 541 234 5678
            formatter.clear();
            Assert.AreEqual("+", formatter.inputDigit('+'));
            Assert.AreEqual("+5", formatter.inputDigit('5'));
            Assert.AreEqual("+52 ", formatter.inputDigit('2'));
            Assert.AreEqual("+52 1", formatter.inputDigit('1'));
            Assert.AreEqual("+52 15", formatter.inputDigit('5'));
            Assert.AreEqual("+52 1 54", formatter.inputDigit('4'));
            Assert.AreEqual("+52 1 541", formatter.inputDigit('1'));
            Assert.AreEqual("+52 1 541 2", formatter.inputDigit('2'));
            Assert.AreEqual("+52 1 541 23", formatter.inputDigit('3'));
            Assert.AreEqual("+52 1 541 234", formatter.inputDigit('4'));
            Assert.AreEqual("+52 1 541 234 5", formatter.inputDigit('5'));
            Assert.AreEqual("+52 1 541 234 56", formatter.inputDigit('6'));
            Assert.AreEqual("+52 1 541 234 567", formatter.inputDigit('7'));
            Assert.AreEqual("+52 1 541 234 5678", formatter.inputDigit('8'));
        }

        [TestMethod]
        public void testAYTF_International_Toll_Free()
        {
            AsYouTypeFormatter formatter = phoneUtil.getAsYouTypeFormatter(RegionCode.US);
            // +800 1234 5678
            Assert.AreEqual("+", formatter.inputDigit('+'));
            Assert.AreEqual("+8", formatter.inputDigit('8'));
            Assert.AreEqual("+80", formatter.inputDigit('0'));
            Assert.AreEqual("+800 ", formatter.inputDigit('0'));
            Assert.AreEqual("+800 1", formatter.inputDigit('1'));
            Assert.AreEqual("+800 12", formatter.inputDigit('2'));
            Assert.AreEqual("+800 123", formatter.inputDigit('3'));
            Assert.AreEqual("+800 1234", formatter.inputDigit('4'));
            Assert.AreEqual("+800 1234 5", formatter.inputDigit('5'));
            Assert.AreEqual("+800 1234 56", formatter.inputDigit('6'));
            Assert.AreEqual("+800 1234 567", formatter.inputDigit('7'));
            Assert.AreEqual("+800 1234 5678", formatter.inputDigit('8'));
            Assert.AreEqual("+800123456789", formatter.inputDigit('9'));
        }

        [TestMethod]
        public void testAYTFMultipleLeadingDigitPatterns()
        {
            // +81 50 2345 6789
            AsYouTypeFormatter formatter = phoneUtil.getAsYouTypeFormatter(RegionCode.JP);
            Assert.AreEqual("+", formatter.inputDigit('+'));
            Assert.AreEqual("+8", formatter.inputDigit('8'));
            Assert.AreEqual("+81 ", formatter.inputDigit('1'));
            Assert.AreEqual("+81 5", formatter.inputDigit('5'));
            Assert.AreEqual("+81 50", formatter.inputDigit('0'));
            Assert.AreEqual("+81 50 2", formatter.inputDigit('2'));
            Assert.AreEqual("+81 50 23", formatter.inputDigit('3'));
            Assert.AreEqual("+81 50 234", formatter.inputDigit('4'));
            Assert.AreEqual("+81 50 2345", formatter.inputDigit('5'));
            Assert.AreEqual("+81 50 2345 6", formatter.inputDigit('6'));
            Assert.AreEqual("+81 50 2345 67", formatter.inputDigit('7'));
            Assert.AreEqual("+81 50 2345 678", formatter.inputDigit('8'));
            Assert.AreEqual("+81 50 2345 6789", formatter.inputDigit('9'));

            // +81 222 12 5678
            formatter.clear();
            Assert.AreEqual("+", formatter.inputDigit('+'));
            Assert.AreEqual("+8", formatter.inputDigit('8'));
            Assert.AreEqual("+81 ", formatter.inputDigit('1'));
            Assert.AreEqual("+81 2", formatter.inputDigit('2'));
            Assert.AreEqual("+81 22", formatter.inputDigit('2'));
            Assert.AreEqual("+81 22 2", formatter.inputDigit('2'));
            Assert.AreEqual("+81 22 21", formatter.inputDigit('1'));
            Assert.AreEqual("+81 2221 2", formatter.inputDigit('2'));
            Assert.AreEqual("+81 222 12 5", formatter.inputDigit('5'));
            Assert.AreEqual("+81 222 12 56", formatter.inputDigit('6'));
            Assert.AreEqual("+81 222 12 567", formatter.inputDigit('7'));
            Assert.AreEqual("+81 222 12 5678", formatter.inputDigit('8'));

            // 011113
            formatter.clear();
            Assert.AreEqual("0", formatter.inputDigit('0'));
            Assert.AreEqual("01", formatter.inputDigit('1'));
            Assert.AreEqual("011", formatter.inputDigit('1'));
            Assert.AreEqual("011 1", formatter.inputDigit('1'));
            Assert.AreEqual("011 11", formatter.inputDigit('1'));
            Assert.AreEqual("011113", formatter.inputDigit('3'));

            // +81 3332 2 5678
            formatter.clear();
            Assert.AreEqual("+", formatter.inputDigit('+'));
            Assert.AreEqual("+8", formatter.inputDigit('8'));
            Assert.AreEqual("+81 ", formatter.inputDigit('1'));
            Assert.AreEqual("+81 3", formatter.inputDigit('3'));
            Assert.AreEqual("+81 33", formatter.inputDigit('3'));
            Assert.AreEqual("+81 33 3", formatter.inputDigit('3'));
            Assert.AreEqual("+81 3332", formatter.inputDigit('2'));
            Assert.AreEqual("+81 3332 2", formatter.inputDigit('2'));
            Assert.AreEqual("+81 3332 2 5", formatter.inputDigit('5'));
            Assert.AreEqual("+81 3332 2 56", formatter.inputDigit('6'));
            Assert.AreEqual("+81 3332 2 567", formatter.inputDigit('7'));
            Assert.AreEqual("+81 3332 2 5678", formatter.inputDigit('8'));
        }

        [TestMethod]
        public void testAYTFLongIDD_AU()
        {
            AsYouTypeFormatter formatter = phoneUtil.getAsYouTypeFormatter("AU");
            // 0011 1 650 253 2250
            Assert.AreEqual("0", formatter.inputDigit('0'));
            Assert.AreEqual("00", formatter.inputDigit('0'));
            Assert.AreEqual("001", formatter.inputDigit('1'));
            Assert.AreEqual("0011", formatter.inputDigit('1'));
            Assert.AreEqual("0011 1 ", formatter.inputDigit('1'));
            Assert.AreEqual("0011 1 6", formatter.inputDigit('6'));
            Assert.AreEqual("0011 1 65", formatter.inputDigit('5'));
            Assert.AreEqual("0011 1 650", formatter.inputDigit('0'));
            Assert.AreEqual("0011 1 650 2", formatter.inputDigit('2'));
            Assert.AreEqual("0011 1 650 25", formatter.inputDigit('5'));
            Assert.AreEqual("0011 1 650 253", formatter.inputDigit('3'));
            Assert.AreEqual("0011 1 650 253 2", formatter.inputDigit('2'));
            Assert.AreEqual("0011 1 650 253 22", formatter.inputDigit('2'));
            Assert.AreEqual("0011 1 650 253 222", formatter.inputDigit('2'));
            Assert.AreEqual("0011 1 650 253 2222", formatter.inputDigit('2'));

            // 0011 81 3332 2 5678
            formatter.clear();
            Assert.AreEqual("0", formatter.inputDigit('0'));
            Assert.AreEqual("00", formatter.inputDigit('0'));
            Assert.AreEqual("001", formatter.inputDigit('1'));
            Assert.AreEqual("0011", formatter.inputDigit('1'));
            Assert.AreEqual("00118", formatter.inputDigit('8'));
            Assert.AreEqual("0011 81 ", formatter.inputDigit('1'));
            Assert.AreEqual("0011 81 3", formatter.inputDigit('3'));
            Assert.AreEqual("0011 81 33", formatter.inputDigit('3'));
            Assert.AreEqual("0011 81 33 3", formatter.inputDigit('3'));
            Assert.AreEqual("0011 81 3332", formatter.inputDigit('2'));
            Assert.AreEqual("0011 81 3332 2", formatter.inputDigit('2'));
            Assert.AreEqual("0011 81 3332 2 5", formatter.inputDigit('5'));
            Assert.AreEqual("0011 81 3332 2 56", formatter.inputDigit('6'));
            Assert.AreEqual("0011 81 3332 2 567", formatter.inputDigit('7'));
            Assert.AreEqual("0011 81 3332 2 5678", formatter.inputDigit('8'));

            // 0011 244 250 253 222
            formatter.clear();
            Assert.AreEqual("0", formatter.inputDigit('0'));
            Assert.AreEqual("00", formatter.inputDigit('0'));
            Assert.AreEqual("001", formatter.inputDigit('1'));
            Assert.AreEqual("0011", formatter.inputDigit('1'));
            Assert.AreEqual("00112", formatter.inputDigit('2'));
            Assert.AreEqual("001124", formatter.inputDigit('4'));
            Assert.AreEqual("0011 244 ", formatter.inputDigit('4'));
            Assert.AreEqual("0011 244 2", formatter.inputDigit('2'));
            Assert.AreEqual("0011 244 25", formatter.inputDigit('5'));
            Assert.AreEqual("0011 244 250", formatter.inputDigit('0'));
            Assert.AreEqual("0011 244 250 2", formatter.inputDigit('2'));
            Assert.AreEqual("0011 244 250 25", formatter.inputDigit('5'));
            Assert.AreEqual("0011 244 250 253", formatter.inputDigit('3'));
            Assert.AreEqual("0011 244 250 253 2", formatter.inputDigit('2'));
            Assert.AreEqual("0011 244 250 253 22", formatter.inputDigit('2'));
            Assert.AreEqual("0011 244 250 253 222", formatter.inputDigit('2'));
        }

        [TestMethod]
        public void testAYTFLongIDD_KR()
        {
            AsYouTypeFormatter formatter = phoneUtil.getAsYouTypeFormatter("KR");
            // 00300 1 650 253 2222
            Assert.AreEqual("0", formatter.inputDigit('0'));
            Assert.AreEqual("00", formatter.inputDigit('0'));
            Assert.AreEqual("003", formatter.inputDigit('3'));
            Assert.AreEqual("0030", formatter.inputDigit('0'));
            Assert.AreEqual("00300", formatter.inputDigit('0'));
            Assert.AreEqual("00300 1 ", formatter.inputDigit('1'));
            Assert.AreEqual("00300 1 6", formatter.inputDigit('6'));
            Assert.AreEqual("00300 1 65", formatter.inputDigit('5'));
            Assert.AreEqual("00300 1 650", formatter.inputDigit('0'));
            Assert.AreEqual("00300 1 650 2", formatter.inputDigit('2'));
            Assert.AreEqual("00300 1 650 25", formatter.inputDigit('5'));
            Assert.AreEqual("00300 1 650 253", formatter.inputDigit('3'));
            Assert.AreEqual("00300 1 650 253 2", formatter.inputDigit('2'));
            Assert.AreEqual("00300 1 650 253 22", formatter.inputDigit('2'));
            Assert.AreEqual("00300 1 650 253 222", formatter.inputDigit('2'));
            Assert.AreEqual("00300 1 650 253 2222", formatter.inputDigit('2'));
        }

        [TestMethod]
        public void testAYTFLongNDD_KR()
        {
            AsYouTypeFormatter formatter = phoneUtil.getAsYouTypeFormatter("KR");
            // 08811-9876-7890
            Assert.AreEqual("0", formatter.inputDigit('0'));
            Assert.AreEqual("08", formatter.inputDigit('8'));
            Assert.AreEqual("088", formatter.inputDigit('8'));
            Assert.AreEqual("0881", formatter.inputDigit('1'));
            Assert.AreEqual("08811", formatter.inputDigit('1'));
            Assert.AreEqual("08811-9", formatter.inputDigit('9'));
            Assert.AreEqual("08811-98", formatter.inputDigit('8'));
            Assert.AreEqual("08811-987", formatter.inputDigit('7'));
            Assert.AreEqual("08811-9876", formatter.inputDigit('6'));
            Assert.AreEqual("08811-9876-7", formatter.inputDigit('7'));
            Assert.AreEqual("08811-9876-78", formatter.inputDigit('8'));
            Assert.AreEqual("08811-9876-789", formatter.inputDigit('9'));
            Assert.AreEqual("08811-9876-7890", formatter.inputDigit('0'));

            // 08500 11-9876-7890
            formatter.clear();
            Assert.AreEqual("0", formatter.inputDigit('0'));
            Assert.AreEqual("08", formatter.inputDigit('8'));
            Assert.AreEqual("085", formatter.inputDigit('5'));
            Assert.AreEqual("0850", formatter.inputDigit('0'));
            Assert.AreEqual("08500 ", formatter.inputDigit('0'));
            Assert.AreEqual("08500 1", formatter.inputDigit('1'));
            Assert.AreEqual("08500 11", formatter.inputDigit('1'));
            Assert.AreEqual("08500 11-9", formatter.inputDigit('9'));
            Assert.AreEqual("08500 11-98", formatter.inputDigit('8'));
            Assert.AreEqual("08500 11-987", formatter.inputDigit('7'));
            Assert.AreEqual("08500 11-9876", formatter.inputDigit('6'));
            Assert.AreEqual("08500 11-9876-7", formatter.inputDigit('7'));
            Assert.AreEqual("08500 11-9876-78", formatter.inputDigit('8'));
            Assert.AreEqual("08500 11-9876-789", formatter.inputDigit('9'));
            Assert.AreEqual("08500 11-9876-7890", formatter.inputDigit('0'));
        }

        [TestMethod]
        public void testAYTFLongNDD_SG()
        {
            AsYouTypeFormatter formatter = phoneUtil.getAsYouTypeFormatter("SG");
            // 777777 9876 7890
            Assert.AreEqual("7", formatter.inputDigit('7'));
            Assert.AreEqual("77", formatter.inputDigit('7'));
            Assert.AreEqual("777", formatter.inputDigit('7'));
            Assert.AreEqual("7777", formatter.inputDigit('7'));
            Assert.AreEqual("77777", formatter.inputDigit('7'));
            Assert.AreEqual("777777 ", formatter.inputDigit('7'));
            Assert.AreEqual("777777 9", formatter.inputDigit('9'));
            Assert.AreEqual("777777 98", formatter.inputDigit('8'));
            Assert.AreEqual("777777 987", formatter.inputDigit('7'));
            Assert.AreEqual("777777 9876", formatter.inputDigit('6'));
            Assert.AreEqual("777777 9876 7", formatter.inputDigit('7'));
            Assert.AreEqual("777777 9876 78", formatter.inputDigit('8'));
            Assert.AreEqual("777777 9876 789", formatter.inputDigit('9'));
            Assert.AreEqual("777777 9876 7890", formatter.inputDigit('0'));
        }
    }
}
