namespace JahanJooy.RealEstate.Core.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1842 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserElectronicPayment", "BankAmount", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.UserElectronicPayment", "BankInvoiceNumber", c => c.String(maxLength: 30));
            AddColumn("dbo.UserElectronicPayment", "BankInvoiceDate", c => c.String(maxLength: 30));
            AddColumn("dbo.UserElectronicPayment", "BankMerchantCode", c => c.String(maxLength: 30));
            AddColumn("dbo.UserElectronicPayment", "BankTerminalCode", c => c.String(maxLength: 30));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserElectronicPayment", "BankTerminalCode");
            DropColumn("dbo.UserElectronicPayment", "BankMerchantCode");
            DropColumn("dbo.UserElectronicPayment", "BankInvoiceDate");
            DropColumn("dbo.UserElectronicPayment", "BankInvoiceNumber");
            DropColumn("dbo.UserElectronicPayment", "BankAmount");
        }
    }
}
