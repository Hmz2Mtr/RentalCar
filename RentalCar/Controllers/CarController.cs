using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalCar.Data;
using RentalCar.Models;

namespace RentalCar.Controllers
{
    [Authorize(Roles = "admin")]
    public class CarController : Controller
    {
        public readonly DatabaseContext _db;
        public CarController(DatabaseContext db)
        {
            _db = db;
        }



        public IActionResult Index()
        {
            var ListeCarObjet = _db.Cars.ToList();
            return View(ListeCarObjet);
        }


        [HttpGet]
        public IActionResult Creation()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Creation(Car obj)
        {
            if (ModelState.IsValid)
            {
                _db.Cars.Add(obj);
                _db.SaveChanges();
                //TempData["succes"] = "L'opération de création est Réussie";
                return RedirectToAction("Index", "Car");
            }
            //TempData["echec"] = "L'opération de création a Echoué";
            return View();
        }






        [HttpGet]
        public IActionResult Modifier(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Car? CarDb = _db.Cars.Find(id);
            if (CarDb == null)
            {
                return NotFound();
            }
            return View(CarDb);
        }



        [HttpPost]
        public IActionResult Modifier(Car obj)
        {
            if (ModelState.IsValid)
            {
                _db.Cars.Update(obj);
                _db.SaveChanges();
                //TempData["succes"] = "L'opération de modification est Réussie";
                return RedirectToAction("Index", "Car");
            }
            //TempData["echec"] = "L'opération de modification a Echoué";
            return View();
        }





        [HttpGet]
        public IActionResult Supprimer(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Car? CarDb = _db.Cars.Find(id);
            if (CarDb == null)
            {
                return NotFound();
            }
            return View(CarDb);
        }


        [HttpPost, ActionName("Supprimer")]
        public IActionResult SupprimerPost(int? id)
        {
            Car? obj = _db.Cars.Find(id);
            if (obj == null)
            {
                //TempData["echec"] = "L'opération de suppression a Echoué";
                return NotFound();
            }
            _db.Cars.Remove(obj);
            _db.SaveChanges();
            //TempData["succes"] = "L'opération de suppression est Réussie";
            return RedirectToAction("Index", "Car");
        }
    }
}
