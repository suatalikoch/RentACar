using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACar.App.Data;
using RentACar.App.Domain;
using RentACar.App.Models.Pending;

namespace RentACar.App.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class PendingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PendingController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult All()
        {
            /*List<PendingRentAllViewModel> pendingRents = _context.PendingRents
                .Join(_context.Cars,
                    pendingRent => pendingRent.CarId,
                    car => car.Id,
                    (pendingRent, car) => new { pendingRent, car })
                .Join(_context.Users,
                    combined => combined.pendingRent.TenantId,
                    user => user.Id,
                    (combined, user) => new PendingRentAllViewModel
                    {
                        Id = combined.pendingRent.Id,
                        CarId = combined.pendingRent.CarId,
                        CarBrand = combined.car.Brand,
                        CarModel = combined.car.Model,
                        StartDate = combined.pendingRent.StartDate,
                        EndDate = combined.pendingRent.EndDate,
                        TenantId = combined.pendingRent.TenantId,
                        TenantFirstName = user.FirstName,
                        TenantLastName = user.LastName
                    }).ToList();*/
            // Initialize a list to store the result
            List<PendingRentAllViewModel> pendingRents = new List<PendingRentAllViewModel>();

            // Loop through each pending rent
            foreach (var pendingRent in _context.PendingRents)
            {
                // Find the corresponding car
                var car = _context.Cars.FirstOrDefault(c => c.Id == pendingRent.CarId);
                if (car != null)
                {
                    // Find the corresponding user
                    var user = _context.Users.FirstOrDefault(u => u.Id == pendingRent.TenantId);
                    if (user != null)
                    {
                        // Create a new PendingRentAllViewModel and populate its properties
                        var viewModel = new PendingRentAllViewModel
                        {
                            Id = pendingRent.Id,
                            CarId = pendingRent.CarId,
                            CarBrand = car.Brand,
                            CarModel = car.Model,
                            StartDate = pendingRent.StartDate,
                            EndDate = pendingRent.EndDate,
                            TenantId = pendingRent.TenantId,
                            TenantFirstName = user.FirstName,
                            TenantLastName = user.LastName
                        };

                        // Add the view model to the result list
                        pendingRents.Add(viewModel);
                    }
                }
            }

            return View(pendingRents);
        }

        public async Task<IActionResult> Accept(string pendingRentId)
        {
            var pendingRent = await _context.PendingRents.FindAsync(pendingRentId);

            if (pendingRent == null)
            {
                return NotFound();
            }

            Rent rent = new()
            {
                Id = pendingRent.Id,
                CarId = pendingRent.CarId,
                TenantId = pendingRent.TenantId,
                StartDate = pendingRent.StartDate,
                EndDate = pendingRent.EndDate
            };

            _context.PendingRents.Remove(pendingRent);
            await _context.Rents.AddAsync(rent);
            await _context.SaveChangesAsync();

            return RedirectToAction("All");
        }

        public async Task<IActionResult> Decline(string pendingRentId)
        {
            var pendingRent = await _context.PendingRents.FindAsync(pendingRentId);

            if (pendingRent == null)
            {
                return NotFound();
            }

            var car = await _context.Cars.FindAsync(pendingRent.CarId);
            var tenant = await _context.Users.FindAsync(pendingRent.TenantId);

            if (car == null || tenant == null)
            {
                return NotFound();
            }

            PendingDeleteViewModel viewModel = new()
            {
                Id = pendingRent.Id,
                CarModel = car.Model,
                CarBrand = car.Brand,
                TenantFirstName = tenant.FirstName,
                TenantLastName = tenant.LastName,
                StartDate = pendingRent.StartDate.ToString(),
                EndDate = pendingRent.EndDate.ToString()
            };

            return View(viewModel);
        }

        public async Task<IActionResult> DeclineConfirm(string pendingRentId)
        {
            var pendingRent = await _context.PendingRents.FindAsync(pendingRentId);

            if (pendingRent == null)
            {
                return NotFound();
            }

            _context.PendingRents.Remove(pendingRent);
            await _context.SaveChangesAsync();

            return RedirectToAction("All");
        }
    }
}
