using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.EMMA;
using QuanLyKhoaLuan.Helpper;
using QuanLyKhoaLuan.Models;

namespace QuanLyKhoaLuan.Areas.Admin.Controllers
{
    public class StudentsController : BaseController
    {
        private QuanLyKhoaLuanDBContext db = new QuanLyKhoaLuanDBContext();

        // GET: Admin/Students
        public ActionResult Index()
        {
            if (TempData["status"] != null)
            {
                ViewBag.Status = TempData["status"].ToString();
                TempData.Remove("status");
            }

            ViewBag.department = db.Departments.ToList();
            ViewBag.shool_year = db.School_years.ToList();
            ViewBag.classes = db.Classes.ToList();
            ViewBag.major = db.Majors.ToList();
            return View();
        }


        //int? page, int? pageSize, string keywork, Guid? department_id, bool? active
        public ActionResult GetStudentData(int? page, int? pageSize, string keywork, bool? active, Guid? department_id, Guid? major_id, Guid? class_id, Guid? shool_year_id)
        {
            db.Configuration.ProxyCreationEnabled = false;


            var results = (from s in db.Students
                           join u in db.Users on s.user_id equals u.user_id
                           join r in db.Roles on u.role_id equals r.role_id
                           join c in db.Classes on s.class_id equals c.class_id
                           join m in db.Majors on c.major_id equals m.major_id
                           join d in db.Departments on m.department_id equals d.department_id
                           join n in db.School_years on s.school_year_id equals n.school_year_id
                           orderby u.updated_at descending
                           select new
                           {

                               student = s,
                               user = u,
                               classes = c,
                               school_year = n,
                               major = m,
                               departments = d,

                           }).ToList();

            if (!string.IsNullOrEmpty(keywork))
            {
                results = results.Where(s => s.student.full_name.ToLower().Contains(keywork.Trim().ToLower())
                || s.student.code.ToLower().Contains(keywork.Trim().ToLower())
                || s.student.email.ToLower().Contains(keywork.Trim().ToLower())
                )
                    .ToList();
            }

            if (department_id != null)
            {
                results = results.Where(u => u.departments.department_id == department_id)
                    .ToList();
            }

            if (major_id != null)
            {
                results = results.Where(u => u.major.major_id == major_id)
                    .ToList();
            }

            if (class_id != null)
            {
                results = results.Where(u => u.classes.class_id == class_id)
                    .ToList();
            }

            if (shool_year_id != null)
            {
                results = results.Where(u => u.school_year.school_year_id == shool_year_id)
                    .ToList();
            }

            if (active != null)
            {
                results = results.Where(u => u.user.active == active)
                    .ToList();
            }

            var _pageSize = pageSize ?? 5;
            var pageIndex = page ?? 1;

            var totalPage = results.Count();
            var numberPage = Math.Ceiling((float)totalPage / _pageSize);
            results = results.Skip((pageIndex - 1) * _pageSize).Take(_pageSize).ToList();

            return Json(new
            {
                Data = results,
                TotalItem = results.Count,
                CurrentPage = pageIndex,
                NumberPage = numberPage,
                PageSize = _pageSize,
            }, JsonRequestBehavior.AllowGet);


        }


        private bool CheckCode(string code)
        {
            return db.Students.Count(x => x.code == code) > 0;
        }

        private bool CheckEmail(string email)
        {
            return db.Users.Count(x => x.email == email) > 0;
        }

        private Guid IdRoStudent()
        {
            var admin = db.Roles.Where(x => x.code == "student").FirstOrDefault();
            return admin.role_id;
        }


