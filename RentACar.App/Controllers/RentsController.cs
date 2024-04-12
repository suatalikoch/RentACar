using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACar.App.Data;
using RentACar.App.Models.Rents;

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
                    CarId = rentFromDb.CarId,
                    RentStart = rentFromDb.RentStart,
                    RentEnd = rentFromDb.RentEnd,
                    Tenant = rentFromDb.Tenant
                }).ToList();

            return View(rents);
        }

        // GET: RentsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // GET: RentsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // POST: RentsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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

        // GET: RentsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: RentsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
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
    }
}
