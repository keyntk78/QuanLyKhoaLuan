using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QuanLyKhoaLuan.Common;
using QuanLyKhoaLuan.Models;

namespace QuanLyKhoaLuan.Areas.Admin.Controllers
{
    public class DepartmentsController : BaseController
    {
        private QuanLyKhoaLuanDBContext db = new QuanLyKhoaLuanDBContext();

        // GET: Admin/Departments
        public ActionResult Index()
        {
            if (TempData["status"] != null)
            {
                ViewBag.Status = TempData["status"].ToString();
                TempData.Remove("status");
            }
            return View();
        }


        //int? page, int? pageSize, string keywork, bool? active
        public ActionResult GetDepartmentsData(int? page, int? pageSize, string keywork)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var results = db.Departments.OrderByDescending(d => d.updated_at).ToList();


            if (!string.IsNullOrEmpty(keywork))
            {
                results = results.Where(u => u.name.ToLower().Contains(keywork.Trim().ToLower()) || u.code.ToLower().Contains(keywork.Trim().ToLower()))
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

        // GET: Admin/Departments/Create
        public ActionResult Create()
        {
            return View();
        }

        private bool CheckCode(string code)
        {
            return db.Departments.Count(x => x.code == code) > 0;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "department_id,code,name,description,founding_date")] Department department)
        {
            if (ModelState.IsValid)
            {

                if(CheckCode(department.code))
                {
                    ModelState.AddModelError("", "Mã khoa đã tồn tại!");
                } else
                {
                    department.department_id = Guid.NewGuid();
                    department.updated_at = DateTime.Now;
                    department.created_at = DateTime.Now;

                    db.Departments.Add(department);
                    var result = db.SaveChanges();
                    if (result != 0)
                    {
                        TempData["status"] = "Thêm khoa thành công!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Thêm khoa  thất bại");
                    }
                }
            }

            return View(department);
        }




        // xóa 
        [HttpPost]
        public ActionResult Delete(Guid id)
        {

            Department department = db.Departments.Find(id);
            db.Departments.Remove(department);
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


        // GET: Admin/Departments/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "department_id,code,name,description,founding_date,created_at,updated_at")] Department model)
        {
            if (ModelState.IsValid)
            {

                Department department = db.Departments.Find(model.department_id);
                department.code = model.code;
                department.name = model.name;
                department.founding_date = model.founding_date;
                department.updated_at = DateTime.Now;

                db.Entry(department).State = EntityState.Modified;
                var rs =  db.SaveChanges();
                if (rs != 0)
                {
                    TempData["status"] = "Cập nhật khoa thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Cập nhật khoa  thất bại");
                }
            }
            return View(model);
        }

        public ActionResult Detail(Guid id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            if (department != null)
            {
                return Json(new
                {
                    Data = department,
                    Success = true
                }, JsonRequestBehavior.AllowGet); 
            }

            return Json(new
            {
                Success = false
            }, JsonRequestBehavior.AllowGet);

        }

    }
}
