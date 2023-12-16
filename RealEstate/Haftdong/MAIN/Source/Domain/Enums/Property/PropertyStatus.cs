namespace JahanJooy.RealEstateAgency.Domain.Enums.Property
{
    public enum PropertyStatus : byte
    {
        InConstruction = 1,
        NoOccupantYet = 2,
        OccupiedByOwner = 3,
        OccupiedByRenter = 4,
        Emptied = 5
    }
}