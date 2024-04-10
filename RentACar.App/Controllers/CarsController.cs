using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACar.App.Data;
using RentACar.App.Domain;
using RentACar.App.Models;
using System.Security.Claims;

namespace RentACar.App.Controllers
{
    [Authorize]
    public class CarsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult All()
        {
            List<CarAllViewModel> cars = _context.Cars
                .Select(carFromDb => new CarAllViewModel
                {
                    Brand = carFromDb.Brand,
                    Model = carFromDb.Model,
                    Year = carFromDb.Year.ToString(),
                    Passenger = carFromDb.Passenger.ToString(),
                    Description = carFromDb.Description,
                    RentPrice = carFromDb.Year.ToString(),
                    Tenant = carFromDb.Tenant.UserName
                }).ToList();

            return View(cars);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CarCreateBindingModel bindingModel)
        {
            if (ModelState.IsValid)
            {
                string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Car carFromDb = new Car
                {
                    Brand = bindingModel.Brand,
                    Model = bindingModel.Model,
                    Year = bindingModel.Year,
                    Passenger = bindingModel.Passenger,
                    Description = bindingModel.Description,
                    RentPrice = bindingModel.RentPrice,
                    TenantId = currentUserId
                };

                _context.Cars.Add(carFromDb);
                _context.SaveChanges();

                return RedirectToAction("All");
            }

            return View();
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
