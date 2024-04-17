using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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

            return View(pendingRents);
        }


        public async Task<IActionResult> Accept(string pendingRentId)
        {
            var pendingRent = await _context.PendingRents.FindAsync(pendingRentId);

            if (pendingRent == null)
            {
                return NotFound();
            }
            Rent rent = new Rent()
            {
                Id = pendingRent.Id,
                CarId = pendingRent.CarId,
                TenantId = pendingRent.TenantId,
                StartDate = pendingRent.StartDate,
                EndDate = pendingRent.EndDate
            };

            _context.Rents.Add(rent);
            _context.PendingRents.Remove(pendingRent);
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


            PendingDeleteBindingModel bindingModel = new()
            {
                Id = pendingRent.Id,
                CarModel = car.Model,
                CarBrand = car.Brand,

                TenantFirstName = tenant.FirstName,
                TenantLastName = tenant.LastName,
                StartDate = pendingRent.StartDate.ToString(),
                EndDate = pendingRent.EndDate.ToString()
            };

            return View(bindingModel);
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
