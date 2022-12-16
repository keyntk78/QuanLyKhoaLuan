namespace QuanLyKhoaLuan.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class validate_department : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Departments", "code", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("dbo.Departments", "founding_date", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Departments", "founding_date", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Departments", "code", c => c.String(maxLength: 20));
        }
    }
}
