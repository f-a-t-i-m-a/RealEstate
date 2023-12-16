using System;

namespace JahanJooy.RealEstateAgency.Util.UserActivityLog
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class SkipUserActivityLoggingAttribute : Attribute
    {
    }
}