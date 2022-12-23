using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Antlr.Runtime.Misc;
using QuanLyKhoaLuan.Common;
using QuanLyKhoaLuan.Helpper;
using QuanLyKhoaLuan.Models;

namespace QuanLyKhoaLuan.Areas.Admin.Controllers
{
    public class LecturersController : BaseController
    {
        private QuanLyKhoaLuanDBContext db = new QuanLyKhoaLuanDBContext();

        // GET: Admin/Lecturers
        public ActionResult Index()
        {
            if (TempData["status"] != null)
            {
                ViewBag.Status = TempData["status"].ToString();
                TempData.Remove("status");
            }
            ViewBag.department = db.Departments.ToList();
            return View();
        }


        //string keywork, bool? active
        public ActionResult GetLecturersData(int? page, int? pageSize, string keywork, Guid? department_id, bool? active)
        {
            db.Configuration.ProxyCreationEnabled = false;

            var results = (from l in db.Lecturer
                           join u in db.Users on l.user_id equals u.user_id
                           join r in db.Roles on u.role_id equals r.role_id
                           join d in db.Departments on l.department_id equals d.department_id
                           where r.code == "lecture"
                           orderby u.updated_at descending
                           select new
                           {
                               lecture = l,
                               user = u,
                               department = d
                           }).ToList();

            if (!string.IsNullOrEmpty(keywork))
            {
                results = results.Where(l => l.lecture.full_name.ToLower().Contains(keywork.Trim().ToLower())
                || l.lecture.code.ToLower().Contains(keywork.Trim().ToLower())
                || l.lecture.email.ToLower().Contains(keywork.Trim().ToLower())
                )
                    .ToList();
            }

            if (department_id != null)
            {
                results = results.Where(u => u.lecture.department_id == department_id)
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



        // GET: Admin/Lecturers/Create
        public ActionResult Create()
        {
            ViewBag.department_id = new SelectList(db.Departments, "department_id", "name");
            return View();
        }


        private bool CheckCode(string code)
        {
            return db.Lecturer.Count(x => x.code == code) > 0;
        }

        private bool CheckEmail(string email)
        {
            return db.Users.Count(x => x.email == email) > 0;
        }

        private Guid IdRoleLecture()
        {
            var admin = db.Roles.Where(x => x.code == "lecture").FirstOrDefault();
            return admin.role_id;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Lecturer model)
        {
            if (ModelState.IsValid)
            {

                if (CheckCode(model.code))
                {
                    ModelState.AddModelError("", "Mã giáo viên đã tồn tại");
                }
                else if (CheckEmail(model.email))
                {
                    ModelState.AddModelError("", "Email đã tồn tại");
                }
                else
                {
                    User user = new User();
                    user.user_id = Guid.NewGuid();
                    user.full_name = model.full_name;
                    user.username = model.code.ToLower();
                    user.email = model.email;
                    user.phone = model.phone;
                    user.password = model.code.ToLower().ToMD5();
                    user.active = true;
                    user.role_id = IdRoleLecture();
                    user.updated_at = DateTime.Now;
                    user.created_at = DateTime.Now;

                    if (model.gender == 1)
                    {
                        user.avatar = "/Uploads/Images/male.png";
                    }
                    else
                    {
                        user.avatar = "/Uploads/Images/female.png";
                    }

                    db.Users.Add(user);
                    db.SaveChanges();

                    var user_id = db.Users.Where(u => u.username == model.code).FirstOrDefault().user_id;
                    if (user_id != null)
                    {
                        model.code = model.code.ToUpper();
                        model.lecturer_id = Guid.NewGuid();
                        model.updated_at = DateTime.Now;
                        model.created_at = DateTime.Now;
                        model.user_id = user_id;
                        db.Lecturer.Add(model);
                        var result = db.SaveChanges();

                        if (result != 0)
                        {
                            TempData["status"] = "Thêm giảng viên thành công!";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Thêm quản trị viên thất bại");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Thêm giảng viên thất bại");
                    }

                }


            }

            ViewBag.department_id = new SelectList(db.Departments, "department_id", "name", model.department_id);

            return View(model);
        }


        [HttpPost]
        public ActionResult UpdateActive(Guid id)
        {
            User user = db.Users.Find(id);
            if (user.active == true)
            {
                user.active = false;
            }
            else
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



        // xóa 
        [HttpPost]
        public ActionResult Delete(Guid id)
        {

            Lecturer lecturer = db.Lecturer.Find(id);
            db.Lecturer.Remove(lecturer);
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



        public ActionResult Detail(Guid id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            var result = (from l in db.Lecturer
                          join u in db.Users on l.user_id equals u.user_id
                          join d in db.Departments on l.department_id equals d.department_id
                           select new
                           {
                               user = u,
                               lecture = l,
                               department = d
                           }).SingleOrDefault(l=>l.lecture.lecturer_id == id);


            if (result != null)
            {
                return Json(new
                {
                    Data = result,
                    Success = true
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                Success = false
            }, JsonRequestBehavior.AllowGet);

        }


        // GET: Admin/Lecturers/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lecturer lecturer = db.Lecturer.Find(id);
            if (lecturer == null)
            {
                return HttpNotFound();
            }
            ViewBag.department_id = new SelectList(db.Departments, "department_id", "code", lecturer.department_id);
            ViewBag.user_id = new SelectList(db.Users, "user_id", "username", lecturer.user_id);
            return View(lecturer);
        }

        // POST: Admin/Lecturers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "lecturer_id,code,full_name,email,phone,birthday,gender,address,user_id,department_id,created_at,updated_at")] Lecturer lecturer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lecturer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.department_id = new SelectList(db.Departments, "department_id", "code", lecturer.department_id);
            ViewBag.user_id = new SelectList(db.Users, "user_id", "username", lecturer.user_id);
            return View(lecturer);
        }


    }
}