        // GET: Admin/Students/Create
        public ActionResult Create()
        {
            ViewBag.class_id = new SelectList(db.Classes, "class_id", "name");
            ViewBag.school_year_id = new SelectList(db.School_years, "school_year_id", "name");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Student model)
        {
            if (ModelState.IsValid)
            {
                if (CheckCode(model.code))
                {
                    ModelState.AddModelError("", "Mã sinh viên đã tồn tại");
                }
                else if (CheckEmail(model.email))
                {
                    ModelState.AddModelError("", "Email đã tồn tại");
                }
                else
                {
                    User user = new User();
                    user.user_id = Guid.NewGuid();
                    user.full_name = model.full_name;
                    user.username = model.code.ToLower();
                    user.email = model.email;
                    user.phone = model.phone;
                    user.password = model.code.ToLower().ToMD5();
                    user.active = true;
                    user.role_id = IdRoStudent();
                    user.updated_at = DateTime.Now;
                    user.created_at = DateTime.Now;

                    if (model.gender == 1)
                    {
                        user.avatar = "/Uploads/Images/male.png";
                    }
                    else
                    {
                        user.avatar = "/Uploads/Images/female.png";
                    }

                    db.Users.Add(user);
                    db.SaveChanges();

                    var user_id = db.Users.Where(u => u.username == model.code.ToLower()).FirstOrDefault().user_id;
                    if (user_id != null)
                    {
                        model.code = model.code.ToUpper();
                        model.student_id = Guid.NewGuid();
                        model.updated_at = DateTime.Now;
                        model.created_at = DateTime.Now;
                        model.user_id = user_id;
                        db.Students.Add(model);
                        var result = db.SaveChanges();

                        if (result != 0)
                        {
                            TempData["status"] = "Thêm sinh viên thành công!";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Thêm sinh viên thất bại");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Thêm sinh viên thất bại");
                    }
                }
            }

            ViewBag.class_id = new SelectList(db.Classes, "class_id", "name", model.class_id);
            ViewBag.school_year_id = new SelectList(db.School_years, "school_year_id", "name", model.school_year_id);
            return View(model);
        }



        [HttpPost]
        public ActionResult UpdateActive(Guid id)
        {
            User user = db.Users.Find(id);
            if (user.active == true)
            {
                user.active = false;
            }
            else
            {
                user.active = true;
            }

            db.Entry(user).State = EntityState.Modified;
            var rs = db.SaveChanges();
            if (rs > 0)
            {
                return Json(new
                {
                    Success = true
                });
            }
            else
            {
                return Json(new
                {
                    Success = false
                });
            }
        }



        // xóa 
        [HttpPost]
        public ActionResult Delete(Guid id)
        {

            Student student = db.Students.Find(id);
            User user = db.Users.Find(student.user_id);
            db.Users.Remove(user);
            db.Students.Remove(student);
            var rs = db.SaveChanges();

            if (rs > 0)
            {
                return Json(new
                {
                    Success = true
                });
            }
            else
            {
                return Json(new
                {
                    Success = false
                });
            }
        }

