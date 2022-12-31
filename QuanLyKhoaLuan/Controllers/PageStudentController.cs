using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Ajax.Utilities;
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

namespace QuanLyKhoaLuan.Controllers
{
    public class PageStudentController : BaseController
    {
        private QuanLyKhoaLuanDBContext db = new QuanLyKhoaLuanDBContext();

        // GET: PageStudent
        public ActionResult Index()
        {
            if (TempData["status"] != null)
            {
                ViewBag.Status = TempData["status"].ToString();
                TempData.Remove("status");
            }
            return View();
        }

        public Student std()
        {
            var user = (Common.UserLogin)Session[CommonConstants.USER_SESSION];
            var user_id = user.id;
            var student = db.Students.SingleOrDefault(s => s.user_id == user_id);
            return student;
        }

        public ActionResult GetThesisData()
        {
            var student = std();
            var school_year = db.School_years.Find(student.school_year_id);
            var date = "01/12/" + (DateTime.Parse(school_year.end_date.ToString()).Year - 1).ToString();
            var registration_start_date = DateTime.Parse(date);
            var registration_end_date = school_year.end_date;
            var date_now = DateTime.Now;
            var thesis_registration = db.thesis_Registrations.Select(x=>x.thesis_id).ToList();

            if (date_now >= registration_start_date && date_now < registration_end_date)
            {

                if(student.gpa > 5.0)
                {
                    var checkregister = db.thesis_Registrations.Where(x=>x.created_at>=registration_start_date && x.created_at <=registration_end_date && x.student_id == student.student_id).Count();

                    var thesis = (from t in db.Theses
                                  join tp in db.Topics on t.topic_id equals tp.topic_id
                                  join m in db.Majors on t.major_id equals m.major_id
                                  join s in db.School_years on t.school_year_id equals s.school_year_id
                                  join cn in db.councils on t.council_id equals cn.council_id
                                  join l in db.Lecturer on t.lecturer_id equals l.lecturer_id
                                  where m.major_id == student.Class.major_id && t.start_date >= registration_start_date && t.end_date < registration_end_date
                                  orderby t.updated_at descending
                                  select new
                                  {
                                      theses = t,
                                      topic = tp,
                                      school_year = s,
                                      major = m,
                                      council = cn,
                                      lecturer= l,
                                  }).ToList();
                    if(checkregister > 0)
                    {
                        return Json(new
                        {
                            Success = true,
                            IsRegistered = true,
                            Thesis_registration = thesis_registration,
                            Student = student,
                            Data = thesis,
                        }, JsonRequestBehavior.AllowGet);
                    } else
                    {
                        return Json(new
                        {
                            Success = true,
                            IsRegistered = false,
                            Thesis_registration = thesis_registration,
                            Student = student,
                            Data = thesis,
                        }, JsonRequestBehavior.AllowGet);
                    }

                    
                }
                else
                {
                    return Json(new
                    {
                        Success = false,
                        Data = "Chưa đủ thời gian đăng ký",
                    }, JsonRequestBehavior.AllowGet);
                }

              
            } else
            {
                return Json(new
                {
                    Success = false,
                    Data = "Chưa đủ thời gian đăng ký",
                }, JsonRequestBehavior.AllowGet);
            }
           
        }


