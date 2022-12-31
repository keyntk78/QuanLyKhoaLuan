using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuanLyKhoaLuan.ModelViews
{
    public class LecturerDepartment
    {
        public System.Guid lecturer_id { get; set; }

        public string code { get; set; }

        public string full_name { get; set; }
        public string name_department { get; set; }


    }
}