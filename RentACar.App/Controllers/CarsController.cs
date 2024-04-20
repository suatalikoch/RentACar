using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACar.App.Data;
using RentACar.App.Domain;
using RentACar.App.Models.Cars;

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
            List<CarAllViewModel> cars = _context.Cars.Select(car => new CarAllViewModel
            {
                Id = car.Id,
                Brand = car.Brand,
                Model = car.Model,
                Year = car.Year.ToString(),
                Passenger = car.Passenger.ToString(),
                Description = car.Description,
                RentPrice = car.RentPrice.ToString(),
            }).ToList();

            return View(cars);
        }

        public IActionResult Create()
        {
            return View(new CarCreateViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CarCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Car car = new()
            {
                Brand = viewModel.Brand,
                Model = viewModel.Model,
                Year = viewModel.Year,
                Passenger = viewModel.Passenger,
                ImageLink = viewModel.ImageLink,
                Description = viewModel.Description,
                RentPrice = viewModel.RentPrice
            };

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            return RedirectToAction("All");
        }

        public async Task<IActionResult> Edit(string id, CarEditViewModel viewModel)
        {
            var car = await _context.Cars.FindAsync(id);

            if (car == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                viewModel.Brand = car.Brand;
                viewModel.Model = car.Model;
                viewModel.Year = car.Year;
                viewModel.Passenger = car.Passenger;
                viewModel.ImageLink = car.ImageLink;
                viewModel.Description = car.Description;
                viewModel.RentPrice = car.RentPrice;

                return View(viewModel);
            }

            car.Brand = viewModel.Brand;
            car.Model = viewModel.Model;
            car.Year = viewModel.Year;
            car.Passenger = viewModel.Passenger;
            car.ImageLink = viewModel.ImageLink;
            car.Description = viewModel.Description;
            car.RentPrice = viewModel.RentPrice;

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

            var viewModel = new CarDetailsViewModel
            {
                Id = car.Id,
                Brand = car.Brand,
                Model = car.Model,
                Year = car.Year,
                Passenger = car.Passenger,
                ImageLink = car.ImageLink,
                Description = car.Description,
                RentPrice = car.RentPrice
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var car = await _context.Cars.FindAsync(id);

            if (car == null)
            {
                return NotFound();
            }

            CarDeleteViewModel viewModel = new()
            {
                Id = car.Id,
                Brand = car.Brand,
                Model = car.Model
            };

            return View(viewModel);
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
