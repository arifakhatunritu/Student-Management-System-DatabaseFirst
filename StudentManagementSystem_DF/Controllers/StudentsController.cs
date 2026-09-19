using StudentManagementSystem_DF.Models;
using StudentManagementSystem_DF.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudentManagementSystem_DF.Controllers
{
    public class StudentsController : Controller
    {
        private StudentDetailsEntities db = new StudentDetailsEntities();
        IEnumerable<SelectListItem>GetDeptList(int?sid=null)=>
            db.Departments.OrderBy(d=>d.DepartmrentName).Select(d => new SelectListItem
            {
                Text = d.DepartmrentName,
                Value = d.DepartmentId.ToString(),
                Selected = sid .HasValue &&   d.DepartmentId== sid.Value
            }).ToList();

            public ActionResult Index()=>View(db.Students.Include("Department").Include("Courses").ToList());
        public ActionResult Create() => View(new StudentVM { Departments = GetDeptList() });
        [HttpPost,ValidateAntiForgeryToken]
        public ActionResult Create(StudentVM vm)
        {
            try
            {
                var s = new Student
                {
                    StudentName = vm.StudentName,
                    DateOfBirth = vm.DateOfBirth,
                    IsRegular = vm.IsRegular,
                    DepartmentId = vm.DepartmentId
                };

                if (vm.Picture != null && vm.Picture.ContentLength > 0)
                {
                    using (var reader = new BinaryReader(vm.Picture.InputStream))
                    {
                        s.StudentPhoto = reader.ReadBytes(vm.Picture.ContentLength);
                    }
                }



                vm.Courses?.Where(x => !string.IsNullOrWhiteSpace(x.CourseName)).ToList()
                            .ForEach(x => s.Courses.Add(new Course { CourseName = x.CourseName, Credit = x.Credit }));
                db.Students.Add(s); db.SaveChanges();
                if (Request.IsAjaxRequest())
                    return PartialView("_success");
            }
            catch
            {
                if (Request.IsAjaxRequest())
                    return PartialView("error");
            }


            return RedirectToAction("Index");

        }
        public ActionResult Edit(int id)
        {
            var s = db.Students.Include("Courses").FirstOrDefault(x => x.StudentId == id);
            return View ( new StudentVM
            {
                StudentId = s.StudentId,
                StudentName = s.StudentName,
                DateOfBirth = s.DateOfBirth,
                IsRegular = s.IsRegular,
                DepartmentId = s.DepartmentId,
                Departments = GetDeptList(s.DepartmentId),
                Courses = s.Courses.ToList()
            });
           
        }
        [HttpPost,ValidateAntiForgeryToken]
        public ActionResult Edit(StudentVM vm)
        {
            try
            {
                var s = db.Students.Find(vm.StudentId);
                
                    s.StudentName = vm.StudentName;
                    s.DateOfBirth = vm.DateOfBirth;
                    s.IsRegular = vm.IsRegular;
                   s. DepartmentId = vm.DepartmentId;
                

                if (vm.Picture != null && vm.Picture.ContentLength > 0)
                {
                    using (var reader = new BinaryReader(vm.Picture.InputStream))
                    {
                        s.StudentPhoto = reader.ReadBytes(vm.Picture.ContentLength);
                    }
                }


                db.Courses.RemoveRange(db.Courses.Where(x => x.StudentId == s.StudentId).ToList());
                vm.Courses?.Where(x => !string.IsNullOrWhiteSpace(x.CourseName)).ToList()
                            .ForEach(x => s.Courses.Add(new Course { CourseName = x.CourseName, Credit = x.Credit }));
                db.SaveChanges();
                if (Request.IsAjaxRequest())
                    return PartialView("_success");
            }
            catch
            {
                if (Request.IsAjaxRequest())
                    return PartialView("error");
            }


            return RedirectToAction("Index");

        }
        



        [HttpPost]
        public JsonResult AjaxDelete(int id)
        {
            var s = db.Students.Find(id);
            db.Courses.RemoveRange(db.Courses.Where(x => x.StudentId == id).ToList());
            db.Students.Remove(s); db.SaveChanges();
            return Json(new { success = true });
        }

    }
}