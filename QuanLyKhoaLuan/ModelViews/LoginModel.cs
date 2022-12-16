using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuanLyKhoaLuan.ModelViews
{
    public class LoginModel
    {
        [Key]
        public System.Guid id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên người dùng")]
        [StringLength(50, ErrorMessage = "Tài khoản chỉ chấp nhận tối đa 50 ký tự")]
        public string username { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(50, ErrorMessage = "Mật khẩu có ít nhất 6 ký tự", MinimumLength = 6)]
        public string password { get; set; }
    }
}