using FPTJob_1670.Data;
using FPTJob_1670.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FPTJob_1670.Controllers
{
    public class AdminController : Controller
    {
       private readonly ApplicationDbContext _context;

        public AdminController (ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index(string status = "Not Confirmed")
        {
            ViewBag.Status = status;
            IQueryable<Job> jobsQuery = _context.Job;

            if (status == "Accepted")
            {
                jobsQuery = jobsQuery.Where(j => j.statusJobs == "Accepted");
            }
            else if (status == "Rejected")
            {
                jobsQuery = jobsQuery.Where(j => j.statusJobs == "Rejected");
            }
            else // Default: Not Confirmed
            {
                jobsQuery = jobsQuery.Where(j => j.statusJobs == "Not Confirmed");
            }

            var jobs = jobsQuery.ToList();
            return View(jobs);
        }

        [HttpPost]
        public async Task<IActionResult> AcceptJob(int id)
        {
            var job = await _context.Job.FindAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            job.statusJobs = "Accepted";
            _context.Job.Update(job);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RejectJob(int id)
        {
            var job = await _context.Job.FindAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            job.statusJobs = "Rejected";
            _context.Job.Update(job);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        [HttpPost]
        public IActionResult ShowAcceptedJobs()
        {
            return RedirectToAction(nameof(Index), new { status = "Accepted" });
        }

        [HttpPost]
        public IActionResult ShowRejectedJobs()
        {
            return RedirectToAction(nameof(Index), new { status = "Rejected" });
        }



    }
}
