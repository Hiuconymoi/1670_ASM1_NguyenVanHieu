using FPTJob_1670.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FPTJob_1670.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public NotificationsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var notifications = await _context.Notification.Where(n => n.UserId == currentUserId).ToListAsync();
            ViewBag.UnreadNotificationCount = await GetUnreadNotificationCount();
            return View(notifications);
        }

        public async Task<int> GetUnreadNotificationCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var unreadCount = await _context.Notification.Where(n => n.UserId == userId && !n.IsRead).CountAsync();
            return unreadCount;
        }


        // GET: Notifications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Notification == null)
            {
                return NotFound();
            }

            var job = await _context.Notification
                .FirstOrDefaultAsync(m => m.Id == id);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var notification = await _context.Notification.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }

            _context.Notification.Remove(notification);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
