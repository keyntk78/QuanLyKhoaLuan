using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuanLyKhoaLuan.Models
{
    public class Thesis
    {
        [Key]
        public System.Guid thesis_id { get; set; }
        [Required(ErrorMessage = "Mã khóa luận không được để trống")]
        [StringLength(20)]
        public string code { get; set; }

        [Required (ErrorMessage ="Không được để trống")]
        public DateTime start_date{ get; set; }
        [Required(ErrorMessage = "Không được để trống")]

        public DateTime end_date { get; set; }
        [Required(ErrorMessage = "Không được để trống")]

        public DateTime start_date_outline { get; set; }
        [Required(ErrorMessage = "Không được để trống")]

        public DateTime end_date_outline { get; set; }
        [Required(ErrorMessage = "Không được để trống")]

        public DateTime start_date_thesis { get; set; }
        [Required(ErrorMessage = "Không được để trống")]

        public DateTime end_date_thesis { get; set; }
        public string file_thesis { get; set; }
        public string file_outline { get; set; }

        public string comment { get; set; }
        public bool status { get; set; }

        public Nullable<double> total_score { get; set; }
        public Nullable<int> result { get; set; }
        public System.Guid topic_id { get; set; }
        public System.Guid major_id { get; set; }
        public System.Guid school_year_id { get; set; }
        public System.Guid council_id { get; set; }
        public System.Guid lecturer_id { get; set; }

        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }

        public virtual Topic Topic { get; set; }
        public virtual Major Major { get; set; }
        public virtual Lecturer Lecturer { get; set; }
        public virtual School_year School_year { get; set; }
        public virtual Council Council { get; set; }
    }
}