        public ActionResult Thesis_registration(Guid id)
        {
            var student = std();
            Thesis_registration thesis_Registration = new Thesis_registration();
            thesis_Registration.thesis_registration_id = Guid.NewGuid();
            thesis_Registration.thesis_id = id;
            thesis_Registration.student_id = student.student_id;
            thesis_Registration.created_at = DateTime.Now;
            thesis_Registration.updated_at = DateTime.Now;
            db.thesis_Registrations.Add(thesis_Registration);
            var rs = db.SaveChanges();
            if (rs > 0)
            {
                return Json(new
                {
                    Success = true,
                }, JsonRequestBehavior.AllowGet);
            } else
            {
                return Json(new
                {
                    Success = false,
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult StatusCard()
        {
            var student = std();
            var school_year = db.School_years.Find(student.school_year_id);
            var date = "01/12/" + (DateTime.Parse(school_year.end_date.ToString()).Year - 1).ToString();
            var registration_start_date = DateTime.Parse(date);
            var registration_end_date = school_year.end_date;
            var date_now = DateTime.Now;
            var checkregister = db.thesis_Registrations.Where(x => x.created_at >= registration_start_date && x.created_at <= registration_end_date && x.student_id == student.student_id).Count();
            var count_theses = db.Theses.Where(t=>t.major_id == student.Class.major_id && t.start_date >= registration_start_date && t.end_date < registration_end_date).Count();
            var count_registered_thesis = db.thesis_Registrations
                .Join(db.Theses, x => x.thesis_id, t => t.thesis_id, (x, t) => new { x, t })
                .Where(a=>a.t.major_id == student.Class.major_id && a.t.start_date >= registration_start_date && a.t.end_date < registration_end_date).Count();
           

            if (checkregister > 0)
            {

                var thesis = db.thesis_Registrations
               .Join(db.Theses, x => x.thesis_id, t => t.thesis_id, (x, t) => new { x, thesis = t })
               .Where(a => a.x.student_id == student.student_id && a.thesis.start_date >= registration_start_date && a.thesis.end_date < registration_end_date)
               .FirstOrDefault();

                return Json(new
                {
                    Data = new
                    {
                        Count_theses = count_theses,
                        Count_registered_thesis = count_registered_thesis,
                        Thesis = thesis
                    },
                    Success = true,
                }, JsonRequestBehavior.AllowGet);
            } else
            {
                return Json(new
                {
                    Success = false,
                }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult SubmitOutline(Guid id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult SubmitOutline(SubmitFile model)
        {
            if (ModelState.IsValid)
            {
                Thesis thesis = db.Theses.Find(model.id);
                thesis.file_outline = model.file;

                db.Entry(thesis).State = EntityState.Modified;
                var rs = db.SaveChanges();
                if (rs != 0)
                {
                    TempData["status"] = "Đã nộp đề cương thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Nộp đề cương thất bại");
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }
        //HttpPostedFileBase file,
        public ActionResult ImportFileOutline(HttpPostedFileBase file)
        {

            string extension = Path.GetExtension(file.FileName);
            if(extension == ".docx" || extension == ".doc" || extension == ".pdf")
            {

                string filePath = Server.MapPath("~/Uploads/Outline/");
                string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + file.FileName;

                filePath = filePath + fileName;
                file.SaveAs(filePath);
                string url = "/Uploads/Outline/" + fileName;

                return Json(new
                {
                    Success = true,
                    FileName = fileName,
                    Url = url,
                }, JsonRequestBehavior.AllowGet);

            } else
            {
                return Json(new
                {
                    Success = false,
                }, JsonRequestBehavior.AllowGet);

            }
        }


        public ActionResult SubmitThese(Guid id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult SubmitThese(SubmitFile model)
        {
            if (ModelState.IsValid)
            {
                Thesis thesis = db.Theses.Find(model.id);
                thesis.file_thesis = model.file;

                db.Entry(thesis).State = EntityState.Modified;
                var rs = db.SaveChanges();
                if (rs != 0)
                {
                    TempData["status"] = "Đã nộp khóa luận thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Nộp khóa luận thất bại");
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        public ActionResult ImportFileThese(HttpPostedFileBase file)
        {

            string extension = Path.GetExtension(file.FileName);
            if (extension == ".docx" || extension == ".doc" || extension == ".pdf")
            {

                string filePath = Server.MapPath("~/Uploads/These/");
                string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + file.FileName;

                filePath = filePath + fileName;
                file.SaveAs(filePath);
                string url = "/Uploads/These/" + fileName;

                return Json(new
                {
                    Success = true,
                    FileName = fileName,
                    Url = url,
                }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(new
                {
                    Success = false,
                }, JsonRequestBehavior.AllowGet);

            }
        }

    }
}