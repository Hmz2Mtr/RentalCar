using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
