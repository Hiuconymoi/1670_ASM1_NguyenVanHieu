using FPTJob_1670.Data;
using FPTJob_1670.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FPTJob_1670.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var jobs = await _context.Job.Where(j => j.statusJobs == "Accepted").ToListAsync();
            return View(jobs);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Search(string searchString)
        {
            var jobs = from j in _context.Job select j;

            
            if (!string.IsNullOrEmpty(searchString))
            {
                
                jobs = jobs.Where(j => j.Name.Contains(searchString) && j.statusJobs == "Accepted");
            }
            else
            {
                
                jobs = jobs.Where(j => j.statusJobs == "Accepted");
            }

            
            return View("Index", await jobs.ToListAsync());
        }
    }
}