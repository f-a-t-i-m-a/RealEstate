namespace JahanJooy.RealEstate.Core.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1124 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserFavoritePropertyListing",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        CreationDate = c.DateTime(nullable: false),
                        CreationSessionID = c.Long(nullable: false),
                        UserID = c.Long(nullable: false),
                        ListingID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.HttpSession", t => t.CreationSessionID)
                .ForeignKey("dbo.User", t => t.UserID, cascadeDelete: true)
                .ForeignKey("dbo.PropertyListing", t => t.ListingID, cascadeDelete: true)
                .Index(t => t.CreationSessionID)
                .Index(t => t.UserID)
                .Index(t => t.ListingID);
            
            AddColumn("dbo.PropertyListing", "GeographicLocation", c => c.Geography());
            AddColumn("dbo.PropertyListing", "GeographicLocationType", c => c.Byte());
            AddColumn("dbo.PropertyListing", "NumberOfContactInfoRetrievals", c => c.Long(nullable: true));
            AddColumn("dbo.PropertyListing", "NormalizedPricePerEstateArea", c => c.Decimal(precision: 17, scale: 2));
            AddColumn("dbo.PropertyListing", "NormalizedPricePerUnitArea", c => c.Decimal(precision: 17, scale: 2));
            AddColumn("dbo.Estate", "GeographicLocation", c => c.Geography());
            AddColumn("dbo.Estate", "GeographicLocationType", c => c.Byte());
            AlterColumn("dbo.PropertyListing", "NormalizedPrice", c => c.Decimal(precision: 17, scale: 2));

			Sql("UPDATE [dbo].[PropertyListing] SET NumberOfContactInfoRetrievals = NumberOfVisits;");

			AlterColumn("dbo.PropertyListing", "NumberOfContactInfoRetrievals", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropIndex("dbo.UserFavoritePropertyListing", new[] { "ListingID" });
            DropIndex("dbo.UserFavoritePropertyListing", new[] { "UserID" });
            DropIndex("dbo.UserFavoritePropertyListing", new[] { "CreationSessionID" });
            DropForeignKey("dbo.UserFavoritePropertyListing", "ListingID", "dbo.PropertyListing");
            DropForeignKey("dbo.UserFavoritePropertyListing", "UserID", "dbo.User");
            DropForeignKey("dbo.UserFavoritePropertyListing", "CreationSessionID", "dbo.HttpSession");
            AlterColumn("dbo.PropertyListing", "NormalizedPrice", c => c.Long());
            DropColumn("dbo.Estate", "GeographicLocationType");
            DropColumn("dbo.Estate", "GeographicLocation");
            DropColumn("dbo.PropertyListing", "NormalizedPricePerUnitArea");
            DropColumn("dbo.PropertyListing", "NormalizedPricePerEstateArea");
            DropColumn("dbo.PropertyListing", "NumberOfContactInfoRetrievals");
            DropColumn("dbo.PropertyListing", "GeographicLocationType");
            DropColumn("dbo.PropertyListing", "GeographicLocation");
            DropTable("dbo.UserFavoritePropertyListing");
        }
    }
}
