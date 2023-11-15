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

namespace EDSU_SYSTEM.Controllers
{
    public class EuslController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EuslController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult GetPaymentKey()
        {
            var paymentKey = Environment.GetEnvironmentVariable("PAYSTACK_EUSL_LIVE_KEY");
            return Json(paymentKey);
        }
        // GET: Eusl
        public async Task<IActionResult> Index()
        {
            ViewBag.success = TempData["suceess"];
            return View();
        }
        public async Task<IActionResult> Assets()
        {
            var applicationDbContext = _context.EuslAssests;
            return View(await applicationDbContext.ToListAsync());
        }
        public async Task<IActionResult> Studentitems()
        {
            var applicationDbContext = _context.StudentManuals;
            return View(await applicationDbContext.ToListAsync());
        }
        public async Task<IActionResult> Applications()
        {
            var applicationDbContext = _context.AssetFinances.Include(a => a.EuslAssests);
            return View(await applicationDbContext.ToListAsync());
        }
        public async Task<IActionResult> Manual()
        {
            ViewData["TypeOfAsset"] = new SelectList(_context.StudentManuals, "Id", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manual(string name, string email, int item, EuslP payment)
        {
            var amount = (from s in _context.StudentManuals where s.Id == item select s.Amount).FirstOrDefault();
            Random r = new();

            var percentage = (double?)amount * 1.5 / 100;
            Console.Write("This is it" + percentage);
            payment.Email = email;
            payment.PayerName = name;
            payment.Type = item;
            payment.PaymentDate = DateTime.Now;
            payment.Status = "Pending";
            payment.Mode = "Paystack";
            if (amount > 2500)
            {
                payment.Amount = (double?)amount + percentage + 100;
            }
            else
            {
                payment.Amount = (double?)amount + percentage;
            }
            
            payment.Ref = "EUSL-" + r.Next(10000000) + DateTime.Now.Millisecond;

            _context.Add(payment);
            await _context.SaveChangesAsync();
            
            ViewData["TypeOfAsset"] = new SelectList(_context.StudentManuals, "Id", "Id");
            return RedirectToAction("ordercheckout","eusl", new {payment.Ref});
           
        }
        public async Task<IActionResult> OrderCheckout(string Ref)
        {
            var paymentToUpdate = _context.EuslPs.Where(i => i.Ref == Ref).FirstOrDefault();
            ViewBag.name = paymentToUpdate.PayerName;
            if (Ref == null || _context.EuslPs == null)
            {
                return NotFound();
            }
            if (paymentToUpdate == null)
            {
                return NotFound();
            }

            return View(paymentToUpdate);
        }
        // GET: Eusl/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.AssetFinances == null)
            {
                return NotFound();
            }

            var assetFinance = await _context.AssetFinances
                .Include(a => a.EuslAssests)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assetFinance == null)
            {
                return NotFound();
            }

            return View(assetFinance);
        }

        // GET: Eusl/Create
        public IActionResult AssetFinanceForm()
        {
            ViewData["TypeOfAsset"] = new SelectList(_context.EuslAssests, "Id", "Name");
            return View();
        }

        // POST: Eusl/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssetFinanceForm(AssetFinance assetFinance)
        {
            
            _context.Add(assetFinance);
            await _context.SaveChangesAsync();
            TempData["success"] = "You have succesfully applied for an asset finance. We will get back to you in no time.";
            return RedirectToAction(nameof(Index));
        }
        public IActionResult CreateItem()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateItem(StudentManuals sm)
        {

            _context.Add(sm);
            await _context.SaveChangesAsync();
            TempData["success"] = "You have succesfully added an item";
            return RedirectToAction(nameof(Index));
        }
        public IActionResult CreateAsset()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsset(EuslAssests asset)
        {

            _context.Add(asset);
            await _context.SaveChangesAsync();
            TempData["success"] = "You have succesfully added an item";
            return RedirectToAction(nameof(Index));
        }
        // GET: Eusl/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.EuslAssests == null)
            {
                return NotFound();
            }

            var assetFinance = await _context.EuslAssests.FindAsync(id);
            if (assetFinance == null)
            {
                return NotFound();
            }
            //ViewData["TypeOfAsset"] = new SelectList(_context.EuslAssests, "Id", "Id", assetFinance.TypeOfAsset);
            return View(assetFinance);
        }

        // POST: Eusl/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, EuslAssests asset)
        {
            if (id != asset.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(asset);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {

                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            //ViewData["TypeOfAsset"] = new SelectList(_context.EuslAssests, "Id", "Id", asset.TypeOfAsset);
            return View(asset);
        }
        public async Task<IActionResult> Editstudentitem(int? id)
        {
            if (id == null || _context.StudentManuals == null)
            {
                return NotFound();
            }

            var assetFinance = await _context.StudentManuals.FindAsync(id);
            if (assetFinance == null)
            {
                return NotFound();
            }
            //ViewData["TypeOfAsset"] = new SelectList(_context.EuslAssests, "Id", "Id", assetFinance.TypeOfAsset);
            return View(assetFinance);
        }

        // POST: Eusl/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editstudentitem(int? id, StudentManuals asset)
        {
            if (id != asset.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(asset);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {

                    return NotFound();
                }
                return RedirectToAction(nameof(Studentitems));
            }
            //ViewData["TypeOfAsset"] = new SelectList(_context.EuslAssests, "Id", "Id", asset.TypeOfAsset);
            return View(asset);
        }
        // GET: Eusl/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.EuslAssests == null)
            {
                return NotFound();
            }

            var assetFinance = await _context.EuslAssests
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assetFinance == null)
            {
                return NotFound();
            }

            return View(assetFinance);
        }

        // POST: Eusl/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.EuslAssests == null)
            {
                return Problem("Entity set 'ApplicationDbContext.AssetFinances'  is null.");
            }
            var assetFinance = await _context.EuslAssests.FindAsync(id);
            if (assetFinance != null)
            {
                _context.EuslAssests.Remove(assetFinance);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> UpdatePayment(string data)
        {
            var payments = _context.EuslPs.FirstOrDefault(c => c.Ref == data);
            payments.Status = "Approved";
            await _context.SaveChangesAsync();
            return RedirectToAction("summary", "eusl", new {data});
        }
        public IActionResult Summary(string data)
        {
            var payment = (from s in _context.EuslPs where s.Ref == data && s.Status == "Approved" select s).Include(x => x.StudentManuals).FirstOrDefault();
            return View(payment);
        }
        private bool AssetFinanceExists(int id)
        {
          return (_context.EuslAssests?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
