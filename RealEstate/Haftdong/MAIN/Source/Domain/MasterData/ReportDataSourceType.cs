namespace JahanJooy.RealEstateAgency.Domain.MasterData
{
    public enum ReportDataSourceType : byte
    {
        /// <summary>
        /// The report template includes a direct connection to the database, and defines SQL queries to retrieve data
        /// </summary>
        DirectDbConnection = 1,

        /// <summary>
        /// Report-specific implementation is included in the application to prepare the data required for the 
        /// report in server-side code.
        /// Such reports are only invoked in the related use cases and are not available in the list of reports
        /// </summary>
        ApplicationImplemented = 9
    }
}