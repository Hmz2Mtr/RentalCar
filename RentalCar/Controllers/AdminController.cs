using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentalCar.Data;
using RentalCar.Models.Domain;

namespace RentalCar.Controllers
{
    [Authorize(Roles = "admin")] // Restrict access to users with the "Admin" role
    public class AdminController : Controller
    {
        
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DatabaseContext _context;

        public AdminController(UserManager<ApplicationUser> userManager, DatabaseContext context)
        {
            _userManager = userManager;
            _context = context;
        }


        public IActionResult Dashboard()
        {
            return View();
        }












        // GET: /Booking/Index
        public async Task<IActionResult> AllBookedCars()
        {
            var bookings = await _context.Bookings
                .ToListAsync();

            return View(bookings);
        }

        // GET: /Booking/Delete/{id}
        public async Task<IActionResult> DeleteBooking(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingID == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: /Booking/Delete/{id}
        [HttpPost, ActionName("DeleteBooking")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction("AllBookedCars", "Admin");
        }

















        // GET: /Booking/Index
        public async Task<IActionResult> RemainingBookedCars()
        {
            // Get the current date as DateOnly
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Today);

            var bookings = await _context.Bookings
                .Where(b => b.EndDate >= currentDate)
                .ToListAsync();

            return View(bookings);
        }



        // GET: /Booking/Delete/{id}
        public async Task<IActionResult> DeleteRemainingBooking(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingID == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: /Booking/Delete/{id}
        [HttpPost, ActionName("DeleteRemainingBooking")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRemainingConfirmed(int? id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction("RemainingBookedCars", "Admin");
        }

    }
}