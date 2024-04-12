using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RentACar.App.Data;
using RentACar.App.Domain;
using RentACar.App.Models.Cars;
using System.Security.Claims;

namespace RentACar.App.Controllers
{
    [Authorize(Roles = "Administrator")]
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
                    Id = carFromDb.Id,
                    Brand = carFromDb.Brand,
                    Model = carFromDb.Model,
                    Year = carFromDb.Year.ToString(),
                    Passenger = carFromDb.Passenger.ToString(),
                    Description = carFromDb.Description,
                    RentPrice = carFromDb.RentPrice.ToString(),
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

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = _context.Cars.FirstOrDefault(c => c.Id == id);

            if (car == null)
            {
                return NotFound();
            }

            var model = new CarDetailsViewModel
            {
                Id = car.Id,
                Brand = car.Brand,
                Model = car.Model,
                Year = car.Year,
                Passenger = car.Passenger,
                Description = car.Description,
                RentPrice = car.RentPrice
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var car = _context.Cars.FirstOrDefault(c => c.Id == id);

            if (car == null)
            {
                return NotFound();
            }

            var result = _context.Cars.Remove(car);
            _context.SaveChanges();

            return RedirectToAction("All");
        }
    }
}
