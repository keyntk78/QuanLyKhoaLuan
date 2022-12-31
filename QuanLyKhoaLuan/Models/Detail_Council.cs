using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuanLyKhoaLuan.Models
{
    public class Detail_Council
    {
        [Key]
        public System.Guid id { get; set; }
        public System.Guid council_id { get; set; }
        public System.Guid lecturer_id { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }

        public virtual Council Council { get; set; }
        public virtual Lecturer Lecturer { get; set; }
    }
}