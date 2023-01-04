using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using QuanLyKhoaLuan.Common;
using QuanLyKhoaLuan.Models;
using QuanLyKhoaLuan.ModelViews;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyKhoaLuan.Areas.Lecture.Controllers
{
    public class CouncilLectureController : BaseController
    {
        private QuanLyKhoaLuanDBContext db = new QuanLyKhoaLuanDBContext();
        // GET: Lecture/CouncilLecture
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetCouncilData(int? page, int? pageSize, string keywork)
        {
            db.Configuration.ProxyCreationEnabled = false;

            var seesion = (UserLogin)Session[CommonConstants.USER_SESSION];
            var results = db.councils
                .Join(db.School_years, c => c.school_year_id, s => s.school_year_id, (c, s) => new
                {
                    council = c,
                    shool_year = s
                })
                .Join(db.detail_Councils, c=>c.council.council_id, d=>d.council_id, (c,d) =>new
                {
                    council = c.council,
                    shool_year = c.shool_year,
                    detail_Councils = d
                })
                .Where(x => x.detail_Councils.Lecturer.user_id == seesion.id)
                .OrderByDescending(c => c.council.updated_at)
                .ToList();



            if (!string.IsNullOrEmpty(keywork))
            {
                results = results.Where(u => u.council.name.ToLower().Contains(keywork.Trim().ToLower())
                ).ToList();
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

        public ActionResult ListThesis(Guid id)
        {
            ViewBag.id = id;
            return View();
        }

        //int? page, int? pageSize, string keywork
        public ActionResult GetThesislData(Guid id, int? page, int? pageSize, string keywork)
        {
            db.Configuration.ProxyCreationEnabled = false;

            //var seesion = (UserLogin)Session[CommonConstants.USER_SESSION];
            var results = db.Theses
                .Join(db.thesis_Registrations, t=>t.thesis_id, tr=>tr.thesis_id, (t, tr) => new
                {
                    thesis = t,
                    topic_name = t.Topic.name,
                    council_name = t.Council.name,
                    schoolyear_name = t.School_year.name,
                    student_name = tr.Student.full_name,
                    lecturer_name = t.Lecturer.full_name,
                })
                .Where(t=>t.thesis.council_id == id).ToList();

            if (!string.IsNullOrEmpty(keywork))
            {
                results = results.Where(u => u.topic_name.ToLower().Contains(keywork.Trim().ToLower())
                || u.student_name.ToLower().Contains(keywork.Trim().ToLower())
                ).ToList();
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
            var seesion = (UserLogin)Session[CommonConstants.USER_SESSION];
            var lecture = db.Lecturer.SingleOrDefault(x => x.user_id == seesion.id);
            var score = db.Scores.Where(x => x.lecturer_id == lecture.lecturer_id && x.thesis_id == thesis.thesis_id).SingleOrDefault();
            if(score != null)
            {
                ViewBag.Score = score.score;
                ViewBag.CheckScore = true;
            } else
            {
                ViewBag.Score = null;
                ViewBag.CheckScore = false;
            }
            
           
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
            var seesion = (UserLogin)Session[CommonConstants.USER_SESSION];
            var lecture = db.Lecturer.SingleOrDefault(x => x.user_id == seesion.id);
            var score = db.Scores.Where(x => x.lecturer_id == lecture.lecturer_id && x.thesis_id == thesis.thesis_id).FirstOrDefault();

            if (ModelState.IsValid)
            {

                Score score2 = new Score();
                score2.score_id = Guid.NewGuid();
                score2.lecturer_id = lecture.lecturer_id;
                score2.thesis_id = thesis.thesis_id;
                score2.score = model.Score;
                score2.updated_at = DateTime.Now;
                score2.created_at= DateTime.Now;
                db.Scores.Add(score2);
                db.SaveChanges();

                var count = db.Scores.Where(s=>s.thesis_id == thesis.thesis_id).Count();
                if(count == 3)
                {
                    var list_score = db.Scores.Where(s => s.thesis_id == thesis.thesis_id).Select(x=>x.score).ToList();
                    double total_score_council = 0;
                    foreach (var item in list_score)
                    {
                        total_score_council += (double)item;
                    }

                    double total_score = ((double)thesis.instructor_score * 2 + total_score_council) / 5;
                    total_score = Math.Round(total_score, 2);

                    thesis.total_score = total_score;
                    if(total_score > 5)
                    {
                        thesis.result = 1;
                    }
                    else
                    {
                        thesis.result = 0;
                    }

                    db.Entry(thesis).State = EntityState.Modified;
                    db.SaveChanges();
                }

                score = db.Scores.Where(x => x.lecturer_id == lecture.lecturer_id && x.thesis_id == thesis.thesis_id).FirstOrDefault();
                if (score != null)
                {
                    if (score != null)
                    {
                        ViewBag.Score = score.score;
                        ViewBag.CheckScore = true;
                    }
                    else
                    {
                        ViewBag.Score = null;
                        ViewBag.CheckScore = false;
                    }
                    ViewBag.Status = "Bạn đã chấm điểm";
       
                    ViewBag.member_council = member_council;
                    ViewBag.student_name = student_name;
                    return View(thesis);
                } else
                {
                    if (score != null)
                    {
                        ViewBag.Score = score.score;
                        ViewBag.CheckScore = true;
                    }
                    else
                    {
                        ViewBag.Score = null;
                        ViewBag.CheckScore = false;
                    }
                    ModelState.AddModelError("", "Chấm điểm thất bại");

                   
                    ViewBag.member_council = member_council;
                    ViewBag.student_name = student_name;
                    return View(thesis);
                }
            }
            else
            {
                if (score != null)
                {
                    ViewBag.CheckScore = true;
                    ViewBag.Score = score.score;
                }
                else
                {
                    ViewBag.CheckScore = false;
                    ViewBag.Score = null;
                }
                ModelState.AddModelError("", "Giá trị từ 0-10");

                ViewBag.member_council = member_council;
                ViewBag.student_name = student_name;
                return View(thesis);
            }
        }



    }



    
}