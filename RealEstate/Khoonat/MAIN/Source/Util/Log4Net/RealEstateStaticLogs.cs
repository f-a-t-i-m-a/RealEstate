using log4net;

namespace JahanJooy.RealEstate.Util.Log4Net
{
    public static class RealEstateStaticLogs
    {
        public static readonly ILog SavedSearch = LogManager.GetLogger("SavedSearch");
        public static readonly ILog Farapayamak = LogManager.GetLogger("Farapayamak");
		public static readonly ILog EmailTx = LogManager.GetLogger("EmailTx");
		public static readonly ILog MobileCrashLog = LogManager.GetLogger("MobileCrashLog");
		public static readonly ILog VicinitySearch = LogManager.GetLogger("VicinitySearch");

		public static readonly ILog ApiInvoked = LogManager.GetLogger("ApiInvoked");
		public static readonly ILog ApiFiltered = LogManager.GetLogger("ApiFiltered");

		public static readonly ILog IpgLogger = LogManager.GetLogger("IpgLogger");
		public static readonly ILog PasargadPaymentGatewayLogger = LogManager.GetLogger("PasargadPaymentGatewayLogger");


    }
}