using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QuanLyKhoaLuan.Common;
using QuanLyKhoaLuan.Helpper;
using QuanLyKhoaLuan.Models;
using QuanLyKhoaLuan.ModelViews;

namespace QuanLyKhoaLuan.Areas.Admin.Controllers
{
    public class UsersController : BaseController
    {
        private QuanLyKhoaLuanDBContext db = new QuanLyKhoaLuanDBContext();

        // GET: Admin/Users
        public ActionResult Index()
        {

            if (TempData["status"] != null)
            {
                ViewBag.Status = TempData["status"].ToString();
                TempData.Remove("status");
            }
            return View();
        }


        public ActionResult GetUsersData(int? page, int? pageSize, string keywork, bool? active)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var results = (from u in db.Users
                           join r in db.Roles on u.role_id equals r.role_id
                           where r.code == "admin"
                           orderby u.updated_at descending
                           select new
                           {
                               user = u,
                               r.role_name
                           }).ToList();

            if (!string.IsNullOrEmpty(keywork))
            {
                results = results.Where(u => u.user.full_name.ToLower().Contains(keywork.Trim().ToLower()) || u.user.username.ToLower().Contains(keywork.Trim().ToLower()))
                    .ToList();
            }

            if (active != null)
            {
                results = results.Where(u => u.user.active == active)
                    .ToList();
            }

            var _pageSize = pageSize ?? 5;
            var pageIndex = page ?? 1;

            var totalPage = results.Count();
            var numberPage = Math.Ceiling((float)totalPage / _pageSize);
            results = results.Skip((pageIndex - 1) * _pageSize).Take(_pageSize).ToList();

            return Json(new
            {
                Data = results,
                TotalItem = results.Count,
                CurrentPage = pageIndex,
                NumberPage = numberPage,
                PageSize = _pageSize,
            }, JsonRequestBehavior.AllowGet);


        }



        // GET: Admin/Users/Create
        [HttpGet]
        public ActionResult Create()
        {

            return View();
        }

        private bool CheckUserName(string userName)
        {
            return db.Users.Count(x => x.username == userName) > 0;
        }

        private bool CheckEmail(string email)
        {
            return db.Users.Count(x => x.email == email) > 0;
        }

        private Guid IdAdmin()
        {
            var admin = db.Roles.Where(x => x.code == "admin").FirstOrDefault();
            return admin.role_id;
        }

        [HttpPost]
        public ActionResult Create(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                if (CheckUserName(model.username))
                {
                    ModelState.AddModelError("", "Tên người dùng đã tồn tại");
                }
                else if (CheckEmail(model.email))
                {
                    ModelState.AddModelError("", "Email đã tồn tại");
                }
                else
                {
                    var user = new User();
                    user.user_id = Guid.NewGuid();
                    user.username = model.username;
                    user.full_name = model.full_name;
                    user.password = model.password.ToMD5();
                    user.email = model.email;
                    user.phone = model.phone;
                    user.active = model.active;
                    if(model.avatar == null)
                    {
                        user.avatar = "/Uploads/Images/male.png";
                    } else
                    {
                        user.avatar = model.avatar;
                    }
                    user.role_id = IdAdmin();
                    user.updated_at = DateTime.Now;
                    user.created_at = DateTime.Now;

                    db.Users.Add(user);
                    var result = db.SaveChanges();
                    if (result != 0)
                    {
                        TempData["status"] = "Thêm quản trị viên thành công!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Thêm quản trị viên thất bại");
                    }
                }
            }
            return View(model);
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


        // xóa 
        [HttpPost]
        public ActionResult Delete(Guid user_id)
        {

            var session = (UserLogin)Session[CommonConstants.USER_SESSION];
            var rs = 0;
            if (session.id != user_id)
            {
                User user = db.Users.Find(user_id);
                db.Users.Remove(user);
                rs = db.SaveChanges();
            } 

            if (rs > 0)
            {
                return Json(new
                {
                    Success = true
                });
            }
            else
            {
                return Json(new
                {
                    Success = false
                });
            }
        }

        [HttpPost]
        public ActionResult UpdateActive(Guid id)
        {
            User user = db.Users.Find(id);
            if(user.active == true)
            {
                user.active = false;
            } else
            {
                user.active = true;
            }

            db.Entry(user).State = EntityState.Modified;
            var rs = db.SaveChanges();
            if (rs > 0)
            {
                return Json(new
                {
                    Success = true
                });
            }
            else
            {
                return Json(new
                {
                    Success = false
                });
            }
        }


        // GET: Admin/Users/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User model)
        {
            if (ModelState.IsValid)
            {
                User user = db.Users.Find(model.user_id);
                user.full_name = model.full_name;
                user.phone = model.phone;
                user.email = model.email;
                user.active = model.active;
                user.updated_at = DateTime.Now;

                if(model.avatar != null)
                {
                    user.avatar = model.avatar;
                }

                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();

                TempData["status"] = "Cập nhật quản trị viên thành công!";
                return RedirectToAction("Index");
            }

            return View("Edit");
        }

    }
}
