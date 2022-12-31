namespace QuanLyKhoaLuan.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateThesis : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Theses", "start_date", c => c.DateTime(nullable: false));
            AddColumn("dbo.Theses", "end_date", c => c.DateTime(nullable: false));
            AddColumn("dbo.Theses", "status", c => c.Boolean(nullable: false));
            DropColumn("dbo.Theses", "start_date_registration");
            DropColumn("dbo.Theses", "end_date_registration");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Theses", "end_date_registration", c => c.DateTime(nullable: false));
            AddColumn("dbo.Theses", "start_date_registration", c => c.DateTime(nullable: false));
            DropColumn("dbo.Theses", "status");
            DropColumn("dbo.Theses", "end_date");
            DropColumn("dbo.Theses", "start_date");
        }
    }
}
