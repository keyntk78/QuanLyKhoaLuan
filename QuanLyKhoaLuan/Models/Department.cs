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
        [StringLength(20, ErrorMessage ="Mã khoa không quá 20 ký tự")]
        [Required(ErrorMessage ="Mã khoa không được để trống")]
        public string code { get; set; }
        [Required(ErrorMessage = "Tên khoa không được để trống")]
        [StringLength(255)]
        public string name { get; set; }
        public string description { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> founding_date { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
    }
}