using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuanLyKhoaLuan.Models
{
    public class Thesis_registration
    {
        [Key]
        public System.Guid thesis_registration_id { get; set; }
        public System.Guid student_id { get; set; }
        public System.Guid thesis_id { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }

        public virtual Student Student { get; set; }
        public virtual Thesis Thesis { get; set; }
    }
}