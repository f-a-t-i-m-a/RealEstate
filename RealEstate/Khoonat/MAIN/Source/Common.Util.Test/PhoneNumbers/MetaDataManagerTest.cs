using System;
using JahanJooy.Common.Util.PhoneNumbers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JahanJooy.Common.Util.Test.PhoneNumbers
{
    [TestClass]
    public class MetaDataManagerTest
    {
        [TestMethod]
        public void testAlternateFormatsContainsData()
        {
            // We should have some data for Germany.
            PhoneMetaData germanyAlternateFormats = MetaDataManager.getAlternateFormatsForCountry(49);
            Assert.IsNotNull(germanyAlternateFormats);
            Assert.IsTrue(germanyAlternateFormats.numberFormats().Count > 0);
        }

        [TestMethod]
        public void testAlternateFormatsFailsGracefully()
        {
            PhoneMetaData noAlternateFormats = MetaDataManager.getAlternateFormatsForCountry(999);
            Assert.IsNull(noAlternateFormats);
        }
    }
}