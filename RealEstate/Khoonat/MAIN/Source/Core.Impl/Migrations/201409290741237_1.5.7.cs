namespace JahanJooy.RealEstate.Core.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Spatial;
    
    public partial class _157 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AgencyBranch", "GeographicLocation", c => c.Geography());
            AddColumn("dbo.AgencyBranch", "GeographicLocationType", c => c.Byte());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AgencyBranch", "GeographicLocationType");
            DropColumn("dbo.AgencyBranch", "GeographicLocation");
        }
    }
}
