namespace JahanJooy.RealEstate.Core.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _151 : DbMigration
    {
        public override void Up()
        {
			Sql("DELETE FROM dbo.AgencyBranch;");
			Sql("DELETE FROM dbo.Agency;");

            DropForeignKey("dbo.AgencyBranch", "CityID", "dbo.City");
            DropForeignKey("dbo.AgencyBranch", "CityRegionID", "dbo.CityRegion");
            DropForeignKey("dbo.AgencyBranch", "NeighborhoodID", "dbo.Neighborhood");
            DropForeignKey("dbo.AgencyBranch", "ProvinceID", "dbo.Province");
            DropIndex("dbo.AgencyBranch", new[] { "CityID" });
            DropIndex("dbo.AgencyBranch", new[] { "CityRegionID" });
            DropIndex("dbo.AgencyBranch", new[] { "NeighborhoodID" });
            DropIndex("dbo.AgencyBranch", new[] { "ProvinceID" });
            CreateTable(
                "dbo.ApiUser",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Key = c.String(maxLength: 50),
                        CreationTime = c.DateTime(nullable: false),
                        LastModificationTime = c.DateTime(nullable: false),
                        ExpirationTime = c.DateTime(),
                        RequestedByUserID = c.Long(nullable: false),
                        ReviewedByUserID = c.Long(),
                        DailyCallQuota = c.Long(nullable: false),
                        DailyCallQuotaPerUser = c.Long(nullable: false),
                        RequireSessions = c.Boolean(nullable: false),
                        RequireSignature = c.Boolean(nullable: false),
                        RequireAuthentication = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.PropertyRequest",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Code = c.Long(nullable: false),
                        DeleteDate = c.DateTime(),
                        Approved = c.Boolean(),
                        CreationTime = c.DateTime(nullable: false),
                        LastModificationTime = c.DateTime(nullable: false),
                        IndexedTime = c.DateTime(),
                        EditPassword = c.String(maxLength: 20),
                        CreatorSessionID = c.Long(nullable: false),
                        CreatorUserID = c.Long(),
                        OwnerUserID = c.Long(),
                        NumberOfVisits = c.Long(nullable: false),
                        NumberOfContactInfoRetrievals = c.Long(nullable: false),
                        NumberOfSearches = c.Long(nullable: false),
                        ContentsString = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.HttpSession", t => t.CreatorSessionID)
                .ForeignKey("dbo.User", t => t.CreatorUserID)
                .ForeignKey("dbo.User", t => t.OwnerUserID)
                .Index(t => t.CreatorSessionID)
                .Index(t => t.CreatorUserID)
                .Index(t => t.OwnerUserID);
            
            AddColumn("dbo.HttpSession", "Type", c => c.Byte());
            AddColumn("dbo.AgencyBranch", "VicinityID", c => c.Long(nullable: false));
            AddColumn("dbo.SavedPropertySearchGeographicRegion", "VicinityID", c => c.Long());
            CreateIndex("dbo.AgencyBranch", "VicinityID");
            CreateIndex("dbo.SavedPropertySearchGeographicRegion", "VicinityID");
            AddForeignKey("dbo.AgencyBranch", "VicinityID", "dbo.Vicinity", "ID");
            AddForeignKey("dbo.SavedPropertySearchGeographicRegion", "VicinityID", "dbo.Vicinity", "ID", cascadeDelete: true);
            DropColumn("dbo.PropertyListingContactInfo", "ContactRole");
            DropColumn("dbo.AgencyBranch", "ProvinceID");
            DropColumn("dbo.AgencyBranch", "CityID");
            DropColumn("dbo.AgencyBranch", "CityRegionID");
            DropColumn("dbo.AgencyBranch", "NeighborhoodID");

			Sql("UPDATE dbo.[HttpSession] SET [Type] = 1");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AgencyBranch", "NeighborhoodID", c => c.Long(nullable: false));
            AddColumn("dbo.AgencyBranch", "CityRegionID", c => c.Long(nullable: false));
            AddColumn("dbo.AgencyBranch", "CityID", c => c.Long(nullable: false));
            AddColumn("dbo.AgencyBranch", "ProvinceID", c => c.Long(nullable: false));
            AddColumn("dbo.PropertyListingContactInfo", "ContactRole", c => c.Byte());
            DropForeignKey("dbo.SavedPropertySearchGeographicRegion", "VicinityID", "dbo.Vicinity");
            DropForeignKey("dbo.PropertyRequest", "OwnerUserID", "dbo.User");
            DropForeignKey("dbo.PropertyRequest", "CreatorUserID", "dbo.User");
            DropForeignKey("dbo.PropertyRequest", "CreatorSessionID", "dbo.HttpSession");
            DropForeignKey("dbo.AgencyBranch", "VicinityID", "dbo.Vicinity");
            DropIndex("dbo.SavedPropertySearchGeographicRegion", new[] { "VicinityID" });
            DropIndex("dbo.PropertyRequest", new[] { "OwnerUserID" });
            DropIndex("dbo.PropertyRequest", new[] { "CreatorUserID" });
            DropIndex("dbo.PropertyRequest", new[] { "CreatorSessionID" });
            DropIndex("dbo.AgencyBranch", new[] { "VicinityID" });
            DropColumn("dbo.SavedPropertySearchGeographicRegion", "VicinityID");
            DropColumn("dbo.AgencyBranch", "VicinityID");
            DropColumn("dbo.HttpSession", "Type");
            DropTable("dbo.PropertyRequest");
            DropTable("dbo.ApiUser");
            CreateIndex("dbo.AgencyBranch", "ProvinceID");
            CreateIndex("dbo.AgencyBranch", "NeighborhoodID");
            CreateIndex("dbo.AgencyBranch", "CityRegionID");
            CreateIndex("dbo.AgencyBranch", "CityID");
            AddForeignKey("dbo.AgencyBranch", "ProvinceID", "dbo.Province", "ID");
            AddForeignKey("dbo.AgencyBranch", "NeighborhoodID", "dbo.Neighborhood", "ID");
            AddForeignKey("dbo.AgencyBranch", "CityRegionID", "dbo.CityRegion", "ID");
            AddForeignKey("dbo.AgencyBranch", "CityID", "dbo.City", "ID");
        }
    }
}
