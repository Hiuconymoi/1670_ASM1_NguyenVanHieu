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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace FPTJob_1670.Controllers
{
    public class EmployersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EmployersController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [Authorize(Roles = "Employer")]
        // GET: Employers
        public async Task<IActionResult> Index()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var employer = await _context.Employer
                .Where(j => j.EmployerId == currentUserId)
                .Include(a => a.User_Id) // Bao gồm thông tin 
                .ToListAsync();

            return View(employer);
        }

        // GET: Employers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Employer == null)
            {
                return NotFound();
            }

            var employer = await _context.Employer
                .Include(e => e.User_Id)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employer == null)
            {
                return NotFound();
            }

            return View(employer);
        }

        // GET: Employers/Create
        public IActionResult Create()
        {

            // Lấy Id của người dùng đang đăng nhập
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Tạo SelectList chỉ chứa mục với EmployerId bằng với Id của người dùng đang đăng nhập
            ViewData["EmployerId"] = new SelectList(_context.Users.Where(u => u.Id == userId), "Id", "Email");
            return View();
        }

        // POST: Employers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EmployerId,Name,CompanyName,Email,Phone")] Employer employer)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem người dùng đã có nhà tuyển dụng chưa
                var existingEmployer = await _context.Employer.FirstOrDefaultAsync(e => e.EmployerId == employer.EmployerId);

                // Nếu người dùng đã có nhà tuyển dụng, chuyển hướng đến trang chỉnh sửa
                if (existingEmployer != null)
                {
         
                    return RedirectToAction("Edit", new { id = existingEmployer.Id });
                }

                _context.Add(employer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployerId"] = new SelectList(_context.Users, "Id", "Email", employer.EmployerId);
            return View(employer);
        }

        // GET: Employers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Employer == null)
            {
                return NotFound();
            }

            var employer = await _context.Employer.FindAsync(id);
            if (employer == null)
            {
                return NotFound();
            }
            ViewData["EmployerId"] = new SelectList(_context.Users, "Id", "Email", employer.EmployerId);
            return View(employer);
        }

        // POST: Employers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmployerId,Name,CompanyName,Email,Phone")] Employer employer)
        {
            if (id != employer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployerExists(employer.Id))
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
            ViewData["EmployerId"] = new SelectList(_context.Users, "Id", "Email", employer.EmployerId);
            return View(employer);
        }

        // GET: Employers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Employer == null)
            {
                return NotFound();
            }

            var employer = await _context.Employer
                .Include(e => e.User_Id)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employer == null)
            {
                return NotFound();
            }

            return View(employer);
        }

        // POST: Employers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Employer == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Employer'  is null.");
            }
            var employer = await _context.Employer.FindAsync(id);
            if (employer != null)
            {
                _context.Employer.Remove(employer);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployerExists(int id)
        {
          return (_context.Employer?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
