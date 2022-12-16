using QuanLyKhoaLuan.Common;
using QuanLyKhoaLuan.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
                        user.full_name = model.full_name;
                        user.email = model.email;
                        user.phone = model.phone;
                        if(model.avatar != null)
                        {
                            user.avatar = model.avatar;

                            var seesion = (UserLogin)Session[CommonConstants.USER_SESSION];
                            seesion.avatar = model.avatar;
                            Console.WriteLine(seesion);
                        }
                       
                        db.Entry(user).State = EntityState.Modified;
                        db.SaveChanges();

                        TempData["status"] = "Cập nhật thông tin thành công";
                        return RedirectToAction("Info", "Home");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            return View("Info");
        }


        [HttpPost]
        public string UploatAvatar(HttpPostedFileBase file)
        {
            // validate
            // xử lý upload
             file.SaveAs(Server.MapPath("~/Uploads/Images" + file.FileName));

            var url = "/Uploads/Images" + file.FileName;

            return url;
        }

    }
}