using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace QuanLyKhoaLuan.Models
{
    [Table("Roles")]    
    
    public class Role
    {
        [Key]
        public System.Guid role_id { get; set; }
        [Required]
        [StringLength(20)]
        public string code { get; set; }
        [Required]
        [StringLength(50)]
        public string role_name { get; set; }
        [StringLength(255)]
        public string  description { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }

    }
}