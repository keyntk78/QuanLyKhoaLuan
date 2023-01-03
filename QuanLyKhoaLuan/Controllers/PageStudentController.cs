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
            var seesion = (UserLogin)Session[CommonConstants.USER_SESSION];
            seesion.thesis_Registration = thesis_Registration.thesis_registration_id;
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

                string filePath = Server.MapPath("~/Uploads/Theses/");
                string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + file.FileName;

                filePath = filePath + fileName;
                file.SaveAs(filePath);
                string url = "/Uploads/Theses/" + fileName;

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

        public ActionResult RegisteredTopic(Guid id)
        {

            var thesis_Registration = db.thesis_Registrations.Find(id);

            DateTime date = (DateTime)thesis_Registration.created_at;

            var cancel_date = date.AddDays(7) ;
            var check = false;
            var date_now = DateTime.Now;
            if(date_now < cancel_date)
            {
                check= true;
            }

            var thesis = db.Theses.Find(thesis_Registration.thesis_id);
            var member_council = db.detail_Councils.Where(d=>d.council_id == thesis.council_id).ToList();

            //foreach(var item in member_council)
            //{
            //    item.Lecturer.full_name
            //}

            ViewBag.member_council = member_council;
            ViewBag.checkDate = check;
            ViewBag.id = thesis_Registration.thesis_registration_id;
            return View(thesis);
        }

        
        public ActionResult CancelRegister(Guid id)
        {

            var thesis_Registration = db.thesis_Registrations.Find(id);
            var thesis = db.Theses.Find(thesis_Registration.thesis_id);
            thesis.file_outline = null;
            thesis.file_thesis = null;
            thesis.result = null;
            thesis.total_score = null;
            thesis.instructor_score = null;
            thesis.status = false;
            var seesion = (UserLogin)Session[CommonConstants.USER_SESSION];
            seesion.thesis_Registration = null;

            db.thesis_Registrations.Remove(thesis_Registration);

            var rs = db.SaveChanges();

            if (rs != 0)
            {
                TempData["status"] = "Đã hủy đăng ký đề tài thành công!";
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Hủy đăng ký đề tài thất bại");
                return RedirectToAction("Index");
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
    }
}