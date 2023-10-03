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
using Microsoft.AspNetCore.Authorization;
using static EDSU_SYSTEM.Models.Enum;

namespace EDSU_SYSTEM.Controllers
{
    [Authorize]
    public class BursaryClearancesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public BursaryClearancesController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        [Authorize(Roles = "bursaryClearance, bursaryAdmin, superAdmin")]
        // GET: busaryClearances
        public async Task<IActionResult> Index()
        {
            var studentIds = _context.BursaryClearances.Select(b => b.StudentId).ToList();
            var students = _context.Students.Where(s => studentIds.Contains(s.Id)).Include(i => i.Departments).Include(i => i.Levels).ToList();

            return View(students);
        }
        [Authorize(Roles = "bursaryClearance, bursaryAdmin, superAdmin")]
        // GET: busaryClearances
        public async Task<IActionResult> Freshers()
        {
            var studentIds = _context.BursaryClearancesFreshers.Select(b => b.ClearanceId).ToList();
            var students = _context.UgApplicants.Where(s => studentIds.Contains(s.UTMENumber)).Include(i => i.Departments).Include(i => i.Levels).ToList();

            return View(students);
        }
        [Authorize(Roles = "bursaryClearance, bursaryAdmin, superAdmin")]
        // GET: busaryClearances
        public async Task<IActionResult> OfflineClearance()
        {
            var studentIds = _context.OfflinePaymentClearances.Select(b => b.StudentId).ToList();
            var students = _context.Students.Where(s => studentIds.Contains(s.Id)).Include(i => i.Departments).Include(i => i.Levels).ToList();

            return View(students);
        }
        [Authorize(Roles = "bursaryAdmin, superAdmin")]
        public async Task<IActionResult> Mainwallets()
        {
            var mainwallets = _context.UgMainWallets.Where(x => x.StudentType == Models.Enum.StudentType.Fee_Paying).ToList();
            return View(mainwallets);
        }
        [Authorize(Roles = "bursaryAdmin, superAdmin")]
        public async Task<IActionResult> Scholarshipwallets()
        {
            var mainwallets = _context.UgMainWallets.Where(x => x.StudentType == Models.Enum.StudentType.Scholarship).ToList();
            return View(mainwallets);
        }
        [Authorize(Roles = "bursaryAdmin, superAdmin")]
        public async Task<IActionResult> StaffChildrenwallets()
        {
            var mainwallets = _context.UgMainWallets.Where(x => x.StudentType == Models.Enum.StudentType.Staff_Sponsored).ToList();
            return View(mainwallets);
        }
        [Authorize(Roles = "bursaryAdmin, superAdmin")]
        [HttpPost]
        public async Task<IActionResult> StudentType(string walletId, StudentType studentType, UgMainWallet main)
        {
            var wallet = (from m in _context.UgMainWallets where m.WalletId == walletId select m).FirstOrDefault();
            wallet.StudentType = studentType;
            _context.SaveChanges();
            var id = wallet.WalletId;
            return RedirectToAction("sub", "bursaryclearances", new {id });
        }
        [Authorize(Roles = "bursaryAdmin, superAdmin")]
        public async Task<IActionResult> Sub(string id)
        {
            var subwallets = _context.UgSubWallets.Where(x => x.WalletId == id).FirstOrDefault();
            ViewData["SessionId"] = new SelectList(_context.Sessions, "Id", "Name");
            ViewData["LevelId"] = new SelectList(_context.Levels, "Id", "Name");
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name");
            return View(subwallets);
        }
        [Authorize(Roles = "bursaryAdmin, superAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubPost(string? id)
        {
            try
            {
                var record = await _context.UgSubWallets
               .FirstOrDefaultAsync(c => c.WalletId == id);

                if (await TryUpdateModelAsync<UgSubWallet>(record, "", c => c.Name, c => c.WalletId, c => c.CreditBalance, 
                    c => c.Tuition, c => c.Tuition2, c => c.FortyPercent, c => c.SixtyPercent, c => c.LMS, 
                    c => c.AcceptanceFee, c => c.SRC, c => c.EDHIS, c => c.SessionId, c => c.Level, c => c.Department))
                {
                    record.SixtyPercent = record.Tuition * 60 / 100;
                    record.FortyPercent = record.Tuition * 40 / 100;

                    record.Tuition -= record.CreditBalance;
                    record.Debit = record.Tuition + record.Tuition2 + record.AcceptanceFee + record.SRC + record.LMS + record.EDHIS;
                    
                    var mainw = _context.UgMainWallets.Where(x => x.WalletId == id).FirstOrDefault();
                    mainw.BulkDebitBalanace = record.Debit;
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
                    return RedirectToAction("mainwallets", "bursaryclearances");

                }
            }
            catch (Exception ex)
            {
                ex.ToString();

            }
            return View();

        }
        [Authorize(Roles = "bursaryClearance, bursaryAdmin, superAdmin")]
        public async Task<IActionResult> Students()
        {
            var applicationDbContext = _context.BursaryClearances.Include(b => b.Payments).ThenInclude(i => i.Sessions).Include(b => b.Students);
            return View(await applicationDbContext.ToListAsync());
        }
        //[Authorize(Roles = "bursaryClearance, bursaryAdmin, superAdmin")]
        //public async Task<IActionResult> Freshers()
        //{
        //    var applicationDbContext = _context.BursaryClearancesFreshers.Include(b => b.Payments).ThenInclude(i => i.Sessions).Include(b => b.Payments).ThenInclude(i => i.Wallets);
        //    return View(await applicationDbContext.ToListAsync());
        //}
        [Authorize(Roles = "student")]
        public async Task<IActionResult> Preview(string? id)
        {
            try
            {
                var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);
                var userId = loggedInUser.StudentsId;
                var student = (from s in _context.Students where s.Id == userId select s)
                    .Include(c => c.Departments).Include(c => c.Levels).Include(c => c.Sessions).Include(c => c.Programs).FirstOrDefault();
                ViewBag.name = student.Fullname;
                ViewBag.email = student.SchoolEmailAddress;
                ViewBag.mat = student.MatNumber;
                ViewBag.dept = student.Departments.Name;
                //ViewBag.programme = student.Programs.NameOfProgram;
                ViewBag.session = student.Sessions.Name;
                ViewBag.level = student.Levels.Name;
                var clearance = (from s in _context.BursaryClearances where s.StudentId == userId && s.Sessions.Name == id select s).Include(i => i.Hostels).Include(i => i.Payments).ThenInclude(i => i.OtherFees).ThenInclude(i => i.Sessions).ToList();
                var walletId = await _context.UgMainWallets.Where(x => x.WalletId == student.UTMENumber).FirstOrDefaultAsync();
                var room = await _context.HostelAllocations.Where(x => x.WalletId == walletId.Id).Include(x => x.HostelRooms).ThenInclude(x => x.Hostels).FirstOrDefaultAsync();
                ViewBag.hostel = room.HostelRooms.Hostels.Name;
                ViewBag.room = room.HostelRooms.RoomNo;
                //if (clearance.Count() == 0)
                //{
                //    return RedirectToAction("resourcenotfound", "error");
                //}
                return View(clearance);
            }
            catch (Exception)
            {
                return RedirectToAction("badreq", "error");
                throw;
            }
            
        }
      //  [Authorize(Roles = "student")]
        public async Task<IActionResult> History()
        {

            var sessions = (from c in _context.Sessions select c);
            return View(await sessions.ToListAsync());
        }
        // GET: busaryClearances/Details/5
        [Authorize(Roles = "bursaryClearance, bursaryAdmin, superAdmin")]
        public async Task<IActionResult> Details(string? id)
        {
            try
            {
                var student = (from s in _context.Students where s.SchoolEmailAddress == id select s).Include(i => i.Departments).Include(x => x.Levels).FirstOrDefault();
                ViewBag.name = student.Fullname;
                ViewBag.mat = student.MatNumber;
                ViewBag.utme = student.UTMENumber;
                ViewBag.phone = student.Phone;
                ViewBag.level = student.Levels.Name;
                ViewBag.department = student.Departments.Name;
                ViewBag.email = student.SchoolEmailAddress;
                ViewBag.currentStatus = (from c in _context.BursaryClearedStudents where c.StudentId == student.Id select c.Remark).FirstOrDefault();
                var clearances = (from c in _context.BursaryClearances where c.Students.SchoolEmailAddress == id select c).Include(i => i.Payments).ThenInclude(i => i.OtherFees).ToList();

                if (clearances == null)
                {
                    return RedirectToAction("PageNotFound", "error");
                }

                return View(clearances);
            }
            catch (Exception)
            {

                throw;
            }
           
        }
        [Authorize(Roles = "bursaryClearance, bursaryAdmin, superAdmin")]
        [HttpPost]
        public async Task<IActionResult> Clearance(ClearanceRemark status, string email, BursaryClearedStudents bs)
        {
            var student = (from v in _context.BursaryClearances where v.Students.SchoolEmailAddress == email select v).FirstOrDefault();
            var studentExist = (from s in _context.BursaryClearedStudents where s.StudentId == student.StudentId && s.SessionId == student.SessionId select s).FirstOrDefault();
            if (studentExist == null)
            {
                bs.StudentId = student.StudentId;
                bs.SessionId = student.SessionId;
                bs.Remark = status;
                bs.CreatedAt = DateTime.Now;
                 _context.Add(bs);
                await _context.SaveChangesAsync();
            }
            else
            {
                studentExist.Remark = status;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "bursaryClearance, bursaryAdmin, superAdmin")]
        public async Task<IActionResult> Fresher(string? id)
        {
            try
            {
                var student = (from s in _context.UgApplicants where s.UTMENumber == id select s).Include(i => i.Departments).FirstOrDefault();
                ViewBag.name = student.Surname + " " + student.FirstName + " " + student.OtherName;
                ViewBag.utme = student.UTMENumber;
                ViewBag.department = student.Departments.Name;
                ViewBag.email = student.Email;
               // ViewBag.currentStatus = (from c in _context.BursaryClearedStudents where c.StudentId == student.Id select c.Remark).FirstOrDefault();
                var clearances = (from c in _context.BursaryClearancesFreshers where c.ClearanceId == id select c).Include(i => i.Payments).ThenInclude(i => i.OtherFees).ToList();

                if (clearances == null)
                {
                    return RedirectToAction("PageNotFound", "error");
                }

                return View(clearances);
            }
            catch (Exception)
            {

                throw;
            }

        }
        // GET: busaryClearances/Create
        //  [Authorize(Roles = "student")]
        public IActionResult Offline()
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
        public async Task<IActionResult> Offline(OfflinePaymentClearance offlinePayment)
        {
            var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);
            var user = loggedInUser.StudentsId;
            offlinePayment.StudentId = user;
            offlinePayment.CreatedAt = DateTime.Now;
           _context.Add(offlinePayment);
            await _context.SaveChangesAsync();
            TempData["success"] = "Offline Payment Added succesfully.";
            return RedirectToAction(nameof(Offline));
          
        }

