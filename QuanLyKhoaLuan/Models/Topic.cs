using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuanLyKhoaLuan.Models
{
    public class Topic
    {
        [Key]
        public System.Guid topic_id { get; set; }
        [Required (ErrorMessage ="Tên đề tài không được để trống")]
        public string name { get; set; }
        public string description { get; set; }
        public System.Guid department_id { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }

        public virtual Department Department { get; set; }
    }
}