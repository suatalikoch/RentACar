using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RentACar.App.Data;
using RentACar.App.Domain;
using RentACar.App.Models.Rents;

namespace RentACar.App.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class RentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public RentsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult All()
        {
            List<RentAllViewModel> rents = _context.Rents
                .Join(_context.Cars,
                    rent => rent.CarId,
                    car => car.Id,
                    (rent, car) => new { rent, car })
                .Join(_context.Users,
                    combined => combined.rent.TenantId,
                    user => user.Id,
                    (combined, user) => new RentAllViewModel
                {
                    Id = combined.rent.Id,
                    CarId = combined.rent.CarId,
                    CarBrand = combined.car.Brand,
                        CarModel = combined.car.Model,
                    StartDate = combined.rent.StartDate,
                    EndDate = combined.rent.EndDate,
                    TenantId = combined.rent.TenantId,
                    TenantFirstName = user.FirstName,
                    TenantLastName = user.LastName
                }).ToList();

            return View(rents);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RentCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Rent rent = new()
            {
                CarId = viewModel.CarId,
                TenantId = viewModel.TenantId,
                StartDate = viewModel.StartDate,
                EndDate = viewModel.EndDate
            };

            await _context.Rents.AddAsync(rent);
            await _context.SaveChangesAsync();

            return RedirectToAction("All");
        }

        public async Task<IActionResult> Edit(string id, RentEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.ErrorMessage);
                    }
                }

                return View(viewModel);
            }

            var rent = await _context.Rents.FindAsync(id);

            if (rent == null)
            {
                return NotFound();
            }

            rent.CarId = viewModel.CarId;
            rent.TenantId = viewModel.TenantId;
            rent.StartDate = viewModel.StartDate;
            rent.EndDate = viewModel.EndDate;

            _context.Rents.Update(rent);
            await _context.SaveChangesAsync();

            return RedirectToAction("All");
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var rent = await _context.Rents.FindAsync(id);

            if (rent == null)
            {
                return NotFound();
            }

            var car = await _context.Cars.FindAsync(rent.CarId);
            var tenant = await _userManager.FindByIdAsync(rent.TenantId);

            if (car == null || tenant == null)
            {
                return NotFound();
            }

            var viewModel = new RentDetailsViewModel
            {
                Id = rent.Id,
                CarId = rent.CarId,
                CarBrand = car.Brand,
                CarModel = car.Model,
                TenantId = rent.TenantId,
                TenantFirstName = tenant.FirstName,
                TenantLastName = tenant.LastName,
                TenantUserName = tenant.UserName,
                StartDate = rent.StartDate.ToString(),
                EndDate = rent.EndDate.ToString()
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var rent = await _context.Rents.FindAsync(id);

            if (rent == null)
            {
                return NotFound();
            }

            var car = await _context.Cars.FindAsync(rent.CarId);
            var tenant = await _userManager.FindByIdAsync(rent.TenantId);

            if (car == null || tenant == null)
            {
                return NotFound();
            }

            RentDeleteViewModel viewModel = new()
            {
                Id = rent.Id,
                CarModel = car.Model,
                CarBrand = car.Brand,
                TenantFirstName = tenant.FirstName,
                TenantLastName = tenant.LastName,
                StartDate = rent.StartDate.ToString(),
                EndDate = rent.EndDate.ToString()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirm(string id)
        {
            var rent = await _context.Rents.FindAsync(id);

            if (rent == null)
            {
                return NotFound();
            }

            _context.Rents.Remove(rent);
            await _context.SaveChangesAsync();

            return RedirectToAction("All");
        }
    }
}
