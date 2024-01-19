using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EDSU_SYSTEM.Data;
using EDSU_SYSTEM.Models;
using System.Collections;
using Microsoft.AspNetCore.Identity;
using static EDSU_SYSTEM.Models.Enum;
using JsonSerializer = System.Text.Json.JsonSerializer;
using OfficeOpenXml.Style;
using Microsoft.AspNetCore.Authorization;
using System.Drawing;
using CanvasApi.Client.Users.Models;
using Enum = EDSU_SYSTEM.Models.Enum;

namespace EDSU_SYSTEM.Controllers
{
   // [Authorize]
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UserManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public StudentsController(UserManager<ApplicationUser> userManager, UserManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IWebHostEnvironment hostingEnvironment, ApplicationDbContext context)
        {
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }
        public async Task<IActionResult> CreateApplicantAccount()
        {
            try
            {
                var applicants = _context.UgApplicants.ToList();
                foreach (var item in applicants)
                {

                }
                var id = "c35a60d0-0ff8-46f3-afbc-9ba7eced4c5b";
                var users = _userManager.Users.Where(x => x.Type == 1).ToList();
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
        public async Task<IActionResult> UsersMigrate()
        {
            var students = (from s in _context.Students where s.Cleared == false select s).ToList();
            foreach (var item in students)
            {
                try
                {
                    var user = new ApplicationUser
                    {
                        Email = item.SchoolEmailAddress,
                        UserName = item.SchoolEmailAddress,
                        StudentsId = item.Id,
                        PhoneNumber = item.Phone,
                        PhoneNumberConfirmed = true,
                        Type = 1,
                        EmailConfirmed = true
                    };
                    var r = await _userManager.CreateAsync(user, item.Phone);
                    try
                    {
                        var roleId = "4e8c6adc-ae5c-46e4-8618-e0b18f0841ba";
                        var role = await _roleManager.FindByIdAsync(roleId);
                        await _userManager.AddToRoleAsync(user, role.Name);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        throw;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }
            

                return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> MigrateStudent(string email)
        {
            try
            {
                var student = (from s in _context.Students where s.SchoolEmailAddress == email select s).FirstOrDefault();

                var user = new ApplicationUser
                {
                    Email = student.SchoolEmailAddress,
                    UserName = student.SchoolEmailAddress,
                    StudentsId = student.Id,
                    PhoneNumber = student.Phone,
                    PhoneNumberConfirmed = true,
                    Type = 1,
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

        [Authorize(Roles = "superAdmin")]
        //Set wallet fees for students
        public async Task<IActionResult> Setwallet(string? id)
        {
            var student = (from st in _context.Students where st.SchoolEmailAddress == id select st).FirstOrDefault();
            var hasMainWallet = (from main in _context.UgMainWallets where main.WalletId == student.UTMENumber select main.WalletId).FirstOrDefault();
            var ugmain = new UgMainWallet();
            if (hasMainWallet == null)
            {
                ugmain.UTME = student.UTMENumber;
                ugmain.WalletId = student.UTMENumber;
                ugmain.Status = true;
                ugmain.CreditBalance = 0;
                ugmain.BulkDebitBalanace = 0;
                ugmain.DateCreated = DateTime.Now;
                ugmain.Name = student.Fullname;
                _context.UgMainWallets.Add(ugmain);
                await _context.SaveChangesAsync();
            }
            try
                {
                var tuition = (from tu in _context.Fees where tu.DepartmentId == student.Department select tu).FirstOrDefault();
                if (tuition == null)
                {
                    tuition.Level1 = 0;
                }
                Random r = new();
                // string a = st.Id.ToString() + r.Next(10000);

                // Create a new instance of UgSubWallet for each student
                var newSubWallet = new UgSubWallet();
                newSubWallet.WalletId = student.UTMENumber;
                newSubWallet.Name = student.Fullname;
                newSubWallet.RegNo = student.UTMENumber;
                newSubWallet.CreditBalance = 0;
                newSubWallet.Status = true;
                newSubWallet.DateCreated = DateTime.Now;
                newSubWallet.Tuition = tuition.Level1;
                   
                newSubWallet.Tuition2 = 0;

                newSubWallet.FortyPercent = newSubWallet.Tuition * 40 / 100;
                newSubWallet.SixtyPercent = newSubWallet.Tuition * 60 / 100;
                newSubWallet.LMS = 40000;
                newSubWallet.AcceptanceFee = 70000;
                newSubWallet.SRC = 2000;
                newSubWallet.EDHIS = 25000;
                newSubWallet.SessionId = 9;
               // newSubWallet.Debit = 
                var sr = newSubWallet.Tuition + newSubWallet.Tuition2 + newSubWallet.LMS
                                    + newSubWallet.EDHIS + newSubWallet.SRC + newSubWallet.AcceptanceFee;
                
                newSubWallet.Level = student.Level;
                newSubWallet.Department = student.Department;

                _context.UgSubWallets.Add(newSubWallet);
                await _context.SaveChangesAsync();

                var main = (from m in _context.UgMainWallets where m.WalletId == newSubWallet.WalletId select m).FirstOrDefault();
                main.BulkDebitBalanace = newSubWallet.Debit;
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                // Handle exceptions appropriately
                throw;
            }
            

            return View();
        }
        [Authorize(Roles = "superAdmin")]
        //Populate fees for applicants 23/34
        public async Task<IActionResult> PopulateWallet(UgSubWallet subWallet, UgMainWallet ugmain)
        {
            var students = (from s in _context.UgApplicants where s.Status == (Enum.MainStatus)1 || s.Status == (Enum.MainStatus)2 select s).ToList();

            foreach (var st in students)
            {
                try
                {
                    var tuition = (from tu in _context.Fees where tu.DepartmentId == st.AdmittedInto select tu).FirstOrDefault();
                    if (tuition == null)
                    {
                        tuition = new Fee { Level1 = 0 };
                    }
                    Random r = new();
                   // string a = st.Id.ToString() + r.Next(10000);

                    // Create a new instance of UgSubWallet for each student
                    var newSubWallet = new UgSubWallet();
                    newSubWallet.WalletId = st.UTMENumber;
                    newSubWallet.Name = st.Surname + " " + st.FirstName + " " + st.OtherName;
                    newSubWallet.RegNo = st.UTMENumber;
                    newSubWallet.CreditBalance = 0;
                    newSubWallet.Status = true;
                    newSubWallet.DateCreated = DateTime.Now;
                    newSubWallet.Tuition = tuition.Level1;
                    if (st.ModeOfEntry == "Transfer" && (st.AdmittedInto == 38 || st.AdmittedInto == 24 || st.AdmittedInto == 1))
                    {
                        newSubWallet.Tuition2 = tuition.Level1;
                    }
                    else
                    {
                        newSubWallet.Tuition2 = 0;
                    }

                    newSubWallet.FortyPercent = newSubWallet.Tuition * 40 / 100;
                    newSubWallet.SixtyPercent = newSubWallet.Tuition * 60 / 100;
                    newSubWallet.LMS = 40000;
                    newSubWallet.AcceptanceFee = 70000;
                    newSubWallet.SRC = 2000;
                    newSubWallet.EDHIS = 25000;
                    newSubWallet.SessionId = 9;
                    newSubWallet.Debit = newSubWallet.Tuition + newSubWallet.Tuition2 + newSubWallet.LMS
                                        + newSubWallet.EDHIS + newSubWallet.SRC + newSubWallet.AcceptanceFee;
                    newSubWallet.Level = st.LevelAdmittedTo;
                    newSubWallet.Department = st.AdmittedInto;

                    _context.UgSubWallets.Add(newSubWallet);
                    await _context.SaveChangesAsync();

                    var main = (from m in _context.UgMainWallets where m.WalletId == newSubWallet.WalletId select m).FirstOrDefault();
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
        //Create mainwallet for applicants
        [Authorize(Roles = "superAdmin")]
        public async Task<IActionResult> CreateMainWalletUGA(UgSubWallet subWallet, UgMainWallet ugmain)
        {
            var students = (from s in _context.Students select s).ToList();

            //var students = (from s in _context.UgApplicants select s).ToList();

            foreach (var st in students)
            {
                try
                {
                    var newUgMain = new UgMainWallet();
                    
                    newUgMain.ApplicantId = 22;
                    //ugmain.Id = int.Parse(p);
                    newUgMain.Name = st.Fullname;
                    newUgMain.WalletId = st.UTMENumber;
                    newUgMain.UTME = st.UTMENumber;
                    newUgMain.BulkDebitBalanace = 0;
                    newUgMain.CreditBalance = 0;
                    newUgMain.Status = false;
                    newUgMain.DateCreated = DateTime.Now;
                    _context.UgMainWallets.Add(newUgMain);
                    await _context.SaveChangesAsync();

                    // Create a new instance of UgSubWallet for each student
                    var newSubWallet = new UgSubWallet();
                    var fee = (from tu in _context.Fees where tu.DepartmentId == st.Department select tu).FirstOrDefault();
                    if (fee == null)
                    {
                        fee = new Fee { Level1 = 0 };
                    }
                    if (st.Level == 2)
                    {
                        newSubWallet.Tuition = fee.Level2;

                    }
                    else if (st.Level == 3)
                    {
                        newSubWallet.Tuition = fee.Level3;
                    }
                    else if (st.Level == 4)
                    {
                        newSubWallet.Tuition = fee.Level4;
                    }
                    else if (st.Level == 5)
                    {
                        newSubWallet.Tuition = fee.Level5;
                    }
                    else if (st.Level == 6)
                    {
                        newSubWallet.Tuition = fee.Level6;
                    }


                   // newSubWallet.Tuition = fee.Level1;
                    newSubWallet.Level = st.Level;
                    newSubWallet.WalletId = st.UTMENumber;
                    newSubWallet.Name = st.Fullname;
                    newSubWallet.RegNo = st.UTMENumber;
                    newSubWallet.CreditBalance = 0;
                    newSubWallet.Status = true;
                    newSubWallet.DateCreated = DateTime.Now;
                    newSubWallet.FortyPercent = newSubWallet.Tuition * 40 / 100;
                    newSubWallet.SixtyPercent = newSubWallet.Tuition * 60 / 100;
                    newSubWallet.LMS = 40000;
                    newSubWallet.AcceptanceFee = 0;
                    newSubWallet.SRC = 2000;
                    newSubWallet.EDHIS = 25000;
                    newSubWallet.SessionId = 9;
                    newSubWallet.Tuition2 = 0;
                    Console.Write("This is the supposed debit " + newSubWallet.Debit);

                    newSubWallet.Debit = newSubWallet.Tuition + newSubWallet.Tuition2 + newSubWallet.LMS
                        + newSubWallet.EDHIS + newSubWallet.SRC + newSubWallet.AcceptanceFee;
                    newSubWallet.Department = st.Department;

                    _context.UgSubWallets.Add(newSubWallet);
                    await _context.SaveChangesAsync();

                    var main = (from m in _context.UgMainWallets where m.WalletId == newSubWallet.WalletId select m).FirstOrDefault();
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
       //[Authorize(Roles = "superAdmin")]
        public async Task<IActionResult> CreateMainWalletUGA1(UgSubWallet subWallet, UgMainWallet ugmain)
        {
            var students = (from s in _context.UgApplicants where s.ApplicantionId == "2" select s).ToList();

            foreach (var st in students)
            {
                var walletExist = (from a in _context.UgMainWallets where st.UTMENumber == a.WalletId select a).FirstOrDefault();
                if (walletExist == null)
                {
                    try
                    {
                        var newUgMain = new UgMainWallet();
                        newUgMain.ApplicantId = 3;
                        //ugmain.Id = int.Parse(p);
                        newUgMain.Name = st.Surname + " " + st.FirstName + " " + st.OtherName;
                        newUgMain.WalletId = st.UTMENumber;
                        newUgMain.UTME = st.UTMENumber;
                        newUgMain.BulkDebitBalanace = 0;
                        newUgMain.CreditBalance = 0;
                        newUgMain.Status = true;
                        ugmain.StudentType = 0;
                        newUgMain.DateCreated = DateTime.Now;
                        _context.UgMainWallets.Add(newUgMain);
                        await _context.SaveChangesAsync();

                        // Create a new instance of UgSubWallet for each student
                        var newSubWallet = new UgSubWallet();
                        var fee = (from tu in _context.Fees where tu.DepartmentId == st.AdmittedInto select tu).FirstOrDefault();
                        if (fee == null)
                        {
                            fee = new Fee { Level1 = 0 };
                        }
                        newSubWallet.Tuition = fee.Level1;
                        newSubWallet.Level = st.LevelAdmittedTo;
                        newSubWallet.WalletId = st.UTMENumber;
                        newSubWallet.Name = st.Surname + " " + st.FirstName + " " + st.OtherName;
                        newSubWallet.RegNo = st.UTMENumber;
                        newSubWallet.CreditBalance = 0;
                        newSubWallet.Status = true;
                        newSubWallet.Waiver = false;
                        newSubWallet.DateCreated = DateTime.Now;

                        if (st.ModeOfEntry == "3" && (st.AdmittedInto == 38 || st.AdmittedInto == 24 || st.AdmittedInto == 1))
                        {
                            newSubWallet.Tuition2 = fee.Level1;
                        }
                        else
                        {
                            newSubWallet.Tuition2 = 0;
                        }

                        newSubWallet.FortyPercent = newSubWallet.Tuition * 40 / 100;
                        newSubWallet.SixtyPercent = newSubWallet.Tuition * 60 / 100;
                        newSubWallet.LMS = 40000;
                        newSubWallet.AcceptanceFee = 70000;
                        newSubWallet.SRC = 2000;
                        newSubWallet.EDHIS = 25000;
                        newSubWallet.SessionId = 9;


                        var f = newSubWallet.Tuition + newSubWallet.Tuition2 + newSubWallet.LMS
                                             + newSubWallet.EDHIS + newSubWallet.SRC + newSubWallet.AcceptanceFee;
                        Console.Write("This is the supposed debit " + f);
                        newSubWallet.Debit = f;
                        newSubWallet.Department = st.AdmittedInto;

                        _context.UgSubWallets.Add(newSubWallet);
                        await _context.SaveChangesAsync();

                        var main = (from m in _context.UgMainWallets where m.WalletId == newSubWallet.WalletId select m).FirstOrDefault();
                        main.BulkDebitBalanace = newSubWallet.Debit;
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception)
                    {
                        // Handle exceptions appropriately
                        throw;
                    }
                }
                
            }

            return View();
        }
        [Authorize(Roles = "superAdmin")]
        public async Task<IActionResult> CreateJupebMainWallet(UgSubWallet subWallet, UgMainWallet ugmain)
        {
            var students = (from s in _context.JupebApplicants select s).ToList();

            foreach (var st in students)
            {
                var walletExist = (from a in _context.UgMainWallets where st.ApplicantionId == a.WalletId select a).FirstOrDefault();
                if (walletExist == null)
                {
                    try
                    {
                        var newUgMain = new UgMainWallet();
                        newUgMain.ApplicantId = 23;
                        //ugmain.Id = int.Parse(p);
                        newUgMain.Name = st.Surname + " " + st.FirstName + " " + st.OtherName;
                        newUgMain.WalletId = st.ApplicantionId;
                        newUgMain.UTME = st.ApplicantionId;
                        newUgMain.BulkDebitBalanace = 0;
                        newUgMain.CreditBalance = 0;
                        newUgMain.Status = true;
                        newUgMain.DateCreated = DateTime.Now;
                        _context.UgMainWallets.Add(newUgMain);
                        await _context.SaveChangesAsync();

                        // Create a new instance of UgSubWallet for each student
                        var newSubWallet = new UgSubWallet();
                       
                        newSubWallet.Tuition = 150000;
                        newSubWallet.Level = st.LevelAdmittedTo;
                        newSubWallet.WalletId = st.ApplicantionId;
                        newSubWallet.Name = st.Surname + " " + st.FirstName + " " + st.OtherName;
                        newSubWallet.RegNo = st.ApplicantionId;
                        newSubWallet.CreditBalance = 0;
                        newSubWallet.Status = true;
                        newSubWallet.DateCreated = DateTime.Now;
                        newSubWallet.Tuition2 = 0;
                        newSubWallet.FortyPercent = newSubWallet.Tuition * 40 / 100;
                        newSubWallet.SixtyPercent = newSubWallet.Tuition * 60 / 100;
                        newSubWallet.LMS = 35000;
                        newSubWallet.AcceptanceFee = 50000;
                        newSubWallet.SRC = 0;
                        newSubWallet.EDHIS = 0;
                        newSubWallet.SessionId = 9;


                        var f = newSubWallet.Tuition + newSubWallet.Tuition2 + newSubWallet.LMS
                                             + newSubWallet.EDHIS + newSubWallet.SRC + newSubWallet.AcceptanceFee;
                        Console.Write("This is the supposed debit " + f);
                        newSubWallet.Debit = f;
                        newSubWallet.Department = st.AdmittedInto;

                        _context.UgSubWallets.Add(newSubWallet);
                        await _context.SaveChangesAsync();

                        var main = (from m in _context.UgMainWallets where m.WalletId == newSubWallet.WalletId select m).FirstOrDefault();
                        main.BulkDebitBalanace = newSubWallet.Debit;
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception)
                    {
                        // Handle exceptions appropriately
                        throw;
                    }
                }

            }

            return View();
        }

        [Authorize(Roles = "superAdmin")]
        //For migrated Students
        public async Task<IActionResult> ActivateWallet(string? id, UgSubWallet myWallet)
        {
            var students = (from s in _context.Students select s).ToList();
            foreach (var item in students)
            {
                var session = await _context.Sessions.FirstOrDefaultAsync(s => s.IsActive == true);
                myWallet.SessionId = session.Id;

                myWallet.Name = item.Fullname;
                myWallet.Pic = item.Picture;
                myWallet.RegNo = item.UTMENumber;
                myWallet.Level = item.Level ;
                myWallet.Department = item.Department;
                myWallet.CreditBalance = 0;
                myWallet.WalletId = item.UTMENumber;
                myWallet.DateCreated = DateTime.Now;
                myWallet.Status = true;
                _context.Add(myWallet);
                await _context.SaveChangesAsync();
                //////////////////////////////////

                //FirstOrDefaultAsync works when the id coming in is not of type int.
                //FindAsync works when the id coming in and the one being compared with is also int.

                if (myWallet.WalletId == null)
                {
                    return NotFound();
                }

                var WalletToUpdate = await _context.UgMainWallets
                .FirstOrDefaultAsync(c => c.WalletId == myWallet.WalletId);

                WalletToUpdate.BulkDebitBalanace = myWallet.Debit;
                WalletToUpdate.Status = true;
                WalletToUpdate.DateUpdated = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles ="student, superAdmin")]
        // GET: students
        public async Task<IActionResult> Index()
        {
            var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);
            var userId = loggedInUser.StudentsId;
            var student = (from c in _context.Students where c.Id == userId select c).Include(i => i.Departments).FirstOrDefault();
            var wallet = (from c in _context.UgSubWallets where c.WalletId == student.UTMENumber select c).FirstOrDefault();
            ViewBag.Name = student.Fullname;
            ViewBag.Department = student.Departments.Name;
            var approvedCourses = (from c in _context.CourseRegistrations
                                   where c.StudentId == userId &&
                            c.Status == MainStatus.Approved &&
                            c.SessionId == student.CurrentSession
                                   select c).Include(c => c.Courses).ToList();
            var timetable = (from c in _context.TimeTables where c.DepartmetId == student.Department && c.LevelId ==
                             student.Level
                                   select c).Include(c => c.Courses).ThenInclude(s => s.Courses).ToList();
            
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

            var model = new StudentDashboardVM
            {
                SubWallet = wallet,
                Courses = approvedCourses,
                TimeTables = timetable
            };

            return View(model);
           
        }
        [Authorize(Roles = "superAdmin")]
        public async Task<IActionResult> Allstudents()
        {
            ViewBag.err = TempData["err"];
            var applicationDbContext = _context.Students.Where(x => x.StudentStatus == 1).Include(s => s.Departments)
                .Include(s => s.Levels);
            return View(await applicationDbContext.ToListAsync());
           
        }
        [Authorize(Roles = "superAdmin")]
        public async Task<IActionResult> Graduated()
        {
            ViewBag.err = TempData["err"];
            var applicationDbContext = _context.Students.Where(x => x.StudentStatus == 2).Include(s => s.Departments)
                .Include(s => s.Levels);
            return View(await applicationDbContext.ToListAsync());
           
        }
        [Authorize(Roles = "superAdmin")]
        public async Task<IActionResult> Expelled()
        {
            ViewBag.err = TempData["err"];
            var applicationDbContext = _context.Students.Where(x => x.StudentStatus == 3).Include(s => s.Departments)
                .Include(s => s.Levels);
            return View(await applicationDbContext.ToListAsync());
           
        }
        [Authorize(Roles = "hod, superAdmin")]
        public async Task<IActionResult> MyStudents()
        {
            var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);
            var userId = loggedInUser.StaffId;
            var HOD = (from i in _context.Staffs where i.Id == userId select i).FirstOrDefault();
            var applicationDbContext = _context.Students.Where(i => i.Department == HOD.DepartmentId).Include(s => s.Departments)
                .Include(s => s.Levels);
            return View(await applicationDbContext.ToListAsync());

        }
        [Authorize(Roles = "superAdmin")]
        public async Task<IActionResult> UpdateStudents()
        {
            return _context.Departments != null ?
                           View(await _context.Departments.ToListAsync()) :
                           Problem("Entity set 'ApplicationDbContext.Students'  is null.");
        }
        //This module updates students session and level 
        //and it is done based on department
        [Authorize(Roles = "superAdmin")]
        public async Task<IActionResult> UpdateStudentsLevel(string id, UgSubWallet myWallet)
        {
            
            try
            {
                var students = (from s in _context.Students select s).ToList();
                foreach (var item in students)
                {
                    //var StudentId = item.Id;
                    var Session = (from s in _context.Sessions where s.IsActive == true select s).FirstOrDefault();
                    
                        item.CurrentSession = Session.Id;
                        var level = item.Level + 1;
                        item.Level = level;
                        await _context.SaveChangesAsync();

                    //if (item.Level == 1)
                    //{
                    //    var TuitionFee = (from t in _context.Fees
                    //                      where t.DepartmentId == item.Department
                    //                      select t.Level1).Sum();
                    //    myWallet.Tuition = TuitionFee;
                    //}
                    //else if (item.Level == 2)
                    //{
                    //    var TuitionFee = (from t in _context.Fees
                    //                      where t.DepartmentId == item.Department
                    //                      select t.Level2).Sum();
                    //    myWallet.Tuition = TuitionFee;
                    //}
                    //else if (item.Level == 3)
                    //{
                    //    var TuitionFee = (from t in _context.Fees
                    //                      where t.DepartmentId == item.Department
                    //                      select t.Level3).Sum();
                    //    myWallet.Tuition = TuitionFee;
                    //}
                    //else if (item.Level == 4)
                    //{
                    //    var TuitionFee = (from t in _context.Fees
                    //                      where t.DepartmentId == item.Department
                    //                      select t.Level4).Sum();
                    //    myWallet.Tuition = TuitionFee;
                    //}
                    //else if (item.Level == 5)
                    //{
                    //    var TuitionFee = (from t in _context.Fees
                    //                      where t.DepartmentId == item.Department
                    //                      select t.Level5).Sum();
                    //    myWallet.Tuition = TuitionFee;
                    //}
                    //else if (item.Level == 6)
                    //{
                    //    var TuitionFee = (from t in _context.Fees
                    //                      where t.DepartmentId == item.Department
                    //                      select t.Level6).Sum();
                    //    myWallet.Tuition = TuitionFee;
                    //}
                    //myWallet.StudentId = item.Id;
                    //myWallet.Name = item.Fullname;
                    //myWallet.Department = item.Department;
                    //myWallet.RegNo = item.UTMENumber;
                    //myWallet.EDHIS = (decimal)20000.00;
                    //myWallet.LMS = (decimal)25000.00;
                    //myWallet.SRC = (decimal)2500.00;
                    //myWallet.Debit = myWallet.Tuition + myWallet.Tuition2 + myWallet.LMS + myWallet.SRC;
                    //myWallet.FortyPercent = myWallet.Tuition * 40 / 100;
                    //myWallet.SixtyPercent = myWallet.Tuition * 60 / 100;
                    //myWallet.CreditBalance = 0;
                    //myWallet.WalletId = item.UTMENumber;
                    //myWallet.DateCreated = DateTime.Now;
                    //myWallet.Status = true;
                    //myWallet.Level = item.Level;
                    //myWallet.Pic = item.Picture;
                    //myWallet.ApplicantId = item.Id;
                    //Next line updates the bulk wallet
                    //var bulkwallet = await _context.MainWallets.FirstOrDefaultAsync(i => i.WalletId == myWallet.WalletId);
                    //var newBulkDebit = bulkwallet.BulkDebitBalanace + myWallet.Debit;
                    //bulkwallet.BulkDebitBalanace = newBulkDebit;

                    //_context.Add(myWallet);
                    //await _context.SaveChangesAsync();
                }

                return RedirectToAction("Index", "Students");

            }
            catch (Exception ex)
            {
                ex.ToString();

            }
            return View();
        }
        [Authorize(Roles = "superAdmin")]
        //Graduate Student by changing their status
        public async Task<IActionResult> Graduate(string id, Student student)
        {
            try
            {
                var graduate = _context.Students.FirstOrDefault(x => x.SchoolEmailAddress == id);
                if (graduate == null)
                {
                    TempData["err"]= "Student Record not found!";
                    return RedirectToAction(nameof(Allstudents));
                }
                graduate.StudentStatus = 2;
                _context.Students.Update(graduate);
                await _context.SaveChangesAsync();
                ViewBag.success = "Student has been graduated successfully";
                return RedirectToAction("details", "students", new {id});

            }
            catch (Exception)
            {
                return RedirectToAction("badreq", "error");
                throw;
            }
        }
        //Graduate Student by changing their status
        [Authorize(Roles = "superAdmin")]
        public async Task<IActionResult> Return(string id, Student student)
        {
            try
            {
                var graduate = _context.Students.FirstOrDefault(x => x.SchoolEmailAddress == id);
                if (graduate == null)
                {
                    TempData["err"]= "Student Record not found!";
                    return RedirectToAction(nameof(Allstudents));
                }
                graduate.StudentStatus = 1;
                _context.Students.Update(graduate);
                await _context.SaveChangesAsync();
                return RedirectToAction("allstudents", "students");

            }
            catch (Exception)
            {
                return RedirectToAction("badreq", "error");
                throw;
            }
        }
        [Authorize(Roles = "student, superAdmin")]
        // GET: students/Details/5
        public async Task<IActionResult> Profile()
        {
            var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);
            var userId = loggedInUser.StudentsId;

            var student = (from s in _context.Students where s.Id == userId select s)    
                .Include(s => s.Departments)
                .Include(s => s.Faculties)
                .Include(s => s.LGAs)
                .Include(s => s.Levels)
                .Include(s => s.Nationalities)
                .Include(s => s.States)
                .Include(s => s.Sessions)
                .FirstOrDefault();
            //Console.WriteLine(student.Fullname);
            return View(student);
        }
        [Authorize(Roles = "student, superAdmin")]
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Departments)
                .Include(s => s.Faculties)
                .Include(s => s.LGAs)
                .Include(s => s.Levels)
                .Include(s => s.Nationalities)
                .Include(s => s.Sessions)
                .Include(s => s.States)
                .FirstOrDefaultAsync(m => m.SchoolEmailAddress == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }
        [Authorize(Roles = "superAdmin")]
        // GET: students/Create
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name");
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "Id", "Name");
            ViewData["LGAId"] = new SelectList(_context.Lgas, "Id", "Name");
            ViewData["Level"] = new SelectList(_context.Levels, "Id", "Name");
            ViewData["NationalityId"] = new SelectList(_context.Countries, "Id", "Name");
            ViewData["CurrentSession"] = new SelectList(_context.Sessions, "Id", "Name");
            ViewData["StateOfOriginId"] = new SelectList(_context.States, "Id", "Name");
            return View();
        }
        [Authorize(Roles = "superAdmin")]
        // POST: students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
           
            _context.Add(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
           
            ViewData["Department"] = new SelectList(_context.Departments, "Id", "Id", student.Department);
            ViewData["Faculty"] = new SelectList(_context.Faculties, "Id", "Id", student.Faculty);
            ViewData["LGAId"] = new SelectList(_context.Lgas, "Id", "Id", student.LGAId);
            ViewData["Level"] = new SelectList(_context.Levels, "Id", "Id", student.Level);
            ViewData["NationalityId"] = new SelectList(_context.Countries, "Id", "Id", student.NationalityId);
            ViewData["CurrentSession"] = new SelectList(_context.Sessions, "Id", "Id", student.CurrentSession);
            ViewData["StateOfOriginId"] = new SelectList(_context.States, "Id", "Id", student.StateOfOriginId);
            return View(student);
        }
        [Authorize(Roles = "superAdmin, student")]
        // GET: students/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = (from c in _context.Students where c.Id == id select c).FirstOrDefault();
            if (student == null)
            {
                return NotFound();
            }
            ViewData["Department"] = new SelectList(_context.Departments, "Id", "Name", student.Department);
            ViewData["Faculty"] = new SelectList(_context.Faculties, "Id", "Name", student.Faculty);
            ViewData["LGAId"] = new SelectList(_context.Lgas, "Id", "Name", student.LGAId);
            ViewData["Level"] = new SelectList(_context.Levels, "Id", "Name", student.Level);
            ViewData["NationalityId"] = new SelectList(_context.Countries, "Id", "Name", student.NationalityId);
            ViewData["CurrentSession"] = new SelectList(_context.Sessions, "Id", "Name", student.CurrentSession);
            ViewData["StateOfOriginId"] = new SelectList(_context.States, "Id", "Name", student.StateOfOriginId);
            return PartialView("_editPartial",student);
        }

        // POST: students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Student student)
        {
            // Check if the provided ID matches the student ID
            if (id != student.Id)
            {
                return NotFound();
            }

            try
            {
                // Retrieve the student from the database
                var studentToUpdate = await _context.Students
                    .FirstOrDefaultAsync(c => c.Id == id);

                // Update the model with the specified properties
                if (await TryUpdateModelAsync(studentToUpdate, "",
                    c => c.Fullname, c => c.DOB, c => c.Sex, c => c.Religion, c => c.Phone,
                    c => c.AltPhoneNumber, c => c.Email, c => c.NationalityId, c => c.StateOfOriginId, c => c.LGAId,
                    c => c.PlaceOfBirth, c => c.ContactAddress, c => c.PermanentHomeAddress, c => c.MaritalStatus, c => c.ParentName,
                    c => c.ParentOccupation, c => c.ParentPhone, c => c.ParentAltPhone, c => c.ParentEmail, c => c.ParentAddress,
                    c => c.SchoolEmailAddress, c => c.UTMENumber, c => c.MatNumber, c => c.Faculty, c => c.Level,
                    c => c.ModeOfAdmission, c => c.YearOfAdmission, c => c.Department, c => c.CurrentSession, c => c.IsStillAStudent,
                    c => c.ProgrameId, c => c.StudentStatus))
                    {
                    // Set additional properties and save changes
                    studentToUpdate.Cleared = true;

                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Profile));
                }
                else
                {
                    ModelState.AddModelError("", "Invalid model data. Please check your inputs.");
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                // Log concurrency exception and add error to ModelState
               // _logger.LogError("Concurrency error while updating student with ID {StudentId}", id);
                ModelState.AddModelError("", "Unable to save changes due to a concurrency conflict. Please refresh and try again.");
            }
            catch (Exception ex)
            {
                // Log other exceptions
               // _logger.LogError(ex, "An error occurred while updating the student with ID {StudentId}", id);
                ModelState.AddModelError("", "An unexpected error occurred. Please try again or contact your system administrator.");
            }

            // Populate ViewData for dropdowns
            ViewData["Department"] = new SelectList(_context.Departments, "Id", "Id", student.Department);
            ViewData["Faculty"] = new SelectList(_context.Faculties, "Id", "Id", student.Faculty);
            ViewData["LGAId"] = new SelectList(_context.Lgas, "Id", "Id", student.LGAId);
            ViewData["Level"] = new SelectList(_context.Levels, "Id", "Id", student.Level);
            ViewData["NationalityId"] = new SelectList(_context.Countries, "Id", "Id", student.NationalityId);
            ViewData["CurrentSession"] = new SelectList(_context.Sessions, "Id", "Id", student.CurrentSession);
            ViewData["StateOfOriginId"] = new SelectList(_context.States, "Id", "Id", student.StateOfOriginId);

            // Return to the "profile" action
            return RedirectToAction("profile");
        }

        [Authorize(Roles = "student, superAdmin")]
        public async Task<IActionResult> Upload(IFormFile passport, int id)
        {
            try
            {
                var student = await _context.Students.FindAsync(id);
                if (student == null)
                {
                    return NotFound(); // Handle the case when the student is not found
                }

                if (passport != null && passport.Length > 0)
                {
                    var uploadDir = @"files/applicantUploads/passports";
                    var fileName = $"{student.UTMENumber}{Path.GetExtension(passport.FileName)}";
                    var webRootPath = _hostingEnvironment.WebRootPath;
                    var path = Path.Combine(webRootPath, uploadDir, fileName);

                    using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
                    {
                        await passport.CopyToAsync(fs);
                        student.Picture = fileName;
                    }

                    using (var transaction = _context.Database.BeginTransaction())
                    {
                        try
                        {
                            _context.Update(student);
                            await _context.SaveChangesAsync();
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            // Log the exception
                            TempData["err"] = 
                            $"Error updating database: {ex.Message}";
                            return RedirectToAction("badreq", "error");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                TempData["err"] =
                            $"Error updating database: {ex.Message}";
                return RedirectToAction("badreq", "error");
            }

            return RedirectToAction(nameof(Profile));
        }

        [Authorize(Roles = "superAdmin")]
        // GET: students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Departments)
                .Include(s => s.Faculties)
                .Include(s => s.LGAs)
                .Include(s => s.Levels)
                .Include(s => s.Nationalities)
                .Include(s => s.Sessions)
                .Include(s => s.States)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            
            if (_context.Students == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Students'  is null.");
            }
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
       
        public async Task<IActionResult> Dashboard()
        {
            return View();
        }
        private bool StudentExists(int? id)
        {
          return _context.Students.Any(e => e.Id == id);
        }
    }
}
