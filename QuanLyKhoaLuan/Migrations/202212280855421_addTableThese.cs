namespace QuanLyKhoaLuan.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTableThese : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Theses",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        code = c.String(nullable: false, maxLength: 20),
                        start_date_registration = c.DateTime(nullable: false),
                        end_date_registration = c.DateTime(nullable: false),
                        start_date_outline = c.DateTime(nullable: false),
                        end_date_outline = c.DateTime(nullable: false),
                        start_date_thesis = c.DateTime(nullable: false),
                        end_date_thesis = c.DateTime(nullable: false),
                        file_thesis = c.String(),
                        file_outline = c.String(),
                        comment = c.String(),
                        total_score = c.Double(),
                        result = c.Int(),
                        topic_id = c.Guid(nullable: false),
                        class_id = c.Guid(nullable: false),
                        school_year_id = c.Guid(nullable: false),
                        council_id = c.Guid(nullable: false),
                        lecturer_id = c.Guid(nullable: false),
                        created_at = c.DateTime(),
                        updated_at = c.DateTime(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Classes", t => t.class_id, cascadeDelete: true)
                .ForeignKey("dbo.Councils", t => t.council_id, cascadeDelete: true)
                .ForeignKey("dbo.Lecturers", t => t.lecturer_id, cascadeDelete: false)
                .ForeignKey("dbo.School_years", t => t.school_year_id, cascadeDelete: false)
                .ForeignKey("dbo.Topics", t => t.topic_id, cascadeDelete: false)
                .Index(t => t.topic_id)
                .Index(t => t.class_id)
                .Index(t => t.school_year_id)
                .Index(t => t.council_id)
                .Index(t => t.lecturer_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Theses", "topic_id", "dbo.Topics");
            DropForeignKey("dbo.Theses", "school_year_id", "dbo.School_years");
            DropForeignKey("dbo.Theses", "lecturer_id", "dbo.Lecturers");
            DropForeignKey("dbo.Theses", "council_id", "dbo.Councils");
            DropForeignKey("dbo.Theses", "class_id", "dbo.Classes");
            DropIndex("dbo.Theses", new[] { "lecturer_id" });
            DropIndex("dbo.Theses", new[] { "council_id" });
            DropIndex("dbo.Theses", new[] { "school_year_id" });
            DropIndex("dbo.Theses", new[] { "class_id" });
            DropIndex("dbo.Theses", new[] { "topic_id" });
            DropTable("dbo.Theses");
        }
    }
}
