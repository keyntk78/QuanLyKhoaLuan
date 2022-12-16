using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace QuanLyKhoaLuan.Models
{
    [Table("Lecturers")]
    public class Lecturer
    {
        [Key]
        public System.Guid lecturer_id { get; set; }
        [Required]
        [StringLength(20)]
        public string code { get; set; }
        [Required]
        [StringLength(255)]
        public string full_name { get; set; }
        [Required]
        [StringLength(100)]
        public string email { get; set; }

        [StringLength(15)]        
        public string phone { get; set; }
        public DateTime birthday { get; set; }
        public Nullable<int> gender { get; set; }
        [StringLength(255)]
        public string address { get; set; }
        public System.Guid user_id { get; set; }
        public System.Guid department_id { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
        public virtual User User { get; set; }

        public virtual Department Department { get; set; }
    }
}