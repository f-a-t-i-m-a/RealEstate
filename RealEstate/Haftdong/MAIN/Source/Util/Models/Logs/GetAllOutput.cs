using System.Collections.Generic;
using TypeLite;

namespace JahanJooy.RealEstateAgency.Util.Models.Logs
{
    [TsClass]
    public class GetAllOutput
    {
        public List<LogFilesOutput> LogFiles { get; set; }
        public List<LoggersOutput> Loggers { get; set; }
    }
}