using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FPTJob_1670.Data;
using FPTJob_1670.Models;
using System.Security.Claims;
using NuGet.Protocol.Plugins;
using Microsoft.AspNetCore.Builder;
using FPTJob_1670.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Twilio.Rest.Api.V2010.Account;

namespace FPTJob_1670.Controllers
{
    public class ApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationHub> _notificationHub;

        private readonly UserManager<IdentityUser> _userManager;

        public ApplicationsController(ApplicationDbContext context, IHubContext<NotificationHub> notificationHub, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _notificationHub = notificationHub;
            _userManager = userManager;
        }

        // GET: Applications
        public async Task<IActionResult> Index()
        {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var applications = await _context.Application
                .Where(j => j.Seeker.SeekerId == currentUserId)
                .Include(a => a.Seeker) 
                .Include(a => a.Job) 
                .ToListAsync();
             return View(applications);
                    
        }

        // GET: Applications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Application == null)
            {
                return NotFound();
            }

            var application = await _context.Application
                .FirstOrDefaultAsync(m => m.Id == id);
            if (application == null)
            {
                return NotFound();
            }

            return View(application);
        }

        // GET: Applications/Create
        public IActionResult Create(int? jobId)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["Seeker_Id"] = new SelectList(_context.Seeker.Where(u => u.SeekerId == userId), "Id", "Name");
            ViewData["Job_Id"] = jobId.HasValue ? new SelectList(_context.Job.Where(j => j.Id == jobId), "Id", "Name", jobId) : new SelectList(_context.Job, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CoverLetter,ApplicationDate,Seeker_Id,Job_Id")] Application application)
        {
            if (ModelState.IsValid)
            {
                var existingApplication = await _context.Application.FirstOrDefaultAsync(a => a.Job_Id == application.Job_Id && a.Seeker_Id == application.Seeker_Id);
                if (existingApplication != null)
                {
                    ModelState.AddModelError(string.Empty, "You have already applied for this job.");
                    ViewData["Seeker_Id"] = new SelectList(_context.Seeker, "Id", "Name", application.Seeker_Id);
                    ViewData["Job_Id"] = new SelectList(_context.Job, "Id", "Name", application.Job_Id);
                    return View(application);
                }

                application.statusApplication = "Not Confirmed";
                _context.Add(application);
                await _context.SaveChangesAsync();


                int jobId = application.Job_Id;


                var job = await _context.Job.Include(j => j.Employer).FirstOrDefaultAsync(j => j.Id == jobId);


                if (job != null && job.Employer != null)
                {

                    string employerId = job.Employer.EmployerId;


                    await CreateNotification(employerId, "A new application has been received for your job");
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["Seeker_Id"] = new SelectList(_context.Seeker, "Id", "Name", application.Seeker_Id);
            ViewData["Job_Id"] = new SelectList(_context.Job, "Id", "Name", application.Job_Id);
            return View(application);
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
            var unreadCount = await _context.Notification.Where(n => n.UserId == userId && !n.IsRead).CountAsync();
            return unreadCount;
        }
        // GET: Applications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Application == null)
            {
                return NotFound();
            }

            var application = await _context.Application.FindAsync(id);
            if (application == null)
            {
                return NotFound();
            }
            return View(application);
        }

        // POST: Applications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CoverLetter,ApplicationDate,Seeker_Id,Job_Id")] Application application)
        {
            if (id != application.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(application);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationExists(application.Id))
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
            return View(application);
        }

        // GET: Applications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Application == null)
            {
                return NotFound();
            }

            var application = await _context.Application
                .FirstOrDefaultAsync(m => m.Id == id);
            if (application == null)
            {
                return NotFound();
            }

            return View(application);
        }

        // POST: Applications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Application == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Application'  is null.");
            }
            var application = await _context.Application.FindAsync(id);
            if (application != null)
            {
                _context.Application.Remove(application);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApplicationExists(int id)
        {
          return (_context.Application?.Any(e => e.Id == id)).GetValueOrDefault();
        }


       




        public async Task<IActionResult> AcceptApplication(int id)
        {
            var application = await _context.Application.FindAsync(id);
            if (application == null)
            {
                return NotFound();
            }
            // Update status to "Confirmed"
            application.statusApplication = "Accepted";
            _context.Application.Update(application);
            await _context.SaveChangesAsync();

            // Lấy seeker_id đang ứng tuyển
            int seekerId = application.Seeker_Id;

            var seeker=await _context.Seeker.FirstOrDefaultAsync(j => j.Id == seekerId);
            if (seeker != null)
            {
                // Lấy Employer_id của công việc
                string seekerNoti = seeker.SeekerId;

                // Gửi notification đến Employer
                await CreateNotification(seekerNoti, "Your application confimred");
            }
            return RedirectToAction(nameof(EmployerManage));
        }
        public async Task<IActionResult> RejectApplication(int id)
        {
            var application = await _context.Application.FindAsync(id);
            if (application == null)
            {
                return NotFound();
            }

            // Update status to "Confirmed"
            application.statusApplication = "Rejected";
            _context.Application.Update(application);
            await _context.SaveChangesAsync();

            // Lấy seeker_id đang ứng tuyển
            int seekerId = application.Seeker_Id;

            var seeker = await _context.Seeker.FirstOrDefaultAsync(j => j.Id == seekerId);
            if (seeker != null)
            {
                // Lấy Employer_id của công việc
                string seekerNoti = seeker.SeekerId;

                // Gửi notification đến Employer
                await CreateNotification(seekerNoti, "Your application Rejected");
            }

            return RedirectToAction(nameof(EmployerManage));
        }


        public async Task<IActionResult> EmployerManage()
        {

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);


                var applications = await _context.Application
                    .Where(a => a.Job.Employer.EmployerId == currentUserId)
                    .Include(a => a.Seeker)
                    .Include(a => a.Job)
                    .ToListAsync();

                return View("EmployerManage", applications);

        }



    }
}
