using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuanLyKhoaLuan.Models
{
    public class Score
    {
        [Key]
        public System.Guid score_id { get; set; }
        public System.Guid lecturer_id { get; set; }
        public System.Guid thesis_id { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
        public Nullable<double> score { get; set; }

        public virtual Thesis Thesis { get; set; }
        public virtual Lecturer Lecturer { get; set; }

    }
}