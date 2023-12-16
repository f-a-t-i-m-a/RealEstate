namespace JahanJooy.RealEstateAgency.Domain.Enums.UserActivity
{
    public enum EntityType : byte
    {
        ReportTemplate = 11,
        ConfigurationDataItem = 12,

        ApplicationUser = 31,
        ApplicationRole = 32,
        BuiltInRole = 36,
        Customer = 37,
        CustomerContactInfo = 38,

        Property = 41,
        Contract = 42,
        Request = 43,
        Supply = 44,
        
        Agency = 61,
        AgencyBranch = 62,
        AgencyBranchContent = 63,
        AgencyContent = 64,

        PhotoInfo = 71,

        Vicinity = 81,

        ChangeHistory = 91,
        ChangeHistoryChangeType = 92,
        UserActivity = 93,
        UserActivityType = 94,
        Index = 95,

        Unknown = 250
    }
}