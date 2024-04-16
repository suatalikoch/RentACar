using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACar.App.Data;
using RentACar.App.Domain;
using RentACar.App.Models.Pending;
using RentACar.App.Models.Rents;

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
                .Select(pendingRentFromDb => new PendingRentAllViewModel
                {
                    Id = pendingRentFromDb.Id,
                    CarId = pendingRentFromDb.CarId,
                    //CarBrand = car.Brand,
                    //CarModel = car.Model,
                    StartDate = pendingRentFromDb.StartDate,
                    EndDate = pendingRentFromDb.EndDate,
                    TenantId = pendingRentFromDb.TenantId,
                    //TenantFirstName = tenant.FirstName,
                    //TenantLastName = tenant.LastName
                }).ToList();

            return View(pendingRents);
        }

        /*
        public async Task<IActionResult> Accept(string pendingRentId)
        {
            var pendingRent = await _context.PendingRents.FindAsync(pendingRentId);

            if (pendingRent == null)
            {
                return NotFound();
            }

            _context.PendingRents.Remove(pendingRent);
            _context.Rents.Add(pendingRent);
            await _context.SaveChangesAsync();

            return View();
        }
        */

        public async Task<IActionResult> Delete(string pendingRentId)
        {
            var pendingRent = await _context.PendingRents.FindAsync(pendingRentId);

            if (pendingRent == null)
            {
                return NotFound();
            }

            PendingDeleteBindingModel bindingModel = new()
            {

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

            return View("All");
        }
    }
}
