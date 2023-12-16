using log4net;

namespace JahanJooy.RealEstateAgency.Util.Log4Net
{
	public class ApplicationStaticLogs
	{
        public static readonly ILog ErrorDetails = LogManager.GetLogger("ErrorDetails");
        public static readonly ILog AccessLog = LogManager.GetLogger("AccessLog");
        public static readonly ILog ElasticLog = LogManager.GetLogger("ElasticLog");

        public static readonly ILog EmailTx = LogManager.GetLogger("EmailTx");
        public static readonly ILog Farapayamak = LogManager.GetLogger("Farapayamak");

        public static readonly ILog MobileAccessLog = LogManager.GetLogger("MobileAccessLog");
        public static readonly ILog MobileCrashLog = LogManager.GetLogger("MobileCrashLog");

        public static readonly ILog KhoonatServiceCallLog = LogManager.GetLogger("KhoonatServiceCallLog");
        public static readonly ILog ExternalServiceCallLog = LogManager.GetLogger("ExternalServiceCallLog");

    }
}