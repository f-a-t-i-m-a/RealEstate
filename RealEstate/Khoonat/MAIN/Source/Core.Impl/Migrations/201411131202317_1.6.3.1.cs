namespace JahanJooy.RealEstate.Core.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1631 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.User", "Type", c => c.Byte(nullable: true));
            AddColumn("dbo.User", "AgencyID", c => c.Long());
            CreateIndex("dbo.User", "AgencyID");
            AddForeignKey("dbo.User", "AgencyID", "dbo.Agency", "ID");

            Sql("UPDATE [dbo].[User] SET [Type] = 1");

            AlterColumn("dbo.User", "Type", c => c.Byte(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.User", "AgencyID", "dbo.Agency");
            DropIndex("dbo.User", new[] { "AgencyID" });
            DropColumn("dbo.User", "AgencyID");
            DropColumn("dbo.User", "Type");
        }
    }
}
