using QuanLyKhoaLuan.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace QuanLyKhoaLuan.Areas.Lecture.Controllers
{
    public class BaseController : Controller
    {
        // GET: Lecture/Base
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = (UserLogin)Session[CommonConstants.USER_SESSION];

            if (session == null)
            {
                filterContext.Result = new RedirectToRouteResult(new
                    RouteValueDictionary(new { controller = "Login", action = "Index", area = "" }));
            }
            else
            {
                if (session.code_role == "admin")
                {
                    filterContext.Result = new RedirectToRouteResult(new
                    RouteValueDictionary(new { controller = "Login", action = "Index", area = "" }));
                }

                if (session.code_role == "student")
                {
                    filterContext.Result = new RedirectToRouteResult(new
                    RouteValueDictionary(new { controller = "Login", action = "Index", area = "" }));
                }

                base.OnActionExecuting(filterContext);
            }
        }
    }
}