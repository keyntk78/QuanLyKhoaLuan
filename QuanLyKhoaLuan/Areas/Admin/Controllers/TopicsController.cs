using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QuanLyKhoaLuan.Models;

namespace QuanLyKhoaLuan.Areas.Admin.Controllers
{
    public class TopicsController : BaseController
    {
        private QuanLyKhoaLuanDBContext db = new QuanLyKhoaLuanDBContext();

        // GET: Admin/Topics
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

        //int? page, int? pageSize, string keywork, Guid? department_id
        public ActionResult GetTopicData(int? page, int? pageSize, string keywork, Guid? department_id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var results = db.Topics
                .Join(db.Departments, t => t.department_id, d => d.department_id, (t, d) => new
                {
                    topic = t,
                    department = d
                })
                .OrderByDescending(m => m.topic.updated_at)
                .ToList();


            if (!string.IsNullOrEmpty(keywork))
            {
                results = results.Where(u => u.topic.name.ToLower().Contains(keywork.Trim().ToLower())
                ).ToList();
            }

            if (department_id != null)
            {
                results = results.Where(u => u.topic.department_id == department_id)
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


        // GET: Admin/Topics/Create
        public ActionResult Create()
        {
            ViewBag.department_id = new SelectList(db.Departments, "department_id", "name");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "topic_id,name,description,department_id,created_at,updated_at")] Topic topic)
        {
            if (ModelState.IsValid)
            {
                topic.topic_id = Guid.NewGuid();
                topic.updated_at= DateTime.Now;
                topic.created_at= DateTime.Now;
                db.Topics.Add(topic);
                var result = db.SaveChanges();
                if (result != 0)
                {
                    TempData["status"] = "Thêm đề tài thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Thêm đề tài thất bại");
                }
            }

            ViewBag.department_id = new SelectList(db.Departments, "department_id", "name", topic.department_id);
            return View(topic);
        }


        // xóa 
        [HttpPost]
        public ActionResult Delete(Guid id)
        {

            Topic topic = db.Topics.Find(id);
            db.Topics.Remove(topic);
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
            var topic = db.Topics.Join(db.Departments, t => t.department_id, d => d.department_id, (t, d) => new {
                topic = t,
                department = d
            }).SingleOrDefault(m => m.topic.topic_id == id);


            if (topic != null)
            {
                return Json(new
                {
                    Data = topic,
                    Success = true
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                Success = false
            }, JsonRequestBehavior.AllowGet);

        }

       


        // GET: Admin/Topics/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Topic topic = db.Topics.Find(id);
            if (topic == null)
            {
                return HttpNotFound();
            }
            ViewBag.department_id = new SelectList(db.Departments, "department_id", "name", topic.department_id);
            return View(topic);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "topic_id,name,description,department_id,created_at,updated_at")] Topic model)
        {
            if (ModelState.IsValid)
            {
                Topic topic = db.Topics.Find(model.topic_id);
                topic.name = model.name;
                topic.description = model.description;
                topic.department_id = model.department_id;
                topic.updated_at= DateTime.Now;

                db.Entry(topic).State = EntityState.Modified;
                var rs = db.SaveChanges();
                if (rs != 0)
                {
                    TempData["status"] = "Cập nhật đề tài thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Cập nhật đề tài thất bại");
                }
            }
            ViewBag.department_id = new SelectList(db.Departments, "department_id", "code", model.department_id);
            return View(model);
        }
    }
}
