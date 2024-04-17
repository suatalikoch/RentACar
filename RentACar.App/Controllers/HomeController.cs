using Microsoft.AspNetCore.Mvc;
using RentACar.App.Data;
using RentACar.App.Domain;
using RentACar.App.Models;
using RentACar.App.Models.Home;
using System.Diagnostics;
using System.Security.Claims;

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

        public IActionResult Search(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
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
                AvailableCars = allCars,
                StartDate = startDate,
                EndDate = endDate
            };

            return View("Index", viewModel);
        }

        public async Task<IActionResult> Rent(string carId, DateTime startDate, DateTime endDate)
        {
            var car = await _context.Cars.FindAsync(carId);

            if (car == null)
            {
                return NotFound();
            }

            TimeSpan rentDuration = endDate - startDate;

            RentConfirmViewModel viewModel = new()
            {
                CarId = car.Id,
                TenantId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Brand = car.Brand,
                Model = car.Model,
                Year = car.Year.ToString(),
                Passenger = car.Passenger.ToString(),
                Description = car.Description,
                RentPrice = car.RentPrice.ToString(),
                StartDate = startDate.ToString(),
                EndDate = endDate.ToString()
            };

            decimal rentTotal = decimal.Parse(viewModel.RentPrice) * rentDuration.Days;

            viewModel.RentDuration = string.Concat(rentDuration.Days + " Days ", rentDuration.Hours + " Hours ", rentDuration.Minutes + " Minutes ");
            viewModel.RentTotal = rentTotal.ToString();

            return View("RentConfirm", viewModel);
        }

        public async Task<IActionResult> RentConfirm(string carId, string tenantId, DateTime startDate, DateTime endDate)
        {
            PendingRent pendingRent = new()
            {
                CarId = carId,
                TenantId = tenantId,
                StartDate = startDate,
                EndDate = endDate
            };

            await _context.PendingRents.AddAsync(pendingRent);
            await _context.SaveChangesAsync();

            return View("Index");
        }
    }
}
