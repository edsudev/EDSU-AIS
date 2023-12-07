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
using PgProgram = EDSU_SYSTEM.Models.PgProgram;

namespace EDSU_SMS.Controllers
{
    public class PgApplicantsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        public PgApplicantsController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment, RoleManager<IdentityRole> roleManager,
                                        UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        public async Task<IActionResult> PopulateWallet(PgSubWallet subWallet, PgMainWallet pgmain)
        {
            var students = (from s in _context.PgApplicants select s).ToList();

            foreach (var st in students)
            {
                try
                {
                    var newPgMain = new PgMainWallet();

                    //ugmain.Id = int.Parse(p);
                    newPgMain.Name = st.Surname + " " + st.FirstName + " " + st.OtherName;
                    newPgMain.WalletId = "EDSU-PG-23-"+st.id;
                    newPgMain.BulkDebitBalanace = 0;
                    newPgMain.CreditBalance = 0;
                    newPgMain.Status = true;
                    newPgMain.DateCreated = DateTime.Now;
                    _context.PgMainWallets.Add(newPgMain);
                    await _context.SaveChangesAsync();
                    Random r = new();

                    // Create a new instance of UgSubWallet for each student
                    var newSubWallet = new PgSubWallet();
                    newSubWallet.WalletId = "EDSU-PG-23-" + st.id;
                    newSubWallet.Name = st.Surname + " " + st.FirstName + " " + st.OtherName;
                    newSubWallet.RegNo = "EDSU-PG-23-" + st.id;
                    newSubWallet.CreditBalance = 0;
                    newSubWallet.Status = true;
                    newSubWallet.DateCreated = DateTime.Now;
                    newSubWallet.Tuition = 300000;
                    newSubWallet.AcceptanceFee = 100000;
                    newSubWallet.FortyPercent = newSubWallet.Tuition * 40 / 100;
                    newSubWallet.SixtyPercent = newSubWallet.Tuition * 60 / 100;
                    newSubWallet.LMS = 50000;
                    newSubWallet.SessionId = 9;
                    newSubWallet.Debit = newSubWallet.Tuition + newSubWallet.LMS + newSubWallet.AcceptanceFee;
                    newSubWallet.Level = 8;
                    newSubWallet.Department = st.AdmittedInto;

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

        public FileResult DownloadFile(string FileName)
        {
            string path = Path.Combine(this._hostingEnvironment.WebRootPath, "PgApplicants/passport/") + FileName;
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            return File(bytes, "application/octet-stream", FileName);
        }
        public FileResult DownloadBirthCertificate(string FileName)
        {
            string path = Path.Combine(this._hostingEnvironment.WebRootPath, "PgApplicants/birthCertificate/") + FileName;
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            return File(bytes, "application/octet-stream", FileName);
        }
        public FileResult DownloadDegree(string FileName)
        {
            string path = Path.Combine(this._hostingEnvironment.WebRootPath, "PgApplicants/degree/") + FileName;
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            return File(bytes, "application/octet-stream", FileName);
        }
        public FileResult DownloadLga(string FileName)
        {
            string path = Path.Combine(this._hostingEnvironment.WebRootPath, "PgApplicants/lga/") + FileName;
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            return File(bytes, "application/octet-stream", FileName);
        }
        public FileResult DownloadNYSC(string FileName)
        {
            string path = Path.Combine(this._hostingEnvironment.WebRootPath, "PgApplicants/NYSC/") + FileName;
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            return File(bytes, "application/octet-stream", FileName);
        }
        public FileResult DownloadRef1(string FileName)
        {
            string path = Path.Combine(this._hostingEnvironment.WebRootPath, "PgApplicants/ref1/") + FileName;
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            return File(bytes, "application/octet-stream", FileName);
        }
        public FileResult DownloadRef2(string FileName)
        {
            string path = Path.Combine(this._hostingEnvironment.WebRootPath, "PgApplicants/ref2/") + FileName;
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            return File(bytes, "application/octet-stream", FileName);
        }
        public FileResult DownloadRef3(string FileName)
        {
            string path = Path.Combine(this._hostingEnvironment.WebRootPath, "PgApplicants/ref3/") + FileName;
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            return File(bytes, "application/octet-stream", FileName);
        }
        public FileResult DownloadSsce1(string FileName)
        {
            string path = Path.Combine(this._hostingEnvironment.WebRootPath, "PgApplicants/ssce1/") + FileName;
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            return File(bytes, "application/octet-stream", FileName);
        }
        public FileResult DownloadSsce2(string FileName)
        {
            string path = Path.Combine(this._hostingEnvironment.WebRootPath, "PgApplicants/ssce2/") + FileName;
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            return File(bytes, "application/octet-stream", FileName);
        }

        [AllowAnonymous]
        // GET: PgApplicants
        public async Task<IActionResult> Index()
        {
            var applicants = from i in _context.PgApplicants
                             select i;

            return View(applicants);
        }

        public async Task<IActionResult> LinkToSecondForm()
        {
            var applicants = from i in _context.PgApplicants
                             select i;

            return View(applicants);
        }
        // GET: PgApplicants/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.PgApplicants == null)
            {
                return NotFound();
            }

            var applicant = await _context.PgApplicants
                .FirstOrDefaultAsync(m => m.id == id);
            if (applicant == null)
            {
                return NotFound();
            }

            var applicants = from i in _context.PgApplicants
                             select i;

            return View(applicant);
        }

        //Post: Applicant/Detail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(int? id, MainStatus status)
        {
            var applicant = await _context.PgApplicants.FindAsync(id);

            if (applicant == null)
                return NotFound();

            applicant.Status = status;
            _context.PgApplicants.Update(applicant);
            //Context.Students.Add((Student)applicant);

            await _context.SaveChangesAsync();

            return View(applicant);
        }
        // GET: PgApplicants/Create
        public async Task<IActionResult> Create()
        {

            //Generating the list of Countries States and LGA
            List<Countries> countries = await _context.Countries.OrderBy(c => c.Name).ToListAsync();
            ViewBag.Country = countries;
            List<States> states = await _context.States.OrderBy(c => c.Name).ToListAsync();
            ViewBag.State = states;
            List<Lga> lgasList = await _context.Lgas.OrderBy(x => x.Name).ToListAsync();

            ViewBag.LGA = lgasList;
            
            return View();
        }

        // POST: PgApplicants/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,UserId,Surname,FirstName,OtherName,Sex,DOB,MaritalStatus,PlaceOfBirth,ContactAddress,PermanentHomeAddress,Nationality,StateOfOrigin,LGA,PhoneNumber,PhoneNumber2,Email,PrimarySchool,SecondarySchool,ProgramApplyingFor,PreviousInstitution,CurrentQualification,ClassOfDegree,YearGraduated,PercentageOfResult,AreaOfSpecialization,EmploymentHistory,ResearchExperience,Status,NoOfSittings,Ssce1Type,Ssce1Year,Ssce1Number,Ssce1Subject1,Ssce1Subject2,Ssce1Subject3,Ssce1Subject4,Ssce1Subject5,Ssce1Subject6,Ssce1Subject7,Ssce1Subject8,Ssce1Subject9,Ssce1Subject1Grade,Ssce1Subject2Grade,Ssce1Subject3Grade,Ssce1Subject4Grade,Ssce1Subject5Grade,Ssce1Subject6Grade,Ssce1Subject7Grade,Ssce1Subject8Grade,Ssce1Subject9Grade,Ssce2Type,Ssce2Year,Ssce2Number,Ssce2Subject1,Ssce2Subject2,Ssce2Subject3,Ssce2Subject4,Ssce2Subject5,Ssce2Subject6,Ssce2Subject7,Ssce2Subject8,Ssce2Subject9,Ssce2Subject1Grade,Ssce2Subject2Grade,Ssce2Subject3Grade,Ssce2Subject4Grade,Ssce2Subject5Grade,Ssce2Subject6Grade,Ssce2Subject7Grade,Ssce2Subject8Grade,Ssce2Subject9Grade,FullNameOfNextOfKin,AddressOfNextOfKin,PhoneNumberOfNextOfKin,AlternatePhoneNumberOfNextOfKin,EmailOfNextOfKin,OccupationOfNextOfKin,Passport,HigherDegrees,Ssce1,Ssce2,BirthCertificate,DirectEntryUpload,LGAUpload,NYSC,Ref1,Ref2,Ref3")] PgApplicant pgApplicant)
        {
            //var currentUser = UserManager.GetUserId(User);
            //List<PgApplicant> applicants = await _context.PgApplicants
            //    .Where(i => i.UserId == currentUser).ToListAsync();
            //if (applicants.Any())
            //    return Forbid();
            //pgApplicant.UserId = currentUser;
            //var isAuthorized = await AuthorizationService.AuthorizeAsync(User, pgApplicant, ApplicationOperations.Create);
            //if (isAuthorized.Succeeded == false)
            //    return Forbid();

            _context.PgApplicants.Add(pgApplicant);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(LinkToSecondForm));
        }

        // GET: PgApplicants/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.PgApplicants == null)
            {
                return NotFound();
            }
            //List<PgProgram> programs = await Context.PgPrograms.OrderBy(c => c.NameOfProgram).ToListAsync();
            //ViewBag.Program = programs;
            //var applicants = await _context.PgApplicants.FindAsync(id);
            //if (applicants == null)
            //{
            //    return NotFound();
            //}
            //var isAuthorized = await AuthorizationService.AuthorizeAsync(User, applicants, ApplicationOperations.Update);
            //if (isAuthorized.Succeeded == false)
            //    return Forbid();

            //List<Course> course = new();
            //course = (from c in Context.Courses select c).ToList();
            //ViewBag.message1 = course;
            //List<Department> Dept = new();
            //Dept = (from c in Context.Departments select c).ToList();
            //ViewBag.message2 = Dept;
            ////Storing Years in a view Bag for front end display
            //var currentYear = DateTime.Now.Year;
            //List<string> yearList = new List<string>();
            //for (var i = currentYear - 50; i <= currentYear; i++)
            //{
            //    yearList.Add(i.ToString());
            //}

            //ViewBag.message3 = yearList.ToList();

            //if (applicants.Status.ToString() != "Pending")
            //    return Forbid();

            return View();
        }

