using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.EMMA;
using QuanLyKhoaLuan.Models;
using QuanLyKhoaLuan.ModelViews;

namespace QuanLyKhoaLuan.Areas.Admin.Controllers
{
    public class ThesesController : BaseController
    {
        private QuanLyKhoaLuanDBContext db = new QuanLyKhoaLuanDBContext();

        // GET: Admin/Theses
        public ActionResult Index()
        {
            if (TempData["status"] != null)
            {
                
                ViewBag.Status = TempData["status"].ToString();
                TempData.Remove("status");
            }
            ViewBag.shool_year = db.School_years.ToList();
            ViewBag.major = db.Majors.ToList();
            return View();
        }


        //int? page, int? pageSize, string keywork, bool? active, Guid? department_id, Guid? major_id, Guid? class_id, Guid? shool_year_id
        public ActionResult GetThesesData(int? page, int? pageSize, string keywork, bool? status, Guid? major_id, Guid? shool_year_id)
        {
            db.Configuration.ProxyCreationEnabled = false;

            var results = (from t in db.Theses
                           join tp in db.Topics on t.topic_id equals tp.topic_id
                           join m in db.Majors on t.major_id equals m.major_id
                           join s in db.School_years on t.school_year_id equals s.school_year_id
                           join cn in db.councils on t.council_id equals cn.council_id
                           orderby t.updated_at descending
                           select new
                           {

                               theses = t,
                               topic = tp,
                               school_year = s,
                               major = m,
                               council = cn
                           }).ToList();

            if (!string.IsNullOrEmpty(keywork))
            {
                results = results.Where(t => t.council.code.ToLower().Contains(keywork.Trim().ToLower())
                || t.topic.name.ToLower().Contains(keywork.Trim().ToLower())
                )
                    .ToList();
            }



            if (major_id != null)
            {
                results = results.Where(u => u.major.major_id == major_id)
                    .ToList();
            }


            if (shool_year_id != null)
            {
                results = results.Where(u => u.school_year.school_year_id == shool_year_id)
                    .ToList();
            }

            if (status != null)
            {
                results = results.Where(u => u.theses.status == status)
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



        // GET: Admin/Theses/Create
        public ActionResult Create()
        {
            ViewBag.major_id = new SelectList(db.Majors, "major_id", "name");
            ViewBag.council_id = db.councils.Where(c => c.is_block == true).ToList();

            ViewBag.school_year_id = new SelectList(db.School_years, "school_year_id", "name");
            ViewBag.topic_id = db.Topics.Join(db.Departments, t => t.department_id, d => d.department_id, (t, d) => new
            {
                topic = t,
                department = d
            }).Select(t => new TopicDepartmet
            {
                topic_id = t.topic.topic_id,
                name = t.topic.name,
                name_department = t.department.name,
            }).ToList();

            ViewBag.lecturer_id = db.Lecturer.Join(db.Departments, l => l.department_id, d => d.department_id, (l, d) => new
            {
                lecturer = l,
                department = d
            }).Select(l => new LecturerDepartment
            {
                lecturer_id = l.lecturer.lecturer_id,
                full_name = l.lecturer.full_name,
                code = l.lecturer.code,
                name_department = l.department.name,
            }).ToList();
            return View();
        }

        private bool CheckCode(string code)
        {
            return db.Theses.Count(x => x.code == code) > 0;
        }

        private bool Checklecturer_id(Guid coucil_id, Guid lecturer_id)
        {
            var detail_council = db.detail_Councils.Where(d => d.council_id == coucil_id).ToList();

            int count = 0;
            foreach (var item in detail_council)
            {
                if (item.lecturer_id == lecturer_id)
                {
                    count++;
                }
            }

            return count > 0;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Thesis model)
        {
            if (ModelState.IsValid)
            {
                if (CheckCode(model.code))
                {
                    ModelState.AddModelError("", "Mã khóa luân đã tồn tại");
                }
                else if (Checklecturer_id(model.council_id, model.lecturer_id))
                {

                    ModelState.AddModelError("", "Giáo viên hướng đẫn đã là thành viên trong hội đồng");

                }
                else
                {
                    Thesis thesis = new Thesis();
                    thesis.thesis_id = Guid.NewGuid();
                    thesis.code = model.code;
                    thesis.start_date = model.start_date;
                    thesis.end_date = model.end_date;
                    thesis.start_date_outline = model.start_date_outline;
                    thesis.end_date_outline = model.end_date_outline;
                    thesis.start_date_thesis = model.start_date_thesis;
                    thesis.end_date_thesis = model.end_date_thesis;
                    thesis.major_id = model.major_id;
                    thesis.status = model.status;
                    thesis.school_year_id = model.school_year_id;
                    thesis.council_id = model.council_id;
                    thesis.lecturer_id = model.lecturer_id;
                    thesis.topic_id = model.topic_id;
                    thesis.updated_at = DateTime.Now;
                    thesis.created_at = DateTime.Now;

                    db.Theses.Add(thesis);
                    var result = db.SaveChanges();
                    if (result != 0)
                    {
                        TempData["status"] = "Thêm khóa luận thành công!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Thêm khóa luận thất bại");
                    }
                }


            }

            ViewBag.major_id = new SelectList(db.Majors, "major_id", "name", model.major_id);


            ViewBag.school_year_id = new SelectList(db.School_years, "school_year_id", "name", model.school_year_id);

            ViewBag.council_id = db.councils.Where(c => c.is_block == true).ToList();


            ViewBag.topic_id = db.Topics.Join(db.Departments, t => t.department_id, d => d.department_id, (t, d) => new
            {
                topic = t,
                department = d
            }).Select(t => new TopicDepartmet
            {
                topic_id = t.topic.topic_id,
                name = t.topic.name,
                name_department = t.department.name,
            }).ToList();

            ViewBag.lecturer_id = db.Lecturer.Join(db.Departments, l => l.department_id, d => d.department_id, (l, d) => new
            {
                lecturer = l,
                department = d
            }).Select(l => new LecturerDepartment
            {
                lecturer_id = l.lecturer.lecturer_id,
                full_name = l.lecturer.full_name,
                code = l.lecturer.code,
                name_department = l.department.name,
            }).ToList();

            return View(model);
        }

        // xóa 
        [HttpPost]
        public ActionResult Delete(Guid id)
        {

            Thesis thesis = db.Theses.Find(id);

            db.Theses.Remove(thesis);

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

            var result = (from t in db.Theses
                           join tp in db.Topics on t.topic_id equals tp.topic_id
                           join m in db.Majors on t.major_id equals m.major_id
                           join s in db.School_years on t.school_year_id equals s.school_year_id
                           join cn in db.councils on t.council_id equals cn.council_id
                           join l in db.Lecturer on t.lecturer_id equals l.lecturer_id
                           orderby t.updated_at descending
                           select new
                           {

                               theses = t,
                               topic = tp,
                               school_year = s,
                               major = m,
                               council = cn,
                               lecturer = l
                           }).SingleOrDefault(t => t.theses.thesis_id == id);

          
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

        public FileResult DownloadOutline(Guid id)
        {
            var thesis = db.Theses.Find(id);
            var file_outline = thesis.file_outline;
            var file_name = Path.GetFileName(file_outline);
            var path = "~" + thesis.file_outline;

            string filePath = Server.MapPath(path);
            return File(filePath, "application/vnd.ms-excel", file_name);
        }

        public FileResult DownloadThese(Guid id)
        {
            var thesis = db.Theses.Find(id);
            var file_thesis = thesis.file_thesis;
            var file_name = Path.GetFileName(file_thesis);
            var path = "~" + thesis.file_thesis;

            string filePath = Server.MapPath(path);
            return File(filePath, "application/vnd.ms-excel", file_name);
        }


        // GET: Admin/Theses/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Thesis thesis = db.Theses.Find(id);
            if (thesis == null)
            {
                return HttpNotFound();
            }

            ViewBag.major_id = new SelectList(db.Majors, "major_id", "name", thesis.major_id);


            ViewBag.school_year_id = new SelectList(db.School_years, "school_year_id", "name", thesis.school_year_id);

            ViewBag.council_id = db.councils.Where(c => c.is_block == true).ToList();


            ViewBag.topic_id = db.Topics.Join(db.Departments, t => t.department_id, d => d.department_id, (t, d) => new
            {
                topic = t,
                department = d
            }).Select(t => new TopicDepartmet
            {
                topic_id = t.topic.topic_id,
                name = t.topic.name,
                name_department = t.department.name,
            }).ToList();

            ViewBag.lecturer_id = db.Lecturer.Join(db.Departments, l => l.department_id, d => d.department_id, (l, d) => new
            {
                lecturer = l,
                department = d
            }).Select(l => new LecturerDepartment
            {
                lecturer_id = l.lecturer.lecturer_id,
                full_name = l.lecturer.full_name,
                code = l.lecturer.code,
                name_department = l.department.name,
            }).ToList();
            return View(thesis);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Thesis model)
        {
            var thesis = db.Theses.Find(model.thesis_id);
            if (ModelState.IsValid)
            {
                var checkCode = db.Theses.Where(x => x.code != thesis.code).Count(x => x.code == model.code);
                var checklecturer = false;
                if (model.lecturer_id != thesis.lecturer_id && model.council_id == thesis.council_id)
                {
                   if (Checklecturer_id(model.council_id, model.lecturer_id))
                    {
                        checklecturer = true;
                    } else
                    {
                        checklecturer = false;
                    }
                } else if(model.council_id != thesis.council_id)
                {
                    if (Checklecturer_id(model.council_id, model.lecturer_id))
                    {
                        checklecturer = true;
                    }
                    else
                    {
                        checklecturer = false;
                    }
                }
                if (checkCode != 0)
                {
                    ModelState.AddModelError("", "Mã khóa luận đã tồn tại");
                } else
                {
                   if(checklecturer)
                    {
                        ModelState.AddModelError("", "Giáo viên đã là thành viên trong hội đồng");
                    } else
                    {
                        thesis.code = model.code;
                        thesis.start_date = model.start_date;
                        thesis.end_date = model.end_date;
                        thesis.start_date_outline = model.start_date_outline;
                        thesis.end_date_outline = model.end_date_outline;
                        thesis.start_date_thesis = model.start_date_thesis;
                        thesis.end_date_thesis = model.end_date_thesis;
                        thesis.major_id = model.major_id;
                        thesis.status = model.status;
                        thesis.school_year_id = model.school_year_id;
                        thesis.council_id = model.council_id;
                        thesis.lecturer_id = model.lecturer_id;
                        thesis.topic_id = model.topic_id;
                        thesis.updated_at = DateTime.Now;
                        db.Entry(thesis).State = EntityState.Modified;
                        var rs = db.SaveChanges();
                        if (rs != 0)
                        {
                            TempData["status"] = "Cập nhật khóa luận thành công!";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Cập nhật khóa luận thất bại");
                        }
                    }
                }

             
            }

            ViewBag.major_id = new SelectList(db.Majors, "major_id", "name", model.major_id);


            ViewBag.school_year_id = new SelectList(db.School_years, "school_year_id", "name", model.school_year_id);

            ViewBag.council_id = db.councils.Where(c => c.is_block == true).ToList();


            ViewBag.topic_id = db.Topics.Join(db.Departments, t => t.department_id, d => d.department_id, (t, d) => new
            {
                topic = t,
                department = d
            }).Select(t => new TopicDepartmet
            {
                topic_id = t.topic.topic_id,
                name = t.topic.name,
                name_department = t.department.name,
            }).ToList();

            ViewBag.lecturer_id = db.Lecturer.Join(db.Departments, l => l.department_id, d => d.department_id, (l, d) => new
            {
                lecturer = l,
                department = d
            }).Select(l => new LecturerDepartment
            {
                lecturer_id = l.lecturer.lecturer_id,
                full_name = l.lecturer.full_name,
                code = l.lecturer.code,
                name_department = l.department.name,
            }).ToList();
            return View(model);
        }
    }
}
