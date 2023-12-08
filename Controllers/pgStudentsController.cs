using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EDSU_SYSTEM.Data;
using EDSU_SYSTEM.Models;
using Microsoft.AspNetCore.Identity;
using static EDSU_SYSTEM.Models.Enum;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace EDSU_SYSTEM.Controllers
{
    public class pgStudentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UserManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public pgStudentsController(UserManager<ApplicationUser> userManager, UserManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IWebHostEnvironment hostingEnvironment, ApplicationDbContext context)
        {
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }
        public async Task<IActionResult> MigrateStudent(string email)
        {
            try
            {
                var student = (from s in _context.PostGraduateStudents where s.SchoolEmailAddress == email select s).FirstOrDefault();

                var user = new ApplicationUser
                {
                    Email = student.SchoolEmailAddress,
                    UserName = student.SchoolEmailAddress,
                    StudentsId = student.Id,
                    PhoneNumber = student.Phone,
                    PhoneNumberConfirmed = true,
                    Type = 3,
                    EmailConfirmed = true
                };
                var r = await _userManager.CreateAsync(user, student.Phone);
            }
            catch (Exception)
            {

                throw;
            }


            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> StudentsRole()
        {
            try
            {
                var id = "32a6763e-6855-433f-9254-eb0fa5b57a50";
                var users = _userManager.Users.Where(x => x.Type == 3).ToList();
                var role = await _roleManager.FindByIdAsync(id);
                foreach (var item in users)
                {
                    await _userManager.AddToRoleAsync(item, role.Name);
                }
            }
            catch (Exception)
            {

                throw;
            }
            return View();
        }
        public async Task<IActionResult> PopulateWallet(PgSubWallet subWallet, PgMainWallet pgmain)
        {
            var students = (from s in _context.PostGraduateStudents select s).ToList();

            foreach (var st in students)
            {
                try
                {
                    var newPgMain = new PgMainWallet();

                    //ugmain.Id = int.Parse(p);
                    newPgMain.Name = st.Fullname;
                    newPgMain.WalletId = st.Phone;
                    newPgMain.BulkDebitBalanace = 0;
                    newPgMain.CreditBalance = 0;
                    newPgMain.Status = true;
                    newPgMain.DateCreated = DateTime.Now;
                    _context.PgMainWallets.Add(newPgMain);
                    await _context.SaveChangesAsync();

                    var tuition = (from tu in _context.Fees where tu.DepartmentId == st.Department select tu).FirstOrDefault();
                    if (tuition == null)
                    {
                        tuition = new Fee { Level1 = 0 };
                    }
                    Random r = new();
                    // string a = st.Id.ToString() + r.Next(10000);

                    // Create a new instance of UgSubWallet for each student
                    var newSubWallet = new PgSubWallet();
                    newSubWallet.WalletId = st.Phone;
                    newSubWallet.Name = st.Fullname;
                    newSubWallet.RegNo = st.Phone;
                    newSubWallet.CreditBalance = 0;
                    newSubWallet.Status = true;
                    newSubWallet.DateCreated = DateTime.Now;
                    newSubWallet.Tuition = 300000;
                    newSubWallet.FortyPercent = newSubWallet.Tuition * 40 / 100;
                    newSubWallet.SixtyPercent = newSubWallet.Tuition * 60 / 100;
                    newSubWallet.LMS = 50000;
                    //newSubWallet.AcceptanceFee = 100000;
                    //newSubWallet.SRC = 2000;
                   //newSubWallet.EDHIS = 25000;
                    newSubWallet.SessionId = 9;
                    newSubWallet.Debit = newSubWallet.Tuition + newSubWallet.LMS
                                        + newSubWallet.EDHIS + newSubWallet.SRC + newSubWallet.AcceptanceFee;
                    newSubWallet.Level = st.Level;
                    newSubWallet.Department = st.Department;

                    _context.PgSubWallets.Add(newSubWallet);
                    await _context.SaveChangesAsync();

                    var main = (from m in _context.PgMainWallets where m.WalletId == newSubWallet.WalletId select m).FirstOrDefault();
                    main.BulkDebitBalanace = newSubWallet.Debit;
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    // Handle exceptions appropriately
                    throw;
                }
            }

            return View();
        }

        // GET: pgStudents
        public async Task<IActionResult> Index()
        {
            var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);
            var userId = loggedInUser.PgStudent;
            var student = (from c in _context.PostGraduateStudents where c.Id == userId select c).Include(i => i.Departments).FirstOrDefault();
            var pgwallet = (from c in _context.PgMainWallets where c.WalletId == student.StudentId select c).FirstOrDefault();
            ViewBag.Name = student.Fullname;
            ViewBag.Department = student.Departments.Name;
            var approvedCourses = (from c in _context.PgCourseRegs
                                   where c.StudentId == userId &&
                            c.Status == MainStatus.Approved &&
                            c.SessionId == student.CurrentSession
                                   select c).Include(c => c.Courses).ToList();
            //var timetable = (from c in _context.TimeTables
            //                 where c.DepartmetId == student.Department && c.LevelId ==
            //                 student.Level
            //                 select c).Include(c => c.Courses).ThenInclude(s => s.Courses).ToList();

            //After getting the courses from coursereg and Scores from Results table, we sorted them using the course code before serializing them
            //so that the courses can align with the courses since they are coming from the same table.
            var grades = (from g in _context.Results where g.StudentId == student.MatNumber select g).ToList();
            var sortedCourses = approvedCourses.OrderBy(s => s.Courses.Code);
            var sortedGrades = grades.OrderBy(c => c.CourseId);

            var CourseCode = (from c in sortedCourses select c.Courses.Code).ToList();
            var TestScores = (from v in sortedGrades select v.CA).ToList();


            var json = JsonSerializer.Serialize(CourseCode);
            var json2 = JsonSerializer.Serialize(TestScores);


            ViewBag.courses = json;
            ViewBag.grade = json2;

            var model = new PGStudentDashboardVM
            {
                MainWallet = pgwallet,
                Courses = approvedCourses,
                //TimeTables = timetable
            };

            return View(model);

        }
        public async Task<IActionResult> Allstudents()
        {
            var applicationDbContext = _context.PostGraduateStudents.Include(c => c.Applicants).Include(c => c.Departments).Include(c => c.Faculties).Include(c => c.LGAs).Include(c => c.Levels).Include(c => c.Nationalities).Include(c => c.Sessions).Include(c => c.Staffs).Include(c => c.States).Include(c => c.YearOfAdmissions);
            return View(await applicationDbContext.ToListAsync());
        }
        public async Task<IActionResult> Graduated()
        {
            var applicationDbContext = _context.PostGraduateStudents.Where(x => x.StudentStatus == 2).Include(c => c.Applicants).Include(c => c.Departments).Include(c => c.Faculties).Include(c => c.LGAs).Include(c => c.Levels).Include(c => c.Nationalities).Include(c => c.Sessions).Include(c => c.Staffs).Include(c => c.States).Include(c => c.YearOfAdmissions);
            return View(await applicationDbContext.ToListAsync());
        }
        public async Task<IActionResult> Expelled()
        {
            var applicationDbContext = _context.PostGraduateStudents.Where(x => x.StudentStatus == 3).Include(c => c.Applicants).Include(c => c.Departments).Include(c => c.Faculties).Include(c => c.LGAs).Include(c => c.Levels).Include(c => c.Nationalities).Include(c => c.Sessions).Include(c => c.Staffs).Include(c => c.States).Include(c => c.YearOfAdmissions);
            return View(await applicationDbContext.ToListAsync());
        }


        // GET: pgStudents/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null || _context.PostGraduateStudents == null)
            {
                return NotFound();
            }

            var pgStudent = await _context.PostGraduateStudents
                .Include(p => p.Departments)
                .Include(p => p.Faculties)
                .Include(p => p.LGAs)
                .Include(p => p.Levels)
                .Include(p => p.Nationalities)
                .Include(p => p.Sessions)
                .Include(p => p.Staffs)
                .Include(p => p.States)
                .FirstOrDefaultAsync(m => m.SchoolEmailAddress == id);
            if (pgStudent == null)
            {
                return NotFound();
            }

            return View(pgStudent);
        }

        // GET: pgStudents/Create
        public IActionResult Create()
        {
            ViewData["Department"] = new SelectList(_context.Departments, "Id", "Id");
            ViewData["Faculty"] = new SelectList(_context.Faculties, "Id", "Id");
            ViewData["LGAId"] = new SelectList(_context.Lgas, "Id", "Id");
            ViewData["Level"] = new SelectList(_context.Levels, "Id", "Id");
            ViewData["NationalityId"] = new SelectList(_context.Countries, "Id", "Id");
            ViewData["CurrentSession"] = new SelectList(_context.Sessions, "Id", "Id");
            ViewData["ClearedBy"] = new SelectList(_context.Staffs, "Id", "Id");
            ViewData["StateOfOriginId"] = new SelectList(_context.States, "Id", "Id");
            return View();
        }

        // POST: pgStudents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentId,Picture,Fullname,Sex,DOB,Religion,Phone,AltPhoneNumber,Email,NationalityId,StateOfOriginId,LGAId,PlaceOfBirth,PermanentHomeAddress,ContactAddress,MaritalStatus,NextOfkinName,NextOfKinRelationship,NextOfKinPhone,NextOfKinAddress,SchoolEmailAddress,MatNumber,Faculty,Level,ModeOfAdmission,YearOfAdmission,Department,CurrentSession,CreatedAt,Cleared,ClearedBy")] PgStudent pgStudent)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pgStudent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Department"] = new SelectList(_context.Departments, "Id", "Id", pgStudent.Department);
            ViewData["Faculty"] = new SelectList(_context.Faculties, "Id", "Id", pgStudent.Faculty);
            ViewData["LGAId"] = new SelectList(_context.Lgas, "Id", "Id", pgStudent.LGAId);
            ViewData["Level"] = new SelectList(_context.Levels, "Id", "Id", pgStudent.Level);
            ViewData["NationalityId"] = new SelectList(_context.Countries, "Id", "Id", pgStudent.NationalityId);
            ViewData["CurrentSession"] = new SelectList(_context.Sessions, "Id", "Id", pgStudent.CurrentSession);
            ViewData["ClearedBy"] = new SelectList(_context.Staffs, "Id", "Id", pgStudent.ClearedBy);
            ViewData["StateOfOriginId"] = new SelectList(_context.States, "Id", "Id", pgStudent.StateOfOriginId);
            return View(pgStudent);
        }

        // GET: pgStudents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.PostGraduateStudents == null)
            {
                return NotFound();
            }

            var pgStudent = await _context.PostGraduateStudents.FindAsync(id);
            if (pgStudent == null)
            {
                return NotFound();
            }
            ViewData["Department"] = new SelectList(_context.Departments, "Id", "Id", pgStudent.Department);
            ViewData["Faculty"] = new SelectList(_context.Faculties, "Id", "Id", pgStudent.Faculty);
            ViewData["LGAId"] = new SelectList(_context.Lgas, "Id", "Id", pgStudent.LGAId);
            ViewData["Level"] = new SelectList(_context.Levels, "Id", "Id", pgStudent.Level);
            ViewData["NationalityId"] = new SelectList(_context.Countries, "Id", "Id", pgStudent.NationalityId);
            ViewData["CurrentSession"] = new SelectList(_context.Sessions, "Id", "Id", pgStudent.CurrentSession);
            ViewData["ClearedBy"] = new SelectList(_context.Staffs, "Id", "Id", pgStudent.ClearedBy);
            ViewData["StateOfOriginId"] = new SelectList(_context.States, "Id", "Id", pgStudent.StateOfOriginId);
            return View(pgStudent);
        }

        // POST: pgStudents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentId,Picture,Fullname,Sex,DOB,Religion,Phone,AltPhoneNumber,Email,NationalityId,StateOfOriginId,LGAId,PlaceOfBirth,PermanentHomeAddress,ContactAddress,MaritalStatus,NextOfkinName,NextOfKinRelationship,NextOfKinPhone,NextOfKinAddress,SchoolEmailAddress,MatNumber,Faculty,Level,ModeOfAdmission,YearOfAdmission,Department,CurrentSession,CreatedAt,Cleared,ClearedBy")] PgStudent pgStudent)
        {
            if (id != pgStudent.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pgStudent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PgStudentExists(pgStudent.Id))
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
            ViewData["Department"] = new SelectList(_context.Departments, "Id", "Id", pgStudent.Department);
            ViewData["Faculty"] = new SelectList(_context.Faculties, "Id", "Id", pgStudent.Faculty);
            ViewData["LGAId"] = new SelectList(_context.Lgas, "Id", "Id", pgStudent.LGAId);
            ViewData["Level"] = new SelectList(_context.Levels, "Id", "Id", pgStudent.Level);
            ViewData["NationalityId"] = new SelectList(_context.Countries, "Id", "Id", pgStudent.NationalityId);
            ViewData["CurrentSession"] = new SelectList(_context.Sessions, "Id", "Id", pgStudent.CurrentSession);
            ViewData["ClearedBy"] = new SelectList(_context.Staffs, "Id", "Id", pgStudent.ClearedBy);
            ViewData["StateOfOriginId"] = new SelectList(_context.States, "Id", "Id", pgStudent.StateOfOriginId);
            return View(pgStudent);
        }

        // GET: pgStudents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PostGraduateStudents == null)
            {
                return NotFound();
            }

            var pgStudent = await _context.PostGraduateStudents
                .Include(p => p.Departments)
                .Include(p => p.Faculties)
                .Include(p => p.LGAs)
                .Include(p => p.Levels)
                .Include(p => p.Nationalities)
                .Include(p => p.Sessions)
                .Include(p => p.Staffs)
                .Include(p => p.States)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pgStudent == null)
            {
                return NotFound();
            }

            return View(pgStudent);
        }

        // POST: pgStudents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.PostGraduateStudents == null)
            {
                return Problem("Entity set 'ApplicationDbContext.PostGraduateStudents'  is null.");
            }
            var pgStudent = await _context.PostGraduateStudents.FindAsync(id);
            if (pgStudent != null)
            {
                _context.PostGraduateStudents.Remove(pgStudent);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Eclearance()
        {
            ViewBag.success = TempData["success"];
            ViewData["SessionId"] = new SelectList(_context.Sessions, "Id", "Name");
            return View();
        }

        // POST: busaryClearances/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // [Authorize(Roles = "student")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eclearance(PgClearance offlinePayment)
        {
            var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);
            var user = loggedInUser.PgStudent;
            offlinePayment.StudentId = user;
            offlinePayment.CreatedAt = DateTime.Now;
            _context.Add(offlinePayment);
            await _context.SaveChangesAsync();
            TempData["success"] = "Record added successfully.";
            return RedirectToAction(nameof(Eclearance));

        }
        public async Task<IActionResult> History()
        {

            var sessions = (from c in _context.Sessions select c);
            return View(await sessions.ToListAsync());
        }
        [Authorize(Roles = "pgStudent")]
        public async Task<IActionResult> Preview(string? id)
        {
            try
            {
                var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);
                var userId = loggedInUser.PgStudent;
                var student = (from s in _context.PostGraduateStudents where s.Id == userId select s)
                    .Include(c => c.Departments).Include(c => c.Levels).Include(c => c.Sessions).FirstOrDefault();
                ViewBag.name = student.Fullname;
                ViewBag.email = student.SchoolEmailAddress;
                ViewBag.mat = student.MatNumber;
                ViewBag.dept = student.Departments.Name;
                //ViewBag.programme = student.Programs.NameOfProgram;
                ViewBag.session = student.Sessions.Name;
                ViewBag.level = student.Levels.Name;
                var clearance = (from s in _context.PgClearances where s.StudentId == userId && s.Sessions.Name == id select s).Include(i => i.Sessions).ToList();

                var clearedStatus = _context.PgClearances
                 .Where(x => x.StudentId == userId)
                 .Include(x => x.Sessions)
                 .ToList();

                if (clearedStatus != null)
                {
                    if (clearedStatus.Any(item => item.Status != MainStatus.Approved))
                    {
                        ViewBag.status = "Pending";
                    }
                    else
                    {
                        ViewBag.status = " ";
                    }
                }
                else
                {
                    ViewBag.status = "Pending";
                }
                return View(clearance);
            }
            catch (Exception e)
            {
                TempData["err"] = e.Message;
                return RedirectToAction("badreq", "error");
                throw;
            }

        }
        private bool PgStudentExists(int? id)
        {
          return _context.PostGraduateStudents.Any(e => e.Id == id);
        }

        ///////////////////////////////////////////////////
        /////// WALLET SECTION
        //////////////////////////////////////////////////
        [Authorize(Roles = "pgStudent, superAdmin")]
        public async Task<IActionResult> Debts()
        {
            var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);
            var user = loggedInUser.PgStudent;

            var student = (from s in _context.PostGraduateStudents where s.Id == user select s).FirstOrDefault();
            var wallet = (from s in _context.PgSubWallets where s.WalletId == student.StudentId select s).Include(c => c.Sessions).ToList();

            ViewBag.utme = student.UTMENumber;
            if (wallet == null)
            {
                return RedirectToAction("pagenotfound", "error");
            }

            return View(wallet);

        }
        [Authorize(Roles = "pgStudent, superAdmin")]
        //Initiating Acceptance payment
        public async Task<IActionResult> Acceptance(string id, PgOrder payment, Student student)
        {
            var wallet = await _context.PgSubWallets
                 .FirstOrDefaultAsync(m => m.WalletId == id);

            Random r = new();
            //Payment is created just before it returns the view
            ViewBag.Name = wallet.Name;
            payment.SessionId = wallet.SessionId;
            payment.WalletId = wallet.Id;
            payment.Amount = (double)wallet.AcceptanceFee + 300;
            payment.Status = "Pending";
            payment.Ref = "EDSU-" + r.Next(10000000) + DateTime.Now.Millisecond;
            payment.PaymentDate = DateTime.Now;
            payment.Type = "Acceptance";
            _context.PgOrders.Add(payment);
            await _context.SaveChangesAsync();

            //Get the payment to return
            var paymentToGet = await _context.PgOrders
                .FindAsync(payment.Id);
            if (paymentToGet == null)
            {
                return NotFound();
            }
            //When the payment row is created, it stores the id in a tempdata then pass it to the verify endpoint
            TempData["PaymentId"] = payment.Wallets.WalletId;
            TempData["walletId"] = id;
            return View(paymentToGet);
        }
        public async Task<IActionResult> Tuition(string id, PgOrder payment, Student student)
        {
            var wallet = await _context.PgSubWallets
                 .FirstOrDefaultAsync(m => m.WalletId == id);

            Random r = new();
            //Payment is created just before it returns the view
            ViewBag.Name = wallet.Name;
            payment.SessionId = wallet.SessionId;
            payment.WalletId = wallet.Id;
            payment.Amount = (double)wallet.Tuition + 300;
            payment.Status = "Pending";
            payment.Ref = "EDSU-" + r.Next(10000000) + DateTime.Now.Millisecond;
            payment.PaymentDate = DateTime.Now;
            payment.Type = "Tuition";
            _context.PgOrders.Add(payment);
            await _context.SaveChangesAsync();

            //Get the payment to return
            var paymentToGet = await _context.PgOrders
                .FindAsync(payment.Id);
            if (paymentToGet == null)
            {
                return NotFound();
            }
            //When the payment row is created, it stores the id in a tempdata then pass it to the verify endpoint
            TempData["PaymentId"] = payment.Wallets.WalletId;
            TempData["walletId"] = id;
            return View(paymentToGet);
        }
        [Authorize(Roles = "pgStudent, superAdmin")]
        //Initiating Tuition 60 Percent payment
        public async Task<IActionResult> Tuition60(string id, PgOrder payment, Student student)
        {
            var wallet = await _context.PgSubWallets
                 .FirstOrDefaultAsync(m => m.WalletId == id);

            Random r = new();
            //Payment is created just before it returns the view
            ViewBag.Name = wallet.Name;
            payment.SessionId = wallet.SessionId;
            payment.WalletId = wallet.Id;
            payment.Amount = (double)wallet.SixtyPercent + 300;
            payment.Status = "Pending";
            payment.Ref = "EDSU-" + r.Next(10000000) + DateTime.Now.Millisecond;
            payment.PaymentDate = DateTime.Now;
            payment.Type = "Tuition(60%)";
            _context.PgOrders.Add(payment);
            await _context.SaveChangesAsync();

            //Get the payment to return
            var paymentToGet = await _context.PgOrders
                .FindAsync(payment.Id);
            if (paymentToGet == null)
            {
                return NotFound();
            }
            //When the payment row is created, it stores the id in a tempdata then pass it to the verify endpoint
            TempData["PaymentId"] = payment.Wallets.WalletId;
            TempData["walletId"] = id;
            return View(paymentToGet);
        }
        [Authorize(Roles = "pgStudent, superAdmin")]
        //Initiating Tuition 40 Percent payment
        public async Task<IActionResult> Tuition40(string id, PgOrder payment, Student student)
        {
            var wallet = await _context.PgSubWallets
                 .FirstOrDefaultAsync(m => m.WalletId == id);

            Random r = new();
            //Payment is created just before it returns the view
            ViewBag.Name = wallet.Name;
            payment.SessionId = wallet.SessionId;
            payment.WalletId = wallet.Id;
            payment.Amount = (double)wallet.FortyPercent + 300;
            payment.Status = "Pending";
            payment.Ref = "EDSU-" + r.Next(10000000) + DateTime.Now.Millisecond;
            payment.PaymentDate = DateTime.Now;
            payment.Type = "Tuition(40%)";
            _context.PgOrders.Add(payment);
            await _context.SaveChangesAsync();

            //Get the payment to return
            var paymentToGet = await _context.PgOrders
                .FindAsync(payment.Id);
            if (paymentToGet == null)
            {
                return NotFound();
            }
            //When the payment row is created, it stores the id in a tempdata then pass it to the verify endpoint
            TempData["PaymentId"] = payment.Wallets.WalletId;
            TempData["walletId"] = id;
            return View(paymentToGet);
        }
        [Authorize(Roles = "pgStudent, superAdmin")]
        //Initiating LMS payment
        public async Task<IActionResult> LMS(string id, PgOrder payment, Student student)
        {
            var wallet = await _context.PgSubWallets
                 .FirstOrDefaultAsync(m => m.WalletId == id);

            Random r = new();
            //Payment is created just before it returns the view
            ViewBag.Name = wallet.Name;
            payment.SessionId = wallet.SessionId;
            payment.WalletId = wallet.Id;
            payment.Amount = (double)wallet.LMS + 300;
            payment.Status = "Pending";
            payment.Ref = "EDSU-" + r.Next(10000000) + DateTime.Now.Millisecond;
            payment.PaymentDate = DateTime.Now;
            payment.Type = "LMS";
            _context.PgOrders.Add(payment);
            await _context.SaveChangesAsync();

            //Get the payment to return
            var paymentToGet = await _context.PgOrders
                .FindAsync(payment.Id);
            if (paymentToGet == null)
            {
                return NotFound();
            }
            //When the payment row is created, it stores the id in a tempdata then pass it to the verify endpoint
            TempData["PaymentId"] = payment.Wallets.WalletId;
            TempData["walletId"] = id;
            return View(paymentToGet);
        }
        [Authorize(Roles = "pgStudent, superAdmin")]
        //Initiating SRC payment
        public async Task<IActionResult> SRC(string id, PgOrder payment, Student student)
        {
            var wallet = await _context.PgSubWallets
                 .FirstOrDefaultAsync(m => m.WalletId == id);

            Random r = new();
            //Payment is created just before it returns the view
            ViewBag.Name = wallet.Name;
            payment.SessionId = wallet.SessionId;
            payment.WalletId = wallet.Id;
            payment.Amount = (double)wallet.SRC + 300;
            payment.Status = "Pending";
            payment.Ref = "EDSU-" + r.Next(10000000) + DateTime.Now.Millisecond;
            payment.PaymentDate = DateTime.Now;
            payment.Type = "SRC";
            _context.PgOrders.Add(payment);
            await _context.SaveChangesAsync();

            //Get the payment to return
            var paymentToGet = await _context.PgOrders
                .FindAsync(payment.Id);
            if (paymentToGet == null)
            {
                return NotFound();
            }
            //When the payment row is created, it stores the id in a tempdata then pass it to the verify endpoint
            TempData["PaymentId"] = payment.Wallets.WalletId;
            TempData["walletId"] = id;
            return View(paymentToGet);
        }
        [Authorize(Roles = "pgStudent, superAdmin")]
        //Initiating EDHIS payment
        public async Task<IActionResult> EDHIS(string id, PgOrder payment, Student student)
        {
            var wallet = await _context.PgSubWallets
                 .FirstOrDefaultAsync(m => m.WalletId == id);

            Random r = new();
            //Payment is created just before it returns the view
            ViewBag.Name = wallet.Name;
            payment.SessionId = wallet.SessionId;
            payment.WalletId = wallet.Id;
            payment.Amount = (double)wallet.EDHIS + 300;
            payment.Status = "Pending";
            payment.Ref = "EDSU-" + r.Next(10000000) + DateTime.Now.Millisecond;
            payment.PaymentDate = DateTime.Now;
            payment.Type = "EDHIS";
            _context.PgOrders.Add(payment);
            await _context.SaveChangesAsync();

            //Get the payment to return
            var paymentToGet = await _context.PgOrders
                .FindAsync(payment.Id);
            if (paymentToGet == null)
            {
                return NotFound();
            }
            //When the payment row is created, it stores the id in a tempdata then pass it to the verify endpoint
            TempData["PaymentId"] = payment.Wallets.WalletId;
            TempData["walletId"] = id;
            return View(paymentToGet);
        }
        [Authorize(Roles = "pgStudent, superAdmin")]
        //Proceed to payment Gateway
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEmail(string? Ref)
        {
            try
            {
                //var order = (from x in _context.Payments where x.Ref == Ref select x.Wallets.WalletId).FirstOrDefault();

                var PaymentToUpdate = await _context.PgOrders
               .FirstOrDefaultAsync(c => c.Ref == Ref);
                var orderid = Ref;
                if (await TryUpdateModelAsync<PgOrder>(PaymentToUpdate, "", c => c.Email))
                {

                    try
                    {
                        PaymentToUpdate.Mode = "Paystack";
                        await _context.SaveChangesAsync();

                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        ModelState.AddModelError("", "Unable to save changes. " +
                            "Try again, and if the problem persists, " +
                            "see your system administrator.");
                    }
                    return RedirectToAction("checkout", "pgstudents", new { orderid });

                }
            }
            catch (Exception ex)
            {
                ex.ToString();

            }
            return View();

        }
        public async Task<IActionResult> Checkout(string? orderid)
        {
            var paymentToGet = await _context.PgOrders
                .FirstOrDefaultAsync(x => x.Ref == orderid);
            if (orderid == null || _context.Payments == null)
            {
                return NotFound();
            }
            if (paymentToGet == null)
            {
                return NotFound();
            }
            return View(paymentToGet);
        }
        public async Task<IActionResult> UpdatePayment(string data, BursaryClearance bursaryClearance, BursaryClearanceFresher bcf)
        {

            var walletId = TempData["walletId"];
            var payments = _context.PgOrders.FirstOrDefault(c => c.Ref == data);
            
            payments.Status = "Approved";
            switch (payments.Type)
            {
                case "Tuition":
                    if (payments.Status == "Approved")
                    {
                        var wallet = _context.PgSubWallets.FirstOrDefault(i => i.WalletId == walletId);
                        var newDebit = wallet.Debit - wallet.Tuition;
                        wallet.Debit = newDebit;

                        var bulkwallet = _context.PgMainWallets.FirstOrDefault(i => i.WalletId == walletId);
                        var newBulkDebit = bulkwallet.BulkDebitBalanace - wallet.Tuition;
                        bulkwallet.BulkDebitBalanace = newBulkDebit;

                        wallet.Tuition = 0;

                        wallet.SixtyPercent = 0;
                        wallet.FortyPercent = 0;
                        _context.SaveChanges();
                    }
                    break;
                case "Tuition(60%)":
                    if (payments.Status == "Approved")
                    {
                        var wallet = _context.PgSubWallets.FirstOrDefault(i => i.WalletId == walletId);
                        var newDebit = wallet.Debit - wallet.SixtyPercent;
                        wallet.Debit = newDebit;
                        var bulkwallet = _context.PgMainWallets.FirstOrDefault(i => i.WalletId == walletId);
                        var newBulkDebit = bulkwallet.BulkDebitBalanace - wallet.SixtyPercent;
                        bulkwallet.BulkDebitBalanace = newBulkDebit;

                        wallet.SixtyPercent = 0;
                        wallet.Tuition = 0;
                        _context.SaveChanges();
                    }
                    break;
                case "Tuition(40%)":
                    if (payments.Status == "Approved")
                    {
                        var wallet = _context.PgSubWallets.FirstOrDefault(i => i.WalletId == walletId);
                        var newDebit = wallet.Debit - wallet.FortyPercent;
                        wallet.Debit = newDebit;

                        var bulkwallet = _context.PgMainWallets.FirstOrDefault(i => i.WalletId == walletId);
                        var newBulkDebit = bulkwallet.BulkDebitBalanace - wallet.FortyPercent;
                        bulkwallet.BulkDebitBalanace = newBulkDebit;

                        wallet.FortyPercent = 0;
                        _context.SaveChanges();
                    }
                    break;
                case "EDHIS":
                    if (payments.Status == "Approved")
                    {
                        var wallet = _context.PgSubWallets.FirstOrDefault(i => i.WalletId == walletId);
                        var newDebit = wallet.Debit - wallet.EDHIS;
                        wallet.Debit = newDebit;
                        var bulkwallet = _context.PgMainWallets.FirstOrDefault(i => i.WalletId == walletId);
                        var newBulkDebit = bulkwallet.BulkDebitBalanace - wallet.EDHIS;
                        bulkwallet.BulkDebitBalanace = newBulkDebit;

                        wallet.EDHIS = 0;
                        _context.SaveChanges();
                    }
                    break;
                case "LMS":
                    if (payments.Status == "Approved")
                    {
                        var wallet = _context.PgSubWallets.FirstOrDefault(i => i.WalletId == walletId);
                        var newDebit = wallet.Debit - wallet.LMS;
                        wallet.Debit = newDebit;

                        var bulkwallet = _context.PgMainWallets.FirstOrDefault(i => i.WalletId == walletId);
                        var newBulkDebit = bulkwallet.BulkDebitBalanace - wallet.LMS;
                        bulkwallet.BulkDebitBalanace = newBulkDebit;

                        wallet.LMS = 0;
                        _context.SaveChanges();
                    }
                    break;
                case "SRC":
                    if (payments.Status == "Approved")
                    {
                        var wallet = _context.PgSubWallets.FirstOrDefault(i => i.WalletId == walletId);
                        var newDebit = wallet.Debit - wallet.SRC;
                        wallet.Debit = newDebit;

                        var bulkwallet = _context.PgMainWallets.FirstOrDefault(i => i.WalletId == walletId);
                        var newBulkDebit = bulkwallet.BulkDebitBalanace - wallet.SRC;
                        bulkwallet.BulkDebitBalanace = newBulkDebit;

                        wallet.SRC = 0;
                        _context.SaveChanges();
                    }
                    break;
                case "Acceptance":
                    if (payments.Status == "Approved")
                    {
                        var wallet = _context.PgSubWallets.FirstOrDefault(i => i.WalletId == walletId);
                        var newDebit = wallet.Debit - wallet.AcceptanceFee;
                        wallet.Debit = newDebit;

                        var bulkwallet = _context.PgMainWallets.FirstOrDefault(i => i.WalletId == walletId);
                        var newBulkDebit = bulkwallet.BulkDebitBalanace - wallet.AcceptanceFee;
                        bulkwallet.BulkDebitBalanace = newBulkDebit;

                        wallet.AcceptanceFee = 0;
                        _context.SaveChanges();
                    }
                    break;
                case "Tuition Custom":
                    if (payments.Status == "Approved")
                    {
                        decimal amount = (decimal)(payments.Amount - 300);
                        var wallet = _context.PgSubWallets.FirstOrDefault(i => i.WalletId == walletId);
                        var newDebit = wallet.Debit - amount;
                        wallet.Debit = newDebit;

                        var bulkwallet = _context.PgMainWallets.FirstOrDefault(i => i.WalletId == walletId);
                        var newBulkDebit = bulkwallet.BulkDebitBalanace - amount;
                        bulkwallet.BulkDebitBalanace = newBulkDebit;

                        wallet.Tuition = 0;
                        wallet.SixtyPercent -= amount;
                        _context.SaveChanges();
                    }

                    break;

            }
           
            return RedirectToAction("summary", "pgstudents");
        }
        public IActionResult Summary()
        {
            return View();

        }

    }
}
