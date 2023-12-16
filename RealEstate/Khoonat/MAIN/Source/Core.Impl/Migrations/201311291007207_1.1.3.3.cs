namespace JahanJooy.RealEstate.Core.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1133 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ContactInfo", "ContactPhone1Verified", c => c.Boolean(nullable: false));
            AddColumn("dbo.ContactInfo", "ContactPhone2Verified", c => c.Boolean(nullable: false));
            AddColumn("dbo.ContactInfo", "ContactEmailVerified", c => c.Boolean(nullable: false));
            AddColumn("dbo.PropertyListing", "IsAgencyListing", c => c.Boolean(nullable: false));
            AddColumn("dbo.PropertyListing", "IsAgencyActivityAllowed", c => c.Boolean(nullable: false));
            AddColumn("dbo.OutgoingSmsMessage", "TransmissionDate", c => c.DateTime());

            Sql("UPDATE [dbo].[PropertyListing] SET IsAgencyListing = " +
                "  IIF((SELECT ContactRole FROM ContactInfo WHERE ContactInfo.ID = PropertyListing.ContactInfoID) = 2, 1, 0);");
            Sql("UPDATE [dbo].[PropertyListing] SET IsAgencyActivityAllowed = 1;");

            RenameTable("dbo.ContactInfo", "PropertyListingContactInfo");
        }
        
        public override void Down()
        {
            RenameTable("PropertyListingContactInfo", "dbo.ContactInfo");

            DropColumn("dbo.OutgoingSmsMessage", "TransmissionDate");
            DropColumn("dbo.PropertyListing", "IsAgencyActivityAllowed");
            DropColumn("dbo.PropertyListing", "IsAgencyListing");
            DropColumn("dbo.ContactInfo", "ContactEmailVerified");
            DropColumn("dbo.ContactInfo", "ContactPhone2Verified");
            DropColumn("dbo.ContactInfo", "ContactPhone1Verified");
        }
    }
}
