using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2013.Word;
using QuanLyKhoaLuan.Common;
using QuanLyKhoaLuan.Models;
using QuanLyKhoaLuan.ModelViews;

namespace QuanLyKhoaLuan.Areas.Admin.Controllers
{
    public class CouncilsController : BaseController
    {
        private QuanLyKhoaLuanDBContext db = new QuanLyKhoaLuanDBContext();

        // GET: Admin/Councils
        public ActionResult Index()
        {
            if (TempData["status"] != null)
            {
                ViewBag.Status = TempData["status"].ToString();
                TempData.Remove("status");
            }

            ViewBag.shool_year = db.School_years.ToList();
            return View();
        }

        // int? page, int? pageSize, string keywork, Guid? department_id
        public ActionResult GetCouncilData(int? page, int? pageSize, string keywork, Guid? shool_year_id)
        {
            db.Configuration.ProxyCreationEnabled = false;

            var results = db.councils
                .Join(db.School_years, c=>c.school_year_id, s=>s.school_year_id, (c,s)=> new
                {
                    council = c,
                    shool_year = s
                })
                .GroupJoin(
                db.detail_Councils,
                c=>c.council.council_id,
                d=>d.council_id,
                (c, d) => new { council = c, detail_Councils = d.Join(db.Lecturer, x=>x.lecturer_id, l=>l.lecturer_id, (x,l) => new
                {
                    lecturer = l
                }) }
                )
                .OrderByDescending(c=>c.council.council.updated_at)
                .ToList();



            if (!string.IsNullOrEmpty(keywork))
            {
                results = results.Where(u => u.council.council.name.ToLower().Contains(keywork.Trim().ToLower())
                ).ToList();
            }

            if (shool_year_id != null)
            {
                results = results.Where(u => u.council.shool_year.school_year_id == shool_year_id)
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

        // GET: Admin/Councils/Create
        public ActionResult Create()
        {

            var lecturer_id = (from l in db.Lecturer
                               join d in db.Departments on l.department_id equals d.department_id
                               select new LecturerDepartment
                               {
                                   lecturer_id= l.lecturer_id,
                                   code = l.code,
                                   full_name = l.full_name,
                                   name_department = d.name
                               }).ToList();

            ViewBag.lecturer_id = lecturer_id;

            ViewBag.school_year_id = new SelectList(db.School_years, "school_year_id", "name");
            return View();
        }

        private bool CheckCode(string code)
        {
            return db.councils.Count(x => x.code == code) > 0;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateCouncil model)
        {
           

            if (ModelState.IsValid)
            {
                if (CheckCode(model.code))
                {
                    ModelState.AddModelError("", "Mã hội đồng đã tồn tại");
                }
                else
                {
                    Council council = new Council();
                    var council_id = Guid.NewGuid();
                    council.council_id = council_id;
                    council.code = model.code;
                    council.name = model.name;
                    council.description = model.description;
                    council.school_year_id = model.school_year_id;
                    council.updated_at = DateTime.Now;
                    council.created_at = DateTime.Now;
                    db.councils.Add(council);
                    db.SaveChanges();

                    council_id = db.councils.SingleOrDefault(x => x.council_id == council_id).council_id;


                    foreach (var lecturer_id in model.lecturer_ids)
                    {
                        if (council_id != null)
                        {
                            Detail_Council detail_Council = new Detail_Council();
                            detail_Council.id = Guid.NewGuid();
                            detail_Council.lecturer_id = lecturer_id;
                            detail_Council.council_id = council_id;
                            detail_Council.updated_at = DateTime.Now;
                            detail_Council.created_at = DateTime.Now;

                            db.detail_Councils.Add(detail_Council);
                            db.SaveChanges();
                        }

                    }
                    TempData["status"] = "Thành lập hội đồng thành công!";
                    return RedirectToAction("Index");
                }   
            }

            var lecturers_id = (from l in db.Lecturer
                               join d in db.Departments on l.department_id equals d.department_id
                               select new LecturerDepartment
                               {
                                   lecturer_id = l.lecturer_id,
                                   code = l.code,
                                   full_name = l.full_name,
                                   name_department = d.name
                               }).ToList();

            ViewBag.lecturer_id = lecturers_id;

            ViewBag.school_year_id = new SelectList(db.School_years, "school_year_id", "name", model.school_year_id);
            return View(model);
        }



        public ActionResult Detail(Guid id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var result = db.councils
                .Join(db.School_years, c => c.school_year_id, s => s.school_year_id, (c, s) => new
                {
                    council = c,
                    shool_year = s
                })
                .GroupJoin(
                db.detail_Councils,
                c => c.council.council_id,
                d => d.council_id,
                (c, d) => new {
                    council = c,
                    detail_Councils = d.Join(db.Lecturer, x => x.lecturer_id, l => l.lecturer_id, (x, l) => new
                    {
                        lecturer = l
                    })
                }
                ).SingleOrDefault(c=>c.council.council.council_id == id);


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


        // xóa 
        [HttpPost]
        public ActionResult Delete(Guid id)
        {

            Council council = db.councils.Find(id);
  
            db.councils.Remove(council);

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




        // GET: Admin/Councils/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Council council = db.councils.Find(id);
            var lecturer_ids = db.detail_Councils.Where(x=>x.council_id == council.council_id).Select(x=>x.lecturer_id).ToList();
            CreateCouncil model= new CreateCouncil();
            model.council_id = council.council_id;
            model.code = council.code;
            model.name = council.name;
            model.description = council.description;
            model.is_block = council.is_block;
            model.school_year_id = council.school_year_id;
            model.lecturer_ids = lecturer_ids;
            if (council == null)
            {
                return HttpNotFound();
            }
            ViewBag.school_year_id = new SelectList(db.School_years, "school_year_id", "name", council.school_year_id);
            var lecturers_id = (from l in db.Lecturer
                                join d in db.Departments on l.department_id equals d.department_id
                                select new LecturerDepartment
                                {
                                    lecturer_id = l.lecturer_id,
                                    code = l.code,
                                    full_name = l.full_name,
                                    name_department = d.name
                                }).ToList();

            ViewBag.lecturer_id = lecturers_id;
            return View(model);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CreateCouncil model)
        {
            Council council = db.councils.Find(model.council_id);
            if (ModelState.IsValid)
            {
                var checkCode = db.councils.Where(x => x.code != council.code).Count(x => x.code == model.code);
                if (checkCode != 0)
                {
                    ModelState.AddModelError("", "Mã hội đồng đã tồn tại");
                }else
                {
                    council.code = model.code;
                    council.name = model.name;
                    council.description = model.description;
                    council.is_block= model.is_block;
                    council.updated_at = DateTime.Now;
                    council.school_year_id= model.school_year_id;
                   var detail_council = db.detail_Councils.Where(x=>x.council_id ==model.council_id).ToList();
                    int i = 0;
                    foreach (var item in detail_council)
                    {
                       
                        while(i < model.lecturer_ids.Count)
                        {
                            item.lecturer_id = model.lecturer_ids[i];
                            db.Entry(item).State = EntityState.Modified;
                            db.SaveChanges();
                            break;
                        }

                        i++;
                    }

                    db.Entry(council).State = EntityState.Modified;
                    var rs = db.SaveChanges();
                    if (rs != 0)
                    {
                        TempData["status"] = "Cập nhật hội đồng thành công!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Cập nhật hội đồng thất bại");
                    }
                }

            }


            ViewBag.school_year_id = new SelectList(db.School_years, "school_year_id", "name", model.school_year_id);
            var lecturers_id = (from l in db.Lecturer
                                join d in db.Departments on l.department_id equals d.department_id
                                select new LecturerDepartment
                                {
                                    lecturer_id = l.lecturer_id,
                                    code = l.code,
                                    full_name = l.full_name,
                                    name_department = d.name
                                }).ToList();

            ViewBag.lecturer_id = lecturers_id;
            return View(model);
        }


        [HttpPost]
        public ActionResult UpdateActive(Guid id)
        {
            Council council = db.councils.Find(id);
            if (council.is_block == true)
            {
                council.is_block = false;
            }
            else
            {
                council.is_block = true;
            }

            db.Entry(council).State = EntityState.Modified;
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

    }
}
