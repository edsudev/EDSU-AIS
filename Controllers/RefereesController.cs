using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EDSU_SYSTEM.Data;
using EDSU_SYSTEM.Models;

namespace EDSU_SYSTEM.Controllers
{
    public class RefereesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RefereesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Referees
        public async Task<IActionResult> Index()
        {
            //var applicationDbContext = _context.Vacancies.Include(v => v.Departments).Include(v => v.Faculties).Include(v => v.LGAs).Include(v => v.Nationalities).Include(v => v.Positions).Include(v => v.States);
            return View();
        }

        // GET: Referees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Vacancies == null)
            {
                return NotFound();
            }

            var vacancy = await _context.Vacancies
                .Include(v => v.Departments)
                .Include(v => v.Faculties)
                .Include(v => v.LGAs)
                .Include(v => v.Nationalities)
                .Include(v => v.Positions)
                .Include(v => v.States)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vacancy == null)
            {
                return NotFound();
            }

            return View(vacancy);
        }

        // GET: Referees/Create
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Id");
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "Id", "Id");
            ViewData["LGAId"] = new SelectList(_context.Lgas, "Id", "Id");
            ViewData["NationalityId"] = new SelectList(_context.Countries, "Id", "Id");
            ViewData["Position"] = new SelectList(_context.Positions, "Id", "Id");
            ViewData["StateId"] = new SelectList(_context.States, "Id", "Id");
            return View();
        }

        // POST: Referees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ApplicantId,Type,Position,FacultyId,DepartmentId,FirstName,LastName,OtherName,Email,Password,Religion,MaritalStatus,DOB,Sex,NationalityId,StateId,LGAId,Phone,ContactAddress,HighestQualification,FieldOfStudy,AreaOfSpecialization,WorkedInHigherInstuition,CurrentPlaceOfWork,PositionAtCurrentPlaceOfWork,YearsOfExperience,CertUpload,CVUpload,Picture,CreatedAt,UpdatedAt,IsEmployed")] Vacancy vacancy)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vacancy);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Id", vacancy.DepartmentId);
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "Id", "Id", vacancy.FacultyId);
            ViewData["LGAId"] = new SelectList(_context.Lgas, "Id", "Id", vacancy.LGAId);
            ViewData["NationalityId"] = new SelectList(_context.Countries, "Id", "Id", vacancy.NationalityId);
            ViewData["Position"] = new SelectList(_context.Positions, "Id", "Id", vacancy.Position);
            ViewData["StateId"] = new SelectList(_context.States, "Id", "Id", vacancy.StateId);
            return View(vacancy);
        }

        // GET: Referees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Vacancies == null)
            {
                return NotFound();
            }

            var vacancy = await _context.Vacancies.FindAsync(id);
            if (vacancy == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Id", vacancy.DepartmentId);
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "Id", "Id", vacancy.FacultyId);
            ViewData["LGAId"] = new SelectList(_context.Lgas, "Id", "Id", vacancy.LGAId);
            ViewData["NationalityId"] = new SelectList(_context.Countries, "Id", "Id", vacancy.NationalityId);
            ViewData["Position"] = new SelectList(_context.Positions, "Id", "Id", vacancy.Position);
            ViewData["StateId"] = new SelectList(_context.States, "Id", "Id", vacancy.StateId);
            return View(vacancy);
        }

        // POST: Referees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,ApplicantId,Type,Position,FacultyId,DepartmentId,FirstName,LastName,OtherName,Email,Password,Religion,MaritalStatus,DOB,Sex,NationalityId,StateId,LGAId,Phone,ContactAddress,HighestQualification,FieldOfStudy,AreaOfSpecialization,WorkedInHigherInstuition,CurrentPlaceOfWork,PositionAtCurrentPlaceOfWork,YearsOfExperience,CertUpload,CVUpload,Picture,CreatedAt,UpdatedAt,IsEmployed")] Vacancy vacancy)
        {
            if (id != vacancy.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vacancy);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VacancyExists(vacancy.Id))
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
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Id", vacancy.DepartmentId);
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "Id", "Id", vacancy.FacultyId);
            ViewData["LGAId"] = new SelectList(_context.Lgas, "Id", "Id", vacancy.LGAId);
            ViewData["NationalityId"] = new SelectList(_context.Countries, "Id", "Id", vacancy.NationalityId);
            ViewData["Position"] = new SelectList(_context.Positions, "Id", "Id", vacancy.Position);
            ViewData["StateId"] = new SelectList(_context.States, "Id", "Id", vacancy.StateId);
            return View(vacancy);
        }

        // GET: Referees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Vacancies == null)
            {
                return NotFound();
            }

            var vacancy = await _context.Vacancies
                .Include(v => v.Departments)
                .Include(v => v.Faculties)
                .Include(v => v.LGAs)
                .Include(v => v.Nationalities)
                .Include(v => v.Positions)
                .Include(v => v.States)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vacancy == null)
            {
                return NotFound();
            }

            return View(vacancy);
        }

        // POST: Referees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (_context.Vacancies == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Vacancies'  is null.");
            }
            var vacancy = await _context.Vacancies.FindAsync(id);
            if (vacancy != null)
            {
                _context.Vacancies.Remove(vacancy);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VacancyExists(int? id)
        {
          return (_context.Vacancies?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
