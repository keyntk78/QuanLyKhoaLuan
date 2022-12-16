using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuanLyKhoaLuan.Controllers
{
    public class PageStudentController : BaseController
    {
        // GET: PageStudent
        public ActionResult Index()
        {
            return View();
        }
    }
}