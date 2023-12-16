namespace JahanJooy.RealEstate.Core.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1632 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PropertyListing", "IndexedTime", c => c.DateTime());
            AddColumn("dbo.PropertyListing", "ContentString", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PropertyListing", "ContentString");
            DropColumn("dbo.PropertyListing", "IndexedTime");
        }
    }
}
