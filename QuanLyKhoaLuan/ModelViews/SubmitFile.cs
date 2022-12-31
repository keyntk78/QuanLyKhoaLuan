using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuanLyKhoaLuan.ModelViews
{
    public class SubmitFile
    {
        
        public System.Guid id { get; set; }
        [Required(ErrorMessage = "Bạn chưa chọn file")]
        public string file { get; set; }

    }
}