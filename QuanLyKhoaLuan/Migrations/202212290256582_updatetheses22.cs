namespace QuanLyKhoaLuan.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatetheses22 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Theses", "class_id", "dbo.Classes");
            DropIndex("dbo.Theses", new[] { "class_id" });
            DropPrimaryKey("dbo.Theses");
            AddColumn("dbo.Theses", "thesis_id", c => c.Guid(nullable: false));
            AddColumn("dbo.Theses", "major_id", c => c.Guid(nullable: false));
            AddPrimaryKey("dbo.Theses", "thesis_id");
            CreateIndex("dbo.Theses", "major_id");
            AddForeignKey("dbo.Theses", "major_id", "dbo.Majors", "major_id", cascadeDelete: true);
            DropColumn("dbo.Theses", "id");
            DropColumn("dbo.Theses", "class_id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Theses", "class_id", c => c.Guid(nullable: false));
            AddColumn("dbo.Theses", "id", c => c.Guid(nullable: false));
            DropForeignKey("dbo.Theses", "major_id", "dbo.Majors");
            DropIndex("dbo.Theses", new[] { "major_id" });
            DropPrimaryKey("dbo.Theses");
            DropColumn("dbo.Theses", "major_id");
            DropColumn("dbo.Theses", "thesis_id");
            AddPrimaryKey("dbo.Theses", "id");
            CreateIndex("dbo.Theses", "class_id");
            AddForeignKey("dbo.Theses", "class_id", "dbo.Classes", "class_id", cascadeDelete: true);
        }
    }
}
