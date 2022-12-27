using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.EMMA;
using QuanLyKhoaLuan.Models;

namespace QuanLyKhoaLuan.Areas.Admin.Controllers
{
    public class School_yearController : BaseController
    {
        private QuanLyKhoaLuanDBContext db = new QuanLyKhoaLuanDBContext();

        // GET: Admin/School_year
        public ActionResult Index()
        {
            if (TempData["status"] != null)
            {
                ViewBag.Status = TempData["status"].ToString();
                TempData.Remove("status");
            }
            return View();
        }

        //int? page, int? pageSize, string keywork
        public ActionResult GetShoolYearData(int? page, int? pageSize, string keywork)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var results = db.School_years.OrderByDescending(d => d.updated_at).ToList();


            if (!string.IsNullOrEmpty(keywork))
            {
                results = results.Where(u => u.name.ToLower().Contains(keywork.Trim().ToLower()))
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


        // GET: Admin/School_year/Create
        public ActionResult Create()
        {
            return View();
        }

        private bool CheckName(string name)
        {
            return db.School_years.Count(x => x.name == name) > 0;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "school_year_id,name,start_date,end_date,created_at,updated_at")] School_year school_year)
        {
            if (ModelState.IsValid)
            {
                if (CheckName(school_year.name))
                {
                    ModelState.AddModelError("", "Niên khóa đã tồn tại");
                } else
                {
                    school_year.school_year_id = Guid.NewGuid();
                    school_year.updated_at = DateTime.Now;
                    school_year.created_at = DateTime.Now;
                    db.School_years.Add(school_year);
                    var result = db.SaveChanges();
                    if (result != 0)
                    {
                        TempData["status"] = "Thêm niên khóa thành công!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Thêm niên khóa thất bại");
                    }
                }
              
            }
            return View(school_year);
        }



        // xóa 
        [HttpPost]
        public ActionResult Delete(Guid id)
        {

            School_year school_Year = db.School_years.Find(id);
            db.School_years.Remove(school_Year);
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
            School_year school_Year = db.School_years.Find(id);
            if (school_Year != null)
            {
                return Json(new
                {
                    Data = school_Year,
                    Success = true
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                Success = false
            }, JsonRequestBehavior.AllowGet);

        }



        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            School_year school_year = db.School_years.Find(id);
            if (school_year == null)
            {
                return HttpNotFound();
            }
            return View(school_year);
        }

       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "school_year_id,name,start_date,end_date,created_at,updated_at")] School_year model)
        {
            if (ModelState.IsValid)
            {
                School_year school_Year = db.School_years.Find(model.school_year_id);

                var checkName = db.School_years.Where(x => x.name != school_Year.name).Count(x => x.name == model.name);
                if (checkName != 0)
                {
                    ModelState.AddModelError("", "Niên khóa đã tồn tại");
                }
                else
                {
                    
                    school_Year.name = model.name;
                    school_Year.start_date = model.start_date;
                    school_Year.end_date = model.end_date;
                    school_Year.updated_at = DateTime.Now;

                    db.Entry(school_Year).State = EntityState.Modified;
                    var rs = db.SaveChanges();
                    if (rs != 0)
                    {
                        TempData["status"] = "Cập nhật niên khóa thành công!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Cập nhật niên khóa  thất bại");
                    }
                }
            }
            return View(model);
        }
    }
}
