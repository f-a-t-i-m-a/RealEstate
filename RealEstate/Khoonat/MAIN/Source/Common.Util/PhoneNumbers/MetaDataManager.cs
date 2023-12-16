using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using JahanJooy.Common.Util.PhoneNumbers.Data;

namespace JahanJooy.Common.Util.PhoneNumbers
{
    /**
     * Class encapsulating loading of PhoneNumber Metadata information. Currently this is used only for
     * additional data files such as PhoneNumberAlternateFormats, but in the future it is envisaged it
     * would handle the main metadata file (PhoneNumberMetaData.xml) as well.
     *
     * @author Lara Rennie
     */

    internal class MetaDataManager
    {
        private const string ResourceName = "JahanJooy.Common.Util.PhoneNumbers.Data.PhoneNumberAlternateFormats.xml";

        private static readonly IDictionary<int, PhoneMetaData> CallingCodeToAlternateFormatsMap = new ConcurrentDictionary<int, PhoneMetaData>();

        // A set of which country calling codes there are alternate format data for. If the set has an
        // entry for a code, then there should be data for that code linked into the resources.
        private static readonly ISet<int> CountryCodeSet = AlternateFormatsCountryCodeSet.getCountryCodeSet();

        private MetaDataManager()
        {
        }

        private static void LoadMetadataFromFile()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName))
            {
                var metaDataCollection = PhoneNumberMetaDataReader.BuildMetaDataCollection(stream, false);
                foreach (PhoneMetaData metadata in metaDataCollection.getMetadataList())
                {
                    CallingCodeToAlternateFormatsMap[metadata.CountryCode] = metadata;
                }
            }
        }

        public static PhoneMetaData getAlternateFormatsForCountry(int countryCallingCode)
        {
            if (!CountryCodeSet.Contains(countryCallingCode))
            {
                return null;
            }

            lock(CallingCodeToAlternateFormatsMap)
            {
                if (!CallingCodeToAlternateFormatsMap.ContainsKey(countryCallingCode))
                {
                    LoadMetadataFromFile();
                }
            }

            return CallingCodeToAlternateFormatsMap[countryCallingCode];
        }
    }

}