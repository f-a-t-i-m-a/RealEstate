namespace JahanJooy.RealEstate.Core.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1112 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PropertyListingPhoto",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        StoreItemID = c.Guid(nullable: false),
                        DeleteDate = c.DateTime(),
                        Approved = c.Boolean(),
                        PropertyListingID = c.Long(nullable: false),
                        UntouchedLength = c.Long(nullable: false),
                        ThumbnailLength = c.Long(nullable: false),
                        MediumSizeLength = c.Long(nullable: false),
                        FullSizeLength = c.Long(nullable: false),
                        Subject = c.Byte(),
                        Title = c.String(maxLength: 100),
                        Description = c.String(maxLength: 2000),
                        Order = c.Long(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        CreatorSessionID = c.Long(nullable: false),
                        CreatorUserID = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.PropertyListing", t => t.PropertyListingID)
                .ForeignKey("dbo.HttpSession", t => t.CreatorSessionID)
                .ForeignKey("dbo.User", t => t.CreatorUserID)
                .Index(t => t.PropertyListingID)
                .Index(t => t.CreatorSessionID)
                .Index(t => t.CreatorUserID);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.PropertyListingPhoto", new[] { "CreatorUserID" });
            DropIndex("dbo.PropertyListingPhoto", new[] { "CreatorSessionID" });
            DropIndex("dbo.PropertyListingPhoto", new[] { "PropertyListingID" });
            DropForeignKey("dbo.PropertyListingPhoto", "CreatorUserID", "dbo.User");
            DropForeignKey("dbo.PropertyListingPhoto", "CreatorSessionID", "dbo.HttpSession");
            DropForeignKey("dbo.PropertyListingPhoto", "PropertyListingID", "dbo.PropertyListing");
            DropTable("dbo.PropertyListingPhoto");
        }
    }
}
