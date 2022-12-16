namespace QuanLyKhoaLuan.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateNewTable_1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        department_id = c.Guid(nullable: false),
                        code = c.String(maxLength: 20),
                        name = c.String(nullable: false, maxLength: 255),
                        description = c.String(),
                        founding_date = c.DateTime(nullable: false),
                        created_at = c.DateTime(),
                        updated_at = c.DateTime(),
                    })
                .PrimaryKey(t => t.department_id);
            
            CreateTable(
                "dbo.Lecturers",
                c => new
                    {
                        lecturer_id = c.Guid(nullable: false),
                        code = c.String(nullable: false, maxLength: 20),
                        full_name = c.String(nullable: false, maxLength: 255),
                        email = c.String(nullable: false, maxLength: 100),
                        phone = c.String(maxLength: 15),
                        birthday = c.DateTime(nullable: false),
                        gender = c.Int(),
                        address = c.String(maxLength: 255),
                        user_id = c.Guid(nullable: false),
                        department_id = c.Guid(nullable: false),
                        created_at = c.DateTime(),
                        updated_at = c.DateTime(),
                    })
                .PrimaryKey(t => t.lecturer_id)
                .ForeignKey("dbo.Departments", t => t.department_id, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.user_id, cascadeDelete: true)
                .Index(t => t.user_id)
                .Index(t => t.department_id);
            
            CreateTable(
                "dbo.Majors",
                c => new
                    {
                        major_id = c.Guid(nullable: false),
                        code = c.String(nullable: false, maxLength: 20),
                        name = c.String(nullable: false, maxLength: 255),
                        description = c.String(),
                        department_id = c.Guid(nullable: false),
                        created_at = c.DateTime(),
                        updated_at = c.DateTime(),
                    })
                .PrimaryKey(t => t.major_id)
                .ForeignKey("dbo.Departments", t => t.department_id, cascadeDelete: true)
                .Index(t => t.department_id);
            
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        student_id = c.Guid(nullable: false),
                        code = c.String(nullable: false, maxLength: 20),
                        full_name = c.String(nullable: false, maxLength: 255),
                        email = c.String(nullable: false, maxLength: 100),
                        phone = c.String(maxLength: 15),
                        birthday = c.DateTime(nullable: false),
                        gender = c.Int(),
                        address = c.String(maxLength: 255),
                        gpa = c.Double(nullable: false),
                        user_id = c.Guid(nullable: false),
                        major_id = c.Guid(nullable: false),
                        school_year_id = c.Guid(nullable: false),
                        created_at = c.DateTime(),
                        updated_at = c.DateTime(),
                    })
                .PrimaryKey(t => t.student_id)
                .ForeignKey("dbo.Majors", t => t.major_id, cascadeDelete: true)
                .ForeignKey("dbo.School_years", t => t.school_year_id, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.user_id, cascadeDelete: true)
                .Index(t => t.user_id)
                .Index(t => t.major_id)
                .Index(t => t.school_year_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Students", "user_id", "dbo.Users");
            DropForeignKey("dbo.Students", "school_year_id", "dbo.School_years");
            DropForeignKey("dbo.Students", "major_id", "dbo.Majors");
            DropForeignKey("dbo.Majors", "department_id", "dbo.Departments");
            DropForeignKey("dbo.Lecturers", "user_id", "dbo.Users");
            DropForeignKey("dbo.Lecturers", "department_id", "dbo.Departments");
            DropIndex("dbo.Students", new[] { "school_year_id" });
            DropIndex("dbo.Students", new[] { "major_id" });
            DropIndex("dbo.Students", new[] { "user_id" });
            DropIndex("dbo.Majors", new[] { "department_id" });
            DropIndex("dbo.Lecturers", new[] { "department_id" });
            DropIndex("dbo.Lecturers", new[] { "user_id" });
            DropTable("dbo.Students");
            DropTable("dbo.Majors");
            DropTable("dbo.Lecturers");
            DropTable("dbo.Departments");
        }
    }
}