        // GET: busaryClearances/Create
        [Authorize(Roles = "superAdmin")]
        public IActionResult Create()
        {
            ViewData["PaymentId"] = new SelectList(_context.Payments, "Id", "Id");
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Id");
            return View();
        }

        // POST: busaryClearances/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClearanceId,StudentId,PaymentId,CreatedAt")] BursaryClearance busaryClearance)
        {
            if (ModelState.IsValid)
            {
                _context.Add(busaryClearance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PaymentId"] = new SelectList(_context.Payments, "Id", "Id", busaryClearance.PaymentId);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Id", busaryClearance.StudentId);
            return View(busaryClearance);
        }

        // GET: busaryClearances/Edit/5
        [Authorize(Roles = "superAdmin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.BursaryClearances == null)
            {
                return NotFound();
            }

            var busaryClearance = await _context.BursaryClearances.FindAsync(id);
            if (busaryClearance == null)
            {
                return NotFound();
            }
            ViewData["PaymentId"] = new SelectList(_context.Payments, "Id", "Id", busaryClearance.PaymentId);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Id", busaryClearance.StudentId);
            return View(busaryClearance);
        }

        // POST: busaryClearances/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClearanceId,StudentId,PaymentId,CreatedAt")] BursaryClearance busaryClearance)
        {
            if (id != busaryClearance.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(busaryClearance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BusaryClearanceExists(busaryClearance.Id))
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
            ViewData["PaymentId"] = new SelectList(_context.Payments, "Id", "Id", busaryClearance.PaymentId);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Id", busaryClearance.StudentId);
            return View(busaryClearance);
        }

        // GET: busaryClearances/Delete/5
        [Authorize(Roles = "bursaryClearance, bursaryAdmin, superAdmin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.BursaryClearances == null)
            {
                return NotFound();
            }

            var busaryClearance = await _context.BursaryClearances
                .Include(b => b.Payments)
                .Include(b => b.Students)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (busaryClearance == null)
            {
                return NotFound();
            }

            return View(busaryClearance);
        }
        [Authorize(Roles = "bursaryClearance, bursaryAdmin, superAdmin")]
        // POST: busaryClearances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.BursaryClearances == null)
            {
                return Problem("Entity set 'ApplicationDbContext.BusaryClearances'  is null.");
            }
            var busaryClearance = await _context.BursaryClearances.FindAsync(id);
            if (busaryClearance != null)
            {
                _context.BursaryClearances.Remove(busaryClearance);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BusaryClearanceExists(int? id)
        {
          return _context.BursaryClearances.Any(e => e.Id == id);
        }
    }
}
