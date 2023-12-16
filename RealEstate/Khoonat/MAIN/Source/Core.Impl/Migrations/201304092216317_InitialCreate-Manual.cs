namespace JahanJooy.RealEstate.Core.Impl.Migrations
{
	using System;
	using System.Data.Entity.Migrations;

	public partial class InitialCreateManual : DbMigration
	{
		public override void Up()
		{
			// Create sequences

			Sql("CREATE SEQUENCE [dbo].[PropertyListingCodeSeq] START WITH 100 INCREMENT BY 1 CACHE 10;");
			Sql("CREATE SEQUENCE [dbo].[UserCodeSeq] START WITH 100 INCREMENT BY 1 CACHE 10;");

			// Set column defaults to use sequences

			Sql("ALTER TABLE [PropertyListing] ADD CONSTRAINT DF_PropertyListing_Code DEFAULT (NEXT VALUE FOR [dbo].[PropertyListingCodeSeq]) FOR [Code];");
			Sql("ALTER TABLE [User] ADD CONSTRAINT DF_User_Code DEFAULT (NEXT VALUE FOR [dbo].[UserCodeSeq]) FOR [Code]");

			// Create indexes

			CreateIndex("AbuseFlag", "ReportDate");
			CreateIndex("AbuseFlag", new[] { "EntityType", "EntityID" });

			CreateIndex("ActivityLog", "LogDate");
			CreateIndex("ActivityLog", "TargetEntity");
			CreateIndex("ActivityLog", new[] { "ReviewDate", "ReviewWeight" });

			CreateIndex("PropertyListing", new[] { "DeleteDate", "PublishEndDate", "PublishDate", "Approved" });
			CreateIndex("PropertyListing", "Code", unique: true);

			CreateIndex("UniqueVisitor", "UniqueIdentifier", unique: true);

			CreateIndex("User", "Code", unique: true);
			CreateIndex("User", "LoginName", unique: true);
			CreateIndex("User", "LastLogin");
		}

		public override void Down()
		{
			DropIndex("User", "IX_LastLogin");
			DropIndex("User", "IX_LoginName");
			DropIndex("User", "IX_Code");
			DropIndex("UniqueVisitor", "IX_UniqueIdentifier");
			DropIndex("PropertyListing", "IX_Code");
			DropIndex("PropertyListing", "IX_DeleteDate_PublishEndDate_PublishDate_Approved");
			DropIndex("ActivityLog", "IX_ReviewDate_ReviewWeight");
			DropIndex("ActivityLog", "IX_TargetEntity");
			DropIndex("ActivityLog", "IX_LogDate");
			DropIndex("AbuseFlag", "IX_EntityType_EntityID");
			DropIndex("AbuseFlag", "IX_ReportDate");

			Sql(@"DECLARE @var0 nvarchar(128)
				  SELECT @var0 = name
				  FROM sys.default_constraints
				  WHERE parent_object_id = object_id(N'dbo.User')
				  AND col_name(parent_object_id, parent_column_id) = 'Code';
				  IF @var0 IS NOT NULL
				  EXECUTE('ALTER TABLE [dbo].[User] DROP CONSTRAINT ' + @var0);");

			Sql(@"DECLARE @var1 nvarchar(128)
				  SELECT @var1 = name
				  FROM sys.default_constraints
				  WHERE parent_object_id = object_id(N'dbo.PropertyListing')
				  AND col_name(parent_object_id, parent_column_id) = 'Code';
				  IF @var1 IS NOT NULL
				  EXECUTE('ALTER TABLE [dbo].[PropertyListing] DROP CONSTRAINT ' + @var1);");

			Sql("DROP SEQUENCE [dbo].[UserCodeSeq];");
			Sql("DROP SEQUENCE [dbo].[PropertyListingCodeSeq];");
		}
	}
}