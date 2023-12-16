namespace JahanJooy.RealEstate.Core.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _154 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SponsoredPropertyListing", "Approved", c => c.Boolean());
            AddColumn("dbo.SponsoredPropertyListing", "IgnoreSearchQuery", c => c.Boolean());
            AddColumn("dbo.SponsoredPropertyListing", "CustomCaption", c => c.String(maxLength: 200));

            Sql("UPDATE [dbo].[SponsoredPropertyListing] SET IgnoreSearchQuery = 0");
        }
        
        public override void Down()
        {
            DropColumn("dbo.SponsoredPropertyListing", "CustomCaption");
            DropColumn("dbo.SponsoredPropertyListing", "IgnoreSearchQuery");
            DropColumn("dbo.SponsoredPropertyListing", "Approved");
        }
    }
}
