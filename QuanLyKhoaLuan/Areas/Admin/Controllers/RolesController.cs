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
    public class RolesController : Controller
    {
        private QuanLyKhoaLuanDBContext db = new QuanLyKhoaLuanDBContext();

        // GET: Admin/Roles
        public ActionResult Index()
        {
            return View(db.Roles.ToList());
        } 
    }
}
