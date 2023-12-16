namespace JahanJooy.RealEstate.Core.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _162 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CityAdjacencies", "BaseCityID", "dbo.City");
            DropForeignKey("dbo.CityAdjacencies", "AdjacentCityID", "dbo.City");
            DropForeignKey("dbo.NeighborhoodAdjacencies", "BaseNeighborhoodID", "dbo.Neighborhood");
            DropForeignKey("dbo.NeighborhoodAdjacencies", "AdjacentNeighborhoodID", "dbo.Neighborhood");
            DropForeignKey("dbo.Neighborhood", "CityID", "dbo.City");
            DropForeignKey("dbo.CityRegionAdjacencies", "BaseCityRegionID", "dbo.CityRegion");
            DropForeignKey("dbo.CityRegionAdjacencies", "AdjacentCityRegionID", "dbo.CityRegion");
            DropForeignKey("dbo.CityRegion", "CityID", "dbo.City");
            DropForeignKey("dbo.CityRegionNeighborhoodRelation", "CityRegion_ID", "dbo.CityRegion");
            DropForeignKey("dbo.CityRegionNeighborhoodRelation", "Neighborhood_ID", "dbo.Neighborhood");
            DropForeignKey("dbo.NeighborhoodSynonyms", "BaseNeighborhoodID", "dbo.Neighborhood");
            DropForeignKey("dbo.NeighborhoodSynonyms", "SynonymNeighborhoodID", "dbo.Neighborhood");
            DropForeignKey("dbo.ProvinceAdjacencies", "BaseProvinceID", "dbo.Province");
            DropForeignKey("dbo.ProvinceAdjacencies", "AdjacentProvinceID", "dbo.Province");
            DropForeignKey("dbo.City", "ProvinceID", "dbo.Province");
            DropForeignKey("dbo.Estate", "CityID", "dbo.City");
            DropForeignKey("dbo.Estate", "CityRegionID", "dbo.CityRegion");
            DropForeignKey("dbo.Estate", "NeighborhoodID", "dbo.Neighborhood");
            DropForeignKey("dbo.Estate", "ProvinceID", "dbo.Province");
            DropForeignKey("dbo.PropertyListing", "CityID", "dbo.City");
            DropForeignKey("dbo.PropertyListing", "CityRegionID", "dbo.CityRegion");
            DropForeignKey("dbo.PropertyListing", "NeighborhoodID", "dbo.Neighborhood");
            DropForeignKey("dbo.PropertyListing", "ProvinceID", "dbo.Province");
            DropForeignKey("dbo.SavedPropertySearch", "CityID", "dbo.City");
            DropForeignKey("dbo.SavedPropertySearch", "CityRegionID", "dbo.CityRegion");
            DropForeignKey("dbo.SavedPropertySearchGeographicRegion", "NeighborhoodID", "dbo.Neighborhood");
            DropForeignKey("dbo.SavedPropertySearch", "ProvinceID", "dbo.Province");
            DropIndex("dbo.PropertyListing", new[] { "ProvinceID" });
            DropIndex("dbo.PropertyListing", new[] { "CityID" });
            DropIndex("dbo.PropertyListing", new[] { "CityRegionID" });
            DropIndex("dbo.PropertyListing", new[] { "NeighborhoodID" });
            DropIndex("dbo.Estate", new[] { "ProvinceID" });
            DropIndex("dbo.Estate", new[] { "CityID" });
            DropIndex("dbo.Estate", new[] { "CityRegionID" });
            DropIndex("dbo.Estate", new[] { "NeighborhoodID" });
            DropIndex("dbo.City", new[] { "ProvinceID" });
            DropIndex("dbo.Neighborhood", new[] { "CityID" });
            DropIndex("dbo.CityRegion", new[] { "CityID" });
            DropIndex("dbo.SavedPropertySearch", new[] { "ProvinceID" });
            DropIndex("dbo.SavedPropertySearch", new[] { "CityID" });
            DropIndex("dbo.SavedPropertySearch", new[] { "CityRegionID" });
            DropIndex("dbo.SavedPropertySearchGeographicRegion", new[] { "NeighborhoodID" });
            DropIndex("dbo.CityAdjacencies", new[] { "BaseCityID" });
            DropIndex("dbo.CityAdjacencies", new[] { "AdjacentCityID" });
            DropIndex("dbo.NeighborhoodAdjacencies", new[] { "BaseNeighborhoodID" });
            DropIndex("dbo.NeighborhoodAdjacencies", new[] { "AdjacentNeighborhoodID" });
            DropIndex("dbo.CityRegionAdjacencies", new[] { "BaseCityRegionID" });
            DropIndex("dbo.CityRegionAdjacencies", new[] { "AdjacentCityRegionID" });
            DropIndex("dbo.CityRegionNeighborhoodRelation", new[] { "CityRegion_ID" });
            DropIndex("dbo.CityRegionNeighborhoodRelation", new[] { "Neighborhood_ID" });
            DropIndex("dbo.NeighborhoodSynonyms", new[] { "BaseNeighborhoodID" });
            DropIndex("dbo.NeighborhoodSynonyms", new[] { "SynonymNeighborhoodID" });
            DropIndex("dbo.ProvinceAdjacencies", new[] { "BaseProvinceID" });
            DropIndex("dbo.ProvinceAdjacencies", new[] { "AdjacentProvinceID" });
            DropColumn("dbo.PropertyListing", "ProvinceID");
            DropColumn("dbo.PropertyListing", "CityID");
            DropColumn("dbo.PropertyListing", "CityRegionID");
            DropColumn("dbo.PropertyListing", "NeighborhoodID");
            DropColumn("dbo.Estate", "ProvinceID");
            DropColumn("dbo.Estate", "CityID");
            DropColumn("dbo.Estate", "CityRegionID");
            DropColumn("dbo.Estate", "NeighborhoodID");
            DropColumn("dbo.SavedPropertySearch", "ProvinceID");
            DropColumn("dbo.SavedPropertySearch", "CityID");
            DropColumn("dbo.SavedPropertySearch", "CityRegionID");
            DropColumn("dbo.SavedPropertySearchGeographicRegion", "NeighborhoodID");
            DropTable("dbo.City");
            DropTable("dbo.Neighborhood");
            DropTable("dbo.CityRegion");
            DropTable("dbo.Province");
            DropTable("dbo.CityAdjacencies");
            DropTable("dbo.NeighborhoodAdjacencies");
            DropTable("dbo.CityRegionAdjacencies");
            DropTable("dbo.CityRegionNeighborhoodRelation");
            DropTable("dbo.NeighborhoodSynonyms");
            DropTable("dbo.ProvinceAdjacencies");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ProvinceAdjacencies",
                c => new
                    {
                        BaseProvinceID = c.Long(nullable: false),
                        AdjacentProvinceID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.BaseProvinceID, t.AdjacentProvinceID });
            
            CreateTable(
                "dbo.NeighborhoodSynonyms",
                c => new
                    {
                        BaseNeighborhoodID = c.Long(nullable: false),
                        SynonymNeighborhoodID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.BaseNeighborhoodID, t.SynonymNeighborhoodID });
            
            CreateTable(
                "dbo.CityRegionNeighborhoodRelation",
                c => new
                    {
                        CityRegion_ID = c.Long(nullable: false),
                        Neighborhood_ID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.CityRegion_ID, t.Neighborhood_ID });
            
            CreateTable(
                "dbo.CityRegionAdjacencies",
                c => new
                    {
                        BaseCityRegionID = c.Long(nullable: false),
                        AdjacentCityRegionID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.BaseCityRegionID, t.AdjacentCityRegionID });
            
            CreateTable(
                "dbo.NeighborhoodAdjacencies",
                c => new
                    {
                        BaseNeighborhoodID = c.Long(nullable: false),
                        AdjacentNeighborhoodID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.BaseNeighborhoodID, t.AdjacentNeighborhoodID });
            
            CreateTable(
                "dbo.CityAdjacencies",
                c => new
                    {
                        BaseCityID = c.Long(nullable: false),
                        AdjacentCityID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.BaseCityID, t.AdjacentCityID });
            
            CreateTable(
                "dbo.Province",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 80),
                        IsCapital = c.Boolean(nullable: false),
                        Importance = c.Int(nullable: false),
                        CenterPoint = c.Geography(),
                        Boundary = c.Geography(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.CityRegion",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 80),
                        Importance = c.Int(nullable: false),
                        CityID = c.Long(nullable: false),
                        CenterPoint = c.Geography(),
                        Boundary = c.Geography(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Neighborhood",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 80),
                        Importance = c.Int(nullable: false),
                        CityID = c.Long(nullable: false),
                        CenterPoint = c.Geography(),
                        Boundary = c.Geography(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.City",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 80),
                        IsCountryCapital = c.Boolean(nullable: false),
                        IsProvinceCapital = c.Boolean(nullable: false),
                        Importance = c.Int(nullable: false),
                        ProvinceID = c.Long(nullable: false),
                        CenterPoint = c.Geography(),
                        Boundary = c.Geography(),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.SavedPropertySearchGeographicRegion", "NeighborhoodID", c => c.Long());
            AddColumn("dbo.SavedPropertySearch", "CityRegionID", c => c.Long());
            AddColumn("dbo.SavedPropertySearch", "CityID", c => c.Long());
            AddColumn("dbo.SavedPropertySearch", "ProvinceID", c => c.Long());
            AddColumn("dbo.Estate", "NeighborhoodID", c => c.Long());
            AddColumn("dbo.Estate", "CityRegionID", c => c.Long());
            AddColumn("dbo.Estate", "CityID", c => c.Long());
            AddColumn("dbo.Estate", "ProvinceID", c => c.Long());
            AddColumn("dbo.PropertyListing", "NeighborhoodID", c => c.Long());
            AddColumn("dbo.PropertyListing", "CityRegionID", c => c.Long());
            AddColumn("dbo.PropertyListing", "CityID", c => c.Long());
            AddColumn("dbo.PropertyListing", "ProvinceID", c => c.Long());
            CreateIndex("dbo.ProvinceAdjacencies", "AdjacentProvinceID");
            CreateIndex("dbo.ProvinceAdjacencies", "BaseProvinceID");
            CreateIndex("dbo.NeighborhoodSynonyms", "SynonymNeighborhoodID");
            CreateIndex("dbo.NeighborhoodSynonyms", "BaseNeighborhoodID");
            CreateIndex("dbo.CityRegionNeighborhoodRelation", "Neighborhood_ID");
            CreateIndex("dbo.CityRegionNeighborhoodRelation", "CityRegion_ID");
            CreateIndex("dbo.CityRegionAdjacencies", "AdjacentCityRegionID");
            CreateIndex("dbo.CityRegionAdjacencies", "BaseCityRegionID");
            CreateIndex("dbo.NeighborhoodAdjacencies", "AdjacentNeighborhoodID");
            CreateIndex("dbo.NeighborhoodAdjacencies", "BaseNeighborhoodID");
            CreateIndex("dbo.CityAdjacencies", "AdjacentCityID");
            CreateIndex("dbo.CityAdjacencies", "BaseCityID");
            CreateIndex("dbo.SavedPropertySearchGeographicRegion", "NeighborhoodID");
            CreateIndex("dbo.SavedPropertySearch", "CityRegionID");
            CreateIndex("dbo.SavedPropertySearch", "CityID");
            CreateIndex("dbo.SavedPropertySearch", "ProvinceID");
            CreateIndex("dbo.CityRegion", "CityID");
            CreateIndex("dbo.Neighborhood", "CityID");
            CreateIndex("dbo.City", "ProvinceID");
            CreateIndex("dbo.Estate", "NeighborhoodID");
            CreateIndex("dbo.Estate", "CityRegionID");
            CreateIndex("dbo.Estate", "CityID");
            CreateIndex("dbo.Estate", "ProvinceID");
            CreateIndex("dbo.PropertyListing", "NeighborhoodID");
            CreateIndex("dbo.PropertyListing", "CityRegionID");
            CreateIndex("dbo.PropertyListing", "CityID");
            CreateIndex("dbo.PropertyListing", "ProvinceID");
            AddForeignKey("dbo.SavedPropertySearch", "ProvinceID", "dbo.Province", "ID", cascadeDelete: true);
            AddForeignKey("dbo.SavedPropertySearchGeographicRegion", "NeighborhoodID", "dbo.Neighborhood", "ID", cascadeDelete: true);
            AddForeignKey("dbo.SavedPropertySearch", "CityRegionID", "dbo.CityRegion", "ID", cascadeDelete: true);
            AddForeignKey("dbo.SavedPropertySearch", "CityID", "dbo.City", "ID", cascadeDelete: true);
            AddForeignKey("dbo.PropertyListing", "ProvinceID", "dbo.Province", "ID");
            AddForeignKey("dbo.PropertyListing", "NeighborhoodID", "dbo.Neighborhood", "ID");
            AddForeignKey("dbo.PropertyListing", "CityRegionID", "dbo.CityRegion", "ID");
            AddForeignKey("dbo.PropertyListing", "CityID", "dbo.City", "ID");
            AddForeignKey("dbo.Estate", "ProvinceID", "dbo.Province", "ID");
            AddForeignKey("dbo.Estate", "NeighborhoodID", "dbo.Neighborhood", "ID");
            AddForeignKey("dbo.Estate", "CityRegionID", "dbo.CityRegion", "ID");
            AddForeignKey("dbo.Estate", "CityID", "dbo.City", "ID");
            AddForeignKey("dbo.City", "ProvinceID", "dbo.Province", "ID");
            AddForeignKey("dbo.ProvinceAdjacencies", "AdjacentProvinceID", "dbo.Province", "ID");
            AddForeignKey("dbo.ProvinceAdjacencies", "BaseProvinceID", "dbo.Province", "ID");
            AddForeignKey("dbo.NeighborhoodSynonyms", "SynonymNeighborhoodID", "dbo.Neighborhood", "ID");
            AddForeignKey("dbo.NeighborhoodSynonyms", "BaseNeighborhoodID", "dbo.Neighborhood", "ID");
            AddForeignKey("dbo.CityRegionNeighborhoodRelation", "Neighborhood_ID", "dbo.Neighborhood", "ID", cascadeDelete: true);
            AddForeignKey("dbo.CityRegionNeighborhoodRelation", "CityRegion_ID", "dbo.CityRegion", "ID", cascadeDelete: true);
            AddForeignKey("dbo.CityRegion", "CityID", "dbo.City", "ID");
            AddForeignKey("dbo.CityRegionAdjacencies", "AdjacentCityRegionID", "dbo.CityRegion", "ID");
            AddForeignKey("dbo.CityRegionAdjacencies", "BaseCityRegionID", "dbo.CityRegion", "ID");
            AddForeignKey("dbo.Neighborhood", "CityID", "dbo.City", "ID");
            AddForeignKey("dbo.NeighborhoodAdjacencies", "AdjacentNeighborhoodID", "dbo.Neighborhood", "ID");
            AddForeignKey("dbo.NeighborhoodAdjacencies", "BaseNeighborhoodID", "dbo.Neighborhood", "ID");
            AddForeignKey("dbo.CityAdjacencies", "AdjacentCityID", "dbo.City", "ID");
            AddForeignKey("dbo.CityAdjacencies", "BaseCityID", "dbo.City", "ID");
        }
    }
}
