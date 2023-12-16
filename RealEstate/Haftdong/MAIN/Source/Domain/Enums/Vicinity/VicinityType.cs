namespace JahanJooy.RealEstateAgency.Domain.Enums.Vicinity
{
    public enum VicinityType
    {
        // General vicinity node types, for organization in hierarchy
        HierarchyNode = 1000,

        // Political divisions
        Country = 3001,
        CountryPartition = 3002,
        State = 3003,
        StatePartition = 3004,
        Province = 3005,
        ProvincePartition = 3006,
        County = 3007, //SHAHRESTAN
        District = 3008, //BAKHSH
        SubDistrict = 3009, //DEHESTAN

        // City and sub-city divisions
        Metropolis = 4001,
        MetropolisPartition = 4002,
        City = 4003,
        CityPartition = 4004,
        Suburb = 4005,
        Village = 4006,
        Phase = 4007,

        Town = 5001, //SHAHRAK
        CityRegion = 5002,
        Neighborhood = 5003,
        Zone = 5004, //NAAHIYEH

        // Natural landmarks
        Hill = 6001,
        Valley = 6002,
        River = 6003,

        // Man-made landmarks 
        Road = 7001,
        Highway = 7002,
        Boulevard = 7003,
        Bridge = 7005,
        Street = 7006,
        Alley = 7007,

        Square = 7101,
        Crossing = 7102,
        FourWay = 7103,
        Tee = 7104,
        Exit = 7105,

        Airport = 8001,
        Station = 8002,
        Park = 8003,

        Landmark = 8501,

        // Well-known buildings and complexes
        Campus = 9001,
        Premises = 9002,
        Complex = 9003,
        Block = 9004,
        Tower = 9005,
        Building = 9006,
    }
}