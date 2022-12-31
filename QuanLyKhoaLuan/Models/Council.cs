using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuanLyKhoaLuan.Models
{
    public class Council
    {
        [Key]
        public System.Guid council_id { get; set; }
        [Required(ErrorMessage = "Mã hội đồng không được để trống")]
        [StringLength(20)]
        public string code { get; set; }
        [Required(ErrorMessage = "Tên hội đồng không được để trống")]
        [StringLength(255)]
        public string name { get; set; }
        public string description { get; set; }
        public bool is_block { get; set; }
        public System.Guid school_year_id { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
        public virtual School_year School_year { get; set; }
    }
}