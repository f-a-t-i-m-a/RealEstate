namespace JahanJooy.RealEstate.Core.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Province",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 80),
                        IsCapital = c.Boolean(nullable: false),
                        Importance = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.City",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 80),
                        IsCountryCapital = c.Boolean(nullable: false),
                        IsProvinceCapital = c.Boolean(nullable: false),
                        Importance = c.Int(nullable: false),
                        ProvinceID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Province", t => t.ProvinceID)
                .Index(t => t.ProvinceID);
            
            CreateTable(
                "dbo.Neighborhood",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 80),
                        Importance = c.Int(nullable: false),
                        CityID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.City", t => t.CityID)
                .Index(t => t.CityID);
            
            CreateTable(
                "dbo.CityRegion",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 80),
                        Importance = c.Int(nullable: false),
                        CityID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.City", t => t.CityID)
                .Index(t => t.CityID);
            
            CreateTable(
                "dbo.PropertyListing",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Code = c.Long(nullable: false),
                        DeleteDate = c.DateTime(),
                        Approved = c.Boolean(),
                        PropertyType = c.Byte(nullable: false),
                        IntentionOfOwner = c.Byte(nullable: false),
                        ProvinceID = c.Long(),
                        CityID = c.Long(),
                        CityRegionID = c.Long(),
                        NeighborhoodID = c.Long(),
                        EstateID = c.Long(),
                        BuildingID = c.Long(),
                        UnitID = c.Long(),
                        SaleConditionsID = c.Long(),
                        RentConditionsID = c.Long(),
                        ContactInfoID = c.Long(),
                        AppropriateVisitTimes = c.String(maxLength: 500),
                        InappropriateVisitTimes = c.String(maxLength: 500),
                        ShouldCoordinateBeforeVisit = c.Boolean(nullable: false),
                        HowToCoordinateBeforeVisit = c.String(maxLength: 500),
                        PropertyStatus = c.Byte(),
                        RenterContractEndDate = c.DateTime(),
                        DeliveryDateSpecificationType = c.Byte(),
                        AbsoluteDeliveryDate = c.DateTime(),
                        DeliveryDaysAfterContract = c.Int(),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                        PublishDate = c.DateTime(),
                        PublishEndDate = c.DateTime(),
                        EditPassword = c.String(maxLength: 20),
                        CreatorSessionID = c.Long(nullable: false),
                        CreatorUserID = c.Long(),
                        OwnerUserID = c.Long(),
                        NumberOfVisits = c.Long(nullable: false),
                        NumberOfSearches = c.Long(nullable: false),
                        NormalizedPrice = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Province", t => t.ProvinceID)
                .ForeignKey("dbo.City", t => t.CityID)
                .ForeignKey("dbo.CityRegion", t => t.CityRegionID)
                .ForeignKey("dbo.Neighborhood", t => t.NeighborhoodID)
                .ForeignKey("dbo.Estate", t => t.EstateID)
                .ForeignKey("dbo.Building", t => t.BuildingID)
                .ForeignKey("dbo.Unit", t => t.UnitID)
                .ForeignKey("dbo.SaleConditions", t => t.SaleConditionsID)
                .ForeignKey("dbo.RentConditions", t => t.RentConditionsID)
                .ForeignKey("dbo.ContactInfo", t => t.ContactInfoID)
                .ForeignKey("dbo.HttpSession", t => t.CreatorSessionID)
                .ForeignKey("dbo.User", t => t.CreatorUserID)
                .ForeignKey("dbo.User", t => t.OwnerUserID)
                .Index(t => t.ProvinceID)
                .Index(t => t.CityID)
                .Index(t => t.CityRegionID)
                .Index(t => t.NeighborhoodID)
                .Index(t => t.EstateID)
                .Index(t => t.BuildingID)
                .Index(t => t.UnitID)
                .Index(t => t.SaleConditionsID)
                .Index(t => t.RentConditionsID)
                .Index(t => t.ContactInfoID)
                .Index(t => t.CreatorSessionID)
                .Index(t => t.CreatorUserID)
                .Index(t => t.OwnerUserID);
            
            CreateTable(
                "dbo.Estate",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        ProvinceID = c.Long(),
                        CityID = c.Long(),
                        CityRegionID = c.Long(),
                        NeighborhoodID = c.Long(),
                        Street1 = c.String(maxLength: 100),
                        Street2 = c.String(maxLength: 100),
                        Alley1 = c.String(maxLength: 100),
                        Alley2 = c.String(maxLength: 100),
                        PlateNumber = c.String(maxLength: 20),
                        AdditionalAddressInfo = c.String(maxLength: 1000),
                        Area = c.Decimal(precision: 10, scale: 3),
                        Direction = c.Byte(),
                        VoucherType = c.Byte(),
                        Sides = c.Int(),
                        PassageEdgeLength = c.Decimal(precision: 6, scale: 2),
                        PassageWidth = c.Decimal(precision: 5, scale: 2),
                        IsInDeadEnd = c.Boolean(),
                        IsCloseToHighway = c.Boolean(),
                        IsOnMainPassage = c.Boolean(),
                        IsOnGreenPassage = c.Boolean(),
                        IsCloseToPark = c.Boolean(),
                        SlopeAmount = c.Byte(),
                        SurfaceType = c.Byte(),
                        HasElectricity = c.Boolean(),
                        HasThreePhaseElectricity = c.Boolean(),
                        HasIndustrialElectricity = c.Boolean(),
                        HasDrinkingWater = c.Boolean(),
                        HasCultivationWater = c.Boolean(),
                        HasGasPiping = c.Boolean(),
                        HasSewerExtension = c.Boolean(),
                        HasWaterWells = c.Boolean(),
                        HasWaterWellsPrivilege = c.Boolean(),
                        HasNiches = c.Boolean(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Province", t => t.ProvinceID)
                .ForeignKey("dbo.City", t => t.CityID)
                .ForeignKey("dbo.CityRegion", t => t.CityRegionID)
                .ForeignKey("dbo.Neighborhood", t => t.NeighborhoodID)
                .Index(t => t.ProvinceID)
                .Index(t => t.CityID)
                .Index(t => t.CityRegionID)
                .Index(t => t.NeighborhoodID);
            
            CreateTable(
                "dbo.Building",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        TotalNumberOfUnits = c.Int(),
                        NumberOfUnitsPerFloor = c.Int(),
                        NumberFloorsAboveGround = c.Int(),
                        NumberOfParkingFloors = c.Int(),
                        HasJanitorUnit = c.Boolean(),
                        HasEntranceLobby = c.Boolean(),
                        FaceType = c.Byte(),
                        StructureType = c.Byte(),
                        RoofCoverType = c.Byte(),
                        BuildingAgeYears = c.Int(),
                        HasEndOfConstructionCertificate = c.Boolean(),
                        PermitDate = c.DateTime(),
                        ConstructionCompletionDate = c.DateTime(),
                        EndOfConstructionCertificateDate = c.DateTime(),
                        HasElevator = c.Boolean(),
                        HasFurnitureElevator = c.Boolean(),
                        HasCentralTelevisionAntenna = c.Boolean(),
                        HasCentralInternetConnection = c.Boolean(),
                        HasGuestParking = c.Boolean(),
                        HasTrashChute = c.Boolean(),
                        HasCentralBoiler = c.Boolean(),
                        HasCentralChiller = c.Boolean(),
                        HasCentralVacuum = c.Boolean(),
                        HasGatheringHall = c.Boolean(),
                        OtherFeatures = c.String(maxLength: 1000),
                        HasAutomaticParkingDoor = c.Boolean(),
                        HasVideoEyePhone = c.Boolean(),
                        HasCarWash = c.Boolean(),
                        HasBackupWaterTank = c.Boolean(),
                        HasAutomaticStairwayLights = c.Boolean(),
                        HasAccessibilityFeatures = c.Boolean(),
                        OtherWelfareFeatures = c.String(maxLength: 1000),
                        HasGreenGarden = c.Boolean(),
                        HasBowerInGarden = c.Boolean(),
                        HasBowerOnRoof = c.Boolean(),
                        HasSwimmingPool = c.Boolean(),
                        HasSauna = c.Boolean(),
                        HasJacuzzi = c.Boolean(),
                        HasGym = c.Boolean(),
                        OtherRecreationFeatures = c.String(maxLength: 1000),
                        HasFireAlarm = c.Boolean(),
                        HasEmergencyEscape = c.Boolean(),
                        HasClosedCircuitCamera = c.Boolean(),
                        HasGuardian = c.Boolean(),
                        HasQuakeInsurance = c.Boolean(),
                        HasFireInsurance = c.Boolean(),
                        HasLightningArrester = c.Boolean(),
                        OtherSafetyFeatures = c.String(maxLength: 1000),
                        EstateID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Estate", t => t.EstateID)
                .Index(t => t.EstateID);
            
            CreateTable(
                "dbo.Unit",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        BlockNumber = c.String(maxLength: 20),
                        FloorNumber = c.Int(),
                        FlatNumber = c.String(maxLength: 20),
                        AdditionalAddressInfo = c.String(maxLength: 1000),
                        NumberOfRooms = c.Int(),
                        NumberOfParkings = c.Int(),
                        NumberOfLavatories = c.Int(),
                        NumberOfBathrooms = c.Int(),
                        NumberOfKitchens = c.Int(),
                        NumberOfClosets = c.Int(),
                        NumberOfBalconies = c.Int(),
                        UsageType = c.Byte(),
                        Area = c.Decimal(precision: 10, scale: 3),
                        KitchenArea = c.Decimal(precision: 7, scale: 3),
                        BalconyArea = c.Decimal(precision: 7, scale: 3),
                        StorageRoomArea = c.Decimal(precision: 7, scale: 3),
                        CurrentMonthlyChargeK = c.Decimal(precision: 9, scale: 3),
                        IsDuplex = c.Boolean(),
                        IsFurnished = c.Boolean(),
                        HasHalfFloorInside = c.Boolean(),
                        HasIranianLavatory = c.Boolean(),
                        HasForeignLavatory = c.Boolean(),
                        HasSeparateLivingRoom = c.Boolean(),
                        HasPrivatePatio = c.Boolean(),
                        HasPrivateGarden = c.Boolean(),
                        HasSecuritySystem = c.Boolean(),
                        HasOpenKitchen = c.Boolean(),
                        HasFurnishedKitchen = c.Boolean(),
                        KitchenCabinetTopType = c.Byte(),
                        KitchenCabinetBodyType = c.Byte(),
                        AdditionalSpecialFeatures = c.String(maxLength: 1000),
                        MainDaylightDirection = c.Byte(),
                        LivingRoomFloor = c.Byte(),
                        LivingRoomWalls = c.Byte(),
                        RoomsFloor = c.Byte(),
                        RoomsWall = c.Byte(),
                        WindowType = c.Byte(),
                        HeatingSystem = c.Byte(),
                        HasIndependentHeatingPackage = c.Boolean(),
                        CoolingSystem = c.Byte(),
                        HasIndependentCoolingPackage = c.Boolean(),
                        HasFireplace = c.Boolean(),
                        WindowsHasDoubleGlass = c.Boolean(),
                        HasInsulatedWalls = c.Boolean(),
                        HasIndependentGasometer = c.Boolean(),
                        AdditionalHeatingAndCoolingInformation = c.String(maxLength: 1000),
                        HasBeenReconstructed = c.Boolean(),
                        NeedsDestruction = c.Boolean(),
                        NeedsReconstruction = c.Boolean(),
                        NeedsPainting = c.Boolean(),
                        NumberOfMasterBedrooms = c.Int(),
                        CeilingHeight = c.Decimal(precision: 4, scale: 2),
                        HasAllSideView = c.Boolean(),
                        HasAllSideBalcony = c.Boolean(),
                        HasPrivatePool = c.Boolean(),
                        HasPrivateElevator = c.Boolean(),
                        HasInUnitParking = c.Boolean(),
                        HasOpenningCeiling = c.Boolean(),
                        HasMobileWall = c.Boolean(),
                        HasGardenInBalcony = c.Boolean(),
                        HasRemoteControlledCurtains = c.Boolean(),
                        HasBuildingManagementSystem = c.Boolean(),
                        HasPrivateJanitorUnit = c.Boolean(),
                        HasGuestSuite = c.Boolean(),
                        AdditionalLuxuryFeatures = c.String(maxLength: 1000),
                        BuildingID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Building", t => t.BuildingID)
                .Index(t => t.BuildingID);
            
            CreateTable(
                "dbo.SaleConditions",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        PriceSpecificationType = c.Byte(),
                        PriceM = c.Decimal(precision: 14, scale: 6),
                        PricePerEstateAreaK = c.Decimal(precision: 10, scale: 3),
                        PricePerUnitAreaK = c.Decimal(precision: 10, scale: 3),
                        PaymentPercentForContract = c.Int(),
                        PaymentPercentForDelivery = c.Int(),
                        CanHaveDebt = c.Boolean(nullable: false),
                        PaymentPercentForDebt = c.Int(),
                        MinimumMonthlyPaymentForDebtK = c.Decimal(precision: 10, scale: 3),
                        DebtGuaranteeType = c.Byte(),
                        HasTransferableLoan = c.Boolean(nullable: false),
                        TransferableLoanAmountM = c.Decimal(precision: 14, scale: 6),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.RentConditions",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        MortgageM = c.Decimal(precision: 12, scale: 6),
                        RentK = c.Decimal(precision: 12, scale: 3),
                        MortgageAndRentConvertible = c.Boolean(nullable: false),
                        MinimumMortgageM = c.Decimal(precision: 12, scale: 6),
                        MinimumRentK = c.Decimal(precision: 12, scale: 3),
                        IsDischargeGuaranteeChequeRequired = c.Boolean(),
                        DiscountOnBulkPayment = c.Boolean(),
                        MinimumContractDuration = c.Byte(),
                        MaximumContractDuration = c.Byte(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.ContactInfo",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        ContactRole = c.Byte(),
                        AgencyName = c.String(maxLength: 80),
                        AgencyAddress = c.String(maxLength: 200),
                        ContactName = c.String(maxLength: 80),
                        ContactPhone1 = c.String(maxLength: 25),
                        ContactPhone2 = c.String(maxLength: 25),
                        ContactEmail = c.String(maxLength: 100),
                        OwnerCanBeContacted = c.Boolean(nullable: false),
                        OwnerName = c.String(maxLength: 80),
                        OwnerPhone1 = c.String(maxLength: 25),
                        OwnerPhone2 = c.String(maxLength: 25),
                        OwnerEmail = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.HttpSession",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Start = c.DateTime(nullable: false),
                        End = c.DateTime(),
                        EndReason = c.Byte(),
                        UserAgent = c.String(maxLength: 200),
                        StartupUri = c.String(maxLength: 2000),
                        ReferrerUri = c.String(maxLength: 2000),
                        ClientAddress = c.String(maxLength: 50),
                        HttpSessionID = c.String(maxLength: 50),
                        PrevHttpSessionID = c.String(maxLength: 50),
                        UserID = c.Long(),
                        UniqueVisitorID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.User", t => t.UserID)
                .ForeignKey("dbo.UniqueVisitor", t => t.UniqueVisitorID)
                .Index(t => t.UserID)
                .Index(t => t.UniqueVisitorID);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Code = c.Long(nullable: false),
                        IsOperator = c.Boolean(nullable: false),
                        LoginName = c.String(nullable: false, maxLength: 80),
                        PasswordHash = c.String(nullable: false, maxLength: 50),
                        PasswordSalt = c.String(nullable: false, maxLength: 20),
                        IsEnabled = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                        CreatorSessionID = c.Long(nullable: false),
                        LastLogin = c.DateTime(),
                        LastLoginAttempt = c.DateTime(),
                        FailedLoginAttempts = c.Int(nullable: false),
                        DisplayName = c.String(nullable: false, maxLength: 80),
                        FullName = c.String(nullable: false, maxLength: 80),
                        Email = c.String(maxLength: 120),
                        Phone1 = c.String(maxLength: 25),
                        Phone2 = c.String(maxLength: 25),
                        VerificationMethod = c.Byte(),
                        VerificationContactID = c.Long(),
                        GeneralPermissionsValue = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.HttpSession", t => t.CreatorSessionID)
                .ForeignKey("dbo.UserContactMethod", t => t.VerificationContactID)
                .Index(t => t.CreatorSessionID)
                .Index(t => t.VerificationContactID);
            
            CreateTable(
                "dbo.UserContactMethod",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        UserID = c.Long(nullable: false),
                        ContactMethodType = c.Byte(nullable: false),
                        ContactMethodText = c.String(nullable: false, maxLength: 150),
                        Visibility = c.Byte(nullable: false),
                        IsVerified = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.User", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.UniqueVisitor",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        UniqueIdentifier = c.String(nullable: false, maxLength: 40),
                        CreationDate = c.DateTime(nullable: false),
                        LastVisitDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.PropertyListingUpdateHistory",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        UpdateDate = c.DateTime(nullable: false),
                        UpdateDetails = c.String(),
                        PropertyListingID = c.Long(nullable: false),
                        SessionID = c.Long(nullable: false),
                        UserID = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.PropertyListing", t => t.PropertyListingID, cascadeDelete: true)
                .ForeignKey("dbo.HttpSession", t => t.SessionID)
                .ForeignKey("dbo.User", t => t.UserID)
                .Index(t => t.PropertyListingID)
                .Index(t => t.SessionID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.PropertyListingPublishHistory",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        PublishDate = c.DateTime(nullable: false),
                        PublishDays = c.Int(nullable: false),
                        PreviousPublishDate = c.DateTime(),
                        PreviousPublishEndDate = c.DateTime(),
                        PropertyListingID = c.Long(nullable: false),
                        SessionID = c.Long(nullable: false),
                        UserID = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.PropertyListing", t => t.PropertyListingID, cascadeDelete: true)
                .ForeignKey("dbo.HttpSession", t => t.SessionID)
                .ForeignKey("dbo.User", t => t.UserID)
                .Index(t => t.PropertyListingID)
                .Index(t => t.SessionID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.PropertySearchHistory",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        SearchDate = c.DateTime(nullable: false),
                        UrlPath = c.String(maxLength: 2000),
                        RequestedPage = c.Int(nullable: false),
                        SessionID = c.Long(nullable: false),
                        UserID = c.Long(),
                        SortOrder = c.Byte(),
                        PropertyType = c.Byte(),
                        IntentionOfOwner = c.Byte(),
                        ProvinceID = c.Long(),
                        CityID = c.Long(),
                        CityRegionID = c.Long(),
                        NeighborhoodID = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.HttpSession", t => t.SessionID)
                .ForeignKey("dbo.User", t => t.UserID)
                .Index(t => t.SessionID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.ActivityLog",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        LogDate = c.DateTime(nullable: false),
                        ApprovalDate = c.DateTime(),
                        ReviewDate = c.DateTime(),
                        ReviewWeight = c.Double(nullable: false),
                        Action = c.Byte(nullable: false),
                        ActionDetails = c.String(nullable: false, maxLength: 80),
                        ActionSucceeded = c.Boolean(),
                        SessionID = c.Long(),
                        AuthenticatedUserID = c.Long(),
                        ReviewedByID = c.Long(),
                        ParentEntity = c.Byte(),
                        TargetEntity = c.Byte(nullable: false),
                        DetailEntity = c.Byte(),
                        AuditEntity = c.Byte(),
                        ParentEntityID = c.Long(),
                        TargetEntityID = c.Long(),
                        DetailEntityID = c.Long(),
                        AuditEntityID = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.HttpSession", t => t.SessionID)
                .ForeignKey("dbo.User", t => t.AuthenticatedUserID)
                .ForeignKey("dbo.User", t => t.ReviewedByID)
                .Index(t => t.SessionID)
                .Index(t => t.AuthenticatedUserID)
                .Index(t => t.ReviewedByID);
            
            CreateTable(
                "dbo.UserContactMethodVerification",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        SessionID = c.Long(nullable: false),
                        TargetUserID = c.Long(nullable: false),
                        TargetContactMethodID = c.Long(nullable: false),
                        StartTime = c.DateTime(nullable: false),
                        ExpirationTime = c.DateTime(nullable: false),
                        CompletionTime = c.DateTime(),
                        VerificationSecret = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.HttpSession", t => t.SessionID)
                .ForeignKey("dbo.User", t => t.TargetUserID)
                .ForeignKey("dbo.UserContactMethod", t => t.TargetContactMethodID, cascadeDelete: true)
                .Index(t => t.SessionID)
                .Index(t => t.TargetUserID)
                .Index(t => t.TargetContactMethodID);
            
            CreateTable(
                "dbo.PasswordResetRequest",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        SessionID = c.Long(nullable: false),
                        TargetUserID = c.Long(nullable: false),
                        StartTime = c.DateTime(nullable: false),
                        ExpirationTime = c.DateTime(nullable: false),
                        CompletionTime = c.DateTime(),
                        PasswordResetToken = c.String(nullable: false, maxLength: 20),
                        ContactMethodID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.HttpSession", t => t.SessionID)
                .ForeignKey("dbo.User", t => t.TargetUserID, cascadeDelete: true)
                .ForeignKey("dbo.UserContactMethod", t => t.ContactMethodID)
                .Index(t => t.SessionID)
                .Index(t => t.TargetUserID)
                .Index(t => t.ContactMethodID);
            
            CreateTable(
                "dbo.LoginNameQuery",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        SessionID = c.Long(nullable: false),
                        TargetUserID = c.Long(nullable: false),
                        QueryTime = c.DateTime(nullable: false),
                        ContactMethodID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.HttpSession", t => t.SessionID)
                .ForeignKey("dbo.User", t => t.TargetUserID, cascadeDelete: true)
                .ForeignKey("dbo.UserContactMethod", t => t.ContactMethodID)
                .Index(t => t.SessionID)
                .Index(t => t.TargetUserID)
                .Index(t => t.ContactMethodID);
            
            CreateTable(
                "dbo.AbuseFlag",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        EntityType = c.Byte(nullable: false),
                        EntityID = c.Long(nullable: false),
                        Reason = c.Byte(nullable: false),
                        Comments = c.String(maxLength: 500),
                        ReportDate = c.DateTime(nullable: false),
                        ReportedByID = c.Long(),
                        ReportedInSessionID = c.Long(nullable: false),
                        Approved = c.Boolean(),
                        ApprovalDate = c.DateTime(),
                        ApprovedByID = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.User", t => t.ReportedByID)
                .ForeignKey("dbo.HttpSession", t => t.ReportedInSessionID)
                .ForeignKey("dbo.User", t => t.ApprovedByID)
                .Index(t => t.ReportedByID)
                .Index(t => t.ReportedInSessionID)
                .Index(t => t.ApprovedByID);
            
            CreateTable(
                "dbo.CityRegionNeighborhoodRelation",
                c => new
                    {
                        Neighborhood_ID = c.Long(nullable: false),
                        CityRegion_ID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.Neighborhood_ID, t.CityRegion_ID })
                .ForeignKey("dbo.Neighborhood", t => t.Neighborhood_ID, cascadeDelete: true)
                .ForeignKey("dbo.CityRegion", t => t.CityRegion_ID, cascadeDelete: true)
                .Index(t => t.Neighborhood_ID)
                .Index(t => t.CityRegion_ID);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.CityRegionNeighborhoodRelation", new[] { "CityRegion_ID" });
            DropIndex("dbo.CityRegionNeighborhoodRelation", new[] { "Neighborhood_ID" });
            DropIndex("dbo.AbuseFlag", new[] { "ApprovedByID" });
            DropIndex("dbo.AbuseFlag", new[] { "ReportedInSessionID" });
            DropIndex("dbo.AbuseFlag", new[] { "ReportedByID" });
            DropIndex("dbo.LoginNameQuery", new[] { "ContactMethodID" });
            DropIndex("dbo.LoginNameQuery", new[] { "TargetUserID" });
            DropIndex("dbo.LoginNameQuery", new[] { "SessionID" });
            DropIndex("dbo.PasswordResetRequest", new[] { "ContactMethodID" });
            DropIndex("dbo.PasswordResetRequest", new[] { "TargetUserID" });
            DropIndex("dbo.PasswordResetRequest", new[] { "SessionID" });
            DropIndex("dbo.UserContactMethodVerification", new[] { "TargetContactMethodID" });
            DropIndex("dbo.UserContactMethodVerification", new[] { "TargetUserID" });
            DropIndex("dbo.UserContactMethodVerification", new[] { "SessionID" });
            DropIndex("dbo.ActivityLog", new[] { "ReviewedByID" });
            DropIndex("dbo.ActivityLog", new[] { "AuthenticatedUserID" });
            DropIndex("dbo.ActivityLog", new[] { "SessionID" });
            DropIndex("dbo.PropertySearchHistory", new[] { "UserID" });
            DropIndex("dbo.PropertySearchHistory", new[] { "SessionID" });
            DropIndex("dbo.PropertyListingPublishHistory", new[] { "UserID" });
            DropIndex("dbo.PropertyListingPublishHistory", new[] { "SessionID" });
            DropIndex("dbo.PropertyListingPublishHistory", new[] { "PropertyListingID" });
            DropIndex("dbo.PropertyListingUpdateHistory", new[] { "UserID" });
            DropIndex("dbo.PropertyListingUpdateHistory", new[] { "SessionID" });
            DropIndex("dbo.PropertyListingUpdateHistory", new[] { "PropertyListingID" });
            DropIndex("dbo.UserContactMethod", new[] { "UserID" });
            DropIndex("dbo.User", new[] { "VerificationContactID" });
            DropIndex("dbo.User", new[] { "CreatorSessionID" });
            DropIndex("dbo.HttpSession", new[] { "UniqueVisitorID" });
            DropIndex("dbo.HttpSession", new[] { "UserID" });
            DropIndex("dbo.Unit", new[] { "BuildingID" });
            DropIndex("dbo.Building", new[] { "EstateID" });
            DropIndex("dbo.Estate", new[] { "NeighborhoodID" });
            DropIndex("dbo.Estate", new[] { "CityRegionID" });
            DropIndex("dbo.Estate", new[] { "CityID" });
            DropIndex("dbo.Estate", new[] { "ProvinceID" });
            DropIndex("dbo.PropertyListing", new[] { "OwnerUserID" });
            DropIndex("dbo.PropertyListing", new[] { "CreatorUserID" });
            DropIndex("dbo.PropertyListing", new[] { "CreatorSessionID" });
            DropIndex("dbo.PropertyListing", new[] { "ContactInfoID" });
            DropIndex("dbo.PropertyListing", new[] { "RentConditionsID" });
            DropIndex("dbo.PropertyListing", new[] { "SaleConditionsID" });
            DropIndex("dbo.PropertyListing", new[] { "UnitID" });
            DropIndex("dbo.PropertyListing", new[] { "BuildingID" });
            DropIndex("dbo.PropertyListing", new[] { "EstateID" });
            DropIndex("dbo.PropertyListing", new[] { "NeighborhoodID" });
            DropIndex("dbo.PropertyListing", new[] { "CityRegionID" });
            DropIndex("dbo.PropertyListing", new[] { "CityID" });
            DropIndex("dbo.PropertyListing", new[] { "ProvinceID" });
            DropIndex("dbo.CityRegion", new[] { "CityID" });
            DropIndex("dbo.Neighborhood", new[] { "CityID" });
            DropIndex("dbo.City", new[] { "ProvinceID" });
            DropForeignKey("dbo.CityRegionNeighborhoodRelation", "CityRegion_ID", "dbo.CityRegion");
            DropForeignKey("dbo.CityRegionNeighborhoodRelation", "Neighborhood_ID", "dbo.Neighborhood");
            DropForeignKey("dbo.AbuseFlag", "ApprovedByID", "dbo.User");
            DropForeignKey("dbo.AbuseFlag", "ReportedInSessionID", "dbo.HttpSession");
            DropForeignKey("dbo.AbuseFlag", "ReportedByID", "dbo.User");
            DropForeignKey("dbo.LoginNameQuery", "ContactMethodID", "dbo.UserContactMethod");
            DropForeignKey("dbo.LoginNameQuery", "TargetUserID", "dbo.User");
            DropForeignKey("dbo.LoginNameQuery", "SessionID", "dbo.HttpSession");
            DropForeignKey("dbo.PasswordResetRequest", "ContactMethodID", "dbo.UserContactMethod");
            DropForeignKey("dbo.PasswordResetRequest", "TargetUserID", "dbo.User");
            DropForeignKey("dbo.PasswordResetRequest", "SessionID", "dbo.HttpSession");
            DropForeignKey("dbo.UserContactMethodVerification", "TargetContactMethodID", "dbo.UserContactMethod");
            DropForeignKey("dbo.UserContactMethodVerification", "TargetUserID", "dbo.User");
            DropForeignKey("dbo.UserContactMethodVerification", "SessionID", "dbo.HttpSession");
            DropForeignKey("dbo.ActivityLog", "ReviewedByID", "dbo.User");
            DropForeignKey("dbo.ActivityLog", "AuthenticatedUserID", "dbo.User");
            DropForeignKey("dbo.ActivityLog", "SessionID", "dbo.HttpSession");
            DropForeignKey("dbo.PropertySearchHistory", "UserID", "dbo.User");
            DropForeignKey("dbo.PropertySearchHistory", "SessionID", "dbo.HttpSession");
            DropForeignKey("dbo.PropertyListingPublishHistory", "UserID", "dbo.User");
            DropForeignKey("dbo.PropertyListingPublishHistory", "SessionID", "dbo.HttpSession");
            DropForeignKey("dbo.PropertyListingPublishHistory", "PropertyListingID", "dbo.PropertyListing");
            DropForeignKey("dbo.PropertyListingUpdateHistory", "UserID", "dbo.User");
            DropForeignKey("dbo.PropertyListingUpdateHistory", "SessionID", "dbo.HttpSession");
            DropForeignKey("dbo.PropertyListingUpdateHistory", "PropertyListingID", "dbo.PropertyListing");
            DropForeignKey("dbo.UserContactMethod", "UserID", "dbo.User");
            DropForeignKey("dbo.User", "VerificationContactID", "dbo.UserContactMethod");
            DropForeignKey("dbo.User", "CreatorSessionID", "dbo.HttpSession");
            DropForeignKey("dbo.HttpSession", "UniqueVisitorID", "dbo.UniqueVisitor");
            DropForeignKey("dbo.HttpSession", "UserID", "dbo.User");
            DropForeignKey("dbo.Unit", "BuildingID", "dbo.Building");
            DropForeignKey("dbo.Building", "EstateID", "dbo.Estate");
            DropForeignKey("dbo.Estate", "NeighborhoodID", "dbo.Neighborhood");
            DropForeignKey("dbo.Estate", "CityRegionID", "dbo.CityRegion");
            DropForeignKey("dbo.Estate", "CityID", "dbo.City");
            DropForeignKey("dbo.Estate", "ProvinceID", "dbo.Province");
            DropForeignKey("dbo.PropertyListing", "OwnerUserID", "dbo.User");
            DropForeignKey("dbo.PropertyListing", "CreatorUserID", "dbo.User");
            DropForeignKey("dbo.PropertyListing", "CreatorSessionID", "dbo.HttpSession");
            DropForeignKey("dbo.PropertyListing", "ContactInfoID", "dbo.ContactInfo");
            DropForeignKey("dbo.PropertyListing", "RentConditionsID", "dbo.RentConditions");
            DropForeignKey("dbo.PropertyListing", "SaleConditionsID", "dbo.SaleConditions");
            DropForeignKey("dbo.PropertyListing", "UnitID", "dbo.Unit");
            DropForeignKey("dbo.PropertyListing", "BuildingID", "dbo.Building");
            DropForeignKey("dbo.PropertyListing", "EstateID", "dbo.Estate");
            DropForeignKey("dbo.PropertyListing", "NeighborhoodID", "dbo.Neighborhood");
            DropForeignKey("dbo.PropertyListing", "CityRegionID", "dbo.CityRegion");
            DropForeignKey("dbo.PropertyListing", "CityID", "dbo.City");
            DropForeignKey("dbo.PropertyListing", "ProvinceID", "dbo.Province");
            DropForeignKey("dbo.CityRegion", "CityID", "dbo.City");
            DropForeignKey("dbo.Neighborhood", "CityID", "dbo.City");
            DropForeignKey("dbo.City", "ProvinceID", "dbo.Province");
            DropTable("dbo.CityRegionNeighborhoodRelation");
            DropTable("dbo.AbuseFlag");
            DropTable("dbo.LoginNameQuery");
            DropTable("dbo.PasswordResetRequest");
            DropTable("dbo.UserContactMethodVerification");
            DropTable("dbo.ActivityLog");
            DropTable("dbo.PropertySearchHistory");
            DropTable("dbo.PropertyListingPublishHistory");
            DropTable("dbo.PropertyListingUpdateHistory");
            DropTable("dbo.UniqueVisitor");
            DropTable("dbo.UserContactMethod");
            DropTable("dbo.User");
            DropTable("dbo.HttpSession");
            DropTable("dbo.ContactInfo");
            DropTable("dbo.RentConditions");
            DropTable("dbo.SaleConditions");
            DropTable("dbo.Unit");
            DropTable("dbo.Building");
            DropTable("dbo.Estate");
            DropTable("dbo.PropertyListing");
            DropTable("dbo.CityRegion");
            DropTable("dbo.Neighborhood");
            DropTable("dbo.City");
            DropTable("dbo.Province");
        }
    }
}
