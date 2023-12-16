using JahanJooy.RealEstate.Domain.Audit;

namespace JahanJooy.RealEstate.Core.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1871HttpSessionIndex : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo." + nameof(HttpSession), nameof(HttpSession.HttpSessionID));
        }
        
        public override void Down()
        {
            DropIndex("dbo." + nameof(HttpSession), new[] {nameof(HttpSession.HttpSessionID)});
        }
    }
}
