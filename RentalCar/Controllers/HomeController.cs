using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalCar.Data;
using RentalCar.Models;
using System.Diagnostics;

namespace RentalCar.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public readonly DatabaseContext _db;
        
        public HomeController(ILogger<HomeController> logger, DatabaseContext db)
        {
            _logger = logger;
            _db = db;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }




        public IActionResult Cars()
        {
            var ListeCarObjet = _db.Cars.ToList();
            return View(ListeCarObjet);
        }
        public IActionResult Notfound()
        {
            return View();
        }
        public IActionResult Blog()
        {
            return View();
        }
        public IActionResult Services()
        {
            return View();
        }
        public IActionResult Features()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult Team()
        {
            return View();
        }
        
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}