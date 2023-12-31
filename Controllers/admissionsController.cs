﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EDSU_SYSTEM.Models;
using EDSU_SYSTEM.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using static EDSU_SYSTEM.Models.Enum;
using System.Collections;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Xml;

namespace EDSU_SYSTEM.Controllers
{
    public class AdmissionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        public AdmissionsController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment,RoleManager<IdentityRole> roleManager,
                                        UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
       


        [Authorize(Roles = "staff, superAdmin, ugAdmission")]
        // GET: admissions
        public async Task<IActionResult> Index()
        {
            var sessions = (from s in _context.Sessions select s).ToList();
            ViewBag.sessions = sessions;
            
            var applicationDbContext = _context.UgApplicants;
            return View(await applicationDbContext.ToListAsync());
         
        }
       // [Authorize(Roles = "ugApplicant, superAdmin")]
        public async Task<IActionResult> Debts(string id)
        {
            var wallet = (from s in _context.UgSubWallets where s.WalletId == id select s).Include(c => c.Sessions).ToList();
            ViewBag.utme = id;
            if (!wallet.Any())
            {
                return RedirectToAction("pagenotfound", "error");
            }
            return View(wallet);

        }

        public async Task<IActionResult> Undergraduate1()
        {
            ViewBag.err = TempData["err"];
            return View();
         
        }
        public IActionResult Scholarships()
        {
            string externalUrl = "https://old.edouniversity.edu.ng/scholarship";
            return Redirect(externalUrl);
        }
        [Authorize(Roles = "staff, superAdmin, ugAdmission")]
        public async Task<IActionResult> List(string? id)
        {
            ViewBag.currentSession = id;

            var sessions = (from s in _context.Sessions select s).ToList();
            ViewBag.sessions = sessions;
            
            var ListofApplicants = _context.UgApplicants.Where(i => i.YearOfAdmissions.Name == id).Include(i => i.YearOfAdmissions);
            return View(await ListofApplicants.ToListAsync());
     
        }
        public IActionResult Instructions()
        {
            return View();
        } 
        public IActionResult Requirements()
        {
            string externalUrl = "https://old.edouniversity.edu.ng/admissions/requirements";
            return Redirect(externalUrl);
        }
        public IActionResult FeesDetails()
        {
            return View();
        }
        public IActionResult Login()
        {
            string externalUrl = "https://old.edouniversity.edu.ng/admissions/instructions";
            return Redirect(externalUrl);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password, Applicant applicant)
        {
            var user = (from s in _context.UgApplicants where s.Email == email && s.Password == password select s).FirstOrDefault();
            
            if (user == null)
            {
                
                return RedirectToAction(nameof(Login));
            }
            else
            {
                var id = user.ApplicantId;
                return RedirectToAction("step1", "admissions", new {id});
            }
            //return RedirectToAction(nameof(Dashboard));
        }
        // GET: admissions/Details/5
        [Authorize(Roles = "superAdmin, ugAdmission")]
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null || _context.UgApplicants == null)
            {
                return NotFound();
            }

            var applicant = await _context.UgApplicants.Include(x => x.Nationalities).Include(x => x.States).Include(x => x.LGAs)
                .FirstOrDefaultAsync(m => m.ApplicantId == id);
            if (applicant == null)
            {
                return NotFound();
            }

            return View(applicant);
        }

        // GET: admissions/Create
        public IActionResult Register()
        {
            string externalUrl = "https://old.edouniversity.edu.ng/admissions/instructions";
            return Redirect(externalUrl);
        }

        // POST: admissions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register1(Applicant applicant, string pword, string cpword)
        {

            if (pword != cpword || (pword == null))
            {
                TempData["err"] = "Something went wrong, make sure you provide data as required!";
                return RedirectToAction(nameof(Undergraduate1));
            }
            applicant.Password = pword;
            applicant.ApplicantId =Guid.NewGuid().ToString() + DateTime.Now.Millisecond;
            var id = applicant.ApplicantId;
            _context.Add(applicant);
            await _context.SaveChangesAsync();
            return RedirectToAction("step1", "admissions", new { id });

            ViewData["LGAId"] = new SelectList(_context.Lgas, "Id", "Id", applicant.LGAId);
            ViewData["NationalityId"] = new SelectList(_context.Countries, "Id", "Id", applicant.NationalityId);
            ViewData["StateOfOriginId"] = new SelectList(_context.States, "Id", "Id", applicant.StateOfOriginId);
            return View(applicant);
        }
        [Authorize(Roles = "superAdmin")]
        public async Task<IActionResult> Step1(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicant = await _context.UgApplicants.FirstOrDefaultAsync(i => i.ApplicantId == id);
            if (applicant == null)
            {
                return NotFound();
            }
            
            ViewData["Country"] = new SelectList(_context.Countries, "Id", "Name");
            ViewData["State"] = new SelectList(_context.States, "Id", "Name");
            ViewData["Lga"] = new SelectList(_context.Lgas, "Id", "Name");

            return View(applicant);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Step1(string id, Applicant applicant)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicantToUpdate = await _context.UgApplicants.FirstOrDefaultAsync(i => i.ApplicantId == id);

            if (applicantToUpdate == null)
            {
                return NotFound();
            }

           
                applicantToUpdate.Surname = applicant.Surname;
                applicantToUpdate.FirstName = applicant.FirstName;
                applicantToUpdate.OtherName = applicant.OtherName;
                applicantToUpdate.Sex = applicant.Sex;
                applicantToUpdate.DOB = applicant.DOB;
                applicantToUpdate.MaritalStatus = applicant.MaritalStatus;
                applicantToUpdate.PlaceOfBirth = applicant.PlaceOfBirth;
                applicantToUpdate.ContactAddress = applicant.ContactAddress;
                applicantToUpdate.PermanentHomeAddress = applicant.PermanentHomeAddress;
                applicantToUpdate.NationalityId = applicant.NationalityId;
                applicantToUpdate.StateOfOriginId = applicant.StateOfOriginId;
                applicantToUpdate.LGAId = applicant.LGAId;
                applicantToUpdate.PhoneNumber = applicant.PhoneNumber;
                applicantToUpdate.Email = applicant.Email;
                applicantToUpdate.PrimarySchool = applicant.PrimarySchool;
                applicantToUpdate.SecondarySchool = applicant.SecondarySchool;
                applicantToUpdate.Indigine = applicant.Indigine;
                applicantToUpdate.ModeOfEntry = applicant.ModeOfEntry;

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

                return RedirectToAction("Step2", "admissions", new { id });
            
        }
        // GET: Applicants/Step2/Id
        [Authorize(Roles = "superAdmin")]
        public async Task<IActionResult> Step2(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicant = await _context.UgApplicants.FirstOrDefaultAsync(i => i.ApplicantId == id);
            if (applicant == null)
            {
                return NotFound();
            }
            ViewData["SSCESubjectId"] = new SelectList(_context.SsceSubjects, "Id", "SubjectName");
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name");

            return View(applicant);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Step2(string id, int a)
        {
            var applicants = await _context.UgApplicants.FirstOrDefaultAsync(i => i.ApplicantId == id);

            if (id == null)
            {
                return NotFound();
            }
            try
            {
                var ApplicatantToUpdate = await _context.UgApplicants
                .FirstOrDefaultAsync(c => c.ApplicantId == id);

                if (await TryUpdateModelAsync<Applicant>(ApplicatantToUpdate, "", c => c.UTMENumber, c => c.UTMESubject1, c => c.UTMESubject1Score,
                    c => c.UTMESubject2, c => c.UTMESubject2Score, c => c.UTMESubject3, c => c.UTMESubject3Score,
                    c => c.UTMESubject4, c => c.UTMESubject4Score, c => c.FirstChoice, c => c.SecondChoice, c => c.ThirdChoice))
                {
                    applicants.UTMETotal = applicants.UTMESubject1Score + applicants.UTMESubject2Score + applicants.UTMESubject3Score + applicants.UTMESubject4Score;
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
                    return RedirectToAction("step3", "admissions", new { id });
                }
            }
            catch (Exception ex)
            {
                ex.ToString();

            }
            return View();

        }
        [Authorize(Roles = "superAdmin")]
        public async Task<IActionResult> Step3(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicant = await _context.UgApplicants.FirstOrDefaultAsync(i => i.ApplicantId == id);
            if (applicant == null)
            {
                return NotFound();
            }


            List<SsceSubjects> Ssce = new();
            Ssce = (from c in _context.SsceSubjects select c).ToList();
            ViewBag.message3 = Ssce;
            List<SSCEGrade> grade = new();
            grade = (from c in _context.SSCEGrades select c).ToList();
            ViewBag.message4 = grade;
            return View(applicant);
        }

        // POST: Applicants/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Step3(string id, int a)
        {
            var applicants = await _context.UgApplicants.FirstOrDefaultAsync(i => i.ApplicantId == id);


            if (id == null)
            {
                return NotFound();
            }
            try
            {
                var ApplicatantToUpdate = await _context.UgApplicants
                .FirstOrDefaultAsync(c => c.ApplicantId == id);

                if (await TryUpdateModelAsync<Applicant>(ApplicatantToUpdate, "", c => c.NoOfSittings, c => c.Ssce1Type,
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
                    return RedirectToAction("step4", "admissions", new { id });
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
        [Authorize(Roles = "superAdmin")]
        public async Task<IActionResult> Step4(string? id)
        {
            if (id == null || _context.UgApplicants == null)
            {
                return NotFound();
            }

            var applicant = await _context.UgApplicants.FirstOrDefaultAsync(i => i.ApplicantId == id);
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
        public async Task<IActionResult> Step4(string id, int a)
        {
            var applicants = await _context.UgApplicants.FirstOrDefaultAsync(i => i.ApplicantId == id);

            if (id == null)
            {
                return NotFound();
            }
            try
            {
               
                var ApplicatantToUpdate = await _context.UgApplicants
                .FirstOrDefaultAsync(c => c.ApplicantId == id);

                if (await TryUpdateModelAsync<Applicant>(ApplicatantToUpdate, "", c => c.ParentFullName, c => c.ParentAddress,
                    c => c.ParentPhoneNumber, c => c.ParentAlternatePhoneNumber, c => c.ParentEmail, c => c.ParentOccupation))
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
                    return RedirectToAction("step5", "admissions", new { id });
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
        [Authorize(Roles = "superAdmin")]
        public async Task<IActionResult> Step5(string? id)
        {
            if (id == null || _context.UgApplicants == null)
            {
                return NotFound();
            }

            var applicant = await _context.UgApplicants.FirstOrDefaultAsync(i => i.ApplicantId == id);
            if (applicant == null)
            {
                return NotFound();
            }
            return View(applicant);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Step5(IFormFile passport, IFormFile jamb, IFormFile ssce1, IFormFile birtCertificate, IFormFile directEntryUpload, IFormFile lga, string ApplicantId)
        {
            var applicants = await _context.UgApplicants.FirstOrDefaultAsync(i => i.ApplicantId == ApplicantId);

            if (ApplicantId == null)
            {
                return NotFound();
            }
            try
            {
                if (passport != null && passport.Length > 0)
                {
                    var uploadDir = @"files/applicantUploads/passports";
                    var fileName = Path.GetFileNameWithoutExtension(passport.FileName);
                    var extension = Path.GetExtension(passport.FileName);
                    var webRootPath = _hostingEnvironment.WebRootPath;
                    fileName = applicants.UTMENumber + extension;

                    //fileName = fileName + extension;
                    var path = Path.Combine(webRootPath, uploadDir, fileName);
                    using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
                    {
                        passport.CopyTo(fs);
                        applicants.PassportUpload = fileName;

                    }
                }
                if (jamb != null && jamb.Length > 0)
                {
                    var uploadDir = @"files/applicantUploads/jamb";
                    var fileName = Path.GetFileNameWithoutExtension(jamb.FileName);
                    var extension = Path.GetExtension(jamb.FileName);
                    var webRootPath = _hostingEnvironment.WebRootPath;
                    //fileName = DateTime.UtcNow.ToString("yymmssfff") + fileName + extension;

                    fileName = applicants.UTMENumber + extension;
                    var path = Path.Combine(webRootPath, uploadDir, fileName);
                    using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
                    {
                        jamb.CopyTo(fs);
                        applicants.JambUpload = fileName;

                    }
                }
                if (ssce1 != null && ssce1.Length > 0)
                {
                    var uploadDir = @"files/applicantUploads/ssce";
                    var fileName = Path.GetFileNameWithoutExtension(ssce1.FileName);
                    var extension = Path.GetExtension(ssce1.FileName);
                    var webRootPath = _hostingEnvironment.WebRootPath;

                    fileName = applicants.UTMENumber + extension;
                    var path = Path.Combine(webRootPath, uploadDir, fileName);
                    using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
                    {
                        ssce1.CopyTo(fs);
                        applicants.Ssce1 = fileName;

                    }

                }
                if (birtCertificate != null && birtCertificate.Length > 0)
                {
                    var uploadDir = @"files/applicantUploads/birthcertificates";
                    var fileName = Path.GetFileNameWithoutExtension(birtCertificate.FileName);
                    var extension = Path.GetExtension(birtCertificate.FileName);
                    var webRootPath = _hostingEnvironment.WebRootPath;

                    fileName = applicants.UTMENumber + extension;
                    var path = Path.Combine(webRootPath, uploadDir, fileName);
                    using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
                    {
                        birtCertificate.CopyTo(fs);
                        applicants.BirthCertUpload = fileName;

                    }
                }
                if (directEntryUpload != null && directEntryUpload.Length > 0)
                {
                    var uploadDir = @"files/applicantUploads/directentry";
                    var fileName = Path.GetFileNameWithoutExtension(directEntryUpload.FileName);
                    var extension = Path.GetExtension(directEntryUpload.FileName);
                    var webRootPath = _hostingEnvironment.WebRootPath;

                    fileName = applicants.UTMENumber + extension;
                    var path = Path.Combine(webRootPath, uploadDir, fileName);
                    using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
                    {
                        directEntryUpload.CopyTo(fs);
                        applicants.DirectEntryUpload = fileName;

                    }

                }
                if (lga != null && lga.Length > 0)
                {
                    var uploadDir = @"files/applicantUploads/lga";
                    var fileName = Path.GetFileNameWithoutExtension(lga.FileName);
                    var extension = Path.GetExtension(lga.FileName);
                    var webRootPath = _hostingEnvironment.WebRootPath;

                    fileName = applicants.UTMENumber + extension;
                    var path = Path.Combine(webRootPath, uploadDir, fileName);
                    using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
                    {
                        lga.CopyTo(fs);
                        applicants.LGAUpload = fileName;

                    }
                }
                await TryUpdateModelAsync<Applicant>(applicants);

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
            return RedirectToAction("step5", "admissions", new { ApplicantId });

        }
        [Authorize(Roles = "superAdmin")]
        public async Task<IActionResult> Eligibility(string? id)
        {
            if (id == null || _context.UgApplicants == null)
            {
                return NotFound();
            }
            var applicant = await _context.UgApplicants.Include(i => i.Nationalities).Include(i => i.States).Include(i => i.LGAs).FirstOrDefaultAsync(i => i.ApplicantId == id);

            if (applicant == null)
            {
                return NotFound();
            }
            try
            {
                
                return View(applicant);
            }
            catch (Exception)
            {
                return RedirectToAction("badreq","error");
                throw;
            }
            
        }
        [Authorize(Roles = "superAdmin")]
        public async Task<IActionResult> Summary(string? id)
        {
            
            if (id == null || _context.UgApplicants == null)
            {
                return NotFound();
            }
            var applicant = await _context.UgApplicants.Include(i => i.Nationalities).Include(i => i.States).Include(i => i.LGAs).FirstOrDefaultAsync(i => i.ApplicantId == id);
            if (applicant == null)
            {
                ViewBag.err = "Applicant with this ID does not exist";
                return RedirectToAction("badreq", "error");
            }
            if (applicant.Paid == true)
            {
                return View(applicant);
            }
            TempData["err"] = "Make sure to have paid your application fee before attempting to access this resource.";
            return RedirectToAction("badreq", "error");

        }
        [Authorize(Roles = "staff, superAdmin, ugAdmission")]
        public async Task<IActionResult> Cancel(int? id)
        {
            var applicants = _context.UgApplicants.Where(x => x.Id == id).FirstOrDefault();
            return PartialView("_cancel", applicants);
        }
        [Authorize(Roles = "staff, superAdmin, ugAdmission")]
        public async Task<IActionResult> Reject(int? id)
        {
            var applicants = _context.UgApplicants.Where(x => x.Id == id).FirstOrDefault();
            return PartialView("_reject", applicants);
        }
        [Authorize(Roles = "staff, superAdmin, ugAdmission")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int? id, int a)
        {
            var applicants = await _context.UgApplicants.FindAsync( id);

           
            try
            {
                var ApplicatantToUpdate = await _context.UgApplicants
                .FirstOrDefaultAsync(c => c.Id == id);

                if (await TryUpdateModelAsync<Applicant>(ApplicatantToUpdate, "", c => c.Id))
                {
                    applicants.Status = MainStatus.Declined;
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "admissions");
                }
            }
            catch (Exception ex)
            {
                ex.ToString();

            }
            return RedirectToAction("Index", "admissions");

        }
        [Authorize(Roles = "staff, superAdmin, ugAdmission")]
        public async Task<IActionResult> Admit(int? id)
        {
            var applicants = _context.UgApplicants.Where(x => x.Id == id).FirstOrDefault();
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name");
            ViewData["ProgramId"] = new SelectList(_context.Programs, "Id", "NameOfProgram");
            ViewData["LevelId"] = new SelectList(_context.Levels, "Id", "Name");
            return PartialView("_admissionPartial", applicants);
        }
        [Authorize(Roles = "staff, superAdmin, ugAdmission")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Admit(int? id, UgMainWallet allwallet)
        {
            var applicants = await _context.UgApplicants.FindAsync(id);

            if (id == null)
            {
                return NotFound();
            }
            try
            {
                var ApplicatantToUpdate = await _context.UgApplicants
                .FirstOrDefaultAsync(c => c.Id == id);

                if (await TryUpdateModelAsync<Applicant>(ApplicatantToUpdate, "", c => c.ProgrameId, c => c.AdmittedInto, c => c.LevelAdmittedTo))
                {
                    var activeSession = (from s in _context.Sessions where s.IsActive == true select s).ToList();
                    
                    applicants.Status = MainStatus.Approved;
                    applicants.Screened = true;
                    foreach (var item in activeSession)
                    {
                        applicants.YearOfAdmission = item.Id;

                    }
                    try
                    {
                        //Creates a bulk wallet
                        allwallet.ApplicantId = applicants.Id;
                        allwallet.Name = applicants.Surname + " " + applicants.FirstName + " " + applicants.OtherName;
                        allwallet.WalletId = applicants.ApplicantId;
                        allwallet.BulkDebitBalanace = 0;
                        allwallet.CreditBalance = 0;
                        allwallet.Status = false;
                        allwallet.DateCreated = DateTime.Now;
                        _context.UgMainWallets.Add(allwallet);
                        await _context.SaveChangesAsync();

                    }
                    catch (DbUpdateConcurrencyException)
                    {

                        ModelState.AddModelError("", "Unable to save changes. " +
                            "Try again, and if the problem persists, " +
                            "see your system administrator.");
                    }

                    return RedirectToAction("Index", "admissions", new { id });
                }
            }
            catch (Exception ex)
            {
                ex.ToString();

            }
            return RedirectToAction("Index", "admissions");

        }
        [Authorize(Roles = "staff, superAdmin, ugAdmission, ugClearance")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearApplicant(Student Newstudent)
        {
            //Get the person who performed this action
            //var loggedInUser = await userManager.GetUserAsync(HttpContext.User);
            //var userId = loggedInUser.StaffId;

           // var applicants = await _context.UgApplicants.ToListAsync();
            var applicants = (from s in _context.UgApplicants where s.ApplicantionId == "3-Jan" select s).ToList();
            foreach (var item in applicants)
            {
                try
                {
                    var student = new Student();
                    item.Cleared = true;
                    var Session = (from T in _context.Sessions
                                   where T.IsActive == true
                                   select T.suffix).FirstOrDefault();
                    var dept = (from g in _context.Departments where g.Id == item.AdmittedInto select g.Id).FirstOrDefault();
                    //Moving Applicant to student table
                    //student.ApplicantId = item.Id;
                    student.StudentId = item.UTMENumber;
                    student.Fullname = item.Surname + " " + item.FirstName + " " + item.OtherName;
                    student.Picture = item.PassportUpload;
                    student.Sex = item.Sex;
                    student.DOB = item.DOB;
                    student.Phone = item.PhoneNumber;
                    student.Email = item.Email;
                    student.NationalityId = item.NationalityId;
                    student.StateOfOriginId = item.StateOfOriginId;
                    student.LGAId = item.LGAId;
                    student.PlaceOfBirth = item.PlaceOfBirth;
                    student.PermanentHomeAddress = item.PermanentHomeAddress;
                    student.ContactAddress = item.ContactAddress;
                    student.MaritalStatus = item.MaritalStatus;
                    student.ParentName = item.ParentFullName;
                    student.ParentOccupation = item.ParentOccupation;
                    student.ParentPhone = item.ParentPhoneNumber;
                    student.ParentAltPhone = item.ParentAlternatePhoneNumber;
                    student.ParentEmail = item.ParentEmail;
                    student.ParentAddress = item.ParentAddress;
                    student.StudentStatus = 1;
                    
                    var edsumail = item.Surname + Session + "." + item.FirstName + "@edouniversity.edu.ng";
                    student.SchoolEmailAddress = edsumail.ToLower();
                    student.YearOfAdmission = 9;
                    student.CurrentSession = 9;
                    
                    student.Department = dept;
                    
                    student.UTMENumber = item.UTMENumber;
                    student.Level = item.LevelAdmittedTo;
                    student.ModeOfAdmission = item.ModeOfEntry;
                    student.Cleared = true;
                   // student.ClearedBy = userId;
                    student.CreatedAt = DateTime.Now;
                    _context.Students.Add(student);
                    await _context.SaveChangesAsync();

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
                    var r = await userManager.CreateAsync(user, student.Phone);
                    try
                    {
                        var roleId = "4e8c6adc-ae5c-46e4-8618-e0b18f0841ba";
                        var role = await roleManager.FindByIdAsync(roleId);
                        await userManager.AddToRoleAsync(user, role.Name);

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
                //change the cleared status to true in the applicants table
                return RedirectToAction("index", "admissions");
        }
        [Authorize(Roles = "staff, superAdmin, ugAdmission, ugClearance")]

        //Upon activation of wallet, this module creates the first debt row in the 
        //wallets table and updates the applicants/student's Main wallet.
        public async Task<IActionResult> ActivateWallet(string? id, UgSubWallet myWallet)
        {
            var applicant = await _context.UgApplicants
                .FirstOrDefaultAsync(m => m.UTMENumber == id);
            if (applicant == null)
            {
                return NotFound();
            }
            var session = await _context.Sessions.FirstOrDefaultAsync(s => s.IsActive == true);
            myWallet.SessionId = session.Id;

            //Since this module only activates wallet for freshers, regardless of the level
            //you're admitted to, you'd pay the amount the 100l are paying for that session
            var fee = (from s in _context.AllFees where s.DepartmentId == applicant.AdmittedInto select s).FirstOrDefault();
            var TuitionFee = fee.Tuition;
            myWallet.Name = applicant.Surname + " " + applicant.FirstName + " " + applicant.OtherName;
           
            myWallet.Pic = applicant.PassportUpload;
            myWallet.RegNo = applicant.UTMENumber;
            myWallet.Level = applicant.LevelAdmittedTo;
            myWallet.Department = applicant.AdmittedInto;
            myWallet.EDHIS = fee.EDHIS;
            myWallet.LMS = fee.LMS;
            myWallet.SRC = fee.SRC;
            myWallet.AcceptanceFee = fee.Acceptance;
            myWallet.Tuition = TuitionFee;
            //If applicant is a transfer student and if the he/she belongs to either MBBS, BNSc, Law.
            //The integer values below are the IDs of MBBS, BNSc and Law respectively in the Departments table.
            if (applicant.ModeOfEntry == "Transfer" && (applicant.AdmittedInto == 5 || applicant.AdmittedInto == 6 || applicant.AdmittedInto == 9))
            {
                myWallet.Tuition2 = myWallet.Tuition;
            }
            else
            {
                myWallet.Tuition2 = 0;
            }
            myWallet.Debit = myWallet.Tuition + myWallet.Tuition2 + myWallet.LMS + myWallet.SRC + myWallet.AcceptanceFee + myWallet.EDHIS;
            myWallet.FortyPercent = myWallet.Tuition * 40 / 100;
            myWallet.SixtyPercent = myWallet.Tuition * 60 / 100;
            myWallet.CreditBalance = 0;
            myWallet.WalletId = applicant.ApplicantId;
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


            return RedirectToAction(nameof(Index));
        }

        // GET: admissions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.UgApplicants == null)
            {
                return NotFound();
            }

            var applicant = await _context.UgApplicants.FindAsync(id);
            if (applicant == null)
            {
                return NotFound();
            }
            ViewData["AdmittedInto"] = new SelectList(_context.Departments, "Id", "Id", applicant.AdmittedInto);
            ViewData["LGAId"] = new SelectList(_context.Lgas, "Id", "Id", applicant.LGAId);
            ViewData["LevelAdmittedTo"] = new SelectList(_context.Levels, "Id", "Id", applicant.LevelAdmittedTo);
            ViewData["NationalityId"] = new SelectList(_context.Countries, "Id", "Id", applicant.NationalityId);
            ViewData["StateOfOriginId"] = new SelectList(_context.States, "Id", "Id", applicant.StateOfOriginId);
            return View(applicant);
        }

        // POST: admissions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Surname,FirstName,OtherName,Sex,DOB,MaritalStatus,PlaceOfBirth,ContactAddress,PermanentHomeAddress,NationalityId,StateOfOriginId,LGAId,PhoneNumber,AltPhoneNumber,PrimarySchool,SecondarySchool,Indigine,ModeOfEntry,PreviousInstitution,PreviousLevel,PreviousGrade,Email,Password,UTMENumber,CourseChoseInJamb,UTMESubject1,UTMESubject2,UTMESubject3,UTMESubject4,UTMESubject1Score,UTMESubject2Score,UTMESubject3Score,UTMESubject4Score,UTMETotal,FirstChoice,SecondChoice,ThirdChoice,NoOfSittings,Ssce1Type,Ssce1Year,Ssce1Number,Ssce1Subject1,Ssce1Subject2,Ssce1Subject3,Ssce1Subject4,Ssce1Subject5,Ssce1Subject6,Ssce1Subject7,Ssce1Subject8,Ssce1Subject9,Ssce1Subject1Grade,Ssce1Subject2Grade,Ssce1Subject3Grade,Ssce1Subject4Grade,Ssce1Subject5Grade,Ssce1Subject6Grade,Ssce1Subject7Grade,Ssce1Subject8Grade,Ssce1Subject9Grade,Ssce2Type,Ssce2Year,Ssce2Number,Ssce2Subject1,Ssce2Subject2,Ssce2Subject3,Ssce2Subject4,Ssce2Subject5,Ssce2Subject6,Ssce2Subject7,Ssce2Subject8,Ssce2Subject9,Ssce2Subject1Grade,Ssce2Subject2Grade,Ssce2Subject3Grade,Ssce2Subject4Grade,Ssce2Subject5Grade,Ssce2Subject6Grade,Ssce2Subject7Grade,Ssce2Subject8Grade,Ssce2Subject9Grade,ParentFullName,ParentAddress,ParentPhoneNumber,ParentAlternatePhoneNumber,ParentEmail,ParentOccupation,PassportUpload,JambUpload,Ssce1,BirthCertUpload,DirectEntryUpload,LGAUpload,Status,Screened,LevelAdmittedTo,AdmittedInto,YearOfAdmission,Cleared,CreatedAt")] Applicant applicant)
        {
            if (id != applicant.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(applicant);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicantExists(applicant.Id))
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
            ViewData["AdmittedInto"] = new SelectList(_context.Departments, "Id", "Id", applicant.AdmittedInto);
            ViewData["LGAId"] = new SelectList(_context.Lgas, "Id", "Id", applicant.LGAId);
            ViewData["LevelAdmittedTo"] = new SelectList(_context.Levels, "Id", "Id", applicant.LevelAdmittedTo);
            ViewData["NationalityId"] = new SelectList(_context.Countries, "Id", "Id", applicant.NationalityId);
            ViewData["StateOfOriginId"] = new SelectList(_context.States, "Id", "Id", applicant.StateOfOriginId);
            return View(applicant);
        }

        // GET: admissions/Delete/5
        [Authorize(Roles = "staff, superAdmin, ugAdmission")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.UgApplicants == null)
            {
                return NotFound();
            }

            var applicant = await _context.UgApplicants
                .Include(a => a.Departments)
                .Include(a => a.LGAs)
                .Include(a => a.Levels)
                .Include(a => a.Nationalities)
                .Include(a => a.States)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (applicant == null)
            {
                return NotFound();
            }

            return PartialView("_delete",applicant);
        }
        [Authorize(Roles = "staff, superAdmin, ugAdmission")]
        // POST: admissions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.UgApplicants == null)
            {
                return Problem("Entity set 'ApplicationDbContext.UgApplicants'  is null.");
            }
            var applicant = await _context.UgApplicants.FindAsync(id);
            if (applicant != null)
            {
                _context.UgApplicants.Remove(applicant);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Applicants()
        {
            return View();
        }
        public async Task<IActionResult> Wallet(string id)
        {
           
            //var applicant = await _context.UgApplicants.Where(x => x.UTMENumber == id).FirstOrDefaultAsync();
            var wallet = await _context.UgSubWallets.Where(x => x.WalletId == id).FirstOrDefaultAsync();
            
            ViewBag.utme =id;
            //var model = new AdmissionWalletVM
            //{
            //    MainWallet = wallet,
            //    //Applicant = applicant
            //};
            return View(wallet);
        }
        //public async Task<IActionResult> Debts(string id)
        //{
        //    var wallet = (from s in _context.UgSubWallets where s.WalletId == id select s).Include(c => c.Sessions).ToList();

        //    if (wallet == null)
        //    {
        //        return RedirectToAction("pagenotfound", "error");
        //    }

        //    return View(wallet);

        //}
        public IActionResult Hostelreq(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicant = _context.UgApplicants.FirstOrDefault(i => i.ApplicantId == id);
            if (applicant == null)
            {
                return NotFound();
            }

            return View(applicant);
        }
        public IActionResult LMSreq(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicant = _context.UgApplicants.FirstOrDefault(i => i.ApplicantId == id);
            if (applicant == null)
            {
                return NotFound();
            }

            return View(applicant);
        } 
        public IActionResult Clearance(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicant = _context.UgApplicants.FirstOrDefault(i => i.ApplicantId == id);
            if (applicant == null)
            {
                return NotFound();
            }

            return View(applicant);
        }
        private bool ApplicantExists(int? id)
        {
          return _context.UgApplicants.Any(e => e.Id == id);
        }

        //////////////////////////////////////////////////////////////
        ////////////////////TRANSACTION MODULES//////////////////////
        //Initiating Acceptance payment
        public async Task<IActionResult> Acceptance(string id, Payment payment, Student student)
        {
            var wallet = await _context.UgSubWallets
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
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            //Get the payment to return
            var paymentToGet = await _context.Payments
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
        //Initiating Tuition payment
        public async Task<IActionResult> TuitionTransfer(string id, Payment payment, Student student)
        {
            var wallet = await _context.UgSubWallets
                .FirstOrDefaultAsync(m => m.WalletId == id);

            Random r = new();
            //Payment is created just before it returns the view
            ViewBag.Name = wallet.Name;
            payment.SessionId = wallet.SessionId;
            payment.WalletId = wallet.Id;
            payment.Amount = (double)wallet.Tuition2 + 300;
            payment.Status = "Pending";
            payment.Ref = "EDSU-" + r.Next(10000000) + DateTime.Now.Millisecond;
            payment.PaymentDate = DateTime.Now;
            payment.Type = "Tuition(Transfer)";
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            //Get the payment to return
            var paymentToGet = await _context.Payments
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
        public async Task<IActionResult> Tuition(string id, Payment payment, Student student)
        {
            var wallet = await _context.UgSubWallets
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
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            //Get the payment to return
            var paymentToGet = await _context.Payments
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
        public async Task<IActionResult> Tuition60(string id, Payment payment, Student student)
        {
            var wallet = await _context.UgSubWallets
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
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            //Get the payment to return
            var paymentToGet = await _context.Payments
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
        public async Task<IActionResult> Tuition40(string id, Payment payment, Student student)
        {
            var wallet = await _context.UgSubWallets
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
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            //Get the payment to return
            var paymentToGet = await _context.Payments
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
        public async Task<IActionResult> LMS(string id, Payment payment, Student student)
        {
            var wallet = await _context.UgSubWallets
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
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            //Get the payment to return
            var paymentToGet = await _context.Payments
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
        //Initiating SRC payment
        public async Task<IActionResult> SRC(string id, Payment payment, Student student)
        {
            var wallet = await _context.UgSubWallets
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
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            //Get the payment to return
            var paymentToGet = await _context.Payments
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
        ////////////////////////////////////////
        /// TUITION CUSTOM
        ///////////////////////////////////////
        /// [Authorize(Roles = "student, superAdmin")]
        //Initiating Custom payment
        public async Task<IActionResult> Custom(string id, double amount, Payment payment)
        {
            var wallet = _context.UgSubWallets
                 .FirstOrDefault(m => m.WalletId == id);
            if (amount <= (double)wallet.SixtyPercent)
            {
                Random r = new();
                //Payment is created just before it returns the view
                ViewBag.Name = wallet.Name;
                payment.SessionId = wallet.SessionId;
                payment.WalletId = wallet.Id;
                payment.Amount = amount + 300;
                payment.Status = "Pending";
                payment.Ref = "EDSU-" + r.Next(10000000) + DateTime.Now.Millisecond;
                payment.PaymentDate = DateTime.Now;
                payment.Type = "Tuition Custom";
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                //Get the payment to return
                var paymentToGet = _context.Payments
                    .Find(payment.Id);
                if (paymentToGet == null)
                {
                    return NotFound();
                }
                //When the payment row is created, it stores the id in a tempdata then pass it to the verify endpoint
                TempData["PaymentId"] = payment.Wallets.WalletId;
                TempData["walletId"] = id;
                return View(paymentToGet);
            }
            else
            {
                TempData["err"] = "You cannot make custom payment above 60%.";
                return RedirectToAction("badreq", "error");
                
            }
        }
        //Initiating EDHIS payment
        public async Task<IActionResult> EDHIS(string id, Payment payment, Student student)
        {
            var wallet = await _context.UgSubWallets
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
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            //Get the payment to return
            var paymentToGet = await _context.Payments
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

                var PaymentToUpdate = _context.Payments
               .FirstOrDefault(c => c.Ref == Ref);
                var orderid = Ref;
                if (await TryUpdateModelAsync<Payment>(PaymentToUpdate, "", c => c.Email))
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
                    return RedirectToAction("checkout", "admissions", new { orderid });

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
            var paymentToGet = await _context.Payments
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
        public async Task<IActionResult> Others(string id, Payment payment, Student student)
        {
            //Using Viewbag to display list of other fees and session from their respective tables table.
            ViewData["otherFees"] = new SelectList(_context.OtherFees, "Id", "Name");

            var wallet = await _context.UgSubWallets
                .FirstOrDefaultAsync(m => m.WalletId == id);
            Random r = new();
            //Payment is created just before it returns the view

            ViewBag.Name = wallet.Name;
            payment.SessionId = wallet.SessionId;
            payment.WalletId = wallet.Id;
            payment.Status = "Pending";
            payment.Ref = "EDSU-" + r.Next(10000000) + DateTime.Now.Millisecond;
            payment.PaymentDate = DateTime.Now;
            _context.Payments.Add(payment);
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
                var PaymentToUpdate = await _context.Payments.FirstOrDefaultAsync(x => x.Ref == Ref);
                //var OtherRef = Ref;

                if (await TryUpdateModelAsync<Payment>(PaymentToUpdate, "", c => c.Email, c => c.OtherFeesDesc))
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
                    return RedirectToAction("otherscheckout", "admissions", new { Ref });

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
            var paymentToUpdate = _context.Payments.Where(i => i.Ref == Ref).FirstOrDefault();
            if (Ref == null || _context.Payments == null)
            {
                return NotFound();
            }
            if (paymentToUpdate == null)
            {
                return NotFound();
            }

            return View(paymentToUpdate);
        }
        public async Task<IActionResult> Preview(string? utme)
        {
            //try
            //{
                var student = (from s in _context.UgApplicants where s.UTMENumber == utme select s)
                    .Include(c => c.Departments).Include(c => c.Levels).Include(c => c.Programs).FirstOrDefault();
                var session = (from ses in _context.Sessions where ses.IsActive == true select ses).FirstOrDefault();
                ViewBag.name = student.Surname + " " + student.FirstName + " " + student.OtherName;
                ViewBag.email = student.Email;
                ViewBag.dept = student.Departments.Name;
                ViewBag.utme = student.UTMENumber;
                ViewBag.session = session.Name;
                ViewBag.level = student.Levels.Name;
                var clearance = (from s in _context.BursaryClearancesFreshers where s.ClearanceId == student.UTMENumber && s.SessionId == session.Id select s)
                    .Include(i => i.Hostels).Include(i => i.Payments).ThenInclude(i => i.OtherFees).ThenInclude(i => i.Sessions).ToList();
               
            var wallet = await _context.UgMainWallets.Where(x => x.WalletId == utme).FirstOrDefaultAsync();
            var subwallet = _context.UgSubWallets.Where(x => x.WalletId == student.UTMENumber).FirstOrDefault();
            var hostelPayment = (from hostel in _context.HostelPayments where hostel.WalletId == subwallet.Id && hostel.Status != "Pending" select hostel).Include(x => x.HostelFees).FirstOrDefault();
            if (hostelPayment != null)
            {
                ViewBag.hostelName = hostelPayment.HostelFees.Name;
                ViewBag.hostelAmount = hostelPayment.Amount;
                ViewBag.hostelMode = hostelPayment.Mode;
                ViewBag.reference = hostelPayment.Ref;
                ViewBag.hostelDate = hostelPayment.PaymentDate;
                ViewBag.hostelStatus = hostelPayment.Status;
            }
            else
            {
                ViewBag.hostelName = "NIL";
                ViewBag.hostelAmount = "NIL";
                ViewBag.hostelMode = "NIL";
                ViewBag.hostelDate = "NIL";
                ViewBag.hostelStatus = "NIL";
                ViewBag.reference = "NIL";
            }

            var room = await _context.HostelAllocations.Where(x => x.WalletId == wallet.Id).Include(x => x.HostelRooms).ThenInclude(x => x.Hostels).FirstOrDefaultAsync();

            if (room != null)
            {
                ViewBag.hostel = room.HostelRooms.Hostels.Name;
                ViewBag.room = room.HostelRooms.RoomNo;
            }
            else
            {
                ViewBag.hostel = "No Hostel Found";
                ViewBag.room = "No Room Found";
            }

            //var clearedStatus = _context.BursaryClearancesFreshers.Where(x => x.ClearanceId == utme).ToList();
            //if (clearedStatus != null)
            //{
            //    ViewBag.status = clearedStatus.Remark;
            //}
            //else
            //{
            //    ViewBag.status = "Pending";
            //}
            return View(clearance);
        }
        public async Task<IActionResult> History(string id)
        {
            var applicationDbContext = (from f in _context.Payments where f.Wallets.WalletId == id select f).Include(i => i.Wallets).Include(i => i.Wallets.Levels).Include(i => i.Sessions);
            return View(await applicationDbContext.ToListAsync());
        }
    }
}
