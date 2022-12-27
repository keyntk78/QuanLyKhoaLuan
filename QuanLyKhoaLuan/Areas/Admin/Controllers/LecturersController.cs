﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Antlr.Runtime.Misc;
using ClosedXML.Excel;
using QuanLyKhoaLuan.Common;
using QuanLyKhoaLuan.Helpper;
using QuanLyKhoaLuan.Models;

namespace QuanLyKhoaLuan.Areas.Admin.Controllers
{
    public class LecturersController : BaseController
    {
        private QuanLyKhoaLuanDBContext db = new QuanLyKhoaLuanDBContext();

        // GET: Admin/Lecturers
        public ActionResult Index()
        {
            if (TempData["status"] != null)
            {
                ViewBag.Status = TempData["status"].ToString();
                TempData.Remove("status");
            }
            ViewBag.department = db.Departments.ToList();

            return View();
        }


        //string keywork, bool? active
        public ActionResult GetLecturersData(int? page, int? pageSize, string keywork, Guid? department_id, bool? active)
        {
            db.Configuration.ProxyCreationEnabled = false;

            var results = (from l in db.Lecturer
                           join u in db.Users on l.user_id equals u.user_id
                           join r in db.Roles on u.role_id equals r.role_id
                           join d in db.Departments on l.department_id equals d.department_id
                           where r.code == "lecture"
                           orderby u.updated_at descending
                           select new
                           {
                               lecture = l,
                               user = u,
                               department = d
                           }).ToList();

            if (!string.IsNullOrEmpty(keywork))
            {
                results = results.Where(l => l.lecture.full_name.ToLower().Contains(keywork.Trim().ToLower())
                || l.lecture.code.ToLower().Contains(keywork.Trim().ToLower())
                || l.lecture.email.ToLower().Contains(keywork.Trim().ToLower())
                )
                    .ToList();
            }

            if (department_id != null)
            {
                results = results.Where(u => u.lecture.department_id == department_id)
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



        // GET: Admin/Lecturers/Create
        public ActionResult Create()
        {
            ViewBag.department_id = new SelectList(db.Departments, "department_id", "name");
            return View();
        }


        private bool CheckCode(string code)
        {
            return db.Lecturer.Count(x => x.code == code) > 0;
        }

        private bool CheckEmail(string email)
        {
            return db.Users.Count(x => x.email == email) > 0;
        }

        private Guid IdRoleLecture()
        {
            var admin = db.Roles.Where(x => x.code == "lecture").FirstOrDefault();
            return admin.role_id;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Lecturer model)
        {
            if (ModelState.IsValid)
            {

                if (CheckCode(model.code))
                {
                    ModelState.AddModelError("", "Mã giáo viên đã tồn tại");
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
                    user.role_id = IdRoleLecture();
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

                    var user_id = db.Users.Where(u => u.username == model.code).FirstOrDefault().user_id;
                    if (user_id != null)
                    {
                        model.code = model.code.ToUpper();
                        model.lecturer_id = Guid.NewGuid();
                        model.updated_at = DateTime.Now;
                        model.created_at = DateTime.Now;
                        model.user_id = user_id;
                        db.Lecturer.Add(model);
                        var result = db.SaveChanges();

                        if (result != 0)
                        {
                            TempData["status"] = "Thêm giảng viên thành công!";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Thêm quản trị viên thất bại");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Thêm giảng viên thất bại");
                    }

                }


            }

            ViewBag.department_id = new SelectList(db.Departments, "department_id", "name", model.department_id);

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

            Lecturer lecturer = db.Lecturer.Find(id);
            User user = db.Users.Find(lecturer.user_id);
            db.Users.Remove(user);
            db.Lecturer.Remove(lecturer);
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


            var result = (from l in db.Lecturer
                          join u in db.Users on l.user_id equals u.user_id
                          join d in db.Departments on l.department_id equals d.department_id
                          select new
                          {
                              user = u,
                              lecture = l,
                              department = d
                          }).SingleOrDefault(l => l.lecture.lecturer_id == id);


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


        // GET: Admin/Lecturers/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lecturer lecturer = db.Lecturer.Find(id);
            if (lecturer == null)
            {
                return HttpNotFound();
            }
            ViewBag.department_id = new SelectList(db.Departments, "department_id", "name", lecturer.department_id);
            ViewBag.avatar = db.Users.Find(lecturer.user_id).avatar;
            return View(lecturer);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Lecturer model)
        {
            var lecturer = db.Lecturer.Find(model.lecturer_id);
            if (ModelState.IsValid)
            {
             
                var checkEmail = db.Lecturer.Where(x => x.email != lecturer.email).Count(x => x.email == model.email);
                var checkCode = db.Lecturer.Where(x => x.code != lecturer.code).Count(x => x.code == model.code);
                var avatar = Request.Form["avatar"];
                if (checkCode != 0)
                {
                    ModelState.AddModelError("", "Mã sinh viên đã tồn tại");
                }
                else if (checkEmail != 0)
                {
                    ModelState.AddModelError("", "Email đã tồn tại");
                } else
                {
                    User user = db.Users.Find(lecturer.user_id);
                    user.full_name = model.full_name;
                    user.username = model.code.ToLower();
                    user.email = model.email;
                    user.phone = model.phone;
                    user.updated_at = DateTime.Now;

                    if (avatar != "")
                    {
                        user.avatar = avatar;
                    }

                    lecturer.code = model.code;
                    lecturer.full_name = model.full_name;
                    lecturer.gender = model.gender;
                    lecturer.phone = model.phone;
                    lecturer.email = model.email;
                    lecturer.address = model.address;
                    lecturer.department_id = model.department_id;
                    lecturer.updated_at = DateTime.Now;
                    lecturer.birthday = model.birthday;


                    db.Entry(user).State = EntityState.Modified;
                    var rs1 = db.SaveChanges();
                    db.Entry(lecturer).State = EntityState.Modified;

                    var rs = db.SaveChanges();
                    if (rs != 0 && rs1 != 0)
                    {
                        TempData["status"] = "Cập nhật giảng viên thành công!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Cập nhật giảng viên thất bại");
                    }
                }
            }
            ViewBag.department_id = new SelectList(db.Departments, "department_id", "name", model.department_id);
            ViewBag.avatar = db.Users.Find(lecturer.user_id).avatar;
            return View(model);
        }

        public FileResult DownloadExcel()
        {
            string filePath = Server.MapPath("~/Uploads/Excel/DanhSachGiangVien.xlsx");
            return File(filePath, "application/vnd.ms-excel", "DanhSachGiangVien.xlsx");
        }

        public ActionResult FormImportFile()
        {

            return View();
        }

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
                            string code_Department = xlWorkbook.Worksheets.Worksheet(1).Cell(row, 8).GetString();

                            var department = db.Departments.SingleOrDefault(x => x.code == code_Department);
                            if (department != null)
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
                                    user.role_id = IdRoleLecture();
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
                                        Lecturer lecturer = new Lecturer();
                                        lecturer.lecturer_id = Guid.NewGuid();
                                        lecturer.code = code.ToUpper();
                                        lecturer.full_name = full_name;
                                        lecturer.phone = phone;
                                        lecturer.email = email;
                                        lecturer.address = address;
                        
                                        if (gender.ToLower() == "nam")
                                        {
                                            lecturer.gender = 1;
                                        }
                                        else
                                        {
                                            lecturer.gender = 0;
                                        }
                                        lecturer.updated_at = DateTime.Now;
                                        lecturer.created_at = DateTime.Now;
                                        lecturer.department_id = department.department_id;
                                        lecturer.user_id = user_id;
                                        lecturer.birthday = birthday;
      

                                        db.Lecturer.Add(lecturer);
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
