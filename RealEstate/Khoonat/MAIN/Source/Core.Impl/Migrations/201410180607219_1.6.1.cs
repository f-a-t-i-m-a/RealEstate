namespace JahanJooy.RealEstate.Core.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _161 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Agency", "Approved", c => c.Boolean());

            Sql("UPDATE [dbo].[Agency] SET [Approved] = 1");

            CreateIndex("dbo.Agency", new[] {"IndexedTime", "ID"});
            CreateIndex("dbo.AgencyBranch", new[] { "IndexedTime", "ID" });
            CreateIndex("dbo.PropertyRequest", new[] { "IndexedTime", "ID" });
        }
        
        public override void Down()
        {
            DropColumn("dbo.Agency", "Approved");
        }
    }
}
