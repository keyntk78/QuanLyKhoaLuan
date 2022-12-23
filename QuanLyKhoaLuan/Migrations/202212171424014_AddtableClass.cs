namespace QuanLyKhoaLuan.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddtableClass : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Classes",
                c => new
                    {
                        class_id = c.Guid(nullable: false),
                        code = c.String(nullable: false, maxLength: 20),
                        name = c.String(nullable: false, maxLength: 255),
                        description = c.String(),
                        department_id = c.Guid(nullable: false),
                        created_at = c.DateTime(),
                        updated_at = c.DateTime(),
                    })
                .PrimaryKey(t => t.class_id)
                .ForeignKey("dbo.Departments", t => t.department_id, cascadeDelete: false)
                .Index(t => t.department_id);
            
            AddColumn("dbo.Students", "class_id", c => c.Guid(nullable: false));
            CreateIndex("dbo.Students", "class_id");
            AddForeignKey("dbo.Students", "class_id", "dbo.Classes", "class_id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Students", "class_id", "dbo.Classes");
            DropForeignKey("dbo.Classes", "department_id", "dbo.Departments");
            DropIndex("dbo.Students", new[] { "class_id" });
            DropIndex("dbo.Classes", new[] { "department_id" });
            DropColumn("dbo.Students", "class_id");
            DropTable("dbo.Classes");
        }
    }
}
