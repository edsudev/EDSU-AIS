﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EDSU_SYSTEM.Data;
using EDSU_SYSTEM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace EDSU_SYSTEM.Controllers
{
    [Authorize(Roles = "hod, superAdmin")]
    public class courseAllocationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public courseAllocationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        // GET: courseAllocations
        public async Task<IActionResult> Index()
        {
            var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);
            var user = loggedInUser.StaffId;
            var staff = (from x in _context.Staffs where x.Id == user select x).FirstOrDefault();
            var applicationDbContext = _context.CourseAllocations.Where(x => x.Courses.DepartmentId == staff.DepartmentId).Include(c => c.Courses).Include(c => c.Staff).ToList();
            return View(applicationDbContext);
        }

        // GET: courseAllocations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CourseAllocations == null)
            {
                return NotFound();
            }

            var courseAllocation = await _context.CourseAllocations
                .Include(c => c.Courses)
                .Include(c => c.Staff)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (courseAllocation == null)
            {
                return NotFound();
            }

            return View(courseAllocation);
        }

        // GET: courseAllocations/Create
        public async Task<IActionResult> Create()
        {
            var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);
            var user = loggedInUser.StaffId;
            var staff = (from x in _context.Staffs where x.Id == user select x).FirstOrDefault();
            ViewData["CourseId"] = new SelectList(_context.Courses.Where(x => x.DepartmentId == staff.DepartmentId), "Id", "Code");
            ViewData["LecturerId"] = new SelectList(_context.Staffs.Select(s => new { Id = s.Id, Fullname = $" {s.Surname} {s.FirstName} {s.MiddleName}" }), "Id", "Fullname");

            return View();
        }

        // POST: courseAllocations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseAllocation courseAllocation)
        {
            
            courseAllocation.CreatedAt = DateTime.Now;
            _context.Add(courseAllocation);
            await _context.SaveChangesAsync();
            
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id", courseAllocation.CourseId);
            ViewData["LecturerId"] = new SelectList(_context.Staffs, "Id", "Id", courseAllocation.LecturerId);
            return RedirectToAction(nameof(Index));
        }

        // GET: courseAllocations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CourseAllocations == null)
            {
                return NotFound();
            }

            var courseAllocation = await _context.CourseAllocations.FindAsync(id);
            if (courseAllocation == null)
            {
                return NotFound();
            }
            var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);
            var user = loggedInUser.StaffId;
            var staff = (from x in _context.Staffs where x.Id == user select x).FirstOrDefault();
            ViewData["CourseId"] = new SelectList(_context.Courses.Where(x => x.DepartmentId == staff.DepartmentId), "Id", "Code");
            ViewData["LecturerId"] = new SelectList(_context.Staffs, "Id", "SchoolEmail", courseAllocation.LecturerId);
            return View(courseAllocation);
        }

        // POST: courseAllocations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,CourseAllocation courseAllocation)
        {
            if (id != courseAllocation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    courseAllocation.UpdatedAt = DateTime.Now;
                    _context.Update(courseAllocation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseAllocationExists(courseAllocation.Id))
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
            return View(courseAllocation);
        }

        // GET: courseAllocations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CourseAllocations == null)
            {
                return NotFound();
            }

            var courseAllocation = await _context.CourseAllocations
                .Include(c => c.Courses)
                .Include(c => c.Staff)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (courseAllocation == null)
            {
                return NotFound();
            }
            return PartialView("_delete",courseAllocation);
        }

        // POST: courseAllocations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CourseAllocations == null)
            {
                return Problem("Entity set 'ApplicationDbContext.CourseAllocations'  is null.");
            }
            var courseAllocation = await _context.CourseAllocations.FindAsync(id);
            if (courseAllocation != null)
            {
                _context.CourseAllocations.Remove(courseAllocation);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseAllocationExists(int? id)
        {
          return _context.CourseAllocations.Any(e => e.Id == id);
        }
    }
}