        public ActionResult Detail(Guid id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var result = (from s in db.Students
                          join u in db.Users on s.user_id equals u.user_id
                          join r in db.Roles on u.role_id equals r.role_id
                          join c in db.Classes on s.class_id equals c.class_id
                          join m in db.Majors on c.major_id equals m.major_id
                          join d in db.Departments on m.department_id equals d.department_id
                          join n in db.School_years on s.school_year_id equals n.school_year_id
                          select new
                          {

                              student = s,
                              user = u,
                              classes = c,
                              school_year = n,
                              major = m,
                              departments = d,

                          }).SingleOrDefault(s => s.student.student_id == id);


            if (result != null)
            {
                return Json(new
                {
                    Data = result,
                    Success = true
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                Success = false
            }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public string UploatAvatar(HttpPostedFileBase file)
        {
            // validate
            // xử lý upload

            string filePath = Server.MapPath("~/Uploads/Images/");
            string fileName = DateTime.Now.ToString("yyyyMMddHHmmss");
            string extension = Path.GetExtension(file.FileName);

            filePath = filePath + fileName + extension;
            file.SaveAs(filePath);

            var url = "/Uploads/Images/" + fileName + extension;

            return url;
        }


        // GET: Admin/Students/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            ViewBag.class_id = new SelectList(db.Classes, "class_id", "name", student.class_id);
            ViewBag.school_year_id = new SelectList(db.School_years, "school_year_id", "name", student.school_year_id);
            ViewBag.avatar = db.Users.Find(student.user_id).avatar;

            return View(student);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Student model)
        {
            var student = db.Students.Find(model.student_id);
            if (ModelState.IsValid)
            {   
                var checkEmail = db.Students.Where(x => x.email != student.email).Count(x => x.email == model.email);
                var checkCode = db.Students.Where(x => x.code != student.code).Count(x => x.code == model.code);
                var avatar = Request.Form["avatar"];

                if (checkCode != 0)
                {
                    ModelState.AddModelError("", "Mã sinh viên đã tồn tại");
                }
                else if (checkEmail != 0)
                {
                    ModelState.AddModelError("", "Email đã tồn tại");
                }
                else
                {
                    User user = db.Users.Find(student.user_id);
                    user.full_name = model.full_name;
                    user.username = model.code.ToLower();
                    user.email = model.email;
                    user.phone = model.phone;
                    user.updated_at = DateTime.Now;

                    

                    if(avatar != "")
                    {
                        user.avatar = avatar;
                    }


                    student.code = model.code;
                    student.full_name = model.full_name;
                    student.gender = model.gender;
                    student.phone = model.phone;
                    student.email = model.email;
                    student.address = model.address;
                    student.class_id = model.class_id;
                    student.school_year_id = model.school_year_id;
                    student.updated_at = DateTime.Now;
                    student.birthday = model.birthday;
                    student.gpa = model.gpa;

                    db.Entry(user).State = EntityState.Modified;
                    var rs1 = db.SaveChanges();
                    db.Entry(student).State = EntityState.Modified;

                    var rs = db.SaveChanges();
                    if (rs != 0 && rs1 != 0)
                    {
                        TempData["status"] = "Cập nhật sinh viên thành công!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Cập nhật sinh viên thất bại");
                    }

                }
            }
            ViewBag.class_id = new SelectList(db.Classes, "class_id", "name", model.class_id);
            ViewBag.school_year_id = new SelectList(db.School_years, "school_year_id", "name", model.school_year_id);
            ViewBag.user_id = new SelectList(db.Users, "user_id", "username", model.user_id);
            ViewBag.avatar = db.Users.Find(student.user_id).avatar;
            return View(model);
        }

        

        public FileResult DownloadExcel()
        {
            string filePath = Server.MapPath("~/Uploads/Excel/DanhSachSinhVien.xlsx");
            return File(filePath, "application/vnd.ms-excel", "DanhSachSinhVien.xlsx");
        }

        public ActionResult FormImportFile()
        {

          
            return View();
        }

        [HttpPost]
        public ActionResult ImportFile(HttpPostedFileBase myExcelData)
        {

            if (myExcelData != null)
            {
                string extension = Path.GetExtension(myExcelData.FileName);

                if (extension == ".xlsx" || extension == ".xls")
                {
                    if (myExcelData.ContentLength > 0)
                    {
                        string filePath = Server.MapPath("~/Uploads/Excel/");
                        string fileName = DateTime.Now.ToString("yyyyMMddHHmmss");

                        filePath = filePath + fileName + ".xlsx";
                        myExcelData.SaveAs(filePath);

                        XLWorkbook xlWorkbook = new XLWorkbook(filePath);
                        int row = 2;
                        while (xlWorkbook.Worksheets.Worksheet(1).Cell(row, 1).GetString() != "")
                        {
                            string code = xlWorkbook.Worksheets.Worksheet(1).Cell(row, 1).GetString();
                            string full_name = xlWorkbook.Worksheets.Worksheet(1).Cell(row, 2).GetString();
                            string email = xlWorkbook.Worksheets.Worksheet(1).Cell(row, 3).GetString();
                            string phone = xlWorkbook.Worksheets.Worksheet(1).Cell(row, 4).GetString();
                            DateTime birthday = xlWorkbook.Worksheets.Worksheet(1).Cell(row, 5).GetDateTime();
                            string gender = xlWorkbook.Worksheets.Worksheet(1).Cell(row, 6).GetString();
                            string address = xlWorkbook.Worksheets.Worksheet(1).Cell(row, 7).GetString();
                            double gpa = xlWorkbook.Worksheets.Worksheet(1).Cell(row, 8).GetDouble();
                            string code_Class = xlWorkbook.Worksheets.Worksheet(1).Cell(row, 9).GetString();
                            string schoolYear = xlWorkbook.Worksheets.Worksheet(1).Cell(row, 10).GetString();

                            var classes = db.Classes.SingleOrDefault(x => x.code == code_Class);
                            var shool_year = db.School_years.SingleOrDefault(x => x.name == schoolYear);
                            if (classes != null && shool_year != null)
                            {
                                if (CheckCode(code))
                                {
                                    return Json(new
                                    {
                                        Success = false,
                                        Data = "Dữ liệu không hợp lệ đã tồn tại",
                                    }, JsonRequestBehavior.AllowGet);
                                }
                                else if (CheckEmail(email))
                                {
                                    return Json(new
                                    {
                                        Success = false,
                                        Data = "Dữ liệu không hợp lệ đã tồn tại",
                                    }, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    User user = new User();
                                    user.user_id = Guid.NewGuid();
                                    user.full_name = full_name;
                                    user.username = code.ToLower();
                                    user.email = email;
                                    user.phone = phone;
                                    user.password = code.ToLower().ToMD5();
                                    user.active = true;
                                    user.role_id = IdRoStudent();
                                    user.updated_at = DateTime.Now;
                                    user.created_at = DateTime.Now;

                                    if (gender.ToLower() == "nam")
                                    {
                                        user.avatar = "/Uploads/Images/male.png";
                                    }
                                    else
                                    {
                                        user.avatar = "/Uploads/Images/female.png";
                                    }

                                    db.Users.Add(user);
                                    db.SaveChanges();

                                    var user_id = db.Users.Where(u => u.username == code.ToLower()).FirstOrDefault().user_id;
                                    if (user_id != null)
                                    {
                                        Student student = new Student();
                                        student.student_id = Guid.NewGuid();
                                        student.code = code.ToUpper();
                                        student.full_name = full_name;
                                        student.phone = phone;
                                        student.email = email;
                                        student.address = address;
                                        student.gpa = gpa;
                                        if (gender.ToLower() == "nam")
                                        {
                                            student.gender = 1;
                                        }
                                        else
                                        {
                                            student.gender = 0;
                                        }
                                        student.updated_at = DateTime.Now;
                                        student.created_at = DateTime.Now;
                                        student.class_id = classes.class_id;
                                        student.user_id = user_id;
                                        student.birthday = birthday;
                                        student.school_year_id = shool_year.school_year_id;

                                        db.Students.Add(student);
                                        db.SaveChanges();

                                    }
                                    else
                                    {
                                        return Json(new
                                        {
                                            Success = false,
                                            Data = "Dữ liệu không hợp lệ đã tồn tại",
                                        }, JsonRequestBehavior.AllowGet);
                                    }
                                }
                            }
                            else
                            {
                                return Json(new
                                {
                                    Success = false,
                                    Data = "Dữ liệu không hợp lệ đã tồn tại",
                                }, JsonRequestBehavior.AllowGet);
                            }
                            row++;
                        }
                    }

                    return Json(new
                    {
                        Success = true,
                        Data = "Thêm thành công",
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        Success = false,
                        Data = "Sai định dạng file",
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new
                {
                    Success = false,
                    Data = "Bạn chưa chọn file",
                }, JsonRequestBehavior.AllowGet);
            }

        }

    }
}
