using System.Web.Http.ExceptionHandling;

namespace JahanJooy.RealEstateAgency.Naroon.Server.Logging
{
    public class UnhandledExceptionLogger : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            UnhandledExceptionLoggingUtils.Log(context);
        }
    }
}