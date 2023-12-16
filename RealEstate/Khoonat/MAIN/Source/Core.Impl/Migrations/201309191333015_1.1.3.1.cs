namespace JahanJooy.RealEstate.Core.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1131 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SavedPropertySearch",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Title = c.String(maxLength: 100),
                        UserID = c.Long(nullable: false),
                        CreatorSessionID = c.Long(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        DeleteDate = c.DateTime(),
                        PropertyType = c.Byte(),
                        IntentionOfOwner = c.Byte(),
                        ProvinceID = c.Long(),
                        CityID = c.Long(),
                        CityRegionID = c.Long(),
                        AdditionalFilters = c.String(maxLength: 2000),
                        SendNotificationEmails = c.Boolean(nullable: false),
                        SendPromotionalSmsMessages = c.Boolean(nullable: false),
                        SendPaidSmsMessages = c.Boolean(nullable: false),
                        SendNotificationsUntil = c.DateTime(),
                        NumberOfNotificationEmailsSent = c.Long(nullable: false),
                        NumberOfPromotionalSmsMessagesSent = c.Long(nullable: false),
                        NumberOfPaidSmsMessagesSent = c.Long(nullable: false),
                        EmailNotificationTargetID = c.Long(),
                        SmsNotificationTargetID = c.Long(),
                        EmailNotificationType = c.Byte(),
                        SmsNotificationParts = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.User", t => t.UserID, cascadeDelete: true)
                .ForeignKey("dbo.HttpSession", t => t.CreatorSessionID)
                .ForeignKey("dbo.Province", t => t.ProvinceID, cascadeDelete: true)
                .ForeignKey("dbo.City", t => t.CityID, cascadeDelete: true)
                .ForeignKey("dbo.CityRegion", t => t.CityRegionID, cascadeDelete: true)
                .ForeignKey("dbo.UserContactMethod", t => t.EmailNotificationTargetID)
                .ForeignKey("dbo.UserContactMethod", t => t.SmsNotificationTargetID)
                .Index(t => t.UserID)
                .Index(t => t.CreatorSessionID)
                .Index(t => t.ProvinceID)
                .Index(t => t.CityID)
                .Index(t => t.CityRegionID)
                .Index(t => t.EmailNotificationTargetID)
                .Index(t => t.SmsNotificationTargetID);
            
            CreateTable(
                "dbo.SavedPropertySearchGeographicRegion",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        PropertySearchID = c.Long(nullable: false),
                        NeighborhoodID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.SavedPropertySearch", t => t.PropertySearchID, cascadeDelete: true)
                .ForeignKey("dbo.Neighborhood", t => t.NeighborhoodID, cascadeDelete: true)
                .Index(t => t.PropertySearchID)
                .Index(t => t.NeighborhoodID);
            
            CreateTable(
                "dbo.ScheduledTask",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        TaskKey = c.String(nullable: false, maxLength: 100),
                        TaskProgress = c.String(maxLength: 2000),
                        LastExecutionTime = c.DateTime(),
                        LastErrorTime = c.DateTime(),
                        NumberOfExecutions = c.Long(nullable: false),
                        NumberOfErrors = c.Long(nullable: false),
                        LastExecutionResult = c.String(maxLength: 2000),
                        LastErrorMessage = c.String(maxLength: 2000),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.SavedPropertySearchGeographicRegion", new[] { "NeighborhoodID" });
            DropIndex("dbo.SavedPropertySearchGeographicRegion", new[] { "PropertySearchID" });
            DropIndex("dbo.SavedPropertySearch", new[] { "SmsNotificationTargetID" });
            DropIndex("dbo.SavedPropertySearch", new[] { "EmailNotificationTargetID" });
            DropIndex("dbo.SavedPropertySearch", new[] { "CityRegionID" });
            DropIndex("dbo.SavedPropertySearch", new[] { "CityID" });
            DropIndex("dbo.SavedPropertySearch", new[] { "ProvinceID" });
            DropIndex("dbo.SavedPropertySearch", new[] { "CreatorSessionID" });
            DropIndex("dbo.SavedPropertySearch", new[] { "UserID" });
            DropForeignKey("dbo.SavedPropertySearchGeographicRegion", "NeighborhoodID", "dbo.Neighborhood");
            DropForeignKey("dbo.SavedPropertySearchGeographicRegion", "PropertySearchID", "dbo.SavedPropertySearch");
            DropForeignKey("dbo.SavedPropertySearch", "SmsNotificationTargetID", "dbo.UserContactMethod");
            DropForeignKey("dbo.SavedPropertySearch", "EmailNotificationTargetID", "dbo.UserContactMethod");
            DropForeignKey("dbo.SavedPropertySearch", "CityRegionID", "dbo.CityRegion");
            DropForeignKey("dbo.SavedPropertySearch", "CityID", "dbo.City");
            DropForeignKey("dbo.SavedPropertySearch", "ProvinceID", "dbo.Province");
            DropForeignKey("dbo.SavedPropertySearch", "CreatorSessionID", "dbo.HttpSession");
            DropForeignKey("dbo.SavedPropertySearch", "UserID", "dbo.User");
            DropTable("dbo.ScheduledTask");
            DropTable("dbo.SavedPropertySearchGeographicRegion");
            DropTable("dbo.SavedPropertySearch");
        }
    }
}
