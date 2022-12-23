namespace QuanLyKhoaLuan.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class chinhsuabangclass : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Classes", "department_id", "dbo.Departments");
            DropForeignKey("dbo.Students", "major_id", "dbo.Majors");
            DropIndex("dbo.Classes", new[] { "department_id" });
            DropIndex("dbo.Students", new[] { "major_id" });
            AddColumn("dbo.Classes", "major_id", c => c.Guid(nullable: false));
            CreateIndex("dbo.Classes", "major_id");
            AddForeignKey("dbo.Classes", "major_id", "dbo.Majors", "major_id", cascadeDelete: true);
            DropColumn("dbo.Classes", "department_id");
            DropColumn("dbo.Students", "major_id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Students", "major_id", c => c.Guid(nullable: false));
            AddColumn("dbo.Classes", "department_id", c => c.Guid(nullable: false));
            DropForeignKey("dbo.Classes", "major_id", "dbo.Majors");
            DropIndex("dbo.Classes", new[] { "major_id" });
            DropColumn("dbo.Classes", "major_id");
            CreateIndex("dbo.Students", "major_id");
            CreateIndex("dbo.Classes", "department_id");
            AddForeignKey("dbo.Students", "major_id", "dbo.Majors", "major_id", cascadeDelete: true);
            AddForeignKey("dbo.Classes", "department_id", "dbo.Departments", "department_id", cascadeDelete: true);
        }
    }
}
