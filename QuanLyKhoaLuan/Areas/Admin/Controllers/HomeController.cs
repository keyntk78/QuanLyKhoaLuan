﻿using QuanLyKhoaLuan.Common;
using QuanLyKhoaLuan.Helpper;
using QuanLyKhoaLuan.Models;
using QuanLyKhoaLuan.ModelViews;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyKhoaLuan.Areas.Admin.Controllers
{
    public class HomeController : BaseController
    {
        private QuanLyKhoaLuanDBContext db = new QuanLyKhoaLuanDBContext();

        // GET: Admin/Home
        public ActionResult Index()
        {

            var count_student = db.Students.Count();
            var count_lectuter = db.Lecturer.Count();
            var count_thesis = db.Theses.Count();
            var count_thesis_finish = db.Theses.Where(x => x.total_score != null).Count();
            if (TempData["status"] != null)
            {
                ViewBag.Status = TempData["status"].ToString();
                TempData.Remove("status");
            }
            ViewBag.count_student = count_student;
            ViewBag.count_lectuter = count_lectuter;
            ViewBag.count_thesis = count_thesis;
            ViewBag.count_thesis_finish = count_thesis_finish;
            return View();
        }


        public ActionResult Info()
        {
            var session = (UserLogin)Session[CommonConstants.USER_SESSION];
            if (session != null)
            {
                var user = db.Users.SingleOrDefault(u => u.user_id == session.id);

                if (TempData["status"] != null)
                {
                    ViewBag.Status = TempData["status"].ToString();
                    TempData.Remove("status");
                }

                return View(user);
            }

            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        public ActionResult Info(User model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    User user = db.Users.Find(model.user_id);
                    if (user == null)
                    {
                        return HttpNotFound();
                    }
                    else
                    {
                        var checkEmail = db.Users.Where(x => x.email != user.email).Count(x => x.email == model.email);
                        if (checkEmail > 0)
                        {
                            ModelState.AddModelError("", "Email đã tồn tại");
                        }
                        else
                        {
                            user.full_name = model.full_name;
                            user.email = model.email;
                            user.phone = model.phone;
                            if (model.avatar != null)
                            {
                                user.avatar = model.avatar;

                                var seesion = (UserLogin)Session[CommonConstants.USER_SESSION];
                                seesion.avatar = model.avatar;
                            }

                            db.Entry(user).State = EntityState.Modified;
                            db.SaveChanges();

                            TempData["status"] = "Cập nhật thông tin thành công";
                            return RedirectToAction("Info", "Home");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            return View(model);
        }

        [HttpPost]
        public string UploatAvatar(HttpPostedFileBase file)
        {
            // validate
            // xử lý upload

            string filePath = Server.MapPath("~/Uploads/Images/");
            string fileName = DateTime.Now.ToString("yyyyMMddHHmmss");
            string extension = Path.GetExtension(file.FileName);

            filePath = filePath + fileName + extension;
            file.SaveAs(filePath);

            var url = "/Uploads/Images/" + fileName + extension;

            return url;
        }

        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(ChangePassword model)
        {
            if (ModelState.IsValid)
            {
                var password = model.password.ToMD5();
                var seesion = (UserLogin)Session[CommonConstants.USER_SESSION];
                var user = db.Users.Find(seesion.id);
                if (user.password == password)
                {
                    user.password = model.newPassword.ToMD5();
                    seesion.password = model.newPassword.ToMD5();
                    db.Entry(user).State = EntityState.Modified;
                    var rs = db.SaveChanges();
                    if (rs != 0)
                    {
                        TempData["status"] = "Đổi mật khẩu thành công!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Đổi mật khẩu thất bại");
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Mật khẩu không đúng");
                }
            }
            return View();
        }
    }
}