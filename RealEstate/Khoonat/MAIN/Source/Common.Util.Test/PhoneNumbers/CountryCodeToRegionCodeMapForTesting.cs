using System.Collections.Generic;

namespace JahanJooy.Common.Util.Test.PhoneNumbers
{
    public class CountryCodeToRegionCodeMapForTesting
    {
        // A mapping from a country code to the region codes which denote the
        // country/region represented by that country code. In the case of multiple
        // countries sharing a calling code, such as the NANPA countries, the one
        // indicated with "isMainCountryForCode" in the metadata should be first.
        internal static IDictionary<int, IList<string>> getCountryCodeToRegionCodeMap()
        {
            // The capacity is set to 24 as there are 18 different country codes,
            // and this offers a load factor of roughly 0.75.
            IDictionary<int, IList<string>> countryCodeToRegionCodeMap = new Dictionary<int, IList<string>>(24);

            countryCodeToRegionCodeMap[1] = new List<string>(2) {"US", "BS"};
            countryCodeToRegionCodeMap[39] = new List<string>(1) {"IT"};
            countryCodeToRegionCodeMap[44] = new List<string>(1) {"GB"};
            countryCodeToRegionCodeMap[48] = new List<string>(1) {"PL"};
            countryCodeToRegionCodeMap[49] = new List<string>(1) {"DE"};
            countryCodeToRegionCodeMap[52] = new List<string>(1) {"MX"};
            countryCodeToRegionCodeMap[54] = new List<string>(1) {"AR"};
            countryCodeToRegionCodeMap[55] = new List<string>(1) {"BR"};
            countryCodeToRegionCodeMap[61] = new List<string>(1) {"AU"};
            countryCodeToRegionCodeMap[64] = new List<string>(1) {"NZ"};
            countryCodeToRegionCodeMap[65] = new List<string>(1) {"SG"};
            countryCodeToRegionCodeMap[81] = new List<string>(1) {"JP"};
            countryCodeToRegionCodeMap[82] = new List<string>(1) {"KR"};
            countryCodeToRegionCodeMap[244] = new List<string>(1) {"AO"};
            countryCodeToRegionCodeMap[262] = new List<string>(2) {"RE", "YT"};
            countryCodeToRegionCodeMap[376] = new List<string>(1) {"AD"};
            countryCodeToRegionCodeMap[800] = new List<string>(1) {"001"};
            countryCodeToRegionCodeMap[979] = new List<string>(1) {"001"};

            return countryCodeToRegionCodeMap;
        }
    }
}