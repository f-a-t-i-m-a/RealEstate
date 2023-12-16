namespace JahanJooy.RealEstate.Core.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _153 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NotificationMessage",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        CreationTime = c.DateTime(nullable: false),
                        TargetUserID = c.Long(nullable: false),
                        Text = c.String(maxLength: 1000),
                        Reason = c.Byte(nullable: false),
                        Severity = c.Byte(nullable: false),
                        SourceEntityType = c.Byte(),
                        SourceEntityID = c.Long(),
                        SeenTime = c.DateTime(),
                        AddressedTime = c.DateTime(),
                        NextMessageTransmissionDue = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.User", t => t.TargetUserID)
                .Index(t => t.TargetUserID);
            
            AddColumn("dbo.Vicinity", "OfficialLinkUrl", c => c.String());
            AddColumn("dbo.Vicinity", "WikiLinkUrl", c => c.String());
            AddColumn("dbo.Vicinity", "ShowInHierarchy", c => c.Boolean(nullable: false));
            AddColumn("dbo.OutgoingSmsMessage", "NotificationID", c => c.Long());
            CreateIndex("dbo.OutgoingSmsMessage", "NotificationID");
            AddForeignKey("dbo.OutgoingSmsMessage", "NotificationID", "dbo.NotificationMessage", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NotificationMessage", "TargetUserID", "dbo.User");
            DropForeignKey("dbo.OutgoingSmsMessage", "NotificationID", "dbo.NotificationMessage");
            DropIndex("dbo.NotificationMessage", new[] { "TargetUserID" });
            DropIndex("dbo.OutgoingSmsMessage", new[] { "NotificationID" });
            DropColumn("dbo.OutgoingSmsMessage", "NotificationID");
            DropColumn("dbo.Vicinity", "ShowInHierarchy");
            DropColumn("dbo.Vicinity", "WikiLinkUrl");
            DropColumn("dbo.Vicinity", "OfficialLinkUrl");
            DropTable("dbo.NotificationMessage");
        }
    }
}
