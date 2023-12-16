namespace JahanJooy.RealEstate.Core.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1132 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ConfigurationDataItem",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Identifier = c.String(nullable: false, maxLength: 100),
                        Value = c.String(maxLength: 2000),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.OutgoingSmsMessage",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        CreationDate = c.DateTime(nullable: false),
                        TargetUserID = c.Long(),
                        ScheduledDate = c.DateTime(),
                        ExpirationDate = c.DateTime(),
                        AllowTransmissionOnAnyTimeOfDay = c.Boolean(nullable: false),
                        Reason = c.Byte(nullable: false),
                        State = c.Byte(nullable: false),
                        StateDate = c.DateTime(nullable: false),
                        SourceEntityType = c.Byte(nullable: false),
                        SourceEntityID = c.Long(nullable: false),
                        RetryIndex = c.Int(nullable: false),
                        RetryForMessageID = c.Long(),
                        SenderNumber = c.String(maxLength: 20),
                        TargetNumber = c.String(maxLength: 20),
                        MessageText = c.String(maxLength: 500),
                        IsFlash = c.Boolean(nullable: false),
                        ErrorCode = c.Int(),
                        LastDeliveryCode = c.Int(),
                        OperatorAssignedID = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.OutgoingSmsMessage", t => t.RetryForMessageID)
                .ForeignKey("dbo.User", t => t.TargetUserID)
                .Index(t => t.RetryForMessageID)
                .Index(t => t.TargetUserID);
            
            CreateTable(
                "dbo.SavedPropertySearchEmailNotification",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        CreationDate = c.DateTime(nullable: false),
                        PropertyListingID = c.Long(nullable: false),
                        SavedSearchID = c.Long(nullable: false),
                        ContactMethodID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.UserContactMethod", t => t.ContactMethodID)
                .ForeignKey("dbo.PropertyListing", t => t.PropertyListingID)
                .ForeignKey("dbo.SavedPropertySearch", t => t.SavedSearchID)
                .Index(t => t.ContactMethodID)
                .Index(t => t.PropertyListingID)
                .Index(t => t.SavedSearchID);
            
            CreateTable(
                "dbo.SavedPropertySearchSmsNotification",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        CreationDate = c.DateTime(nullable: false),
                        Delivered = c.Boolean(),
                        NumberOfSmsSegments = c.Int(nullable: false),
                        PropertyListingID = c.Long(nullable: false),
                        SavedSearchID = c.Long(nullable: false),
                        ContactMethodID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.UserContactMethod", t => t.ContactMethodID)
                .ForeignKey("dbo.PropertyListing", t => t.PropertyListingID)
                .ForeignKey("dbo.SavedPropertySearch", t => t.SavedSearchID)
                .Index(t => t.ContactMethodID)
                .Index(t => t.PropertyListingID)
                .Index(t => t.SavedSearchID);
            
            CreateTable(
                "dbo.SavedPropertySearchUpdateHistory",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        UpdateDate = c.DateTime(nullable: false),
                        UpdateDetails = c.String(),
                        SavedSearchID = c.Long(nullable: false),
                        SessionID = c.Long(nullable: false),
                        UserID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.SavedPropertySearch", t => t.SavedSearchID, cascadeDelete: true)
                .ForeignKey("dbo.HttpSession", t => t.SessionID)
                .ForeignKey("dbo.User", t => t.UserID)
                .Index(t => t.SavedSearchID)
                .Index(t => t.SessionID)
                .Index(t => t.UserID);

			RenameColumn("dbo.PropertySearchHistory", "UrlPath", "SearchQuery");
        }
        
        public override void Down()
        {
			RenameColumn("dbo.PropertySearchHistory", "SearchQuery", "UrlPath");
			DropForeignKey("dbo.SavedPropertySearchUpdateHistory", "UserID", "dbo.User");
            DropForeignKey("dbo.SavedPropertySearchUpdateHistory", "SessionID", "dbo.HttpSession");
            DropForeignKey("dbo.SavedPropertySearchUpdateHistory", "SavedSearchID", "dbo.SavedPropertySearch");
            DropForeignKey("dbo.SavedPropertySearchSmsNotification", "SavedSearchID", "dbo.SavedPropertySearch");
            DropForeignKey("dbo.SavedPropertySearchSmsNotification", "PropertyListingID", "dbo.PropertyListing");
            DropForeignKey("dbo.SavedPropertySearchSmsNotification", "ContactMethodID", "dbo.UserContactMethod");
            DropForeignKey("dbo.SavedPropertySearchEmailNotification", "SavedSearchID", "dbo.SavedPropertySearch");
            DropForeignKey("dbo.SavedPropertySearchEmailNotification", "PropertyListingID", "dbo.PropertyListing");
            DropForeignKey("dbo.SavedPropertySearchEmailNotification", "ContactMethodID", "dbo.UserContactMethod");
            DropForeignKey("dbo.OutgoingSmsMessage", "TargetUserID", "dbo.User");
            DropForeignKey("dbo.OutgoingSmsMessage", "RetryForMessageID", "dbo.OutgoingSmsMessage");
            DropIndex("dbo.SavedPropertySearchUpdateHistory", new[] { "UserID" });
            DropIndex("dbo.SavedPropertySearchUpdateHistory", new[] { "SessionID" });
            DropIndex("dbo.SavedPropertySearchUpdateHistory", new[] { "SavedSearchID" });
            DropIndex("dbo.SavedPropertySearchSmsNotification", new[] { "SavedSearchID" });
            DropIndex("dbo.SavedPropertySearchSmsNotification", new[] { "PropertyListingID" });
            DropIndex("dbo.SavedPropertySearchSmsNotification", new[] { "ContactMethodID" });
            DropIndex("dbo.SavedPropertySearchEmailNotification", new[] { "SavedSearchID" });
            DropIndex("dbo.SavedPropertySearchEmailNotification", new[] { "PropertyListingID" });
            DropIndex("dbo.SavedPropertySearchEmailNotification", new[] { "ContactMethodID" });
            DropIndex("dbo.OutgoingSmsMessage", new[] { "TargetUserID" });
            DropIndex("dbo.OutgoingSmsMessage", new[] { "RetryForMessageID" });
            DropTable("dbo.SavedPropertySearchUpdateHistory");
            DropTable("dbo.SavedPropertySearchSmsNotification");
            DropTable("dbo.SavedPropertySearchEmailNotification");
            DropTable("dbo.OutgoingSmsMessage");
            DropTable("dbo.ConfigurationDataItem");
        }
    }
}
