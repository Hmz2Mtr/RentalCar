using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RentalCar.Controllers
{
    [Authorize(Roles = "admin")] // Restrict access to users with the "Admin" role
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }

    }
}