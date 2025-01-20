using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RentalCar.Models.Domain;

namespace RentalCar.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        
        public IActionResult BookedCars()
        {
            return View();
        }
    }
}
