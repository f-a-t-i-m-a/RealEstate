namespace JahanJooy.RealEstate.Core.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _142CmsAndVicinity : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SponsoredEntityClick", "BillingEntityID", "dbo.SponsoredEntityClickBilling");
            DropForeignKey("dbo.SponsoredEntityClick", "HttpSessionID", "dbo.HttpSession");
            DropForeignKey("dbo.SponsoredEntityImpressionBilling", "ForwardTransactionID", "dbo.UserBillingTransaction");
            DropForeignKey("dbo.SponsoredEntityImpression", "BillingEntityID", "dbo.SponsoredEntityImpressionBilling");
            DropForeignKey("dbo.SponsoredEntityImpressionBilling", "ReverseTransactionID", "dbo.UserBillingTransaction");
            DropForeignKey("dbo.SponsoredEntity", "BilledUserID", "dbo.User");
            DropForeignKey("dbo.SponsoredEntityClickBilling", "SponsoredEntityID", "dbo.SponsoredEntity");
            DropForeignKey("dbo.SponsoredEntityClick", "SponsoredEntityID", "dbo.SponsoredEntity");
            DropForeignKey("dbo.SponsoredEntityImpressionBilling", "SponsoredEntityID", "dbo.SponsoredEntity");
            DropForeignKey("dbo.SponsoredEntityImpression", "SponsoredEntityID", "dbo.SponsoredEntity");
            DropForeignKey("dbo.SponsoredEntityImpressionBilling", "TargetUserID", "dbo.User");
            DropForeignKey("dbo.SponsoredEntityImpression", "ContentOwnerUserID", "dbo.User");
            DropForeignKey("dbo.SponsoredEntityImpression", "HttpSessionID", "dbo.HttpSession");
            DropForeignKey("dbo.SponsoredEntityClick", "ImpressionID", "dbo.SponsoredEntityImpression");
            DropForeignKey("dbo.SponsoredEntityClickBilling", "ForwardTransactionID", "dbo.UserBillingTransaction");
            DropForeignKey("dbo.SponsoredEntityClickBilling", "ReverseTransactionID", "dbo.UserBillingTransaction");
            DropForeignKey("dbo.SponsoredEntityClickBilling", "TargetUserID", "dbo.User");
            DropForeignKey("dbo.SponsoredPropertyListing", "ID", "dbo.SponsoredEntity");
            DropForeignKey("dbo.SponsoredPropertyListing", "ListingID", "dbo.PropertyListing");
            DropIndex("dbo.SponsoredEntityClick", new[] { "BillingEntityID" });
            DropIndex("dbo.SponsoredEntityClick", new[] { "HttpSessionID" });
            DropIndex("dbo.SponsoredEntityImpressionBilling", new[] { "ForwardTransactionID" });
            DropIndex("dbo.SponsoredEntityImpression", new[] { "BillingEntityID" });
            DropIndex("dbo.SponsoredEntityImpressionBilling", new[] { "ReverseTransactionID" });
            DropIndex("dbo.SponsoredEntity", new[] { "BilledUserID" });
            DropIndex("dbo.SponsoredEntityClickBilling", new[] { "SponsoredEntityID" });
            DropIndex("dbo.SponsoredEntityClick", new[] { "SponsoredEntityID" });
            DropIndex("dbo.SponsoredEntityImpressionBilling", new[] { "SponsoredEntityID" });
            DropIndex("dbo.SponsoredEntityImpression", new[] { "SponsoredEntityID" });
            DropIndex("dbo.SponsoredEntityImpressionBilling", new[] { "TargetUserID" });
            DropIndex("dbo.SponsoredEntityImpression", new[] { "ContentOwnerUserID" });
            DropIndex("dbo.SponsoredEntityImpression", new[] { "HttpSessionID" });
            DropIndex("dbo.SponsoredEntityClick", new[] { "ImpressionID" });
            DropIndex("dbo.SponsoredEntityClickBilling", new[] { "ForwardTransactionID" });
            DropIndex("dbo.SponsoredEntityClickBilling", new[] { "ReverseTransactionID" });
            DropIndex("dbo.SponsoredEntityClickBilling", new[] { "TargetUserID" });
            DropIndex("dbo.SponsoredPropertyListing", new[] { "ID" });
            DropIndex("dbo.SponsoredPropertyListing", new[] { "ListingID" });
            CreateTable(
                "dbo.Vicinity",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        AlternativeNames = c.String(),
                        AdditionalSearchText = c.String(),
                        Description = c.String(),
                        AdministrativeNotes = c.String(),
                        Order = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        WellKnownScope = c.Int(nullable: false),
                        ShowTypeInTitle = c.Boolean(nullable: false),
                        ShowInSummary = c.Boolean(nullable: false),
                        CanContainPropertyRecords = c.Boolean(nullable: false),
                        CenterPoint = c.Geography(),
                        Boundary = c.Geography(),
                        ParentID = c.Long(),
                        OriginalProvinceID = c.Long(),
                        OriginalCityID = c.Long(),
                        OriginalCityRegionID = c.Long(),
                        OriginalNeighborhoodID = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Vicinity", t => t.ParentID)
                .Index(t => t.ParentID);
            
            CreateTable(
                "dbo.ArticleCategory",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Identifier = c.String(maxLength: 200),
                        DefaultDisplayType = c.Byte(nullable: false),
                        DefaultStyleClass = c.String(maxLength: 50),
                        Name = c.String(maxLength: 100),
                        Description = c.String(maxLength: 500),
                        CreationTime = c.DateTime(nullable: false),
                        LastModificationTime = c.DateTime(nullable: false),
                        DeleteTime = c.DateTime(),
                        OwnerUserID = c.Long(),
                        CreatedByUserID = c.Long(nullable: false),
                        LastModifiedByUserID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.User", t => t.CreatedByUserID)
                .ForeignKey("dbo.User", t => t.LastModifiedByUserID)
                .ForeignKey("dbo.User", t => t.OwnerUserID)
                .Index(t => t.CreatedByUserID)
                .Index(t => t.LastModifiedByUserID)
                .Index(t => t.OwnerUserID);
            
            CreateTable(
                "dbo.Article",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Identifier = c.String(maxLength: 200),
                        Order = c.Long(nullable: false),
                        CategoryID = c.Long(nullable: false),
                        SetID = c.Long(),
                        ParentArticleID = c.Long(),
                        DisplayType = c.Byte(nullable: false),
                        StyleClass = c.String(maxLength: 50),
                        Name = c.String(maxLength: 100),
                        IsLocked = c.Boolean(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                        LastModificationTime = c.DateTime(nullable: false),
                        DeleteTime = c.DateTime(),
                        OwnerUserID = c.Long(),
                        CreatedByUserID = c.Long(nullable: false),
                        LastModifiedByUserID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.User", t => t.CreatedByUserID)
                .ForeignKey("dbo.User", t => t.LastModifiedByUserID)
                .ForeignKey("dbo.User", t => t.OwnerUserID)
                .ForeignKey("dbo.Article", t => t.ParentArticleID)
                .ForeignKey("dbo.ArticleSet", t => t.SetID)
                .ForeignKey("dbo.ArticleCategory", t => t.CategoryID)
                .Index(t => t.CreatedByUserID)
                .Index(t => t.LastModifiedByUserID)
                .Index(t => t.OwnerUserID)
                .Index(t => t.ParentArticleID)
                .Index(t => t.SetID)
                .Index(t => t.CategoryID);
            
            CreateTable(
                "dbo.ArticleRevision",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        RevisionNumber = c.Int(nullable: false),
                        ArticleID = c.Long(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                        LastModificationTime = c.DateTime(nullable: false),
                        FinalizationTime = c.DateTime(),
                        PublishTime = c.DateTime(),
                        CreatedByUserID = c.Long(nullable: false),
                        LastModifiedByUserID = c.Long(nullable: false),
                        FinalizedByUserID = c.Long(),
                        PublishedByUserID = c.Long(),
                        LinkText = c.String(maxLength: 100),
                        Title = c.String(maxLength: 200),
                        Description = c.String(maxLength: 500),
                        Markdown = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Article", t => t.ArticleID)
                .ForeignKey("dbo.User", t => t.CreatedByUserID)
                .ForeignKey("dbo.User", t => t.FinalizedByUserID)
                .ForeignKey("dbo.User", t => t.LastModifiedByUserID)
                .ForeignKey("dbo.User", t => t.PublishedByUserID)
                .Index(t => t.ArticleID)
                .Index(t => t.CreatedByUserID)
                .Index(t => t.FinalizedByUserID)
                .Index(t => t.LastModifiedByUserID)
                .Index(t => t.PublishedByUserID);
            
            CreateTable(
                "dbo.ArticleSet",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Identifier = c.String(maxLength: 200),
                        CategoryID = c.Long(nullable: false),
                        DefaultDisplayType = c.Byte(nullable: false),
                        DefaultStyleClass = c.String(maxLength: 50),
                        Name = c.String(maxLength: 100),
                        Description = c.String(maxLength: 500),
                        IsLocked = c.Boolean(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                        LastModificationTime = c.DateTime(nullable: false),
                        DeleteTime = c.DateTime(),
                        OwnerUserID = c.Long(),
                        CreatedByUserID = c.Long(nullable: false),
                        LastModifiedByUserID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.User", t => t.CreatedByUserID)
                .ForeignKey("dbo.User", t => t.LastModifiedByUserID)
                .ForeignKey("dbo.User", t => t.OwnerUserID)
                .ForeignKey("dbo.ArticleCategory", t => t.CategoryID)
                .Index(t => t.CreatedByUserID)
                .Index(t => t.LastModifiedByUserID)
                .Index(t => t.OwnerUserID)
                .Index(t => t.CategoryID);
            
            AddColumn("dbo.PropertyListing", "VicinityID", c => c.Long());
            AddColumn("dbo.PropertyListing", "VicinityHierarchyString", c => c.String(maxLength: 150));
            AddColumn("dbo.Estate", "VicinityID", c => c.Long());
            CreateIndex("dbo.Estate", "VicinityID");
            CreateIndex("dbo.PropertyListing", "VicinityID");
            AddForeignKey("dbo.Estate", "VicinityID", "dbo.Vicinity", "ID");
            AddForeignKey("dbo.PropertyListing", "VicinityID", "dbo.Vicinity", "ID");
            DropTable("dbo.SponsoredEntity");
            DropTable("dbo.SponsoredEntityClickBilling");
            DropTable("dbo.SponsoredEntityClick");
            DropTable("dbo.SponsoredEntityImpression");
            DropTable("dbo.SponsoredEntityImpressionBilling");
            DropTable("dbo.SponsoredPropertyListing");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SponsoredPropertyListing",
                c => new
                    {
                        ID = c.Long(nullable: false),
                        ListingID = c.Long(nullable: false),
                        ShowInAllPages = c.Boolean(nullable: false),
                        ShowOnMap = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
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
                .PrimaryKey(t => t.ID);
            
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
                .PrimaryKey(t => t.ID);
            
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
                .PrimaryKey(t => t.ID);
            
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
                .PrimaryKey(t => t.ID);
            
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
                .PrimaryKey(t => t.ID);
            
            DropForeignKey("dbo.ArticleCategory", "OwnerUserID", "dbo.User");
            DropForeignKey("dbo.ArticleCategory", "LastModifiedByUserID", "dbo.User");
            DropForeignKey("dbo.ArticleCategory", "CreatedByUserID", "dbo.User");
            DropForeignKey("dbo.ArticleSet", "CategoryID", "dbo.ArticleCategory");
            DropForeignKey("dbo.Article", "CategoryID", "dbo.ArticleCategory");
            DropForeignKey("dbo.Article", "SetID", "dbo.ArticleSet");
            DropForeignKey("dbo.ArticleSet", "OwnerUserID", "dbo.User");
            DropForeignKey("dbo.ArticleSet", "LastModifiedByUserID", "dbo.User");
            DropForeignKey("dbo.ArticleSet", "CreatedByUserID", "dbo.User");
            DropForeignKey("dbo.ArticleRevision", "PublishedByUserID", "dbo.User");
            DropForeignKey("dbo.ArticleRevision", "LastModifiedByUserID", "dbo.User");
            DropForeignKey("dbo.ArticleRevision", "FinalizedByUserID", "dbo.User");
            DropForeignKey("dbo.ArticleRevision", "CreatedByUserID", "dbo.User");
            DropForeignKey("dbo.ArticleRevision", "ArticleID", "dbo.Article");
            DropForeignKey("dbo.Article", "ParentArticleID", "dbo.Article");
            DropForeignKey("dbo.Article", "OwnerUserID", "dbo.User");
            DropForeignKey("dbo.Article", "LastModifiedByUserID", "dbo.User");
            DropForeignKey("dbo.Article", "CreatedByUserID", "dbo.User");
            DropForeignKey("dbo.PropertyListing", "VicinityID", "dbo.Vicinity");
            DropForeignKey("dbo.Estate", "VicinityID", "dbo.Vicinity");
            DropForeignKey("dbo.Vicinity", "ParentID", "dbo.Vicinity");
            DropIndex("dbo.ArticleCategory", new[] { "OwnerUserID" });
            DropIndex("dbo.ArticleCategory", new[] { "LastModifiedByUserID" });
            DropIndex("dbo.ArticleCategory", new[] { "CreatedByUserID" });
            DropIndex("dbo.ArticleSet", new[] { "CategoryID" });
            DropIndex("dbo.Article", new[] { "CategoryID" });
            DropIndex("dbo.Article", new[] { "SetID" });
            DropIndex("dbo.ArticleSet", new[] { "OwnerUserID" });
            DropIndex("dbo.ArticleSet", new[] { "LastModifiedByUserID" });
            DropIndex("dbo.ArticleSet", new[] { "CreatedByUserID" });
            DropIndex("dbo.ArticleRevision", new[] { "PublishedByUserID" });
            DropIndex("dbo.ArticleRevision", new[] { "LastModifiedByUserID" });
            DropIndex("dbo.ArticleRevision", new[] { "FinalizedByUserID" });
            DropIndex("dbo.ArticleRevision", new[] { "CreatedByUserID" });
            DropIndex("dbo.ArticleRevision", new[] { "ArticleID" });
            DropIndex("dbo.Article", new[] { "ParentArticleID" });
            DropIndex("dbo.Article", new[] { "OwnerUserID" });
            DropIndex("dbo.Article", new[] { "LastModifiedByUserID" });
            DropIndex("dbo.Article", new[] { "CreatedByUserID" });
            DropIndex("dbo.PropertyListing", new[] { "VicinityID" });
            DropIndex("dbo.Estate", new[] { "VicinityID" });
            DropIndex("dbo.Vicinity", new[] { "ParentID" });
            DropColumn("dbo.Estate", "VicinityID");
            DropColumn("dbo.PropertyListing", "VicinityHierarchyString");
            DropColumn("dbo.PropertyListing", "VicinityID");
            DropTable("dbo.ArticleSet");
            DropTable("dbo.ArticleRevision");
            DropTable("dbo.Article");
            DropTable("dbo.ArticleCategory");
            DropTable("dbo.Vicinity");
            CreateIndex("dbo.SponsoredPropertyListing", "ListingID");
            CreateIndex("dbo.SponsoredPropertyListing", "ID");
            CreateIndex("dbo.SponsoredEntityClickBilling", "TargetUserID");
            CreateIndex("dbo.SponsoredEntityClickBilling", "ReverseTransactionID");
            CreateIndex("dbo.SponsoredEntityClickBilling", "ForwardTransactionID");
            CreateIndex("dbo.SponsoredEntityClick", "ImpressionID");
            CreateIndex("dbo.SponsoredEntityImpression", "HttpSessionID");
            CreateIndex("dbo.SponsoredEntityImpression", "ContentOwnerUserID");
            CreateIndex("dbo.SponsoredEntityImpressionBilling", "TargetUserID");
            CreateIndex("dbo.SponsoredEntityImpression", "SponsoredEntityID");
            CreateIndex("dbo.SponsoredEntityImpressionBilling", "SponsoredEntityID");
            CreateIndex("dbo.SponsoredEntityClick", "SponsoredEntityID");
            CreateIndex("dbo.SponsoredEntityClickBilling", "SponsoredEntityID");
            CreateIndex("dbo.SponsoredEntity", "BilledUserID");
            CreateIndex("dbo.SponsoredEntityImpressionBilling", "ReverseTransactionID");
            CreateIndex("dbo.SponsoredEntityImpression", "BillingEntityID");
            CreateIndex("dbo.SponsoredEntityImpressionBilling", "ForwardTransactionID");
            CreateIndex("dbo.SponsoredEntityClick", "HttpSessionID");
            CreateIndex("dbo.SponsoredEntityClick", "BillingEntityID");
            AddForeignKey("dbo.SponsoredPropertyListing", "ListingID", "dbo.PropertyListing", "ID");
            AddForeignKey("dbo.SponsoredPropertyListing", "ID", "dbo.SponsoredEntity", "ID");
            AddForeignKey("dbo.SponsoredEntityClickBilling", "TargetUserID", "dbo.User", "ID");
            AddForeignKey("dbo.SponsoredEntityClickBilling", "ReverseTransactionID", "dbo.UserBillingTransaction", "ID");
            AddForeignKey("dbo.SponsoredEntityClickBilling", "ForwardTransactionID", "dbo.UserBillingTransaction", "ID");
            AddForeignKey("dbo.SponsoredEntityClick", "ImpressionID", "dbo.SponsoredEntityImpression", "ID");
            AddForeignKey("dbo.SponsoredEntityImpression", "HttpSessionID", "dbo.HttpSession", "ID");
            AddForeignKey("dbo.SponsoredEntityImpression", "ContentOwnerUserID", "dbo.User", "ID");
            AddForeignKey("dbo.SponsoredEntityImpressionBilling", "TargetUserID", "dbo.User", "ID");
            AddForeignKey("dbo.SponsoredEntityImpression", "SponsoredEntityID", "dbo.SponsoredEntity", "ID");
            AddForeignKey("dbo.SponsoredEntityImpressionBilling", "SponsoredEntityID", "dbo.SponsoredEntity", "ID");
            AddForeignKey("dbo.SponsoredEntityClick", "SponsoredEntityID", "dbo.SponsoredEntity", "ID");
            AddForeignKey("dbo.SponsoredEntityClickBilling", "SponsoredEntityID", "dbo.SponsoredEntity", "ID");
            AddForeignKey("dbo.SponsoredEntity", "BilledUserID", "dbo.User", "ID");
            AddForeignKey("dbo.SponsoredEntityImpressionBilling", "ReverseTransactionID", "dbo.UserBillingTransaction", "ID");
            AddForeignKey("dbo.SponsoredEntityImpression", "BillingEntityID", "dbo.SponsoredEntityImpressionBilling", "ID");
            AddForeignKey("dbo.SponsoredEntityImpressionBilling", "ForwardTransactionID", "dbo.UserBillingTransaction", "ID");
            AddForeignKey("dbo.SponsoredEntityClick", "HttpSessionID", "dbo.HttpSession", "ID");
            AddForeignKey("dbo.SponsoredEntityClick", "BillingEntityID", "dbo.SponsoredEntityClickBilling", "ID");
        }
    }
}
