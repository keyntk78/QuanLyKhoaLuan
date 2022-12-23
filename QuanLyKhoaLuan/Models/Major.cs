﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace QuanLyKhoaLuan.Models
{
    [Table("Majors")]
    public class Major
    {
        [Key]
        public System.Guid major_id { get; set; }
        [Required (ErrorMessage ="Mã bộ môn không được để trống")]
        [StringLength(20)]
        public string code { get; set; }
        [StringLength(255)]
        [Required(ErrorMessage = "Tên bộ môn không được để trống")]
        public string name { get; set; }
        public string description { get; set; }
        public System.Guid department_id { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }

        public virtual Department Department { get; set; }
    }
}