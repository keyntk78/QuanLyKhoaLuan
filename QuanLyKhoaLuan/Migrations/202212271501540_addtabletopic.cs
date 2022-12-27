namespace QuanLyKhoaLuan.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addtabletopic : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Topics",
                c => new
                    {
                        topic_id = c.Guid(nullable: false),
                        name = c.String(nullable: false),
                        description = c.String(),
                        department_id = c.Guid(nullable: false),
                        created_at = c.DateTime(),
                        updated_at = c.DateTime(),
                    })
                .PrimaryKey(t => t.topic_id)
                .ForeignKey("dbo.Departments", t => t.department_id, cascadeDelete: true)
                .Index(t => t.department_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Topics", "department_id", "dbo.Departments");
            DropIndex("dbo.Topics", new[] { "department_id" });
            DropTable("dbo.Topics");
        }
    }
}
