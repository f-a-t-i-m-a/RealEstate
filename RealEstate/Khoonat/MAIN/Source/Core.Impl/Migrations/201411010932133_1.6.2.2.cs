namespace JahanJooy.RealEstate.Core.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1622 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AgencyBranch", "VicinityID", "dbo.Vicinity");
            DropIndex("dbo.AgencyBranch", new[] { "VicinityID" });
            AddColumn("dbo.SponsoredEntity", "SelectionProbabilityWeight", c => c.Double(nullable: false));
            DropColumn("dbo.Agency", "Name");
            DropColumn("dbo.Agency", "ManagerName");
            DropColumn("dbo.Agency", "Code");
            DropColumn("dbo.Agency", "Description");
            DropColumn("dbo.AgencyBranch", "BranchName");
            DropColumn("dbo.AgencyBranch", "BranchManagerName");
            DropColumn("dbo.AgencyBranch", "VicinityID");
            DropColumn("dbo.AgencyBranch", "GeographicLocation");
            DropColumn("dbo.AgencyBranch", "GeographicLocationType");
            DropColumn("dbo.AgencyBranch", "FullAddress");
            DropColumn("dbo.AgencyBranch", "GeoLocation");
            DropColumn("dbo.AgencyBranch", "Phone1");
            DropColumn("dbo.AgencyBranch", "Phone2");
            DropColumn("dbo.AgencyBranch", "CellPhone1");
            DropColumn("dbo.AgencyBranch", "CellPhone2");
            DropColumn("dbo.AgencyBranch", "Fax");
            DropColumn("dbo.AgencyBranch", "Email");
            DropColumn("dbo.AgencyBranch", "SmsNumber");
            DropColumn("dbo.AgencyBranch", "Description");
            DropColumn("dbo.AgencyBranch", "ActivityRegion");
            DropColumn("dbo.AgencyBranch", "GeoActivityRegion");

			Sql("UPDATE [dbo].[SponsoredEntity] SET [NextRecalcDue] = GETDATE() WHERE [NextRecalcDue] IS NOT NULL");
		}
        
        public override void Down()
        {
            AddColumn("dbo.AgencyBranch", "GeoActivityRegion", c => c.Geography());
            AddColumn("dbo.AgencyBranch", "ActivityRegion", c => c.String(maxLength: 1000));
            AddColumn("dbo.AgencyBranch", "Description", c => c.String(maxLength: 1000));
            AddColumn("dbo.AgencyBranch", "SmsNumber", c => c.String(maxLength: 30));
            AddColumn("dbo.AgencyBranch", "Email", c => c.String(maxLength: 50));
            AddColumn("dbo.AgencyBranch", "Fax", c => c.String(maxLength: 30));
            AddColumn("dbo.AgencyBranch", "CellPhone2", c => c.String(maxLength: 30));
            AddColumn("dbo.AgencyBranch", "CellPhone1", c => c.String(maxLength: 30));
            AddColumn("dbo.AgencyBranch", "Phone2", c => c.String(maxLength: 30));
            AddColumn("dbo.AgencyBranch", "Phone1", c => c.String(maxLength: 30));
            AddColumn("dbo.AgencyBranch", "GeoLocation", c => c.Geography());
            AddColumn("dbo.AgencyBranch", "FullAddress", c => c.String(maxLength: 1000));
            AddColumn("dbo.AgencyBranch", "GeographicLocationType", c => c.Byte());
            AddColumn("dbo.AgencyBranch", "GeographicLocation", c => c.Geography());
            AddColumn("dbo.AgencyBranch", "VicinityID", c => c.Long());
            AddColumn("dbo.AgencyBranch", "BranchManagerName", c => c.String(maxLength: 80));
            AddColumn("dbo.AgencyBranch", "BranchName", c => c.String(maxLength: 80));
            AddColumn("dbo.Agency", "Description", c => c.String(maxLength: 1000));
            AddColumn("dbo.Agency", "Code", c => c.String(maxLength: 50));
            AddColumn("dbo.Agency", "ManagerName", c => c.String(maxLength: 80));
            AddColumn("dbo.Agency", "Name", c => c.String(maxLength: 80));
            DropColumn("dbo.SponsoredEntity", "SelectionProbabilityWeight");
            CreateIndex("dbo.AgencyBranch", "VicinityID");
            AddForeignKey("dbo.AgencyBranch", "VicinityID", "dbo.Vicinity", "ID");
        }
    }
}
