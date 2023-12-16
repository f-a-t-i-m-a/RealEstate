namespace JahanJooy.RealEstate.Core.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _141Sponsorship : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SponsoredEntity",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        EntityType = c.Byte(nullable: false),
                        BilledUserID = c.Long(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                        LastImpressionTime = c.DateTime(),
                        ExpirationTime = c.DateTime(),
                        DeleteTime = c.DateTime(),
                        Title = c.String(maxLength: 100),
                        Enabled = c.Boolean(nullable: false),
                        BillingMethod = c.Int(nullable: false),
                        MaxPayPerImpression = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaxPayPerClick = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BlockedForLowCredit = c.Boolean(nullable: false),
                        EstimatedClicksPerImpression = c.Decimal(nullable: false, precision: 18, scale: 10),
                        ProjectedMaxPayPerImpression = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NextRecalcDue = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.User", t => t.BilledUserID)
                .Index(t => t.BilledUserID);
            
            CreateTable(
                "dbo.SponsoredEntityClickBilling",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        CreationTime = c.DateTime(nullable: false),
                        CompletionTime = c.DateTime(),
                        ClicksDate = c.DateTime(nullable: false),
                        SponsoredEntityID = c.Long(nullable: false),
                        NumberOfClicks = c.Int(nullable: false),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TargetUserID = c.Long(),
                        BillingState = c.Byte(nullable: false),
                        ForwardTransactionID = c.Long(),
                        ReverseTransactionID = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.SponsoredEntity", t => t.SponsoredEntityID)
                .ForeignKey("dbo.UserBillingTransaction", t => t.ForwardTransactionID)
                .ForeignKey("dbo.UserBillingTransaction", t => t.ReverseTransactionID)
                .ForeignKey("dbo.User", t => t.TargetUserID)
                .Index(t => t.SponsoredEntityID)
                .Index(t => t.ForwardTransactionID)
                .Index(t => t.ReverseTransactionID)
                .Index(t => t.TargetUserID);
            
            CreateTable(
                "dbo.SponsoredEntityClick",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        CreationTime = c.DateTime(nullable: false),
                        SponsoredEntityID = c.Long(nullable: false),
                        ImpressionID = c.Long(nullable: false),
                        HttpSessionID = c.Long(nullable: false),
                        BillingEntityID = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.SponsoredEntityClickBilling", t => t.BillingEntityID)
                .ForeignKey("dbo.HttpSession", t => t.HttpSessionID)
                .ForeignKey("dbo.SponsoredEntity", t => t.SponsoredEntityID)
                .ForeignKey("dbo.SponsoredEntityImpression", t => t.ImpressionID)
                .Index(t => t.BillingEntityID)
                .Index(t => t.HttpSessionID)
                .Index(t => t.SponsoredEntityID)
                .Index(t => t.ImpressionID);
            
            CreateTable(
                "dbo.SponsoredEntityImpression",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        GUID = c.Guid(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                        BidAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SponsoredEntityID = c.Long(nullable: false),
                        ContentOwnerUserID = c.Long(),
                        HttpSessionID = c.Long(nullable: false),
                        BillingEntityID = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.SponsoredEntityImpressionBilling", t => t.BillingEntityID)
                .ForeignKey("dbo.SponsoredEntity", t => t.SponsoredEntityID)
                .ForeignKey("dbo.User", t => t.ContentOwnerUserID)
                .ForeignKey("dbo.HttpSession", t => t.HttpSessionID)
                .Index(t => t.BillingEntityID)
                .Index(t => t.SponsoredEntityID)
                .Index(t => t.ContentOwnerUserID)
                .Index(t => t.HttpSessionID);
            
            CreateTable(
                "dbo.SponsoredEntityImpressionBilling",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        CreationTime = c.DateTime(nullable: false),
                        CompletionTime = c.DateTime(),
                        ImpressionsDate = c.DateTime(nullable: false),
                        SponsoredEntityID = c.Long(nullable: false),
                        NumberOfImpressions = c.Int(nullable: false),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TargetUserID = c.Long(),
                        BillingState = c.Byte(nullable: false),
                        ForwardTransactionID = c.Long(),
                        ReverseTransactionID = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.UserBillingTransaction", t => t.ForwardTransactionID)
                .ForeignKey("dbo.UserBillingTransaction", t => t.ReverseTransactionID)
                .ForeignKey("dbo.SponsoredEntity", t => t.SponsoredEntityID)
                .ForeignKey("dbo.User", t => t.TargetUserID)
                .Index(t => t.ForwardTransactionID)
                .Index(t => t.ReverseTransactionID)
                .Index(t => t.SponsoredEntityID)
                .Index(t => t.TargetUserID);
            
            CreateTable(
                "dbo.Agency",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 80),
                        ManagerName = c.String(maxLength: 80),
                        Code = c.String(maxLength: 50),
                        Description = c.String(maxLength: 1000),
                        CreationTime = c.DateTime(nullable: false),
                        LastModificationTime = c.DateTime(nullable: false),
                        DeleteTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.AgencyBranch",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        AgencyID = c.Long(nullable: false),
                        BranchName = c.String(maxLength: 80),
                        BranchManagerName = c.String(maxLength: 80),
                        IsMainBranch = c.Boolean(nullable: false),
                        ProvinceID = c.Long(nullable: false),
                        CityID = c.Long(nullable: false),
                        CityRegionID = c.Long(nullable: false),
                        NeighborhoodID = c.Long(nullable: false),
                        FullAddress = c.String(maxLength: 1000),
                        GeoLocation = c.Geography(),
                        Phone1 = c.String(maxLength: 30),
                        Phone2 = c.String(maxLength: 30),
                        CellPhone1 = c.String(maxLength: 30),
                        CellPhone2 = c.String(maxLength: 30),
                        Fax = c.String(maxLength: 30),
                        Email = c.String(maxLength: 50),
                        SmsNumber = c.String(maxLength: 30),
                        Description = c.String(maxLength: 1000),
                        ActivityRegion = c.String(maxLength: 1000),
                        GeoActivityRegion = c.Geography(),
                        CreationTime = c.DateTime(nullable: false),
                        LastModificationTime = c.DateTime(nullable: false),
                        DeleteTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Agency", t => t.AgencyID, cascadeDelete: true)
                .ForeignKey("dbo.City", t => t.CityID)
                .ForeignKey("dbo.CityRegion", t => t.CityRegionID)
                .ForeignKey("dbo.Neighborhood", t => t.NeighborhoodID)
                .ForeignKey("dbo.Province", t => t.ProvinceID)
                .Index(t => t.AgencyID)
                .Index(t => t.CityID)
                .Index(t => t.CityRegionID)
                .Index(t => t.NeighborhoodID)
                .Index(t => t.ProvinceID);
            
            CreateTable(
                "dbo.SponsoredPropertyListing",
                c => new
                    {
                        ID = c.Long(nullable: false),
                        ListingID = c.Long(nullable: false),
                        ShowInAllPages = c.Boolean(nullable: false),
                        ShowOnMap = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.SponsoredEntity", t => t.ID)
                .ForeignKey("dbo.PropertyListing", t => t.ListingID)
                .Index(t => t.ID)
                .Index(t => t.ListingID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SponsoredPropertyListing", "ListingID", "dbo.PropertyListing");
            DropForeignKey("dbo.SponsoredPropertyListing", "ID", "dbo.SponsoredEntity");
            DropForeignKey("dbo.AgencyBranch", "ProvinceID", "dbo.Province");
            DropForeignKey("dbo.AgencyBranch", "NeighborhoodID", "dbo.Neighborhood");
            DropForeignKey("dbo.AgencyBranch", "CityRegionID", "dbo.CityRegion");
            DropForeignKey("dbo.AgencyBranch", "CityID", "dbo.City");
            DropForeignKey("dbo.AgencyBranch", "AgencyID", "dbo.Agency");
            DropForeignKey("dbo.SponsoredEntityClickBilling", "TargetUserID", "dbo.User");
            DropForeignKey("dbo.SponsoredEntityClickBilling", "ReverseTransactionID", "dbo.UserBillingTransaction");
            DropForeignKey("dbo.SponsoredEntityClickBilling", "ForwardTransactionID", "dbo.UserBillingTransaction");
            DropForeignKey("dbo.SponsoredEntityClick", "ImpressionID", "dbo.SponsoredEntityImpression");
            DropForeignKey("dbo.SponsoredEntityImpression", "HttpSessionID", "dbo.HttpSession");
            DropForeignKey("dbo.SponsoredEntityImpression", "ContentOwnerUserID", "dbo.User");
            DropForeignKey("dbo.SponsoredEntityImpressionBilling", "TargetUserID", "dbo.User");
            DropForeignKey("dbo.SponsoredEntityImpression", "SponsoredEntityID", "dbo.SponsoredEntity");
            DropForeignKey("dbo.SponsoredEntityImpressionBilling", "SponsoredEntityID", "dbo.SponsoredEntity");
            DropForeignKey("dbo.SponsoredEntityClick", "SponsoredEntityID", "dbo.SponsoredEntity");
            DropForeignKey("dbo.SponsoredEntityClickBilling", "SponsoredEntityID", "dbo.SponsoredEntity");
            DropForeignKey("dbo.SponsoredEntity", "BilledUserID", "dbo.User");
            DropForeignKey("dbo.SponsoredEntityImpressionBilling", "ReverseTransactionID", "dbo.UserBillingTransaction");
            DropForeignKey("dbo.SponsoredEntityImpression", "BillingEntityID", "dbo.SponsoredEntityImpressionBilling");
            DropForeignKey("dbo.SponsoredEntityImpressionBilling", "ForwardTransactionID", "dbo.UserBillingTransaction");
            DropForeignKey("dbo.SponsoredEntityClick", "HttpSessionID", "dbo.HttpSession");
            DropForeignKey("dbo.SponsoredEntityClick", "BillingEntityID", "dbo.SponsoredEntityClickBilling");
            DropIndex("dbo.SponsoredPropertyListing", new[] { "ListingID" });
            DropIndex("dbo.SponsoredPropertyListing", new[] { "ID" });
            DropIndex("dbo.AgencyBranch", new[] { "ProvinceID" });
            DropIndex("dbo.AgencyBranch", new[] { "NeighborhoodID" });
            DropIndex("dbo.AgencyBranch", new[] { "CityRegionID" });
            DropIndex("dbo.AgencyBranch", new[] { "CityID" });
            DropIndex("dbo.AgencyBranch", new[] { "AgencyID" });
            DropIndex("dbo.SponsoredEntityClickBilling", new[] { "TargetUserID" });
            DropIndex("dbo.SponsoredEntityClickBilling", new[] { "ReverseTransactionID" });
            DropIndex("dbo.SponsoredEntityClickBilling", new[] { "ForwardTransactionID" });
            DropIndex("dbo.SponsoredEntityClick", new[] { "ImpressionID" });
            DropIndex("dbo.SponsoredEntityImpression", new[] { "HttpSessionID" });
            DropIndex("dbo.SponsoredEntityImpression", new[] { "ContentOwnerUserID" });
            DropIndex("dbo.SponsoredEntityImpressionBilling", new[] { "TargetUserID" });
            DropIndex("dbo.SponsoredEntityImpression", new[] { "SponsoredEntityID" });
            DropIndex("dbo.SponsoredEntityImpressionBilling", new[] { "SponsoredEntityID" });
            DropIndex("dbo.SponsoredEntityClick", new[] { "SponsoredEntityID" });
            DropIndex("dbo.SponsoredEntityClickBilling", new[] { "SponsoredEntityID" });
            DropIndex("dbo.SponsoredEntity", new[] { "BilledUserID" });
            DropIndex("dbo.SponsoredEntityImpressionBilling", new[] { "ReverseTransactionID" });
            DropIndex("dbo.SponsoredEntityImpression", new[] { "BillingEntityID" });
            DropIndex("dbo.SponsoredEntityImpressionBilling", new[] { "ForwardTransactionID" });
            DropIndex("dbo.SponsoredEntityClick", new[] { "HttpSessionID" });
            DropIndex("dbo.SponsoredEntityClick", new[] { "BillingEntityID" });
            DropTable("dbo.SponsoredPropertyListing");
            DropTable("dbo.AgencyBranch");
            DropTable("dbo.Agency");
            DropTable("dbo.SponsoredEntityImpressionBilling");
            DropTable("dbo.SponsoredEntityImpression");
            DropTable("dbo.SponsoredEntityClick");
            DropTable("dbo.SponsoredEntityClickBilling");
            DropTable("dbo.SponsoredEntity");
        }
    }
}
