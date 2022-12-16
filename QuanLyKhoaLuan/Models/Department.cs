using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace QuanLyKhoaLuan.Models
{
    [Table("Departments")]
    public class Department
    {
        [Key]
        public System.Guid department_id { get; set; }
        [StringLength(20)]
        public string code { get; set; }
        [Required]
        [StringLength(255)]
        public string name { get; set; }
        public string description { get; set; }
        public DateTime founding_date { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
    }
}