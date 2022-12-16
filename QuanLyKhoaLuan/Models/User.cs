using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace QuanLyKhoaLuan.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public System.Guid user_id { get; set; }

        [Required]
        [StringLength(100)]
        public string username { get; set; }
        [Required]
        [StringLength(100)]
        public string password { get; set; }
        [Required]
        [StringLength(255)]
        public string full_name { get; set; }
        [Required]
        [StringLength(255)]
        public string email { get; set; }
        [StringLength(15)]
        public string phone { get; set; }
        public string avatar { get; set; }
        [Required]
        public bool active { get; set; }

        public System.Guid role_id { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }

        public virtual Role Role { get; set; }
    }
}