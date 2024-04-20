using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACar.App.Data;
using RentACar.App.Domain;
using RentACar.App.Models.Pending;
using System.Collections.Generic;
using System.Linq; 
using System.Threading.Tasks; 

namespace RentACar.App.Controllers
{
    [Authorize(Roles = "Administrator")] // Ensures only users with "Administrator" role can access these actions
    public class PendingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PendingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Action to display all pending rents
        public IActionResult All()
        {
            // Fetch pending rents and join with cars and users to create a list of view models
            List<PendingRentAllViewModel> pendingRents = _context.PendingRents
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
                    }).ToList();

            return View(pendingRents); // Return view with pending rents
        }

        // Action to accept a pending rent
        public async Task<IActionResult> Accept(string pendingRentId)
        {
            var pendingRent = await _context.PendingRents.FindAsync(pendingRentId); // Find pending rent by id

            if (pendingRent == null)
            {
                return NotFound(); // If not found, return 404
            }

            // Create rent object from pending rent
            Rent rent = new()
            {
                Id = pendingRent.Id,
                CarId = pendingRent.CarId,
                TenantId = pendingRent.TenantId,
                StartDate = pendingRent.StartDate,
                EndDate = pendingRent.EndDate
            };

            _context.PendingRents.Remove(pendingRent); // Remove pending rent
            await _context.Rents.AddAsync(rent); // Add rent
            await _context.SaveChangesAsync(); // Save changes to database

            return RedirectToAction("All"); // Redirect to All action
        }

        // Action to decline a pending rent
        public async Task<IActionResult> Decline(string pendingRentId)
        {
            var pendingRent = await _context.PendingRents.FindAsync(pendingRentId); // Find pending rent by id

            if (pendingRent == null)
            {
                return NotFound(); // If not found, return 404
            }

            var car = await _context.Cars.FindAsync(pendingRent.CarId); // Find car related to pending rent
            var tenant = await _context.Users.FindAsync(pendingRent.TenantId); // Find tenant related to pending rent

            if (car == null || tenant == null)
            {
                return NotFound(); // If car or tenant not found, return 404
            }

            // Create view model for confirmation
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

            return View(viewModel); // Return view with confirmation details
        }

        // Action to confirm declining a pending rent
        public async Task<IActionResult> DeclineConfirm(string pendingRentId)
        {
            var pendingRent = await _context.PendingRents.FindAsync(pendingRentId); // Find pending rent by id

            if (pendingRent == null)
            {
                return NotFound(); // If not found, return 404
            }

            _context.PendingRents.Remove(pendingRent); // Remove pending rent
            await _context.SaveChangesAsync(); // Save changes to database

            return RedirectToAction("All"); // Redirect to All action
        }
    }
}
