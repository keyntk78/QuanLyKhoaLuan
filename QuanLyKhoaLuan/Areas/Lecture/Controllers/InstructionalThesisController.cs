using DocumentFormat.OpenXml.Office2010.Excel;
using QuanLyKhoaLuan.Common;
using QuanLyKhoaLuan.Models;
using QuanLyKhoaLuan.ModelViews;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyKhoaLuan.Areas.Lecture.Controllers
{
    public class InstructionalThesisController : BaseController
    {
        private QuanLyKhoaLuanDBContext db = new QuanLyKhoaLuanDBContext();

        // GET: Lecture/InstructionalThesis
        public ActionResult Index()
        {
            ViewBag.shool_year = db.School_years.ToList();
            ViewBag.major = db.Majors.ToList();

            if (TempData["status"] != null)
            {

                ViewBag.Status = TempData["status"].ToString();
                TempData.Remove("status");
            }
            return View();
        }

        //Lecture/InstructionalThesis/GetThesesData
        public ActionResult GetThesesData(int? page, int? pageSize, string keywork, bool? status, Guid? major_id, Guid? shool_year_id)
        {
            var seesion = (UserLogin)Session[CommonConstants.USER_SESSION];

            db.Configuration.ProxyCreationEnabled = false;

            var results = (from t in db.Theses
                           join tp in db.Topics on t.topic_id equals tp.topic_id
                           join m in db.Majors on t.major_id equals m.major_id
                           join s in db.School_years on t.school_year_id equals s.school_year_id
                           join cn in db.councils on t.council_id equals cn.council_id
                           join th_res in db.thesis_Registrations on t.thesis_id equals th_res.thesis_id
                           join st in db.Students on th_res.student_id equals st.student_id
                           where t.Lecturer.user_id == seesion.id
                           orderby t.updated_at descending
                           select new
                           {
                               theses = t,
                               topic = tp,
                               school_year = s,
                               major = m,
                               council = cn,
                               student = st
                           }).ToList();

            if (!string.IsNullOrEmpty(keywork))
            {
                results = results.Where(t => t.council.code.ToLower().Contains(keywork.Trim().ToLower())
                || t.topic.name.ToLower().Contains(keywork.Trim().ToLower())
                || t.student.full_name.ToLower().Contains(keywork.Trim().ToLower())
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

        

        public ActionResult Detail(Guid id)
        {
            var thesis = db.Theses.Find(id);
            var thesis_Registrations = db.thesis_Registrations.SingleOrDefault(x => x.thesis_id == id);
            var student_name = thesis_Registrations.Student.full_name;
            var member_council = db.detail_Councils.Where(d => d.council_id == thesis.council_id).ToList();

           
            ViewBag.member_council = member_council;
            ViewBag.student_name = student_name;
            return View(thesis);
        }


        [HttpPost]
        public ActionResult Detail(MarkScore model)
        {

            var thesis = db.Theses.Find(model.ID);
            var thesis_Registrations = db.thesis_Registrations.SingleOrDefault(x => x.thesis_id == model.ID);
            var student_name = thesis_Registrations.Student.full_name;
            var member_council = db.detail_Councils.Where(d => d.council_id == thesis.council_id).ToList();

            if (ModelState.IsValid)
            {
                thesis.instructor_score = model.Score;
                db.Entry(thesis).State = EntityState.Modified;
                var rs = db.SaveChanges();
                if (rs != 0)
                {
                    ViewBag.Status = "Bạn đã chấm điểm";
                    ViewBag.member_council = member_council;
                    ViewBag.student_name = student_name;
                    return View(thesis);
                }
                else
                {
                    ModelState.AddModelError("", "Chấm điểm thất bại");
                    ViewBag.member_council = member_council;
                    ViewBag.student_name = student_name;
                    return View(thesis);
                }

               
                
            } else
            {
                ModelState.AddModelError("", "Giá trị từ 0-10");
                ViewBag.member_council = member_council;
                ViewBag.student_name = student_name;
                ViewBag.MarkScore = model;
                return View(thesis);
            }
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


        public ActionResult UpdateStatus(Guid id)
        {
            Thesis thesis = db.Theses.Find(id);
            if (thesis.status == true)
            {
                thesis.status = false;
            }
            else
            {
                thesis.status = true;
            }

            db.Entry(thesis).State = EntityState.Modified;
            var rs = db.SaveChanges();
            if (rs > 0)
            {
                return RedirectToAction("Detail", "InstructionalThesis", new { id = id });
            }
            else
            {
                return RedirectToAction("Detail", "InstructionalThesis", new { id = id });
            }
        }


    }
}