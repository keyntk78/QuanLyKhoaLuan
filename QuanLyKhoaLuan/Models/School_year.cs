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
        [Required (ErrorMessage = "Niên khóa không được để trống")]
        [StringLength(50)]
        public string name { get; set; }
        [Required  (ErrorMessage ="Thời gian bắt đầu không được để trống")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime start_date { get; set; }
        [Required (ErrorMessage = "Thời gian kết thúc không được để trống")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime end_date { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
    }
}