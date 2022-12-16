namespace QuanLyKhoaLuan.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateSchool_year : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.School_years");
            AddColumn("dbo.School_years", "school_year_id", c => c.Guid(nullable: false));
            AddPrimaryKey("dbo.School_years", "school_year_id");
            DropColumn("dbo.School_years", "id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.School_years", "id", c => c.Guid(nullable: false));
            DropPrimaryKey("dbo.School_years");
            DropColumn("dbo.School_years", "school_year_id");
            AddPrimaryKey("dbo.School_years", "id");
        }
    }
}
