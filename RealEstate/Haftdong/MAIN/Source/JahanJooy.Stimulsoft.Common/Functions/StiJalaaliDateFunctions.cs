using System;
using System.Globalization;

namespace JahanJooy.Stimulsoft.Common.Functions
{
    public static class StiJalaaliDateFunctions
    {
        public static string JalaaliDateToStr(DateTime? value)
        {
            if (!value.HasValue)
                return "";
            var cal = new PersianCalendar();
            return $"{cal.GetYear(value.Value)}/{cal.GetMonth(value.Value)}/{cal.GetDayOfMonth(value.Value)}";
        }
    }
}