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
        private readonly UserManager<User> _userManager;

        public CarsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
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
                    Renter = carFromDb.Renter
                }).ToList();

            return View(cars);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CarCreateBindingModel bindingModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.FindByIdAsync(currentUserId);

            Car carFromDb = new()
            {
                Brand = bindingModel.Brand,
                Model = bindingModel.Model,
                Year = bindingModel.Year,
                Passenger = bindingModel.Passenger,
                Description = bindingModel.Description,
                RentPrice = bindingModel.RentPrice,
                Renter = currentUser.UserName,
                RenterId = currentUserId
            };

            _context.Cars.Add(carFromDb);
            await _context.SaveChangesAsync();

            return RedirectToAction("All");
        }

        public async Task<IActionResult> Edit(string id, CarEditBindingModel bindingModel)
        {
            var car = await _context.Cars.FindAsync(id);

            if (car == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                bindingModel.Brand = car.Brand;
                bindingModel.Model = car.Model;
                bindingModel.Year = car.Year;
                bindingModel.Passenger = car.Passenger;
                bindingModel.Description = car.Description;
                bindingModel.RentPrice = car.RentPrice;

                return View(bindingModel);
            }

            car.Brand = bindingModel.Brand;
            car.Model = bindingModel.Model;
            car.Year = bindingModel.Year;
            car.Passenger = bindingModel.Passenger;
            car.Description = bindingModel.Description;
            car.RentPrice = bindingModel.RentPrice;

            _context.Cars.Update(car);
            await _context.SaveChangesAsync();

            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.ErrorMessage);
                }
            }

            return RedirectToAction("All");
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var car = await _context.Cars.FindAsync(id);

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
                RentPrice = car.RentPrice,
                Renter = car.Renter,
                RenterId = car.RenterId
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var car = await _context.Cars.FindAsync(id);

            if (car == null)
            {
                return NotFound();
            }

            CarDeleteBindingModel bindingModel = new()
            {
                Id = car.Id,
                Brand = car.Brand,
                Model = car.Model
            };

            return View(bindingModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirm(string id)
        {
            var car = await _context.Cars.FindAsync(id);

            if (car == null)
            {
                return NotFound();
            }

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            return RedirectToAction("All");
        }
    }
}
