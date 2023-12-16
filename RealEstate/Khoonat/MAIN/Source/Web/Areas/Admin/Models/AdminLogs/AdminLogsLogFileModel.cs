using System;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminLogs
{
    public class AdminLogsLogFileModel
    {
        public string Name { get; set; } 
        public long Size { get; set; }

        public DateTime CreationTime { get; set; }
        public DateTime LastWriteTime { get; set; }

        public bool CanBeDeleted { get; set; }
        public bool CanBeArchived { get; set; }
    }
}