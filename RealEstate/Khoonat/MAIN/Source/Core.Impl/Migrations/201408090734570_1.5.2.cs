namespace JahanJooy.RealEstate.Core.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _152 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SavedPropertySearchGeographicRegion", "NeighborhoodID", "dbo.Neighborhood");
            DropIndex("dbo.SavedPropertySearchGeographicRegion", new[] { "NeighborhoodID" });
            AlterColumn("dbo.SavedPropertySearchGeographicRegion", "NeighborhoodID", c => c.Long());
            CreateIndex("dbo.SavedPropertySearchGeographicRegion", "NeighborhoodID");
            AddForeignKey("dbo.SavedPropertySearchGeographicRegion", "NeighborhoodID", "dbo.Neighborhood", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SavedPropertySearchGeographicRegion", "NeighborhoodID", "dbo.Neighborhood");
            DropIndex("dbo.SavedPropertySearchGeographicRegion", new[] { "NeighborhoodID" });
            AlterColumn("dbo.SavedPropertySearchGeographicRegion", "NeighborhoodID", c => c.Long(nullable: false));
            CreateIndex("dbo.SavedPropertySearchGeographicRegion", "NeighborhoodID");
            AddForeignKey("dbo.SavedPropertySearchGeographicRegion", "NeighborhoodID", "dbo.Neighborhood", "ID", cascadeDelete: true);
        }
    }
}
