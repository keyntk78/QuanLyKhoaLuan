namespace QuanLyKhoaLuan.Migrations
{
    using QuanLyKhoaLuan.Helpper;
    using QuanLyKhoaLuan.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<QuanLyKhoaLuan.Models.QuanLyKhoaLuanDBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(QuanLyKhoaLuan.Models.QuanLyKhoaLuanDBContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
            CreateRoleValue(context);
            if (context.Roles.Any())
            {
                if (!context.Users.Any())
                {
                    var admin = context.Roles.Where(x => x.code == "admin").FirstOrDefault();
                    var id_admin = admin.role_id;
                    var password = "123456";
                    context.Users.AddOrUpdate(x => x.user_id,
                       new User() { user_id = Guid.NewGuid(), username = "admin", password = password.ToMD5(), full_name = "Nguyễn Tuấn Kiệt", email = "key.ntk@gmail.com", phone = "0355882728", role_id = id_admin, active = true, avatar = "male.png", created_at = DateTime.Now, updated_at = DateTime.Now }

                   );
                }
            }
        }

        private void CreateRoleValue(QuanLyKhoaLuanDBContext context)
        {
            if (!context.Roles.Any())
            {
                context.Roles.AddOrUpdate(x => x.role_id,
                   new Role() { role_id = Guid.NewGuid(), code = "admin", role_name = "Admin", description = "Admin", created_at = DateTime.Now, updated_at = DateTime.Now },
                   new Role() { role_id = Guid.NewGuid(), code = "student", role_name = "Sinh Viên", description = "Sinh viên", created_at = DateTime.Now, updated_at = DateTime.Now },
                   new Role() { role_id = Guid.NewGuid(), code = "lecture", role_name = "Giảng viên", description = "Giảng viên", created_at = DateTime.Now, updated_at = DateTime.Now }
                );
            }
        }

    }
}
