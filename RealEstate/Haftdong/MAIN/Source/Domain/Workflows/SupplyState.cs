namespace JahanJooy.RealEstateAgency.Domain.Workflows
{
    public enum SupplyState : byte
    {
        New = 1,
        Reserved = 10,
        Completed = 200,
        Canceled = 210,
        Deleted = 250
    }
}