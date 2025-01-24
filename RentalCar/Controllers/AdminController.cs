using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using RentalCar.Data;
using RentalCar.Models.Domain;

using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using System.IO;

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
            // Fetch stats
            ViewBag.TotalCars = _context.Cars.Count();
            ViewBag.TotalClients = _context.Users.Count();
            ViewBag.TotalBookings = _context.Bookings.Count();
            ViewBag.TotalRevenue = _context.Bookings.Sum(b => b.TotalPrice);

            // Fetch recent bookings
            ViewBag.RecentBookings = _context.Bookings
                .OrderByDescending(b => b.BookingID)
                .Take(5)
                .ToList();

            // Fetch data for the Cars Revenue Chart
            var carsRevenueData = _context.Bookings
                .Join(
                    _context.Cars, // Join with Cars table
                    booking => booking.CarID, // Foreign key in Bookings
                    car => car.CarID, // Primary key in Cars
                    (booking, car) => new { booking, car } // Project into an anonymous type
                )
                .GroupBy(x => x.car.Marque) // Group by car marque
                .Select(g => new
                {
                    Marque = g.Key,
                    Revenue = g.Sum(x => x.booking.TotalPrice)
                })
                .OrderByDescending(g => g.Revenue)
                .Take(5) // Top 5 cars by revenue
                .ToList();

            ViewBag.CarsRevenueLabels = carsRevenueData.Select(c => c.Marque).ToList();
            ViewBag.CarsRevenueValues = carsRevenueData.Select(c => c.Revenue).ToList();

            // Fetch data for the Bookings by Car Marque Chart
            var bookingsByMarqueData = _context.Bookings
                .Join(
                    _context.Cars, // Join with Cars table
                    booking => booking.CarID, // Foreign key in Bookings
                    car => car.CarID, // Primary key in Cars
                    (booking, car) => new { booking, car } // Project into an anonymous type
                )
                .GroupBy(x => x.car.Marque) // Group by car marque
                .Select(g => new
                {
                    Marque = g.Key,
                    Count = g.Count()
                })
                .ToList();

            ViewBag.BookingsByMarqueLabels = bookingsByMarqueData.Select(b => b.Marque).ToList();
            ViewBag.BookingsByMarqueValues = bookingsByMarqueData.Select(b => b.Count).ToList();

            // Fetch data for the Monthly Revenue Chart
            var monthlyRevenueData = _context.Bookings
                .GroupBy(b => new { b.StartDate.Year, b.StartDate.Month }) // Group by year and month
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Revenue = g.Sum(b => b.TotalPrice)
                })
                .AsEnumerable() // Switch to client evaluation
                .Select(g => new
                {
                    Month = new DateTime(g.Year, g.Month, 1).ToString("MMM yyyy"), // Format on the client side
                    Revenue = g.Revenue
                })
                .OrderBy(g => g.Month)
                .Take(7) // Last 7 months
                .ToList();

            ViewBag.MonthlyRevenueLabels = monthlyRevenueData.Select(m => m.Month).ToList();
            ViewBag.MonthlyRevenueValues = monthlyRevenueData.Select(m => m.Revenue).ToList();

            // Fetch data for the Top 5 Clients by Revenue
            var topClientsData = _context.Bookings
                .GroupBy(b => b.UserName) // Group by username
                .Select(g => new
                {
                    UserName = g.Key,
                    TotalRevenue = g.Sum(b => b.TotalPrice)
                })
                .OrderByDescending(g => g.TotalRevenue)
                .Take(5) // Top 5 clients by revenue
                .ToList();

            ViewBag.TopClients = topClientsData;

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



    public class SystemAlert
    {
        public int AlertID { get; set; }
        public string Message { get; set; }
        public string Severity { get; set; }
        public DateTime Timestamp { get; set; }
    }







}