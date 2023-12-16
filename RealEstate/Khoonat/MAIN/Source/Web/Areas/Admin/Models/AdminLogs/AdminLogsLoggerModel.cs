namespace JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminLogs
{
    public class AdminLogsLoggerModel
    {
        public string Name { get; set; }
        public bool Additivity { get; set; }
        public string EffectiveLevel { get; set; }

        public bool IsDebugEnabled { get; set; }
        public bool AssociatedToDebugAppender { get; set; }
        public bool CanChangeLevelAndAssociation { get; set; }
    }
}