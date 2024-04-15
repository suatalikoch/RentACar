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

        public async Task <ActionResult> All()
        {
            List<RentAllViewModel> rents = _context.Rents
                .Select(rentFromDb => new RentAllViewModel
                {
                    Id = rentFromDb.Id,
                    CarId = rentFromDb.CarId,
                    //CarBrand = car.Brand,
                    //CarModel = car.Model,
                    StartDate = rentFromDb.StartDate,
                    EndDate = rentFromDb.EndDate,
                    TenantId = rentFromDb.TenantId,
                    //TenantFirstName = tenant.FirstName,
                    //TenantLastName = tenant.LastName
                }).ToList();

            return View(rents);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RentCreateBindingModel bindingModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            Rent rentFromDb = new()
            {
                CarId = bindingModel.CarId,
                TenantId = bindingModel.TenantId,
                StartDate = bindingModel.StartDate,
                EndDate = bindingModel.EndDate,
                Approved = bindingModel.Approved
            };

            _context.Rents.Add(rentFromDb);
            await _context.SaveChangesAsync();

            return RedirectToAction("All");
        }

        public async Task<IActionResult> Edit(string id, RentEditBindingModel bindingModel)
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

                return View(bindingModel);
            }

            var rent = await _context.Rents.FindAsync(id);

            if (rent == null)
            {
                return NotFound();
            }

            rent.CarId = bindingModel.CarId;
            rent.TenantId = bindingModel.TenantId;
            rent.StartDate = bindingModel.StartDate;
            rent.EndDate = bindingModel.EndDate;
            rent.Approved = bindingModel.Approved;

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

            var model = new RentDetailsViewModel
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
                EndDate = rent.EndDate.ToString(),
                Approved = rent.Approved.ToString()
            };

            return View(model);
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

            RentDeleteBindingModel bindingModel = new()
            {
                Id = rent.Id,
                CarModel = car.Model,
                CarBrand = car.Brand,
                TenantFirstName = tenant.FirstName,
                TenantLastName = tenant.LastName,
                StartDate = rent.StartDate.ToString(),
                EndDate = rent.EndDate.ToString()
            };

            return View(bindingModel);
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
