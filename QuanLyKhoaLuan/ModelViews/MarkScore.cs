using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuanLyKhoaLuan.ModelViews
{
    public class MarkScore
    {
        public Guid ID { get; set; }
        [Required(ErrorMessage ="Điểm không được để trống")]
        [Range(0.0, 10.0, ErrorMessage = "Giá trị từ 0-10")]
        public double Score { get; set; }
    }
}