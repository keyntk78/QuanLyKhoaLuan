using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace QuanLyKhoaLuan.Models
{
    [Table("Students")]
    public class Student
    {
        [Key]
        public System.Guid student_id { get; set; }
        [Required]
        [StringLength(20)]
        public string code { get; set; }
        [Required]
        [StringLength(255)]
        public string full_name { get; set; }
        [StringLength(100)]
        [Required]
        public string email { get; set; }
        [StringLength(15)]
        public string phone { get; set; }
        public DateTime birthday { get; set; }
        public Nullable<int> gender { get; set; }
        [StringLength(255)]
        public string address { get; set; }
        [Range(0, 10)]
        public double gpa { get; set; }
        public System.Guid user_id { get; set; }
        public System.Guid major_id { get; set; }
        public System.Guid school_year_id { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
        public virtual User User { get; set; }
        public virtual Major Major { get; set; }
        public virtual School_year School_year { get; set; }
    }
}