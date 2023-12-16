using System.Collections.Generic;

namespace JahanJooy.Common.Util.PhoneNumbers
{
    public class AlternateFormatsCountryCodeSet
    {
        // A set of all country codes for which data is available.
        internal static ISet<int> getCountryCodeSet()
        {
            // The capacity is set to 6 as there are 5 different country codes,
            // and this offers a load factor of roughly 0.75.
            ISet<int> countryCodeSet = new HashSet<int>();

            countryCodeSet.Add(44);
            countryCodeSet.Add(49);
            countryCodeSet.Add(55);
            countryCodeSet.Add(61);
            countryCodeSet.Add(81);

            return countryCodeSet;
        }
        
    }
}