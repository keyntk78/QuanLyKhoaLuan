namespace QuanLyKhoaLuan.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatatablethesis : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Theses", "instructor_score", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Theses", "instructor_score");
        }
    }
}
