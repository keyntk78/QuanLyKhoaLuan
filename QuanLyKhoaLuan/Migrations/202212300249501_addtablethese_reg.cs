namespace QuanLyKhoaLuan.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addtablethese_reg : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Thesis_registration",
                c => new
                    {
                        thesis_registration_id = c.Guid(nullable: false),
                        student_id = c.Guid(nullable: false),
                        thesis_id = c.Guid(nullable: false),
                        created_at = c.DateTime(),
                        updated_at = c.DateTime(),
                    })
                .PrimaryKey(t => t.thesis_registration_id)
                .ForeignKey("dbo.Students", t => t.student_id, cascadeDelete: false)
                .ForeignKey("dbo.Theses", t => t.thesis_id, cascadeDelete: true)
                .Index(t => t.student_id)
                .Index(t => t.thesis_id);
            
            AlterColumn("dbo.Students", "gpa", c => c.Double());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Thesis_registration", "thesis_id", "dbo.Theses");
            DropForeignKey("dbo.Thesis_registration", "student_id", "dbo.Students");
            DropIndex("dbo.Thesis_registration", new[] { "thesis_id" });
            DropIndex("dbo.Thesis_registration", new[] { "student_id" });
            AlterColumn("dbo.Students", "gpa", c => c.Double(nullable: false));
            DropTable("dbo.Thesis_registration");
        }
    }
}
