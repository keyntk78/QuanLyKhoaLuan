using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;


namespace QuanLyKhoaLuan.Models
{



    public class QuanLyKhoaLuanDBContext : DbContext
    {

        public QuanLyKhoaLuanDBContext() : base("MyConnecttionsString") { }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get;set; }
        public DbSet<School_year> School_years { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Major> Majors { get; set; }
        public DbSet<Lecturer> Lecturer { get; set;}
        public DbSet<Student> Students { get; set; }
        public DbSet<Class> Classes { get; set; }

    }
}