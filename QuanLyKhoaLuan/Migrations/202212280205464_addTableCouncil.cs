namespace QuanLyKhoaLuan.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTableCouncil : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Councils",
                c => new
                    {
                        council_id = c.Guid(nullable: false),
                        code = c.String(nullable: false, maxLength: 20),
                        name = c.String(nullable: false, maxLength: 255),
                        description = c.String(),
                        is_block = c.Boolean(nullable: false),
                        school_year_id = c.Guid(nullable: false),
                        created_at = c.DateTime(),
                        updated_at = c.DateTime(),
                    })
                .PrimaryKey(t => t.council_id)
                .ForeignKey("dbo.School_years", t => t.school_year_id, cascadeDelete: true)
                .Index(t => t.school_year_id);
            
            CreateTable(
                "dbo.Detail_Council",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        council_id = c.Guid(nullable: false),
                        lecturer_id = c.Guid(nullable: false),
                        created_at = c.DateTime(),
                        updated_at = c.DateTime(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Councils", t => t.council_id, cascadeDelete: true)
                .ForeignKey("dbo.Lecturers", t => t.lecturer_id, cascadeDelete: true)
                .Index(t => t.council_id)
                .Index(t => t.lecturer_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Detail_Council", "lecturer_id", "dbo.Lecturers");
            DropForeignKey("dbo.Detail_Council", "council_id", "dbo.Councils");
            DropForeignKey("dbo.Councils", "school_year_id", "dbo.School_years");
            DropIndex("dbo.Detail_Council", new[] { "lecturer_id" });
            DropIndex("dbo.Detail_Council", new[] { "council_id" });
            DropIndex("dbo.Councils", new[] { "school_year_id" });
            DropTable("dbo.Detail_Council");
            DropTable("dbo.Councils");
        }
    }
}
