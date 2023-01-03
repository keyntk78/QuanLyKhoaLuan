using System.Web.Mvc;

namespace QuanLyKhoaLuan.Areas.Lecture
{
    public class LectureAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Lecture";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Lecture_default",
                "Lecture/{controller}/{action}/{id}",
                new { action = "Index", controller = "HomeLecture", id = UrlParameter.Optional }
            );
        }
    }
}