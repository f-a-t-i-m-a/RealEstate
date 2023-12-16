namespace JahanJooy.RealEstate.Core.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1083 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.HttpSession", "UserAgent", c => c.String(maxLength: 400));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.HttpSession", "UserAgent", c => c.String(maxLength: 200));
        }
    }
}