        // POST: PgApplicants/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,UserId,Surname,FirstName,OtherName,Sex,DOB,MaritalStatus,PlaceOfBirth,ContactAddress,PermanentHomeAddress,Nationality,StateOfOrigin,LGA,PhoneNumber,PhoneNumber2,Email,PrimarySchool,SecondarySchool,ProgramApplyingFor,PreviousInstitution,CurrentQualification,ClassOfDegree,YearGraduated,PercentageOfResult,AreaOfSpecialization,EmploymentHistory,ResearchExperience,Status,NoOfSittings,Ssce1Type,Ssce1Year,Ssce1Number,Ssce1Subject1,Ssce1Subject2,Ssce1Subject3,Ssce1Subject4,Ssce1Subject5,Ssce1Subject6,Ssce1Subject7,Ssce1Subject8,Ssce1Subject9,Ssce1Subject1Grade,Ssce1Subject2Grade,Ssce1Subject3Grade,Ssce1Subject4Grade,Ssce1Subject5Grade,Ssce1Subject6Grade,Ssce1Subject7Grade,Ssce1Subject8Grade,Ssce1Subject9Grade,Ssce2Type,Ssce2Year,Ssce2Number,Ssce2Subject1,Ssce2Subject2,Ssce2Subject3,Ssce2Subject4,Ssce2Subject5,Ssce2Subject6,Ssce2Subject7,Ssce2Subject8,Ssce2Subject9,Ssce2Subject1Grade,Ssce2Subject2Grade,Ssce2Subject3Grade,Ssce2Subject4Grade,Ssce2Subject5Grade,Ssce2Subject6Grade,Ssce2Subject7Grade,Ssce2Subject8Grade,Ssce2Subject9Grade,FullNameOfNextOfKin,AddressOfNextOfKin,PhoneNumberOfNextOfKin,AlternatePhoneNumberOfNextOfKin,EmailOfNextOfKin,OccupationOfNextOfKin,Passport,HigherDegrees,Ssce1,Ssce2,BirthCertificate,DirectEntryUpload,LGAUpload,NYSC,Ref1,Ref2,Ref3")] PgApplicant pgApplicant)
        {
            var applicants = await _context.PgApplicants.FindAsync(id);

            if (id == null)
            {
                return NotFound();
            }
            
            try
            {
              
                var ApplicatantToUpdate = await _context.PgApplicants
                .FirstOrDefaultAsync(c => c.id == id);

                if (await TryUpdateModelAsync<PgApplicant>(ApplicatantToUpdate, "", c => c.ProgramApplyingFor, c => c.PreviousInstitution,
                    c => c.CurrentQualification, c => c.ClassOfDegree, c => c.YearGraduated, c => c.PercentageOfResult,
                    c => c.AreaOfSpecialization, c => c.EmploymentHistory, c => c.ResearchExperience))
                {
                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {

                        ModelState.AddModelError("", "Unable to save changes. " +
                            "Try again, and if the problem persists, " +
                            "see your system administrator.");
                    }
                    return RedirectToAction("Step33", "PgApplicants", new { id });
                }



                //Context.Update(applicant);
                //await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ex.ToString();

            }
            return View();

        }

