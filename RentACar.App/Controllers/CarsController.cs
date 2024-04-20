using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            return View(new CarCreateEditViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CarCreateEditViewModel viewModel)
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

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var car = await _context.Cars.FindAsync(id);

            if (car == null)
            {
                return NotFound();
            }

            var viewModel = new CarCreateEditViewModel
            {
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, CarCreateEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var car = await _context.Cars.FindAsync(id);

                if (car == null)
                {
                    return NotFound();
                }

                // Update car properties
                car.Brand = viewModel.Brand;
                car.Model = viewModel.Model;
                car.Year = viewModel.Year;
                car.Passenger = viewModel.Passenger;
                car.ImageLink = viewModel.ImageLink;
                car.Description = viewModel.Description;
                car.RentPrice = viewModel.RentPrice;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    Console.WriteLine(e.Message);
                }

                return RedirectToAction(nameof(All));
            }

            return View(viewModel);
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

            return RedirectToAction(nameof(All));
        }
    }
}
