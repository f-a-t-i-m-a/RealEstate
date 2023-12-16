namespace JahanJooy.RealEstate.Core.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _131Billing : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.User", "VerificationContactID", "dbo.UserContactMethod");
            DropIndex("dbo.User", new[] { "VerificationContactID" });
            CreateTable(
                "dbo.UserBillingTransaction",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        UserID = c.Long(nullable: false),
                        TransactionTime = c.DateTime(nullable: false),
                        CashDelta = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BonusDelta = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CashBalance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BonusBalance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CashTurnover = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BonusTurnover = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SourceType = c.Byte(nullable: false),
                        SourceID = c.Long(nullable: false),
                        IsReverse = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.User", t => t.UserID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.PromotionalBonusCoupon",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        CreatedByUserID = c.Long(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                        ExpirationTime = c.DateTime(),
                        ClaimTime = c.DateTime(),
                        CouponValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CouponNumber = c.String(maxLength: 20),
                        CouponPassword = c.String(maxLength: 20),
                        TargetUserID = c.Long(),
                        BillingState = c.Byte(nullable: false),
                        ForwardTransactionID = c.Long(),
                        ReverseTransactionID = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.User", t => t.CreatedByUserID)
                .ForeignKey("dbo.UserBillingTransaction", t => t.ForwardTransactionID)
                .ForeignKey("dbo.UserBillingTransaction", t => t.ReverseTransactionID)
                .ForeignKey("dbo.User", t => t.TargetUserID)
                .Index(t => t.CreatedByUserID)
                .Index(t => t.ForwardTransactionID)
                .Index(t => t.ReverseTransactionID)
                .Index(t => t.TargetUserID);
            
            CreateTable(
                "dbo.PromotionalBonus",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        CreatedByUserID = c.Long(),
                        Description = c.String(maxLength: 2000),
                        Reason = c.Byte(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                        BonusAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TargetUserID = c.Long(),
                        BillingState = c.Byte(nullable: false),
                        ForwardTransactionID = c.Long(),
                        ReverseTransactionID = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.User", t => t.CreatedByUserID)
                .ForeignKey("dbo.UserBillingTransaction", t => t.ForwardTransactionID)
                .ForeignKey("dbo.UserBillingTransaction", t => t.ReverseTransactionID)
                .ForeignKey("dbo.User", t => t.TargetUserID)
                .Index(t => t.CreatedByUserID)
                .Index(t => t.ForwardTransactionID)
                .Index(t => t.ReverseTransactionID)
                .Index(t => t.TargetUserID);
            
            CreateTable(
                "dbo.SavedPropertySearchPromotionalSms",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        CreationDate = c.DateTime(nullable: false),
                        PropertyListingID = c.Long(nullable: false),
                        MessageText = c.String(maxLength: 300),
                        NumberOfSmsSegments = c.Int(nullable: false),
                        TargetUserID = c.Long(),
                        BillingState = c.Byte(nullable: false),
                        ForwardTransactionID = c.Long(),
                        ReverseTransactionID = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.UserBillingTransaction", t => t.ForwardTransactionID)
                .ForeignKey("dbo.PropertyListing", t => t.PropertyListingID)
                .ForeignKey("dbo.UserBillingTransaction", t => t.ReverseTransactionID)
                .ForeignKey("dbo.User", t => t.TargetUserID)
                .Index(t => t.ForwardTransactionID)
                .Index(t => t.PropertyListingID)
                .Index(t => t.ReverseTransactionID)
                .Index(t => t.TargetUserID);
            
            CreateTable(
                "dbo.SavedPropertySearchPromotionalSmsNotDeliveredReturn",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        CreationDate = c.DateTime(nullable: false),
                        PropertyListingID = c.Long(nullable: false),
                        PromotionalSmsID = c.Long(nullable: false),
                        BillingState = c.Byte(nullable: false),
                        TargetUserID = c.Long(),
                        ForwardTransactionID = c.Long(),
                        ReverseTransactionID = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.UserBillingTransaction", t => t.ForwardTransactionID)
                .ForeignKey("dbo.SavedPropertySearchPromotionalSms", t => t.PromotionalSmsID)
                .ForeignKey("dbo.PropertyListing", t => t.PropertyListingID)
                .ForeignKey("dbo.UserBillingTransaction", t => t.ReverseTransactionID)
                .ForeignKey("dbo.User", t => t.TargetUserID)
                .Index(t => t.ForwardTransactionID)
                .Index(t => t.PromotionalSmsID)
                .Index(t => t.PropertyListingID)
                .Index(t => t.ReverseTransactionID)
                .Index(t => t.TargetUserID);
            
            CreateTable(
                "dbo.UserBalanceAdministrativeChange",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        CreatedByUserID = c.Long(nullable: false),
                        ReviewedByUserID = c.Long(),
                        Description = c.String(maxLength: 2000),
                        AdministrativeNotes = c.String(maxLength: 2000),
                        CreationTime = c.DateTime(nullable: false),
                        CompletionTime = c.DateTime(),
                        CashDelta = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BonusDelta = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TargetUserID = c.Long(),
                        BillingState = c.Byte(nullable: false),
                        ForwardTransactionID = c.Long(),
                        ReverseTransactionID = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.User", t => t.CreatedByUserID)
                .ForeignKey("dbo.UserBillingTransaction", t => t.ForwardTransactionID)
                .ForeignKey("dbo.UserBillingTransaction", t => t.ReverseTransactionID)
                .ForeignKey("dbo.User", t => t.ReviewedByUserID)
                .ForeignKey("dbo.User", t => t.TargetUserID)
                .Index(t => t.CreatedByUserID)
                .Index(t => t.ForwardTransactionID)
                .Index(t => t.ReverseTransactionID)
                .Index(t => t.ReviewedByUserID)
                .Index(t => t.TargetUserID);
            
            CreateTable(
                "dbo.UserElectronicPayment",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        VerifiedByBank = c.Boolean(),
                        ReviewedByUserID = c.Long(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreationTime = c.DateTime(nullable: false),
                        CompletionTime = c.DateTime(),
                        GatewayBank = c.Byte(nullable: false),
                        BankTransactionCode = c.String(maxLength: 50),
                        BankFollowUpCode = c.String(maxLength: 50),
                        BankVerificationResponse = c.String(maxLength: 50),
                        TargetUserID = c.Long(),
                        BillingState = c.Byte(nullable: false),
                        ForwardTransactionID = c.Long(),
                        ReverseTransactionID = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.UserBillingTransaction", t => t.ForwardTransactionID)
                .ForeignKey("dbo.UserBillingTransaction", t => t.ReverseTransactionID)
                .ForeignKey("dbo.User", t => t.ReviewedByUserID)
                .ForeignKey("dbo.User", t => t.TargetUserID)
                .Index(t => t.ForwardTransactionID)
                .Index(t => t.ReverseTransactionID)
                .Index(t => t.ReviewedByUserID)
                .Index(t => t.TargetUserID);
            
            CreateTable(
                "dbo.UserRefundRequest",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        ReviewedByUserID = c.Long(),
                        ClearedByUserID = c.Long(),
                        CreationTime = c.DateTime(nullable: false),
                        CompletionTime = c.DateTime(),
                        RequestedMaximumAmount = c.Boolean(nullable: false),
                        RequestedAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DeductibleBankTransactionFee = c.Decimal(precision: 18, scale: 2),
                        PayableAmount = c.Decimal(precision: 18, scale: 2),
                        TargetCardNumber = c.String(maxLength: 25),
                        TargetShebaNumber = c.String(maxLength: 35),
                        TargetBank = c.Byte(nullable: false),
                        TargetAccountHolderName = c.String(maxLength: 80),
                        UserEnteredReason = c.String(maxLength: 500),
                        UserEnteredDescription = c.String(maxLength: 500),
                        TargetUserID = c.Long(),
                        BillingState = c.Byte(nullable: false),
                        ForwardTransactionID = c.Long(),
                        ReverseTransactionID = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.User", t => t.ClearedByUserID)
                .ForeignKey("dbo.UserBillingTransaction", t => t.ForwardTransactionID)
                .ForeignKey("dbo.UserBillingTransaction", t => t.ReverseTransactionID)
                .ForeignKey("dbo.User", t => t.ReviewedByUserID)
                .ForeignKey("dbo.User", t => t.TargetUserID)
                .Index(t => t.ClearedByUserID)
                .Index(t => t.ForwardTransactionID)
                .Index(t => t.ReverseTransactionID)
                .Index(t => t.ReviewedByUserID)
                .Index(t => t.TargetUserID);
            
            CreateTable(
                "dbo.UserWireTransferPayment",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        ReviewedByUserID = c.Long(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreationTime = c.DateTime(nullable: false),
                        CompletionTime = c.DateTime(),
                        SourceBank = c.Byte(nullable: false),
                        SourceCardNumberLastDigits = c.String(maxLength: 10),
                        SourceAccountHolderName = c.String(maxLength: 80),
                        UserEnteredDate = c.DateTime(nullable: false),
                        UserEnteredDescription = c.String(maxLength: 500),
                        TargetBank = c.Byte(nullable: false),
                        FollowUpNumber = c.String(maxLength: 50),
                        TargetUserID = c.Long(),
                        BillingState = c.Byte(nullable: false),
                        ForwardTransactionID = c.Long(),
                        ReverseTransactionID = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.UserBillingTransaction", t => t.ForwardTransactionID)
                .ForeignKey("dbo.UserBillingTransaction", t => t.ReverseTransactionID)
                .ForeignKey("dbo.User", t => t.ReviewedByUserID)
                .ForeignKey("dbo.User", t => t.TargetUserID)
                .Index(t => t.ForwardTransactionID)
                .Index(t => t.ReverseTransactionID)
                .Index(t => t.ReviewedByUserID)
                .Index(t => t.TargetUserID);
            
            AddColumn("dbo.User", "ShowInUsersList", c => c.Boolean(nullable: false));
            AddColumn("dbo.User", "Activity", c => c.String(maxLength: 200));
            AddColumn("dbo.User", "About", c => c.String(maxLength: 2000));
            AddColumn("dbo.User", "Services", c => c.String(maxLength: 2000));
            AddColumn("dbo.User", "WorkBackground", c => c.String(maxLength: 2000));
            AddColumn("dbo.User", "Address", c => c.String(maxLength: 300));
            AddColumn("dbo.User", "WebSiteUrl", c => c.String(maxLength: 250));
            AddColumn("dbo.User", "IsVerified", c => c.Boolean(nullable: false));
            AddColumn("dbo.User", "ReputationScore", c => c.Int(nullable: false));
            AddColumn("dbo.UserContactMethod", "IsVerifiable", c => c.Boolean(nullable: false));
            AddColumn("dbo.SavedPropertySearchSmsNotification", "TargetUserID", c => c.Long());
            AddColumn("dbo.SavedPropertySearchSmsNotification", "BillingState", c => c.Byte(nullable: false));
            AddColumn("dbo.SavedPropertySearchSmsNotification", "ForwardTransactionID", c => c.Long());
            AddColumn("dbo.SavedPropertySearchSmsNotification", "ReverseTransactionID", c => c.Long());
            CreateIndex("dbo.SavedPropertySearchSmsNotification", "ForwardTransactionID");
            CreateIndex("dbo.SavedPropertySearchSmsNotification", "ReverseTransactionID");
            CreateIndex("dbo.SavedPropertySearchSmsNotification", "TargetUserID");
            AddForeignKey("dbo.SavedPropertySearchSmsNotification", "ForwardTransactionID", "dbo.UserBillingTransaction", "ID");
            AddForeignKey("dbo.SavedPropertySearchSmsNotification", "ReverseTransactionID", "dbo.UserBillingTransaction", "ID");
            AddForeignKey("dbo.SavedPropertySearchSmsNotification", "TargetUserID", "dbo.User", "ID");
            DropColumn("dbo.User", "VerificationMethod");
            DropColumn("dbo.User", "VerificationContactID");

            Sql("UPDATE [dbo].[UserContactMethod] SET IsVerifiable = 1 WHERE IsVerified = 1");
            Sql("UPDATE [dbo].[UserContactMethod] SET IsVerifiable = 1 WHERE ContactMethodType = 2");
            Sql("UPDATE [dbo].[UserContactMethod] SET IsVerifiable = 1 WHERE ContactMethodType = 1 AND ContactMethodText LIKE '+98 9%'");

            Sql("UPDATE [dbo].[User] SET IsVerified = 1 WHERE " +
                "EXISTS (SELECT * FROM UserContactMethod WHERE UserContactMethod.UserID = [User].ID AND UserContactMethod.IsDeleted = 0 AND UserContactMethod.IsVerified = 1 AND UserContactMethod.ContactMethodType = 1) " +
                "AND " +
                "EXISTS (SELECT * FROM UserContactMethod WHERE UserContactMethod.UserID = [User].ID AND UserContactMethod.IsDeleted = 0 AND UserContactMethod.IsVerified = 1 AND UserContactMethod.ContactMethodType = 2)");
        }
        
        public override void Down()
        {
            AddColumn("dbo.User", "VerificationContactID", c => c.Long());
            AddColumn("dbo.User", "VerificationMethod", c => c.Byte());
            DropForeignKey("dbo.UserWireTransferPayment", "TargetUserID", "dbo.User");
            DropForeignKey("dbo.UserWireTransferPayment", "ReviewedByUserID", "dbo.User");
            DropForeignKey("dbo.UserWireTransferPayment", "ReverseTransactionID", "dbo.UserBillingTransaction");
            DropForeignKey("dbo.UserWireTransferPayment", "ForwardTransactionID", "dbo.UserBillingTransaction");
            DropForeignKey("dbo.UserRefundRequest", "TargetUserID", "dbo.User");
            DropForeignKey("dbo.UserRefundRequest", "ReviewedByUserID", "dbo.User");
            DropForeignKey("dbo.UserRefundRequest", "ReverseTransactionID", "dbo.UserBillingTransaction");
            DropForeignKey("dbo.UserRefundRequest", "ForwardTransactionID", "dbo.UserBillingTransaction");
            DropForeignKey("dbo.UserRefundRequest", "ClearedByUserID", "dbo.User");
            DropForeignKey("dbo.UserElectronicPayment", "TargetUserID", "dbo.User");
            DropForeignKey("dbo.UserElectronicPayment", "ReviewedByUserID", "dbo.User");
            DropForeignKey("dbo.UserElectronicPayment", "ReverseTransactionID", "dbo.UserBillingTransaction");
            DropForeignKey("dbo.UserElectronicPayment", "ForwardTransactionID", "dbo.UserBillingTransaction");
            DropForeignKey("dbo.UserBalanceAdministrativeChange", "TargetUserID", "dbo.User");
            DropForeignKey("dbo.UserBalanceAdministrativeChange", "ReviewedByUserID", "dbo.User");
            DropForeignKey("dbo.UserBalanceAdministrativeChange", "ReverseTransactionID", "dbo.UserBillingTransaction");
            DropForeignKey("dbo.UserBalanceAdministrativeChange", "ForwardTransactionID", "dbo.UserBillingTransaction");
            DropForeignKey("dbo.UserBalanceAdministrativeChange", "CreatedByUserID", "dbo.User");
            DropForeignKey("dbo.SavedPropertySearchSmsNotification", "TargetUserID", "dbo.User");
            DropForeignKey("dbo.SavedPropertySearchSmsNotification", "ReverseTransactionID", "dbo.UserBillingTransaction");
            DropForeignKey("dbo.SavedPropertySearchSmsNotification", "ForwardTransactionID", "dbo.UserBillingTransaction");
            DropForeignKey("dbo.SavedPropertySearchPromotionalSmsNotDeliveredReturn", "TargetUserID", "dbo.User");
            DropForeignKey("dbo.SavedPropertySearchPromotionalSmsNotDeliveredReturn", "ReverseTransactionID", "dbo.UserBillingTransaction");
            DropForeignKey("dbo.SavedPropertySearchPromotionalSmsNotDeliveredReturn", "PropertyListingID", "dbo.PropertyListing");
            DropForeignKey("dbo.SavedPropertySearchPromotionalSmsNotDeliveredReturn", "PromotionalSmsID", "dbo.SavedPropertySearchPromotionalSms");
            DropForeignKey("dbo.SavedPropertySearchPromotionalSmsNotDeliveredReturn", "ForwardTransactionID", "dbo.UserBillingTransaction");
            DropForeignKey("dbo.SavedPropertySearchPromotionalSms", "TargetUserID", "dbo.User");
            DropForeignKey("dbo.SavedPropertySearchPromotionalSms", "ReverseTransactionID", "dbo.UserBillingTransaction");
            DropForeignKey("dbo.SavedPropertySearchPromotionalSms", "PropertyListingID", "dbo.PropertyListing");
            DropForeignKey("dbo.SavedPropertySearchPromotionalSms", "ForwardTransactionID", "dbo.UserBillingTransaction");
            DropForeignKey("dbo.PromotionalBonus", "TargetUserID", "dbo.User");
            DropForeignKey("dbo.PromotionalBonus", "ReverseTransactionID", "dbo.UserBillingTransaction");
            DropForeignKey("dbo.PromotionalBonus", "ForwardTransactionID", "dbo.UserBillingTransaction");
            DropForeignKey("dbo.PromotionalBonus", "CreatedByUserID", "dbo.User");
            DropForeignKey("dbo.PromotionalBonusCoupon", "TargetUserID", "dbo.User");
            DropForeignKey("dbo.PromotionalBonusCoupon", "ReverseTransactionID", "dbo.UserBillingTransaction");
            DropForeignKey("dbo.PromotionalBonusCoupon", "ForwardTransactionID", "dbo.UserBillingTransaction");
            DropForeignKey("dbo.PromotionalBonusCoupon", "CreatedByUserID", "dbo.User");
            DropForeignKey("dbo.UserBillingTransaction", "UserID", "dbo.User");
            DropIndex("dbo.UserWireTransferPayment", new[] { "TargetUserID" });
            DropIndex("dbo.UserWireTransferPayment", new[] { "ReviewedByUserID" });
            DropIndex("dbo.UserWireTransferPayment", new[] { "ReverseTransactionID" });
            DropIndex("dbo.UserWireTransferPayment", new[] { "ForwardTransactionID" });
            DropIndex("dbo.UserRefundRequest", new[] { "TargetUserID" });
            DropIndex("dbo.UserRefundRequest", new[] { "ReviewedByUserID" });
            DropIndex("dbo.UserRefundRequest", new[] { "ReverseTransactionID" });
            DropIndex("dbo.UserRefundRequest", new[] { "ForwardTransactionID" });
            DropIndex("dbo.UserRefundRequest", new[] { "ClearedByUserID" });
            DropIndex("dbo.UserElectronicPayment", new[] { "TargetUserID" });
            DropIndex("dbo.UserElectronicPayment", new[] { "ReviewedByUserID" });
            DropIndex("dbo.UserElectronicPayment", new[] { "ReverseTransactionID" });
            DropIndex("dbo.UserElectronicPayment", new[] { "ForwardTransactionID" });
            DropIndex("dbo.UserBalanceAdministrativeChange", new[] { "TargetUserID" });
            DropIndex("dbo.UserBalanceAdministrativeChange", new[] { "ReviewedByUserID" });
            DropIndex("dbo.UserBalanceAdministrativeChange", new[] { "ReverseTransactionID" });
            DropIndex("dbo.UserBalanceAdministrativeChange", new[] { "ForwardTransactionID" });
            DropIndex("dbo.UserBalanceAdministrativeChange", new[] { "CreatedByUserID" });
            DropIndex("dbo.SavedPropertySearchSmsNotification", new[] { "TargetUserID" });
            DropIndex("dbo.SavedPropertySearchSmsNotification", new[] { "ReverseTransactionID" });
            DropIndex("dbo.SavedPropertySearchSmsNotification", new[] { "ForwardTransactionID" });
            DropIndex("dbo.SavedPropertySearchPromotionalSmsNotDeliveredReturn", new[] { "TargetUserID" });
            DropIndex("dbo.SavedPropertySearchPromotionalSmsNotDeliveredReturn", new[] { "ReverseTransactionID" });
            DropIndex("dbo.SavedPropertySearchPromotionalSmsNotDeliveredReturn", new[] { "PropertyListingID" });
            DropIndex("dbo.SavedPropertySearchPromotionalSmsNotDeliveredReturn", new[] { "PromotionalSmsID" });
            DropIndex("dbo.SavedPropertySearchPromotionalSmsNotDeliveredReturn", new[] { "ForwardTransactionID" });
            DropIndex("dbo.SavedPropertySearchPromotionalSms", new[] { "TargetUserID" });
            DropIndex("dbo.SavedPropertySearchPromotionalSms", new[] { "ReverseTransactionID" });
            DropIndex("dbo.SavedPropertySearchPromotionalSms", new[] { "PropertyListingID" });
            DropIndex("dbo.SavedPropertySearchPromotionalSms", new[] { "ForwardTransactionID" });
            DropIndex("dbo.PromotionalBonus", new[] { "TargetUserID" });
            DropIndex("dbo.PromotionalBonus", new[] { "ReverseTransactionID" });
            DropIndex("dbo.PromotionalBonus", new[] { "ForwardTransactionID" });
            DropIndex("dbo.PromotionalBonus", new[] { "CreatedByUserID" });
            DropIndex("dbo.PromotionalBonusCoupon", new[] { "TargetUserID" });
            DropIndex("dbo.PromotionalBonusCoupon", new[] { "ReverseTransactionID" });
            DropIndex("dbo.PromotionalBonusCoupon", new[] { "ForwardTransactionID" });
            DropIndex("dbo.PromotionalBonusCoupon", new[] { "CreatedByUserID" });
            DropIndex("dbo.UserBillingTransaction", new[] { "UserID" });
            DropColumn("dbo.SavedPropertySearchSmsNotification", "ReverseTransactionID");
            DropColumn("dbo.SavedPropertySearchSmsNotification", "ForwardTransactionID");
            DropColumn("dbo.SavedPropertySearchSmsNotification", "BillingState");
            DropColumn("dbo.SavedPropertySearchSmsNotification", "TargetUserID");
            DropColumn("dbo.UserContactMethod", "IsVerifiable");
            DropColumn("dbo.User", "ReputationScore");
            DropColumn("dbo.User", "IsVerified");
            DropColumn("dbo.User", "WebSiteUrl");
            DropColumn("dbo.User", "Address");
            DropColumn("dbo.User", "WorkBackground");
            DropColumn("dbo.User", "Services");
            DropColumn("dbo.User", "About");
            DropColumn("dbo.User", "Activity");
            DropColumn("dbo.User", "ShowInUsersList");
            DropTable("dbo.UserWireTransferPayment");
            DropTable("dbo.UserRefundRequest");
            DropTable("dbo.UserElectronicPayment");
            DropTable("dbo.UserBalanceAdministrativeChange");
            DropTable("dbo.SavedPropertySearchPromotionalSmsNotDeliveredReturn");
            DropTable("dbo.SavedPropertySearchPromotionalSms");
            DropTable("dbo.PromotionalBonus");
            DropTable("dbo.PromotionalBonusCoupon");
            DropTable("dbo.UserBillingTransaction");
            CreateIndex("dbo.User", "VerificationContactID");
            AddForeignKey("dbo.User", "VerificationContactID", "dbo.UserContactMethod", "ID");
        }
    }
}
