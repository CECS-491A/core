namespace DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addClientClientServicemodifiedClaim : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        Disabled = c.Boolean(nullable: false),
                        Address = c.String(),
                        UpdatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ClientUsers",
                c => new
                    {
                        ClientId = c.Guid(nullable: false),
                        UserId = c.Guid(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        CreatedAt = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => new { t.ClientId, t.UserId })
                .ForeignKey("dbo.Clients", t => t.ClientId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.ClientId)
                .Index(t => t.UserId);
            
            AddColumn("dbo.Claims", "SubjectUserId", c => c.Guid(nullable: true));
            AddColumn("dbo.Claims", "ClientId", c => c.Guid(nullable: true));
            CreateIndex("dbo.Claims", "SubjectUserId");
            CreateIndex("dbo.Claims", "ClientId");
            AddForeignKey("dbo.Claims", "ClientId", "dbo.Clients", "Id", cascadeDelete: false);
            AddForeignKey("dbo.Claims", "SubjectUserId", "dbo.Users", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ClientUsers", "UserId", "dbo.Users");
            DropForeignKey("dbo.ClientUsers", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.Claims", "SubjectUserId", "dbo.Users");
            DropForeignKey("dbo.Claims", "ClientId", "dbo.Clients");
            DropIndex("dbo.ClientUsers", new[] { "UserId" });
            DropIndex("dbo.ClientUsers", new[] { "ClientId" });
            DropIndex("dbo.Claims", new[] { "ClientId" });
            DropIndex("dbo.Claims", new[] { "SubjectUserId" });
            DropColumn("dbo.Claims", "ClientId");
            DropColumn("dbo.Claims", "SubjectUserId");
            DropTable("dbo.ClientUsers");
            DropTable("dbo.Clients");
        }
    }
}