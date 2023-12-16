namespace JahanJooy.RealEstate.Core.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _158AgencyContent2 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.AgencyBranch", new[] { "VicinityID" });
            AlterColumn("dbo.Agency", "Name", c => c.String(maxLength: 80));
            AlterColumn("dbo.AgencyBranch", "VicinityID", c => c.Long());
            CreateIndex("dbo.AgencyBranch", "VicinityID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.AgencyBranch", new[] { "VicinityID" });
            AlterColumn("dbo.AgencyBranch", "VicinityID", c => c.Long(nullable: false));
            AlterColumn("dbo.Agency", "Name", c => c.String(nullable: false, maxLength: 80));
            CreateIndex("dbo.AgencyBranch", "VicinityID");
        }
    }
}
