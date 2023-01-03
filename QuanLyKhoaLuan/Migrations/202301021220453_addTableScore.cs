namespace QuanLyKhoaLuan.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTableScore : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Scores",
                c => new
                    {
                        score_id = c.Guid(nullable: false),
                        lecturer_id = c.Guid(nullable: false),
                        thesis_id = c.Guid(nullable: false),
                        created_at = c.DateTime(),
                        updated_at = c.DateTime(),
                    })
                .PrimaryKey(t => t.score_id)
                .ForeignKey("dbo.Lecturers", t => t.lecturer_id, cascadeDelete: false)
                .ForeignKey("dbo.Theses", t => t.thesis_id, cascadeDelete: false)
                .Index(t => t.lecturer_id)
                .Index(t => t.thesis_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Scores", "thesis_id", "dbo.Theses");
            DropForeignKey("dbo.Scores", "lecturer_id", "dbo.Lecturers");
            DropIndex("dbo.Scores", new[] { "thesis_id" });
            DropIndex("dbo.Scores", new[] { "lecturer_id" });
            DropTable("dbo.Scores");
        }
    }
}
