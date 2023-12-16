using System.Collections;
using System.Collections.Generic;

namespace JahanJooy.Common.Util.Tarrif
{
    public static class TarrifCalculationUtil
    {
        public static decimal CalculateSteppingTarrif(decimal input, IEnumerable<SteppingTarrifRow> rows)
        {
            decimal currentMinimum = decimal.MinValue;
            decimal result = decimal.Zero;

            foreach (var row in rows)
            {
                if (row.MinimumInput > currentMinimum && input > row.MinimumInput)
                {
                    currentMinimum = row.MinimumInput;
                    result = row.Result;
                }
            }

            return result;
        }
    }
}