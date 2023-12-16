namespace JahanJooy.RealEstateAgency.Util.Models.Logs
{
    public class LoggersOutput
    {
        public string Name { get; set; }
        public bool Additivity { get; set; }
        public string EffectiveLevel { get; set; }

        public bool IsDebugEnabled { get; set; }
        public bool AssociatedToDebugAppender { get; set; }
        public bool CanChangeLevelAndAssociation { get; set; }
    }
}