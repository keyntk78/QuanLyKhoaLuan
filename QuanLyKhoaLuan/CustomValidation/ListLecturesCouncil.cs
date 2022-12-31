using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Collections;

namespace QuanLyKhoaLuan.CustomValidation
{
    public class ListLecturesCouncil : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var list = value as IList;
            if (list != null)
            {
                if(list.Count == 3)
                {
                    return ValidationResult.Success;
                } else
                {
                    return new ValidationResult(this.ErrorMessage);
                }
            } else
            {
                return new ValidationResult(this.ErrorMessage);
            }

        }
    }
}