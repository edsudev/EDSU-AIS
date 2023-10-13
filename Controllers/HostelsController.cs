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
using Microsoft.AspNetCore.Routing.Template;

namespace EDSU_SYSTEM.Controllers
{
   //[Authorize(Roles = "superAdmin")]
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
        //[HttpGet]
        //public IActionResult GetRoomsList(int hostelId)
        //{
        //    // Query your database or data source to get the rooms for the selected hostel
        //    // Replace this with your actual data retrieval logic
        //    var rooms = GetRoomsForHostel(hostelId);

        //    // Create a JSON response with room options
        //    var roomOptions = rooms.Select(r => new { Value = r.Id, Text = r.Name });

        //    return Json(roomOptions);
        //}

        public async Task<IActionResult> GetRoom(string? utme, HostelAllocation hostel)
        {
            ViewBag.success = TempData["success"];
            ViewBag.error = TempData["error"];
            var wallet = (from s in _context.UgMainWallets where s.WalletId == utme select s).FirstOrDefault();
            var subwallet = (from s in _context.UgSubWallets where s.WalletId == utme select s).FirstOrDefault();
            var allocation = (from s in _context.HostelAllocations where s.WalletId == wallet.Id select s).FirstOrDefault();
            if(allocation != null)
            {
                TempData["error"] = "Chief, You already have a room";
                return View();
            }
            else
            {

                var pmt = _context.HostelPayments.Where(hostel => (hostel.WalletId == wallet.Id || hostel.WalletId == subwallet.Id) && hostel.Status != "Pending").FirstOrDefault();
                if (pmt == null)
                {
                    TempData["error"] = "You must make hostel payment to get a room. If you already paid, contact the ICT";
                    return View();
                }
                var availableHostel = _context.Hostels.Where(hostel => hostel.Id == pmt.HostelType).FirstOrDefault();

                if (availableHostel.BedspacesCount > 0)
                {
                    var availableRooms = _context.HostelRoomDetails.Where(rm => rm.HostelId == pmt.HostelType && rm.BedSpacesCount > 0).ToList();

                    var random = new Random();
                    var shuffledRooms = availableRooms.OrderBy(x => random.Next()).ToList();

                    foreach (var item in shuffledRooms)
                    {
                       
                        hostel.WalletId = wallet.Id;
                        hostel.RoomId = item.Id;
                        hostel.HostelId = item.HostelId;
                        hostel.CreatedAt = DateTime.Now;
                        _context.HostelAllocations.Add(hostel);
                        item.BedSpacesCount -= 1;
                        _context.HostelRoomDetails.Update(item);
                        availableHostel.BedspacesCount -= 1;
                        _context.Hostels.Update(availableHostel);
                        await _context.SaveChangesAsync();
                        TempData["Success"] = "Your room has been allocated to you! Check you clearance slip";
                        return RedirectToAction("getroom", "hostels", new {utme});
                        //break;

                    }
                }
                
            }
             //ViewBag.success = TempData["Success"];
             //   ViewBag.error = TempData["error"];
            return View();
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
        public async Task<IActionResult> ApplicantChoice(string utme)
        {
            //Check if the student owes anything
            var Subwallet = (from sf in _context.UgSubWallets where sf.WalletId == utme select sf).FirstOrDefault();
            if (Subwallet != null)
            {
                if((Subwallet.Tuition2 > 0 || Subwallet.SixtyPercent > 0 || Subwallet.LMS > 0 || Subwallet.EDHIS > 0 || Subwallet.SRC > 0) && Subwallet.Waiver == false)
                {
                    ViewBag.ErrorMessage = "Kindly clear all outstanding debts and try again. For further complaints, contact the ICT";
                    return View();
                }
            }
            var wallet = (from sf in _context.UgMainWallets where sf.WalletId == utme select sf.Id).FirstOrDefault();
            
            ViewData["HostelId"] = new SelectList(_context.Hostels.Where(x => x.Id != 5), "Id", "Name");
            var student = (from s in _context.UgSubWallets where s.WalletId == utme select s).FirstOrDefault();
            if (student != null)
            {
                string err = (string)TempData["err"];
                ViewBag.ErrorMessage = err;
                TempData["utme"] = utme;
                return View();
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }
            
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplicantChoice(int hostel, string utme)
        {
            try
            {
                utme = (string)TempData["utme"];
                var student = (from st in _context.UgApplicants where st.UTMENumber == utme select st).FirstOrDefault();

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
                return RedirectToAction("order", "hostels", new {utme});
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

                var student = (from st in _context.UgApplicants where st.UTMENumber == utme select st).FirstOrDefault();
                ViewBag.Name = student.FirstName + " " + student.Surname;
                // I did this because the walletId is same as the student UTME Number
                var wallet = await _context.UgSubWallets
                .FirstOrDefaultAsync(m => m.WalletId == student.UTMENumber);
                if (wallet == null)
                {
                    TempData["err"] = "Student doesn't have a wallet, contact the bursary.";
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                var sessionId = (from ss in _context.Sessions where ss.IsActive == true select ss.Id).FirstOrDefault();
                Random r = new();
                //ViewBag.hostel =
               payment.SessionId = sessionId;
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

            var paymentToUpdate = await _context.HostelPayments.Where(i => i.Ref == reference).Include(i => i.HostelFees).Include(i => i.Wallets).FirstOrDefaultAsync();
           
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
                TempData["walletId"] = payment.WalletId;
                if (payment != null)
                {
                    //Update the payment record if the payment is successful
                    payment.Status = status;
                    payment.Mode = "Rave";
                    await _context.SaveChangesAsync();


                    var wlt = _context.UgSubWallets.Where(e => e.Id == payment.WalletId).FirstOrDefault();

                    //Get the student making payment
                    var hostelApplicant = _context.UgApplicants.Where(ha => ha.UTMENumber == wlt.WalletId).FirstOrDefault();

                    var availableHostel = _context.Hostels.Where(hostel => hostel.Id == payment.HostelType).FirstOrDefault();

                    if (availableHostel.BedspacesCount > 0)
                    {
                        var availableRooms = _context.HostelRoomDetails.Where(rm => rm.HostelId == payment.HostelType && rm.BedSpacesCount > 0).ToList();

                        var random = new Random();
                        var shuffledRooms = availableRooms.OrderBy(x => random.Next()).ToList();


                        var alreadyPaid = _context.HostelAllocations.Where(sd => sd.WalletId == wlt.Id).FirstOrDefault();
                        if (alreadyPaid != null)
                        {
                            ViewBag.ErrorMessage = "Check your clearance slip for your room details. For further complaints, contact the ICT";
                            return View();
                        }


                        foreach (var item in shuffledRooms)
                        {
                           
                            var roomFound = false;
                           // var eligible4room = _context.HostelAllocations.Where(er => er.RoomId == item.Id).Include(x => x.UgMainWallets).ToList();
                            var allocationsToAdd = new List<int?>();
                            //foreach (var i in eligible4room)
                            //{
                            //    var studentDept = _context.UgSubWallets.Where( dt =>dt.WalletId == wlt.WalletId).FirstOrDefault();
                            //    allocationsToAdd.Add(studentDept.Level);

                            //}
                           // var applicantLevel = hostelApplicant.LevelAdmittedTo;

                            //if (!allocationsToAdd.Any(allocatedLevel => allocatedLevel == applicantLevel))
                            //{
                                var ugmainwallet = _context.UgMainWallets.Where(dt => dt.WalletId == wlt.WalletId).FirstOrDefault();

                                // If no existing allocations are found in the same department or level, add a new allocation.
                                haa.WalletId = ugmainwallet.Id;
                                haa.RoomId = item.Id;
                                haa.HostelId = item.HostelId;
                                haa.CreatedAt = DateTime.Now;
                                _context.HostelAllocations.Add(haa);
                                item.BedSpacesCount -= 1;
                                _context.HostelRoomDetails.Update(item);
                                availableHostel.BedspacesCount -= 1;
                                _context.Hostels.Update(availableHostel);
                                await _context.SaveChangesAsync();
                                
                            //}

                        }
                    }
                }
                else
                {
                    
                    return RedirectToAction("badreq", "error");
                }
                
            }
            catch (Exception e)
            {
                TempData["err"] = e.Message;
                throw;
            }
           
            return RedirectToAction("Success", "Hostels");
        }
        public async Task<IActionResult> Success(int utme)
        {
            utme = (int)TempData["walletId"];
            Console.Write("utme number" + utme);
            var walletid = _context.UgSubWallets.Where(x => x.Id == utme).FirstOrDefault();
            var main = _context.UgMainWallets.Where(x => x.WalletId == walletid.WalletId).FirstOrDefault();
            var room = _context.HostelAllocations.Where(s => s.WalletId == main.Id) 
                .Include(x => x.Hostels)
                .Include(x => x.HostelRooms)
                .Include(x => x.UgMainWallets)
                .FirstOrDefault();
            return View(room);
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
        [Authorize(Roles = "busaryAdmin, superAdmin")]
        public async Task<IActionResult> Allocations()
        {
            var all = _context.HostelAllocations.Include(x => x.Hostels).Include(s => s.HostelRooms).Include(x => x.UgMainWallets).ToList();
            return View(all);
        }
        [Authorize(Roles = "busaryAdmin, superAdmin")]
        // GET: Hostels/Create
        public IActionResult Back()
        {
            ViewData["hostel"] = new SelectList(_context.Hostels, "Id", "Name");
            ViewData["room"] = new SelectList(_context.HostelRoomDetails, "Id", "RoomNo");
            ViewData["wallet"] = new SelectList(_context.UgMainWallets, "Id", "Name");
            return View();
        }
        [Authorize(Roles = "busaryAdmin, superAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Back(HostelAllocation hostel)
        {
            hostel.CreatedAt = DateTime.Now;
            _context.Add(hostel);
            await _context.SaveChangesAsync();

            var hall = (from h in _context.Hostels where h.Id == hostel.HostelId select h).FirstOrDefault();
            var hallRoom = (from h in _context.HostelRoomDetails where h.Id == hostel.RoomId select h).FirstOrDefault();
            hall.BedspacesCount -= 1;
            _context.Hostels.Update(hall);

            hallRoom.BedSpacesCount -= 1;
            _context.HostelRoomDetails.Update(hallRoom);

            await _context.SaveChangesAsync();

            
           
            return RedirectToAction(nameof(Allocations));
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
            if (id == null || _context.HostelAllocations == null)
            {
                return NotFound();
            }

            var hostel = await _context.HostelAllocations
                .Include(h => h.HostelRooms)
                .Include(h => h.UgMainWallets)
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
            if (_context.HostelAllocations == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Hostels'  is null.");
            }
            var hostel = await _context.HostelAllocations.FindAsync(id);
            if (hostel != null)
            {
                _context.HostelAllocations.Remove(hostel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Allocations));
        }

        private bool HostelExists(int? id)
        {
            return _context.Hostels.Any(e => e.Id == id);
        }

        ////////////////////////////////////////////////////////////////////////
        ///
        ////////////////////////////////////////////////////////////////////////
        public async Task<IActionResult> Choice(string utme)
        {
            ViewBag.utme = utme;
            //Check if the student owes anything
            var Subwallet = (from sf in _context.UgSubWallets where sf.WalletId == utme select sf).FirstOrDefault();
            if (Subwallet != null)
            {
                if (Subwallet.Tuition2 > 0 || Subwallet.SixtyPercent > 0 || Subwallet.LMS > 0 || Subwallet.EDHIS > 0 || Subwallet.SRC > 0 && Subwallet.Waiver == false)
                {
                    ViewBag.ErrorMessage = "Kindly clear all outstanding debts and try again. For further complaints, contact the ICT";
                    return View();
                }
            }
            var wallet = (from sf in _context.UgMainWallets where sf.WalletId == utme select sf.Id).FirstOrDefault();
            var alreadyPaid = (from sd in _context.HostelAllocations where sd.WalletId == wallet select sd).FirstOrDefault();
            //if (alreadyPaid != null)
            //{
            //    ViewBag.ErrorMessage = "It Appears you have already been allocated a room. For further complaints, contact the ICT";
            //    return View();
            //}
            ViewData["HostelId"] = new SelectList(_context.Hostels.Where(x =>x.Id != 5), "Id", "Name");
            var student = (from s in _context.UgSubWallets where s.WalletId == utme select s).FirstOrDefault();
            if (student != null)
            {
                string err = (string)TempData["err"];
                ViewBag.ErrorMessage = err;
                TempData["utme"] = utme;
                return View();
            }
            return View();
            //return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Choice(int hostel, string utme)
        {
            try
            {
               // utme = (string)TempData["utme"];
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
                //var utme = hostelIsAvailable.Id;
                return RedirectToAction("hostelorder", "hostels", new { utme });
            }
            catch (Exception)
            {
                throw;

            }

        }

        public async Task<IActionResult> HostelOrder(HostelPayment payment, string utme)
        {
            try
            {
                //Gets the logged in user (Student)
                //var loggedInUser = await _userManager.GetUserAsync(HttpContext.User);
                //var ugStudent = loggedInUser.StudentsId;

                var student = (from st in _context.Students where st.UTMENumber == utme select st).FirstOrDefault();
                ViewBag.Name = student.Fullname;
                // I did this because the walletId is same as the student UTME Number
                var wallet = await _context.UgSubWallets
                .FirstOrDefaultAsync(m => m.WalletId == student.UTMENumber);
                if (wallet == null)
                {
                    TempData["err"] = "Student doesn't have a wallet, contact the bursary.";
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                var sessionId = (from ss in _context.Sessions where ss.IsActive == true select ss.Id).FirstOrDefault();
                Random r = new();
                //ViewBag.hostel =
                payment.SessionId = sessionId;
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
        public async Task<IActionResult> HostelOrder(string? reference)
        {
            Console.Write(reference);
            try
            {
                var PaymentToUpdate = _context.HostelPayments.FirstOrDefault(i => i.Ref == reference);

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
                    return RedirectToAction("checkout", "hostels", new { reference });

                }
            }
            catch (Exception ex)
            {
                ex.ToString();

            }
            return RedirectToAction("checkout", "hostels", new { reference });

        }
        public async Task<IActionResult> Checkout(string reference)
        {

            var paymentToUpdate = _context.HostelPayments.Where(i => i.Ref == reference).Include(i => i.HostelFees).Include(x => x.Wallets).FirstOrDefault();

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
        [HttpGet("/hostels/Rave")]
        public async Task<IActionResult> Rave(string status, string tx_ref, string transaction_id, BursaryClearance bursaryClearance, HostelAllocation haa)
        {
            try
            {
                var payment = _context.HostelPayments.Where(x => x.Ref == tx_ref).FirstOrDefault();
                TempData["walletId"] = payment.WalletId;
                if (payment != null)
                {
                    //Update the payment record if the payment is successful
                    payment.Status = status;
                    payment.Mode = "Rave";
                    await _context.SaveChangesAsync();


                    var wlt = _context.UgSubWallets.Where(e => e.Id == payment.WalletId).FirstOrDefault();

                    //Get the student making payment
                    var hostelApplicant = _context.Students.Where(ha=> ha.UTMENumber == wlt.WalletId).FirstOrDefault();

                    var availableHostel = _context.Hostels.Where(hostel => hostel.Id == payment.HostelType).FirstOrDefault();

                    if (availableHostel.BedspacesCount > 0)
                    {
                        var availableRooms = _context.HostelRoomDetails.Where(rm=> rm.HostelId == payment.HostelType && rm.BedSpacesCount > 0).ToList();



                        var random = new Random();
                        var shuffledRooms = availableRooms.OrderBy(x => random.Next()).ToList();

                        var alreadyPaid = _context.HostelAllocations.Where(sd => sd.WalletId == wlt.Id).FirstOrDefault();
                        if (alreadyPaid != null)
                        {
                            ViewBag.ErrorMessage = "Check your clearance slip for your room details. For further complaints, contact the ICT";
                            return View();
                        }

                        foreach (var item in shuffledRooms)
                        {

                            var roomFound = false;
                            var eligible4room = _context.HostelAllocations.Where(er => er.RoomId == item.Id).Include(x => x.UgMainWallets).ToList();
                            var allocationsToAdd = new List<int?>();
                            //foreach (var i in eligible4room)
                            //{
                            //    var studentDept =  _context.UgSubWallets.Where(dt => dt.WalletId == wlt.WalletId).FirstOrDefault();
                            //    allocationsToAdd.Add(studentDept.Level);

                            //}
                            //var applicantLevel = hostelApplicant.Level;

                            //if (!allocationsToAdd.Any(allocatedLevel => allocatedLevel == applicantLevel))
                            //{
                                var ugmainwallet = _context.UgMainWallets.Where(dt => dt.WalletId == wlt.WalletId).FirstOrDefault();

                                // If no existing allocations are found in the same department or level, add a new allocation.
                                haa.WalletId = ugmainwallet.Id;
                                haa.RoomId = item.Id;
                                haa.HostelId = item.HostelId;
                                haa.CreatedAt = DateTime.Now;
                                _context.HostelAllocations.Add(haa);
                                item.BedSpacesCount -= 1;
                                _context.HostelRoomDetails.Update(item);
                                availableHostel.BedspacesCount -= 1;
                                _context.Hostels.Update(availableHostel);
                                await _context.SaveChangesAsync();

                                
                           // }


                        }
                    }
                }
                else
                {
                    return RedirectToAction("badreq", "error");
                }

            }
            catch (Exception)
            {

                throw;
            }

            return RedirectToAction("Success", "Hostels");
        }


    }
}
