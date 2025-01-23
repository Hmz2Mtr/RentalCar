using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentalCar.Data;
using RentalCar.Models;
using RentalCar.Models.Domain;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace RentalCar.Controllers
{
    [Authorize] // Only authenticated users can access booking functionality
    public class BookingController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingController(UserManager<ApplicationUser> userManager, DatabaseContext context)
        {
            _userManager = userManager;
            _context = context;
        }






        // GET: /Booking/Index
        public async Task<IActionResult> Index()
        {
            var bookings = await _context.Bookings
                .Include(b => b.UserID)
                .Include(b => b.CarID)
                .ToListAsync();

            //var cars = _context.Cars;
            return View(bookings);
        }


















        // GET: /Booking/Create
        public async Task<IActionResult> Create(int? carId)
        {
            if (carId == null)
            {
                return NotFound();
            }

            var car = _context.Cars.Find(carId);
            if (car == null)
            {
                return NotFound();
            }


            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            ViewBag.User = user;


            // Pass the car information to the view
            ViewBag.Car = car;
            return View();
        }


        // POST: /Booking/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if not authenticated
            }

            if (ModelState.IsValid)
            {
               

                // Get the current user
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user == null)
                {
                    return NotFound();
                }

                // Get the car details
                var car = await _context.Cars.FindAsync(booking.CarID);
                if (car == null)
                {
                    return NotFound();
                }



                // Validate that StartDate is not later than EndDate
                if (booking.StartDate > booking.EndDate)
                {
                    TempData["ErrorMessage"] = "The start date cannot be later than the end date.";
                    ViewBag.Car = car;
                    ViewBag.User = user;
                    return View();
                }


                // Check if the car is already booked for the selected dates
                var isCarBooked = await _context.Bookings
                    .AnyAsync(b => b.CarID == booking.CarID &&
                                   !(booking.EndDate < b.StartDate || booking.StartDate > b.EndDate));

                if (isCarBooked)
                {
                    TempData["ErrorMessage"] = "The car is already booked for the selected dates.";
                    ViewBag.Car = car;
                    ViewBag.User = user;
                    return View();
                }

                // Get the current date
                DateOnly currentDate = DateOnly.FromDateTime(DateTime.Today);
                DateOnly userDate = booking.EndDate;

                // Check if the booking date is in the past
                if (userDate < currentDate)
                {
                    TempData["ErrorMessage"] = "The booking date cannot be later than today.";
                    ViewBag.Car = car;
                    ViewBag.User = user;
                    return View();
                }





                try
                {
                    _context.Add(booking);
                    await _context.SaveChangesAsync();
                    //return RedirectToAction(nameof(Index));
                    return RedirectToAction("Cars", "Home");
                }
                catch (Exception ex)
                {
                    // Log the error (e.g., using a logging framework)
                    ModelState.AddModelError("", "An error occurred while saving the booking.");
                }
            }

            // If validation fails, re-populate the car information
            ViewBag.Car = await _context.Cars.FindAsync(booking.CarID);
            return View(booking);
        }












        // GET: /Booking/Delete/{id}
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.UserID)
                .Include(b => b.CarID)
                .FirstOrDefaultAsync(b => b.BookingID == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: /Booking/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}