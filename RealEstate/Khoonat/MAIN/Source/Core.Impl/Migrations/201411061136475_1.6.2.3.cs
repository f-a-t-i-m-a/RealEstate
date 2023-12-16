namespace JahanJooy.RealEstate.Core.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1623 : DbMigration
    {
        public override void Up()
        {
			Sql("UPDATE [dbo].[HttpSession] SET [Type] = 1");

            DropIndex("dbo.AbuseFlag", new[] { "ReportedInSessionID" });
            DropIndex("dbo.PropertyListing", new[] { "CreatorSessionID" });
            DropIndex("dbo.PropertyListingPhoto", new[] { "CreatorSessionID" });
            DropIndex("dbo.PropertyListingPublishHistory", new[] { "SessionID" });
            DropIndex("dbo.PropertyListingUpdateHistory", new[] { "SessionID" });
            DropIndex("dbo.PropertyRequest", new[] { "CreatorSessionID" });
            DropIndex("dbo.PropertySearchHistory", new[] { "SessionID" });
            DropIndex("dbo.SavedPropertySearch", new[] { "CreatorSessionID" });
            DropIndex("dbo.SavedPropertySearchUpdateHistory", new[] { "SessionID" });
            AlterColumn("dbo.AbuseFlag", "ReportedInSessionID", c => c.Long());
            AlterColumn("dbo.HttpSession", "Type", c => c.Byte(nullable: false));
            AlterColumn("dbo.PropertyListing", "CreatorSessionID", c => c.Long());
            AlterColumn("dbo.PropertyListingPhoto", "CreatorSessionID", c => c.Long());
            AlterColumn("dbo.PropertyListingPublishHistory", "SessionID", c => c.Long());
            AlterColumn("dbo.PropertyListingUpdateHistory", "SessionID", c => c.Long());
            AlterColumn("dbo.PropertyRequest", "CreatorSessionID", c => c.Long());
            AlterColumn("dbo.PropertySearchHistory", "SessionID", c => c.Long());
            AlterColumn("dbo.SavedPropertySearch", "CreatorSessionID", c => c.Long());
            AlterColumn("dbo.SavedPropertySearchUpdateHistory", "SessionID", c => c.Long());
            CreateIndex("dbo.AbuseFlag", "ReportedInSessionID");
            CreateIndex("dbo.PropertyListing", "CreatorSessionID");
            CreateIndex("dbo.PropertyListingPhoto", "CreatorSessionID");
            CreateIndex("dbo.PropertyListingPublishHistory", "SessionID");
            CreateIndex("dbo.PropertyListingUpdateHistory", "SessionID");
            CreateIndex("dbo.PropertyRequest", "CreatorSessionID");
            CreateIndex("dbo.PropertySearchHistory", "SessionID");
            CreateIndex("dbo.SavedPropertySearch", "CreatorSessionID");
            CreateIndex("dbo.SavedPropertySearchUpdateHistory", "SessionID");

			RenameColumn("dbo.Estate", "Address1", "Address");
//			AddColumn("dbo.Estate", "Address", c => c.String(maxLength: 150));
//			DropColumn("dbo.Estate", "Address1");

			RenameColumn("dbo.PropertyListingPhoto", "DeleteDate", "DeleteTime");
//			AddColumn("dbo.PropertyListingPhoto", "DeleteTime", c => c.DateTime());
//			DropColumn("dbo.PropertyListingPhoto", "DeleteDate");

			RenameColumn("dbo.PropertyListingPhoto", "CreationDate", "CreationTime");
//			AddColumn("dbo.PropertyListingPhoto", "CreationTime", c => c.DateTime(nullable: false));
//			DropColumn("dbo.PropertyListingPhoto", "CreationDate");

			RenameColumn("dbo.PropertyListingUpdateHistory", "UpdateDate", "UpdateTime");
//			AddColumn("dbo.PropertyListingUpdateHistory", "UpdateTime", c => c.DateTime(nullable: false));
//			DropColumn("dbo.PropertyListingUpdateHistory", "UpdateDate");

			RenameColumn("dbo.SavedPropertySearch", "CreationDate", "CreationTime");
//			AddColumn("dbo.SavedPropertySearch", "CreationTime", c => c.DateTime(nullable: false));
//			DropColumn("dbo.SavedPropertySearch", "CreationDate");

			RenameColumn("dbo.SavedPropertySearch", "DeleteDate", "DeleteTime");
//			AddColumn("dbo.SavedPropertySearch", "DeleteTime", c => c.DateTime());
//			DropColumn("dbo.SavedPropertySearch", "DeleteDate");

			RenameColumn("dbo.SavedPropertySearchUpdateHistory", "UpdateDate", "UpdateTime");
//			AddColumn("dbo.SavedPropertySearchUpdateHistory", "UpdateTime", c => c.DateTime(nullable: false));
//			DropColumn("dbo.SavedPropertySearchUpdateHistory", "UpdateDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SavedPropertySearchUpdateHistory", "UpdateDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.SavedPropertySearch", "DeleteDate", c => c.DateTime());
            AddColumn("dbo.SavedPropertySearch", "CreationDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.PropertyListingUpdateHistory", "UpdateDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.PropertyListingPhoto", "CreationDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.PropertyListingPhoto", "DeleteDate", c => c.DateTime());
            AddColumn("dbo.Estate", "Address1", c => c.String(maxLength: 150));
            DropIndex("dbo.SavedPropertySearchUpdateHistory", new[] { "SessionID" });
            DropIndex("dbo.SavedPropertySearch", new[] { "CreatorSessionID" });
            DropIndex("dbo.PropertySearchHistory", new[] { "SessionID" });
            DropIndex("dbo.PropertyRequest", new[] { "CreatorSessionID" });
            DropIndex("dbo.PropertyListingUpdateHistory", new[] { "SessionID" });
            DropIndex("dbo.PropertyListingPublishHistory", new[] { "SessionID" });
            DropIndex("dbo.PropertyListingPhoto", new[] { "CreatorSessionID" });
            DropIndex("dbo.PropertyListing", new[] { "CreatorSessionID" });
            DropIndex("dbo.AbuseFlag", new[] { "ReportedInSessionID" });
            AlterColumn("dbo.SavedPropertySearchUpdateHistory", "SessionID", c => c.Long(nullable: false));
            AlterColumn("dbo.SavedPropertySearch", "CreatorSessionID", c => c.Long(nullable: false));
            AlterColumn("dbo.PropertySearchHistory", "SessionID", c => c.Long(nullable: false));
            AlterColumn("dbo.PropertyRequest", "CreatorSessionID", c => c.Long(nullable: false));
            AlterColumn("dbo.PropertyListingUpdateHistory", "SessionID", c => c.Long(nullable: false));
            AlterColumn("dbo.PropertyListingPublishHistory", "SessionID", c => c.Long(nullable: false));
            AlterColumn("dbo.PropertyListingPhoto", "CreatorSessionID", c => c.Long(nullable: false));
            AlterColumn("dbo.PropertyListing", "CreatorSessionID", c => c.Long(nullable: false));
            AlterColumn("dbo.HttpSession", "Type", c => c.Byte());
            AlterColumn("dbo.AbuseFlag", "ReportedInSessionID", c => c.Long(nullable: false));
            DropColumn("dbo.SavedPropertySearchUpdateHistory", "UpdateTime");
            DropColumn("dbo.SavedPropertySearch", "DeleteTime");
            DropColumn("dbo.SavedPropertySearch", "CreationTime");
            DropColumn("dbo.PropertyListingUpdateHistory", "UpdateTime");
            DropColumn("dbo.PropertyListingPhoto", "CreationTime");
            DropColumn("dbo.PropertyListingPhoto", "DeleteTime");
            DropColumn("dbo.Estate", "Address");
            CreateIndex("dbo.SavedPropertySearchUpdateHistory", "SessionID");
            CreateIndex("dbo.SavedPropertySearch", "CreatorSessionID");
            CreateIndex("dbo.PropertySearchHistory", "SessionID");
            CreateIndex("dbo.PropertyRequest", "CreatorSessionID");
            CreateIndex("dbo.PropertyListingUpdateHistory", "SessionID");
            CreateIndex("dbo.PropertyListingPublishHistory", "SessionID");
            CreateIndex("dbo.PropertyListingPhoto", "CreatorSessionID");
            CreateIndex("dbo.PropertyListing", "CreatorSessionID");
            CreateIndex("dbo.AbuseFlag", "ReportedInSessionID");
        }
    }
}
