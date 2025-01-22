using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RentalCar.Models.Domain;
using RentalCar.Models;
using Microsoft.EntityFrameworkCore;
using RentalCar.Data;

namespace RentalCar.Controllers
{
    [Authorize] // Only authenticated users can access this controller
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DatabaseContext _context;

        public ProfileController(UserManager<ApplicationUser> userManager, DatabaseContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: /Profile/Index
        public async Task<IActionResult> Index()
        {
            // Get the current logged-in user
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            // Map the user data to a view model
            var model = new ProfileViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber
            };

            return View(model);
        }

        // GET: /Profile/Edit
        public async Task<IActionResult> Edit()
        {
            // Get the current logged-in user
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            // Map the user data to a view model
            var model = new ProfileViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber
            };

            return View(model);
        }

        // POST: /Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Get the current logged-in user
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return NotFound();
                }

                // Update the user's details
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.PhoneNumber = model.PhoneNumber;

                // Save the changes
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    TempData["Message"] = "Profile updated successfully!";
                    return RedirectToAction("Index");
                }

                // If there are errors, add them to the ModelState
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If the model is invalid, return to the edit view with errors
            return View(model);
        }








        // GET: /Booking/Index
        public async Task<IActionResult> MyBookedCars()
        {
            // Get the currently logged-in user
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            // Filter bookings by the current user's ID
            var bookings = await _context.Bookings
                .Where(b => b.UserID == user.Id) // Filter by UserID
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
            return RedirectToAction("MyBookedCars", "Profile");
        }


    }
}

