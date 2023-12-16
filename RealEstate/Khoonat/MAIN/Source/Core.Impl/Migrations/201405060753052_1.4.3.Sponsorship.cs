namespace JahanJooy.RealEstate.Core.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _143Sponsorship : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SponsoredPropertyListing",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        SponsoredEntityID = c.Long(nullable: false),
                        ListingID = c.Long(nullable: false),
                        ShowInAllPages = c.Boolean(nullable: false),
                        ShowOnMap = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.PropertyListing", t => t.ListingID, cascadeDelete: true)
                .ForeignKey("dbo.SponsoredEntity", t => t.SponsoredEntityID, cascadeDelete: true)
                .Index(t => t.ListingID)
                .Index(t => t.SponsoredEntityID);
            
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
                .ForeignKey("dbo.UserBillingTransaction", t => t.ForwardTransactionID)
                .ForeignKey("dbo.UserBillingTransaction", t => t.ReverseTransactionID)
                .ForeignKey("dbo.User", t => t.TargetUserID)
                .ForeignKey("dbo.SponsoredEntity", t => t.SponsoredEntityID)
                .Index(t => t.ForwardTransactionID)
                .Index(t => t.ReverseTransactionID)
                .Index(t => t.TargetUserID)
                .Index(t => t.SponsoredEntityID);
            
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
                .ForeignKey("dbo.HttpSession", t => t.HttpSessionID)
                .ForeignKey("dbo.SponsoredEntityImpression", t => t.ImpressionID)
                .ForeignKey("dbo.SponsoredEntityClickBilling", t => t.BillingEntityID)
                .ForeignKey("dbo.SponsoredEntity", t => t.SponsoredEntityID)
                .Index(t => t.HttpSessionID)
                .Index(t => t.ImpressionID)
                .Index(t => t.BillingEntityID)
                .Index(t => t.SponsoredEntityID);
            
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
                .ForeignKey("dbo.User", t => t.ContentOwnerUserID)
                .ForeignKey("dbo.HttpSession", t => t.HttpSessionID)
                .ForeignKey("dbo.SponsoredEntity", t => t.SponsoredEntityID)
                .Index(t => t.BillingEntityID)
                .Index(t => t.ContentOwnerUserID)
                .Index(t => t.HttpSessionID)
                .Index(t => t.SponsoredEntityID);
            
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
                .ForeignKey("dbo.User", t => t.TargetUserID)
                .ForeignKey("dbo.SponsoredEntity", t => t.SponsoredEntityID)
                .Index(t => t.ForwardTransactionID)
                .Index(t => t.ReverseTransactionID)
                .Index(t => t.TargetUserID)
                .Index(t => t.SponsoredEntityID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SponsoredPropertyListing", "SponsoredEntityID", "dbo.SponsoredEntity");
            DropForeignKey("dbo.SponsoredEntityImpression", "SponsoredEntityID", "dbo.SponsoredEntity");
            DropForeignKey("dbo.SponsoredEntityImpressionBilling", "SponsoredEntityID", "dbo.SponsoredEntity");
            DropForeignKey("dbo.SponsoredEntityClick", "SponsoredEntityID", "dbo.SponsoredEntity");
            DropForeignKey("dbo.SponsoredEntityClickBilling", "SponsoredEntityID", "dbo.SponsoredEntity");
            DropForeignKey("dbo.SponsoredEntityClickBilling", "TargetUserID", "dbo.User");
            DropForeignKey("dbo.SponsoredEntityClickBilling", "ReverseTransactionID", "dbo.UserBillingTransaction");
            DropForeignKey("dbo.SponsoredEntityClickBilling", "ForwardTransactionID", "dbo.UserBillingTransaction");
            DropForeignKey("dbo.SponsoredEntityClick", "BillingEntityID", "dbo.SponsoredEntityClickBilling");
            DropForeignKey("dbo.SponsoredEntityClick", "ImpressionID", "dbo.SponsoredEntityImpression");
            DropForeignKey("dbo.SponsoredEntityImpression", "HttpSessionID", "dbo.HttpSession");
            DropForeignKey("dbo.SponsoredEntityImpression", "ContentOwnerUserID", "dbo.User");
            DropForeignKey("dbo.SponsoredEntityImpressionBilling", "TargetUserID", "dbo.User");
            DropForeignKey("dbo.SponsoredEntityImpressionBilling", "ReverseTransactionID", "dbo.UserBillingTransaction");
            DropForeignKey("dbo.SponsoredEntityImpression", "BillingEntityID", "dbo.SponsoredEntityImpressionBilling");
            DropForeignKey("dbo.SponsoredEntityImpressionBilling", "ForwardTransactionID", "dbo.UserBillingTransaction");
            DropForeignKey("dbo.SponsoredEntityClick", "HttpSessionID", "dbo.HttpSession");
            DropForeignKey("dbo.SponsoredEntity", "BilledUserID", "dbo.User");
            DropForeignKey("dbo.SponsoredPropertyListing", "ListingID", "dbo.PropertyListing");
            DropIndex("dbo.SponsoredPropertyListing", new[] { "SponsoredEntityID" });
            DropIndex("dbo.SponsoredEntityImpression", new[] { "SponsoredEntityID" });
            DropIndex("dbo.SponsoredEntityImpressionBilling", new[] { "SponsoredEntityID" });
            DropIndex("dbo.SponsoredEntityClick", new[] { "SponsoredEntityID" });
            DropIndex("dbo.SponsoredEntityClickBilling", new[] { "SponsoredEntityID" });
            DropIndex("dbo.SponsoredEntityClickBilling", new[] { "TargetUserID" });
            DropIndex("dbo.SponsoredEntityClickBilling", new[] { "ReverseTransactionID" });
            DropIndex("dbo.SponsoredEntityClickBilling", new[] { "ForwardTransactionID" });
            DropIndex("dbo.SponsoredEntityClick", new[] { "BillingEntityID" });
            DropIndex("dbo.SponsoredEntityClick", new[] { "ImpressionID" });
            DropIndex("dbo.SponsoredEntityImpression", new[] { "HttpSessionID" });
            DropIndex("dbo.SponsoredEntityImpression", new[] { "ContentOwnerUserID" });
            DropIndex("dbo.SponsoredEntityImpressionBilling", new[] { "TargetUserID" });
            DropIndex("dbo.SponsoredEntityImpressionBilling", new[] { "ReverseTransactionID" });
            DropIndex("dbo.SponsoredEntityImpression", new[] { "BillingEntityID" });
            DropIndex("dbo.SponsoredEntityImpressionBilling", new[] { "ForwardTransactionID" });
            DropIndex("dbo.SponsoredEntityClick", new[] { "HttpSessionID" });
            DropIndex("dbo.SponsoredEntity", new[] { "BilledUserID" });
            DropIndex("dbo.SponsoredPropertyListing", new[] { "ListingID" });
            DropTable("dbo.SponsoredEntityImpressionBilling");
            DropTable("dbo.SponsoredEntityImpression");
            DropTable("dbo.SponsoredEntityClick");
            DropTable("dbo.SponsoredEntityClickBilling");
            DropTable("dbo.SponsoredEntity");
            DropTable("dbo.SponsoredPropertyListing");
        }
    }
}