        // GET: Applicants/Edit/5
        public async Task<IActionResult> Step33(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicants = await _context.PgApplicants.FindAsync(id);
            if (applicants == null)
            {
                return NotFound();
            }

            //Storing Years in a viewBag for front end display
            var currentYear = DateTime.Now.Year;
            List<string> yearList = new List<string>();
            for (var i = currentYear - 50; i <= currentYear; i++)
            {
                yearList.Add(i.ToString());
            }

            ViewBag.Year = yearList.ToList();
            
            List<SsceSubjects> Ssce = new();
            Ssce = (from c in _context.SsceSubjects select c).ToList();
            ViewBag.message3 = Ssce;
            List<SSCEGrade> grade = new();
            grade = (from c in _context.SSCEGrades select c).ToList();
            ViewBag.message4 = grade;

            if (applicants.Status.ToString() != "Pending")
                return Forbid();

            return View(applicants);
        }

        // POST: Applicants/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Step33(int id)
        {
            var applicants = await _context.PgApplicants.FindAsync(id);

            if (id == null)
            {
                return NotFound();
            }
            
            try
            {
                
                var ApplicatantToUpdate = await _context.PgApplicants
                .FirstOrDefaultAsync(c => c.id == id);

                if (await TryUpdateModelAsync<PgApplicant>(ApplicatantToUpdate, "", c => c.NoOfSittings, c => c.Ssce1Type,
                    c => c.Ssce1Year, c => c.Ssce1Number, c => c.Ssce1Subject1, c => c.Ssce1Subject1Grade, c => c.Ssce1Subject2,
                    c => c.Ssce1Subject2Grade, c => c.Ssce1Subject3, c => c.Ssce1Subject3Grade, c => c.Ssce1Subject4,
                    c => c.Ssce1Subject4Grade, c => c.Ssce1Subject5, c => c.Ssce1Subject5Grade, c => c.Ssce1Subject6,
                    c => c.Ssce1Subject6Grade, c => c.Ssce1Subject7, c => c.Ssce1Subject7Grade, c => c.Ssce1Subject8,
                    c => c.Ssce1Subject8Grade, c => c.Ssce1Subject9, c => c.Ssce1Subject9Grade, c => c.Ssce2Type,
                    c => c.Ssce2Year, c => c.Ssce2Number, c => c.Ssce2Subject1, c => c.Ssce2Subject1Grade, c => c.Ssce2Subject2,
                    c => c.Ssce2Subject2Grade, c => c.Ssce2Subject3, c => c.Ssce2Subject3Grade, c => c.Ssce2Subject4,
                    c => c.Ssce2Subject4Grade, c => c.Ssce2Subject5, c => c.Ssce2Subject5Grade, c => c.Ssce2Subject6,
                    c => c.Ssce2Subject6Grade, c => c.Ssce2Subject7, c => c.Ssce2Subject7Grade, c => c.Ssce2Subject8,
                    c => c.Ssce2Subject8Grade, c => c.Ssce2Subject9, c => c.Ssce2Subject9Grade))
                {
                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {

                        //Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "Unable to save changes. " +
                            "Try again, and if the problem persists, " +
                            "see your system administrator.");
                    }
                    return RedirectToAction("Step3", "PgApplicants", new { id });
                }



                //Context.Update(applicant);
                //await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ex.ToString();

            }
            return View();

        }


        // GET: Applicants/Edit/Parent Guardian Information Page
        public async Task<IActionResult> Step3(int? id)
        {
            if (id == null || _context.PgApplicants == null)
            {
                return NotFound();
            }

            var applicant = await _context.PgApplicants.FindAsync(id);
            if (applicant == null)
            {
                return NotFound();
            }
            return View(applicant);
        }

        // POST: Applicants1/Edit/Parent Guardian Information Page
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Step3(int id)
        {
            var applicants = await _context.PgApplicants.FindAsync(id);

            if (id == null)
            {
                return NotFound();
            }
           
            try
            {
               
                var ApplicatantToUpdate = await _context.PgApplicants
                .FirstOrDefaultAsync(c => c.id == id);

                if (await TryUpdateModelAsync<PgApplicant>(ApplicatantToUpdate, "", c => c.FullNameOfNextOfKin, c => c.AddressOfNextOfKin,
                    c => c.PhoneNumber, c => c.AlternatePhoneNumberOfNextOfKin, c => c.EmailOfNextOfKin, c => c.OccupationOfNextOfKin))
                {
                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {

                        //Log the error (uncomment ex variable name and write a log.)
                        ModelState.AddModelError("", "Unable to save changes. " +
                            "Try again, and if the problem persists, " +
                            "see your system administrator.");
                    }
                    return RedirectToAction("Step4", "PgApplicants", new { id });
                }

                //Context.Update(applicant);
                //await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ex.ToString();

            }
            return View();

        }

        // GET: Applicants/Edit/File Upload Page
        public async Task<IActionResult> Step4(int? id)
        {
            if (id == null || _context.PgApplicants == null)
            {
                return NotFound();
            }

            var applicant = await _context.PgApplicants.FindAsync(id);
            if (applicant == null)
            {
                return NotFound();
            }
            if (applicant.Passport != null)
            {
                ViewBag.passport = "True";
            }
            if (applicant.HigherDegrees != null)
            {
                ViewBag.Degrees = "True";
            }
            if (applicant.Ssce1 != null)
            {
                ViewBag.Ssce1 = "True";
            }
            if (applicant.Ssce2 != null)
            {
                ViewBag.Ssce2 = "True";
            }
            if (applicant.BirthCertificate != null)
            {
                ViewBag.BirthCertificate = "True";
            }
            if (applicant.LGAUpload != null)
            {
                ViewBag.LGA = "True";
            }
            if (applicant.NYSC != null)
            {
                ViewBag.NYSC = "True";
            }
            if (applicant.Ref1 != null)
            {
                ViewBag.Ref1 = "True";
            }
            if (applicant.Ref2 != null)
            {
                ViewBag.Ref2 = "True";
            }
            if (applicant.Ref3 != null)
            {
                ViewBag.Ref3 = "True";
            }
            return View(applicant);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Step4(int id, IFormFile passport, PgApplicant model)
        {
            var applicants = await _context.PgApplicants.FindAsync(id);

            // var applicants = await _context.PgApplicants.FindAsync(model.id);

            if (model.id == null)
            {
                return NotFound();
            }
           
            try
            {
               
                if (passport != null && passport.Length > 0)
                {
                    var uploadDir = @"PgApplicants/passport";
                    var fileName = Path.GetFileNameWithoutExtension(passport.FileName);
                    var extension = Path.GetExtension(passport.FileName);
                    var webRootPath = _hostingEnvironment.WebRootPath;
                    //fileName = DateTime.UtcNow.ToString("yymmssfff") + fileName + extension;

                    fileName = fileName + extension;
                    var path = Path.Combine(webRootPath, uploadDir, fileName);
                    using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
                    {
                        passport.CopyTo(fs);
                        applicants.Passport = fileName;
                    }

                }
                await TryUpdateModelAsync<PgApplicant>(applicants);               //    c => c.Ssce1, c => c.Ssce2, c => c.BirthCertificate, c => c.DirectEntryUpload, c => c.LGAUpload))

                try
                {
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {

                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            catch (Exception ex)
            {
                ex.ToString();

            }
            ViewBag.ID = id;
            //  return View();
            return RedirectToAction("Step4", "PgApplicants", new { model.id });

        }

        public async Task<IActionResult> Degree(int id, IFormFile degree, PgApplicant model)
        {
            var applicants = await _context.PgApplicants.FindAsync(model.id);

            if (model.id == null)
            {
                return NotFound();
            }
        
            try
            {
               
                if (degree != null && degree.Length > 0)
                {
                    var uploadDir = @"PgApplicants/degree";
                    var fileName = Path.GetFileNameWithoutExtension(degree.FileName);
                    var extension = Path.GetExtension(degree.FileName);
                    var webRootPath = _hostingEnvironment.WebRootPath;
                    //fileName = DateTime.UtcNow.ToString("yymmssfff") + fileName + extension;

                    fileName = fileName + extension;
                    var path = Path.Combine(webRootPath, uploadDir, fileName);
                    using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
                    {
                        degree.CopyTo(fs);
                        applicants.HigherDegrees = fileName;
                        //if (fs != null)
                        //{
                        //    fs.Close();
                        //    fs.Dispose();
                        //}
                    }

                }
                await TryUpdateModelAsync<PgApplicant>(applicants);               //    c => c.Ssce1, c => c.Ssce2, c => c.BirthCertificate, c => c.DirectEntryUpload, c => c.LGAUpload))

                try
                {
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {

                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            catch (Exception ex)
            {
                ex.ToString();

            }
            //  return View();
            return RedirectToAction("Step4", "PgApplicants", new { model.id });

        }

        public async Task<IActionResult> Ssce1(int id, IFormFile ssce1, PgApplicant model)
        {
            var applicants = await _context.PgApplicants.FindAsync(model.id);

            if (model.id == null)
            {
                return NotFound();
            }
            try
            {
               
                if (ssce1 != null && ssce1.Length > 0)
                {
                    var uploadDir = @"PgApplicants/ssce1";
                    var fileName = Path.GetFileNameWithoutExtension(ssce1.FileName);
                    var extension = Path.GetExtension(ssce1.FileName);
                    var webRootPath = _hostingEnvironment.WebRootPath;
                    //fileName = DateTime.UtcNow.ToString("yymmssfff") + fileName + extension;

                    fileName = fileName + extension;
                    var path = Path.Combine(webRootPath, uploadDir, fileName);
                    using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
                    {
                        ssce1.CopyTo(fs);
                        applicants.Ssce1 = fileName;
                    }

                }
                await TryUpdateModelAsync<PgApplicant>(applicants);               //    c => c.Ssce1, c => c.Ssce2, c => c.BirthCertificate, c => c.DirectEntryUpload, c => c.LGAUpload))

                try
                {
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {

                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            catch (Exception ex)
            {
                ex.ToString();

            }
            //  return View();
            return RedirectToAction("Step4", "PgApplicants", new { model.id });

        }

        public async Task<IActionResult> Ssce2(int id, IFormFile ssce2, PgApplicant model)
        {
            var applicants = await _context.PgApplicants.FindAsync(model.id);

            if (model.id == null)
            {
                return NotFound();
            }
            try
            {
                
                if (ssce2 != null && ssce2.Length > 0)
                {
                    var uploadDir = @"PgApplicants/ssce2";
                    var fileName = Path.GetFileNameWithoutExtension(ssce2.FileName);
                    var extension = Path.GetExtension(ssce2.FileName);
                    var webRootPath = _hostingEnvironment.WebRootPath;
                    //fileName = DateTime.UtcNow.ToString("yymmssfff") + fileName + extension;

                    fileName = fileName + extension;
                    var path = Path.Combine(webRootPath, uploadDir, fileName);
                    using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
                    {
                        ssce2.CopyTo(fs);
                        applicants.Ssce2 = fileName;

                    }

                }
                await TryUpdateModelAsync<PgApplicant>(applicants);               //    c => c.Ssce1, c => c.Ssce2, c => c.BirthCertificate, c => c.DirectEntryUpload, c => c.LGAUpload))

                try
                {
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {

                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            catch (Exception ex)
            {
                ex.ToString();

            }
            //  return View();
            return RedirectToAction("Step4", "PgApplicants", new { model.id });

        }

        public async Task<IActionResult> BirthCertificate(int id, IFormFile birthCertificate, PgApplicant model)
        {
            var applicants = await _context.PgApplicants.FindAsync(model.id);

            if (model.id == null)
            {
                return NotFound();
            }
            
            try
            {
              
                if (birthCertificate != null && birthCertificate.Length > 0)
                {
                    var uploadDir = @"PgApplicants/birthCertificate";
                    var fileName = Path.GetFileNameWithoutExtension(birthCertificate.FileName);
                    var extension = Path.GetExtension(birthCertificate.FileName);
                    var webRootPath = _hostingEnvironment.WebRootPath;
                    //fileName = DateTime.UtcNow.ToString("yymmssfff") + fileName + extension;

                    fileName = fileName + extension;
                    var path = Path.Combine(webRootPath, uploadDir, fileName);
                    using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
                    {
                        birthCertificate.CopyTo(fs);
                        applicants.BirthCertificate = fileName;
                    }

                }
                await TryUpdateModelAsync<PgApplicant>(applicants);               //    c => c.Ssce1, c => c.Ssce2, c => c.BirthCertificate, c => c.DirectEntryUpload, c => c.LGAUpload))

                try
                {
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {

                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            catch (Exception ex)
            {
                ex.ToString();

            }
            //  return View();
            return RedirectToAction("Step4", "PgApplicants", new { model.id });

        }

        public async Task<IActionResult> Lga(int id, IFormFile lga, PgApplicant model)
        {
            var applicants = await _context.PgApplicants.FindAsync(model.id);

            if (model.id == null)
            {
                return NotFound();
            }
            try
            {
                if (lga != null && lga.Length > 0)
                {
                    var uploadDir = @"PgApplicants/lga";
                    var fileName = Path.GetFileNameWithoutExtension(lga.FileName);
                    var extension = Path.GetExtension(lga.FileName);
                    var webRootPath = _hostingEnvironment.WebRootPath;
                    //fileName = DateTime.UtcNow.ToString("yymmssfff") + fileName + extension;

                    fileName = fileName + extension;
                    var path = Path.Combine(webRootPath, uploadDir, fileName);
                    using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
                    {
                        lga.CopyTo(fs);
                        applicants.LGAUpload = fileName;
                    }

                }
                await TryUpdateModelAsync<PgApplicant>(applicants);               //    c => c.Ssce1, c => c.Ssce2, c => c.BirthCertificate, c => c.DirectEntryUpload, c => c.LGAUpload))

                try
                {
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {

                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            catch (Exception ex)
            {
                ex.ToString();

            }
            //  return View();
            return RedirectToAction("Step4", "PgApplicants", new { model.id });

        }

        public async Task<IActionResult> NYSC(int id, IFormFile nYSC, PgApplicant model)
        {
            var applicants = await _context.PgApplicants.FindAsync(model.id);

            if (model.id == null)
            {
                return NotFound();
            }
            try
            {
                if (nYSC != null && nYSC.Length > 0)
                {
                    var uploadDir = @"PgApplicants/NYSC";
                    var fileName = Path.GetFileNameWithoutExtension(nYSC.FileName);
                    var extension = Path.GetExtension(nYSC.FileName);
                    var webRootPath = _hostingEnvironment.WebRootPath;
                    //fileName = DateTime.UtcNow.ToString("yymmssfff") + fileName + extension;

                    fileName = fileName + extension;
                    var path = Path.Combine(webRootPath, uploadDir, fileName);
                    using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
                    {
                        nYSC.CopyTo(fs);
                        applicants.NYSC = fileName;

                    }

                }
                await TryUpdateModelAsync<PgApplicant>(applicants);               //    c => c.Ssce1, c => c.Ssce2, c => c.BirthCertificate, c => c.DirectEntryUpload, c => c.LGAUpload))

                try
                {
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {

                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            catch (Exception ex)
            {
                ex.ToString();

            }
            //  return View();
            return RedirectToAction("Step4", "PgApplicants", new { model.id });

        }

        public async Task<IActionResult> Ref1(int id, IFormFile ref1, PgApplicant model)
        {
            var applicants = await _context.PgApplicants.FindAsync(model.id);

            if (model.id == null)
            {
                return NotFound();
            }
            try
            {
                if (ref1 != null && ref1.Length > 0)
                {
                    var uploadDir = @"PgApplicants/ref1";
                    var fileName = Path.GetFileNameWithoutExtension(ref1.FileName);
                    var extension = Path.GetExtension(ref1.FileName);
                    var webRootPath = _hostingEnvironment.WebRootPath;
                    //fileName = DateTime.UtcNow.ToString("yymmssfff") + fileName + extension;

                    fileName = fileName + extension;
                    var path = Path.Combine(webRootPath, uploadDir, fileName);
                    using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
                    {
                        ref1.CopyTo(fs);
                        applicants.Ref1 = fileName;

                    }

                }
                await TryUpdateModelAsync<PgApplicant>(applicants);    //    c => c.Ssce1, c => c.Ssce2, c => c.BirthCertificate, c => c.DirectEntryUpload, c => c.LGAUpload))

                try
                {
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {

                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            catch (Exception ex)
            {
                ex.ToString();

            }
            //  return View();
            return RedirectToAction("Step4", "PgApplicants", new { model.id });

        }

        public async Task<IActionResult> Ref2(int id, IFormFile ref2, PgApplicant model)
        {
            var applicants = await _context.PgApplicants.FindAsync(model.id);

            if (model.id == null)
            {
                return NotFound();
            }
            try
            {
                
                if (ref2 != null && ref2.Length > 0)
                {
                    var uploadDir = @"PgApplicants/ref2";
                    var fileName = Path.GetFileNameWithoutExtension(ref2.FileName);
                    var extension = Path.GetExtension(ref2.FileName);
                    var webRootPath = _hostingEnvironment.WebRootPath;
                    //fileName = DateTime.UtcNow.ToString("yymmssfff") + fileName + extension;

                    fileName = fileName + extension;
                    var path = Path.Combine(webRootPath, uploadDir, fileName);
                    using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
                    {
                        ref2.CopyTo(fs);
                        applicants.Ref2 = fileName;

                    }

                }
                await TryUpdateModelAsync<PgApplicant>(applicants);               //    c => c.Ssce1, c => c.Ssce2, c => c.BirthCertificate, c => c.DirectEntryUpload, c => c.LGAUpload))

                try
                {
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {

                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            catch (Exception ex)
            {
                ex.ToString();

            }
            //  return View();
            return RedirectToAction("Step4", "PgApplicants", new { model.id });

        }

        public async Task<IActionResult> Ref3(int id, IFormFile ref3, PgApplicant model)
        {
            var applicants = await _context.PgApplicants.FindAsync(model.id);

            if (model.id == null)
            {
                return NotFound();
            }
            try
            {
                
                if (ref3 != null && ref3.Length > 0)
                {
                    var uploadDir = @"PgApplicants/ref3";
                    var fileName = Path.GetFileNameWithoutExtension(ref3.FileName);
                    var extension = Path.GetExtension(ref3.FileName);
                    var webRootPath = _hostingEnvironment.WebRootPath;
                    //fileName = DateTime.UtcNow.ToString("yymmssfff") + fileName + extension;

                    fileName = fileName + extension;
                    var path = Path.Combine(webRootPath, uploadDir, fileName);
                    using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
                    {
                        ref3.CopyTo(fs);
                        applicants.Ref3 = fileName;
                    }

                }
                await TryUpdateModelAsync<PgApplicant>(applicants);               //    c => c.Ssce1, c => c.Ssce2, c => c.BirthCertificate, c => c.DirectEntryUpload, c => c.LGAUpload))

                try
                {
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {

                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            catch (Exception ex)
            {
                ex.ToString();

            }
            //  return View();
            return RedirectToAction("Step4", "PgApplicants", new { model.id });

        }

        // GET: PgApplicants/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PgApplicants == null)
            {
                return NotFound();
            }

            var pgApplicant = await _context.PgApplicants
                .FirstOrDefaultAsync(m => m.id == id);
            if (pgApplicant == null)
            {
                return NotFound();
            }

            return View(pgApplicant);
        }

        // POST: PgApplicants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.PgApplicants == null)
            {
                return Problem("Entity set 'ApplicationDb_context.PgApplicants'  is null.");
            }
            var pgApplicant = await _context.PgApplicants.FindAsync(id);
            if (pgApplicant != null)
            {
                _context.PgApplicants.Remove(pgApplicant);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PgApplicantExists(int id)
        {
            return _context.PgApplicants.Any(e => e.id == id);
        }
        public async Task<IActionResult> CreateMainWalletPG(PgSubWallet subWallet, PgMainWallet ugmain)
        {
            var students = (from s in _context.PgApplicants select s).ToList();

            foreach (var st in students)
            {
               // var walletExist = (from a in _context.UgMainWallets where st.Email == a.WalletId select a).FirstOrDefault();
                //if (walletExist == null)
                //{
                    try
                    {
                        var newUgMain = new PgMainWallet();
                        
                        newUgMain.Name = st.Surname + " " + st.FirstName + " " + st.OtherName;
                        newUgMain.WalletId = st.UserId;
                       
                        newUgMain.BulkDebitBalanace = 0;
                        newUgMain.CreditBalance = 0;
                        newUgMain.Status = true;
                       
                        newUgMain.DateCreated = DateTime.Now;
                        _context.PgMainWallets.Add(newUgMain);
                        await _context.SaveChangesAsync();

                        // Create a new instance of UgSubWallet for each student
                        var newSubWallet = new PgSubWallet();
                        var fee = (from tu in _context.Fees where tu.DepartmentId == st.AdmittedInto select tu).FirstOrDefault();
                        if (fee == null)
                        {
                            fee = new Fee { Level1 = 0 };
                        }
                        newSubWallet.Tuition = fee.Pgd;
                        newSubWallet.Level = st.LevelAdmittedTo;
                        newSubWallet.WalletId = st.UserId;
                        newSubWallet.Name = st.Surname + " " + st.FirstName + " " + st.OtherName;
                        newSubWallet.RegNo = st.Email;
                        newSubWallet.CreditBalance = 0;
                        newSubWallet.Status = true;
                        newSubWallet.DateCreated = DateTime.Now;

                        newSubWallet.FortyPercent = newSubWallet.Tuition * 40 / 100;
                        newSubWallet.SixtyPercent = newSubWallet.Tuition * 60 / 100;
                        newSubWallet.LMS = 50000;
                        newSubWallet.AcceptanceFee = 100000;
                        newSubWallet.SessionId = 9;


                        var f = newSubWallet.Tuition  + newSubWallet.LMS
                                             + newSubWallet.EDHIS + newSubWallet.SRC + newSubWallet.AcceptanceFee;
                       
                        newSubWallet.Debit = f;
                        newSubWallet.Department = st.AdmittedInto;

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
              //  }

            }

            return View();
        }

        //////////////////////////////////////////////////////////////
        ////////////////////TRANSACTION MODULES//////////////////////
        public async Task<IActionResult> Pay()
        {
            return View();
        }
         public async Task<IActionResult> Debts(string id)
         {
            var wallet = (from s in _context.PgSubWallets where s.WalletId == id select s).Include(c => c.Sessions).ToList();
            ViewBag.utme = id;
            if (!wallet.Any())
            {
                return RedirectToAction("pagenotfound", "error");
            }
            return View(wallet);

         }
            //Initiating Acceptance payment
        public async Task<IActionResult> Acceptance(string id, PgOrder payment)
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
            payment.Ref = "EDSU-PG-" + r.Next(10000000) + DateTime.Now.Millisecond;
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
         //[Authorize(Roles = "student, superAdmin")]
        //Initiating Tuition payment
        public async Task<IActionResult> Tuition(string id, PgOrder payment)
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
            payment.Ref = "EDSU-PG-" + r.Next(10000000) + DateTime.Now.Millisecond;
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
        //Initiating Tuition 60 Percent payment
        public async Task<IActionResult> Tuition60(string id, PgOrder payment)
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
            payment.Ref = "EDSU-PG-" + r.Next(10000000) + DateTime.Now.Millisecond;
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
        //Initiating Tuition 40 Percent payment
        public async Task<IActionResult> Tuition40(string id, PgOrder payment)
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
            payment.Ref = "EDSU-PG-" + r.Next(10000000) + DateTime.Now.Millisecond;
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

        //Initiating LMS payment
        public async Task<IActionResult> LMS(string id, PgOrder payment)
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
            payment.Ref = "EDSU-PG-" + r.Next(10000000) + DateTime.Now.Millisecond;
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEmail(string? Ref)
        {
            try
            {
                //var order = (from x in _context.Payments where x.Ref == Ref select x.Wallets.WalletId).FirstOrDefault();

                var PaymentToUpdate = _context.PgOrders
               .FirstOrDefault(c => c.Ref == Ref);
                var orderid = Ref;
                if (await TryUpdateModelAsync<PgOrder>(PaymentToUpdate, "", c => c.Email))
                {

                    try
                    {
                        await _context.SaveChangesAsync();

                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        ModelState.AddModelError("", "Unable to save changes. " +
                            "Try again, and if the problem persists, " +
                            "see your system administrator.");
                    }
                    return RedirectToAction("checkout", "pgapplicants", new { orderid });

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
            if (orderid == null || _context.PgOrders == null)
            {
                return NotFound();
            }
            if (paymentToGet == null)
            {
                return NotFound();
            }
            return View(paymentToGet);
        }
        public async Task<IActionResult> Others(string id, PgOrder payment)
        {
            //Using Viewbag to display list of other fees and session from their respective tables table.
            ViewData["otherFees"] = new SelectList(_context.OtherFees, "Id", "Name");

            var wallet = await _context.PgSubWallets
                .FirstOrDefaultAsync(m => m.WalletId == id);
            Random r = new();
            //Payment is created just before it returns the view

            ViewBag.Name = wallet.Name;
            payment.SessionId = wallet.SessionId;
            payment.WalletId = wallet.Id;
            payment.Status = "Pending";
            payment.Ref = "EDSU-PG-" + r.Next(10000000) + DateTime.Now.Millisecond;
            payment.PaymentDate = DateTime.Now;
            _context.PgOrders.Add(payment);
            await _context.SaveChangesAsync();
            //When the payment row is created, it stores the id in a tempdata then pass it to the verify endpoint and the post method to update record
            TempData["PaymentId"] = payment.Wallets.WalletId;
            TempData["WalletId"] = wallet.Id;

            return View(payment);
        }
        //Initiating Other Payments
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Others(string Ref)
        {
            try
            {
                var PaymentToUpdate = await _context.PgOrders.FirstOrDefaultAsync(x => x.Ref == Ref);
                //var OtherRef = Ref;

                if (await TryUpdateModelAsync<PgOrder>(PaymentToUpdate, "", c => c.Email, c => c.OtherFeesDesc))
                {
                    try
                    {
                        await _context.SaveChangesAsync();
                        var othersText = (from o in _context.OtherFees where o.Id == PaymentToUpdate.OtherFeesDesc select o.Amount).Sum();
                        PaymentToUpdate.Amount = (double?)othersText;
                        await _context.SaveChangesAsync();

                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        ModelState.AddModelError("", "Unable to save changes. " +
                            "Try again, and if the problem persists, " +
                            "see your system administrator.");
                    }
                    return RedirectToAction("otherscheckout", "pgapplicants", new { Ref });

                }
            }
            catch (Exception ex)
            {
                ex.ToString();

            }
            return View();

        }
        public async Task<IActionResult> OthersCheckout(string Ref)
        {
            var paymentToUpdate = _context.PgOrders.Where(i => i.Ref == Ref).FirstOrDefault();
            if (Ref == null || _context.PgOrders == null)
            {
                return NotFound();
            }
            if (paymentToUpdate == null)
            {
                return NotFound();
            }

            return View(paymentToUpdate);
        }

    }
}