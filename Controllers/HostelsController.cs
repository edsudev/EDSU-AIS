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
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace EDSU_SYSTEM.Controllers
{
  //  [Authorize(Roles = "superAdmin")]
    public class HostelsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HostelsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [Authorize(Roles = "busaryAdmin, superAdmin")]
        // GET: Hostels
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Hostels.Include(h => h.Sessions);
            return View(await applicationDbContext.ToListAsync());
        }
        public IActionResult RoomDetails()
        {
            ViewData["HostelId"] = new SelectList(_context.Hostels, "Id", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RoomDetails(HostelRoomDetails hostel)
        {
            hostel.BedSpacesCount = hostel.BedSpaces;
            hostel.CreatedAt = DateTime.Now;
            _context.Add(hostel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Roomdetailslist));
           
        }
        public async Task<IActionResult> Roomdetailslist()
        {
            var applicationDbContext = _context.HostelRoomDetails.Include(h => h.Hostels);
            return View(await applicationDbContext.ToListAsync());
        }
        public async Task<IActionResult> Choice()
        {
            ViewData["HostelId"] = new SelectList(_context.Hostels, "Id", "Name");
            ViewBag.err = TempData["err"];
            string err = (string)TempData["err"];
            ViewBag.ErrorMessage = err;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Choice(int hostel, string utme)
        {
            try
            {
                //Gets the logged in user (Student)
                //var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);
                //var ugStudent = loggedInUser.StudentsId;


                var student = (from st in _context.Students where st.UTMENumber == utme select st).FirstOrDefault();

                //Checks if this hostel exists
                var hostelIsAvailable = (from s in _context.Hostels where s.Id == hostel select s).FirstOrDefault();
                if (hostelIsAvailable == null)
                {
                    TempData["err"] = "Hostel does not exist. Kindly make another choice.";
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                //Checks if the hostel is for the gender of the logged in user
                var isForMyGender = (from f in _context.Hostels where f.Id == hostel select f.Gender).FirstOrDefault();
                if (isForMyGender != student.Sex)
                {
                    TempData["err"] = "You picked a hostel for the opposite gender😔. Kindly make another choice.";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                //checks if there's a room in this hostel
                if (hostelIsAvailable.BedspacesCount <= 0)
                {
                    TempData["err"] = "Unfortunately the hostel type you picked is not available at this time. Kindly make another choice.";
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                else if (hostelIsAvailable.BedspacesCount > 0)
                {
                    TempData["hostelType"] = hostelIsAvailable.Id;
                    TempData["hostelName"] = hostelIsAvailable.Name;
                    TempData["hostelAmount"] = hostelIsAvailable.Amount;
                }
                var id = hostelIsAvailable.Id;
                return RedirectToAction("order", "hostels");
            }
            catch (Exception)
            {
                throw;

            }

        }

        public async Task<IActionResult> Order(HostelPayment payment, string utme)
        {
            try
            {
                //Gets the logged in user (Student)
                //var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);
                //var ugStudent = loggedInUser.StudentsId;

                var student = (from st in _context.Students where st.UTMENumber == utme select st).FirstOrDefault();
                ViewBag.Name = student.Fullname;
                // I did this because the walletId is same as the student UTME Number
                var wallet = await _context.UgMainWallets
                .FirstOrDefaultAsync(m => m.WalletId == student.UTMENumber);
                if (wallet == null)
                {
                    TempData["err"] = "Student doesn't have a wallet, contact the bursary.";
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                Random r = new();
                //ViewBag.hostel =
                payment.SessionId = student.CurrentSession;
                payment.WalletId = wallet.Id;
                payment.Amount = (double)TempData["hostelAmount"];
                payment.HostelType = (int)TempData["hostelType"];
                payment.Status = "Pending";
                payment.Ref = "EDSU-" + r.Next(10000000) + DateTime.Now.Millisecond;
                payment.PaymentDate = DateTime.Now;
                _context.HostelPayments.Add(payment);
                await _context.SaveChangesAsync();

                ViewBag.reference = payment.Ref;
                ViewBag.hostel = TempData["hostelName"];
                Console.WriteLine(ViewBag.hostel);
                return View();
            }
            catch (Exception)
            {

                throw;
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Order(string? reference)
        {
            Console.Write(reference);
            try
            {
                var PaymentToUpdate = await _context.HostelPayments.FirstOrDefaultAsync(i => i.Ref == reference);

                if (await TryUpdateModelAsync<HostelPayment>(PaymentToUpdate, "", c => c.Email))
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
                    return RedirectToAction("OrderCheckout", "hostels", new { reference });

                }
            }
            catch (Exception ex)
            {
                ex.ToString();

            }
            return RedirectToAction("OrderCheckout", "hostels", new { reference });

        }
        public async Task<IActionResult> OrderCheckout(string reference)
        {

            var paymentToUpdate = _context.HostelPayments.Where(i => i.Ref == reference).Include(i => i.HostelFees).FirstOrDefault();
            ViewBag.pk = "FLWPUBK_TEST-3f35866dc8566ccf6b5b8a468536f069-X";
            if (reference == null || _context.HostelPayments == null)
            {
                return NotFound();
            }
            if (paymentToUpdate == null)
            {
                return NotFound();
            }

            return View(paymentToUpdate);
        }
        [HttpGet("/hostels/RaveRedirect")]
        public async Task<IActionResult> RaveRedirect(string status, string tx_ref, string transaction_id, BursaryClearance bursaryClearance, HostelAllocation haa)
        {
            try
            {
                var payment = _context.HostelPayments.Where(x => x.Ref == tx_ref).FirstOrDefault();
                if (payment != null)
                {
                    //Update the payment record if the payment is successful
                    payment.Status = status;
                    payment.Mode = "Rave";
                    _context.SaveChangesAsync();

                   
                    var wlt = (from e in _context.UgSubWallets where e.Id == payment.WalletId select e).FirstOrDefault();
                   
                    //Get the student making payment
                    var hostelApplicant = (from ha in _context.Students where ha.UTMENumber == wlt.WalletId select ha).FirstOrDefault();
                   
                    var availableHostel = (from hostel in _context.Hostels where hostel.Id == payment.HostelType select hostel).FirstOrDefault();
                  
                    if (availableHostel.BedspacesCount > 0)
                    {
                        var availableRooms = (from rm in _context.HostelRoomDetails where rm.HostelId == payment.HostelType && rm.BedSpacesCount > 0 select rm).ToList();



                        foreach (var item in availableRooms)
                        {
                            Console.WriteLine("Room avail " + item.HostelId);
                            var roomFound = false;
                            var eligible4room = (from er in _context.HostelAllocations where er.RoomIdId == item.Id select er).ToList();
                            var allocationsToAdd = new List<int?>();
                            foreach (var i in eligible4room)
                            {
                                var student = (from st in _context.Students where st.UTMENumber == i.UgMainWallets.UTME select st).FirstOrDefault();
                                allocationsToAdd.Add(student.Department);

                            }
                            if (!allocationsToAdd.Contains(hostelApplicant.Department))
                            {
                                //work here
                                haa.WalletId = hostelApplicant.Id;
                                haa.RoomIdId = item.Id;
                                haa.HostelId = item.HostelId;

                                _context.HostelAllocations.Add(haa);
                                await _context.SaveChangesAsync();
                            }


                        }
                    }
                }
                else
                {
                    Console.Write($"else tx_ref: {tx_ref}");
                }
            }
            catch (Exception)
            {

                throw;
            }
            return RedirectToAction("Index", "Wallets");
        }
        public IActionResult Receipt()
        {
            //var d = _context.HostelPayments.Where(x => x.Ref == Ref).FirstOrDefault();
            ViewBag.PaymentRef = TempData["PaymentRef"];
            ViewBag.ReceiptNo = TempData["ReceiptNo"];
            ViewBag.Date = TempData["PaymentDate"];
            ViewBag.Name = TempData["PaymentName"];
            ViewBag.Email = TempData["PaymentEmail"];
            ViewBag.UTME = TempData["PaymentUTME"];
            ViewBag.Department = TempData["PaymentDepartment"];
            ViewBag.Session = TempData["PaymentSession"];
            ViewBag.Amount = TempData["PaymentAmount"];
            ViewBag.Description = TempData["PaymentDescription"];
            ViewBag.WalletId = TempData["PaymentWalletId"];

            Console.WriteLine("update receipt with " + ViewBag.PaymentRef);
            return View();
        }
        // GET: Hostels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Hostels == null)
            {
                return NotFound();
            }

            var hostel = await _context.Hostels
                .Include(h => h.Sessions)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hostel == null)
            {
                return NotFound();
            }

            return View(hostel);
        }
        [Authorize(Roles = "busaryAdmin, superAdmin")]
        // GET: Hostels/Create
        public IActionResult Create()
        {
            ViewData["SessionId"] = new SelectList(_context.Sessions, "Id", "Name");
            return View();
        }

        // POST: Hostels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "busaryAdmin, superAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Hostel hostel)
        {

            _context.Add(hostel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

            ViewData["SessionId"] = new SelectList(_context.Sessions, "Id", "Name", hostel.SessionId);
            return View(hostel);
        }

        // GET: Hostels/Edit/5
        [Authorize(Roles = "busaryAdmin, superAdmin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Hostels == null)
            {
                return NotFound();
            }

            var hostel = await _context.Hostels.FindAsync(id);
            if (hostel == null)
            {
                return NotFound();
            }
            ViewData["SessionId"] = new SelectList(_context.Sessions, "Id", "Name", hostel.SessionId);
            return View(hostel);
        }

        // POST: Hostels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "busaryAdmin, superAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Hostel hostel)
        {
            if (id != hostel.Id)
            {
                return NotFound();
            }


            try
            {
                _context.Update(hostel);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HostelExists(hostel.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));

            ViewData["SessionId"] = new SelectList(_context.Sessions, "Id", "Name", hostel.SessionId);
            return View(hostel);
        }

        // GET: Hostels/Delete/5
        [Authorize(Roles = "busaryAdmin, superAdmin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Hostels == null)
            {
                return NotFound();
            }

            var hostel = await _context.Hostels
                .Include(h => h.Sessions)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hostel == null)
            {
                return NotFound();
            }

            return View(hostel);
        }
        [Authorize(Roles = "busaryAdmin, superAdmin")]
        // POST: Hostels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Hostels == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Hostels'  is null.");
            }
            var hostel = await _context.Hostels.FindAsync(id);
            if (hostel != null)
            {
                _context.Hostels.Remove(hostel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HostelExists(int? id)
        {
            return _context.Hostels.Any(e => e.Id == id);
        }
    }
}
