using JahanJooy.Common.Util.PhoneNumbers;

namespace JahanJooy.Common.Util.Test.PhoneNumbers
{
    public class TestMetaDataBase
    {
        private const string MetaDataResourceName = "JahanJooy.Common.Util.PhoneNumbers.Data.PhoneNumberMetaDataForTesting.xml";
        protected readonly PhoneNumberUtil phoneUtil;

        public TestMetaDataBase()
        {
            phoneUtil = initializePhoneUtilForTesting();
        }

        private static PhoneNumberUtil initializePhoneUtilForTesting()
        {
            PhoneNumberUtil.resetInstance();
            PhoneNumberUtil phoneUtil = PhoneNumberUtil.getInstance(MetaDataResourceName, CountryCodeToRegionCodeMapForTesting.getCountryCodeToRegionCodeMap());
            return phoneUtil;
        }
    }
}