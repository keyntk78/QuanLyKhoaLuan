using QuanLyKhoaLuan.Common;
using QuanLyKhoaLuan.Helpper;
using QuanLyKhoaLuan.Models;
using QuanLyKhoaLuan.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static System.Collections.Specialized.BitVector32;

namespace QuanLyKhoaLuan.Controllers
{
    public class LoginController : Controller
    {

        private QuanLyKhoaLuanDBContext db = new QuanLyKhoaLuanDBContext();

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }



        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {

                var password = model.password.ToMD5();

                var result = db.Users.SingleOrDefault(u => u.username == model.username);


                if (result == null)
                {
                    ModelState.AddModelError("", "Tên người dùng không tồn tại");
                    return View("Index");
                }
                else
                {
                    if (result.active == false)
                    {
                        ModelState.AddModelError("", "Tài khoản đã bị khóa");
                        return View("Index");
                    }
                    else
                    {
                        if (result.password == password)
                        {

                            var code_role = db.Roles.SingleOrDefault(r => r.role_id == result.role_id).code;

                            var userSession = new UserLogin();
                            userSession.id = result.user_id;
                            userSession.full_name = result.full_name;
                            userSession.username = result.username;
                            userSession.password = result.password;
                            userSession.code_role = code_role;
                            userSession.avatar = result.avatar;


                            if (code_role == "student")
                            {
                                var student = db.Students.SingleOrDefault(s => s.user_id == result.user_id);
                                var school_year = db.School_years.Find(student.school_year_id);
                                var date = "01/12/" + (DateTime.Parse(school_year.end_date.ToString()).Year - 1).ToString();
                                var registration_start_date = DateTime.Parse(date);
                                var registration_end_date = school_year.end_date;
                                var thesis = db.thesis_Registrations
                                          .Join(db.Theses, x => x.thesis_id, t => t.thesis_id, (x, t) => new { x, thesis = t })
                                          .Where(a => a.x.student_id == student.student_id && a.thesis.start_date >= registration_start_date && a.thesis.end_date < registration_end_date)
                                          .FirstOrDefault();
                                if(thesis != null)
                                {
                                    userSession.thesis_Registration = thesis.x.thesis_registration_id;
                                }
                            }




                            Session.Add(CommonConstants.USER_SESSION, userSession);

                            if (code_role == "student")
                            {
                                return RedirectToAction("Index", "PageStudent");
                            } 
                            else if(code_role == "admin")
                            {
                                return Redirect("~/admin/home");
                            } else
                            {
                                return Redirect("~/Lecture/HomeLecture");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Mật khẩu không hợp lệ");
                            return View("Index");
                        }
                    }

                }
            }

            return View("Index", model);
        }

        public ActionResult Logout()
        {
            Session[CommonConstants.USER_SESSION] = null;
            return RedirectToAction("Index", "Login");
        }
    }
}