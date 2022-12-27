using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyKhoaLuan.Common
{
    [Serializable]
    public class UserLogin
    {
        public System.Guid id { get; set; }
        public string username { get; set; }
        public string full_name { get; set; }
        public string password { get; set; }
        public string code_role { get; set; }
        public string avatar { get; set; }


    }
}