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
    public class School_yearController : Controller
    {
        private QuanLyKhoaLuanDBContext db = new QuanLyKhoaLuanDBContext();

        // GET: Admin/School_year
        public ActionResult Index()
        {
            return View(db.School_years.ToList());
        }

        // GET: Admin/School_year/Details/5
        public ActionResult Details(Guid? id)
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

        // GET: Admin/School_year/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/School_year/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "school_year_id,name,start_date,end_date,created_at,updated_at")] School_year school_year)
        {
            if (ModelState.IsValid)
            {
                school_year.school_year_id = Guid.NewGuid();
                db.School_years.Add(school_year);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(school_year);
        }

        // GET: Admin/School_year/Edit/5
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

        // POST: Admin/School_year/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "school_year_id,name,start_date,end_date,created_at,updated_at")] School_year school_year)
        {
            if (ModelState.IsValid)
            {
                db.Entry(school_year).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(school_year);
        }

        // GET: Admin/School_year/Delete/5
        public ActionResult Delete(Guid? id)
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

        // POST: Admin/School_year/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            School_year school_year = db.School_years.Find(id);
            db.School_years.Remove(school_year);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
