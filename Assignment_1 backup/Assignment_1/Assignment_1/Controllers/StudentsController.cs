using Assignment_1.Data;
using Assignment_1.Models;
using Assignment_1.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment_1.Controllers
{
    public class StudentsController : Controller
        
    {
        private readonly ApplicationDbContext dbContext;

        public StudentsController(ApplicationDbContext dbContext)
        {
           this.dbContext = dbContext;


        } 

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(AddStudentViewModel viewmodel) {
            var student = new Student {
                Name = viewmodel.Name,
                Email = viewmodel.Email,
                Phone = viewmodel.Phone,
                Subscribed = viewmodel.Subscribed
           };
            await dbContext.Students.AddAsync(student);
            await dbContext.SaveChangesAsync();
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> List() {
            var students = await dbContext.Students.ToListAsync();
            return View(students);
        }

    }
}
