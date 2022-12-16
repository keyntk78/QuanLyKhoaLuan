using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace QuanLyKhoaLuan.Models
{
    [Table("School_years")]
    public class School_year
    {
        [Key]
        public System.Guid school_year_id { get; set; }
        [Required]
        [StringLength(50)]
        public string name { get; set; }
        [Required]
        public DateTime start_date { get; set; }
        [Required]
        public DateTime end_date { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
    }
}