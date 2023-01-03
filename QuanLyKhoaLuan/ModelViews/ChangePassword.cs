using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuanLyKhoaLuan.ModelViews
{
    public class ChangePassword
    {
        public int Id { get; set; }
        [StringLength(100, ErrorMessage ="Mật khẩu phải lớn hơn 6 ký tự", MinimumLength =6)]
        [Required (ErrorMessage = "Mật khẩu không được để trống")]
        public string password { get; set; }
        [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải lớn hơn 6 ký tự", MinimumLength = 6)]


        public string newPassword { get; set; }
        [Required(ErrorMessage = "Nhập lại mật khẩu không được để trống")]

        [Compare("newPassword", ErrorMessage ="Nhập lại mật khẩu không chính xác")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải lớn hơn 6 ký tự", MinimumLength = 6)]

        public string confirmPassword { get; set; }
    } 
}