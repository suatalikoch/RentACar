using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RentACar.App.Data;
using RentACar.App.Domain;
using RentACar.App.Models.Cars;
using RentACar.App.Models.Rents;
using System.Security.Claims;

namespace RentACar.App.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class RentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: RentsController
        public ActionResult All()
        {
            List<RentAllViewModel> rents = _context.Rents
                .Select(rentFromDb => new RentAllViewModel
                {
                    Id = rentFromDb.Id,
                    CarId = rentFromDb.CarId,
                    StartDate = rentFromDb.StartDate,
                    EndDate = rentFromDb.EndDate,
                    TenantId = rentFromDb.TenantId
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

            Rent rentFromDb = new Rent
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

        // GET: RentsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: RentsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(All));
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var rent = await _context.Rents.FindAsync(id);

            if (rent == null)
            {
                return NotFound();
            }

            var model = new RentDetailsViewModel
            {
                Id = rent.Id,
                CarId = rent.CarId,
                TenantId = rent.TenantId,
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

            RentDeleteBindingModel bindingModel = new RentDeleteBindingModel()
            {
                Id = rent.Id,
                CarId = rent.CarId,
                TenantId = rent.TenantId
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
