using System.Collections.Generic;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminLogs
{
    public class AdminLogsIndexModel
    {
        public List<AdminLogsLogFileModel> LogFiles { get; set; }
        public List<AdminLogsLoggerModel> Loggers { get; set; } 
    }
}