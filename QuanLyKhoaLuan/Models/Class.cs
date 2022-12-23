using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace QuanLyKhoaLuan.Models
{
    public class Class
    {
        [Key]
        public System.Guid class_id { get; set; }
        [StringLength(20, ErrorMessage = "Mã lớp không quá 20 ký tự")]
        [Required(ErrorMessage = "Mã lớp không được để trống")]
        public string code { get; set; }
        [Required(ErrorMessage = "Tên lớp không được để trống")]
        [StringLength(255)]
        public string name { get; set; }

        public string description { get; set; }
        public System.Guid major_id { get; set; }


        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }

        public virtual Major Major { get; set; }
    }
}