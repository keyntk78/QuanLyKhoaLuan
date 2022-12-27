using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Antlr.Runtime.Misc;
using DocumentFormat.OpenXml.Bibliography;
using QuanLyKhoaLuan.Models;

namespace QuanLyKhoaLuan.Areas.Admin.Controllers
{
    public class MajorsController : BaseController
    {
        private QuanLyKhoaLuanDBContext db = new QuanLyKhoaLuanDBContext();

        // GET: Admin/Majors
        public ActionResult Index()
        {
            ViewBag.department = db.Departments.ToList();
            if (TempData["status"] != null)
            {
                ViewBag.Status = TempData["status"].ToString();
                TempData.Remove("status");
            }
            return View();
        }


        private bool CheckCode(string code)
        {
            return db.Majors.Count(x => x.code == code) > 0;
        }


        // GET: Admin/Majors/Create
        public ActionResult Create()
        {
            ViewBag.department_id = new SelectList(db.Departments, "department_id", "name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "major_id,code,name,description,department_id,created_at,updated_at")] Major major)
        {
            if (ModelState.IsValid)
            {
                if(CheckCode(major.code))
                {
                    ModelState.AddModelError("", "Mã bộ môn đã tồn tại!");
                } else
                {
                    major.major_id = Guid.NewGuid();
                    major.updated_at = DateTime.Now;
                    major.created_at = DateTime.Now;
                    db.Majors.Add(major);
                    var result = db.SaveChanges();
                    if (result != 0)
                    {
                        TempData["status"] = "Thêm bộ môn thành công!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Thêm Bộ môn  thất bại");
                    }
                }
              
            }

            ViewBag.department_id = new SelectList(db.Departments, "department_id", "name", major.department_id);
            return View(major);
        }


        // string keywork
        public ActionResult GetMajorData(int? page, int? pageSize, string keywork, Guid? department_id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var results = db.Majors
                .Join(db.Departments, m => m.department_id, d => d.department_id, (m, d) => new {
                    major = m,
                    department = d
                })
                .OrderByDescending(m => m.major.updated_at)
                .ToList();


            if (!string.IsNullOrEmpty(keywork))
            {
                results = results.Where(u => u.major.name.ToLower().Contains(keywork.Trim().ToLower()) || u.major.code.ToLower().Contains(keywork.Trim().ToLower()) ||
                u.department.name.ToLower().Contains(keywork.Trim().ToLower())
                ).ToList();
            }

            if (department_id != null)
            {
                results = results.Where(u => u.major.department_id == department_id)
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



        // xóa 
        [HttpPost]
        public ActionResult Delete(Guid id)
        {

            Major major = db.Majors.Find(id);
            db.Majors.Remove(major);
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
            var major = db.Majors.Join(db.Departments, m => m.department_id, d => d.department_id, (m, d) => new {
                major = m,
                department = d
            }).SingleOrDefault(m => m.major.major_id == id);


            if (major != null)
            {
                return Json(new
                {
                    Data = major,
                    Success = true
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                Success = false
            }, JsonRequestBehavior.AllowGet);

        }


        // GET: Admin/Majors/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Major major = db.Majors.Find(id);
            if (major == null)
            {
                return HttpNotFound();
            }
            ViewBag.department_id = new SelectList(db.Departments, "department_id", "code", major.department_id);
            return View(major);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "major_id,code,name,description,department_id,created_at,updated_at")] Major model)
        {
            if (ModelState.IsValid)
            {

                Major major = db.Majors.Find(model.major_id);
                var checkCode = db.Majors.Where(x => x.code != major.code).Count(x => x.code == model.code);
                if (checkCode != 0)
                {
                    ModelState.AddModelError("", "Mã bộ môn đã tồn tại");
                } else
                {
                    major.name = model.name;
                    major.code = model.code;
                    major.department_id = model.department_id;
                    major.description = model.description;

                    major.updated_at = DateTime.Now;


                    db.Entry(major).State = EntityState.Modified;
                    var rs = db.SaveChanges();
                    if (rs != 0)
                    {
                        TempData["status"] = "Cập nhật bộ môn thành công!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Cập nhật bộ môn thất bại");
                    }
                }
          
            }
            ViewBag.department_id = new SelectList(db.Departments, "department_id", "code", model.department_id);
            return View(model);
        }
    }
}
