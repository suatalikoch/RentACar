using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RentACar.App.Data;
using RentACar.App.Domain;
using RentACar.App.Models.Rents;

namespace RentACar.App.Controllers
{
    // Controller for managing rents, accessible only to users with the role "Administrator"
    [Authorize(Roles = "Administrator")]
    public class RentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        // Constructor to inject ApplicationDbContext and UserManager dependencies
        public RentsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Action method to display all rents
        public IActionResult All()
        {
            // Retrieve all rents along with associated car and tenant information
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

        // Action method to display form for creating a new rent
        public IActionResult Create()
        {
            return View();
        }

        // HTTP POST action method for creating a new rent
        [HttpPost]
        public async Task<IActionResult> Create(RentCreateEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                // If model state is not valid, return the same view with validation errors
                return View();
            }

            // Create a new rent object from the view model
            Rent rent = new()
            {
                CarId = viewModel.CarId,
                TenantId = viewModel.TenantId,
                StartDate = viewModel.StartDate,
                EndDate = viewModel.EndDate
            };

            // Add the rent to the database and save changes
            await _context.Rents.AddAsync(rent);
            await _context.SaveChangesAsync();

            // Redirect to the action method that displays all rents
            return RedirectToAction(nameof(All));
        }

        public async Task<IActionResult> Edit(string id, RentCreateEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                // If model state is not valid, add validation errors to model state and return the view
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.ErrorMessage);
                    }
                }

                return View(viewModel);
            }

            // Find the rent by id
            var rent = await _context.Rents.FindAsync(id);

            if (rent == null)
            {
                // If rent is not found, return not found error
                return NotFound();
            }

            // Update rent properties with values from the view model
            rent.CarId = viewModel.CarId;
            rent.TenantId = viewModel.TenantId;
            rent.StartDate = viewModel.StartDate;
            rent.EndDate = viewModel.EndDate;

            // Update the rent in the database and save changes
            _context.Rents.Update(rent);
            await _context.SaveChangesAsync();

            // Redirect to the action method that displays all rents
            return RedirectToAction("All");
        }

        // Action method to display details of a rent
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            // Find the rent by id
            var rent = await _context.Rents.FindAsync(id);

            if (rent == null)
            {
                // If rent is not found, return not found error
                return NotFound();
            }

            // Find the associated car and tenant
            var car = await _context.Cars.FindAsync(rent.CarId);
            var tenant = await _userManager.FindByIdAsync(rent.TenantId);

            if (car == null || tenant == null)
            {
                // If associated car or tenant is not found, return not found error
                return NotFound();
            }

            // Create view model with rent details
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

            // Return view with rent details
            return View(viewModel);
        }

        // Action method to display form for confirming deletion of a rent
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            // Find the rent by id
            var rent = await _context.Rents.FindAsync(id);

            if (rent == null)
            {
                // If rent is not found, return not found error
                return NotFound();
            }

            // Find the associated car and tenant
            var car = await _context.Cars.FindAsync(rent.CarId);
            var tenant = await _userManager.FindByIdAsync(rent.TenantId);

            if (car == null || tenant == null)
            {
                // If associated car or tenant is not found, return not found error
                return NotFound();
            }

            // Create view model for delete confirmation
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

            // Return view with delete confirmation
            return View(viewModel);
        }

        // HTTP POST action method for confirming deletion of a rent
        [HttpPost]
        public async Task<IActionResult> DeleteConfirm(string id)
        {
            // Find the rent by id
            var rent = await _context.Rents.FindAsync(id);

            if (rent == null)
            {
                // If rent is not found, return not found error
                return NotFound();
            }

            // Remove the rent from the database and save changes
            _context.Rents.Remove(rent);
            await _context.SaveChangesAsync();

            // Redirect to the action method that displays all rents
            return RedirectToAction(nameof(All));
        }
    }
}
