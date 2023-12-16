namespace JahanJooy.RealEstate.Core.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _144 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SavedPropertySearchSmsNotification", "ForwardTransactionID", "dbo.UserBillingTransaction");
            DropForeignKey("dbo.SavedPropertySearchSmsNotification", "ReverseTransactionID", "dbo.UserBillingTransaction");
            DropForeignKey("dbo.SponsoredPropertyListing", "ListingID", "dbo.PropertyListing");
            DropForeignKey("dbo.SponsoredPropertyListing", "SponsoredEntityID", "dbo.SponsoredEntity");
            DropForeignKey("dbo.SavedPropertySearchSmsNotification", "TargetUserID", "dbo.User");
            DropIndex("dbo.SavedPropertySearchSmsNotification", new[] { "ForwardTransactionID" });
            DropIndex("dbo.SavedPropertySearchSmsNotification", new[] { "ReverseTransactionID" });
            DropIndex("dbo.SponsoredPropertyListing", new[] { "ListingID" });
            DropIndex("dbo.SponsoredPropertyListing", new[] { "SponsoredEntityID" });
            DropIndex("dbo.SavedPropertySearchSmsNotification", new[] { "TargetUserID" });
            CreateTable(
                "dbo.SavedSearchSmsNotificationBilling",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        CreationTime = c.DateTime(nullable: false),
                        CompletionTime = c.DateTime(),
                        NotificationsDate = c.DateTime(nullable: false),
                        NumberOfNotifications = c.Int(nullable: false),
                        NumberOfNotificationParts = c.Int(nullable: false),
                        TargetUserID = c.Long(),
                        BillingState = c.Byte(nullable: false),
                        ForwardTransactionID = c.Long(),
                        ReverseTransactionID = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.UserBillingTransaction", t => t.ForwardTransactionID)
                .ForeignKey("dbo.UserBillingTransaction", t => t.ReverseTransactionID)
                .ForeignKey("dbo.User", t => t.TargetUserID)
                .Index(t => t.ForwardTransactionID)
                .Index(t => t.ReverseTransactionID)
                .Index(t => t.TargetUserID);

            Sql("DELETE FROM dbo.SavedPropertySearchSmsNotification");

            AddColumn("dbo.UserBillingTransaction", "IsPartial", c => c.Boolean(nullable: false));
            AddColumn("dbo.UserBillingTransaction", "HasPartialInHistory", c => c.Boolean(nullable: false));
            AddColumn("dbo.Vicinity", "Enabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.SavedPropertySearchSmsNotification", "BillingID", c => c.Long());
            AlterColumn("dbo.SavedPropertySearchSmsNotification", "TargetUserID", c => c.Long(nullable: false));
            CreateIndex("dbo.SavedPropertySearchSmsNotification", "BillingID");
            CreateIndex("dbo.SponsoredPropertyListing", "ListingID");
            CreateIndex("dbo.SponsoredPropertyListing", "SponsoredEntityID");
            CreateIndex("dbo.SavedPropertySearchSmsNotification", "TargetUserID");
            AddForeignKey("dbo.SavedPropertySearchSmsNotification", "BillingID", "dbo.SavedSearchSmsNotificationBilling", "ID");
            AddForeignKey("dbo.SponsoredPropertyListing", "ListingID", "dbo.PropertyListing", "ID");
            AddForeignKey("dbo.SponsoredPropertyListing", "SponsoredEntityID", "dbo.SponsoredEntity", "ID");
            AddForeignKey("dbo.SavedPropertySearchSmsNotification", "TargetUserID", "dbo.User", "ID");
            DropColumn("dbo.SavedPropertySearchSmsNotification", "BillingState");
            DropColumn("dbo.SavedPropertySearchSmsNotification", "ForwardTransactionID");
            DropColumn("dbo.SavedPropertySearchSmsNotification", "ReverseTransactionID");

            RenameColumn("dbo.SavedPropertySearchSmsNotification", "CreationDate", "CreationTime");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SavedPropertySearchSmsNotification", "ReverseTransactionID", c => c.Long());
            AddColumn("dbo.SavedPropertySearchSmsNotification", "ForwardTransactionID", c => c.Long());
            AddColumn("dbo.SavedPropertySearchSmsNotification", "BillingState", c => c.Byte(nullable: false));
            AddColumn("dbo.SavedPropertySearchSmsNotification", "CreationDate", c => c.DateTime(nullable: false));
            DropForeignKey("dbo.SavedPropertySearchSmsNotification", "TargetUserID", "dbo.User");
            DropForeignKey("dbo.SponsoredPropertyListing", "SponsoredEntityID", "dbo.SponsoredEntity");
            DropForeignKey("dbo.SponsoredPropertyListing", "ListingID", "dbo.PropertyListing");
            DropForeignKey("dbo.SavedPropertySearchSmsNotification", "BillingID", "dbo.SavedSearchSmsNotificationBilling");
            DropForeignKey("dbo.SavedSearchSmsNotificationBilling", "TargetUserID", "dbo.User");
            DropForeignKey("dbo.SavedSearchSmsNotificationBilling", "ReverseTransactionID", "dbo.UserBillingTransaction");
            DropForeignKey("dbo.SavedSearchSmsNotificationBilling", "ForwardTransactionID", "dbo.UserBillingTransaction");
            DropIndex("dbo.SavedPropertySearchSmsNotification", new[] { "TargetUserID" });
            DropIndex("dbo.SponsoredPropertyListing", new[] { "SponsoredEntityID" });
            DropIndex("dbo.SponsoredPropertyListing", new[] { "ListingID" });
            DropIndex("dbo.SavedPropertySearchSmsNotification", new[] { "BillingID" });
            DropIndex("dbo.SavedSearchSmsNotificationBilling", new[] { "TargetUserID" });
            DropIndex("dbo.SavedSearchSmsNotificationBilling", new[] { "ReverseTransactionID" });
            DropIndex("dbo.SavedSearchSmsNotificationBilling", new[] { "ForwardTransactionID" });
            AlterColumn("dbo.SavedPropertySearchSmsNotification", "TargetUserID", c => c.Long());
            DropColumn("dbo.SavedPropertySearchSmsNotification", "BillingID");
            DropColumn("dbo.SavedPropertySearchSmsNotification", "CreationTime");
            DropColumn("dbo.Vicinity", "Enabled");
            DropColumn("dbo.UserBillingTransaction", "HasPartialInHistory");
            DropColumn("dbo.UserBillingTransaction", "IsPartial");
            DropTable("dbo.SavedSearchSmsNotificationBilling");
            CreateIndex("dbo.SavedPropertySearchSmsNotification", "TargetUserID");
            CreateIndex("dbo.SponsoredPropertyListing", "SponsoredEntityID");
            CreateIndex("dbo.SponsoredPropertyListing", "ListingID");
            CreateIndex("dbo.SavedPropertySearchSmsNotification", "ReverseTransactionID");
            CreateIndex("dbo.SavedPropertySearchSmsNotification", "ForwardTransactionID");
            AddForeignKey("dbo.SavedPropertySearchSmsNotification", "TargetUserID", "dbo.User", "ID");
            AddForeignKey("dbo.SponsoredPropertyListing", "SponsoredEntityID", "dbo.SponsoredEntity", "ID", cascadeDelete: true);
            AddForeignKey("dbo.SponsoredPropertyListing", "ListingID", "dbo.PropertyListing", "ID", cascadeDelete: true);
            AddForeignKey("dbo.SavedPropertySearchSmsNotification", "ReverseTransactionID", "dbo.UserBillingTransaction", "ID");
            AddForeignKey("dbo.SavedPropertySearchSmsNotification", "ForwardTransactionID", "dbo.UserBillingTransaction", "ID");
        }
    }
}
