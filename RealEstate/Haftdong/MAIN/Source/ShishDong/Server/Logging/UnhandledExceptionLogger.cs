using System.Web.Http.ExceptionHandling;

namespace JahanJooy.RealEstateAgency.ShishDong.Server.Logging
{
    public class UnhandledExceptionLogger : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            UnhandledExceptionLoggingUtils.Log(context);
        }
    }
}