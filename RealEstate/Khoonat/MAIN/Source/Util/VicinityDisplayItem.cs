using System.Collections.Generic;
using JahanJooy.RealEstate.Util.Resources;

namespace JahanJooy.RealEstate.Util
{
    public class VicinityDisplayItem
    {
        public string VicinityType;
        public string Name;
        public string AlternativeNames;

        public override string ToString()
        {
            var result = "";
            if (!string.IsNullOrWhiteSpace(VicinityType))
                result = VicinityType + " ";

            result += Name;

            if (!string.IsNullOrWhiteSpace(AlternativeNames))
                result += string.Format(GeneralResources.Paranthesis, AlternativeNames);

            return result;
        }

        public static string ToString(IEnumerable<VicinityDisplayItem> items)
        {
            return string.Join(GeneralResources.Comma, items);
        }
    }
}
