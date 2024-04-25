using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FPTJob_1670.Data;
using FPTJob_1670.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using FPTJob_1670.Settings;
using Microsoft.JSInterop.Implementation;

namespace FPTJob_1670.Controllers
{
    public class JobsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IWebHostEnvironment webHostEnvironment;

        private readonly IHubContext<NotificationHub> _notificationHub;

        private readonly UserManager<IdentityUser> _userManager;
        public JobsController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment, IHubContext<NotificationHub> notificationHub, UserManager<IdentityUser> userManager)
        {
            _context = context;
            webHostEnvironment = hostEnvironment;
            _notificationHub = notificationHub;
            _userManager = userManager;
        }

        [Authorize(Roles = "Employer")]
        // GET: Jobs
        public async Task<IActionResult> Index()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var jobs = await _context.Job
                           .Where(j => j.Employer.EmployerId == currentUserId)
                           .Include(a => a.Employer) 
                           .ToListAsync();

            return View(jobs);
        }

        // GET: Jobs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Job == null)
            {
                return NotFound();
            }

            var job = await _context.Job
                .Include(j => j.Employer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }


        [Authorize(Roles = "Employer")]
        // GET: Jobs/Create
        public IActionResult Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["Employer_id"] = new SelectList(_context.Employer.Where(u => u.EmployerId == userId), "Id", "Name");
            return View();
        }

        // POST: Jobs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Title,Application_Deadline,Salary,Employer_id,ProfileImage")] Job job)
        {
            if (ModelState.IsValid)
            {
                if (job.ProfileImage != null)
                {
                    string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + job.ProfileImage.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await job.ProfileImage.CopyToAsync(fileStream);
                    }
                    job.Image = "/images/" + uniqueFileName;
                }

                job.statusJobs = "Not Confirmed";

                _context.Add(job);
                await _context.SaveChangesAsync();

                var admins = await _userManager.GetUsersInRoleAsync("Admin");
                foreach (var admin in admins)
                {
                    await CreateNotification(admin.Id, "A new job want to accepted");
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["Employer_id"] = new SelectList(_context.Employer, "Id", "Name", job.Employer_id);
            return View(job);
        }



        public async Task CreateNotification(string userId, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
            };

            _context.Notification.Add(notification);
            await _context.SaveChangesAsync();

            await _notificationHub.Clients.User(userId).SendAsync("ReceiveNotification", message);
        }

        public async Task<int> GetUnreadNotificationCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var unreadCount=await _context.Notification.Where(n=>n.UserId== userId && !n.IsRead).CountAsync();
            return unreadCount;
        }

        [Authorize(Roles = "Employer")]
        // GET: Jobs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Job == null)
            {
                return NotFound();
            }

            var job = await _context.Job.FindAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            ViewData["Employer_id"] = new SelectList(_context.Employer, "Id", "Name", job.Employer_id);
            return View(job);
        }

        // POST: Jobs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Title,Application_Deadline,Salary,Employer_id,ProfileImage")] Job job)
        {
            if (id != job.Id)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {

                try
                {
                    var jobUpdate = await _context.Job.FindAsync(id);
                    if (jobUpdate == null)
                    {
                        return NotFound();
                    }

                    // Copy các thông tin mới từ jobSeeker vào jobSeekerToUpdate
                    jobUpdate.Name = job.Name;
                    //jobSeekerToUpdate.Email = jobSeeker.Email;
                    jobUpdate.Description = job.Description;
                    jobUpdate.Title = job.Title;
                    jobUpdate.Application_Deadline = job.Application_Deadline;
                    jobUpdate.Salary = job.Salary;

                    jobUpdate.statusJobs = jobUpdate.statusJobs;

                    // Xử lý tải lên ảnh mới và cập nhật đường dẫn ảnh mới
                    if (job.ProfileImage != null)
                    {
                        string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + job.ProfileImage.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await job.ProfileImage.CopyToAsync(fileStream);
                        }
                        jobUpdate.Image = "/images/" + uniqueFileName;
                    }

                    _context.Update(jobUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobExists(job.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
           ViewData["Employer_id"] = new SelectList(_context.Employer, "Id", "Name", job.Employer_id);
            return View(job);
        }


        [Authorize(Roles = "Employer")]
        // GET: Jobs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Job == null)
            {
                return NotFound();
            }

            var job = await _context.Job
                .Include(j => j.Employer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        // POST: Jobs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Job == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Job'  is null.");
            }
            var job = await _context.Job.FindAsync(id);
            if (job != null)
            {
                _context.Job.Remove(job);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobExists(int id)
        {
            return (_context.Job?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpPost]
        public async Task<IActionResult> AcceptJob (int id)
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





    }
}