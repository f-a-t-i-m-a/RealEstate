module MongoDB.Bson {
    export enum BsonType {
        EndOfDocument = 0,
        Double = 1,
        String = 2,
        Document = 3,
        Array = 4,
        Binary = 5,
        Undefined = 6,
        ObjectId = 7,
        Boolean = 8,
        DateTime = 9,
        Null = 10,
        RegularExpression = 11,
        JavaScript = 13,
        Symbol = 14,
        JavaScriptWithScope = 15,
        Int32 = 16,
        Timestamp = 17,
        Int64 = 18,
        MinKey = 255,
        MaxKey = 127
    }
    export enum GuidRepresentation {
        Unspecified = 0,
        Standard = 1,
        CSharpLegacy = 2,
        JavaLegacy = 3,
        PythonLegacy = 4
    }
    export enum BsonBinarySubType {
        Binary = 0,
        Function = 1,
        OldBinary = 2,
        UuidLegacy = 3,
        UuidStandard = 4,
        MD5 = 5,
        UserDefined = 128
    }
}
module MongoDB.Driver.GeoJsonObjectModel {
    export enum GeoJsonObjectType {
        Feature = 0,
        FeatureCollection = 1,
        GeometryCollection = 2,
        LineString = 3,
        MultiLineString = 4,
        MultiPoint = 5,
        MultiPolygon = 6,
        Point = 7,
        Polygon = 8
    }
}
module System.Text.RegularExpressions {
    export enum RegexOptions {
        None = 0,
        IgnoreCase = 1,
        Multiline = 2,
        ExplicitCapture = 4,
        Compiled = 8,
        Singleline = 16,
        IgnorePatternWhitespace = 32,
        RightToLeft = 64,
        ECMAScript = 256,
        CultureInvariant = 512
    }
}
module JahanJooy.RealEstateAgency.Util.Models.Users {
    export enum ApplicationUserSortColumn {
        CreationTime = 0,
        LicenseActivationTime = 1
    }
}
module JahanJooy.RealEstateAgency.Util.Models.Base {
    export enum SortDirectionType {
        Desc = 1,
        Asc = 2
    }
}
module JahanJooy.RealEstateAgency.Util.Models.UserActivities {
    export enum UserActivitySortColumn {
        ActivityTime = 0
    }
}
module JahanJooy.RealEstateAgency.Util.Models.Supplies {
    export enum SupplySortColumn {
        CreationTime = 0,
        TotalPrice = 1
    }
}
module JahanJooy.RealEstateAgency.Util.Models.Properties {
    export enum PropertySortColumn {
        CreationTime = 0
    }
}
module JahanJooy.RealEstateAgency.Util.Models.Requests {
    export enum RequestSortColumn {
        PropertyTypes = 0,
        UsageType = 1,
        IntentionOfCustomer = 2,
        DisplayName = 3,
        TotalPrice = 4,
        Rent = 5,
        Mortgage = 6
    }
}
module JahanJooy.RealEstateAgency.Util.Models.Customers {
    export enum CustomerSortColumn {
        LastVisitTime = 0
    }
}
module JahanJooy.RealEstateAgency.Domain.MasterData {
    export enum ParameterType {
        String = 1,
        DateTime = 2
    }
    export enum ReportDataSourceType {
        DirectDbConnection = 1,
        ApplicationImplemented = 9
    }
    export enum ApplicationImplementedReportDataSourceType {
        Properties = 1,
        Requests = 2,
        Customers = 3,
        Users = 4,
        Supplies = 5,
        Contracts = 6,
        Property = 41,
        Request = 42,
        Customer = 43,
        User = 44,
        Supply = 45,
        Contract = 46,
        Dashboard = 80
    }
}
module JahanJooy.RealEstateAgency.Util.Models.Dashboard {
    export enum DashboardSortColumn {
        CreationTime = 0,
        ModificationTime = 1
    }
}
module JahanJooy.RealEstateAgency.Util.Models.Contracts {
    export enum ContractSortColumn {
        ContractDate = 0
    }
}
module JahanJooy.RealEstateAgency.Domain.Enums.Vicinity {
    export enum VicinityType {
        HierarchyNode = 1000,
        Country = 3001,
        CountryPartition = 3002,
        State = 3003,
        StatePartition = 3004,
        Province = 3005,
        ProvincePartition = 3006,
        County = 3007,
        District = 3008,
        SubDistrict = 3009,
        Metropolis = 4001,
        MetropolisPartition = 4002,
        City = 4003,
        CityPartition = 4004,
        Suburb = 4005,
        Village = 4006,
        Phase = 4007,
        Town = 5001,
        CityRegion = 5002,
        Neighborhood = 5003,
        Zone = 5004,
        Hill = 6001,
        Valley = 6002,
        River = 6003,
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
        Campus = 9001,
        Premises = 9002,
        Complex = 9003,
        Block = 9004,
        Tower = 9005,
        Building = 9006
    }
}
module JahanJooy.RealEstateAgency.Domain.Enums.Property {
    export enum IntentionOfOwner {
        ForRent = 1,
        ForSale = 2,
        ForFullMortgage = 3,
        ForDailyRent = 4,
        ForSwap = 5,
        ForCooperation = 6
    }
    export enum SalePriceSpecificationType {
        Total = 1,
        PerEstateArea = 2,
        PerUnitArea = 3
    }
    export enum PropertyType {
        Suite = 200,
        Apartment = 201,
        Penthouse = 202,
        Villa = 102,
        House = 101,
        Complex = 4,
        Tower = 5,
        GardenTower = 6,
        Garden = 2,
        Land = 1,
        Tenement = 3,
        OldHouse = 103,
        Office = 104,
        OfficialResidency = 106,
        Shop = 203,
        Commercial = 205,
        CommercialResidency = 206,
        Shed = 207,
        AgriculturalLand = 208,
        Factory = 209,
        WorkShop = 210,
        RepairShop = 211,
        StoreHouse = 212,
        Parking = 213,
        Gym = 214,
        CityService = 215
    }
    export enum UsageType {
        Residency = 1,
        Office = 3,
        Shop = 4,
        Agricultural = 5,
        Industrial = 6,
        Mixed = 7,
        Other = 8,
        Unknown = 9
    }
    export enum SourceType {
        Haftdong = 1,
        Khoonat = 2,
        Other = 3,
        Sheshdong = 4
    }
    export enum PropertyStatus {
        InConstruction = 1,
        NoOccupantYet = 2,
        OccupiedByOwner = 3,
        OccupiedByRenter = 4,
        Emptied = 5
    }
    export enum EstateDirection {
        North = 1,
        South = 2,
        NorthAndSouth = 3,
        Other = 4
    }
    export enum EstateVoucherType {
        Normal = 1,
        BreakDownVoucher = 2,
        Owghaf = 3,
        Cooperative = 4,
        Other = 99,
        WithoutVoucher = 100
    }
    export enum KitchenCabinetType {
        Mdf = 1,
        Wooden = 2,
        Metal = 3,
        MdfAndStone = 4,
        WoodenAndMetal = 5,
        Hbf = 6,
        Hdf = 7,
        Hpl = 8,
        Pvc = 9,
        StoneAndWooden = 10,
        FiberGlass = 11,
        MetalAndGlass = 12,
        Formica = 13,
        HighGlass = 14,
        Mixed = 15,
        Other = 99
    }
    export enum DaylightDirection {
        South = 1,
        North = 2,
        East = 3,
        West = 4,
        ViaPatio = 5
    }
    export enum FloorCoverType {
        Mosaic = 1,
        Ceramic = 2,
        Stone = 3,
        Parquet = 4,
        Moquette = 5,
        Laminate = 6,
        Other = 99
    }
    export enum BuildingFaceType {
        Stone = 1,
        Brick = 2,
        Concrete = 3,
        CompositePanel = 4,
        Wood = 5,
        Glass = 6,
        StoneAndBrick = 7,
        Other = 99
    }
}
module JahanJooy.RealEstateAgency.Domain.Workflows {
    export enum SupplyState {
        New = 1,
        Reserved = 10,
        Completed = 200,
        Canceled = 210,
        Deleted = 250
    }
    export enum PropertyState {
        New = 1,
        Deleted = 250
    }
    export enum SupplyCompletionReason {
        Contracted = 11,
        DoneByOthers = 21
    }
    export enum RequestState {
        New = 1,
        Compelete = 200,
        Deleted = 250
    }
    export enum ContractState {
        Agreement = 1,
        Contracted = 10,
        Cancellation = 200
    }
}
module JahanJooy.RealEstateAgency.Domain.Enums.User {
    export enum LicenseType {
        Trial = 10,
        SingleUser = 20
    }
    export enum UserType {
        NormalUser = 1,
        IndependentAgent = 2,
        IndependentAgencyMember = 3,
        AgencyAgent = 4,
        Administrator = 99
    }
    export enum ContactMethodType {
        Phone = 1,
        Email = 2,
        Address = 3
    }
    export enum UserGeneralPermission {
        AccessAdminMenu = 9000,
        ManageUsers = 9001
    }
    export enum BuiltInRole {
        Administrator = 1,
        RealEstateAgent = 10,
        VerifiedUser = 200
    }
}
module JahanJooy.RealEstateAgency.Domain.Enums.UserActivity {
    export enum ApplicationType {
        Haftdong = 1,
        Sheshdong = 4
    }
    export enum EntityType {
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
    export enum UserActivityType {
        Unknown = 0,
        View = 1,
        Print = 2,
        Create = 11,
        Edit = 12,
        Delete = 13,
        ChangeState = 16,
        Enable = 21,
        Disable = 22,
        Publish = 23,
        Unpublish = 24,
        Approve = 31,
        Reject = 32,
        Reverse = 33,
        Export = 41,
        Import = 42,
        Other = 99,
        ViewDetail = 101,
        PrintDetail = 102,
        CreateDetail = 111,
        EditDetail = 112,
        DeleteDetail = 113,
        ChangeStateOfDetail = 116,
        EnableDetail = 121,
        DisableDetail = 122,
        PublishDetail = 123,
        UnpublishDetail = 124,
        ApproveDetail = 131,
        RejectDetail = 132,
        ExportDetail = 141,
        ImportDetail = 142,
        OtherDetail = 199,
        Authenticate = 200,
        Comment = 250
    }
}
module JahanJooy.RealEstateAgency.Domain.Enums.Request {
    export enum IntentionOfCustomer {
        ForRent = 1,
        ForBuy = 2,
        ForFullMortgage = 3,
        ForDailyRent = 4,
        ForSwap = 5,
        ForCooperation = 6
    }
}

