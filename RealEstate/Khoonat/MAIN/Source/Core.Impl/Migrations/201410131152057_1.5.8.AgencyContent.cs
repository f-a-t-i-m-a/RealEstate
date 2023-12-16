namespace JahanJooy.RealEstate.Core.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _158AgencyContent : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.SavedPropertySearchGeographicRegion", new[] { "VicinityID" });
            AddColumn("dbo.SponsoredEntity", "LastUnblockTime", c => c.DateTime());
            AddColumn("dbo.Agency", "IndexedTime", c => c.DateTime());
            AddColumn("dbo.Agency", "ContentString", c => c.String());
            AddColumn("dbo.AgencyBranch", "IndexedTime", c => c.DateTime());
            AddColumn("dbo.AgencyBranch", "ContentString", c => c.String());
            AddColumn("dbo.PropertyRequest", "ContentString", c => c.String());
            AlterColumn("dbo.SponsoredPropertyListing", "IgnoreSearchQuery", c => c.Boolean(nullable: false));
            AlterColumn("dbo.SavedPropertySearchGeographicRegion", "VicinityID", c => c.Long(nullable: false));
            CreateIndex("dbo.SavedPropertySearchGeographicRegion", "VicinityID");
            DropColumn("dbo.Estate", "Address2");
            DropColumn("dbo.Estate", "PlateNumber");
            DropColumn("dbo.Vicinity", "OriginalProvinceID");
            DropColumn("dbo.Vicinity", "OriginalCityID");
            DropColumn("dbo.Vicinity", "OriginalCityRegionID");
            DropColumn("dbo.Vicinity", "OriginalNeighborhoodID");
            DropColumn("dbo.Unit", "BlockNumber");
            DropColumn("dbo.Unit", "FlatNumber");
            DropColumn("dbo.Unit", "AdditionalAddressInfo");
            DropColumn("dbo.PropertyRequest", "ContentsString");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PropertyRequest", "ContentsString", c => c.String());
            AddColumn("dbo.Unit", "AdditionalAddressInfo", c => c.String(maxLength: 1000));
            AddColumn("dbo.Unit", "FlatNumber", c => c.String(maxLength: 20));
            AddColumn("dbo.Unit", "BlockNumber", c => c.String(maxLength: 20));
            AddColumn("dbo.Vicinity", "OriginalNeighborhoodID", c => c.Long());
            AddColumn("dbo.Vicinity", "OriginalCityRegionID", c => c.Long());
            AddColumn("dbo.Vicinity", "OriginalCityID", c => c.Long());
            AddColumn("dbo.Vicinity", "OriginalProvinceID", c => c.Long());
            AddColumn("dbo.Estate", "PlateNumber", c => c.String(maxLength: 20));
            AddColumn("dbo.Estate", "Address2", c => c.String(maxLength: 150));
            DropIndex("dbo.SavedPropertySearchGeographicRegion", new[] { "VicinityID" });
            AlterColumn("dbo.SavedPropertySearchGeographicRegion", "VicinityID", c => c.Long());
            AlterColumn("dbo.SponsoredPropertyListing", "IgnoreSearchQuery", c => c.Boolean());
            DropColumn("dbo.PropertyRequest", "ContentString");
            DropColumn("dbo.AgencyBranch", "ContentString");
            DropColumn("dbo.AgencyBranch", "IndexedTime");
            DropColumn("dbo.Agency", "ContentString");
            DropColumn("dbo.Agency", "IndexedTime");
            DropColumn("dbo.SponsoredEntity", "LastUnblockTime");
            CreateIndex("dbo.SavedPropertySearchGeographicRegion", "VicinityID");
        }
    }
}
