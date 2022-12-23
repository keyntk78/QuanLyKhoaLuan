namespace QuanLyKhoaLuan.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datime_foundingdat : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Departments", "founding_date", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Departments", "founding_date", c => c.DateTime());
        }
    }
}
