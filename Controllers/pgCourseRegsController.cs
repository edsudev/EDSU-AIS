using System;
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
using static EDSU_SYSTEM.Models.Enum;

namespace EDSU_SYSTEM.Controllers
{
    public class pgCourseRegsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public pgCourseRegsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: pgCourseRegs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.PgCourseRegs == null)
            {
                return NotFound();
            }

            var pgCourseReg = await _context.PgCourseRegs
                .Include(p => p.Courses)
                .Include(p => p.Sessions)
                .Include(p => p.Students)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pgCourseReg == null)
            {
                return NotFound();
            }

            return View(pgCourseReg);
        }

        // GET: pgCourseRegs/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.PgCourses, "Id", "Id");
            ViewData["SessionId"] = new SelectList(_context.Sessions, "Id", "Id");
            ViewData["StudentId"] = new SelectList(_context.PostGraduateStudents, "Id", "Id");
            return View();
        }

        // POST: pgCourseRegs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CourseRegId,CourseId,StudentId,Comment,SessionId,Status,CreatedAt,DateApproved")] PgCourseReg pgCourseReg)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pgCourseReg);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.PgCourses, "Id", "Id", pgCourseReg.CourseId);
            ViewData["SessionId"] = new SelectList(_context.Sessions, "Id", "Id", pgCourseReg.SessionId);
            ViewData["StudentId"] = new SelectList(_context.PostGraduateStudents, "Id", "Id", pgCourseReg.StudentId);
            return View(pgCourseReg);
        }

        // GET: pgCourseRegs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.PgCourseRegs == null)
            {
                return NotFound();
            }

            var pgCourseReg = await _context.PgCourseRegs.FindAsync(id);
            if (pgCourseReg == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.PgCourses, "Id", "Id", pgCourseReg.CourseId);
            ViewData["SessionId"] = new SelectList(_context.Sessions, "Id", "Id", pgCourseReg.SessionId);
            ViewData["StudentId"] = new SelectList(_context.PostGraduateStudents, "Id", "Id", pgCourseReg.StudentId);
            return View(pgCourseReg);
        }

        // POST: pgCourseRegs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,CourseRegId,CourseId,StudentId,Comment,SessionId,Status,CreatedAt,DateApproved")] PgCourseReg pgCourseReg)
        {
            if (id != pgCourseReg.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pgCourseReg);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PgCourseRegExists(pgCourseReg.Id))
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
            ViewData["CourseId"] = new SelectList(_context.PgCourses, "Id", "Id", pgCourseReg.CourseId);
            ViewData["SessionId"] = new SelectList(_context.Sessions, "Id", "Id", pgCourseReg.SessionId);
            ViewData["StudentId"] = new SelectList(_context.PostGraduateStudents, "Id", "Id", pgCourseReg.StudentId);
            return View(pgCourseReg);
        }

        /// <summary>
        /// REGISTRATION
        /// </summary>
        /// 
        /// <returns></returns>
        [Authorize(Roles = "pgStudent, superAdmin")]
        // GET: courseRegistrations/Create
        public async Task<IActionResult> First()
        {
            try
            {
                var userId = await _userManager.GetUserAsync(HttpContext.User);
                var student = (from c in _context.PostGraduateStudents where c.Id == userId.PgStudent select c).FirstOrDefault();
                var courses = (from d in _context.PgCourses where d.DepartmentId == student.Department && d.Semester == 1 select d)
                    .Include(c => c.Departments).Include(c => c.Semesters).Include(c => c.Levels);

                //Getting Credit unit Info
                var creditunit = (from c in _context.CreditUnits where c.DepartmentId == student.Department && c.LevelId == student.Level && c.SemesterId == 1 select c).FirstOrDefault();
                ViewBag.max = creditunit.Max;

                var NoOfCoursesRegInFirstSemester = (from c in _context.PgCourseRegs where c.StudentId == userId.StudentsId && c.Courses.Semester == 1 select c).Include(c => c.Courses).ToList();
                ViewBag.No = NoOfCoursesRegInFirstSemester.Count();

                var TotalCredit = (from c in NoOfCoursesRegInFirstSemester select c.Courses.CreditUnit).ToList();
                ViewBag.sum = TotalCredit.Sum();

                ViewBag.sumLeft = ViewBag.max - ViewBag.sum;
                TempData["sum"] = ViewBag.sumLeft;

                string errorMessage = (string)TempData["error"];
                string errorMessage2 = (string)TempData["maxcredit"];
                ViewBag.ErrorMessage = errorMessage;
                ViewBag.ErrorMessage2 = errorMessage2;
                return View(await courses.ToListAsync());

            }
            catch (Exception)
            {
                TempData["err"] = "Kindly ask your HOD to set maximum credit unit for your program";
                return RedirectToAction("badreq", "error");
                throw;
            }

        }
        [Authorize(Roles = "pgStudent, superAdmin")]
        public async Task<IActionResult> Second()
        {
            try
            {
                var userId = await _userManager.GetUserAsync(HttpContext.User);
                var student = (from c in _context.PostGraduateStudents where c.Id == userId.PgStudent select c).FirstOrDefault();
                var courses = (from d in _context.PgCourses where d.DepartmentId == student.Department && d.Semester == 2 select d)
                    .Include(c => c.Departments).Include(c => c.Semesters).Include(c => c.Levels);

                //Getting Credit unit Info
                var creditunit = (from c in _context.CreditUnits where c.DepartmentId == student.Department && c.LevelId == student.Level && c.SemesterId == 2 select c).FirstOrDefault();
                ViewBag.max = creditunit.Max;

                var NoOfCoursesRegInSecondSemester = (from c in _context.PgCourseRegs where c.StudentId == userId.StudentsId && c.Courses.Semester == 2 select c).Include(c => c.Courses).ToList();
                ViewBag.No = NoOfCoursesRegInSecondSemester.Count();

                var TotalCredit = (from c in NoOfCoursesRegInSecondSemester select c.Courses.CreditUnit).ToList();
                ViewBag.sum = TotalCredit.Sum();

                ViewBag.sumLeft = ViewBag.max - ViewBag.sum;
                TempData["sum"] = ViewBag.sumLeft;

                string errorMessage = (string)TempData["error"];
                string errorMessage2 = (string)TempData["maxcredit"];
                ViewBag.ErrorMessage = errorMessage;
                ViewBag.ErrorMessage2 = errorMessage2;
                return View(await courses.ToListAsync());
            }
            catch (Exception e)
            {
                TempData["err"] = e.Message;
                return RedirectToAction("badreq", "Error");

                throw;
            }


        }


        [Authorize(Roles = "pgStudent, superAdmin")]
        public async Task<IActionResult> AddCourse(string id, PgCourseReg courseRegistration)
        {
            try
            {
                var userId = await _userManager.GetUserAsync(HttpContext.User);
                var student = (from s in _context.PostGraduateStudents where s.Id == userId.PgStudent select s).FirstOrDefault();
                if (id == null || _context.PgCourseRegs == null)
                {
                    return NotFound();
                }

                var course = await _context.PgCourses
                    .FirstOrDefaultAsync(m => m.Code == id && m.DepartmentId == student.Department);
                if (course == null)
                {
                    return NotFound();
                }
                var session = (from c in _context.Sessions where c.IsActive == true select c.Id).FirstOrDefault();
                courseRegistration.CourseId = course.Id;
                courseRegistration.StudentId = userId.StudentsId;
                courseRegistration.SessionId = session;
                courseRegistration.Status = MainStatus.Pending;
                courseRegistration.CreatedAt = DateTime.Now;
                _context.PgCourseRegs.Add(courseRegistration);
                ViewBag.courseunit = courseRegistration.Courses.CreditUnit;
                if (ViewBag.courseunit > (int)TempData["sum"])
                {
                    TempData["maxcredit"] = "There was an attempt to exceed the maximum credit load";
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                await _context.SaveChangesAsync();
                return Redirect(Request.Headers["Referer"].ToString());
            }
            catch (Exception ex)
            {
                TempData["error"] = "The course you clicked has been registered!";
                return Redirect(Request.Headers["Referer"].ToString());
                //throw;
            }

        }
        [Authorize(Roles = "pgStudent, levelAdviser, superAdmin")]
        // GET: courseRegistrations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PgCourseRegs == null)
            {
                return NotFound();
            }

            var courseRegistration = await _context.PgCourseRegs
                .Include(c => c.Courses)

                .FirstOrDefaultAsync(m => m.Id == id);
            if (courseRegistration == null)
            {
                return NotFound();
            }

            return PartialView("_delete", courseRegistration);
        }
        [Authorize(Roles = "pgStudent, levelAdviser, superAdmin")]
        // POST: courseRegistrations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CourseRegistrations == null)
            {
                return Problem("Entity set 'ApplicationDbContext.CourseRegistrations'  is null.");
            }
            var courseRegistration = await _context.PgCourseRegs.FindAsync(id);
            if (courseRegistration != null)
            {
                _context.PgCourseRegs.Remove(courseRegistration);
            }

            await _context.SaveChangesAsync();
            var previousPage = Request.Headers["Referer"].ToString();
            return Redirect(previousPage);
        }
        public async Task<IActionResult> Index()
        {
            var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);
            var ugStudent = loggedInUser.PgStudent;
            
                var courses = (from c in _context.PgCourseRegs where c.StudentId == ugStudent select c).Include(c => c.Courses).ThenInclude(c => c.Semesters).Include(c => c.Students);
                ViewData["student"] = ugStudent;
                return View(await courses.ToListAsync());
            

        }
        [Authorize(Roles = "pgStudent, superAdmin")]
        public async Task<IActionResult> History()
        {

            var sessions = (from c in _context.Sessions select c);

            return View(await sessions.ToListAsync());
        }
        [Authorize(Roles = "levelAdviser")]
        // GET: My students
        public async Task<IActionResult> Pending()
        {
            var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);
            var id = loggedInUser.StaffId;
            var staff = (from c in _context.Staffs where c.Id == id select c.DepartmentId).FirstOrDefault();
            var AdviserLevel = (from l in _context.LevelAdvisers where l.StaffId == id select l).ToList();
            var Levelstudents = new List<PgStudent>();
            foreach (var item in AdviserLevel)
            {
                var students = (from c in _context.PostGraduateStudents
                                where c
                                .Department == staff &&
                                c.Level == item.LevelId
                                && _context.PgCourseRegs.Any(cr => cr.StudentId == c.Id && cr.Status == MainStatus.Pending)
                                select c).ToList();
                Levelstudents.AddRange(students);
            }
            return View(Levelstudents);
        }
        [Authorize(Roles = "levelAdviser")]
        public async Task<IActionResult> Approved()
        {
            ViewBag.success = TempData["success"];
            var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);
            var id = loggedInUser.StaffId;
            var staff = (from c in _context.Staffs where c.Id == id select c.DepartmentId).FirstOrDefault();
            var AdviserLevel = (from l in _context.LevelAdvisers where l.StaffId == id select l).ToList();
            var Levelstudents = new List<PgStudent>();
            foreach (var item in AdviserLevel)
            {
                var students = (from c in _context.PostGraduateStudents
                                where c.Department == staff && c.Level == item.LevelId &&
                                _context.PgCourseRegs.Any(cr => cr.StudentId == c.Id && cr.Status == MainStatus.Approved)
                                select c).ToList();
                Levelstudents.AddRange(students);
            }
            return View(Levelstudents);

        }
        [Authorize(Roles = "pgStudent")]
        public async Task<IActionResult> Summary(string id)
        {
            try
            {
                var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);
                var studentId = loggedInUser.PgStudent;
                var student = (from s in _context.PostGraduateStudents where s.Id == studentId select s)
                    .Include(c => c.Departments).Include(c => c.Levels).Include(c => c.Sessions).FirstOrDefault();
                ViewBag.name = student.Fullname;
                ViewBag.email = student.SchoolEmailAddress;
                ViewBag.mat = student.MatNumber;
                ViewBag.dept = student.Departments.Name;
                ViewBag.session = student.Sessions.Name;
                //ViewBag.program = student.Programs.NameOfProgram;
                ViewBag.level = student.Levels.Name;
                var approvedCourses = (from c in _context.PgCourseRegs
                                       where c.StudentId == studentId &&
                                        c.Status == MainStatus.Approved &&
                                        c.Sessions.Name == id
                                       select c).Include(c => c.Courses).Include(c => c.Students).ToList();

                var creditunit = (from c in _context.CreditUnits where c.DepartmentId == student.Department && c.LevelId == student.Level && c.Sessions.Name == id select c).FirstOrDefault();
                ViewBag.min = creditunit.Min;
                ViewBag.max = creditunit.Max;
                TempData["max"] = creditunit.Max;
                TempData["min"] = creditunit.Min;

                var TotalCredit = (from c in approvedCourses select c.Courses.CreditUnit).ToList();
                ViewBag.sum = TotalCredit.Sum();


                if (approvedCourses.Count() == 0)
                {
                    return RedirectToAction("badreq", "error");
                }
                else
                {
                    return View("Summary", approvedCourses);
                }

            }
            catch (Exception)
            {
                return RedirectToAction("nocourses", "error");
                throw;
            }

        }
        public IActionResult Pendingreg(string id)
        {
            TempData["studentEmail"] = id;
            var student = _context.PostGraduateStudents.Where(x => x.SchoolEmailAddress == id).Select(x => x.Id).FirstOrDefault();
            var studentCourses = (from c in _context.PgCourseRegs where c.StudentId == student && c.Status == MainStatus.Pending select c).Include(c => c.Courses).ThenInclude(c => c.Semesters).Include(c => c.Students).ToList();
            //foreach (var item in studentCourses)
            //{
            //    var s = item.Courses.CreditUnit;
            //}

            return View(studentCourses);
        }
        public IActionResult Approvedreg(int id)
        {
            ViewBag.StudentId = id;
            var students = (from c in _context.PgCourseRegs where c.StudentId == id && c.Status == MainStatus.Approved select c).Include(c => c.Courses).ThenInclude(i => i.Semesters).Include(c => c.Students).ToList();
            return View(students);
        }
        public async Task<IActionResult> RejectCourse(int? id)
        {
            var code = (from s in _context.PgCourseRegs where s.Id == id select s.Students.SchoolEmailAddress).FirstOrDefault();
            if (id == null || _context.PgCourseRegs == null)
            {
                return NotFound();
            }
            var courseRegistration = (from s in _context.PgCourseRegs where s.Id == id select s).Include(i => i.Students).Include(c => c.Courses).FirstOrDefault();

            if (courseRegistration == null)
            {
                return NotFound();
            }
            return PartialView("_reject", courseRegistration);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int? id)
        {
            var coursereg = await _context.PgCourseRegs.FindAsync(id);
            var course = (from s in _context.PgCourseRegs where s.Id == id select s).Include(i => i.Students).Include(c => c.Courses).FirstOrDefault();

            //await _context.CourseRegistrations.FirstOrDefaultAsync(i => i.Courses.Code == code && i.Students.SchoolEmailAddress == student);
            if (id == null)
            {
                return NotFound();
            }
            if (await TryUpdateModelAsync<PgCourseReg>(coursereg, "", c => c.Comment))
            {
                try
                {
                    course.Status = MainStatus.Declined;
                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {

                        return RedirectToAction("badreq", "error");
                    }

                }
                catch (Exception ex)
                {
                    ex.ToString();

                }
            }
            //var iid = courseRegistration.Students.SchoolEmailAddress;
            return RedirectToAction("pending", "pgcourseregs");
        }
        [Authorize(Roles = "levelAdviser")]
        public async Task<IActionResult> Approve(string code, string student)
        {

            var course = await _context.PgCourseRegs.FirstOrDefaultAsync(i => i.Courses.Code == code && i.Students.SchoolEmailAddress == student);
            if (code == null)
            {
                return NotFound();
            }
            try
            {

                course.Status = MainStatus.Approved;
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {

                    return RedirectToAction("badreq", "error");
                }
                //return RedirectToAction("pendingreg", "courseregistrations", new { id });

            }
            catch (Exception ex)
            {
                ex.ToString();

            }
            var id = (string)TempData["studentEmail"];
            return RedirectToAction("pendingreg", "pgcourseregs", new { id });
        }
        private bool PgCourseRegExists(int? id)
        {
          return (_context.PgCourseRegs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
