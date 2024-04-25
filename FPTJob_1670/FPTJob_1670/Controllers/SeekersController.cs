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

namespace FPTJob_1670.Controllers
{
    public class SeekersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SeekersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Seeker")]
        // GET: Seekers
        public async Task<IActionResult> Index()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var seecker = await _context.Seeker
                .Where(j => j.SeekerId == currentUserId)
                .Include(a => a.User_ID)
                .ToListAsync();

            return View(seecker);
        }

        // GET: Seekers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Seeker == null)
            {
                return NotFound();
            }

            var seeker = await _context.Seeker
                .Include(s => s.User_ID)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (seeker == null)
            {
                return NotFound();
            }

            return View(seeker);
        }

        // GET: Seekers/Create
        public IActionResult Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["SeekerId"] = new SelectList(_context.Users.Where(u => u.Id == userId), "Id", "Email");
            return View();
        }

        // POST: Seekers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SeekerId,Name,Age,Gender")] Seeker seeker)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (seeker.SeekerId != userId)
                {
                    return RedirectToAction(nameof(Index));
                }
                _context.Add(seeker);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SeekerId"] = new SelectList(_context.Users, "Id", "Email", seeker.SeekerId);
            return View(seeker);
        }

        // GET: Seekers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Seeker == null)
            {
                return NotFound();
            }

            var seeker = await _context.Seeker.FindAsync(id);
            if (seeker == null)
            {
                return NotFound();
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["SeekerId"] = new SelectList(_context.Users.Where(u => u.Id == userId), "Id", "Email");
            return View(seeker);
        }

        // POST: Seekers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SeekerId,Name,Age,Gender")] Seeker seeker)
        {
            if (id != seeker.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(seeker);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SeekerExists(seeker.Id))
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
            ViewData["SeekerId"] = new SelectList(_context.Users, "Id", "Email", seeker.SeekerId);
            return View(seeker);
        }

        // GET: Seekers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Seeker == null)
            {
                return NotFound();
            }

            var seeker = await _context.Seeker
                .Include(s => s.User_ID)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (seeker == null)
            {
                return NotFound();
            }

            return View(seeker);
        }

        // POST: Seekers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Seeker == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Seeker'  is null.");
            }
            var seeker = await _context.Seeker.FindAsync(id);
            if (seeker != null)
            {
                _context.Seeker.Remove(seeker);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SeekerExists(int id)
        {
          return (_context.Seeker?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
