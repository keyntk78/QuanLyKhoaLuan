using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Bibliography;
using QuanLyKhoaLuan.Models;

namespace QuanLyKhoaLuan.Areas.Admin.Controllers
{
    public class ClassesController : BaseController
    {
        private QuanLyKhoaLuanDBContext db = new QuanLyKhoaLuanDBContext();

        // GET: 
        public ActionResult Index()
        {
            if (TempData["status"] != null)
            {
                ViewBag.Status = TempData["status"].ToString();
                TempData.Remove("status");
            }

            ViewBag.department = db.Departments.ToList();
            ViewBag.major = db.Majors.ToList();


            return View();
        }

        //int? page, int? pageSize, string keywork, Guid? department_id
        public ActionResult GetClassData(int? page, int? pageSize, string keywork, Guid? major_id, Guid? department_id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var results = db.Classes
                .Join(db.Majors, c => c.major_id, m => m.major_id, (c, m) => new
                {
                    classes = c,
                    majors = m
                })
                .Join(db.Departments, m => m.majors.department_id, d => d.department_id, (m, d) => new
                {
                    department = d,
                    classes = m.classes,
                    majors = m.majors
                })
                .OrderByDescending(c => c.classes.updated_at)
                .ToList();


            if (!string.IsNullOrEmpty(keywork))
            {
                results = results.Where(u => u.classes.name.ToLower().Contains(keywork.Trim().ToLower()) || u.classes.code.ToLower().Contains(keywork.Trim().ToLower()) ||
                u.department.name.ToLower().Contains(keywork.Trim().ToLower())
                ).ToList();
            }

            if (major_id != null)
            {
                results = results.Where(u => u.classes.Major.major_id == major_id)
                    .ToList();
            }

            if (department_id != null)
            {
                results = results.Where(u => u.department.department_id == department_id)
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






        // GET: Admin/Classes/Create
        public ActionResult Create()
        {
            ViewBag.major_id = new SelectList(db.Majors, "major_id", "name");
            return View();
        }



        private bool CheckCode(string code)
        {
            return db.Classes.Count(x => x.code == code) > 0;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "class_id,code,name,description,major_id,created_at,updated_at")] Class model)
        {
            if (ModelState.IsValid)
            {

                if (CheckCode(model.code))
                {
                    ModelState.AddModelError("", "Mã lớp đã tồn tại!");
                }
                else
                {
                    model.class_id = Guid.NewGuid();
                    model.created_at = DateTime.Now;
                    model.updated_at = DateTime.Now;
                    db.Classes.Add(model);
                    var result = db.SaveChanges();
                    if (result != 0)
                    {
                        TempData["status"] = "Thêm lớp thành công!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Thêm lớp  thất bại");
                    }
                }


            }

            ViewBag.major_id = new SelectList(db.Majors, "major_id", "name", model.major_id);
            return View(model);
        }



        // xóa 
        [HttpPost]
        public ActionResult Delete(Guid id)
        {

            Class cla = db.Classes.Find(id);
            db.Classes.Remove(cla);
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
     

            var cls = db.Classes.SingleOrDefault(c=>c.class_id== id);

            if (cls != null)
            {
                return Json(new
                {
                    Data = cls,
                    Success = true
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                Success = false
            }, JsonRequestBehavior.AllowGet);

        }



        // GET: Admin/Classes/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Class cls = db.Classes.Find(id);
            if (cls == null)
            {
                return HttpNotFound();
            }
            ViewBag.major_id = new SelectList(db.Majors, "major_id", "name", cls.major_id);
            return View(cls);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "class_id,code,name,description,major_id,created_at,updated_at")] Class model)
        {
            if (ModelState.IsValid)
            {
                Class cls = db.Classes.Find(model.class_id);
                var checkCode = db.Classes.Where(x => x.code != cls.code).Count(x => x.code == model.code);

                if (checkCode != 0)
                {
                    ModelState.AddModelError("", "Mã lớp đã tồn tại");
                } else
                {
                    cls.name = model.name;
                    cls.code = model.code;
                    cls.major_id = model.major_id;
                    cls.description = model.description;

                    cls.updated_at = DateTime.Now;


                    db.Entry(cls).State = EntityState.Modified;
                    var rs = db.SaveChanges();
                    if (rs != 0)
                    {
                        TempData["status"] = "Cập nhật lớp thành công!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Cập nhật lớp thất bại");
                    }
                }
            }
            ViewBag.major_id = new SelectList(db.Majors, "major_id", "code", model.major_id);
            return View(model);
        }


    }
}
