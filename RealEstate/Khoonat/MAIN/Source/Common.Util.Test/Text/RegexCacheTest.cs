using System;
using JahanJooy.Common.Util.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JahanJooy.Common.Util.Test.Text
{
    [TestClass]
    public class RegexCacheTest
    {
        private RegexCache regexCache;

        public RegexCacheTest()
        {
            regexCache = new RegexCache(2);
        }

        [TestMethod]
        public void TestRegexInsertion()
        {
            string regex1 = "[1-5]";
            string regex2 = "(?:12|34)";
            string regex3 = "[1-3][58]";

            regexCache.getPatternForRegex(regex1);
            Assert.IsTrue(regexCache.containsRegex(regex1));

            regexCache.getPatternForRegex(regex2);
            Assert.IsTrue(regexCache.containsRegex(regex2));
            Assert.IsTrue(regexCache.containsRegex(regex1));

            regexCache.getPatternForRegex(regex1);
            Assert.IsTrue(regexCache.containsRegex(regex1));

            regexCache.getPatternForRegex(regex3);
            Assert.IsTrue(regexCache.containsRegex(regex3));

            Assert.IsFalse(regexCache.containsRegex(regex2));
            Assert.IsTrue(regexCache.containsRegex(regex1));
        }
    }
}
