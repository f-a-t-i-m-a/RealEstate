using JahanJooy.Common.Util.Text;

namespace JahanJooy.RealEstateAgency.Util.Resources
{
    public static class NumberUtil
    {
        public static int? FullyTextualOrdinalNumber(string strNumber)
        {
            if (strNumber.Equals("جهارم"))
            {
                strNumber = strNumber.Replace("ج", "چ");
            }

            if (strNumber.Equals("زيرزمين"))
            {
                return -1;
            }

            if (strNumber.Equals("همکف"))
            {
                return 0;
            }

            for (int i = 1; i < 100; i++)//calculates up to 100 floors
            {
                if (strNumber.Equals(NumericStringUtils.FullyTextualOrdinalNumber(i)))
                {
                    return i;
                }
            }

            return null;
        }
    }
}