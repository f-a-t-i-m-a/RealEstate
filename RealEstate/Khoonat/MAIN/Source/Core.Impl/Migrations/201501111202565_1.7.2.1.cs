namespace JahanJooy.RealEstate.Core.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1721 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.User", "Approved", c => c.Boolean());
            AddColumn("dbo.User", "DeleteTime", c => c.DateTime());
            AddColumn("dbo.User", "ProfilePictureStoreItemID", c => c.Guid());

            Sql("UPDATE [dbo].[User] SET [Approved] = 1");
        }
        
        public override void Down()
        {
            DropColumn("dbo.User", "ProfilePictureStoreItemID");
            DropColumn("dbo.User", "DeleteTime");
            DropColumn("dbo.User", "Approved");
        }
    }
}
