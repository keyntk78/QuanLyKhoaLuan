namespace QuanLyKhoaLuan.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        role_id = c.Guid(nullable: false),
                        code = c.String(nullable: false, maxLength: 20),
                        role_name = c.String(nullable: false, maxLength: 50),
                        description = c.String(maxLength: 255),
                        created_at = c.DateTime(),
                        updated_at = c.DateTime(),
                    })
                .PrimaryKey(t => t.role_id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        user_id = c.Guid(nullable: false),
                        username = c.String(nullable: false, maxLength: 100),
                        password = c.String(nullable: false, maxLength: 100),
                        full_name = c.String(nullable: false, maxLength: 255),
                        email = c.String(nullable: false, maxLength: 255),
                        phone = c.String(maxLength: 15),
                        avatar = c.String(),
                        active = c.Boolean(nullable: false),
                        role_id = c.Guid(nullable: false),
                        created_at = c.DateTime(),
                        updated_at = c.DateTime(),
                    })
                .PrimaryKey(t => t.user_id)
                .ForeignKey("dbo.Roles", t => t.role_id, cascadeDelete: true)
                .Index(t => t.role_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "role_id", "dbo.Roles");
            DropIndex("dbo.Users", new[] { "role_id" });
            DropTable("dbo.Users");
            DropTable("dbo.Roles");
        }
    }
}
