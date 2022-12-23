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
        [Required (ErrorMessage = "Mã giảng viên không được để trống")]
        [StringLength(20)]
        public string code { get; set; }
        [Required(ErrorMessage = "Tên giảng viên không được để trống")]
        [StringLength(255)]
        public string full_name { get; set; }
        [Required(ErrorMessage = "Email không được để trống")]
        [StringLength(100)]
        public string email { get; set; }

        [StringLength(15)]        
        public string phone { get; set; }
        [Required(ErrorMessage = "Ngày sinh không được để trống")]
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