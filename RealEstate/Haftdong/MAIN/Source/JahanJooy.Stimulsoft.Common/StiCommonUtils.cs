using System;
using JahanJooy.Stimulsoft.Common.Functions;
using Stimulsoft.Report.Dictionary;

namespace JahanJooy.Stimulsoft.Common
{
    public static class StiCommonUtils
    {
        public static void RegisterCustomFunctions()
        {
            StiFunctions.AddFunction("Jalaali", "JalaaliDateToStr", "Converts a date to Jalaali calendar",
                typeof (StiJalaaliDateFunctions), typeof (string), "", new[] {typeof (DateTime)}, new[] {"value"});
        }
    }
}