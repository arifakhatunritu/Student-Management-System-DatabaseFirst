using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudentManagementSystem_DF.Models.ViewModel
{
    public class StudentVM
    {
        [Key]
        public int StudentId { get; set; }
        [Required,StringLength(100)]
        [Display(Name = "Student Name")]
        public string StudentName { get; set; }
        [Required,Column(TypeName = "date"),Display(Name = "Date of Birth"),DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public DateTime DateOfBirth { get; set; }= DateTime.Now;
        [Required]
        public bool IsRegular { get; set; }
        public HttpPostedFileBase Picture { get; set; }
        public string StudentPhoto { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please select a department.")]

        public int DepartmentId { get; set; }

        public  IEnumerable<SelectListItem> Departments { get; set; }
        public List<Course>  Courses { get; set; }

        public StudentVM()
        {
            Departments = new List<SelectListItem>();
            Courses = new List<Course>();
        }
    }
}