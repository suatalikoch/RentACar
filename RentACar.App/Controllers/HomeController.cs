using Microsoft.AspNetCore.Mvc;
using RentACar.App.Data;
using RentACar.App.Domain;
using RentACar.App.Models;
using RentACar.App.Models.Home;
using System.Diagnostics;

namespace RentACar.App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Search(DateTime startDate, DateTime endDate)
        {
            if (startDate >= endDate)
            {
                ModelState.AddModelError(string.Empty, "End date must be after start date.");

                return View("Index");
            }

            ViewBag.SearchButtonClicked = Request.Query["searchButton"] == "true";

            var unavailableRents = _context.Rents
                .Where(r => !(r.EndDate < startDate || r.StartDate > endDate))
                .ToList();

            List<Car> allCars = _context.Cars.ToList();

            foreach (var rent in unavailableRents)
            {
                allCars.RemoveAll(car => car.Id == rent.CarId);
            }

            var viewModel = new AvailableCarsViewModel
            {
                AvailableCars = allCars
            };

            return View("Index", viewModel);
        }
    }
}
