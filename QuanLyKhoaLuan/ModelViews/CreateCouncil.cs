using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using QuanLyKhoaLuan.CustomValidation;

namespace QuanLyKhoaLuan.ModelViews
{
    public class CreateCouncil
    {
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
        [Required (ErrorMessage ="Thành viên không được để trống")]
        [ListLecturesCouncil(ErrorMessage = "Số lượng thành viên hội đồng là 3")]
        public List<Guid> lecturer_ids { get; set; }
        
    }
}