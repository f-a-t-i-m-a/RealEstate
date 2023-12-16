namespace JahanJooy.RealEstate.Core.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1841 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserElectronicPayment", "ReviewedByUserID", "dbo.User");
            DropIndex("dbo.UserElectronicPayment", new[] { "ReviewedByUserID" });
            AddColumn("dbo.UserElectronicPayment", "PaymentGatewayProvider", c => c.Int(nullable: false));
            AddColumn("dbo.UserElectronicPayment", "BankPaymentResult", c => c.Boolean());
            AddColumn("dbo.UserElectronicPayment", "BankTransactionReferenceID", c => c.Long());
            AddColumn("dbo.UserElectronicPayment", "BankTraceNumber", c => c.Long());
            AddColumn("dbo.UserElectronicPayment", "BankReferenceNumber", c => c.Long());
            AddColumn("dbo.UserElectronicPayment", "BankTransactionDate", c => c.String(maxLength: 25));
            AddColumn("dbo.UserElectronicPayment", "BankVerifyPaymentResult", c => c.Boolean());
            AddColumn("dbo.UserElectronicPayment", "BankVerifyPaymentResultMessage", c => c.String(maxLength: 250));
            DropColumn("dbo.UserElectronicPayment", "VerifiedByBank");
            DropColumn("dbo.UserElectronicPayment", "ReviewedByUserID");
            DropColumn("dbo.UserElectronicPayment", "GatewayBank");
            DropColumn("dbo.UserElectronicPayment", "BankTransactionCode");
            DropColumn("dbo.UserElectronicPayment", "BankFollowUpCode");
            DropColumn("dbo.UserElectronicPayment", "BankVerificationResponse");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserElectronicPayment", "BankVerificationResponse", c => c.String(maxLength: 50));
            AddColumn("dbo.UserElectronicPayment", "BankFollowUpCode", c => c.String(maxLength: 50));
            AddColumn("dbo.UserElectronicPayment", "BankTransactionCode", c => c.String(maxLength: 50));
            AddColumn("dbo.UserElectronicPayment", "GatewayBank", c => c.Byte(nullable: false));
            AddColumn("dbo.UserElectronicPayment", "ReviewedByUserID", c => c.Long());
            AddColumn("dbo.UserElectronicPayment", "VerifiedByBank", c => c.Boolean());
            DropColumn("dbo.UserElectronicPayment", "BankVerifyPaymentResultMessage");
            DropColumn("dbo.UserElectronicPayment", "BankVerifyPaymentResult");
            DropColumn("dbo.UserElectronicPayment", "BankTransactionDate");
            DropColumn("dbo.UserElectronicPayment", "BankReferenceNumber");
            DropColumn("dbo.UserElectronicPayment", "BankTraceNumber");
            DropColumn("dbo.UserElectronicPayment", "BankTransactionReferenceID");
            DropColumn("dbo.UserElectronicPayment", "BankPaymentResult");
            DropColumn("dbo.UserElectronicPayment", "PaymentGatewayProvider");
            CreateIndex("dbo.UserElectronicPayment", "ReviewedByUserID");
            AddForeignKey("dbo.UserElectronicPayment", "ReviewedByUserID", "dbo.User", "ID");
        }
    }
}
